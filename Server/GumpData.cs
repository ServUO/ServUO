#region References
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

        public static bool CheckFile => File.Exists(Core.FindDataFile("artLegacyMUL.uop"));

        static GumpData()
		{
            if (CheckFile)
            {
                m_FileIndex = new FileIndex("gumpartLegacyMUL.uop", 0xFFFF, ".tga", -1, true);
            }
		}

        public static Bitmap GetGump(int index, int hue, bool onlyHueGrayPixels)
        {
            return GetGump(index, hue, onlyHueGrayPixels, out _);
        }

        public static Bitmap GetGump(int index, int hue, bool onlyHueGrayPixels, out bool patched)
        {
            Bitmap image = new Bitmap(GetGump(index, out patched));

            HueData.ApplyTo(image, hue, onlyHueGrayPixels);

            return image;
        }

        public static Bitmap GetGump(int index)
		{
			return GetGump(index, out _);
		}

		public static unsafe Bitmap GetGump(int index, out bool patched)
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

			Stream stream = m_FileIndex.Seek(index, out int length, out int extra, out patched);

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

			int width = (extra >> 16) & 0xFFFF;
			int height = extra & 0xFFFF;

			if (width <= 0 || height <= 0)
			{
				return null;
			}

			Bitmap bmp = new Bitmap(width, height, PixelFormat);

			BitmapData bd = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat);

			if (m_StreamBuffer == null || m_StreamBuffer.Length < length)
			{
				m_StreamBuffer = new byte[length];
			}

			stream.Read(m_StreamBuffer, 0, length);

			fixed (byte* data = m_StreamBuffer)
			{
				int* lookup = (int*)data;
				ushort* dat = (ushort*)data;
				ushort* line = (ushort*)bd.Scan0;

				int delta = bd.Stride >> 1;

                for (int y = 0; y < height; ++y, line += delta)
				{
					var count = (*lookup++ * 2);

					ushort* cur = line;
					ushort* end = line + bd.Width;

					while (cur < end)
					{
						ushort color = dat[count++];
						ushort* next = cur + dat[count++];

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

            return bmp;
        }
	}
}
