#region Header
// **********
// ServUO - MultiData.cs
// **********
#endregion

#region References
using System;
using System.IO;
#endregion

namespace Server
{
	public static class MultiData
	{
		private static readonly MultiComponentList[] m_Components;

		private static readonly FileStream m_Index;
		private static readonly FileStream m_Stream;
		private static readonly BinaryReader m_IndexReader;
		private static readonly BinaryReader m_StreamReader;

		public static MultiComponentList GetComponents(int multiID)
		{
			MultiComponentList mcl;

			#region Stygian Abyss
			multiID &= 0x3FFF; // The value of the actual multi is shifted by 0x4000, so this is left alone.
			#endregion

			if (multiID >= 0 && multiID < m_Components.Length)
			{
				mcl = m_Components[multiID];

				if (mcl == null)
				{
					m_Components[multiID] = mcl = Load(multiID);
				}
			}
			else
			{
				mcl = MultiComponentList.Empty;
			}

			return mcl;
		}

		public static MultiComponentList Load(int multiID)
		{
			try
			{
				m_IndexReader.BaseStream.Seek(multiID * 12, SeekOrigin.Begin);

				int lookup = m_IndexReader.ReadInt32();
				int length = m_IndexReader.ReadInt32();

				if (lookup < 0 || length <= 0)
				{
					return MultiComponentList.Empty;
				}

				m_StreamReader.BaseStream.Seek(lookup, SeekOrigin.Begin);

				return new MultiComponentList(m_StreamReader, length / (MultiComponentList.PostHSFormat ? 16 : 12));
			}
			catch
			{
				return MultiComponentList.Empty;
			}
		}

		static MultiData()
		{
			string idxPath = Core.FindDataFile("multi.idx");
			string mulPath = Core.FindDataFile("multi.mul");

			if (File.Exists(idxPath) && File.Exists(mulPath))
			{
				m_Index = new FileStream(idxPath, FileMode.Open, FileAccess.Read, FileShare.Read);
				m_IndexReader = new BinaryReader(m_Index);

				m_Stream = new FileStream(mulPath, FileMode.Open, FileAccess.Read, FileShare.Read);
				m_StreamReader = new BinaryReader(m_Stream);

				m_Components = new MultiComponentList[(int)(m_Index.Length / 12)];

				string vdPath = Core.FindDataFile("verdata.mul");

				if (File.Exists(vdPath))
				{
					using (FileStream fs = new FileStream(vdPath, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						BinaryReader bin = new BinaryReader(fs);

						int count = bin.ReadInt32();

						for (int i = 0; i < count; ++i)
						{
							int file = bin.ReadInt32();
							int index = bin.ReadInt32();
							int lookup = bin.ReadInt32();
							int length = bin.ReadInt32();
							int extra = bin.ReadInt32();

							if (file == 14 && index >= 0 && index < m_Components.Length && lookup >= 0 && length > 0)
							{
								bin.BaseStream.Seek(lookup, SeekOrigin.Begin);

								m_Components[index] = new MultiComponentList(bin, length / 12);

								bin.BaseStream.Seek(24 + (i * 20), SeekOrigin.Begin);
							}
						}

						bin.Close();
					}
				}
			}
			else
			{
				Console.WriteLine("Warning: Multi data files not found");

				m_Components = new MultiComponentList[0];
			}
		}
	}

	public struct MultiTileEntry
	{
		public ushort m_ItemID;
		public short m_OffsetX, m_OffsetY, m_OffsetZ;
		public int m_Flags;

		public MultiTileEntry(ushort itemID, short xOffset, short yOffset, short zOffset, int flags)
		{
			m_ItemID = itemID;
			m_OffsetX = xOffset;
			m_OffsetY = yOffset;
			m_OffsetZ = zOffset;
			m_Flags = flags;
		}
	}

	public sealed class MultiComponentList
	{
		public static bool PostHSFormat { get { return _PostHSFormat; } set { _PostHSFormat = value; } }

		private static bool _PostHSFormat;

		private Point2D m_Min, m_Max, m_Center;
		private int m_Width, m_Height;
		private StaticTile[][][] m_Tiles;
		private MultiTileEntry[] m_List;

		public static readonly MultiComponentList Empty = new MultiComponentList();

		public Point2D Min { get { return m_Min; } }
		public Point2D Max { get { return m_Max; } }

		public Point2D Center { get { return m_Center; } }

		public int Width { get { return m_Width; } }
		public int Height { get { return m_Height; } }

		public StaticTile[][][] Tiles { get { return m_Tiles; } }
		public MultiTileEntry[] List { get { return m_List; } }

