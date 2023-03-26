#region References
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#endregion

namespace Server
{
	public class TileMatrix
	{
		private static readonly List<TileMatrix> m_Instances = new List<TileMatrix>();

		private readonly StaticTile[][][][][] m_StaticTiles;
		private readonly LandTile[][][] m_LandTiles;

		private readonly LandTile[] m_InvalidLandBlock;

		private readonly UOPIndex m_MapIndex;

		private readonly int m_FileIndex;
		private readonly int m_Width, m_Height;

		private readonly Map m_Owner;
		private readonly int[][] m_StaticPatches;
		private readonly int[][] m_LandPatches;

		public int BlockWidth { get; }
		public int BlockHeight { get; }

		public TileMatrixPatch Patch { get; }

		public BinaryReader IndexReader { get; set; }

		public FileStream MapStream { get; set; }
		public FileStream IndexStream { get; set; }
		public FileStream DataStream { get; set; }

		public bool Exists => MapStream != null && IndexStream != null && DataStream != null;

		private readonly List<TileMatrix> m_FileShare = new List<TileMatrix>();

		public TileMatrix(Map owner, int fileIndex, int mapID, int width, int height)
		{
			lock (m_Instances)
			{
				for (var i = 0; i < m_Instances.Count; ++i)
				{
					var tm = m_Instances[i];

					if (tm.m_FileIndex == fileIndex)
					{
						lock (m_FileShare)
						{
							lock (tm.m_FileShare)
							{
								tm.m_FileShare.Add(this);
								m_FileShare.Add(tm);
							}
						}
					}
				}

				m_Instances.Add(this);
			}

			m_FileIndex = fileIndex;
			m_Width = width;
			m_Height = height;

			BlockWidth = width >> 3;
			BlockHeight = height >> 3;

			m_Owner = owner;

			if (fileIndex != 0x7F)
			{
				var mapPath = Core.FindDataFile($"map{fileIndex}LegacyMUL.uop") ?? Core.FindDataFile($"map{fileIndex}.mul");

				if (mapPath == null)
					throw new FileNotFoundException($"Could not load map file for {fileIndex}:{owner?.Name ?? "null"}");

				MapStream = new FileStream(mapPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

				if (Insensitive.EndsWith(mapPath, "uop"))
				{
					Console.WriteLine($"UOP format detected for {fileIndex}:{owner?.Name ?? "null"}");

					m_MapIndex = new UOPIndex(MapStream);
				}

				var indexPath = Core.FindDataFile("staidx{0}.mul", fileIndex);

				if (indexPath != null)
				{
					IndexStream = new FileStream(indexPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					IndexReader = new BinaryReader(IndexStream);
				}

				var staticsPath = Core.FindDataFile("statics{0}.mul", fileIndex);

				if (staticsPath != null)
				{
					DataStream = new FileStream(staticsPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				}
			}

			EmptyStaticBlock = new StaticTile[8][][];

			for (var i = 0; i < 8; ++i)
			{
				EmptyStaticBlock[i] = new StaticTile[8][];

				for (var j = 0; j < 8; ++j)
				{
					EmptyStaticBlock[i][j] = new StaticTile[0];
				}
			}

			m_InvalidLandBlock = new LandTile[196];

			m_LandTiles = new LandTile[BlockWidth][][];
			m_StaticTiles = new StaticTile[BlockWidth][][][][];
			m_StaticPatches = new int[BlockWidth][];
			m_LandPatches = new int[BlockWidth][];

			Patch = new TileMatrixPatch(this, mapID);
		}

		public StaticTile[][][] EmptyStaticBlock { get; }


		[MethodImpl(MethodImplOptions.Synchronized)]
		public void SetStaticBlock(int x, int y, StaticTile[][][] value)
		{
			if (x < 0 || y < 0 || x >= BlockWidth || y >= BlockHeight)
			{
				return;
			}

			if (m_StaticTiles[x] == null)
			{
				m_StaticTiles[x] = new StaticTile[BlockHeight][][][];
			}

			m_StaticTiles[x][y] = value;

			if (m_StaticPatches[x] == null)
			{
				m_StaticPatches[x] = new int[(BlockHeight + 31) >> 5];
			}

			m_StaticPatches[x][y >> 5] |= 1 << (y & 0x1F);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public StaticTile[][][] GetStaticBlock(int x, int y)
		{
			if (x < 0 || y < 0 || x >= BlockWidth || y >= BlockHeight || DataStream == null || IndexStream == null)
			{
				return EmptyStaticBlock;
			}

			if (m_StaticTiles[x] == null)
			{
				m_StaticTiles[x] = new StaticTile[BlockHeight][][][];
			}

			var tiles = m_StaticTiles[x][y];

			if (tiles == null)
			{
				lock (m_FileShare)
				{
					for (var i = 0; tiles == null && i < m_FileShare.Count; ++i)
					{
						var shared = m_FileShare[i];

						lock (shared)
						{
							if (x >= 0 && x < shared.BlockWidth && y >= 0 && y < shared.BlockHeight)
							{
								var theirTiles = shared.m_StaticTiles[x];

								if (theirTiles != null)
								{
									tiles = theirTiles[y];
								}

								if (tiles != null)
								{
									var theirBits = shared.m_StaticPatches[x];

									if (theirBits != null && (theirBits[y >> 5] & (1 << (y & 0x1F))) != 0)
									{
										tiles = null;
									}
								}
							}
						}
					}
				}

				if (tiles == null)
				{
					tiles = ReadStaticBlock(x, y);
				}

				m_StaticTiles[x][y] = tiles;
			}

			return tiles;
		}

		public StaticTile[] GetStaticTiles(int x, int y)
		{
			var tiles = GetStaticBlock(x >> 3, y >> 3);

			return tiles[x & 0x7][y & 0x7];
		}

		private readonly TileList m_TilesList = new TileList();

		[MethodImpl(MethodImplOptions.Synchronized)]
		public StaticTile[] GetStaticTiles(int x, int y, bool multis)
		{
			var tiles = GetStaticBlock(x >> 3, y >> 3);

			if (multis)
			{
				var any = false;

				foreach (var multiTiles in m_Owner.GetMultiTilesAt(x, y))
				{
					if (!any)
					{
						any = true;
					}

					m_TilesList.AddRange(multiTiles);
				}

				if (any)
				{
					m_TilesList.AddRange(tiles[x & 0x7][y & 0x7]);

					return m_TilesList.ToArray();
				}
			}

			return tiles[x & 0x7][y & 0x7];
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void SetLandBlock(int x, int y, LandTile[] value)
		{
			if (x < 0 || y < 0 || x >= BlockWidth || y >= BlockHeight)
			{
				return;
			}

			if (m_LandTiles[x] == null)
			{
				m_LandTiles[x] = new LandTile[BlockHeight][];
			}

			m_LandTiles[x][y] = value;

			if (m_LandPatches[x] == null)
			{
				m_LandPatches[x] = new int[(BlockHeight + 31) >> 5];
			}

			m_LandPatches[x][y >> 5] |= 1 << (y & 0x1F);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public LandTile[] GetLandBlock(int x, int y)
		{
			if (x < 0 || y < 0 || x >= BlockWidth || y >= BlockHeight || MapStream == null)
			{
				return m_InvalidLandBlock;
			}

			if (m_LandTiles[x] == null)
			{
				m_LandTiles[x] = new LandTile[BlockHeight][];
			}

			var tiles = m_LandTiles[x][y];

			if (tiles == null)
			{
				lock (m_FileShare)
				{
					for (var i = 0; tiles == null && i < m_FileShare.Count; ++i)
					{
						var shared = m_FileShare[i];

						lock (shared)
						{
							if (x >= 0 && x < shared.BlockWidth && y >= 0 && y < shared.BlockHeight)
							{
								var theirTiles = shared.m_LandTiles[x];

								if (theirTiles != null)
								{
									tiles = theirTiles[y];
								}

								if (tiles != null)
								{
									var theirBits = shared.m_LandPatches[x];

									if (theirBits != null && (theirBits[y >> 5] & (1 << (y & 0x1F))) != 0)
									{
										tiles = null;
									}
								}
							}
						}
					}
				}

				if (tiles == null)
				{
					tiles = ReadLandBlock(x, y);
				}

				m_LandTiles[x][y] = tiles;
			}

			return tiles;
		}

		public LandTile GetLandTile(int x, int y)
		{
			var tiles = GetLandBlock(x >> 3, y >> 3);

			return tiles[((y & 0x7) << 3) + (x & 0x7)];
		}

		private TileList[][] m_Lists;

		private StaticTile[] m_TileBuffer = new StaticTile[128];

		[MethodImpl(MethodImplOptions.Synchronized)]
		private unsafe StaticTile[][][] ReadStaticBlock(int x, int y)
		{
			try
			{
				IndexReader.BaseStream.Seek(((x * BlockHeight) + y) * 12, SeekOrigin.Begin);

				var lookup = IndexReader.ReadInt32();
				var length = IndexReader.ReadInt32();

				if (lookup < 0 || length <= 0)
				{
					return EmptyStaticBlock;
				}

				var count = length / 7;

				if (m_TileBuffer.Length < count)
				{
					m_TileBuffer = new StaticTile[count];
				}

				var staTiles = m_TileBuffer; //new StaticTile[tileCount];

				fixed (StaticTile* pTiles = staTiles)
				{
					NativeReader.Read(DataStream, lookup, pTiles, length);

					if (m_Lists == null)
					{
						m_Lists = new TileList[8][];

						for (var i = 0; i < 8; ++i)
						{
							m_Lists[i] = new TileList[8];

							for (var j = 0; j < 8; ++j)
							{
								m_Lists[i][j] = new TileList();
							}
						}
					}

					var lists = m_Lists;

					StaticTile* pCur = pTiles, pEnd = pTiles + count;

					while (pCur < pEnd)
					{
						lists[pCur->m_X & 0x7][pCur->m_Y & 0x7].Add(pCur->m_ID, pCur->m_Z);

						++pCur;
					}

					var tiles = new StaticTile[8][][];

					for (var i = 0; i < 8; ++i)
					{
						tiles[i] = new StaticTile[8][];

						for (var j = 0; j < 8; ++j)
						{
							tiles[i][j] = lists[i][j].ToArray();
						}
					}

					return tiles;
				}
			}
			catch (EndOfStreamException ex)
			{
				Diagnostics.ExceptionLogging.LogException(ex);

				if (DateTime.UtcNow >= m_NextStaticWarning)
				{
					Console.WriteLine("Warning: Static EOS for {0} ({1}, {2})", m_Owner, x, y);
					m_NextStaticWarning = DateTime.UtcNow + TimeSpan.FromMinutes(1.0);
				}

				return EmptyStaticBlock;
			}
		}

		private DateTime m_NextStaticWarning;
		private DateTime m_NextLandWarning;

		public void Force()
		{
			if (ScriptCompiler.Assemblies == null || ScriptCompiler.Assemblies.Length == 0)
			{
				throw new Exception();
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private unsafe LandTile[] ReadLandBlock(int x, int y)
		{
			try
			{
				var tiles = new LandTile[64];

				var size = sizeof(LandTile);
				var offset = (((x * BlockHeight) + y) * ((tiles.Length * size) + 4)) + 4;

				if (m_MapIndex != null)
				{
					offset = m_MapIndex.Lookup(offset);
				}

				fixed (LandTile* pTiles = tiles)
				{
					NativeReader.Read(MapStream, offset, pTiles, tiles.Length * size);
				}

				return tiles;
			}
			catch (Exception ex)
			{
				Diagnostics.ExceptionLogging.LogException(ex);

				if (DateTime.UtcNow >= m_NextLandWarning)
				{
					Console.WriteLine("Warning: Land EOS for {0} ({1}, {2})", m_Owner, x, y);
					m_NextLandWarning = DateTime.UtcNow + TimeSpan.FromMinutes(1.0);
				}

				return m_InvalidLandBlock;
			}
		}

		public void Dispose()
		{
			if (m_MapIndex != null)
			{
				m_MapIndex.Close();
			}
			else if (MapStream != null)
			{
				MapStream.Close();
			}

			if (DataStream != null)
			{
				DataStream.Close();
			}

			if (IndexReader != null)
			{
				IndexReader.Close();
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 3)]
	public struct LandTile
	{
		internal short m_ID;
		internal sbyte m_Z;

		public int ID => m_ID;

		public int Z
		{
			get => m_Z;
			set => m_Z = (sbyte)value;
		}

		public int Height => 0;

		public bool Ignored => m_ID == 2 || m_ID == 0x1DB || (m_ID >= 0x1AE && m_ID <= 0x1B5);

		public LandTile(short id, sbyte z)
		{
			m_ID = id;
			m_Z = z;
		}

		public void Set(short id, sbyte z)
		{
			m_ID = id;
			m_Z = z;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = m_ID;

				hash = (hash * 397) ^ m_Z;

				return hash;
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 7)]
	public struct StaticTile
	{
		internal ushort m_ID;
		internal byte m_X;
		internal byte m_Y;
		internal sbyte m_Z;
		internal short m_Hue;

		public int ID => m_ID;

		public int X { get => m_X; set => m_X = (byte)value; }
		public int Y { get => m_Y; set => m_Y = (byte)value; }
		public int Z { get => m_Z; set => m_Z = (sbyte)value; }
		public int Hue { get => m_Hue; set => m_Hue = (short)value; }

		public int Height => TileData.ItemTable[m_ID & TileData.MaxItemValue].Height;

		public bool Ignored => m_ID <= 1 || m_ID == 0x1796;

		public StaticTile(ushort id, sbyte z)
		{
			m_ID = id;
			m_Z = z;

			m_X = 0;
			m_Y = 0;
			m_Hue = 0;
		}

		public StaticTile(ushort id, byte x, byte y, sbyte z, short hue)
		{
			m_ID = id;
			m_X = x;
			m_Y = y;
			m_Z = z;
			m_Hue = hue;
		}

		public void Set(ushort id, sbyte z)
		{
			m_ID = id;
			m_Z = z;
		}

		public void Set(ushort id, byte x, byte y, sbyte z, short hue)
		{
			m_ID = id;
			m_X = x;
			m_Y = y;
			m_Z = z;
			m_Hue = hue;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = m_ID;

				hash = (hash * 397) ^ m_X;
				hash = (hash * 397) ^ m_Y;
				hash = (hash * 397) ^ m_Z;
				hash = (hash * 397) ^ m_Hue;

				return hash;
			}
		}
	}

	public class UOPIndex
	{
		private class UOPEntry : IComparable<UOPEntry>
		{
			public int m_Offset;
			public readonly int m_Length;
			public int m_Order;

			public UOPEntry(int offset, int length)
			{
				m_Offset = offset;
				m_Length = length;
				m_Order = 0;
			}

			public int CompareTo(UOPEntry other)
			{
				return m_Order.CompareTo(other.m_Order);
			}
		}

		private class OffsetComparer : IComparer<UOPEntry>
		{
			public static readonly IComparer<UOPEntry> Instance = new OffsetComparer();

			public int Compare(UOPEntry x, UOPEntry y)
			{
				return x.m_Offset.CompareTo(y.m_Offset);
			}
		}

		private readonly BinaryReader m_Reader;
		private readonly int m_Length;
		private readonly UOPEntry[] m_Entries;

		public int Version { get; }

		public UOPIndex(FileStream stream)
		{
			m_Reader = new BinaryReader(stream);
			m_Length = (int)stream.Length;

			if (m_Reader.ReadInt32() != 0x50594D)
			{
				throw new ArgumentException("Invalid UOP file.");
			}

			Version = m_Reader.ReadInt32();
			m_Reader.ReadInt32();
			var nextTable = m_Reader.ReadInt32();

			var entries = new List<UOPEntry>();

			do
			{
				stream.Seek(nextTable, SeekOrigin.Begin);
				var count = m_Reader.ReadInt32();
				nextTable = m_Reader.ReadInt32();
				m_Reader.ReadInt32();

				for (var i = 0; i < count; ++i)
				{
					var offset = m_Reader.ReadInt32();

					if (offset == 0)
					{
						stream.Seek(30, SeekOrigin.Current);
						continue;
					}

					m_Reader.ReadInt64();
					var length = m_Reader.ReadInt32();

					entries.Add(new UOPEntry(offset, length));

					stream.Seek(18, SeekOrigin.Current);
				}
			}
			while (nextTable != 0 && nextTable < m_Length);

			entries.Sort(OffsetComparer.Instance);

			for (var i = 0; i < entries.Count; ++i)
			{
				stream.Seek(entries[i].m_Offset + 2, SeekOrigin.Begin);

				int dataOffset = m_Reader.ReadInt16();
				entries[i].m_Offset += 4 + dataOffset;

				stream.Seek(dataOffset, SeekOrigin.Current);
				entries[i].m_Order = m_Reader.ReadInt32();
			}

			entries.Sort();
			m_Entries = entries.ToArray();
		}

		public int Lookup(int offset)
		{
			var total = 0;

			for (var i = 0; i < m_Entries.Length; ++i)
			{
				var newTotal = total + m_Entries[i].m_Length;

				if (offset < newTotal)
				{
					return m_Entries[i].m_Offset + (offset - total);
				}

				total = newTotal;
			}

			return m_Length;
		}

		public void Close()
		{
			m_Reader.Close();
		}
	}
}
