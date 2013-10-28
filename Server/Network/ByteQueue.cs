#region Header
// **********
// ServUO - ByteQueue.cs
// **********
#endregion

#region References
using System;
#endregion

namespace Server.Network
{
	public class ByteQueue
	{
		private int m_Head;
		private int m_Tail;
		private int m_Size;

		private byte[] m_Buffer;

		public int Length { get { return m_Size; } }

		public ByteQueue()
		{
			m_Buffer = new byte[2048];
		}

		public void Clear()
		{
			m_Head = 0;
			m_Tail = 0;
			m_Size = 0;
		}

		private void SetCapacity(int capacity)
		{
			var newBuffer = new byte[capacity];

			if (m_Size > 0)
			{
				if (m_Head < m_Tail)
				{
					Buffer.BlockCopy(m_Buffer, m_Head, newBuffer, 0, m_Size);
				}
				else
				{
					Buffer.BlockCopy(m_Buffer, m_Head, newBuffer, 0, m_Buffer.Length - m_Head);
					Buffer.BlockCopy(m_Buffer, 0, newBuffer, m_Buffer.Length - m_Head, m_Tail);
				}
			}

			m_Head = 0;
			m_Tail = m_Size;
			m_Buffer = newBuffer;
		}

		public byte GetPacketID()
		{
			if (m_Size >= 1)
			{
				return m_Buffer[m_Head];
			}

			return 0xFF;
		}

		public int GetPacketLength()
		{
			if (m_Size >= 3)
			{
				return (m_Buffer[(m_Head + 1) % m_Buffer.Length] << 8) | m_Buffer[(m_Head + 2) % m_Buffer.Length];
			}

			return 0;
		}

		public int Dequeue(byte[] buffer, int offset, int size)
		{
			if (size > m_Size)
			{
				size = m_Size;
			}

			if (size == 0)
			{
				return 0;
			}

			if (m_Head < m_Tail)
			{
				Buffer.BlockCopy(m_Buffer, m_Head, buffer, offset, size);
			}
			else
			{
				int rightLength = (m_Buffer.Length - m_Head);

				if (rightLength >= size)
				{
					Buffer.BlockCopy(m_Buffer, m_Head, buffer, offset, size);
				}
				else
				{
					Buffer.BlockCopy(m_Buffer, m_Head, buffer, offset, rightLength);
					Buffer.BlockCopy(m_Buffer, 0, buffer, offset + rightLength, size - rightLength);
				}
			}

			m_Head = (m_Head + size) % m_Buffer.Length;
			m_Size -= size;

			if (m_Size == 0)
			{
				m_Head = 0;
				m_Tail = 0;
			}

			return size;
		}

		public void Enqueue(byte[] buffer, int offset, int size)
		{
			if ((m_Size + size) > m_Buffer.Length)
			{
				SetCapacity((m_Size + size + 2047) & ~2047);
			}

			if (m_Head < m_Tail)
			{
				int rightLength = (m_Buffer.Length - m_Tail);

				if (rightLength >= size)
				{
					Buffer.BlockCopy(buffer, offset, m_Buffer, m_Tail, size);
				}
				else
				{
					Buffer.BlockCopy(buffer, offset, m_Buffer, m_Tail, rightLength);
					Buffer.BlockCopy(buffer, offset + rightLength, m_Buffer, 0, size - rightLength);
				}
			}
			else
			{
				Buffer.BlockCopy(buffer, offset, m_Buffer, m_Tail, size);
			}

			m_Tail = (m_Tail + size) % m_Buffer.Length;
			m_Size += size;
		}
	}
}