#region References
using System.IO;
using System.Text;
#endregion

namespace Server.Network
{
	public class PacketReader
	{
		private readonly byte[] m_Data;

		private int m_Index, m_Size, m_Slice;

		public int Index
		{
			get => m_Index;
			set => Seek(value, SeekOrigin.Begin);
		}

		public int Size => m_Size;
		public int Chop => m_Slice;

		public byte[] Buffer => m_Data;

		public byte ID => m_Data[0];

		public PacketReader(byte[] data, int size, bool fixedSize)
		{
			m_Data = data;
			m_Size = size;
			m_Index = fixedSize ? 1 : 3;
		}

		public void Trace(NetState state)
		{
			try
			{
				using (var sw = new StreamWriter("Packets.log", true))
				{
					var buffer = m_Data;

					if (buffer.Length > 0)
					{
						sw.WriteLine($"Client: {state}: Unhandled packet 0x{buffer[0]:X2}");
					}

					using (var ms = new MemoryStream(buffer))
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

		public void Slice()
		{
			if (m_Index <= m_Size)
			{
				m_Slice = m_Size - m_Index;
				m_Size -= m_Slice;

				if (m_Index > m_Size)
				{
					m_Index = m_Size;
				}
			}
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

		public void Skip(int size)
		{
			m_Index = Utility.Clamp(m_Index + size, 0, m_Size);
		}

		public byte[] ReadBytes(int count)
		{
			var buffer = new byte[count];

			ReadBytes(buffer);

			return buffer;
		}

		public void ReadBytes(byte[] buffer)
		{
			for (var i = 0; i < buffer.Length; i++)
			{
				buffer[i] = ReadByte();
			}
		}

		public Mobile ReadMobile()
		{
			return World.FindMobile(ReadSerial());
		}

		public Item ReadItem()
		{
			return World.FindItem(ReadSerial());
		}

		public IEntity ReadEntity()
		{
			return World.FindEntity(ReadSerial());
		}

		public Serial ReadSerial()
		{
			return new Serial(ReadInt32());
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

			return m_Data[m_Index++] != 0;
		}

		public string ReadUnicodeStringLE()
		{
			var sb = new StringBuilder();

			int c;

			while ((m_Index + 1) < m_Size && (c = m_Data[m_Index++] | (m_Data[m_Index++] << 8)) != 0)
			{
				sb.Append((char)c);
			}

			return sb.ToString();
		}

		public string ReadUnicodeStringLESafe(int fixedLength)
		{
			var bound = m_Index + (fixedLength << 1);
			var end = bound;

			if (bound > m_Size)
			{
				bound = m_Size;
			}

			var sb = new StringBuilder();

			int c;

			while ((m_Index + 1) < bound && (c = m_Data[m_Index++] | (m_Data[m_Index++] << 8)) != 0)
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
			var sb = new StringBuilder();

			int c;

			while ((m_Index + 1) < m_Size && (c = m_Data[m_Index++] | (m_Data[m_Index++] << 8)) != 0)
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
			var sb = new StringBuilder();

			int c;

			while ((m_Index + 1) < m_Size && (c = (m_Data[m_Index++] << 8) | m_Data[m_Index++]) != 0)
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
			var sb = new StringBuilder();

			int c;

			while ((m_Index + 1) < m_Size && (c = (m_Data[m_Index++] << 8) | m_Data[m_Index++]) != 0)
			{
				sb.Append((char)c);
			}

			return sb.ToString();
		}

		public bool IsSafeChar(int c)
		{
			return c >= 0x20 && c < 0xFFFE;
		}

		public string ReadUTF8StringSafe(int fixedLength)
		{
			if (m_Index >= m_Size)
			{
				m_Index += fixedLength;
				return System.String.Empty;
			}

			var bound = m_Index + fixedLength;
			//int end   = bound;

			if (bound > m_Size)
			{
				bound = m_Size;
			}

			var count = 0;
			var index = m_Index;
			var start = m_Index;

			while (index < bound && m_Data[index++] != 0)
			{
				++count;
			}

			index = 0;

			var buffer = new byte[count];
			var value = 0;

			while (m_Index < bound && (value = m_Data[m_Index++]) != 0)
			{
				buffer[index++] = (byte)value;
			}

			var s = Utility.UTF8.GetString(buffer);

			var isSafe = true;

			for (var i = 0; isSafe && i < s.Length; ++i)
			{
				isSafe = IsSafeChar(s[i]);
			}

			m_Index = start + fixedLength;

			if (isSafe)
			{
				return s;
			}

			var sb = new StringBuilder(s.Length);

			for (var i = 0; i < s.Length; ++i)
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
				return System.String.Empty;
			}

			var count = 0;
			var index = m_Index;

			while (index < m_Size && m_Data[index++] != 0)
			{
				++count;
			}

			index = 0;

			var buffer = new byte[count];
			var value = 0;

			while (m_Index < m_Size && (value = m_Data[m_Index++]) != 0)
			{
				buffer[index++] = (byte)value;
			}

			var s = Utility.UTF8.GetString(buffer);

			var isSafe = true;

			for (var i = 0; isSafe && i < s.Length; ++i)
			{
				isSafe = IsSafeChar(s[i]);
			}

			if (isSafe)
			{
				return s;
			}

			var sb = new StringBuilder(s.Length);

			for (var i = 0; i < s.Length; ++i)
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
				return System.String.Empty;
			}

			var count = 0;
			var index = m_Index;

			while (index < m_Size && m_Data[index++] != 0)
			{
				++count;
			}

			index = 0;

			var buffer = new byte[count];
			var value = 0;

			while (m_Index < m_Size && (value = m_Data[m_Index++]) != 0)
			{
				buffer[index++] = (byte)value;
			}

			return Utility.UTF8.GetString(buffer);
		}

		public string ReadString()
		{
			var sb = new StringBuilder();

			int c;

			while (m_Index < m_Size && (c = m_Data[m_Index++]) != 0)
			{
				sb.Append((char)c);
			}

			return sb.ToString();
		}

		public string ReadStringSafe()
		{
			var sb = new StringBuilder();

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
			var bound = m_Index + (fixedLength << 1);
			var end = bound;

			if (bound > m_Size)
			{
				bound = m_Size;
			}

			var sb = new StringBuilder();

			int c;

			while ((m_Index + 1) < bound && (c = (m_Data[m_Index++] << 8) | m_Data[m_Index++]) != 0)
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
			var bound = m_Index + (fixedLength << 1);
			var end = bound;

			if (bound > m_Size)
			{
				bound = m_Size;
			}

			var sb = new StringBuilder();

			int c;

			while ((m_Index + 1) < bound && (c = (m_Data[m_Index++] << 8) | m_Data[m_Index++]) != 0)
			{
				sb.Append((char)c);
			}

			m_Index = end;

			return sb.ToString();
		}

		public string ReadStringSafe(int fixedLength)
		{
			var bound = m_Index + fixedLength;
			var end = bound;

			if (bound > m_Size)
			{
				bound = m_Size;
			}

			var sb = new StringBuilder();

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
			var bound = m_Index + fixedLength;
			var end = bound;

			if (bound > m_Size)
			{
				bound = m_Size;
			}

			var sb = new StringBuilder();

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