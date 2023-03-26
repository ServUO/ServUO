#region References
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
#endregion

namespace Server.Network
{
	public class SendQueue
	{
		public class Gram
		{
			private static readonly ConcurrentStack<Gram> m_Pool = new ConcurrentStack<Gram>();

			public static Gram Acquire()
			{
				if (!m_Pool.TryPop(out var gram))
				{
					gram = new Gram();
				}

				gram.Buffer = AcquireBuffer();
				gram.Length = 0;

				return gram;
			}

			public byte[] Buffer { get; private set; }

			public int Length { get; private set; }

			public int Available => Buffer.Length - Length;

			public bool IsFull => Length == Buffer.Length;

			private Gram()
			{ }

			public int Write(byte[] buffer, int offset, int length)
			{
				var write = Math.Min(length, Available);

				System.Buffer.BlockCopy(buffer, offset, Buffer, Length, write);

				Length += write;

				return write;
			}

			public void Release()
			{
				m_Pool.Push(this);

				ReleaseBuffer(Buffer);
			}
		}

		private static int m_CoalesceBufferSize = 512;

		private static BufferPool m_UnusedBuffers = new BufferPool("Coalesced", 2048, m_CoalesceBufferSize);

		public static int CoalesceBufferSize
		{
			get => m_CoalesceBufferSize;
			set
			{
				if (m_CoalesceBufferSize == value)
				{
					return;
				}

				var unused = new BufferPool("Coalesced", 2048, value);

				var old = Interlocked.Exchange(ref m_UnusedBuffers, unused);

				old?.Free();

				m_CoalesceBufferSize = value;
			}
		}

		public static byte[] AcquireBuffer()
		{
			return m_UnusedBuffers.AcquireBuffer();
		}

		public static void ReleaseBuffer(byte[] buffer)
		{
			if (buffer != null && buffer.Length == m_CoalesceBufferSize)
			{
				m_UnusedBuffers.ReleaseBuffer(ref buffer);
			}
		}

		private readonly ConcurrentQueue<Gram> m_Pending;

		private Gram m_Buffered;

		public bool IsFlushReady => m_Pending.IsEmpty && m_Buffered != null;

		public bool IsEmpty => m_Pending.IsEmpty && m_Buffered == null;

		public SendQueue()
		{
			m_Pending = new ConcurrentQueue<Gram>();
		}

		public Gram CheckFlushReady()
		{
			var gram = Interlocked.Exchange(ref m_Buffered, null);

			m_Pending.Enqueue(gram);

			return gram;
		}

		public Gram Dequeue()
		{
			if (!m_Pending.TryDequeue(out var gram))
			{
				return null;
			}

			gram?.Release();

			if (!m_Pending.TryPeek(out gram))
			{
				return null;
			}

			return gram;
		}

		private const int PendingCap = 0x200000;

		public Gram Enqueue(byte[] buffer, int length)
		{
			return Enqueue(buffer, 0, length);
		}

		public Gram Enqueue(byte[] buffer, int offset, int length)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}

			if (offset < 0 || offset >= buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset", offset, "Offset must be greater than or equal to zero and less than the size of the buffer.");
			}

			if (length < 0 || length > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("length", length, "Length cannot be less than zero or greater than the size of the buffer.");
			}

			if (buffer.Length - offset < length)
			{
				throw new ArgumentException("Offset and length do not point to a valid segment within the buffer.");
			}

			var existingBytes = (m_Pending.Count * m_CoalesceBufferSize) + m_Buffered?.Length;

			if (existingBytes + length > PendingCap)
			{
				throw new CapacityExceededException();
			}

			Gram gram = null;

			while (length > 0)
			{
				if (m_Buffered == null)
				{
					// nothing yet buffered
					m_Buffered = Gram.Acquire();
				}

				var bytesWritten = m_Buffered.Write(buffer, offset, length);

				offset += bytesWritten;
				length -= bytesWritten;

				if (m_Buffered.IsFull)
				{
					if (m_Pending.IsEmpty)
					{
						gram = m_Buffered;
					}

					m_Pending.Enqueue(m_Buffered);

					m_Buffered = null;
				}
			}

			return gram;
		}

		public void Clear()
		{
			m_Buffered?.Release();
			m_Buffered = null;

			while (m_Pending.TryDequeue(out var gram))
			{
				gram?.Release();
			}
		}
	}

	[Serializable]
	public sealed class CapacityExceededException : Exception
	{
		public CapacityExceededException()
			: base("Too much data pending.")
		{ }
	}
}
