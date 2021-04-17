#region References
using System;
using System.IO;
using System.Text;
#endregion

namespace Server
{
	public struct LandData
	{
		public string Name { get; set; }
		public TileFlag Flags { get; set; }

		public LandData(string name, TileFlag flags)
		{
			Name = name;
			Flags = flags;
		}
	}

	[PropertyObject]
	public struct ItemData
	{
		private byte m_Weight;
		private byte m_Quality;
		private ushort m_Animation;
		private byte m_Quantity;
		private byte m_Value;
		private byte m_Height;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public string Name { get; set; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Developer)]
		public TileFlag Flags { get; set; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public int Weight { get => m_Weight; set => m_Weight = (byte)value; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public int Quality { get => m_Quality; set => m_Quality = (byte)value; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public int Animation { get => m_Animation; set => m_Animation = (ushort)value; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public int Quantity { get => m_Quantity; set => m_Quantity = (byte)value; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public int Value { get => m_Value; set => m_Value = (byte)value; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public int Height { get => m_Height; set => m_Height = (byte)value; }

		// computed properties

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public int CalcHeight => Bridge ? m_Height / 2 : m_Height;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public bool Bridge { get => this[TileFlag.Bridge]; set => this[TileFlag.Bridge] = value; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public bool Wall { get => this[TileFlag.Wall]; set => this[TileFlag.Wall] = value; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public bool Window { get => this[TileFlag.Window]; set => this[TileFlag.Window] = value; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public bool Impassable { get => this[TileFlag.Impassable]; set => this[TileFlag.Impassable] = value; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public bool Surface { get => this[TileFlag.Surface]; set => this[TileFlag.Surface] = value; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public bool Roof { get => this[TileFlag.Roof]; set => this[TileFlag.Roof] = value; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public bool LightSource { get => this[TileFlag.LightSource]; set => this[TileFlag.LightSource] = value; }

		public bool this[TileFlag flag]
		{
			get => (Flags & flag) != 0;
			set
			{
				if (value)
					Flags |= flag;
				else
					Flags &= ~flag;
			}
		}

		public ItemData(ItemData d)
			: this(d.Name, d.Flags, d.Weight, d.Quality, d.Animation, d.Quantity, d.Value, d.Height)
		{ }

		public ItemData(string name, TileFlag flags, int weight, int quality, int animation, int quantity, int value, int height)
		{
			Name = name;
			Flags = flags;

			m_Weight = (byte)weight;
			m_Quality = (byte)quality;
			m_Animation = (ushort)animation;
			m_Quantity = (byte)quantity;
			m_Value = (byte)value;
			m_Height = (byte)height;
		}

		public ItemData(GenericReader reader)
			: this()
		{
			Deserialize(reader);
		}

		public void Serialize(GenericWriter writer)
		{
			writer.WriteEncodedInt(0);

			writer.Write(Name);

			writer.Write((ulong)Flags);

			writer.Write(m_Weight);
			writer.Write(m_Quality);
			writer.Write(m_Animation);
			writer.Write(m_Quantity);
			writer.Write(m_Value);
			writer.Write(m_Height);
		}

		public void Deserialize(GenericReader reader)
		{
			reader.ReadEncodedInt();

			Name = reader.ReadString();

			Flags = (TileFlag)reader.ReadULong();

			m_Weight = reader.ReadByte();
			m_Quality = reader.ReadByte();
			m_Animation = reader.ReadUShort();
			m_Quantity = reader.ReadByte();
			m_Value = reader.ReadByte();
			m_Height = reader.ReadByte();
		}
	}

	[Flags]
	public enum TileFlag : ulong
	{
		None = 0x0000000000000000,

		Background = 0x0000000000000001,
		Weapon = 0x0000000000000002,
		Transparent = 0x0000000000000004,
		Translucent = 0x0000000000000008,
		Wall = 0x0000000000000010,
		Damaging = 0x0000000000000020,
		Impassable = 0x0000000000000040,
		Wet = 0x0000000000000080,
		Unknown1 = 0x0000000000000100,
		Surface = 0x0000000000000200,
		Bridge = 0x0000000000000400,
		Generic = 0x0000000000000800,
		Window = 0x0000000000001000,
		NoShoot = 0x0000000000002000,
		ArticleA = 0x0000000000004000,
		ArticleAn = 0x0000000000008000,
		Internal = 0x0000000000010000,
		Foliage = 0x0000000000020000,
		PartialHue = 0x0000000000040000,
		Unknown2 = 0x0000000000080000,
		Map = 0x0000000000100000,
		Container = 0x0000000000200000,
		Wearable = 0x0000000000400000,
		LightSource = 0x0000000000800000,
		Animation = 0x0000000001000000,
		HoverOver = 0x0000000002000000,
		Unknown3 = 0x0000000004000000,
		Armor = 0x0000000008000000,
		Roof = 0x0000000010000000,
		Door = 0x0000000020000000,
		StairBack = 0x0000000040000000,
		StairRight = 0x0000000080000000,

		HS33 = 0x0000000100000000,
		HS34 = 0x0000000200000000,
		HS35 = 0x0000000400000000,
		HS36 = 0x0000000800000000,
		HS37 = 0x0000001000000000,
		HS38 = 0x0000002000000000,
		HS39 = 0x0000004000000000,
		HS40 = 0x0000008000000000,
		HS41 = 0x0000010000000000,
		HS42 = 0x0000020000000000,
		HS43 = 0x0000040000000000,
		HS44 = 0x0000080000000000,
		HS45 = 0x0000100000000000,
		HS46 = 0x0000200000000000,
		HS47 = 0x0000400000000000,
		HS48 = 0x0000800000000000,
		HS49 = 0x0001000000000000,
		HS50 = 0x0002000000000000,
		HS51 = 0x0004000000000000,
		HS52 = 0x0008000000000000,
		HS53 = 0x0010000000000000,
		HS54 = 0x0020000000000000,
		HS55 = 0x0040000000000000,
		HS56 = 0x0080000000000000,
		HS57 = 0x0100000000000000,
		HS58 = 0x0200000000000000,
		HS59 = 0x0400000000000000,
		HS60 = 0x0800000000000000,
		HS61 = 0x1000000000000000,
		HS62 = 0x2000000000000000,
		HS63 = 0x4000000000000000,
		HS64 = 0x8000000000000000
	}

	public static class TileData
	{
		public static LandData[] LandTable { get; } = new LandData[0x4000];
		public static ItemData[] ItemTable { get; } = new ItemData[0x10000];

		public static int MaxLandValue { get; private set; }
		public static int MaxItemValue { get; private set; }

		public static bool Is64BitFlags { get; private set; }

		private static readonly byte[] m_StringBuffer = new byte[20];

		private static string ReadNameString(BinaryReader bin)
		{
			bin.Read(m_StringBuffer, 0, 20);

			var count = 0;

			while (count < m_StringBuffer.Length && m_StringBuffer[count] != 0)
			{
				++count;
			}

			return Encoding.ASCII.GetString(m_StringBuffer, 0, count);
		}

		static TileData()
		{
			Load();
		}

		public static void Load()
		{
			Is64BitFlags = false;

			if (MaxLandValue > 0)
			{
				Array.Clear(LandTable, 0, MaxLandValue);

				MaxLandValue = 0;
			}

			if (MaxItemValue > 0)
			{
				Array.Clear(ItemTable, 0, MaxItemValue);

				MaxItemValue = 0;
			}

			var flags64bit = false;
			var landLength = 0;
			var itemLength = 0;

			var filePath = Core.FindDataFile("tiledata.mul");

			if (File.Exists(filePath))
			{
				using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					var bin = new BinaryReader(fs);

					if (fs.Length >= 3188736) // 7.0.9.0
					{
						flags64bit = true;
						landLength = 0x4000;
						itemLength = 0x10000;
					}
					else if (fs.Length >= 1644544) // 7.0.0.0
					{
						flags64bit = false;
						landLength = 0x4000;
						itemLength = 0x8000;
					}
					else
					{
						flags64bit = false;
						landLength = 0x4000;
						itemLength = 0x4000;
					}

					for (var i = 0; i < landLength; ++i)
					{
						if (flags64bit ? (i == 1 || (i > 0 && (i & 0x1F) == 0)) : ((i & 0x1F) == 0))
						{
							bin.ReadInt32(); // header
						}

						var flags = (TileFlag)(flags64bit ? bin.ReadUInt64() : bin.ReadUInt32());

						bin.ReadInt16(); // skip 2 bytes -- textureID

						LandTable[i] = new LandData(ReadNameString(bin), flags);
					}

					for (var i = 0; i < itemLength; ++i)
					{
						if ((i & 0x1F) == 0)
						{
							bin.ReadInt32(); // header
						}

						var flags = (TileFlag)(flags64bit ? bin.ReadUInt64() : bin.ReadUInt32());
						var weight = bin.ReadByte();
						var quality = bin.ReadByte();
						var anim = bin.ReadUInt16();
						bin.ReadByte();
						var quantity = bin.ReadByte();
						bin.ReadInt32();
						bin.ReadByte();
						var value = bin.ReadByte();
						var height = bin.ReadByte();

						ItemTable[i] = new ItemData(ReadNameString(bin), flags, weight, quality, anim, quantity, value, height);
					}
				}

				Is64BitFlags = flags64bit;

				MaxLandValue = landLength - 1;
				MaxItemValue = itemLength - 1;
			}
			else
			{
				throw new Exception($"TileData: {filePath} not found");
			}
		}
	}
}
