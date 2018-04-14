#region Header
// **********
// ServUO - BufferPool.cs
// **********
#endregion

#region References
using System.Collections.Generic;
#endregion

namespace Server.Network
{
	public class BufferPool
	{
		public static List<BufferPool> Pools { get; private set; }

		static BufferPool()
		{
			Pools = new List<BufferPool>();
		}

		private readonly string m_Name;

		private readonly int m_InitialCapacity;
		private readonly int m_BufferSize;

		private int m_Misses;

		private readonly Queue<byte[]> m_FreeBuffers;

		public int Count
		{
			get
			{
				lock (this)
					return m_FreeBuffers.Count;
			}
		}

		public void GetInfo(
			out string name,
			out int freeCount,
			out int initialCapacity,
			out int currentCapacity,
			out int bufferSize,
			out int misses)
		{
			lock (this)
			{
				name = m_Name;
				freeCount = m_FreeBuffers.Count;
				initialCapacity = m_InitialCapacity;
				currentCapacity = m_InitialCapacity * (1 + m_Misses);
				bufferSize = m_BufferSize;
				misses = m_Misses;
			}
		}

		public BufferPool(string name, int initialCapacity, int bufferSize)
		{
			m_Name = name;

			m_InitialCapacity = initialCapacity;
			m_BufferSize = bufferSize;

			m_FreeBuffers = new Queue<byte[]>(initialCapacity);

			for (int i = 0; i < initialCapacity; ++i)
			{
				m_FreeBuffers.Enqueue(new byte[bufferSize]);
			}

			lock (Pools)
				Pools.Add(this);
		}

		public byte[] AcquireBuffer()
		{
			lock (this)
			{
				if (m_FreeBuffers.Count > 0)
				{
					return m_FreeBuffers.Dequeue();
				}

				++m_Misses;

				for (int i = 0; i < m_InitialCapacity; ++i)
				{
					m_FreeBuffers.Enqueue(new byte[m_BufferSize]);
				}

				return m_FreeBuffers.Dequeue();
			}
		}

		public void ReleaseBuffer(byte[] buffer)
		{
			if (buffer == null)
			{
				return;
			}

			lock (this)
				m_FreeBuffers.Enqueue(buffer);
		}

		public void Free()
		{
			lock (Pools)
				Pools.Remove(this);
		}
	}
}