		public void Add(int itemID, int x, int y, int z)
		{
			#region Stygian Abyss
			itemID &= TileData.MaxItemValue;
			itemID |= 0x10000;
			#endregion

			int vx = x + m_Center.m_X;
			int vy = y + m_Center.m_Y;

			if (vx >= 0 && vx < m_Width && vy >= 0 && vy < m_Height)
			{
				var oldTiles = m_Tiles[vx][vy];

				for (int i = oldTiles.Length - 1; i >= 0; --i)
				{
					ItemData data = TileData.ItemTable[itemID & TileData.MaxItemValue];

					if (oldTiles[i].Z == z && (oldTiles[i].Height > 0 == data.Height > 0))
					{
						bool newIsRoof = (data.Flags & TileFlag.Roof) != 0;
						bool oldIsRoof = (TileData.ItemTable[oldTiles[i].ID & TileData.MaxItemValue].Flags & TileFlag.Roof) != 0;

						if (newIsRoof == oldIsRoof)
						{
							Remove(oldTiles[i].ID, x, y, z);
						}
					}
				}

				oldTiles = m_Tiles[vx][vy];

				var newTiles = new StaticTile[oldTiles.Length + 1];

				for (int i = 0; i < oldTiles.Length; ++i)
				{
					newTiles[i] = oldTiles[i];
				}

				newTiles[oldTiles.Length] = new StaticTile((ushort)itemID, (sbyte)z);

				m_Tiles[vx][vy] = newTiles;

				var oldList = m_List;
				var newList = new MultiTileEntry[oldList.Length + 1];

				for (int i = 0; i < oldList.Length; ++i)
				{
					newList[i] = oldList[i];
				}

				newList[oldList.Length] = new MultiTileEntry((ushort)itemID, (short)x, (short)y, (short)z, 1);

				m_List = newList;

				if (x < m_Min.m_X)
				{
					m_Min.m_X = x;
				}

				if (y < m_Min.m_Y)
				{
					m_Min.m_Y = y;
				}

				if (x > m_Max.m_X)
				{
					m_Max.m_X = x;
				}

				if (y > m_Max.m_Y)
				{
					m_Max.m_Y = y;
				}
			}
		}

		public void RemoveXYZH(int x, int y, int z, int minHeight)
		{
			int vx = x + m_Center.m_X;
			int vy = y + m_Center.m_Y;

			if (vx >= 0 && vx < m_Width && vy >= 0 && vy < m_Height)
			{
				var oldTiles = m_Tiles[vx][vy];

				for (int i = 0; i < oldTiles.Length; ++i)
				{
					StaticTile tile = oldTiles[i];

					if (tile.Z == z && tile.Height >= minHeight)
					{
						var newTiles = new StaticTile[oldTiles.Length - 1];

						for (int j = 0; j < i; ++j)
						{
							newTiles[j] = oldTiles[j];
						}

						for (int j = i + 1; j < oldTiles.Length; ++j)
						{
							newTiles[j - 1] = oldTiles[j];
						}

						m_Tiles[vx][vy] = newTiles;

						break;
					}
				}

				var oldList = m_List;

				for (int i = 0; i < oldList.Length; ++i)
				{
					MultiTileEntry tile = oldList[i];

					if (tile.m_OffsetX == (short)x && tile.m_OffsetY == (short)y && tile.m_OffsetZ == (short)z &&
						TileData.ItemTable[tile.m_ItemID & TileData.MaxItemValue].Height >= minHeight)
					{
						var newList = new MultiTileEntry[oldList.Length - 1];

						for (int j = 0; j < i; ++j)
						{
							newList[j] = oldList[j];
						}

						for (int j = i + 1; j < oldList.Length; ++j)
						{
							newList[j - 1] = oldList[j];
						}

						m_List = newList;

						break;
					}
				}
			}
		}

		public void Remove(int itemID, int x, int y, int z)
		{
			int vx = x + m_Center.m_X;
			int vy = y + m_Center.m_Y;

			if (vx >= 0 && vx < m_Width && vy >= 0 && vy < m_Height)
			{
				var oldTiles = m_Tiles[vx][vy];

				for (int i = 0; i < oldTiles.Length; ++i)
				{
					StaticTile tile = oldTiles[i];

					if (tile.ID == itemID && tile.Z == z)
					{
						var newTiles = new StaticTile[oldTiles.Length - 1];

						for (int j = 0; j < i; ++j)
						{
							newTiles[j] = oldTiles[j];
						}

						for (int j = i + 1; j < oldTiles.Length; ++j)
						{
							newTiles[j - 1] = oldTiles[j];
						}

						m_Tiles[vx][vy] = newTiles;

						break;
					}
				}

				var oldList = m_List;

				for (int i = 0; i < oldList.Length; ++i)
				{
					MultiTileEntry tile = oldList[i];

					if (tile.m_ItemID == itemID && tile.m_OffsetX == (short)x && tile.m_OffsetY == (short)y &&
						tile.m_OffsetZ == (short)z)
					{
						var newList = new MultiTileEntry[oldList.Length - 1];

						for (int j = 0; j < i; ++j)
						{
							newList[j] = oldList[j];
						}

						for (int j = i + 1; j < oldList.Length; ++j)
						{
							newList[j - 1] = oldList[j];
						}

						m_List = newList;

						break;
					}
				}
			}
		}

