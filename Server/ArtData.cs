#region References
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
#endregion

namespace Server
{
	public static class ArtData
	{
#if MONO
		public const PixelFormat PixelFormat = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
#else
		public const PixelFormat PixelFormat = System.Drawing.Imaging.PixelFormat.Format16bppArgb1555;
#endif

		private static readonly FileIndex m_FileIndex;

		private static readonly Bitmap[] m_Cache = new Bitmap[0x14000];

		private static readonly bool[] m_Removed = new bool[0x14000];

		private static readonly Hashtable m_Patched = new Hashtable();

		public static bool Modified { get; }

		private static byte[] m_StreamBuffer;

		public static bool UOP => File.Exists(Core.FindDataFile("artLegacyMUL.uop"));
		public static bool MUL => File.Exists(Core.FindDataFile("art.mul"));

		static ArtData()
		{
			if (UOP)
			{
				m_FileIndex = new FileIndex("artLegacyMUL.uop", 0x14000, ".tga", 0x13FDC, false);
			}
			else if (MUL)
			{
				m_FileIndex = new FileIndex("artidx.mul", "art.mul", 0x14000);
			}
		}

		public static int GetMaxItemID()
		{
			if (GetIdxLength() >= 0x13FDC)
			{
				return 0xFFFF;
			}

			if (GetIdxLength() >= 0xC000)
			{
				return 0x7FFF;
			}

			return 0x3FFF;
		}

		public static ushort GetLegalItemID(int itemID, bool checkmaxid = true)
		{
			if (itemID < 0)
			{
				return 0;
			}

			if (checkmaxid)
			{
				var max = GetMaxItemID();

				if (itemID > max)
				{
					return 0;
				}
			}

			return (ushort)itemID;
		}

		public static int GetIdxLength()
		{
			return (int)(m_FileIndex.IdxLength / 12);
		}

		public static Bitmap GetStatic(int index, int hue, bool onlyHueGrayPixels)
		{
			return GetStatic(index, hue, onlyHueGrayPixels, out _, true);
		}

		public static Bitmap GetStatic(int index, int hue, bool onlyHueGrayPixels, out bool patched)
		{
			return GetStatic(index, hue, onlyHueGrayPixels, out patched, true);
		}

		public static Bitmap GetStatic(int index, int hue, bool onlyHueGrayPixels, bool checkmaxid)
		{
			return GetStatic(index, hue, onlyHueGrayPixels, out _, checkmaxid);
		}

		public static Bitmap GetStatic(int index, int hue, bool onlyHueGrayPixels, out bool patched, bool checkmaxid)
		{
			try
			{
				var orig = GetStatic(index, out patched);

				if (orig == null)
					return null;

				var image = new Bitmap(orig);

				HueData.ApplyTo(image, hue, onlyHueGrayPixels);

				return image;
			}
			catch (Exception e)
			{
				patched = false;

				if (Core.Debug)
				{
					Console.WriteLine($"[Ultima]: ArtData.GetStatic({nameof(index)}:{index}, {nameof(hue)}:{hue}, {nameof(onlyHueGrayPixels)}:{onlyHueGrayPixels}, ...)\n{e}");
				}

				return null;
			}
		}

		public static Bitmap GetStatic(int index)
		{
			return GetStatic(index, out _, true);
		}

		public static Bitmap GetStatic(int index, out bool patched)
		{
			return GetStatic(index, out patched, true);
		}

		public static Bitmap GetStatic(int index, bool checkmaxid)
		{
			return GetStatic(index, out _, checkmaxid);
		}

		public static Bitmap GetStatic(int index, out bool patched, bool checkmaxid)
		{
			try
			{
				index = GetLegalItemID(index, checkmaxid);
				index += 0x4000;

				if (m_Patched.Contains(index))
				{
					patched = (bool)m_Patched[index];
				}
				else
				{
					patched = false;
				}

				if (m_Removed[index])
				{
					return null;
				}

				if (m_Cache[index] != null)
				{
					return m_Cache[index];
				}

				var stream = m_FileIndex.Seek(index, out var length, out var extra, out patched);

				if (stream == null)
				{
					return null;
				}

				if (patched)
				{
					m_Patched[index] = true;
				}

				return m_Cache[index] = LoadStatic(stream, length);
			}
			catch (Exception e)
			{
				patched = false;

				if (Core.Debug)
				{
					Console.WriteLine($"[Ultima]: ArtData.GetStatic({nameof(index)}:{index}, ...)\n{e}");
				}

				return null;
			}
		}

		public static unsafe void Measure(Bitmap bmp, out int xMin, out int yMin, out int xMax, out int yMax)
		{
			xMin = yMin = 0;
			xMax = yMax = -1;

			try
			{
				if (bmp == null || bmp.Width <= 0 || bmp.Height <= 0)
				{
					return;
				}

				var bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat);

				try
				{
					var delta = (bd.Stride >> 1) - bd.Width;

					var lineDelta = bd.Stride >> 1;

					var pBuffer = (ushort*)bd.Scan0;

					var pLineEnd = pBuffer + bd.Width;

					var pEnd = pBuffer + (bd.Height * lineDelta);

					var foundPixel = false;

					int x = 0, y = 0;

					while (pBuffer < pEnd)
					{
						while (pBuffer < pLineEnd)
						{
							var c = *pBuffer++;

							if ((c & 0x8000) != 0)
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
									{
										xMin = x;
									}

									if (y < yMin)
									{
										yMin = y;
									}

									if (x > xMax)
									{
										xMax = x;
									}

									if (y > yMax)
									{
										yMax = y;
									}
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
				{
					Console.WriteLine($"[Ultima]: ArtData.Measure(...)\n{e}");
				}
			}
		}

		private static unsafe Bitmap LoadStatic(Stream stream, int length)
		{
			Bitmap bmp;

			if (m_StreamBuffer == null || m_StreamBuffer.Length < length)
			{
				m_StreamBuffer = new byte[length];
			}

			stream.Read(m_StreamBuffer, 0, length);

			stream.Close();

			fixed (byte* data = m_StreamBuffer)
			{
				var bindata = (ushort*)data;

				var count = 2;

				int width = bindata[count++];

				int height = bindata[count++];

				if (width <= 0 || height <= 0)
				{
					return null;
				}

				var lookups = new int[height];

				var start = height + 4;

				for (var i = 0; i < height; ++i)
				{
					lookups[i] = start + bindata[count++];
				}

				bmp = new Bitmap(width, height, PixelFormat);

				var bd = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat);

				try
				{
					var line = (ushort*)bd.Scan0;

					var delta = bd.Stride >> 1;

					for (var y = 0; y < height; ++y, line += delta)
					{
						count = lookups[y];

						ushort* cur = line, end;

						int xOffset, xRun;

						while (((xOffset = bindata[count++]) + (xRun = bindata[count++])) != 0)
						{
							if (xOffset > delta)
							{
								break;
							}

							cur += xOffset;

							if (xOffset + xRun > delta)
							{
								break;
							}

							end = cur + xRun;

							while (cur < end)
							{
								*cur++ = (ushort)(bindata[count++] ^ 0x8000);
							}
						}
					}
				}
				finally
				{
					bmp.UnlockBits(bd);
				}
			}

			return bmp;
		}
	}
}
