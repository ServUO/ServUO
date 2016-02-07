#region Header
// **********
// ServUO - TileData.cs
// **********
#endregion

#region References
using System;
using System.IO;
using System.Text;
#endregion

namespace Server
{
	public struct LandData
	{
		private string m_Name;
		private TileFlag m_Flags;

		public LandData(string name, TileFlag flags)
		{
			m_Name = name;
			m_Flags = flags;
		}

		public string Name { get { return m_Name; } set { m_Name = value; } }

		public TileFlag Flags { get { return m_Flags; } set { m_Flags = value; } }
	}

	public struct ItemData
	{
		private string m_Name;
		private TileFlag m_Flags;
		private byte m_Weight;
		private byte m_Quality;
		private byte m_Quantity;
		private byte m_Value;
		private byte m_Height;

		public ItemData(string name, TileFlag flags, int weight, int quality, int quantity, int value, int height)
		{
			m_Name = name;
			m_Flags = flags;
			m_Weight = (byte)weight;
			m_Quality = (byte)quality;
			m_Quantity = (byte)quantity;
			m_Value = (byte)value;
			m_Height = (byte)height;
		}

		public string Name { get { return m_Name; } set { m_Name = value; } }

		public TileFlag Flags { get { return m_Flags; } set { m_Flags = value; } }

		public bool Bridge
		{
			get { return (m_Flags & TileFlag.Bridge) != 0; }
			set
			{
				if (value)
				{
					m_Flags |= TileFlag.Bridge;
				}
				else
				{
					m_Flags &= ~TileFlag.Bridge;
				}
			}
		}

		public bool Impassable
		{
			get { return (m_Flags & TileFlag.Impassable) != 0; }
			set
			{
				if (value)
				{
					m_Flags |= TileFlag.Impassable;
				}
				else
				{
					m_Flags &= ~TileFlag.Impassable;
				}
			}
		}

		public bool Surface
		{
			get { return (m_Flags & TileFlag.Surface) != 0; }
			set
			{
				if (value)
				{
					m_Flags |= TileFlag.Surface;
				}
				else
				{
					m_Flags &= ~TileFlag.Surface;
				}
			}
		}

		public int Weight { get { return m_Weight; } set { m_Weight = (byte)value; } }

		public int Quality { get { return m_Quality; } set { m_Quality = (byte)value; } }

		public int Quantity { get { return m_Quantity; } set { m_Quantity = (byte)value; } }

		public int Value { get { return m_Value; } set { m_Value = (byte)value; } }

		public int Height { get { return m_Height; } set { m_Height = (byte)value; } }

		public int CalcHeight
		{
			get
			{
				if ((m_Flags & TileFlag.Bridge) != 0)
				{
					return m_Height / 2;
				}
				else
				{
					return m_Height;
				}
			}
		}
	}

	[Flags]
	public enum TileFlag : long
	{
		None = 0x00000000,
		Background = 0x00000001,
		Weapon = 0x00000002,
		Transparent = 0x00000004,
		Translucent = 0x00000008,
		Wall = 0x00000010,
		Damaging = 0x00000020,
		Impassable = 0x00000040,
		Wet = 0x00000080,
		Unknown1 = 0x00000100,
		Surface = 0x00000200,
		Bridge = 0x00000400,
		Generic = 0x00000800,
		Window = 0x00001000,
		NoShoot = 0x00002000,
		ArticleA = 0x00004000,
		ArticleAn = 0x00008000,
		Internal = 0x00010000,
		Foliage = 0x00020000,
		PartialHue = 0x00040000,
		Unknown2 = 0x00080000,
		Map = 0x00100000,
		Container = 0x00200000,
		Wearable = 0x00400000,
		LightSource = 0x00800000,
		Animation = 0x01000000,
		NoDiagonal = 0x02000000,
		Unknown3 = 0x04000000,
		Armor = 0x08000000,
		Roof = 0x10000000,
		Door = 0x20000000,
		StairBack = 0x40000000,
		StairRight = 0x80000000
	}

	public static class TileData
	{
		private static readonly LandData[] m_LandData;
		private static readonly ItemData[] m_ItemData;

		public static LandData[] LandTable { get { return m_LandData; } }

		public static ItemData[] ItemTable { get { return m_ItemData; } }

		private static readonly int m_MaxLandValue;
		private static readonly int m_MaxItemValue;

