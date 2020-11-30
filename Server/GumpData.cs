#region References

using System;
using System.Collections;

using System.Drawing;

using System.Drawing.Imaging;

using System.IO;

#endregion

namespace Server
{
	public static class GumpData
	{
#if MONO
		public const PixelFormat PixelFormat = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
#else
		public const PixelFormat PixelFormat = System.Drawing.Imaging.PixelFormat.Format16bppArgb1555;
#endif

		private static readonly FileIndex m_FileIndex;

		private static byte[] m_StreamBuffer;

		private static readonly Bitmap[] m_Cache = new Bitmap[0xFFFF];

		private static readonly bool[] m_Removed = new bool[0xFFFF];

		private static readonly Hashtable m_Patched = new Hashtable();

		public static int Length => m_Cache.Length;

		public static bool UOP => File.Exists(Core.FindDataFile("gumpartLegacyMUL.uop"));
		public static bool MUL => File.Exists(Core.FindDataFile("gumpart.mul"));

		static GumpData()
		{
			if (UOP)
			{
				m_FileIndex = new FileIndex("gumpartLegacyMUL.uop", 0xFFFF, ".tga", -1, true);
			}
			else if (MUL)
			{
				m_FileIndex = new FileIndex("gumpidx.mul", "gumpart.mul", 0xFFFF);
			}
		}

		public static Bitmap GetGump(int index, int hue, bool onlyHueGrayPixels)
		{
			return GetGump(index, hue, onlyHueGrayPixels, out _);
		}

		public static Bitmap GetGump(int index, int hue, bool onlyHueGrayPixels, out bool patched)
		{
			try
			{
				var orig = GetGump(index, out patched);

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
					Console.WriteLine($"[Ultima]: GumpData.GetGump({nameof(index)}:{index}, {nameof(hue)}:{hue}, {nameof(onlyHueGrayPixels)}:{onlyHueGrayPixels}, ...)\n{e}");
				}

				return null;
			}
		}

		public static Bitmap GetGump(int index)
		{
			return GetGump(index, out _);
		}

		public static unsafe Bitmap GetGump(int index, out bool patched)
		{
			try
			{
				if (m_Patched.Contains(index))
				{
					patched = (bool)m_Patched[index];
				}
				else
				{
					patched = false;
				}

				if (index > m_Cache.Length - 1)
				{
					return null;
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

				if (extra == -1)
				{
					stream.Close();

					return null;
				}

				if (patched)
				{
					m_Patched[index] = true;
				}

				var width = (extra >> 16) & 0xFFFF;

				var height = extra & 0xFFFF;

				if (width <= 0 || height <= 0)
				{
					return null;
				}

				var bmp = new Bitmap(width, height, PixelFormat);

				var bd = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat);

				try
				{
					if (m_StreamBuffer == null || m_StreamBuffer.Length < length)
					{
						m_StreamBuffer = new byte[length];
					}

					stream.Read(m_StreamBuffer, 0, length);

					fixed (byte* data = m_StreamBuffer)
					{
						var lookup = (int*)data;

						var dat = (ushort*)data;

						var line = (ushort*)bd.Scan0;

						var delta = bd.Stride >> 1;

						var count = 0;

						for (var y = 0; y < height; ++y, line += delta)
						{
							count = (*lookup++ * 2);

							var cur = line;

							var end = line + bd.Width;

							while (cur < end)
							{
								var color = dat[count++];

								var next = cur + dat[count++];

								if (color == 0)
								{
									cur = next;
								}

								else
								{
									color ^= 0x8000;

									while (cur < next)
									{
										*cur++ = color;
									}
								}
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
			catch (Exception e)
			{
				patched = false;

				if (Core.Debug)
				{
					Console.WriteLine($"[Ultima]: GumpData.GetGump({nameof(index)}:{index}, ...)\n{e}");
				}

				return null;
			}
		}
	}
}
