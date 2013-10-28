#region Header
// **********
// ServUO - EncodedReader.cs
// **********
#endregion

namespace Server.Network
{
	public class EncodedReader
	{
		private readonly PacketReader m_Reader;

		public EncodedReader(PacketReader reader)
		{
			m_Reader = reader;
		}

		public byte[] Buffer { get { return m_Reader.Buffer; } }

		public void Trace(NetState state)
		{
			m_Reader.Trace(state);
		}

		public int ReadInt32()
		{
			if (m_Reader.ReadByte() != 0)
			{
				return 0;
			}

			return m_Reader.ReadInt32();
		}

		public Point3D ReadPoint3D()
		{
			if (m_Reader.ReadByte() != 3)
			{
				return Point3D.Zero;
			}

			return new Point3D(m_Reader.ReadInt16(), m_Reader.ReadInt16(), m_Reader.ReadByte());
		}

		public string ReadUnicodeStringSafe()
		{
			if (m_Reader.ReadByte() != 2)
			{
				return "";
			}

			int length = m_Reader.ReadUInt16();

			return m_Reader.ReadUnicodeStringSafe(length);
		}

		public string ReadUnicodeString()
		{
			if (m_Reader.ReadByte() != 2)
			{
				return "";
			}

			int length = m_Reader.ReadUInt16();

			return m_Reader.ReadUnicodeString(length);
		}
	}
}