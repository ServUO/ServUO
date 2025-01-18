#region References
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
#endregion

namespace Server.Network
{
    public class BufferPool
    {
        private static readonly ConcurrentDictionary<BufferPool, string> m_Pools = new ConcurrentDictionary<BufferPool, string>();

        public static ReadOnlyCollection<BufferPool> Pools => (ReadOnlyCollection<BufferPool>)m_Pools.Keys;

        private readonly string m_Name;

        private readonly int m_InitialCapacity;
        private readonly int m_BufferSize;

        private volatile int m_Misses;

        private readonly ConcurrentQueue<byte[]> m_FreeBuffers = new ConcurrentQueue<byte[]>();

        public int Count
        {
            get
            {
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
            name = m_Name;
            freeCount = m_FreeBuffers.Count;
            initialCapacity = m_InitialCapacity;
            currentCapacity = m_InitialCapacity * (1 + m_Misses);
            bufferSize = m_BufferSize;
            misses = m_Misses;
        }

        public BufferPool(string name, int initialCapacity, int bufferSize)
        {
            m_Name = name;

            m_InitialCapacity = initialCapacity;
            m_BufferSize = bufferSize;

            while (m_FreeBuffers.Count < initialCapacity)
            {
                m_FreeBuffers.Enqueue(new byte[bufferSize]);
            }

            m_Pools[this] = name;
        }

        public byte[] AcquireBuffer()
        {
            if (m_FreeBuffers.TryDequeue(out var buffer))
            {
                return buffer;
            }

            ++m_Misses;

            while (m_FreeBuffers.Count < m_InitialCapacity - 1)
            {
                m_FreeBuffers.Enqueue(new byte[m_BufferSize]);
            }

            return new byte[m_BufferSize];
        }

        public void ReleaseBuffer(byte[] buffer)
        {
            if (buffer == null || buffer.Length != m_BufferSize)
            {
                return;
            }

            Array.Clear(buffer, 0, buffer.Length);

            m_FreeBuffers.Enqueue(buffer);
        }

        public void Free()
        {
            m_Pools.TryRemove(this, out _);
        }
    }
}
