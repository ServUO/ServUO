#region References
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
#endregion

namespace Server
{
    public class ArtData
    {
#if MONO
		public const PixelFormat PixelFormat = System.Drawing.Imaging.PixelFormat.Format16bppRgb555;
#else
        public const PixelFormat PixelFormat = System.Drawing.Imaging.PixelFormat.Format16bppArgb1555;
#endif

        private static FileIndex m_FileIndex;

        private static readonly Bitmap[] m_Cache;
        private static readonly bool[] m_Removed;
        private static readonly Hashtable m_patched = new Hashtable();
        public static bool Modified = false;

        private static byte[] m_StreamBuffer;

        public static bool CheckFile = File.Exists(Core.FindDataFile("artLegacyMUL.uop"));

        static ArtData()
        {
            if (CheckFile)
            {
                m_Cache = new Bitmap[0x14000];
                m_Removed = new bool[0x14000];
                
                m_FileIndex = new FileIndex(
                "artLegacyMUL.uop", 0x10000, ".tga", 0x13FDC, false);
            }
        }

        public static int GetMaxItemID()
        {
            if (GetIdxLength() >= 0x13FDC)
            {
                return 0xFFFF;
            }

            if (GetIdxLength() == 0xC000)
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
                int max = GetMaxItemID();
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

        /// <summary>
        ///     Returns Bitmap of Static (with Cache)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Bitmap GetStatic(int index, bool checkmaxid = true)
        {
            return GetStatic(index, out bool patched, checkmaxid);
        }

        /// <summary>
        ///     Returns Bitmap of Static (with Cache) and verdata bool
        /// </summary>
        /// <param name="index"></param>
        /// <param name="patched"></param>
        /// <returns></returns>
        public static Bitmap GetStatic(int index, out bool patched, bool checkmaxid = true)
        {
            index = GetLegalItemID(index, checkmaxid);
            index += 0x4000;

            if (m_patched.Contains(index))
            {
                patched = (bool)m_patched[index];
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

            Stream stream = m_FileIndex.Seek(index, out int length, out int extra, out patched);

            if (stream == null)
            {
                return null;
            }
            if (patched)
            {
                m_patched[index] = true;
            }

            return LoadStatic(stream, length);
        }

        public static unsafe void Measure(Bitmap bmp, out int xMin, out int yMin, out int xMax, out int yMax)
        {
            xMin = yMin = 0;
            xMax = yMax = -1;

            if (bmp == null || bmp.Width <= 0 || bmp.Height <= 0)
            {
                return;
            }

            BitmapData bd = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat);

            int delta = (bd.Stride >> 1) - bd.Width;
            int lineDelta = bd.Stride >> 1;

            ushort* pBuffer = (ushort*)bd.Scan0;
            ushort* pLineEnd = pBuffer + bd.Width;
            ushort* pEnd = pBuffer + (bd.Height * lineDelta);

            bool foundPixel = false;

            int x = 0, y = 0;

            while (pBuffer < pEnd)
            {
                while (pBuffer < pLineEnd)
                {
                    ushort c = *pBuffer++;

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

            bmp.UnlockBits(bd);
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
                ushort* bindata = (ushort*)data;
                int count = 2;
                int width = bindata[count++];
                int height = bindata[count++];

                if (width <= 0 || height <= 0)
                {
                    return null;
                }

                int[] lookups = new int[height];

                int start = height + 4;

                for (int i = 0; i < height; ++i)
                {
                    lookups[i] = start + bindata[count++];
                }

                bmp = new Bitmap(width, height, PixelFormat);
                BitmapData bd = bmp.LockBits(
                    new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat);

                ushort* line = (ushort*)bd.Scan0;
                int delta = bd.Stride >> 1;

                for (int y = 0; y < height; ++y, line += delta)
                {
                    count = lookups[y];

                    ushort* cur = line;
                    ushort* end;
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
                bmp.UnlockBits(bd);
            }
            return bmp;
        }
    }
}
