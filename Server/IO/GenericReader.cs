#region Header
// **********
// ServUO - Serialization.cs
// **********
#endregion

#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using CustomsFramework;

using Server.Guilds;
#endregion

namespace Server
{
	public abstract class GenericReader
	{
		public abstract string ReadString();
		public abstract DateTime ReadDateTime();
		public abstract DateTimeOffset ReadDateTimeOffset();
		public abstract TimeSpan ReadTimeSpan();
		public abstract DateTime ReadDeltaTime();
		public abstract decimal ReadDecimal();
		public abstract long ReadLong();
		public abstract ulong ReadULong();
		public abstract int PeekInt();
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
		public abstract IPAddress ReadIPAddress();

		public abstract Point3D ReadPoint3D();
		public abstract Point2D ReadPoint2D();
		public abstract Rectangle2D ReadRect2D();
		public abstract Rectangle3D ReadRect3D();
		public abstract Map ReadMap();

		public abstract Item ReadItem();
		public abstract Mobile ReadMobile();
		public abstract BaseGuild ReadGuild();
		public abstract SaveData ReadData();

		public abstract T ReadItem<T>() where T : Item;
		public abstract T ReadMobile<T>() where T : Mobile;
		public abstract T ReadGuild<T>() where T : BaseGuild;
		public abstract T ReadData<T>() where T : SaveData;

		public abstract ArrayList ReadItemList();
		public abstract ArrayList ReadMobileList();
		public abstract ArrayList ReadGuildList();
		public abstract ArrayList ReadDataList();

		public abstract List<Item> ReadStrongItemList();
		public abstract List<T> ReadStrongItemList<T>() where T : Item;

		public abstract List<Mobile> ReadStrongMobileList();
		public abstract List<T> ReadStrongMobileList<T>() where T : Mobile;

		public abstract List<BaseGuild> ReadStrongGuildList();
		public abstract List<T> ReadStrongGuildList<T>() where T : BaseGuild;

		public abstract List<SaveData> ReadStrongDataList();
		public abstract List<T> ReadStrongDataList<T>() where T : SaveData;

		public abstract HashSet<Item> ReadItemSet();
		public abstract HashSet<T> ReadItemSet<T>() where T : Item;

		public abstract HashSet<Mobile> ReadMobileSet();
		public abstract HashSet<T> ReadMobileSet<T>() where T : Mobile;

		public abstract HashSet<BaseGuild> ReadGuildSet();
		public abstract HashSet<T> ReadGuildSet<T>() where T : BaseGuild;

		public abstract HashSet<SaveData> ReadDataSet();
		public abstract HashSet<T> ReadDataSet<T>() where T : SaveData;

		public abstract Race ReadRace();

		public abstract bool End();
	}
}