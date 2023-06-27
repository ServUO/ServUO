#region References
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

using Server.Guilds;
#endregion

namespace Server
{
	public abstract class GenericReader
	{
		public abstract Type ReadObjectType();

		public abstract string ReadString();

		public abstract DateTime ReadDeltaTime();
		public abstract DateTime ReadDateTime();
		public abstract DateTimeOffset ReadDateTimeOffset();
		public abstract TimeSpan ReadTimeSpan();

		public abstract Enum ReadEnum();
		public abstract T ReadEnum<T>() where T : struct, Enum;

		public abstract decimal ReadDecimal();
		public abstract long ReadLong();
		public abstract ulong ReadULong();
		public abstract int ReadInt();
		public abstract uint ReadUInt();
		public abstract short ReadShort();
		public abstract ushort ReadUShort();
		public abstract double ReadDouble();
		public abstract float ReadFloat();
		public abstract char ReadChar();
		public abstract byte ReadByte();
		public abstract sbyte ReadSByte();
		public abstract bool ReadBool();

		public abstract int ReadEncodedInt();
		public abstract uint ReadEncodedUInt();
		public abstract long ReadEncodedLong();
		public abstract ulong ReadEncodedULong();

		public abstract IPAddress ReadIPAddress();

		public abstract Point2D ReadPoint2D();
		public abstract Point3D ReadPoint3D();
		public abstract Rectangle2D ReadRect2D();
		public abstract Rectangle3D ReadRect3D();
		public abstract Map ReadMap();

		public abstract Race ReadRace();

		public abstract Serial ReadSerial();

		public abstract IEntity ReadEntity();
		public abstract Item ReadItem();
		public abstract Mobile ReadMobile();
		public abstract BaseGuild ReadGuild();

		public abstract T ReadEntity<T>() where T : class, IEntity;
		public abstract T ReadItem<T>() where T : Item;
		public abstract T ReadMobile<T>() where T : Mobile;
		public abstract T ReadGuild<T>() where T : BaseGuild;

		public abstract ArrayList ReadObjectList<T>(Func<T> reader);

		public abstract ArrayList ReadEntityList();
		public abstract ArrayList ReadItemList();
		public abstract ArrayList ReadMobileList();
		public abstract ArrayList ReadGuildList();

		public abstract List<T> ReadStrongObjectList<T>(Func<T> reader);

		public abstract List<T> ReadStrongEntityList<T>() where T : class, IEntity;
		public abstract List<T> ReadStrongItemList<T>() where T : Item;
		public abstract List<T> ReadStrongMobileList<T>() where T : Mobile;
		public abstract List<T> ReadStrongGuildList<T>() where T : BaseGuild;

		public abstract List<IEntity> ReadStrongEntityList();
		public abstract List<Item> ReadStrongItemList();
		public abstract List<Mobile> ReadStrongMobileList();
		public abstract List<BaseGuild> ReadStrongGuildList();

		public abstract HashSet<T> ReadObjectSet<T>(Func<T> reader);

		public abstract HashSet<T> ReadEntitySet<T>() where T : class, IEntity;
		public abstract HashSet<T> ReadItemSet<T>() where T : Item;
		public abstract HashSet<T> ReadMobileSet<T>() where T : Mobile;
		public abstract HashSet<T> ReadGuildSet<T>() where T : BaseGuild;

		public abstract HashSet<IEntity> ReadEntitySet();
		public abstract HashSet<Item> ReadItemSet();
		public abstract HashSet<Mobile> ReadMobileSet();
		public abstract HashSet<BaseGuild> ReadGuildSet();

		public abstract void Skip(int count);

		public abstract int PeekInt();

		public abstract bool End();
	}

	public abstract class GenericWriter
	{
		public abstract void Close();

		public abstract long Position { get; }

		public abstract void WriteObjectType(object value);
		public abstract void WriteObjectType(Type value);

		public abstract void Write(string value);

		public abstract void WriteDeltaTime(DateTime value);
		public abstract void Write(DateTime value);
		public abstract void Write(DateTimeOffset value);
		public abstract void Write(TimeSpan value);

		public abstract void Write(Enum value);

		public abstract void Write(decimal value);
		public abstract void Write(long value);
		public abstract void Write(ulong value);
		public abstract void Write(int value);
		public abstract void Write(uint value);
		public abstract void Write(short value);
		public abstract void Write(ushort value);
		public abstract void Write(double value);
		public abstract void Write(float value);
		public abstract void Write(char value);
		public abstract void Write(byte value);
		public abstract void Write(sbyte value);
		public abstract void Write(bool value);

		public abstract void WriteEncodedInt(int value);
		public abstract void WriteEncodedUInt(uint value);
		public abstract void WriteEncodedLong(long value);
		public abstract void WriteEncodedULong(ulong value);

		public abstract void Write(IPAddress value);

		public abstract void Write(Point2D value);
		public abstract void Write(Point3D value);
		public abstract void Write(Rectangle2D value);
		public abstract void Write(Rectangle3D value);
		public abstract void Write(Map value);

		public abstract void Write(Race value);

		public abstract void Write(Serial value);

		public abstract void Write(IEntity value);
		public abstract void Write(Item value);
		public abstract void Write(Mobile value);
		public abstract void Write(BaseGuild value);

		public abstract void WriteObjectList<T>(ArrayList list, Action<GenericWriter, T> writer);

		public abstract void WriteEntityList(ArrayList list);
		public abstract void WriteEntityList(ArrayList list, bool tidy);

		public abstract void WriteItemList(ArrayList list);
		public abstract void WriteItemList(ArrayList list, bool tidy);

		public abstract void WriteMobileList(ArrayList list);
		public abstract void WriteMobileList(ArrayList list, bool tidy);

		public abstract void WriteGuildList(ArrayList list);
		public abstract void WriteGuildList(ArrayList list, bool tidy);

		public abstract void Write<T>(List<T> list, Action<GenericWriter, T> writer);

		public abstract void Write(List<IEntity> list);
		public abstract void Write(List<IEntity> list, bool tidy);

		public abstract void Write(List<Item> list);
		public abstract void Write(List<Item> list, bool tidy);

		public abstract void Write(List<Mobile> list);
		public abstract void Write(List<Mobile> list, bool tidy);

		public abstract void Write(List<BaseGuild> list);
		public abstract void Write(List<BaseGuild> list, bool tidy);

