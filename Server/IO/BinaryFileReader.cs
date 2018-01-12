using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using CustomsFramework;
using Server.Guilds;

namespace Server
{
	public sealed class BinaryFileReader : GenericReader
	{
		private readonly BinaryReader m_File;

		public BinaryFileReader(BinaryReader br)
		{
			m_File = br;
		}

		public void Close()
		{
			m_File.Close();
		}

		public long Position { get { return m_File.BaseStream.Position; } }

		public long Seek(long offset, SeekOrigin origin)
		{
			return m_File.BaseStream.Seek(offset, origin);
		}

		public override string ReadString()
		{
			if (ReadByte() != 0)
			{
				return m_File.ReadString();
			}
			else
			{
				return null;
			}
		}

		public override DateTime ReadDeltaTime()
		{
			long ticks = m_File.ReadInt64();
			long now = DateTime.UtcNow.Ticks;

			if (ticks > 0 && (ticks + now) < 0)
			{
				return DateTime.MaxValue;
			}
			else if (ticks < 0 && (ticks + now) < 0)
			{
				return DateTime.MinValue;
			}

			try
			{
				return new DateTime(now + ticks);
			}
			catch
			{
				if (ticks > 0)
				{
					return DateTime.MaxValue;
				}
				else
				{
					return DateTime.MinValue;
				}
			}
		}

		public override IPAddress ReadIPAddress()
		{
			return new IPAddress(m_File.ReadInt64());
		}

		public override int ReadEncodedInt()
		{
			int v = 0, shift = 0;
			byte b;

			do
			{
				b = m_File.ReadByte();
				v |= (b & 0x7F) << shift;
				shift += 7;
			}
			while (b >= 0x80);

			return v;
		}

		public override DateTime ReadDateTime()
		{
			return new DateTime(m_File.ReadInt64());
		}

		public override DateTimeOffset ReadDateTimeOffset()
		{
			long ticks = m_File.ReadInt64();
			TimeSpan offset = new TimeSpan(m_File.ReadInt64());

			return new DateTimeOffset(ticks, offset);
		}

		public override TimeSpan ReadTimeSpan()
		{
			return new TimeSpan(m_File.ReadInt64());
		}

		public override decimal ReadDecimal()
		{
			return m_File.ReadDecimal();
		}

		public override long ReadLong()
		{
			return m_File.ReadInt64();
		}

		public override ulong ReadULong()
		{
			return m_File.ReadUInt64();
		}

		public override int PeekInt()
		{
			int value = 0;
			long returnTo = m_File.BaseStream.Position;

			try
			{
				value = m_File.ReadInt32();
			}
			catch(EndOfStreamException)
			{
				// Ignore this exception, the defalut value 0 will be returned
			}

			m_File.BaseStream.Seek(returnTo, SeekOrigin.Begin);
			return value;
		}

		public override int ReadInt()
		{
			return m_File.ReadInt32();
		}

		public override uint ReadUInt()
		{
			return m_File.ReadUInt32();
		}

		public override short ReadShort()
		{
			return m_File.ReadInt16();
		}

		public override ushort ReadUShort()
		{
			return m_File.ReadUInt16();
		}

		public override double ReadDouble()
		{
			return m_File.ReadDouble();
		}

		public override float ReadFloat()
		{
			return m_File.ReadSingle();
		}

		public override char ReadChar()
		{
			return m_File.ReadChar();
		}

		public override byte ReadByte()
		{
			return m_File.ReadByte();
		}

		public override sbyte ReadSByte()
		{
			return m_File.ReadSByte();
		}

		public override bool ReadBool()
		{
			return m_File.ReadBoolean();
		}

		public override Point3D ReadPoint3D()
		{
			return new Point3D(ReadInt(), ReadInt(), ReadInt());
		}

		public override Point2D ReadPoint2D()
		{
			return new Point2D(ReadInt(), ReadInt());
		}

		public override Rectangle2D ReadRect2D()
		{
			return new Rectangle2D(ReadPoint2D(), ReadPoint2D());
		}

		public override Rectangle3D ReadRect3D()
		{
			return new Rectangle3D(ReadPoint3D(), ReadPoint3D());
		}

		public override Map ReadMap()
		{
			return Map.Maps[ReadByte()];
		}

		public override Item ReadItem()
		{
			return World.FindItem(ReadInt());
		}

		public override Mobile ReadMobile()
		{
			return World.FindMobile(ReadInt());
		}

		public override BaseGuild ReadGuild()
		{
			return BaseGuild.Find(ReadInt());
		}

		public override SaveData ReadData()
		{
			return World.GetData(ReadInt());
		}

		public override T ReadItem<T>()
		{
			return ReadItem() as T;
		}

		public override T ReadMobile<T>()
		{
			return ReadMobile() as T;
		}

		public override T ReadGuild<T>()
		{
			return ReadGuild() as T;
		}

		public override T ReadData<T>()
		{
			return ReadData() as T;
		}

		public override ArrayList ReadItemList()
		{
			int count = ReadInt();

			if (count > 0)
			{
				ArrayList list = new ArrayList(count);

				for (int i = 0; i < count; ++i)
				{
					Item item = ReadItem();

					if (item != null)
					{
						list.Add(item);
					}
				}

				return list;
			}
			else
			{
				return new ArrayList();
			}
		}

