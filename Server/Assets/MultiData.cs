#region References
using System;
using System.Collections.Generic;
using System.IO;

using Server.Network;
#endregion

namespace Server
{
	public static class MultiData
	{
		public static Dictionary<int, MultiComponentList> Components { get; private set; }

		private static readonly BinaryReader m_IndexReader;
		private static readonly BinaryReader m_StreamReader;

		public static bool UsingUOPFormat { get; private set; }

		public static MultiComponentList GetComponents(int multiID)
		{
			MultiComponentList mcl;

			multiID &= 0x3FFF; // The value of the actual multi is shifted by 0x4000, so this is left alone.

			if (Components.ContainsKey(multiID))
			{
				mcl = Components[multiID];
			}
			else if (!UsingUOPFormat)
			{
				Components[multiID] = mcl = Load(multiID);
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

				var lookup = m_IndexReader.ReadInt32();
				var length = m_IndexReader.ReadInt32();

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

		public static void UOPLoad(string path)
		{
			var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
			var streamReader = new BinaryReader(stream);

			// Head Information Start
			if (streamReader.ReadInt32() != 0x0050594D) // Not a UOP Files
				return;

			if (streamReader.ReadInt32() > 5) // Bad Version
				return;

			// Multi ID List Array Start
			var chunkIds = new Dictionary<ulong, int>();
			var chunkIds2 = new Dictionary<ulong, int>();

			UOPHash.BuildChunkIDs(ref chunkIds, ref chunkIds2);
			// Multi ID List Array End

			streamReader.ReadUInt32();                      // format timestamp? 0xFD23EC43
			var startAddress = streamReader.ReadInt64();

			var blockSize = streamReader.ReadInt32();       // files in each block
			var totalSize = streamReader.ReadInt32();       // Total File Count

			stream.Seek(startAddress, SeekOrigin.Begin);    // Head Information End

			long nextBlock;

			do
			{
				var blockFileCount = streamReader.ReadInt32();
				nextBlock = streamReader.ReadInt64();

				var index = 0;

				do
				{
					var offset = streamReader.ReadInt64();

					var headerSize = streamReader.ReadInt32();          // header length
					var compressedSize = streamReader.ReadInt32();      // compressed size
					var decompressedSize = streamReader.ReadInt32();    // decompressed size

					var filehash = streamReader.ReadUInt64();         // filename hash (HashLittle2)
					var datablockhash = streamReader.ReadUInt32();     // data hash (Adler32)
					var flag = streamReader.ReadInt16();              // compression method (0 = none, 1 = zlib)

					index++;

					if (offset == 0 || decompressedSize == 0 || filehash == 0x126D1E99DDEDEE0A) // Exclude housing.bin
						continue;

					// Multi ID Search Start
					var chunkID = -1;

					if (!chunkIds.TryGetValue(filehash, out chunkID))
					{

						if (chunkIds2.TryGetValue(filehash, out var tmpChunkID))
						{
							chunkID = tmpChunkID;
						}
					}
					// Multi ID Search End                        

					var positionpoint = stream.Position;  // save current position

					// Decompress Data Start
					stream.Seek(offset + headerSize, SeekOrigin.Begin);

					var sourceData = new byte[compressedSize];

					if (stream.Read(sourceData, 0, compressedSize) != compressedSize)
						continue;

					byte[] data;

					if (flag == 1)
					{
						var destData = new byte[decompressedSize];
						/*ZLibError error = */
						Compression.Compressor.Decompress(destData, ref decompressedSize, sourceData, compressedSize);

						data = destData;
					}
					else
					{
						data = sourceData;
					}
					// End Decompress Data

					var tileList = new List<MultiTileEntry>();

					using (var fs = new MemoryStream(data))
					{
						using (var reader = new BinaryReader(fs))
						{
							var a = reader.ReadUInt32();
							var count = reader.ReadUInt32();

							for (uint i = 0; i < count; i++)
							{
								var ItemId = reader.ReadUInt16();
								var x = reader.ReadInt16();
								var y = reader.ReadInt16();
								var z = reader.ReadInt16();

								var flagint = reader.ReadUInt16();

								TileFlag flagg;

								switch (flagint)
								{
									default:
									case 0:
									flagg = TileFlag.Background;
									break;
									case 1:
									flagg = TileFlag.None;
									break;
									case 257:
									flagg = TileFlag.Generic;
									break;
								}

								var clilocsCount = reader.ReadUInt32();

								if (clilocsCount != 0)
								{
									fs.Seek(fs.Position + (clilocsCount * 4), SeekOrigin.Begin); // binary block bypass
								}

								tileList.Add(new MultiTileEntry(ItemId, x, y, z, flagg));
							}

							reader.Close();
						}
					}

					Components[chunkID] = new MultiComponentList(tileList);

					stream.Seek(positionpoint, SeekOrigin.Begin); // back to position
				}
				while (index < blockFileCount);
			}
			while (stream.Seek(nextBlock, SeekOrigin.Begin) != 0);

			chunkIds.Clear();
			chunkIds2.Clear();
		}

		static MultiData()
		{
			Components = new Dictionary<int, MultiComponentList>();

			var multicollectionPath = Core.FindDataFile("MultiCollection.uop");

			if (File.Exists(multicollectionPath))
			{
				UOPLoad(multicollectionPath);
				UsingUOPFormat = true;
			}
			else
			{
				var idxPath = Core.FindDataFile("multi.idx");
				var mulPath = Core.FindDataFile("multi.mul");

				if (File.Exists(idxPath) && File.Exists(mulPath))
				{
					var idx = new FileStream(idxPath, FileMode.Open, FileAccess.Read, FileShare.Read);
					m_IndexReader = new BinaryReader(idx);

					var stream = new FileStream(mulPath, FileMode.Open, FileAccess.Read, FileShare.Read);
					m_StreamReader = new BinaryReader(stream);

					var vdPath = Core.FindDataFile("verdata.mul");

					if (File.Exists(vdPath))
					{
						using (var fs = new FileStream(vdPath, FileMode.Open, FileAccess.Read, FileShare.Read))
						{
							var bin = new BinaryReader(fs);

							var count = bin.ReadInt32();

							for (var i = 0; i < count; ++i)
							{
								var file = bin.ReadInt32();
								var index = bin.ReadInt32();
								var lookup = bin.ReadInt32();
								var length = bin.ReadInt32();
								/*int extra = */
								bin.ReadInt32();

								if (file == 14 && index >= 0 && lookup >= 0 && length > 0)
								{
									bin.BaseStream.Seek(lookup, SeekOrigin.Begin);

									Components[index] = new MultiComponentList(bin, length / 12);

									bin.BaseStream.Seek(24 + (i * 20), SeekOrigin.Begin);
								}
							}

							bin.Close();
						}
					}
					else
					{
						Console.WriteLine("Warning: Multi data files not found!");
					}
				}
			}
		}
	}

	public struct MultiTileEntry
	{
		public ushort m_ItemID;
		public short m_OffsetX, m_OffsetY, m_OffsetZ;
		public TileFlag m_Flags;

		public MultiTileEntry(ushort itemID, short xOffset, short yOffset, short zOffset, TileFlag flags)
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
		public static bool PostHSFormat { get; set; }

		private Point2D m_Min, m_Max, m_Center;
		private MultiTileEntry[] m_List;

		public static readonly MultiComponentList Empty = new MultiComponentList();

		public Point2D Min => m_Min;
		public Point2D Max => m_Max;

		public Point2D Center => m_Center;

		public int Width { get; private set; }
		public int Height { get; private set; }

		public StaticTile[][][] Tiles { get; private set; }
		public MultiTileEntry[] List => m_List;

		public void Add(int itemID, int x, int y, int z)
		{
			itemID &= TileData.MaxItemValue;
			itemID |= 0x10000;

			var vx = x + m_Center.m_X;
			var vy = y + m_Center.m_Y;

			if (vx >= 0 && vx < Width && vy >= 0 && vy < Height)
			{
				var oldTiles = Tiles[vx][vy];

				for (var i = oldTiles.Length - 1; i >= 0; --i)
				{
					var data = TileData.ItemTable[itemID & TileData.MaxItemValue];

					if (oldTiles[i].Z == z && (oldTiles[i].Height > 0 == data.Height > 0))
					{
						var newIsRoof = (data.Flags & TileFlag.Roof) != 0;
						var oldIsRoof = (TileData.ItemTable[oldTiles[i].ID & TileData.MaxItemValue].Flags & TileFlag.Roof) != 0;

						if (newIsRoof == oldIsRoof)
						{
							Remove(oldTiles[i].ID, x, y, z);
						}
					}
				}

				oldTiles = Tiles[vx][vy];

				var newTiles = new StaticTile[oldTiles.Length + 1];

				for (var i = 0; i < oldTiles.Length; ++i)
				{
					newTiles[i] = oldTiles[i];
				}

				newTiles[oldTiles.Length] = new StaticTile((ushort)itemID, (sbyte)z);

				Tiles[vx][vy] = newTiles;

				var oldList = m_List;
				var newList = new MultiTileEntry[oldList.Length + 1];

				for (var i = 0; i < oldList.Length; ++i)
				{
					newList[i] = oldList[i];
				}

				newList[oldList.Length] = new MultiTileEntry((ushort)itemID, (short)x, (short)y, (short)z, TileFlag.Background);

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
			var vx = x + m_Center.m_X;
			var vy = y + m_Center.m_Y;

			if (vx >= 0 && vx < Width && vy >= 0 && vy < Height)
			{
				var oldTiles = Tiles[vx][vy];

				for (var i = 0; i < oldTiles.Length; ++i)
				{
					var tile = oldTiles[i];

					if (tile.Z == z && tile.Height >= minHeight)
					{
						var newTiles = new StaticTile[oldTiles.Length - 1];

						for (var j = 0; j < i; ++j)
						{
							newTiles[j] = oldTiles[j];
						}

						for (var j = i + 1; j < oldTiles.Length; ++j)
						{
							newTiles[j - 1] = oldTiles[j];
						}

						Tiles[vx][vy] = newTiles;

						break;
					}
				}

				var oldList = m_List;

				for (var i = 0; i < oldList.Length; ++i)
				{
					var tile = oldList[i];

					if (tile.m_OffsetX == (short)x && tile.m_OffsetY == (short)y && tile.m_OffsetZ == (short)z &&
						TileData.ItemTable[tile.m_ItemID & TileData.MaxItemValue].Height >= minHeight)
					{
						var newList = new MultiTileEntry[oldList.Length - 1];

						for (var j = 0; j < i; ++j)
						{
							newList[j] = oldList[j];
						}

						for (var j = i + 1; j < oldList.Length; ++j)
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
			var vx = x + m_Center.m_X;
			var vy = y + m_Center.m_Y;

			if (vx >= 0 && vx < Width && vy >= 0 && vy < Height)
			{
				var oldTiles = Tiles[vx][vy];

				for (var i = 0; i < oldTiles.Length; ++i)
				{
					var tile = oldTiles[i];

					if (tile.ID == itemID && tile.Z == z)
					{
						var newTiles = new StaticTile[oldTiles.Length - 1];

						for (var j = 0; j < i; ++j)
						{
							newTiles[j] = oldTiles[j];
						}

						for (var j = i + 1; j < oldTiles.Length; ++j)
						{
							newTiles[j - 1] = oldTiles[j];
						}

						Tiles[vx][vy] = newTiles;

						break;
					}
				}

				var oldList = m_List;

				for (var i = 0; i < oldList.Length; ++i)
				{
					var tile = oldList[i];

					if (tile.m_ItemID == itemID && tile.m_OffsetX == (short)x && tile.m_OffsetY == (short)y &&
						tile.m_OffsetZ == (short)z)
					{
						var newList = new MultiTileEntry[oldList.Length - 1];

						for (var j = 0; j < i; ++j)
						{
							newList[j] = oldList[j];
						}

						for (var j = i + 1; j < oldList.Length; ++j)
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
			int oldWidth = Width, oldHeight = Height;
			var oldTiles = Tiles;

			var totalLength = 0;

			var newTiles = new StaticTile[newWidth][][];

			for (var x = 0; x < newWidth; ++x)
			{
				newTiles[x] = new StaticTile[newHeight][];

				for (var y = 0; y < newHeight; ++y)
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

			Tiles = newTiles;
			m_List = new MultiTileEntry[totalLength];
			Width = newWidth;
			Height = newHeight;

			m_Min = Point2D.Zero;
			m_Max = Point2D.Zero;

			var index = 0;

			for (var x = 0; x < newWidth; ++x)
			{
				for (var y = 0; y < newHeight; ++y)
				{
					var tiles = newTiles[x][y];

					foreach (var tile in tiles)
					{
						var vx = x - m_Center.X;
						var vy = y - m_Center.Y;

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

						m_List[index++] = new MultiTileEntry((ushort)tile.ID, (short)vx, (short)vy, (short)tile.Z, TileFlag.Background);
					}
				}
			}
		}

		public MultiComponentList(MultiComponentList toCopy)
		{
			m_Min = toCopy.m_Min;
			m_Max = toCopy.m_Max;

			m_Center = toCopy.m_Center;

			Width = toCopy.Width;
			Height = toCopy.Height;

			Tiles = new StaticTile[Width][][];

			for (var x = 0; x < Width; ++x)
			{
				Tiles[x] = new StaticTile[Height][];

				for (var y = 0; y < Height; ++y)
				{
					Tiles[x][y] = new StaticTile[toCopy.Tiles[x][y].Length];

					for (var i = 0; i < Tiles[x][y].Length; ++i)
					{
						Tiles[x][y][i] = toCopy.Tiles[x][y][i];
					}
				}
			}

			m_List = new MultiTileEntry[toCopy.m_List.Length];

			for (var i = 0; i < m_List.Length; ++i)
			{
				m_List[i] = toCopy.m_List[i];
			}
		}

		public void Serialize(GenericWriter writer)
		{
			writer.Write(2); // version;

			writer.Write(m_Min);
			writer.Write(m_Max);
			writer.Write(m_Center);

			writer.Write(Width);
			writer.Write(Height);

			writer.Write(m_List.Length);

			foreach (var ent in m_List)
			{
				writer.Write(ent.m_ItemID);
				writer.Write(ent.m_OffsetX);
				writer.Write(ent.m_OffsetY);
				writer.Write(ent.m_OffsetZ);

				writer.Write((ulong)ent.m_Flags);
			}
		}

		public MultiComponentList(GenericReader reader)
		{
			var version = reader.ReadInt();

			m_Min = reader.ReadPoint2D();
			m_Max = reader.ReadPoint2D();
			m_Center = reader.ReadPoint2D();
			Width = reader.ReadInt();
			Height = reader.ReadInt();

			var length = reader.ReadInt();

			var allTiles = m_List = new MultiTileEntry[length];

			if (version == 0)
			{
				for (var i = 0; i < length; ++i)
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

					allTiles[i].m_Flags = (TileFlag)reader.ReadUInt();
				}
			}
			else
			{
				for (var i = 0; i < length; ++i)
				{
					allTiles[i].m_ItemID = reader.ReadUShort();
					allTiles[i].m_OffsetX = reader.ReadShort();
					allTiles[i].m_OffsetY = reader.ReadShort();
					allTiles[i].m_OffsetZ = reader.ReadShort();

					if (version > 1)
						allTiles[i].m_Flags = (TileFlag)reader.ReadULong();
					else
						allTiles[i].m_Flags = (TileFlag)reader.ReadUInt();
				}
			}

			var tiles = new TileList[Width][];

			Tiles = new StaticTile[Width][][];

			for (var x = 0; x < Width; ++x)
			{
				tiles[x] = new TileList[Height];
				Tiles[x] = new StaticTile[Height][];

				for (var y = 0; y < Height; ++y)
				{
					tiles[x][y] = new TileList();
				}
			}

			for (var i = 0; i < allTiles.Length; ++i)
			{
				if (i == 0 || allTiles[i].m_Flags != 0)
				{
					var xOffset = allTiles[i].m_OffsetX + m_Center.m_X;
					var yOffset = allTiles[i].m_OffsetY + m_Center.m_Y;
					var itemID = (allTiles[i].m_ItemID & TileData.MaxItemValue) | 0x10000;

					tiles[xOffset][yOffset].Add((ushort)itemID, (sbyte)allTiles[i].m_OffsetZ);
				}
			}

			for (var x = 0; x < Width; ++x)
			{
				for (var y = 0; y < Height; ++y)
				{
					Tiles[x][y] = tiles[x][y].ToArray();
				}
			}
		}

		public MultiComponentList(BinaryReader reader, int count)
		{
			var allTiles = m_List = new MultiTileEntry[count];

			for (var i = 0; i < count; ++i)
			{
				allTiles[i].m_ItemID = reader.ReadUInt16();
				allTiles[i].m_OffsetX = reader.ReadInt16();
				allTiles[i].m_OffsetY = reader.ReadInt16();
				allTiles[i].m_OffsetZ = reader.ReadInt16();

				if (PostHSFormat)
					allTiles[i].m_Flags = (TileFlag)reader.ReadUInt64();
				else
					allTiles[i].m_Flags = (TileFlag)reader.ReadUInt32();

				var e = allTiles[i];

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
			Width = m_Max.m_X - m_Min.m_X + 1;
			Height = m_Max.m_Y - m_Min.m_Y + 1;

			var tiles = new TileList[Width][];

			Tiles = new StaticTile[Width][][];

			for (var x = 0; x < Width; ++x)
			{
				tiles[x] = new TileList[Height];
				Tiles[x] = new StaticTile[Height][];

				for (var y = 0; y < Height; ++y)
				{
					tiles[x][y] = new TileList();
				}
			}

			for (var i = 0; i < allTiles.Length; ++i)
			{
				if (i == 0 || allTiles[i].m_Flags != 0)
				{
					var xOffset = allTiles[i].m_OffsetX + m_Center.m_X;
					var yOffset = allTiles[i].m_OffsetY + m_Center.m_Y;
					var itemID = (allTiles[i].m_ItemID & TileData.MaxItemValue) | 0x10000;

					tiles[xOffset][yOffset].Add((ushort)itemID, (sbyte)allTiles[i].m_OffsetZ);
				}
			}

			for (var x = 0; x < Width; ++x)
			{
				for (var y = 0; y < Height; ++y)
				{
					Tiles[x][y] = tiles[x][y].ToArray();
				}
			}
		}

		public MultiComponentList(List<MultiTileEntry> list)
		{
			var allTiles = m_List = new MultiTileEntry[list.Count];

			for (var i = 0; i < list.Count; ++i)
			{
				allTiles[i].m_ItemID = list[i].m_ItemID;
				allTiles[i].m_OffsetX = list[i].m_OffsetX;
				allTiles[i].m_OffsetY = list[i].m_OffsetY;
				allTiles[i].m_OffsetZ = list[i].m_OffsetZ;

				allTiles[i].m_Flags = list[i].m_Flags;

				var e = allTiles[i];

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
			Width = m_Max.m_X - m_Min.m_X + 1;
			Height = m_Max.m_Y - m_Min.m_Y + 1;

			var tiles = new TileList[Width][];

			Tiles = new StaticTile[Width][][];

			for (var x = 0; x < Width; ++x)
			{
				tiles[x] = new TileList[Height];
				Tiles[x] = new StaticTile[Height][];

				for (var y = 0; y < Height; ++y)
				{
					tiles[x][y] = new TileList();
				}
			}

			for (var i = 0; i < allTiles.Length; ++i)
			{
				if (i == 0 || allTiles[i].m_Flags != 0)
				{
					var xOffset = allTiles[i].m_OffsetX + m_Center.m_X;
					var yOffset = allTiles[i].m_OffsetY + m_Center.m_Y;
					var itemID = (allTiles[i].m_ItemID & TileData.MaxItemValue) | 0x10000;

					tiles[xOffset][yOffset].Add((ushort)itemID, (sbyte)allTiles[i].m_OffsetZ);
				}
			}

			for (var x = 0; x < Width; ++x)
			{
				for (var y = 0; y < Height; ++y)
				{
					Tiles[x][y] = tiles[x][y].ToArray();
				}
			}
		}

		private MultiComponentList()
		{
			Tiles = new StaticTile[0][][];
			m_List = new MultiTileEntry[0];
		}
	}

	public class UOPHash
	{
		public static void BuildChunkIDs(ref Dictionary<ulong, int> chunkIds, ref Dictionary<ulong, int> chunkIds2)
		{

			var formats = GetHashFormat(0, out var maxId);

			for (var i = 0; i < maxId; ++i)
			{
				chunkIds[HashLittle2(String.Format(formats[0], i))] = i;
			}

			if (formats[1] != "")
			{
				for (var i = 0; i < maxId; ++i)
					chunkIds2[HashLittle2(String.Format(formats[1], i))] = i;
			}
		}

		private static string[] GetHashFormat(int typeIndex, out int maxId)
		{
			/*
			 * MaxID is only used for constructing a lookup table.
			 * Decrease to save some possibly unneeded computation.
			 */
			maxId = 0x10000;

			return new string[] { "build/multicollection/{0:000000}.bin", "" };
		}

		public static ulong HashLittle2(string s)
		{
			var length = s.Length;

			uint a, b, c;
			a = b = c = 0xDEADBEEF + (uint)length;

			var k = 0;

			while (length > 12)
			{
				a += s[k];
				a += ((uint)s[k + 1]) << 8;
				a += ((uint)s[k + 2]) << 16;
				a += ((uint)s[k + 3]) << 24;
				b += s[k + 4];
				b += ((uint)s[k + 5]) << 8;
				b += ((uint)s[k + 6]) << 16;
				b += ((uint)s[k + 7]) << 24;
				c += s[k + 8];
				c += ((uint)s[k + 9]) << 8;
				c += ((uint)s[k + 10]) << 16;
				c += ((uint)s[k + 11]) << 24;

				a -= c;
				a ^= (c << 4) | (c >> 28);
				c += b;
				b -= a;
				b ^= (a << 6) | (a >> 26);
				a += c;
				c -= b;
				c ^= (b << 8) | (b >> 24);
				b += a;
				a -= c;
				a ^= (c << 16) | (c >> 16);
				c += b;
				b -= a;
				b ^= (a << 19) | (a >> 13);
				a += c;
				c -= b;
				c ^= (b << 4) | (b >> 28);
				b += a;

				length -= 12;
				k += 12;
			}

			if (length != 0)
			{
				switch (length)
				{
					case 12:
					c += ((uint)s[k + 11]) << 24;
					goto case 11;
					case 11:
					c += ((uint)s[k + 10]) << 16;
					goto case 10;
					case 10:
					c += ((uint)s[k + 9]) << 8;
					goto case 9;
					case 9:
					c += s[k + 8];
					goto case 8;
					case 8:
					b += ((uint)s[k + 7]) << 24;
					goto case 7;
					case 7:
					b += ((uint)s[k + 6]) << 16;
					goto case 6;
					case 6:
					b += ((uint)s[k + 5]) << 8;
					goto case 5;
					case 5:
					b += s[k + 4];
					goto case 4;
					case 4:
					a += ((uint)s[k + 3]) << 24;
					goto case 3;
					case 3:
					a += ((uint)s[k + 2]) << 16;
					goto case 2;
					case 2:
					a += ((uint)s[k + 1]) << 8;
					goto case 1;
					case 1:
					a += s[k];
					break;
				}

				c ^= b;
				c -= (b << 14) | (b >> 18);
				a ^= c;
				a -= (c << 11) | (c >> 21);
				b ^= a;
				b -= (a << 25) | (a >> 7);
				c ^= b;
				c -= (b << 16) | (b >> 16);
				a ^= c;
				a -= (c << 4) | (c >> 28);
				b ^= a;
				b -= (a << 14) | (a >> 18);
				c ^= b;
				c -= (b << 24) | (b >> 8);
			}

			return ((ulong)b << 32) | c;
		}
	}
}
