#region References
using System;
using System.Drawing;
using System.Threading;

#endregion

namespace Server
{
	public static class ArtData
	{
		private static readonly AutoResetEvent m_readSync = new AutoResetEvent(true);

		private static byte[] m_StreamBuffer;

		private static readonly FileIndex m_FileIndex;

		private static readonly ArtInfo[] m_Cache = new ArtInfo[81920];
		private static readonly bool[] m_Invalid = new bool[81920];

		public static bool UOP => Core.FindDataFile("artLegacyMUL.uop") != null;
		public static bool MUL => Core.FindDataFile("art.mul") != null;

		public static int MaxLandID { get; }
		public static int MaxItemID { get; }

		static ArtData()
		{
			if (UOP)
				m_FileIndex = new FileIndex("artLegacyMUL.uop", 81920, ".tga", false);
			else if (MUL)
				m_FileIndex = new FileIndex("artidx.mul", "art.mul", 81920);

			if (m_FileIndex != null)
			{
				MaxLandID = 16383;
				MaxItemID = m_FileIndex.IdxCount - (MaxLandID + 1);
			}
		}
		
		public static unsafe ArtInfo GetStatic(int index)
		{
			m_readSync.WaitOne();

			try
			{
				if (index < 0 || index > MaxItemID)
					return null;

				index += 16384;

				if (m_Invalid[index])
					return null;

				if (m_Cache[index] != null)
					return m_Cache[index];

				if (!m_FileIndex.Seek(index, ref m_StreamBuffer, out var length, out var extra))
				{
					m_Invalid[index] = true;
					return null;
				}

				fixed (byte* data = m_StreamBuffer)
				{
					var dat = (ushort*)data;

					var count = 2;

					int width = dat[count++];
					int height = dat[count++];

					if (width <= 0 || height <= 0)
						return null;

					var lookups = new int[height];

					var start = height + 4;

					for (var i = 0; i < height; ++i)
						lookups[i] = start + dat[count++];

					var xMin = 0;
					var yMin = 0;
					var xMax = -1;
					var yMax = -1;
					var foundPixel = false;

					try
					{
						for (var y = 0; y < height; y++)
						{
							count = lookups[y];

							int x = 0, xOffset, xRun;
							
							while (((xOffset = dat[count++]) + (xRun = dat[count++])) != 0)
							{
								if (xOffset > width)
									break;
								
								x += xOffset;
								
								if (xOffset + xRun > width)
									break;
								
								var end = x + xRun;

								while (x < end)
								{
									var c = dat[count++];
									if (c != 0)
									{
										if (!foundPixel)
										{
											foundPixel = true;
											xMin = xMax = x;
											yMin = yMax = y;
										}
										else
										{
											if (x < xMin)
												xMin = x;
											if (x > xMax)
												xMax = x;
											if (y < yMin)
												yMin = y;
											if (y > yMax)
												yMax = y;
										}
									}
									x++;
								}
							}
						}
					}
					catch (Exception e)
					{
						if (Core.Debug)
							Console.WriteLine($"[Ultima]: ArtData.GetStatic({nameof(index)}:{index})\n{e}");

						m_Invalid[index] = true;

						return null;
					}
					return m_Cache[index] = new ArtInfo(width,
						height,
						new Rectangle(xMin, yMin, xMax - xMin, yMax - yMin));
				}
			}
			catch (Exception e)
			{
				if (Core.Debug)
					Console.WriteLine($"[Ultima]: ArtData.GetStatic({nameof(index)}:{index})\n{e}");

				return null;
			}
			finally
			{
				m_readSync.Set();
			}
		}
	}
}
