using System;
using System.Collections.Generic;
using System.Threading;

using Server.Network;

namespace Server
{
	public delegate void FileCommitCallback(FileQueue.Chunk chunk);

	public sealed class FileQueue : IDisposable
	{
		private static readonly int bufferSize;
		private static readonly BufferPool bufferPool;
		
		private readonly object syncRoot;
		private readonly Chunk[] active;
		private readonly Queue<Page> pending;
		private Page buffered;
		private readonly FileCommitCallback callback;
		private int activeCount;
		private ManualResetEvent idle;
		private long position;
		
		public long Position => position;

		public FileQueue(int concurrentWrites, FileCommitCallback callback)
		{
			if (concurrentWrites < 1)
			{
				throw new ArgumentOutOfRangeException("concurrentWrites");
			}
			else if (bufferSize < 1)
			{
				throw new ArgumentOutOfRangeException("bufferSize");
			}
			else if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}

			syncRoot = new object();

			active = new Chunk[concurrentWrites];
			pending = new Queue<Page>();

			this.callback = callback;

			idle = new ManualResetEvent(true);
		}

		static FileQueue()
		{
			bufferSize = FileOperations.BufferSize;
			bufferPool = new BufferPool("File Buffers", 64, bufferSize);
		}

		public void Dispose()
		{
			if (idle != null)
			{
				idle.Close();
				idle = null;
			}
		}

		public void Flush()
		{
			if (buffered.buffer != null)
			{
				Append(buffered);

				buffered.buffer = null;
				buffered.length = 0;
			}

			idle.WaitOne();
		}

		public void Enqueue(byte[] buffer, int offset, int size)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			else if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			else if (size < 0)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			else if ((buffer.Length - offset) < size)
			{
				throw new ArgumentException();
			}

			position += size;

			while (size > 0)
			{
				if (buffered.buffer == null) // nothing yet buffered
				{
					buffered.buffer = bufferPool.AcquireBuffer();
				}

				var page = buffered.buffer; // buffer page
				var pageSpace = page.Length - buffered.length; // available bytes in page
				var byteCount = size > pageSpace ? pageSpace : size; // how many bytes we can copy over

				Buffer.BlockCopy(buffer, offset, page, buffered.length, byteCount);

				buffered.length += byteCount;
				offset += byteCount;
				size -= byteCount;

				if (buffered.length == page.Length) // page full
				{
					Append(buffered);

					buffered.buffer = null;
					buffered.length = 0;
				}
			}
		}

		private void Append(Page page)
		{
			lock (syncRoot)
			{
				if (activeCount == 0)
				{
					idle.Reset();
				}

				++activeCount;

				for (var slot = 0; slot < active.Length; ++slot)
				{
					if (active[slot] == null)
					{
						active[slot] = new Chunk(this, slot, 0, page.length, page.buffer);

						callback(active[slot]);

						return;
					}
				}

				pending.Enqueue(page);
			}
		}

		private void Commit(Chunk chunk, int slot)
		{
			if (slot < 0 || slot >= active.Length)
			{
				throw new ArgumentOutOfRangeException("slot");
			}

			lock (syncRoot)
			{
				if (active[slot] != chunk)
				{
					throw new ArgumentException();
				}

				bufferPool.ReleaseBuffer(ref chunk.Buffer);

				if (pending.Count > 0)
				{
					var page = pending.Dequeue();

					active[slot] = new Chunk(this, slot, 0, page.length, page.buffer);

					callback(active[slot]);
				}
				else
				{
					active[slot] = null;
				}

				--activeCount;

				if (activeCount == 0)
				{
					idle.Set();
				}
			}
		}

		private struct Page
		{
			public byte[] buffer;
			public int length;
		}

		public sealed class Chunk
		{
			private readonly FileQueue m_Owner;
			private readonly int m_Slot;

			public int Offset, Size;

			public byte[] Buffer;

			public Chunk(FileQueue owner, int slot, int offset, int size, byte[] buffer)
			{
				m_Owner = owner;
				m_Slot = slot;

				Offset = offset;
				Size = size;

				Buffer = buffer;
			}

			public void Commit()
			{
				m_Owner.Commit(this, m_Slot);
			}
		}
	}
}