		public override ArrayList ReadMobileList()
		{
			int count = ReadInt();

			if (count > 0)
			{
				ArrayList list = new ArrayList(count);

				for (int i = 0; i < count; ++i)
				{
					Mobile m = ReadMobile();

					if (m != null)
					{
						list.Add(m);
					}
				}

				return list;
			}
			else
			{
				return new ArrayList();
			}
		}

		public override ArrayList ReadGuildList()
		{
			int count = ReadInt();

			if (count > 0)
			{
				ArrayList list = new ArrayList(count);

				for (int i = 0; i < count; ++i)
				{
					BaseGuild g = ReadGuild();

					if (g != null)
					{
						list.Add(g);
					}
				}

				return list;
			}
			else
			{
				return new ArrayList();
			}
		}

		public override ArrayList ReadDataList()
		{
			int count = ReadInt();

			if (count > 0)
			{
				ArrayList list = new ArrayList(count);

				for (int i = 0; i < count; ++i)
				{
					SaveData data = ReadData();

					if (data != null)
					{
						list.Add(data);
					}
				}

				return list;
			}
			else
			{
				return new ArrayList();
			}
		}

		public override List<Item> ReadStrongItemList()
		{
			return ReadStrongItemList<Item>();
		}

		public override List<T> ReadStrongItemList<T>()
		{
			int count = ReadInt();

			if (count > 0)
			{
				var list = new List<T>(count);

				for (int i = 0; i < count; ++i)
				{
					T item = ReadItem() as T;

					if (item != null)
					{
						list.Add(item);
					}
				}

				return list;
			}
			else
			{
				return new List<T>();
			}
		}

		public override HashSet<Item> ReadItemSet()
		{
			return ReadItemSet<Item>();
		}

		public override HashSet<T> ReadItemSet<T>()
		{
			int count = ReadInt();

			if (count > 0)
			{
				var set = new HashSet<T>();

				for (int i = 0; i < count; ++i)
				{
					T item = ReadItem() as T;

					if (item != null)
					{
						set.Add(item);
					}
				}

				return set;
			}
			else
			{
				return new HashSet<T>();
			}
		}

		public override List<Mobile> ReadStrongMobileList()
		{
			return ReadStrongMobileList<Mobile>();
		}

		public override List<T> ReadStrongMobileList<T>()
		{
			int count = ReadInt();

			if (count > 0)
			{
				var list = new List<T>(count);

				for (int i = 0; i < count; ++i)
				{
					T m = ReadMobile() as T;

					if (m != null)
					{
						list.Add(m);
					}
				}

				return list;
			}
			else
			{
				return new List<T>();
			}
		}

		public override HashSet<Mobile> ReadMobileSet()
		{
			return ReadMobileSet<Mobile>();
		}

		public override HashSet<T> ReadMobileSet<T>()
		{
			int count = ReadInt();

			if (count > 0)
			{
				var set = new HashSet<T>();

				for (int i = 0; i < count; ++i)
				{
					T item = ReadMobile() as T;

					if (item != null)
					{
						set.Add(item);
					}
				}

				return set;
			}
			else
			{
				return new HashSet<T>();
			}
		}

		public override List<BaseGuild> ReadStrongGuildList()
		{
			return ReadStrongGuildList<BaseGuild>();
		}

		public override List<T> ReadStrongGuildList<T>()
		{
			int count = ReadInt();

			if (count > 0)
			{
				var list = new List<T>(count);

				for (int i = 0; i < count; ++i)
				{
					T g = ReadGuild() as T;

					if (g != null)
					{
						list.Add(g);
					}
				}

				return list;
			}
			else
			{
				return new List<T>();
			}
		}

		public override HashSet<BaseGuild> ReadGuildSet()
		{
			return ReadGuildSet<BaseGuild>();
		}

		public override HashSet<T> ReadGuildSet<T>()
		{
			int count = ReadInt();

			if (count > 0)
			{
				var set = new HashSet<T>();

				for (int i = 0; i < count; ++i)
				{
					T item = ReadGuild() as T;

					if (item != null)
					{
						set.Add(item);
					}
				}

				return set;
			}
			else
			{
				return new HashSet<T>();
			}
		}

		public override List<SaveData> ReadStrongDataList()
		{
			return ReadStrongDataList<SaveData>();
		}

		public override List<T> ReadStrongDataList<T>()
		{
			int count = ReadInt();

			if (count > 0)
			{
				var list = new List<T>(count);

				for (int i = 0; i < count; ++i)
				{
					T data = ReadData() as T;

					if (data != null)
					{
						list.Add(data);
					}
				}

				return list;
			}
			else
			{
				return new List<T>();
			}
		}

		public override HashSet<SaveData> ReadDataSet()
		{
			return ReadDataSet<SaveData>();
		}

		public override HashSet<T> ReadDataSet<T>()
		{
			int count = ReadInt();

			if (count > 0)
			{
				var set = new HashSet<T>();

				for (int i = 0; i < count; ++i)
				{
					T data = ReadData() as T;

					if (data != null)
					{
						set.Add(data);
					}
				}

				return set;
			}
			else
			{
				return new HashSet<T>();
			}
		}

		public override Race ReadRace()
		{
			return Race.Races[ReadByte()];
		}

		public override bool End()
		{
			return m_File.PeekChar() == -1;
		}
	}
}