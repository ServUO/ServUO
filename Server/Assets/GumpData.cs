#region References

using System;

using System.Drawing;

using System.Drawing.Imaging;
using System.Threading;

#endregion

namespace Server
{
	public static class GumpData
	{
		private static readonly AutoResetEvent m_RenderSync = new AutoResetEvent(true);

		private static readonly FileIndex m_FileIndex;

		private static byte[] m_StreamBuffer;

		private static readonly Bitmap[] m_Cache = new Bitmap[65535];
		private static readonly bool[] m_Invalid = new bool[65535];

		public static int Length => m_Cache.Length;

		public static bool UOP => Core.FindDataFile("gumpartLegacyMUL.uop") != null;
		public static bool MUL => Core.FindDataFile("gumpart.mul") != null;

		public static int MaxGumpID { get; }

		static GumpData()
		{
			if (UOP)
				m_FileIndex = new FileIndex("gumpartLegacyMUL.uop", 65535, ".tga", true);
			else if (MUL)
				m_FileIndex = new FileIndex("gumpidx.mul", "gumpart.mul", 65535);

			if (m_FileIndex != null)
				MaxGumpID = m_FileIndex.IdxCount - 1;
		}

		public static Bitmap GetGump(int index, int hue, bool onlyHueGrayPixels)
		{
			try
			{
				var orig = GetGump(index);

				if (orig == null)
					return null;

				var image = new Bitmap(orig);

				HueData.ApplyTo(image, hue, onlyHueGrayPixels);

				return image;
			}
			catch (Exception e)
			{
				if (Core.Debug)
				{
					Console.WriteLine($"[Ultima]: GumpData.GetGump({nameof(index)}:{index}, {nameof(hue)}:{hue}, {nameof(onlyHueGrayPixels)}:{onlyHueGrayPixels})\n{e}");
				}

				return null;
			}
		}

		public static unsafe Bitmap GetGump(int index)
		{
			m_RenderSync.WaitOne();

			try
			{
				if (index < 0 || index > MaxGumpID)
					return null;

				if (m_Invalid[index])
					return null;

				if (m_Cache[index] != null)
					return m_Cache[index];

				if (!m_FileIndex.Seek(index, ref m_StreamBuffer, out var length, out var extra) || extra == -1)
				{
					m_Invalid[index] = true;
					return null;
				}

				var width = (extra >> 16) & 0xFFFF;
				var height = extra & 0xFFFF;

				if (width <= 0 || height <= 0)
					return null;

				fixed (byte* data = m_StreamBuffer)
				{
					var bin = (int*)data;
					var dat = (ushort*)data;

					var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
					var bd = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);

					try
					{
						var line = (int*)bd.Scan0;
						var delta = bd.Stride >> 2;

						int count = 0, argb;

						for (var y = 0; y < height; ++y, line += delta)
						{
							count = (*bin++ * 2);

							var cur = line;
							var end = line + delta;

							while (cur < end)
							{
								var color = dat[count++];
								var next = cur + dat[count++];

								if (color != 0)
								{
									color ^= 0x8000;

									argb = ((color & 0x7C00) << 9) | ((color & 0x03E0) << 6) | ((color & 0x1F) << 3);
									argb = ((color & 0x8000) * 0x1FE00) | argb | ((argb >> 5) & 0x070707);

									while (cur < next)
										*cur++ = argb;
								}
								else
								{
									cur = next;
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
					Console.WriteLine($"[Ultima]: GumpData.GetGump({nameof(index)}:{index})\n{e}");

				return null;
			}
			finally
			{
				m_RenderSync.Set();
			}
		}
	}
}