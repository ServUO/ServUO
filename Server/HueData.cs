#region References
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
#endregion

namespace Server
{
    public static class HueData
    {
#if MONO
		public const PixelFormat PixelFormat = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
#else
        public const PixelFormat PixelFormat = System.Drawing.Imaging.PixelFormat.Format16bppArgb1555;
#endif
        private static readonly int[] m_Header = new int[0];

        public static Hue[] List { get; } = new Hue[3000];

        public static bool CheckFile => File.Exists(Core.FindDataFile("hues.mul"));

        static HueData()
        {
            int index = 0;

            if (CheckFile)
            {
                string path = Core.FindDataFile("hues.mul");

                if (path != null)
                {
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        int blockCount = (int)fs.Length / 708;

                        if (blockCount > 375)
                        {
                            blockCount = 375;
                        }

                        m_Header = new int[blockCount];

                        int structsize = Marshal.SizeOf(typeof(HueEntry));
                        byte[] buffer = new byte[blockCount * (4 + 8 * structsize)];

                        GCHandle gc = GCHandle.Alloc(buffer, GCHandleType.Pinned);

                        try
                        {
                            fs.Read(buffer, 0, buffer.Length);

                            long currpos = 0;

                            for (int i = 0; i < blockCount; ++i)
                            {
                                IntPtr ptrheader = new IntPtr((long)gc.AddrOfPinnedObject() + currpos);

                                currpos += 4;

                                m_Header[i] = (int)Marshal.PtrToStructure(ptrheader, typeof(int));

                                for (int j = 0; j < 8; ++j, ++index)
                                {
                                    IntPtr ptr = new IntPtr((long)gc.AddrOfPinnedObject() + currpos);

                                    currpos += structsize;

                                    HueEntry cur = (HueEntry)Marshal.PtrToStructure(ptr, typeof(HueEntry));

                                    List[index] = new Hue(index, cur);
                                }
                            }
                        }
                        finally
                        {
                            gc.Free();
                        }
                    }
                }
            }

            while (index < List.Length)
            {
                List[index] = new Hue(index);

                ++index;
            }
        }

        public static Hue GetHue(int index)
        {
            index &= 0x3FFF;

            if (index >= 0 && index < List.Length)
            {
                return List[index];
            }

            return List[0];
        }

        private const float ScaleF = 31f / 255f;

        public static short ColorToHue(Color c)
        {
            ushort origred = c.R;
            ushort origgreen = c.G;
            ushort origblue = c.B;

            ushort newred = (ushort)(origred * ScaleF);

            if (newred == 0 && origred != 0)
            {
                newred = 1;
            }

            ushort newgreen = (ushort)(origgreen * ScaleF);

            if (newgreen == 0 && origgreen != 0)
            {
                newgreen = 1;
            }

            ushort newblue = (ushort)(origblue * ScaleF);

            if (newblue == 0 && origblue != 0)
            {
                newblue = 1;
            }

            return (short)((newred << 10) | (newgreen << 5) | (newblue));
        }

        private const int ScaleI = 255 / 31;

        public static Color HueToColor(short hue)
        {
            return Color.FromArgb(((hue & 0x7c00) >> 10) * ScaleI, ((hue & 0x3e0) >> 5) * ScaleI, (hue & 0x1f) * ScaleI);
        }

        public static int HueToColorR(short hue)
        {
            return ((hue & 0x7c00) >> 10) * ScaleI;
        }

        public static int HueToColorG(short hue)
        {
            return ((hue & 0x3e0) >> 5) * ScaleI;
        }

        public static int HueToColorB(short hue)
        {
            return (hue & 0x1f) * ScaleI;
        }

        public static unsafe void ApplyTo(Bitmap bmp, int hue, bool onlyHueGrayPixels)
        {
            ApplyTo(bmp, GetHue(hue), onlyHueGrayPixels);
        }

        public static unsafe void ApplyTo(Bitmap bmp, Hue hue, bool onlyHueGrayPixels)
        {
            ApplyTo(bmp, hue?.Colors, onlyHueGrayPixels);
        }