		public static int MaxLandValue { get { return m_MaxLandValue; } }

		public static int MaxItemValue { get { return m_MaxItemValue; } }

		private static readonly byte[] m_StringBuffer = new byte[20];

		private static string ReadNameString(BinaryReader bin)
		{
			bin.Read(m_StringBuffer, 0, 20);

			int count;

			for (count = 0; count < 20 && m_StringBuffer[count] != 0; ++count)
			{
				;
			}

			return Encoding.ASCII.GetString(m_StringBuffer, 0, count);
		}

		static TileData()
		{
			string filePath = Core.FindDataFile("tiledata.mul");

			if (File.Exists(filePath))
			{
				using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					BinaryReader bin = new BinaryReader(fs);

					if (fs.Length >= 3188736)
					{
						// 7.0.9.0
						m_LandData = new LandData[0x4000];

						for (int i = 0; i < 0x4000; ++i)
						{
							if (i == 1 || (i > 0 && (i & 0x1F) == 0))
							{
								bin.ReadInt32(); // header
							}

							TileFlag flags = (TileFlag)bin.ReadInt64();
							bin.ReadInt16(); // skip 2 bytes -- textureID

							m_LandData[i] = new LandData(ReadNameString(bin), flags);
						}

						m_ItemData = new ItemData[0x10000];

						for (int i = 0; i < 0x10000; ++i)
						{
							if ((i & 0x1F) == 0)
							{
								bin.ReadInt32(); // header
							}

							TileFlag flags = (TileFlag)bin.ReadInt64();
							int weight = bin.ReadByte();
							int quality = bin.ReadByte();
							bin.ReadInt16();
							bin.ReadByte();
							int quantity = bin.ReadByte();
							bin.ReadInt32();
							bin.ReadByte();
							int value = bin.ReadByte();
							int height = bin.ReadByte();

							m_ItemData[i] = new ItemData(ReadNameString(bin), flags, weight, quality, quantity, value, height);
						}
					}
					else
					{
						m_LandData = new LandData[0x4000];

						for (int i = 0; i < 0x4000; ++i)
						{
							if ((i & 0x1F) == 0)
							{
								bin.ReadInt32(); // header
							}

							TileFlag flags = (TileFlag)bin.ReadInt32();
							bin.ReadInt16(); // skip 2 bytes -- textureID

							m_LandData[i] = new LandData(ReadNameString(bin), flags);
						}

						if (fs.Length == 1644544)
						{
							// 7.0.0.0
							m_ItemData = new ItemData[0x8000];

							for (int i = 0; i < 0x8000; ++i)
							{
								if ((i & 0x1F) == 0)
								{
									bin.ReadInt32(); // header
								}

								TileFlag flags = (TileFlag)bin.ReadInt32();
								int weight = bin.ReadByte();
								int quality = bin.ReadByte();
								bin.ReadInt16();
								bin.ReadByte();
								int quantity = bin.ReadByte();
								bin.ReadInt32();
								bin.ReadByte();
								int value = bin.ReadByte();
								int height = bin.ReadByte();

								m_ItemData[i] = new ItemData(ReadNameString(bin), flags, weight, quality, quantity, value, height);
							}
						}
						else
						{
							m_ItemData = new ItemData[0x4000];

							for (int i = 0; i < 0x4000; ++i)
							{
								if ((i & 0x1F) == 0)
								{
									bin.ReadInt32(); // header
								}

								TileFlag flags = (TileFlag)bin.ReadInt32();
								int weight = bin.ReadByte();
								int quality = bin.ReadByte();
								bin.ReadInt16();
								bin.ReadByte();
								int quantity = bin.ReadByte();
								bin.ReadInt32();
								bin.ReadByte();
								int value = bin.ReadByte();
								int height = bin.ReadByte();

								m_ItemData[i] = new ItemData(ReadNameString(bin), flags, weight, quality, quantity, value, height);
							}
						}
					}
				}

				m_MaxLandValue = m_LandData.Length - 1;
				m_MaxItemValue = m_ItemData.Length - 1;
			}
			else
			{
				Console.WriteLine("tiledata.mul was not found");
				Console.WriteLine("Make sure your Scripts/Misc/DataPath.cs is properly configured");
				Console.WriteLine("After pressing return an exception will be thrown and the server will terminate");

				throw new Exception(String.Format("TileData: {0} not found", filePath));
			}
		}
	}
}