		public abstract void WriteEntityList<T>(List<T> list) where T : class, IEntity;
		public abstract void WriteEntityList<T>(List<T> list, bool tidy) where T : class, IEntity;

		public abstract void WriteItemList<T>(List<T> list) where T : Item;
		public abstract void WriteItemList<T>(List<T> list, bool tidy) where T : Item;

		public abstract void WriteMobileList<T>(List<T> list) where T : Mobile;
		public abstract void WriteMobileList<T>(List<T> list, bool tidy) where T : Mobile;

		public abstract void WriteGuildList<T>(List<T> list) where T : BaseGuild;
		public abstract void WriteGuildList<T>(List<T> list, bool tidy) where T : BaseGuild;

		public abstract void Write<T>(HashSet<T> list, Action<GenericWriter, T> writer);

		public abstract void Write(HashSet<IEntity> list);
		public abstract void Write(HashSet<IEntity> list, bool tidy);

		public abstract void Write(HashSet<Item> list);
		public abstract void Write(HashSet<Item> list, bool tidy);

		public abstract void Write(HashSet<Mobile> list);
		public abstract void Write(HashSet<Mobile> list, bool tidy);

		public abstract void Write(HashSet<BaseGuild> list);
		public abstract void Write(HashSet<BaseGuild> list, bool tidy);

		public abstract void WriteEntitySet<T>(HashSet<T> set) where T : IEntity;
		public abstract void WriteEntitySet<T>(HashSet<T> set, bool tidy) where T : IEntity;

		public abstract void WriteItemSet<T>(HashSet<T> set) where T : Item;
		public abstract void WriteItemSet<T>(HashSet<T> set, bool tidy) where T : Item;

		public abstract void WriteMobileSet<T>(HashSet<T> set) where T : Mobile;
		public abstract void WriteMobileSet<T>(HashSet<T> set, bool tidy) where T : Mobile;

		public abstract void WriteGuildSet<T>(HashSet<T> set) where T : BaseGuild;
		public abstract void WriteGuildSet<T>(HashSet<T> set, bool tidy) where T : BaseGuild;
	}

	public class BinaryFileWriter : GenericWriter
	{
		private readonly bool PrefixStrings;
		private readonly Stream m_File;

		protected virtual int BufferSize => 81920;

		private readonly byte[] m_Buffer;

		private int m_Index;

		private readonly Encoding m_Encoding;

		public BinaryFileWriter(Stream strm, bool prefixStr)
		{
			PrefixStrings = prefixStr;

			m_Encoding = Utility.UTF8;
			m_Buffer = new byte[BufferSize];
			m_File = strm;
		}

		public BinaryFileWriter(string filename, bool prefixStr)
			: this(filename, prefixStr, false)
		{ }

		public BinaryFileWriter(string filename, bool prefixStr, bool async)
		{
			PrefixStrings = prefixStr;

			m_Buffer = new byte[BufferSize];

			if (async)
			{
				m_File = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, BufferSize, true);
			}
			else
			{
				m_File = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, BufferSize, FileOptions.WriteThrough);
			}