		public void Resize(int newWidth, int newHeight)
		{
			int oldWidth = m_Width, oldHeight = m_Height;
			var oldTiles = m_Tiles;

			int totalLength = 0;

			var newTiles = new StaticTile[newWidth][][];

			for (int x = 0; x < newWidth; ++x)
			{
				newTiles[x] = new StaticTile[newHeight][];

				for (int y = 0; y < newHeight; ++y)
				{
					if (x < oldWidth && y < oldHeight)
					{
						newTiles[x][y] = oldTiles[x][y];
					}
					else
					{
						newTiles[x][y] = new StaticTile[0];
					}

					totalLength += newTiles[x][y].Length;
				}
			}

			m_Tiles = newTiles;
			m_List = new MultiTileEntry[totalLength];
			m_Width = newWidth;
			m_Height = newHeight;

			m_Min = Point2D.Zero;
			m_Max = Point2D.Zero;

			int index = 0;

			for (int x = 0; x < newWidth; ++x)
			{
				for (int y = 0; y < newHeight; ++y)
				{
					var tiles = newTiles[x][y];

					for (int i = 0; i < tiles.Length; ++i)
					{
						StaticTile tile = tiles[i];

						int vx = x - m_Center.X;
						int vy = y - m_Center.Y;

						if (vx < m_Min.m_X)
						{
							m_Min.m_X = vx;
						}

						if (vy < m_Min.m_Y)
						{
							m_Min.m_Y = vy;
						}

						if (vx > m_Max.m_X)
						{
							m_Max.m_X = vx;
						}

						if (vy > m_Max.m_Y)
						{
							m_Max.m_Y = vy;
						}

						m_List[index++] = new MultiTileEntry((ushort)tile.ID, (short)vx, (short)vy, (short)tile.Z, 1);
					}
				}
			}
		}

		public MultiComponentList(MultiComponentList toCopy)
		{
			m_Min = toCopy.m_Min;
			m_Max = toCopy.m_Max;

			m_Center = toCopy.m_Center;

			m_Width = toCopy.m_Width;
			m_Height = toCopy.m_Height;

			m_Tiles = new StaticTile[m_Width][][];

			for (int x = 0; x < m_Width; ++x)
			{
				m_Tiles[x] = new StaticTile[m_Height][];

				for (int y = 0; y < m_Height; ++y)
				{
					m_Tiles[x][y] = new StaticTile[toCopy.m_Tiles[x][y].Length];

					for (int i = 0; i < m_Tiles[x][y].Length; ++i)
					{
						m_Tiles[x][y][i] = toCopy.m_Tiles[x][y][i];
					}
				}
			}

			m_List = new MultiTileEntry[toCopy.m_List.Length];

			for (int i = 0; i < m_List.Length; ++i)
			{
				m_List[i] = toCopy.m_List[i];
			}
		}

		public void Serialize(GenericWriter writer)
		{
			writer.Write(1); // version;

			writer.Write(m_Min);
			writer.Write(m_Max);
			writer.Write(m_Center);

			writer.Write(m_Width);
			writer.Write(m_Height);

			writer.Write(m_List.Length);

			for (int i = 0; i < m_List.Length; ++i)
			{
				MultiTileEntry ent = m_List[i];

				writer.Write(ent.m_ItemID);
				writer.Write(ent.m_OffsetX);
				writer.Write(ent.m_OffsetY);
				writer.Write(ent.m_OffsetZ);
				writer.Write(ent.m_Flags);
			}
		}

