#region Header
// **********
// ServUO - SendQueue.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;
#endregion

namespace Server.Network
{
	public class SendQueue
	{
		public class Gram
		{
			private static readonly Stack<Gram> _pool = new Stack<Gram>();

			public static Gram Acquire()
			{
				lock (_pool)
				{
					Gram gram;

					if (_pool.Count > 0)
					{
						gram = _pool.Pop();
					}
					else
					{
						gram = new Gram();
					}

					gram._buffer = AcquireBuffer();
					gram._length = 0;

					return gram;
				}
			}

			private byte[] _buffer;
			private int _length;

			public byte[] Buffer { get { return _buffer; } }

			public int Length { get { return _length; } }

			public int Available { get { return (_buffer.Length - _length); } }

			public bool IsFull { get { return (_length == _buffer.Length); } }

			private Gram()
			{ }

			public int Write(byte[] buffer, int offset, int length)
			{
				int write = Math.Min(length, Available);

				System.Buffer.BlockCopy(buffer, offset, _buffer, _length, write);

				_length += write;

				return write;
			}

			public void Release()
			{
				lock (_pool)
				{
					_pool.Push(this);
					ReleaseBuffer(_buffer);
				}
			}
		}

		private static int m_CoalesceBufferSize = 512;
		private static BufferPool m_UnusedBuffers = new BufferPool("Coalesced", 2048, m_CoalesceBufferSize);

		public static int CoalesceBufferSize
		{
			get { return m_CoalesceBufferSize; }
			set
			{
				if (m_CoalesceBufferSize == value)
				{
					return;
				}

				BufferPool old = m_UnusedBuffers;

				lock (old)
				{
					if (m_UnusedBuffers != null)
					{
						m_UnusedBuffers.Free();
					}

					m_CoalesceBufferSize = value;
					m_UnusedBuffers = new BufferPool("Coalesced", 2048, m_CoalesceBufferSize);
				}
			}
		}

		public static byte[] AcquireBuffer()
		{
			lock (m_UnusedBuffers)
				return m_UnusedBuffers.AcquireBuffer();
		}

		public static void ReleaseBuffer(byte[] buffer)
		{
			lock (m_UnusedBuffers)
				if (buffer != null && buffer.Length == m_CoalesceBufferSize)
				{
					m_UnusedBuffers.ReleaseBuffer(buffer);
				}
		}

		private readonly Queue<Gram> _pending;

		private Gram _buffered;

		public bool IsFlushReady { get { return (_pending.Count == 0 && _buffered != null); } }

		public bool IsEmpty { get { return (_pending.Count == 0 && _buffered == null); } }

		public SendQueue()
		{
			_pending = new Queue<Gram>();
		}

		public Gram CheckFlushReady()
		{
			Gram gram = _buffered;
			_pending.Enqueue(_buffered);
			_buffered = null;
			return gram;
		}

		public Gram Dequeue()
		{
			Gram gram = null;

			if (_pending.Count > 0)
			{
				_pending.Dequeue().Release();

				if (_pending.Count > 0)
				{
					gram = _pending.Peek();
				}
			}

			return gram;
		}

		private const int PendingCap = 256 * 1024;

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
			else if (!(offset >= 0 && offset < buffer.Length))
			{
				throw new ArgumentOutOfRangeException(
					"offset", offset, "Offset must be greater than or equal to zero and less than the size of the buffer.");
			}
			else if (length < 0 || length > buffer.Length)
			{
				throw new ArgumentOutOfRangeException(
					"length", length, "Length cannot be less than zero or greater than the size of the buffer.");
			}
			else if ((buffer.Length - offset) < length)
			{
				throw new ArgumentException("Offset and length do not point to a valid segment within the buffer.");
			}

			int existingBytes = (_pending.Count * m_CoalesceBufferSize) + (_buffered == null ? 0 : _buffered.Length);

			if ((existingBytes + length) > PendingCap)
			{
				throw new CapacityExceededException();
			}

			Gram gram = null;

			while (length > 0)
			{
				if (_buffered == null)
				{
					// nothing yet buffered
					_buffered = Gram.Acquire();
				}

				int bytesWritten = _buffered.Write(buffer, offset, length);

				offset += bytesWritten;
				length -= bytesWritten;

				if (_buffered.IsFull)
				{
					if (_pending.Count == 0)
					{
						gram = _buffered;
					}

					_pending.Enqueue(_buffered);
					_buffered = null;
				}
			}

			return gram;
		}

		public void Clear()
		{
			if (_buffered != null)
			{
				_buffered.Release();
				_buffered = null;
			}

			while (_pending.Count > 0)
			{
				_pending.Dequeue().Release();
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