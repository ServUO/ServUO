#region References
using System.IO;
using System.Runtime.CompilerServices;
#endregion

namespace Server
{
	public class TileMatrixPatch
	{
		private readonly int m_LandBlocks;
		private readonly int m_StaticBlocks;

		private static bool m_Enabled = true;

		public static bool Enabled { get => m_Enabled; set => m_Enabled = value; }

		public int LandBlocks
		{
			get
			{
				lock (this)
					return m_LandBlocks;
			}
		}

		public int StaticBlocks
		{
			get
			{
				lock (this)
					return m_StaticBlocks;
			}
		}

		public TileMatrixPatch(TileMatrix matrix, int index)
		{
			if (!m_Enabled)
			{
				return;
			}

			var mapDataPath = Core.FindDataFile("mapdif{0}.mul", index);
			var mapIndexPath = Core.FindDataFile("mapdifl{0}.mul", index);

			if (File.Exists(mapDataPath) && File.Exists(mapIndexPath))
			{
				m_LandBlocks = PatchLand(matrix, mapDataPath, mapIndexPath);
			}

			var staDataPath = Core.FindDataFile("stadif{0}.mul", index);
			var staIndexPath = Core.FindDataFile("stadifl{0}.mul", index);
			var staLookupPath = Core.FindDataFile("stadifi{0}.mul", index);

			if (File.Exists(staDataPath) && File.Exists(staIndexPath) && File.Exists(staLookupPath))
			{
				m_StaticBlocks = PatchStatics(matrix, staDataPath, staIndexPath, staLookupPath);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private unsafe int PatchLand(TileMatrix matrix, string dataPath, string indexPath)
		{
			using (var fsData = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (var fsIndex = new FileStream(indexPath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					var indexReader = new BinaryReader(fsIndex);

					var count = (int)(indexReader.BaseStream.Length / 4);

					for (var i = 0; i < count; ++i)
					{
						var blockID = indexReader.ReadInt32();
						var x = blockID / matrix.BlockHeight;
						var y = blockID % matrix.BlockHeight;

						fsData.Seek(4, SeekOrigin.Current);

						var tiles = new LandTile[64];

						fixed (LandTile* pTiles = tiles)
						{
							NativeReader.Read(fsData, pTiles, 192);
						}

						matrix.SetLandBlock(x, y, tiles);
					}

					indexReader.Close();

					return count;
				}
			}
		}

		private StaticTile[] m_TileBuffer = new StaticTile[128];

		[MethodImpl(MethodImplOptions.Synchronized)]
		private unsafe int PatchStatics(TileMatrix matrix, string dataPath, string indexPath, string lookupPath)
		{
			using (var fsData = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (var fsIndex = new FileStream(indexPath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					using (var fsLookup = new FileStream(lookupPath, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						var indexReader = new BinaryReader(fsIndex);
						var lookupReader = new BinaryReader(fsLookup);

						var count = (int)(indexReader.BaseStream.Length / 4);

						var lists = new TileList[8][];

						for (var x = 0; x < 8; ++x)
						{
							lists[x] = new TileList[8];

							for (var y = 0; y < 8; ++y)
							{
								lists[x][y] = new TileList();
							}
						}

						for (var i = 0; i < count; ++i)
						{
							var blockID = indexReader.ReadInt32();
							var blockX = blockID / matrix.BlockHeight;
							var blockY = blockID % matrix.BlockHeight;

							var offset = lookupReader.ReadInt32();
							var length = lookupReader.ReadInt32();
							lookupReader.ReadInt32(); // Extra

							if (offset < 0 || length <= 0)
							{
								matrix.SetStaticBlock(blockX, blockY, matrix.EmptyStaticBlock);
								continue;
							}

							fsData.Seek(offset, SeekOrigin.Begin);

							var tileCount = length / 7;

							if (m_TileBuffer.Length < tileCount)
							{
								m_TileBuffer = new StaticTile[tileCount];
							}

							var staTiles = m_TileBuffer;

							fixed (StaticTile* pTiles = staTiles)
							{
								NativeReader.Read(fsData, pTiles, length);

								StaticTile* pCur = pTiles, pEnd = pTiles + tileCount;

								while (pCur < pEnd)
								{
									lists[pCur->m_X & 0x7][pCur->m_Y & 0x7].Add(pCur->m_ID, pCur->m_Z);
									pCur = pCur + 1;
								}

								var tiles = new StaticTile[8][][];

								for (var x = 0; x < 8; ++x)
								{
									tiles[x] = new StaticTile[8][];

									for (var y = 0; y < 8; ++y)
									{
										tiles[x][y] = lists[x][y].ToArray();
									}
								}

								matrix.SetStaticBlock(blockX, blockY, tiles);
							}
						}

						indexReader.Close();
						lookupReader.Close();

						return count;
					}
				}
			}
		}
	}
}
