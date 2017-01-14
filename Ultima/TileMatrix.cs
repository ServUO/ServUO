#region References
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
#endregion

namespace Ultima
{
	public sealed class TileMatrix
	{
		private readonly HuedTile[][][][][] m_StaticTiles;
		private readonly Tile[][][] m_LandTiles;
		private bool[][] m_RemovedStaticBlock;
		private List<StaticTile>[][] m_StaticTiles_ToAdd;

		public static Tile[] InvalidLandBlock { get; private set; }
		public static HuedTile[][][] EmptyStaticBlock { get; private set; }

		private FileStream m_Map;
		private BinaryReader m_UOPReader;
		private FileStream m_Statics;
		private Entry3D[] m_StaticIndex;

		public Entry3D[] StaticIndex
		{
			get
			{
				if (!StaticIndexInit)
				{
					InitStatics();
				}
				return m_StaticIndex;
			}
		}

		public bool StaticIndexInit;

		public TileMatrixPatch Patch { get; private set; }

		public int BlockWidth { get; private set; }

		public int BlockHeight { get; private set; }

		public int Width { get; private set; }

		public int Height { get; private set; }

		private readonly string mapPath;
		private readonly string indexPath;
		private readonly string staticsPath;

		public void CloseStreams()
		{
			if (m_Map != null)
			{
				m_Map.Close();
			}

			if (m_UOPReader != null)
			{
				m_UOPReader.Close();
			}

			if (m_Statics != null)
			{
				m_Statics.Close();
			}
		}

		public TileMatrix(int fileIndex, int mapID, int width, int height, string path)
		{
			Width = width;
			Height = height;
			BlockWidth = width >> 3;
			BlockHeight = height >> 3;

			if (path == null)
			{
				mapPath = Files.GetFilePath("map{0}.mul", fileIndex);

				if (String.IsNullOrEmpty(mapPath) || !File.Exists(mapPath))
				{
					mapPath = Files.GetFilePath("map{0}LegacyMUL.uop", fileIndex);
				}

				if (mapPath != null && mapPath.EndsWith(".uop"))
				{
					IsUOPFormat = true;
				}
			}
			else
			{
				mapPath = Path.Combine(path, String.Format("map{0}.mul", fileIndex));

				if (!File.Exists(mapPath))
				{
					mapPath = Path.Combine(path, String.Format("map{0}LegacyMUL.uop", fileIndex));
				}

				if (!File.Exists(mapPath))
				{
					mapPath = null;
				}
				else if (mapPath != null && mapPath.EndsWith(".uop"))
				{
					IsUOPFormat = true;
				}
			}

			if (path == null)
			{
				indexPath = Files.GetFilePath("staidx{0}.mul", fileIndex);
			}
			else
			{
				indexPath = Path.Combine(path, String.Format("staidx{0}.mul", fileIndex));
				if (!File.Exists(indexPath))
				{
					indexPath = null;
				}
			}

			if (path == null)
			{
				staticsPath = Files.GetFilePath("statics{0}.mul", fileIndex);
			}
			else
			{
				staticsPath = Path.Combine(path, String.Format("statics{0}.mul", fileIndex));
				if (!File.Exists(staticsPath))
				{
					staticsPath = null;
				}
			}

			EmptyStaticBlock = new HuedTile[8][][];

			for (int i = 0; i < 8; ++i)
			{
				EmptyStaticBlock[i] = new HuedTile[8][];

				for (int j = 0; j < 8; ++j)
				{
					EmptyStaticBlock[i][j] = new HuedTile[0];
				}
			}

			InvalidLandBlock = new Tile[196];

			m_LandTiles = new Tile[BlockWidth][][];
			m_StaticTiles = new HuedTile[BlockWidth][][][][];

			Patch = new TileMatrixPatch(this, mapID, path);
		}

