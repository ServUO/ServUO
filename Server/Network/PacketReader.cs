#region Header
// **********
// ServUO - PacketReader.cs
// **********
#endregion

#region References
using System;
using System.IO;
using System.Text;
#endregion

namespace Server.Network
{
	public class PacketReader
	{
		private readonly byte[] m_Data;
		private readonly int m_Size;
		private int m_Index;

		public PacketReader(byte[] data, int size, bool fixedSize)
		{
			m_Data = data;
			m_Size = size;
			m_Index = fixedSize ? 1 : 3;
		}

		public byte[] Buffer { get { return m_Data; } }

		public int Size { get { return m_Size; } }

		public void Trace(NetState state)
		{
			try
			{
				using (StreamWriter sw = new StreamWriter("Packets.log", true))
				{
					var buffer = m_Data;

					if (buffer.Length > 0)
					{
						sw.WriteLine("Client: {0}: Unhandled packet 0x{1:X2}", state, buffer[0]);
					}

					using (MemoryStream ms = new MemoryStream(buffer))
					{
						Utility.FormatBuffer(sw, ms, buffer.Length);
					}

					sw.WriteLine();
					sw.WriteLine();
				}
			}
			catch
			{ }
		}

		public int Seek(int offset, SeekOrigin origin)
		{
			switch (origin)
			{
				case SeekOrigin.Begin:
					m_Index = offset;
					break;
				case SeekOrigin.Current:
					m_Index += offset;
					break;
				case SeekOrigin.End:
					m_Index = m_Size - offset;
					break;
			}

			return m_Index;
		}

		public int ReadInt32()
		{
			if ((m_Index + 4) > m_Size)
			{
				return 0;
			}

			return (m_Data[m_Index++] << 24) | (m_Data[m_Index++] << 16) | (m_Data[m_Index++] << 8) | m_Data[m_Index++];
		}

		public short ReadInt16()
		{
			if ((m_Index + 2) > m_Size)
			{
				return 0;
			}

			return (short)((m_Data[m_Index++] << 8) | m_Data[m_Index++]);
		}

		public byte ReadByte()
		{
			if ((m_Index + 1) > m_Size)
			{
				return 0;
			}

			return m_Data[m_Index++];
		}

		public uint ReadUInt32()
		{
			if ((m_Index + 4) > m_Size)
			{
				return 0;
			}

			return (uint)((m_Data[m_Index++] << 24) | (m_Data[m_Index++] << 16) | (m_Data[m_Index++] << 8) | m_Data[m_Index++]);
		}

		public ushort ReadUInt16()
		{
			if ((m_Index + 2) > m_Size)
			{
				return 0;
			}

			return (ushort)((m_Data[m_Index++] << 8) | m_Data[m_Index++]);
		}

		public sbyte ReadSByte()
		{
			if ((m_Index + 1) > m_Size)
			{
				return 0;
			}

			return (sbyte)m_Data[m_Index++];
		}

		public bool ReadBoolean()
		{
			if ((m_Index + 1) > m_Size)
			{
				return false;
			}

			return (m_Data[m_Index++] != 0);
		}

		public string ReadUnicodeStringLE()
		{
			StringBuilder sb = new StringBuilder();

			int c;

			while ((m_Index + 1) < m_Size && (c = (m_Data[m_Index++] | (m_Data[m_Index++] << 8))) != 0)
			{
				sb.Append((char)c);
			}

			return sb.ToString();
		}

		public string ReadUnicodeStringLESafe(int fixedLength)
		{
			int bound = m_Index + (fixedLength << 1);
			int end = bound;

			if (bound > m_Size)
			{
				bound = m_Size;
			}

			StringBuilder sb = new StringBuilder();

			int c;

			while ((m_Index + 1) < bound && (c = (m_Data[m_Index++] | (m_Data[m_Index++] << 8))) != 0)
			{
				if (IsSafeChar(c))
				{
					sb.Append((char)c);
				}
			}

			m_Index = end;

			return sb.ToString();
		}

		public string ReadUnicodeStringLESafe()
		{
			StringBuilder sb = new StringBuilder();

			int c;

			while ((m_Index + 1) < m_Size && (c = (m_Data[m_Index++] | (m_Data[m_Index++] << 8))) != 0)
			{
				if (IsSafeChar(c))
				{
					sb.Append((char)c);
				}
			}

			return sb.ToString();
		}

		public string ReadUnicodeStringSafe()
		{
			StringBuilder sb = new StringBuilder();

			int c;

			while ((m_Index + 1) < m_Size && (c = ((m_Data[m_Index++] << 8) | m_Data[m_Index++])) != 0)
			{
				if (IsSafeChar(c))
				{
					sb.Append((char)c);
				}
			}

			return sb.ToString();
		}

		public string ReadUnicodeString()
		{
			StringBuilder sb = new StringBuilder();

			int c;

			while ((m_Index + 1) < m_Size && (c = ((m_Data[m_Index++] << 8) | m_Data[m_Index++])) != 0)
			{
				sb.Append((char)c);
			}

			return sb.ToString();
		}

		public bool IsSafeChar(int c)
		{
			return (c >= 0x20 && c < 0xFFFE);
		}