			m_Encoding = Utility.UTF8WithEncoding;
		}

		public void Flush()
		{
			if (m_Index > 0)
			{
				m_Position += m_Index;

				m_File.Write(m_Buffer, 0, m_Index);
				m_Index = 0;
			}
		}

		private long m_Position;

		public override long Position => m_Position + m_Index;

		public Stream UnderlyingStream
		{
			get
			{
				if (m_Index > 0)
				{
					Flush();
				}

				return m_File;
			}
		}

		public override void Close()
		{
			if (m_Index > 0)
			{
				Flush();
			}

			m_File.Close();
		}

		public override void WriteObjectType(object value)
		{
			WriteObjectType(value?.GetType());
		}

		public override void WriteObjectType(Type value)
		{
			var hash = ScriptCompiler.FindHashByFullName(value?.FullName);

			WriteEncodedInt(hash);
		}

		private byte[] m_CharacterBuffer;
		private int m_MaxBufferChars;
		private const int LargeByteBufferSize = 256;

		internal void InternalWriteString(string value)
		{
			var length = m_Encoding.GetByteCount(value);

			WriteEncodedInt(length);

			if (m_CharacterBuffer == null)
			{
				m_CharacterBuffer = new byte[LargeByteBufferSize];
				m_MaxBufferChars = LargeByteBufferSize / m_Encoding.GetMaxByteCount(1);
			}

			if (length > LargeByteBufferSize)
			{
				var current = 0;
				var charsLeft = value.Length;

				while (charsLeft > 0)
				{
					var charCount = (charsLeft > m_MaxBufferChars) ? m_MaxBufferChars : charsLeft;
					var byteLength = m_Encoding.GetBytes(value, current, charCount, m_CharacterBuffer, 0);

					if ((m_Index + byteLength) > m_Buffer.Length)
					{
						Flush();
					}

					Buffer.BlockCopy(m_CharacterBuffer, 0, m_Buffer, m_Index, byteLength);
					m_Index += byteLength;

					current += charCount;
					charsLeft -= charCount;
				}
			}
			else
			{
				var byteLength = m_Encoding.GetBytes(value, 0, value.Length, m_CharacterBuffer, 0);

				if ((m_Index + byteLength) > m_Buffer.Length)
				{
					Flush();
				}

				Buffer.BlockCopy(m_CharacterBuffer, 0, m_Buffer, m_Index, byteLength);
				m_Index += byteLength;
			}
		}

		public override void Write(string value)
		{
			if (PrefixStrings)
			{
				if (value == null)
				{
					if (m_Index + 1 > m_Buffer.Length)
					{
						Flush();
					}

					m_Buffer[m_Index++] = 0;
				}
				else
				{
					if (m_Index + 1 > m_Buffer.Length)
					{
						Flush();
					}

					m_Buffer[m_Index++] = 1;

					InternalWriteString(value);
				}
			}
			else
			{
				InternalWriteString(value);
			}
		}

		public override void WriteDeltaTime(DateTime value)
		{
			Write(value.Ticks - DateTime.UtcNow.Ticks);
		}

		public override void Write(DateTime value)
		{
			Write(value.Ticks);
		}

		public override void Write(DateTimeOffset value)
		{
			Write(value.Ticks);
			Write(value.Offset.Ticks);
		}

		public override void Write(TimeSpan value)
		{
			Write(value.Ticks);
		}

		public override void Write(Enum value)
		{
			WriteObjectType(value);

			if (value != null)
			{
				if ((int)value.GetTypeCode() % 2 == 1)
				{
					WriteEncodedLong(Convert.ToInt64(value));
				}
				else
				{
					WriteEncodedULong(Convert.ToUInt64(value));
				}
			}
			else
			{
				WriteEncodedULong(0UL);
			}
		}

		public override void Write(decimal value)
		{
			var bits = Decimal.GetBits(value);

			for (var i = 0; i < bits.Length; ++i)
			{
				Write(bits[i]);
			}
		}

		public override void Write(long value)
		{
			if ((m_Index + 8) > m_Buffer.Length)
			{
				Flush();
			}

			m_Buffer[m_Index] = (byte)value;
			m_Buffer[m_Index + 1] = (byte)(value >> 8);
			m_Buffer[m_Index + 2] = (byte)(value >> 16);
			m_Buffer[m_Index + 3] = (byte)(value >> 24);
			m_Buffer[m_Index + 4] = (byte)(value >> 32);
			m_Buffer[m_Index + 5] = (byte)(value >> 40);
			m_Buffer[m_Index + 6] = (byte)(value >> 48);
			m_Buffer[m_Index + 7] = (byte)(value >> 56);
			m_Index += 8;
		}

		public override void Write(ulong value)
		{
			if ((m_Index + 8) > m_Buffer.Length)
			{
				Flush();
			}

			m_Buffer[m_Index] = (byte)value;
			m_Buffer[m_Index + 1] = (byte)(value >> 8);
			m_Buffer[m_Index + 2] = (byte)(value >> 16);
			m_Buffer[m_Index + 3] = (byte)(value >> 24);
			m_Buffer[m_Index + 4] = (byte)(value >> 32);
			m_Buffer[m_Index + 5] = (byte)(value >> 40);
			m_Buffer[m_Index + 6] = (byte)(value >> 48);
			m_Buffer[m_Index + 7] = (byte)(value >> 56);
			m_Index += 8;
		}

		public override void Write(int value)
		{
			if ((m_Index + 4) > m_Buffer.Length)
			{
				Flush();
			}

			m_Buffer[m_Index] = (byte)value;
			m_Buffer[m_Index + 1] = (byte)(value >> 8);
			m_Buffer[m_Index + 2] = (byte)(value >> 16);
			m_Buffer[m_Index + 3] = (byte)(value >> 24);
			m_Index += 4;
		}

		public override void Write(uint value)
		{
			if ((m_Index + 4) > m_Buffer.Length)
			{
				Flush();
			}

			m_Buffer[m_Index] = (byte)value;
			m_Buffer[m_Index + 1] = (byte)(value >> 8);
			m_Buffer[m_Index + 2] = (byte)(value >> 16);
			m_Buffer[m_Index + 3] = (byte)(value >> 24);
			m_Index += 4;
		}

		public override void Write(short value)
		{
			if ((m_Index + 2) > m_Buffer.Length)
			{
				Flush();
			}

			m_Buffer[m_Index] = (byte)value;
			m_Buffer[m_Index + 1] = (byte)(value >> 8);
			m_Index += 2;
		}

		public override void Write(ushort value)
		{
			if ((m_Index + 2) > m_Buffer.Length)
			{
				Flush();
			}

			m_Buffer[m_Index] = (byte)value;
			m_Buffer[m_Index + 1] = (byte)(value >> 8);
			m_Index += 2;
		}

		public override unsafe void Write(double value)
		{
			if ((m_Index + 8) > m_Buffer.Length)
			{
				Flush();
			}

			fixed (byte* pBuffer = m_Buffer)
			{
				*(double*)(pBuffer + m_Index) = value;
			}

			m_Index += 8;
		}

		public override unsafe void Write(float value)
		{
			if ((m_Index + 4) > m_Buffer.Length)
			{
				Flush();
			}

			fixed (byte* pBuffer = m_Buffer)
			{
				*(float*)(pBuffer + m_Index) = value;
			}

			m_Index += 4;
		}

		private readonly char[] m_SingleCharBuffer = new char[1];

		public override void Write(char value)
		{
			if ((m_Index + 8) > m_Buffer.Length)
			{
				Flush();
			}

			m_SingleCharBuffer[0] = value;

			var byteCount = m_Encoding.GetBytes(m_SingleCharBuffer, 0, 1, m_Buffer, m_Index);

			m_Index += byteCount;
		}

		public override void Write(byte value)
		{
			if (m_Index + 1 > m_Buffer.Length)
			{
				Flush();
			}

			m_Buffer[m_Index++] = value;
		}

		public override void Write(sbyte value)
		{
			if (m_Index + 1 > m_Buffer.Length)
			{
				Flush();
			}

			m_Buffer[m_Index++] = (byte)value;
		}

		public override void Write(bool value)
		{
			if (m_Index + 1 > m_Buffer.Length)
			{
				Flush();
			}

			m_Buffer[m_Index++] = (byte)(value ? 1 : 0);
		}

		public override void WriteEncodedInt(int value)
		{
			WriteEncodedUInt((uint)value);
		}

		public override void WriteEncodedUInt(uint value)
		{
			while (value >= 0x80)
			{
				if (m_Index + 1 > m_Buffer.Length)
				{
					Flush();
				}

				m_Buffer[m_Index++] = (byte)(value | 0x80);

				value >>= 7;
			}

			if (m_Index + 1 > m_Buffer.Length)
			{
				Flush();
			}

			m_Buffer[m_Index++] = (byte)value;
		}

		public override void WriteEncodedLong(long value)
		{
			WriteEncodedULong((ulong)value);
		}

		public override void WriteEncodedULong(ulong value)
		{
			while (value >= 0x80)
			{
				if (m_Index + 1 > m_Buffer.Length)
				{
					Flush();
				}

				m_Buffer[m_Index++] = (byte)(value | 0x80);

				value >>= 7;
			}

			if (m_Index + 1 > m_Buffer.Length)
			{
				Flush();
			}

			m_Buffer[m_Index++] = (byte)value;
		}

		public override void Write(IPAddress value)
		{
			Write(Utility.GetLongAddressValue(value));
		}

		public override void Write(Point2D value)
		{
			Write(value.m_X);
			Write(value.m_Y);
		}

		public override void Write(Point3D value)
		{
			Write(value.m_X);
			Write(value.m_Y);
			Write(value.m_Z);
		}

		public override void Write(Rectangle2D value)
		{
			Write(value.Start);
			Write(value.End);
		}

		public override void Write(Rectangle3D value)
		{
			Write(value.Start);
			Write(value.End);
		}

		public override void Write(Map value)
		{
			if (value != null)
			{
				Write((byte)value.MapIndex);
			}
			else
			{
				Write((byte)0xFF);
			}
		}

		public override void Write(Race value)
		{
			if (value != null)
			{
				Write((byte)value.RaceIndex);
			}
			else
			{
				Write((byte)0xFF);
			}
		}

		public override void Write(Serial value)
		{
			Write(value.Value);
		}

		public override void Write(IEntity value)
		{
			if (value == null || value.Deleted)
			{
				Write(Serial.MinusOne);
			}
			else
			{
				Write(value.Serial);
			}
		}

		public override void Write(Item value)
		{
			if (value == null || value.Deleted)
			{
				Write(Serial.MinusOne);
			}
			else
			{
				Write(value.Serial);
			}
		}

		public override void Write(Mobile value)
		{
			if (value == null || value.Deleted)
			{
				Write(Serial.MinusOne);
			}
			else
			{
				Write(value.Serial);
			}
		}

		public override void Write(BaseGuild value)
		{
			if (value == null)
			{
				Write(0);
			}
			else
			{
				Write(value.Id);
			}
		}

		private void TidyObjectList<T>(ArrayList list, Predicate<T> tidy)
		{
			if (list == null)
			{
				return;
			}

			var i = list.Count;

			while (--i >= 0)
			{
				if (list[i] is T o && !tidy(o))
				{
					continue;
				}

				list.RemoveAt(i);
			}
		}

		public override void WriteObjectList<T>(ArrayList list, Action<GenericWriter, T> writer)
		{
			if (list == null)
			{
				Write((int)0);
			}
			else
			{
				Write(list.Count);

				foreach (T obj in list)
				{
					writer(this, obj);
				}
			}
		}

		public override void WriteEntityList(ArrayList list)
		{
			WriteObjectList<IEntity>(list, (w, o) => w.Write(o));
		}

		public override void WriteEntityList(ArrayList list, bool tidy)
		{
			if (tidy)
			{
				TidyObjectList<IEntity>(list, o => o.Deleted);
			}

			WriteEntityList(list);
		}

		public override void WriteItemList(ArrayList list)
		{
			WriteObjectList<Item>(list, (w, o) => w.Write(o));
		}

		public override void WriteItemList(ArrayList list, bool tidy)
		{
			if (tidy)
			{
				TidyObjectList<Item>(list, o => o.Deleted);
			}

			WriteItemList(list);
		}

		public override void WriteMobileList(ArrayList list)
		{
			WriteObjectList<Mobile>(list, (w, o) => w.Write(o));
		}

		public override void WriteMobileList(ArrayList list, bool tidy)
		{
			if (tidy)
			{
				TidyObjectList<Mobile>(list, o => o.Deleted);
			}

			WriteMobileList(list);
		}

		public override void WriteGuildList(ArrayList list)
		{
			WriteObjectList<BaseGuild>(list, (w, o) => w.Write(o));
		}

		public override void WriteGuildList(ArrayList list, bool tidy)
		{
			if (tidy)
			{
				TidyObjectList<BaseGuild>(list, o => o.Disbanded);
			}

			WriteGuildList(list);
		}

		public override void Write<T>(List<T> list, Action<GenericWriter, T> writer)
		{
			Write(list.Count);

			foreach (var obj in list)
			{
				writer(this, obj);
			}
		}

		public override void Write(List<IEntity> list)
		{
			Write(list, (w, o) => w.Write(o));
		}

		public override void Write(List<IEntity> list, bool tidy)
		{
			if (tidy)
			{
				list.RemoveAll(o => o?.Deleted != false);
			}

			Write(list);
		}

		public override void Write(List<Item> list)
		{
			Write(list, (w, o) => w.Write(o));
		}

		public override void Write(List<Item> list, bool tidy)
		{
			if (tidy)
			{
				list.RemoveAll(o => o?.Deleted != false);
			}

			Write(list);
		}

		public override void Write(List<Mobile> list)
		{
			Write(list, (w, o) => w.Write(o));
		}

		public override void Write(List<Mobile> list, bool tidy)
		{
			if (tidy)
			{
				list.RemoveAll(o => o?.Deleted != false);
			}

			Write(list);
		}

		public override void Write(List<BaseGuild> list)
		{
			Write(list, (w, o) => w.Write(o));
		}

		public override void Write(List<BaseGuild> list, bool tidy)
		{
			if (tidy)
			{
				list.RemoveAll(g => g?.Disbanded != false);
			}

			Write(list);
		}

		public override void WriteEntityList<T>(List<T> list)
		{
			Write(list, (w, o) => w.Write(o));
		}

		public override void WriteEntityList<T>(List<T> list, bool tidy)
		{
			if (tidy)
			{
				list.RemoveAll(o => o?.Deleted != false);
			}

			WriteEntityList(list);
		}

		public override void WriteItemList<T>(List<T> list)
		{
			Write(list, (w, o) => w.Write(o));
		}

		public override void WriteItemList<T>(List<T> list, bool tidy)
		{
			if (tidy)
			{
				list.RemoveAll(o => o?.Deleted != false);
			}

			WriteItemList(list);
		}

		public override void WriteMobileList<T>(List<T> list)
		{
			Write(list, (w, o) => w.Write(o));
		}

		public override void WriteMobileList<T>(List<T> list, bool tidy)
		{
			if (tidy)
			{
				list.RemoveAll(o => o?.Deleted != false);
			}

			WriteMobileList(list);
		}

		public override void WriteGuildList<T>(List<T> list)
		{
			Write(list, (w, o) => w.Write(o));
		}

		public override void WriteGuildList<T>(List<T> list, bool tidy)
		{
			if (tidy)
			{
				list.RemoveAll(o => o?.Disbanded != false);
			}

			WriteGuildList(list);
		}

		public override void Write<T>(HashSet<T> list, Action<GenericWriter, T> writer)
		{
			Write(list.Count);

			foreach (var obj in list)
			{
				writer(this, obj);
			}
		}

		public override void Write(HashSet<IEntity> set)
		{
			Write(set, (w, o) => w.Write(o));
		}

		public override void Write(HashSet<IEntity> set, bool tidy)
		{
			if (tidy)
			{
				set.RemoveWhere(o => o?.Deleted != false);
			}

			Write(set);
		}

		public override void Write(HashSet<Item> set)
		{
			Write(set, (w, o) => w.Write(o));
		}

		public override void Write(HashSet<Item> set, bool tidy)
		{
			if (tidy)
			{
				set.RemoveWhere(o => o?.Deleted != false);
			}

			Write(set);
		}

		public override void Write(HashSet<Mobile> set)
		{
			Write(set, (w, o) => w.Write(o));
		}

		public override void Write(HashSet<Mobile> set, bool tidy)
		{
			if (tidy)
			{
				set.RemoveWhere(o => o?.Deleted != false);
			}

			Write(set);
		}

		public override void Write(HashSet<BaseGuild> set)
		{
			Write(set, (w, o) => w.Write(o));
		}

		public override void Write(HashSet<BaseGuild> set, bool tidy)
		{
			if (tidy)
			{
				set.RemoveWhere(o => o?.Disbanded != false);
			}

			Write(set);
		}

		public override void WriteEntitySet<T>(HashSet<T> set)
		{
			Write(set, (w, o) => w.Write(o));
		}

		public override void WriteEntitySet<T>(HashSet<T> set, bool tidy)
		{
			if (tidy)
			{
				set.RemoveWhere(o => o?.Deleted != false);
			}

			WriteEntitySet(set);
		}

		public override void WriteItemSet<T>(HashSet<T> set)
		{
			Write(set, (w, o) => w.Write(o));
		}

		public override void WriteItemSet<T>(HashSet<T> set, bool tidy)
		{
			if (tidy)
			{
				set.RemoveWhere(o => o?.Deleted != false);
			}

			WriteItemSet(set);
		}

		public override void WriteMobileSet<T>(HashSet<T> set)
		{
			Write(set, (w, o) => w.Write(o));
		}

		public override void WriteMobileSet<T>(HashSet<T> set, bool tidy)
		{
			if (tidy)
			{
				set.RemoveWhere(o => o?.Deleted != false);
			}

			WriteMobileSet(set);
		}

		public override void WriteGuildSet<T>(HashSet<T> set)
		{
			Write(set, (w, o) => w.Write(o));
		}

		public override void WriteGuildSet<T>(HashSet<T> set, bool tidy)
		{
			if (tidy)
			{
				set.RemoveWhere(o => o?.Disbanded != false);
			}

			WriteGuildSet(set);
		}
	}

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

		public long Position => m_File.BaseStream.Position;

		public long Seek(long offset, SeekOrigin origin)
		{
			return m_File.BaseStream.Seek(offset, origin);
		}

		public override Type ReadObjectType()
		{
			var hash = ReadEncodedInt();

			return ScriptCompiler.FindTypeByFullNameHash(hash);
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
			var diff = ReadLong();

			try
			{
				return DateTime.UtcNow.AddTicks(diff);
			}
			catch
			{
				return DateTime.MaxValue;
			}
		}

		public override DateTime ReadDateTime()
		{
			return new DateTime(ReadLong());
		}

		public override DateTimeOffset ReadDateTimeOffset()
		{
			var ticks = ReadLong();
			var offset = new TimeSpan(ReadLong());

			return new DateTimeOffset(ticks, offset);
		}

		public override TimeSpan ReadTimeSpan()
		{
			return new TimeSpan(ReadLong());
		}

		public override Enum ReadEnum()
		{
			var type = ReadObjectType();

			var value = default(Enum);

			if (type?.IsEnum == true)
			{
				if ((int)Type.GetTypeCode(type) % 2 == 1)
				{
					value = (Enum)Enum.ToObject(type, ReadEncodedLong());
				}
				else
				{
					value = (Enum)Enum.ToObject(type, ReadEncodedULong());
				}
			}
			else
			{
				Skip(8);
			}

			return value;
		}

		public override T ReadEnum<T>()
		{
			return (T)ReadEnum();
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

		public override int ReadEncodedInt()
		{
			return unchecked((int)ReadEncodedUInt());
		}

		public override uint ReadEncodedUInt()
		{
			uint v = 0, b;
			var shift = 0;

			do
			{
				b = m_File.ReadByte();
				v |= (b & 0x7F) << shift;
				shift += 7;
			}
			while (b >= 0x80);

			return v;
		}

		public override long ReadEncodedLong()
		{
			return unchecked((long)ReadEncodedULong());
		}

		public override ulong ReadEncodedULong()
		{
			ulong v = 0, b;
			var shift = 0;

			do
			{
				b = m_File.ReadByte();
				v |= (b & 0x7F) << shift;
				shift += 7;
			}
			while (b >= 0x80);

			return v;
		}

		public override IPAddress ReadIPAddress()
		{
			return new IPAddress(ReadLong());
		}

		public override Point2D ReadPoint2D()
		{
			return new Point2D(ReadInt(), ReadInt());
		}

		public override Point3D ReadPoint3D()
		{
			return new Point3D(ReadInt(), ReadInt(), ReadInt());
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

		public override Race ReadRace()
		{
			return Race.Races[ReadByte()];
		}

		public override Serial ReadSerial()
		{
			return new Serial(ReadInt());
		}

		public override IEntity ReadEntity()
		{
			return World.FindEntity(ReadSerial());
		}

		public override Item ReadItem()
		{
			return World.FindItem(ReadSerial());
		}

		public override Mobile ReadMobile()
		{
			return World.FindMobile(ReadSerial());
		}

		public override BaseGuild ReadGuild()
		{
			return BaseGuild.Find(ReadInt());
		}

		public override T ReadEntity<T>()
		{
			return ReadEntity() as T;
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

		public override ArrayList ReadObjectList<T>(Func<T> reader)
		{
			var count = ReadInt();

			var list = new ArrayList(count);

			while (--count >= 0)
			{
				var obj = reader();

				if (obj != null)
				{
					list.Add(obj);
				}
			}

			list.TrimToSize();

			return list;
		}

		public override ArrayList ReadEntityList()
		{
			return ReadObjectList(ReadEntity);
		}

		public override ArrayList ReadItemList()
		{
			return ReadObjectList(ReadItem);
		}

		public override ArrayList ReadMobileList()
		{
			return ReadObjectList(ReadMobile);
		}

		public override ArrayList ReadGuildList()
		{
			return ReadObjectList(ReadGuild);
		}

		public override List<T> ReadStrongObjectList<T>(Func<T> reader)
		{
			var count = ReadInt();

			var list = new List<T>(count);

			while (--count >= 0)
			{
				var obj = reader();

				if (obj != null)
				{
					list.Add(obj);
				}
			}

			list.TrimExcess();

			return list;
		}

		public override List<IEntity> ReadStrongEntityList()
		{
			return ReadStrongObjectList(ReadEntity);
		}

		public override List<Item> ReadStrongItemList()
		{
			return ReadStrongObjectList(ReadItem);
		}

		public override List<Mobile> ReadStrongMobileList()
		{
			return ReadStrongObjectList(ReadMobile);
		}

		public override List<BaseGuild> ReadStrongGuildList()
		{
			return ReadStrongObjectList(ReadGuild);
		}

		public override List<T> ReadStrongEntityList<T>()
		{
			return ReadStrongObjectList(ReadEntity<T>);
		}

		public override List<T> ReadStrongItemList<T>()
		{
			return ReadStrongObjectList(ReadItem<T>);
		}

		public override List<T> ReadStrongMobileList<T>()
		{
			return ReadStrongObjectList(ReadMobile<T>);
		}

		public override List<T> ReadStrongGuildList<T>()
		{
			return ReadStrongObjectList(ReadGuild<T>);
		}

		public override HashSet<T> ReadObjectSet<T>(Func<T> reader)
		{
			var count = ReadInt();

			var list = new HashSet<T>(count);

			while (--count >= 0)
			{
				var obj = reader();

				if (obj != null)
				{
					list.Add(obj);
				}
			}

			list.TrimExcess();

			return list;
		}

		public override HashSet<IEntity> ReadEntitySet()
		{
			return ReadObjectSet(ReadEntity);
		}

		public override HashSet<Item> ReadItemSet()
		{
			return ReadObjectSet(ReadItem);
		}

		public override HashSet<Mobile> ReadMobileSet()
		{
			return ReadObjectSet(ReadMobile);
		}

		public override HashSet<BaseGuild> ReadGuildSet()
		{
			return ReadObjectSet(ReadGuild);
		}

		public override HashSet<T> ReadEntitySet<T>()
		{
			return ReadObjectSet(ReadEntity<T>);
		}

		public override HashSet<T> ReadItemSet<T>()
		{
			return ReadObjectSet(ReadItem<T>);
		}

		public override HashSet<T> ReadMobileSet<T>()
		{
			return ReadObjectSet(ReadMobile<T>);
		}

		public override HashSet<T> ReadGuildSet<T>()
		{
			return ReadObjectSet(ReadGuild<T>);
		}

		public override void Skip(int count)
		{
			while (--count >= 0)
			{
				_ = ReadByte();
			}
		}

		public override int PeekInt()
		{
			var stream = m_File.BaseStream;

			if (stream.Position + 4 > stream.Length)
			{
				return 0;
			}

			var returnTo = stream.Position;

			try
			{
				return m_File.ReadInt32();
			}
			finally
			{
				stream.Seek(returnTo, SeekOrigin.Begin);
			}
		}

		public override bool End()
		{
			return m_File.PeekChar() == -1;
		}
	}

	public sealed class AsyncWriter : GenericWriter
	{
		private static volatile int m_ThreadCount;

		public static int ThreadCount => m_ThreadCount;

		private readonly int m_BufferSize;
		private readonly bool m_PrefixStrings;

		private readonly FileStream m_File;

		private readonly ConcurrentQueue<MemoryStream> m_WriteQueue = new ConcurrentQueue<MemoryStream>();

		private MemoryStream m_Mem;
		private BinaryWriter m_Bin;

		private Thread m_WorkerThread;

		private bool m_Closed;

		private long m_LastPos, m_CurPos;

		public override long Position => m_CurPos;

		public MemoryStream MemStream
		{
			get => m_Mem;
			set
			{
				if (m_Mem.Length > 0)
				{
					Enqueue(m_Mem);
				}

				m_Mem = value;
				m_Bin = new BinaryWriter(m_Mem, Utility.UTF8WithEncoding);

				m_LastPos = 0;
				m_CurPos = m_Mem.Length;

				m_Mem.Seek(0, SeekOrigin.End);
			}
		}

		public AsyncWriter(string filename, bool prefix)
			: this(filename, 1048576, prefix) //1 mb buffer
		{ }

		public AsyncWriter(string filename, int buffSize, bool prefix)
		{
			m_BufferSize = buffSize;
			m_PrefixStrings = prefix;

			m_File = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, m_BufferSize, FileOptions.Asynchronous);

			m_Mem = new MemoryStream(m_BufferSize + 1024);
			m_Bin = new BinaryWriter(m_Mem, Utility.UTF8WithEncoding);
		}

		private void Enqueue(MemoryStream mem)
		{
			m_WriteQueue.Enqueue(mem);

			if (m_WorkerThread?.IsAlive != true)
			{
				m_WorkerThread = new Thread(WorkerThread.Work)
				{
					Priority = ThreadPriority.BelowNormal
				};

				m_WorkerThread.Start(this);
			}
		}

		private void OnWrite()
		{
			var curlen = m_Mem.Length;

			m_CurPos += curlen - m_LastPos;
			m_LastPos = curlen;

			if (curlen >= m_BufferSize)
			{
				Enqueue(m_Mem);

				m_Mem = new MemoryStream(m_BufferSize + 1024);
				m_Bin = new BinaryWriter(m_Mem, Utility.UTF8WithEncoding);

				m_LastPos = 0;
			}
		}

		public override void Close()
		{
			Enqueue(m_Mem);

			m_Closed = true;
		}

		public override void WriteObjectType(object value)
		{
			WriteObjectType(value?.GetType());
		}

		public override void WriteObjectType(Type value)
		{
			var hash = ScriptCompiler.FindHashByFullName(value?.FullName);

			WriteEncodedInt(hash);
		}

		public override void Write(string value)
		{
			if (m_PrefixStrings)
			{
				if (value == null)
				{
					m_Bin.Write((byte)0);
				}
				else
				{
					m_Bin.Write((byte)1);
					m_Bin.Write(value);
				}
			}
			else
			{
				m_Bin.Write(value);
			}

			OnWrite();
		}

		public override void WriteDeltaTime(DateTime value)
		{
			Write(value.Ticks - DateTime.UtcNow.Ticks);
		}

		public override void Write(DateTime value)
		{
			Write(value.Ticks);
		}

		public override void Write(DateTimeOffset value)
		{
			Write(value.Ticks);
			Write(value.Offset.Ticks);
		}

		public override void Write(TimeSpan value)
		{
			Write(value.Ticks);
		}

		public override void Write(Enum value)
		{
			WriteObjectType(value);

			if (value != null)
			{
				if ((int)value.GetTypeCode() % 2 == 1)
				{
					WriteEncodedLong(Convert.ToInt64(value));
				}
				else
				{
					WriteEncodedULong(Convert.ToUInt64(value));
				}
			}
			else
			{
				WriteEncodedULong(0UL);
			}
		}

		public override void Write(decimal value)
		{
			m_Bin.Write(value);

			OnWrite();
		}

		public override void Write(long value)
		{
			m_Bin.Write(value);

			OnWrite();
		}

		public override void Write(ulong value)
		{
			m_Bin.Write(value);

			OnWrite();
		}

		public override void Write(int value)
		{
			m_Bin.Write(value);

			OnWrite();
		}

		public override void Write(uint value)
		{
			m_Bin.Write(value);

			OnWrite();
		}

		public override void Write(short value)
		{
			m_Bin.Write(value);

			OnWrite();
		}

		public override void Write(ushort value)
		{
			m_Bin.Write(value);

			OnWrite();
		}

		public override void Write(double value)
		{
			m_Bin.Write(value);

			OnWrite();
		}

		public override void Write(float value)
		{
			m_Bin.Write(value);

			OnWrite();
		}

		public override void Write(char value)
		{
			m_Bin.Write(value);

			OnWrite();
		}

		public override void Write(byte value)
		{
			m_Bin.Write(value);

			OnWrite();
		}

		public override void Write(sbyte value)
		{
			m_Bin.Write(value);

			OnWrite();
		}

		public override void Write(bool value)
		{
			m_Bin.Write(value);

			OnWrite();
		}

		public override void WriteEncodedInt(int value)
		{
			WriteEncodedUInt((uint)value);
		}

		public override void WriteEncodedUInt(uint value)
		{
			while (value >= 0x80)
			{
				m_Bin.Write((byte)(value | 0x80));

				value >>= 7;
			}

			m_Bin.Write((byte)value);

			OnWrite();
		}

		public override void WriteEncodedLong(long value)
		{
			WriteEncodedULong((ulong)value);
		}

		public override void WriteEncodedULong(ulong value)
		{
			while (value >= 0x80)
			{
				m_Bin.Write((byte)(value | 0x80));

				value >>= 7;
			}

			m_Bin.Write((byte)value);

			OnWrite();
		}

		public override void Write(IPAddress value)
		{
			m_Bin.Write(Utility.GetLongAddressValue(value));
			OnWrite();
		}

		public override void Write(Point2D value)
		{
			Write(value.m_X);
			Write(value.m_Y);
		}

		public override void Write(Point3D value)
		{
			Write(value.m_X);
			Write(value.m_Y);
			Write(value.m_Z);
		}

		public override void Write(Rectangle2D value)
		{
			Write(value.Start);
			Write(value.End);
		}

		public override void Write(Rectangle3D value)
		{
			Write(value.Start);
			Write(value.End);
		}

		public override void Write(Map value)
		{
			if (value != null)
			{
				Write((byte)value.MapIndex);
			}
			else
			{
				Write((byte)0xFF);
			}
		}

		public override void Write(Race value)
		{
			if (value != null)
			{
				Write((byte)value.RaceIndex);
			}
			else
			{
				Write((byte)0xFF);
			}
		}

		public override void Write(Serial value)
		{
			Write(value.Value);
		}

		public override void Write(IEntity value)
		{
			if (value == null || value.Deleted)
			{
				Write(Serial.MinusOne);
			}
			else
			{
				Write(value.Serial);
			}
		}

		public override void Write(Item value)
		{
			if (value == null || value.Deleted)
			{
				Write(Serial.MinusOne);
			}
			else
			{
				Write(value.Serial);
			}
		}

		public override void Write(Mobile value)
		{
			if (value == null || value.Deleted)
			{
				Write(Serial.MinusOne);
			}
			else
			{
				Write(value.Serial);
			}
		}

		public override void Write(BaseGuild value)
		{
			if (value == null)
			{
				Write(0);
			}
			else
			{
				Write(value.Id);
			}
		}

		private void TidyObjectList<T>(ArrayList list, Predicate<T> tidy)
		{
			if (list == null)
			{
				return;
			}

			var i = list.Count;

			while (--i >= 0)
			{
				if (list[i] is T o && !tidy(o))
				{
					continue;
				}

				list.RemoveAt(i);
			}
		}

		public override void WriteObjectList<T>(ArrayList list, Action<GenericWriter, T> writer)
		{
			if (list == null)
			{
				Write((int)0);
			}
			else
			{
				Write(list.Count);

				foreach (T obj in list)
				{
					writer(this, obj);
				}
			}
		}

		public override void WriteEntityList(ArrayList list)
		{
			WriteObjectList<IEntity>(list, (w, o) => w.Write(o));
		}

		public override void WriteEntityList(ArrayList list, bool tidy)
		{
			if (tidy)
			{
				TidyObjectList<IEntity>(list, o => o.Deleted);
			}

			WriteEntityList(list);
		}

		public override void WriteItemList(ArrayList list)
		{
			WriteObjectList<Item>(list, (w, o) => w.Write(o));
		}

		public override void WriteItemList(ArrayList list, bool tidy)
		{
			if (tidy)
			{
				TidyObjectList<Item>(list, o => o.Deleted);
			}

			WriteItemList(list);
		}

		public override void WriteMobileList(ArrayList list)
		{
			WriteObjectList<Mobile>(list, (w, o) => w.Write(o));
		}

		public override void WriteMobileList(ArrayList list, bool tidy)
		{
			if (tidy)
			{
				TidyObjectList<Mobile>(list, o => o.Deleted);
			}

			WriteMobileList(list);
		}

		public override void WriteGuildList(ArrayList list)
		{
			WriteObjectList<BaseGuild>(list, (w, o) => w.Write(o));
		}

		public override void WriteGuildList(ArrayList list, bool tidy)
		{
			if (tidy)
			{
				TidyObjectList<BaseGuild>(list, o => o.Disbanded);
			}

			WriteGuildList(list);
		}

		public override void Write<T>(List<T> list, Action<GenericWriter, T> writer)
		{
			Write(list.Count);

			foreach (var obj in list)
			{
				writer(this, obj);
			}
		}

		public override void Write(List<IEntity> list)
		{
			Write(list, (w, o) => w.Write(o));
		}

		public override void Write(List<IEntity> list, bool tidy)
		{
			if (tidy)
			{
				list.RemoveAll(o => o?.Deleted != false);
			}

			Write(list);
		}

		public override void Write(List<Item> list)
		{
			Write(list, (w, o) => w.Write(o));
		}

		public override void Write(List<Item> list, bool tidy)
		{
			if (tidy)
			{
				list.RemoveAll(o => o?.Deleted != false);
			}

			Write(list);
		}

		public override void Write(List<Mobile> list)
		{
			Write(list, (w, o) => w.Write(o));
		}

		public override void Write(List<Mobile> list, bool tidy)
		{
			if (tidy)
			{
				list.RemoveAll(o => o?.Deleted != false);
			}

			Write(list);
		}

		public override void Write(List<BaseGuild> list)
		{
			Write(list, (w, o) => w.Write(o));
		}

		public override void Write(List<BaseGuild> list, bool tidy)
		{
			if (tidy)
			{
				list.RemoveAll(g => g?.Disbanded != false);
			}

			Write(list);
		}

		public override void WriteEntityList<T>(List<T> list)
		{
			Write(list, (w, o) => w.Write(o));
		}

		public override void WriteEntityList<T>(List<T> list, bool tidy)
		{
			if (tidy)
			{
				list.RemoveAll(o => o?.Deleted != false);
			}

			WriteEntityList(list);
		}

		public override void WriteItemList<T>(List<T> list)
		{
			Write(list, (w, o) => w.Write(o));
		}

		public override void WriteItemList<T>(List<T> list, bool tidy)
		{
			if (tidy)
			{
				list.RemoveAll(o => o?.Deleted != false);
			}

			WriteItemList(list);
		}

		public override void WriteMobileList<T>(List<T> list)
		{
			Write(list, (w, o) => w.Write(o));
		}

		public override void WriteMobileList<T>(List<T> list, bool tidy)
		{
			if (tidy)
			{
				list.RemoveAll(o => o?.Deleted != false);
			}

			WriteMobileList(list);
		}

		public override void WriteGuildList<T>(List<T> list)
		{
			Write(list, (w, o) => w.Write(o));
		}

		public override void WriteGuildList<T>(List<T> list, bool tidy)
		{
			if (tidy)
			{
				list.RemoveAll(o => o?.Disbanded != false);
			}

			WriteGuildList(list);
		}

		public override void Write<T>(HashSet<T> list, Action<GenericWriter, T> writer)
		{
			Write(list.Count);

			foreach (var obj in list)
			{
				writer(this, obj);
			}
		}

		public override void Write(HashSet<IEntity> set)
		{
			Write(set, (w, o) => w.Write(o));
		}

		public override void Write(HashSet<IEntity> set, bool tidy)
		{
			if (tidy)
			{
				set.RemoveWhere(o => o?.Deleted != false);
			}

			Write(set);
		}

		public override void Write(HashSet<Item> set)
		{
			Write(set, (w, o) => w.Write(o));
		}

		public override void Write(HashSet<Item> set, bool tidy)
		{
			if (tidy)
			{
				set.RemoveWhere(o => o?.Deleted != false);
			}

			Write(set);
		}

		public override void Write(HashSet<Mobile> set)
		{
			Write(set, (w, o) => w.Write(o));
		}

		public override void Write(HashSet<Mobile> set, bool tidy)
		{
			if (tidy)
			{
				set.RemoveWhere(o => o?.Deleted != false);
			}

			Write(set);
		}

		public override void Write(HashSet<BaseGuild> set)
		{
			Write(set, (w, o) => w.Write(o));
		}

		public override void Write(HashSet<BaseGuild> set, bool tidy)
		{
			if (tidy)
			{
				set.RemoveWhere(o => o?.Disbanded != false);
			}

			Write(set);
		}

		public override void WriteEntitySet<T>(HashSet<T> set)
		{
			Write(set, (w, o) => w.Write(o));
		}

		public override void WriteEntitySet<T>(HashSet<T> set, bool tidy)
		{
			if (tidy)
			{
				set.RemoveWhere(o => o?.Deleted != false);
			}

			WriteEntitySet(set);
		}

		public override void WriteItemSet<T>(HashSet<T> set)
		{
			Write(set, (w, o) => w.Write(o));
		}

		public override void WriteItemSet<T>(HashSet<T> set, bool tidy)
		{
			if (tidy)
			{
				set.RemoveWhere(o => o?.Deleted != false);
			}

			WriteItemSet(set);
		}

		public override void WriteMobileSet<T>(HashSet<T> set)
		{
			Write(set, (w, o) => w.Write(o));
		}

		public override void WriteMobileSet<T>(HashSet<T> set, bool tidy)
		{
			if (tidy)
			{
				set.RemoveWhere(o => o?.Deleted != false);
			}

			WriteMobileSet(set);
		}

		public override void WriteGuildSet<T>(HashSet<T> set)
		{
			Write(set, (w, o) => w.Write(o));
		}

		public override void WriteGuildSet<T>(HashSet<T> set, bool tidy)
		{
			if (tidy)
			{
				set.RemoveWhere(o => o?.Disbanded != false);
			}

			WriteGuildSet(set);
		}

		private static class WorkerThread
		{
			public static void Work(object owner)
			{
				Work((AsyncWriter)owner);
			}

			private static void Work(AsyncWriter owner)
			{
				Interlocked.Increment(ref m_ThreadCount);

				while (!owner.m_WriteQueue.IsEmpty)
				{
					if (owner.m_WriteQueue.TryDequeue(out var mem) && mem?.Length > 0)
					{
						mem.WriteTo(owner.m_File);
					}
				}

				if (owner.m_Closed)
				{
					owner.m_File.Close();
				}

				if (Interlocked.Decrement(ref m_ThreadCount) <= 0)
				{
					World.NotifyDiskWriteComplete();
				}
			}
		}
	}

	public interface ISerializable
	{
		int TypeReference { get; }
		int SerialIdentity { get; }

		void Serialize(GenericWriter writer);
	}
}
