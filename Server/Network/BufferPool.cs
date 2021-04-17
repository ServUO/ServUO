#region References
using System.Collections.Concurrent;
using System.Collections.Generic;
#endregion

namespace Server.Network
{
	public class BufferPool
	{
		private static readonly ConcurrentDictionary<int, BufferPool> m_Pools = new ConcurrentDictionary<int, BufferPool>();

		public static ICollection<BufferPool> Pools => m_Pools.Values;

		private static volatile int m_NewUID;

		private readonly int m_UID;
		private readonly string m_Name;

		private readonly int m_InitialCapacity;
		private readonly int m_BufferSize;

		private volatile int m_Misses;

		private readonly ConcurrentQueue<byte[]> m_FreeBuffers = new ConcurrentQueue<byte[]>();

		public int Count => m_FreeBuffers.Count;

		public void GetInfo(out string name, out int freeCount, out int initialCapacity, out int currentCapacity, out int bufferSize, out int misses)
		{
			name = m_Name;
			freeCount = m_FreeBuffers.Count;
			initialCapacity = m_InitialCapacity;
			currentCapacity = m_InitialCapacity + m_Misses;
			bufferSize = m_BufferSize;
			misses = m_Misses;
		}

		public BufferPool(string name, int initialCapacity, int bufferSize)
		{
			m_UID = ++m_NewUID;
			m_Name = name;

			m_InitialCapacity = initialCapacity;
			m_BufferSize = bufferSize;

			for (var i = 0; i < initialCapacity; ++i)
			{
				m_FreeBuffers.Enqueue(new byte[bufferSize]);
			}

			m_Pools[m_UID] = this;
		}

		public byte[] AcquireBuffer()
		{
			if (!m_FreeBuffers.TryDequeue(out var buffer))
			{
				buffer = new byte[m_BufferSize];

				++m_Misses;
			}

			return buffer;
		}

		public void ReleaseBuffer(ref byte[] buffer)
		{
			if (buffer != null)
			{
				m_FreeBuffers.Enqueue(buffer);
			}
		}

		public void Free()
		{
			m_Pools.TryRemove(m_UID, out _);
		}
	}
}
