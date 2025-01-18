#region References
using System;
using System.Collections.Concurrent;
using System.Threading;
#endregion

namespace Server.Network
{
	public class SendQueue
	{
		public class Gram
		{
			private static readonly ConcurrentQueue<Gram> _pool = new ConcurrentQueue<Gram>();

            public static Gram Acquire()
            {
                if (!_pool.TryDequeue(out Gram gram))
                {
                    gram = new Gram();
                }

                gram._buffer = AcquireBuffer();
                gram._length = 0;

                return gram;
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
                _pool.Enqueue(this);

                ReleaseBuffer(_buffer);
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

                m_UnusedBuffers?.Free();

                m_CoalesceBufferSize = value;
                m_UnusedBuffers = new BufferPool("Coalesced", 2048, m_CoalesceBufferSize);
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
                m_UnusedBuffers.ReleaseBuffer(buffer);
            }
        }

		private readonly ConcurrentQueue<Gram> _pending = new ConcurrentQueue<Gram>();

		private volatile Gram _buffered;

		public bool IsFlushReady { get { return (_pending.Count == 0 && _buffered != null); } }

		public bool IsEmpty { get { return (_pending.Count == 0 && _buffered == null); } }

		public Gram CheckFlushReady()
		{
			Gram gram = Interlocked.Exchange(ref _buffered, null);

            _pending.Enqueue(gram);

			return gram;
		}

		public Gram Dequeue()
		{
            if (_pending.TryDequeue(out Gram gram))
            {
                gram?.Release();
                gram = null;

                if (_pending.TryPeek(out Gram peek))
                    gram = peek;
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

            int existingBytes = (_pending.Count * m_CoalesceBufferSize) + (_buffered?.Length ?? 0);

			if ((existingBytes + length) > PendingCap)
			{
				throw new CapacityExceededException();
			}

			Gram gram = null;

			while (length > 0)
			{
                Gram buffered = _buffered;

                if (buffered == null)
                {
                    Interlocked.Exchange(ref _buffered, buffered = Gram.Acquire());
                }

				int bytesWritten = buffered.Write(buffer, offset, length);

				offset += bytesWritten;
				length -= bytesWritten;

				if (buffered.IsFull)
				{
                    Interlocked.CompareExchange(ref _buffered, null, buffered);

					if (_pending.IsEmpty)
					{
						gram = buffered;
					}

					_pending.Enqueue(buffered);
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

			while (_pending.TryDequeue(out Gram gram))
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