        public static unsafe void ApplyTo(Bitmap bmp, short[] colors, bool onlyHueGrayPixels)
        {
            if (bmp == null || colors == null || colors.Length < 32)
            {
                return;
            }

            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat);

            int stride = bd.Stride >> 1;
            int width = bd.Width;
            int height = bd.Height;
            int delta = stride - width;

            ushort* pBuffer = (ushort*)bd.Scan0;
            ushort* pLineEnd = pBuffer + width;
            ushort* pImageEnd = pBuffer + (stride * height);

            if (onlyHueGrayPixels)
            {
                int c, r, g, b;

                while (pBuffer < pImageEnd)
                {
                    while (pBuffer < pLineEnd)
                    {
                        c = *pBuffer;

                        if (c != 0)
                        {
                            r = (c >> 10) & 0x1F;
                            g = (c >> 5) & 0x1F;
                            b = c & 0x1F;

                            if (r == g && r == b)
                            {
                                *pBuffer = (ushort)colors[(c >> 10) & 0x1F];
                            }
                        }

                        ++pBuffer;
                    }

                    pBuffer += delta;
                    pLineEnd += stride;
                }
            }
            else
            {
                while (pBuffer < pImageEnd)
                {
                    while (pBuffer < pLineEnd)
                    {
                        if (*pBuffer != 0)
                        {
                            *pBuffer = (ushort)colors[(*pBuffer >> 10) & 0x1F];
                        }

                        ++pBuffer;
                    }

                    pBuffer += delta;
                    pLineEnd += stride;
                }
            }

            bmp.UnlockBits(bd);
        }

        public sealed class Hue
        {
            public int Index { get; }

            public short[] Colors { get; } = new short[32];

            public string Name { get; }

            public short TableStart { get; }
            public short TableEnd { get; }

            internal Hue(int index)
            {
                Name = "Null";
                Index = index;
            }

            internal Hue(int index, HueEntry entry)
            {
                Index = index;

                for (int i = 0; i < 32; ++i)
                {
                    Colors[i] = (short)(entry.Colors[i] | 0x8000);
                }

                TableStart = (short)(entry.TableStart | 0x8000);
                TableEnd = (short)(entry.TableEnd | 0x8000);

                int count = 0;

                while (count < 20 && count < entry.Name.Length && entry.Name[count] != 0)
                {
                    ++count;
                }

                Name = Encoding.Default.GetString(entry.Name, 0, count);
                Name = Name.Replace("\n", " ");
            }

            public Color GetColor(int index)
            {
                return HueToColor(Colors[index]);
            }

            public unsafe void ApplyTo(Bitmap bmp, bool onlyHueGrayPixels)
            {
                BitmapData bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat);

                int stride = bd.Stride >> 1;
                int width = bd.Width;
                int height = bd.Height;
                int delta = stride - width;

                ushort* pBuffer = (ushort*)bd.Scan0;
                ushort* pLineEnd = pBuffer + width;
                ushort* pImageEnd = pBuffer + (stride * height);

                if (onlyHueGrayPixels)
                {
                    int c;
                    int r;
                    int g;
                    int b;

                    while (pBuffer < pImageEnd)
                    {
                        while (pBuffer < pLineEnd)
                        {
                            c = *pBuffer;

                            if (c != 0)
                            {
                                r = (c >> 10) & 0x1F;
                                g = (c >> 5) & 0x1F;
                                b = c & 0x1F;

                                if (r == g && r == b)
                                {
                                    *pBuffer = (ushort)Colors[(c >> 10) & 0x1F];
                                }
                            }

                            ++pBuffer;
                        }

                        pBuffer += delta;
                        pLineEnd += stride;
                    }
                }
                else
                {
                    while (pBuffer < pImageEnd)
                    {
                        while (pBuffer < pLineEnd)
                        {
                            if (*pBuffer != 0)
                            {
                                *pBuffer = (ushort)Colors[(*pBuffer >> 10) & 0x1F];
                            }

                            ++pBuffer;
                        }

                        pBuffer += delta;
                        pLineEnd += stride;
                    }
                }

                bmp.UnlockBits(bd);
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct HueEntry
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public ushort[] Colors;

            public ushort TableStart;
            public ushort TableEnd;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] Name;
        }
    }
}