		public void SetStaticBlock(int x, int y, HuedTile[][][] value)
		{
			if (x < 0 || y < 0 || x >= BlockWidth || y >= BlockHeight)
			{
				return;
			}

			if (m_StaticTiles[x] == null)
			{
				m_StaticTiles[x] = new HuedTile[BlockHeight][][][];
			}

			m_StaticTiles[x][y] = value;
		}

		public HuedTile[][][] GetStaticBlock(int x, int y)
		{
			return GetStaticBlock(x, y, true);
		}

		public HuedTile[][][] GetStaticBlock(int x, int y, bool patch)
		{
			if (x < 0 || y < 0 || x >= BlockWidth || y >= BlockHeight)
			{
				return EmptyStaticBlock;
			}

			if (m_StaticTiles[x] == null)
			{
				m_StaticTiles[x] = new HuedTile[BlockHeight][][][];
			}

			HuedTile[][][] tiles = m_StaticTiles[x][y];

			if (tiles == null)
			{
				tiles = m_StaticTiles[x][y] = ReadStaticBlock(x, y);
			}

			if ((Map.UseDiff) && (patch))
			{
				if (Patch.StaticBlocksCount > 0)
				{
					if (Patch.StaticBlocks[x] != null)
					{
						if (Patch.StaticBlocks[x][y] != null)
						{
							tiles = Patch.StaticBlocks[x][y];
						}
					}
				}
			}
			return tiles;
		}

		public HuedTile[] GetStaticTiles(int x, int y, bool patch)
		{
			return GetStaticBlock(x >> 3, y >> 3, patch)[x & 0x7][y & 0x7];
		}

		public HuedTile[] GetStaticTiles(int x, int y)
		{
			return GetStaticBlock(x >> 3, y >> 3)[x & 0x7][y & 0x7];
		}

		public void SetLandBlock(int x, int y, Tile[] value)
		{
			if (x < 0 || y < 0 || x >= BlockWidth || y >= BlockHeight)
			{
				return;
			}

			if (m_LandTiles[x] == null)
			{
				m_LandTiles[x] = new Tile[BlockHeight][];
			}

			m_LandTiles[x][y] = value;
		}

		public Tile[] GetLandBlock(int x, int y)
		{
			return GetLandBlock(x, y, true);
		}

		public Tile[] GetLandBlock(int x, int y, bool patch)
		{
			if (x < 0 || y < 0 || x >= BlockWidth || y >= BlockHeight)
			{
				return InvalidLandBlock;
			}

			if (m_LandTiles[x] == null)
			{
				m_LandTiles[x] = new Tile[BlockHeight][];
			}

			Tile[] tiles = m_LandTiles[x][y];

			if (tiles == null)
			{
				tiles = m_LandTiles[x][y] = ReadLandBlock(x, y);
			}

			if ((Map.UseDiff) && (patch))
			{
				if (Patch.LandBlocksCount > 0)
				{
					if (Patch.LandBlocks[x] != null)
					{
						if (Patch.LandBlocks[x][y] != null)
						{
							tiles = Patch.LandBlocks[x][y];
						}
					}
				}
			}
			return tiles;
		}

		public Tile GetLandTile(int x, int y, bool patch)
		{
			return GetLandBlock(x >> 3, y >> 3, patch)[((y & 0x7) << 3) + (x & 0x7)];
		}

		public Tile GetLandTile(int x, int y)
		{
			return GetLandBlock(x >> 3, y >> 3)[((y & 0x7) << 3) + (x & 0x7)];
		}