		public MultiComponentList(GenericReader reader)
		{
			int version = reader.ReadInt();

			m_Min = reader.ReadPoint2D();
			m_Max = reader.ReadPoint2D();
			m_Center = reader.ReadPoint2D();
			m_Width = reader.ReadInt();
			m_Height = reader.ReadInt();

			int length = reader.ReadInt();

			var allTiles = m_List = new MultiTileEntry[length];

			if (version == 0)
			{
				for (int i = 0; i < length; ++i)
				{
					int id = reader.ReadShort();
					if (id >= 0x4000)
					{
						id -= 0x4000;
					}

					allTiles[i].m_ItemID = (ushort)id;
					allTiles[i].m_OffsetX = reader.ReadShort();
					allTiles[i].m_OffsetY = reader.ReadShort();
					allTiles[i].m_OffsetZ = reader.ReadShort();
					allTiles[i].m_Flags = reader.ReadInt();
				}
			}
			else
			{
				for (int i = 0; i < length; ++i)
				{
					allTiles[i].m_ItemID = reader.ReadUShort();
					allTiles[i].m_OffsetX = reader.ReadShort();
					allTiles[i].m_OffsetY = reader.ReadShort();
					allTiles[i].m_OffsetZ = reader.ReadShort();
					allTiles[i].m_Flags = reader.ReadInt();
				}
			}

			var tiles = new TileList[m_Width][];
			m_Tiles = new StaticTile[m_Width][][];

			for (int x = 0; x < m_Width; ++x)
			{
				tiles[x] = new TileList[m_Height];
				m_Tiles[x] = new StaticTile[m_Height][];

				for (int y = 0; y < m_Height; ++y)
				{
					tiles[x][y] = new TileList();
				}
			}

			for (int i = 0; i < allTiles.Length; ++i)
			{
				if (i == 0 || allTiles[i].m_Flags != 0)
				{
					int xOffset = allTiles[i].m_OffsetX + m_Center.m_X;
					int yOffset = allTiles[i].m_OffsetY + m_Center.m_Y;

					#region Stygian Abyss
					//tiles[xOffset][yOffset].Add( (ushort)allTiles[i].m_ItemID, (sbyte)allTiles[i].m_OffsetZ );
					tiles[xOffset][yOffset].Add(
						(ushort)((allTiles[i].m_ItemID & TileData.MaxItemValue) | 0x10000), (sbyte)allTiles[i].m_OffsetZ);
					#endregion
				}
			}

			for (int x = 0; x < m_Width; ++x)
			{
				for (int y = 0; y < m_Height; ++y)
				{
					m_Tiles[x][y] = tiles[x][y].ToArray();
				}
			}
		}

		public MultiComponentList(BinaryReader reader, int count)
		{
			var allTiles = m_List = new MultiTileEntry[count];

			for (int i = 0; i < count; ++i)
			{
				allTiles[i].m_ItemID = reader.ReadUInt16();
				allTiles[i].m_OffsetX = reader.ReadInt16();
				allTiles[i].m_OffsetY = reader.ReadInt16();
				allTiles[i].m_OffsetZ = reader.ReadInt16();
				allTiles[i].m_Flags = reader.ReadInt32();

				if (_PostHSFormat)
				{
					reader.ReadInt32(); // ??
				}

				MultiTileEntry e = allTiles[i];

				if (i == 0 || e.m_Flags != 0)
				{
					if (e.m_OffsetX < m_Min.m_X)
					{
						m_Min.m_X = e.m_OffsetX;
					}

					if (e.m_OffsetY < m_Min.m_Y)
					{
						m_Min.m_Y = e.m_OffsetY;
					}

					if (e.m_OffsetX > m_Max.m_X)
					{
						m_Max.m_X = e.m_OffsetX;
					}

					if (e.m_OffsetY > m_Max.m_Y)
					{
						m_Max.m_Y = e.m_OffsetY;
					}
				}
			}

			m_Center = new Point2D(-m_Min.m_X, -m_Min.m_Y);
			m_Width = (m_Max.m_X - m_Min.m_X) + 1;
			m_Height = (m_Max.m_Y - m_Min.m_Y) + 1;

			var tiles = new TileList[m_Width][];
			m_Tiles = new StaticTile[m_Width][][];

			for (int x = 0; x < m_Width; ++x)
			{
				tiles[x] = new TileList[m_Height];
				m_Tiles[x] = new StaticTile[m_Height][];

				for (int y = 0; y < m_Height; ++y)
				{
					tiles[x][y] = new TileList();
				}
			}

			for (int i = 0; i < allTiles.Length; ++i)
			{
				if (i == 0 || allTiles[i].m_Flags != 0)
				{
					int xOffset = allTiles[i].m_OffsetX + m_Center.m_X;
					int yOffset = allTiles[i].m_OffsetY + m_Center.m_Y;

					#region Stygian Abyss
					//tiles[xOffset][yOffset].Add( (ushort)allTiles[i].m_ItemID, (sbyte)allTiles[i].m_OffsetZ );
					tiles[xOffset][yOffset].Add(
						(ushort)((allTiles[i].m_ItemID & TileData.MaxItemValue) | 0x10000), (sbyte)allTiles[i].m_OffsetZ);
					#endregion
				}
			}

			for (int x = 0; x < m_Width; ++x)
			{
				for (int y = 0; y < m_Height; ++y)
				{
					m_Tiles[x][y] = tiles[x][y].ToArray();
				}
			}
		}

		private MultiComponentList()
		{
			m_Tiles = new StaticTile[0][][];
			m_List = new MultiTileEntry[0];
		}
	}
}