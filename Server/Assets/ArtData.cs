#region References
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
#endregion

namespace Server
{
	public static class ArtData
	{
		private static readonly AutoResetEvent m_RenderSync = new AutoResetEvent(true);
		private static readonly AutoResetEvent m_MeasureSync = new AutoResetEvent(true);

		private static byte[] m_StreamBuffer;

		private static readonly FileIndex m_FileIndex;

		private static readonly Bitmap[] m_Cache = new Bitmap[81920];
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

		public static Bitmap GetStatic(int index, int hue, bool onlyHueGrayPixels)
		{
			try
			{
				var orig = GetStatic(index);

				if (orig == null)
					return null;

				var image = new Bitmap(orig);

				HueData.ApplyTo(image, hue, onlyHueGrayPixels);

				return image;
			}
			catch (Exception e)
			{
				if (Core.Debug)
					Console.WriteLine($"[Ultima]: ArtData.GetStatic({nameof(index)}:{index}, {nameof(hue)}:{hue}, {nameof(onlyHueGrayPixels)}:{onlyHueGrayPixels})\n{e}");

				return null;
			}
		}

		public static unsafe Bitmap GetStatic(int index)
		{
			m_RenderSync.WaitOne();

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

					var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
					var bd = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);

					try
					{
						var line = (int*)bd.Scan0;
						var delta = bd.Stride >> 2;

						for (var y = 0; y < height; ++y, line += delta)
						{
							count = lookups[y];

							int* cur = line, end;
							int xOffset, xRun, c, argb;

							while (((xOffset = dat[count++]) + (xRun = dat[count++])) != 0)
							{
								if (xOffset > delta)
									break;

								cur += xOffset;

								if (xOffset + xRun > delta)
									break;

								end = cur + xRun;

								while (cur < end)
								{
									c = dat[count++] ^ 0x8000;

									argb = ((c & 0x7C00) << 9) | ((c & 0x03E0) << 6) | ((c & 0x1F) << 3);
									argb = ((c & 0x8000) * 0x1FE00) | argb | ((argb >> 5) & 0x070707);

									*cur++ = argb;
								}
							}
						}
					}
					finally
					{
						bmp.UnlockBits(bd);
					}

					return m_Cache[index] = bmp;
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
				m_RenderSync.Set();
			}
		}

		public static unsafe void Measure(Bitmap bmp, out int xMin, out int yMin, out int xMax, out int yMax)
		{
			m_MeasureSync.WaitOne();

			xMin = yMin = 0;
			xMax = yMax = -1;

			try
			{
				if (bmp == null || bmp.Width <= 0 || bmp.Height <= 0)
					return;

				var bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

				try
				{
					var lineDelta = bd.Stride >> 2;
					var pBuffer = (uint*)bd.Scan0;
					var pLineEnd = pBuffer + bd.Width;
					var pEnd = pBuffer + (bd.Height * lineDelta);
					var delta = lineDelta - bd.Width;

					var foundPixel = false;

					int x = 0, y = 0;

					while (pBuffer < pEnd)
					{
						while (pBuffer < pLineEnd)
						{
							var c = *pBuffer++;

							if ((c & 0x80000000) != 0)
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

									if (y < yMin)
										yMin = y;

									if (x > xMax)
										xMax = x;

									if (y > yMax)
										yMax = y;
								}
							}

							++x;
						}

						pBuffer += delta;
						pLineEnd += lineDelta;

						++y;
						x = 0;
					}
				}
				finally
				{
					bmp.UnlockBits(bd);
				}
			}
			catch (Exception e)
			{
				if (Core.Debug)
					Console.WriteLine($"[Ultima]: ArtData.Measure(...)\n{e}");
			}
			finally
			{
				m_MeasureSync.Set();
			}
		}
	}
}