		public string ReadUTF8StringSafe(int fixedLength)
		{
			if (m_Index >= m_Size)
			{
				m_Index += fixedLength;
				return String.Empty;
			}

			int bound = m_Index + fixedLength;
			//int end   = bound;

			if (bound > m_Size)
			{
				bound = m_Size;
			}

			int count = 0;
			int index = m_Index;
			int start = m_Index;

			while (index < bound && m_Data[index++] != 0)
			{
				++count;
			}

			index = 0;

			var buffer = new byte[count];
			int value = 0;

			while (m_Index < bound && (value = m_Data[m_Index++]) != 0)
			{
				buffer[index++] = (byte)value;
			}

			string s = Utility.UTF8.GetString(buffer);

			bool isSafe = true;

			for (int i = 0; isSafe && i < s.Length; ++i)
			{
				isSafe = IsSafeChar(s[i]);
			}

			m_Index = start + fixedLength;

			if (isSafe)
			{
				return s;
			}

			StringBuilder sb = new StringBuilder(s.Length);

			for (int i = 0; i < s.Length; ++i)
			{
				if (IsSafeChar(s[i]))
				{
					sb.Append(s[i]);
				}
			}

			return sb.ToString();
		}

		public string ReadUTF8StringSafe()
		{
			if (m_Index >= m_Size)
			{
				return String.Empty;
			}

			int count = 0;
			int index = m_Index;

			while (index < m_Size && m_Data[index++] != 0)
			{
				++count;
			}

			index = 0;

			var buffer = new byte[count];
			int value = 0;

			while (m_Index < m_Size && (value = m_Data[m_Index++]) != 0)
			{
				buffer[index++] = (byte)value;
			}

			string s = Utility.UTF8.GetString(buffer);

			bool isSafe = true;

			for (int i = 0; isSafe && i < s.Length; ++i)
			{
				isSafe = IsSafeChar(s[i]);
			}

			if (isSafe)
			{
				return s;
			}

			StringBuilder sb = new StringBuilder(s.Length);

			for (int i = 0; i < s.Length; ++i)
			{
				if (IsSafeChar(s[i]))
				{
					sb.Append(s[i]);
				}
			}

			return sb.ToString();
		}

		public string ReadUTF8String()
		{
			if (m_Index >= m_Size)
			{
				return String.Empty;
			}

			int count = 0;
			int index = m_Index;

			while (index < m_Size && m_Data[index++] != 0)
			{
				++count;
			}

			index = 0;

			var buffer = new byte[count];
			int value = 0;

			while (m_Index < m_Size && (value = m_Data[m_Index++]) != 0)
			{
				buffer[index++] = (byte)value;
			}

			return Utility.UTF8.GetString(buffer);
		}

		public string ReadString()
		{
			StringBuilder sb = new StringBuilder();

			int c;

			while (m_Index < m_Size && (c = m_Data[m_Index++]) != 0)
			{
				sb.Append((char)c);
			}

			return sb.ToString();
		}

		public string ReadStringSafe()
		{
			StringBuilder sb = new StringBuilder();

			int c;

			while (m_Index < m_Size && (c = m_Data[m_Index++]) != 0)
			{
				if (IsSafeChar(c))
				{
					sb.Append((char)c);
				}
			}

			return sb.ToString();
		}

		public string ReadUnicodeStringSafe(int fixedLength)
		{
			int bound = m_Index + (fixedLength << 1);
			int end = bound;

			if (bound > m_Size)
			{
				bound = m_Size;
			}

			StringBuilder sb = new StringBuilder();

			int c;

			while ((m_Index + 1) < bound && (c = ((m_Data[m_Index++] << 8) | m_Data[m_Index++])) != 0)
			{
				if (IsSafeChar(c))
				{
					sb.Append((char)c);
				}
			}

			m_Index = end;

			return sb.ToString();
		}

		public string ReadUnicodeString(int fixedLength)
		{
			int bound = m_Index + (fixedLength << 1);
			int end = bound;

			if (bound > m_Size)
			{
				bound = m_Size;
			}

			StringBuilder sb = new StringBuilder();

			int c;

			while ((m_Index + 1) < bound && (c = ((m_Data[m_Index++] << 8) | m_Data[m_Index++])) != 0)
			{
				sb.Append((char)c);
			}

			m_Index = end;

			return sb.ToString();
		}

		public string ReadStringSafe(int fixedLength)
		{
			int bound = m_Index + fixedLength;
			int end = bound;

			if (bound > m_Size)
			{
				bound = m_Size;
			}

			StringBuilder sb = new StringBuilder();

			int c;

			while (m_Index < bound && (c = m_Data[m_Index++]) != 0)
			{
				if (IsSafeChar(c))
				{
					sb.Append((char)c);
				}
			}

			m_Index = end;

			return sb.ToString();
		}

		public string ReadString(int fixedLength)
		{
			int bound = m_Index + fixedLength;
			int end = bound;

			if (bound > m_Size)
			{
				bound = m_Size;
			}

			StringBuilder sb = new StringBuilder();

			int c;

			while (m_Index < bound && (c = m_Data[m_Index++]) != 0)
			{
				sb.Append((char)c);
			}

			m_Index = end;

			return sb.ToString();
		}
	}
}