		private void InitStatics()
		{
			m_StaticIndex = new Entry3D[BlockHeight * BlockWidth];
			if (indexPath == null)
			{
				return;
			}
			using (var index = new FileStream(indexPath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				m_Statics = new FileStream(staticsPath, FileMode.Open, FileAccess.Read, FileShare.Read);
				var count = (int)(index.Length / 12);
				GCHandle gc = GCHandle.Alloc(m_StaticIndex, GCHandleType.Pinned);
				var buffer = new byte[index.Length];
				index.Read(buffer, 0, (int)index.Length);
				Marshal.Copy(buffer, 0, gc.AddrOfPinnedObject(), (int)Math.Min(index.Length, BlockHeight * BlockWidth * 12));
				gc.Free();
				for (var i = (int)Math.Min(index.Length, BlockHeight * BlockWidth); i < BlockHeight * BlockWidth; ++i)
				{
					m_StaticIndex[i].lookup = -1;
					m_StaticIndex[i].length = -1;
					m_StaticIndex[i].extra = -1;
				}
				StaticIndexInit = true;
			}
		}

		private static HuedTileList[][] m_Lists;
		private static byte[] m_Buffer;

		private unsafe HuedTile[][][] ReadStaticBlock(int x, int y)
		{
			try
			{
				if (!StaticIndexInit)
				{
					InitStatics();
				}
				if (m_Statics == null || !m_Statics.CanRead || !m_Statics.CanSeek)
				{
					if (staticsPath == null)
					{
						m_Statics = null;
					}
					else
					{
						m_Statics = new FileStream(staticsPath, FileMode.Open, FileAccess.Read, FileShare.Read);
					}
				}
				if (m_Statics == null)
				{
					return EmptyStaticBlock;
				}

				int lookup = m_StaticIndex[(x * BlockHeight) + y].lookup;
				int length = m_StaticIndex[(x * BlockHeight) + y].length;

				if (lookup < 0 || length <= 0)
				{
					return EmptyStaticBlock;
				}
				else
				{
					int count = length / 7;

					m_Statics.Seek(lookup, SeekOrigin.Begin);

					if (m_Buffer == null || m_Buffer.Length < length)
					{
						m_Buffer = new byte[length];
					}

					GCHandle gc = GCHandle.Alloc(m_Buffer, GCHandleType.Pinned);
					try
					{
						m_Statics.Read(m_Buffer, 0, length);

						if (m_Lists == null)
						{
							m_Lists = new HuedTileList[8][];

							for (int i = 0; i < 8; ++i)
							{
								m_Lists[i] = new HuedTileList[8];

								for (int j = 0; j < 8; ++j)
								{
									m_Lists[i][j] = new HuedTileList();
								}
							}
						}

						HuedTileList[][] lists = m_Lists;

						for (int i = 0; i < count; ++i)
						{
							var ptr = new IntPtr((long)gc.AddrOfPinnedObject() + i * sizeof(StaticTile));
							var cur = (StaticTile)Marshal.PtrToStructure(ptr, typeof(StaticTile));
							lists[cur.m_X & 0x7][cur.m_Y & 0x7].Add(Art.GetLegalItemID(cur.m_ID), cur.m_Hue, cur.m_Z);
						}

						var tiles = new HuedTile[8][][];

						for (int i = 0; i < 8; ++i)
						{
							tiles[i] = new HuedTile[8][];

							for (int j = 0; j < 8; ++j)
							{
								tiles[i][j] = lists[i][j].ToArray();
							}
						}

						return tiles;
					}
					finally
					{
						gc.Free();
					}
				}
			}
			finally
			{
				//if (m_Statics != null)
				//    m_Statics.Close();
			}
		}

		/* UOP map files support code, written by Wyatt (c) www.ruosi.org
		 * It's not possible if some entry has unknown hash. Throwed exception
		 * means that EA changed maps UOPs again.
		 */

		#region UOP
		public bool IsUOPFormat { get; set; }
		public bool IsUOPAlreadyRead { get; set; }

		private struct UOPFile
		{
			public readonly long Offset;
			public readonly int Length;

			public UOPFile(long offset, int length)
			{
				Offset = offset;
				Length = length;
			}
		}

		private UOPFile[] UOPFiles { get; set; }
		private long UOPLength { get { return m_Map.Length; } }

		private void ReadUOPFiles(string pattern)
		{
			m_UOPReader = new BinaryReader(m_Map);

			m_UOPReader.BaseStream.Seek(0, SeekOrigin.Begin);

			if (m_UOPReader.ReadInt32() != 0x50594D)
			{
				throw new ArgumentException("Bad UOP file.");
			}

			m_UOPReader.ReadInt64(); // version + signature
			long nextBlock = m_UOPReader.ReadInt64();
			m_UOPReader.ReadInt32(); // block capacity
			int count = m_UOPReader.ReadInt32();

			UOPFiles = new UOPFile[count];

			var hashes = new Dictionary<ulong, int>();

			for (int i = 0; i < count; i++)
			{
				string file = string.Format("build/{0}/{1:D8}.dat", pattern, i);
				ulong hash = FileIndex.HashFileName(file);

				if (!hashes.ContainsKey(hash))
				{
					hashes.Add(hash, i);
				}
			}

			m_UOPReader.BaseStream.Seek(nextBlock, SeekOrigin.Begin);

			do
			{
				int filesCount = m_UOPReader.ReadInt32();
				nextBlock = m_UOPReader.ReadInt64();

				for (int i = 0; i < filesCount; i++)
				{
					long offset = m_UOPReader.ReadInt64();
					int headerLength = m_UOPReader.ReadInt32();
					int compressedLength = m_UOPReader.ReadInt32();
					int decompressedLength = m_UOPReader.ReadInt32();
					ulong hash = m_UOPReader.ReadUInt64();
					m_UOPReader.ReadUInt32(); // Adler32
					short flag = m_UOPReader.ReadInt16();

					int length = flag == 1 ? compressedLength : decompressedLength;

					if (offset == 0)
					{
						continue;
					}

					int idx;
					if (hashes.TryGetValue(hash, out idx))
					{
						if (idx < 0 || idx > UOPFiles.Length)
						{
							throw new IndexOutOfRangeException("hashes dictionary and files collection have different count of entries!");
						}

						UOPFiles[idx] = new UOPFile(offset + headerLength, length);
					}
					else
					{
						throw new ArgumentException(
							string.Format("File with hash 0x{0:X8} was not found in hashes dictionary! EA Mythic changed UOP format!", hash));
					}
				}
			}
			while (m_UOPReader.BaseStream.Seek(nextBlock, SeekOrigin.Begin) != 0);
		}

		private long CalculateOffsetFromUOP(long offset)
		{
			long pos = 0;

			foreach (UOPFile t in UOPFiles)
			{
				long currPos = pos + t.Length;

				if (offset < currPos)
				{
					return t.Offset + (offset - pos);
				}

				pos = currPos;
			}

			return UOPLength;
		}
		#endregion

		private Tile[] ReadLandBlock(int x, int y)
		{
			if (m_Map == null || !m_Map.CanRead || !m_Map.CanSeek)
			{
				if (mapPath == null)
				{
					m_Map = null;
				}
				else
				{
					m_Map = new FileStream(mapPath, FileMode.Open, FileAccess.Read, FileShare.Read);
				}

				if (IsUOPFormat && mapPath != null && !IsUOPAlreadyRead)
				{
					var fi = new FileInfo(mapPath);
					string uopPattern = fi.Name.Replace(fi.Extension, "").ToLowerInvariant();

					ReadUOPFiles(uopPattern);
					IsUOPAlreadyRead = true;
				}
			}
			var tiles = new Tile[64];
			if (m_Map != null)
			{
				long offset = ((x * BlockHeight) + y) * 196 + 4;

				if (IsUOPFormat)
				{
					offset = CalculateOffsetFromUOP(offset);
				}

				m_Map.Seek(offset, SeekOrigin.Begin);

				GCHandle gc = GCHandle.Alloc(tiles, GCHandleType.Pinned);
				try
				{
					if (m_Buffer == null || m_Buffer.Length < 192)
					{
						m_Buffer = new byte[192];
					}

					m_Map.Read(m_Buffer, 0, 192);

					Marshal.Copy(m_Buffer, 0, gc.AddrOfPinnedObject(), 192);
				}
				finally
				{
					gc.Free();
				}
				//m_Map.Close();
			}

			return tiles;
		}

		public void RemoveStaticBlock(int blockx, int blocky)
		{
			if (m_RemovedStaticBlock == null)
			{
				m_RemovedStaticBlock = new bool[BlockWidth][];
			}
			if (m_RemovedStaticBlock[blockx] == null)
			{
				m_RemovedStaticBlock[blockx] = new bool[BlockHeight];
			}
			m_RemovedStaticBlock[blockx][blocky] = true;
			if (m_StaticTiles[blockx] == null)
			{
				m_StaticTiles[blockx] = new HuedTile[BlockHeight][][][];
			}
			m_StaticTiles[blockx][blocky] = EmptyStaticBlock;
		}

		public bool IsStaticBlockRemoved(int blockx, int blocky)
		{
			if (m_RemovedStaticBlock == null)
			{
				return false;
			}
			if (m_RemovedStaticBlock[blockx] == null)
			{
				return false;
			}
			return m_RemovedStaticBlock[blockx][blocky];
		}

		public bool PendingStatic(int blockx, int blocky)
		{
			if (m_StaticTiles_ToAdd == null)
			{
				return false;
			}
			if (m_StaticTiles_ToAdd[blocky] == null)
			{
				return false;
			}
			if (m_StaticTiles_ToAdd[blocky][blockx] == null)
			{
				return false;
			}
			return true;
		}

		public void AddPendingStatic(int blockx, int blocky, StaticTile toadd)
		{
			if (m_StaticTiles_ToAdd == null)
			{
				m_StaticTiles_ToAdd = new List<StaticTile>[BlockHeight][];
			}
			if (m_StaticTiles_ToAdd[blocky] == null)
			{
				m_StaticTiles_ToAdd[blocky] = new List<StaticTile>[BlockWidth];
			}
			if (m_StaticTiles_ToAdd[blocky][blockx] == null)
			{
				m_StaticTiles_ToAdd[blocky][blockx] = new List<StaticTile>();
			}
			m_StaticTiles_ToAdd[blocky][blockx].Add(toadd);
		}

		public StaticTile[] GetPendingStatics(int blockx, int blocky)
		{
			if (m_StaticTiles_ToAdd == null)
			{
				return null;
			}
			if (m_StaticTiles_ToAdd[blocky] == null)
			{
				return null;
			}
			if (m_StaticTiles_ToAdd[blocky][blockx] == null)
			{
				return null;
			}

			return m_StaticTiles_ToAdd[blocky][blockx].ToArray();
		}

		public void Dispose()
		{
			if (m_Map != null)
			{
				m_Map.Close();
			}

			if (m_UOPReader != null)
			{
				m_UOPReader.Close();
			}

			if (m_Statics != null)
			{
				m_Statics.Close();
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct StaticTile
	{
		public ushort m_ID;
		public byte m_X;
		public byte m_Y;
		public sbyte m_Z;
		public short m_Hue;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct HuedTile
	{
		internal sbyte m_Z;
		internal ushort m_ID;
		internal int m_Hue;

		public ushort ID { get { return m_ID; } set { m_ID = value; } }
		public int Hue { get { return m_Hue; } set { m_Hue = value; } }
		public int Z { get { return m_Z; } set { m_Z = (sbyte)value; } }

		public HuedTile(ushort id, short hue, sbyte z)
		{
			m_ID = id;
			m_Hue = hue;
			m_Z = z;
		}

		public void Set(ushort id, short hue, sbyte z)
		{
			m_ID = id;
			m_Hue = hue;
			m_Z = z;
		}
	}

	public struct MTile : IComparable
	{
		internal ushort m_ID;
		internal sbyte m_Z;
		internal sbyte m_Flag;
		internal int m_Unk1;
		internal int m_Solver;

		public ushort ID { get { return m_ID; } }
		public int Z { get { return m_Z; } set { m_Z = (sbyte)value; } }

		public int Flag { get { return m_Flag; } set { m_Flag = (sbyte)value; } }
		public int Unk1 { get { return m_Unk1; } set { m_Unk1 = value; } }
		public int Solver { get { return m_Solver; } set { m_Solver = value; } }

		public MTile(ushort id, sbyte z)
		{
			m_ID = Art.GetLegalItemID(id);
			m_Z = z;
			m_Flag = 1;
			m_Solver = 0;
			m_Unk1 = 0;
		}

		public MTile(ushort id, sbyte z, sbyte flag)
		{
			m_ID = Art.GetLegalItemID(id);
			m_Z = z;
			m_Flag = flag;
			m_Solver = 0;
			m_Unk1 = 0;
		}

		public MTile(ushort id, sbyte z, sbyte flag, int unk1)
		{
			m_ID = Art.GetLegalItemID(id);
			m_Z = z;
			m_Flag = flag;
			m_Solver = 0;
			m_Unk1 = unk1;
		}

		public void Set(ushort id, sbyte z)
		{
			m_ID = Art.GetLegalItemID(id);
			m_Z = z;
		}

		public void Set(ushort id, sbyte z, sbyte flag)
		{
			m_ID = Art.GetLegalItemID(id);
			m_Z = z;
			m_Flag = flag;
		}

		public void Set(ushort id, sbyte z, sbyte flag, int unk1)
		{
			m_ID = Art.GetLegalItemID(id);
			m_Z = z;
			m_Flag = flag;
			m_Unk1 = unk1;
		}

		public int CompareTo(object x)
		{
			if (x == null)
			{
				return 1;
			}

			if (!(x is MTile))
			{
				throw new ArgumentNullException();
			}

			var a = (MTile)x;

			ItemData ourData = TileData.ItemTable[m_ID];
			ItemData theirData = TileData.ItemTable[a.ID];

			int ourTreshold = 0;
			if (ourData.Height > 0)
			{
				++ourTreshold;
			}
			if (!ourData.Background)
			{
				++ourTreshold;
			}
			int ourZ = Z;
			int theirTreshold = 0;
			if (theirData.Height > 0)
			{
				++theirTreshold;
			}
			if (!theirData.Background)
			{
				++theirTreshold;
			}
			int theirZ = a.Z;

			ourZ += ourTreshold;
			theirZ += theirTreshold;
			int res = ourZ - theirZ;
			if (res == 0)
			{
				res = ourTreshold - theirTreshold;
			}
			if (res == 0)
			{
				res = m_Solver - a.Solver;
			}
			return res;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Tile : IComparable
	{
		internal ushort m_ID;
		internal sbyte m_Z;

		public ushort ID { get { return m_ID; } }
		public int Z { get { return m_Z; } set { m_Z = (sbyte)value; } }

		public Tile(ushort id, sbyte z)
		{
			m_ID = id;
			m_Z = z;
		}

		public Tile(ushort id, sbyte z, sbyte flag)
		{
			m_ID = id;
			m_Z = z;
		}

		public void Set(ushort id, sbyte z)
		{
			m_ID = id;
			m_Z = z;
		}

		public void Set(ushort id, sbyte z, sbyte flag)
		{
			m_ID = id;
			m_Z = z;
		}

		public int CompareTo(object x)
		{
			if (x == null)
			{
				return 1;
			}

			if (!(x is Tile))
			{
				throw new ArgumentNullException();
			}

			var a = (Tile)x;

			if (m_Z > a.m_Z)
			{
				return 1;
			}
			else if (a.m_Z > m_Z)
			{
				return -1;
			}

			ItemData ourData = TileData.ItemTable[m_ID];
			ItemData theirData = TileData.ItemTable[a.m_ID];

			if (ourData.Height > theirData.Height)
			{
				return 1;
			}
			else if (theirData.Height > ourData.Height)
			{
				return -1;
			}

			if (ourData.Background && !theirData.Background)
			{
				return -1;
			}
			else if (theirData.Background && !ourData.Background)
			{
				return 1;
			}

			return 0;
		}
	}
}