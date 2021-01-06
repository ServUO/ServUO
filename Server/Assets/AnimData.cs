#region References
using System.Collections;
using System.IO;
#endregion

namespace Server
{
    public sealed class AnimData
    {
        private static readonly int[] m_Header;
        private static readonly byte[] m_Unknown;

        public static Hashtable Table { get; } = new Hashtable();

        static unsafe AnimData()
        {
            string path = Core.FindDataFile("animdata.mul");

            if (path == null)
                return;

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader bin = new BinaryReader(fs))
            {
                int id = 0;
                int h = 0;

                byte unk, fcount, finter, fstart;

                sbyte[] fdata;

                m_Header = new int[bin.BaseStream.Length / (4 + 8 * (64 + 4))];

                while (h < m_Header.Length)
                {
                    m_Header[h++] = bin.ReadInt32(); // chunk header

                    byte[] buffer = bin.ReadBytes(544); // Read 8 tiles

                    fixed (byte* buf = buffer)
                    {
                        byte* data = buf;

                        for (int i = 0; i < 8; ++i, ++id)
                        {
                            fdata = new sbyte[64];

                            for (int j = 0; j < 64; ++j)
                                fdata[j] = (sbyte)*data++;

                            unk = *data++;
                            fcount = *data++;
                            finter = *data++;
                            fstart = *data++;

                            if (fcount > 0)
                                Table[id] = new AnimationData(fdata, unk, fcount, finter, fstart);
                        }
                    }
                }

                int remaining = (int)(bin.BaseStream.Length - bin.BaseStream.Position);

                if (remaining > 0)
                    m_Unknown = bin.ReadBytes(remaining);
            }
        }

        public static AnimationData GetData(int id)
        {
            return (AnimationData)Table[id];
        }
    }

    public class AnimationData
    {
        public sbyte[] FrameData { get; set; }
        public byte Unknown { get; private set; }
        public byte FrameCount { get; set; }
        public byte FrameInterval { get; set; }
        public byte FrameStart { get; set; }

        public AnimationData(sbyte[] frame, byte unk, byte fcount, byte finter, byte fstart)
        {
            FrameData = frame;
            Unknown = unk;
            FrameCount = fcount;
            FrameInterval = finter;
            FrameStart = fstart;
        }
    }
}
