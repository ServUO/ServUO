#region References
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
#endregion

namespace Server
{
    public sealed class FileIndex
    {
        public Entry3D[] Index { get; private set; }
        public Stream Stream { get; private set; }
        public long IdxLength { get; private set; }
        private readonly string MulPath;

        public Stream Seek(int index, out int length, out int extra, out bool patched)
        {
            if (index < 0 || index >= Index.Length)
            {
                length = extra = 0;
                patched = false;
                return null;
            }

            Entry3D e = Index[index];

            if (e.lookup < 0)
            {
                length = extra = 0;
                patched = false;
                return null;
            }

            length = e.length & 0x7FFFFFFF;
            extra = e.extra;

            if (e.length < 0)
            {
                length = extra = 0;
                patched = false;
                return null;
            }

            if ((Stream == null) || (!Stream.CanRead) || (!Stream.CanSeek))
            {
                if (MulPath == null)
                {
                    Stream = null;
                }
                else
                {
                    Stream = new FileStream(MulPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                }
            }

            if (Stream == null)
            {
                length = extra = 0;
                patched = false;
                return null;
            }
            else if (Stream.Length < e.lookup)
            {
                length = extra = 0;
                patched = false;
                return null;
            }

            patched = false;

            Stream.Seek(e.lookup, SeekOrigin.Begin);
            return Stream;
        }

        public FileIndex(
            string uopFile,
            int length,
            string uopEntryExtension,
            int idxLength,
            bool hasExtra)
        {
            Index = new Entry3D[length];

            MulPath = Core.FindDataFile(uopFile);

            /* UOP files support code, written by Wyatt (c) www.ruosi.org
			 * idxLength variable was added for compatibility with legacy code for art (see art.cs)
			 * At the moment the only UOP file having entries with extra field is gumpartlegacy.uop,
			 * and it's two dwords in the beginning of the entry.
			 * It's possible that UOP can include some entries with unknown hash: not really unknown for me, but
			 * not useful for reading legacy entries. That's why i removed unknown hash exception throwing from this code
			 */
            if (MulPath != null && MulPath.EndsWith(".uop"))
            {
                using (FileStream index = new FileStream(MulPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    Stream = new FileStream(MulPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

                    FileInfo fi = new FileInfo(MulPath);
                    string uopPattern = fi.Name.Replace(fi.Extension, "").ToLowerInvariant();

                    using (BinaryReader br = new BinaryReader(Stream))
                    {
                        br.BaseStream.Seek(0, SeekOrigin.Begin);

                        if (br.ReadInt32() != 0x50594D)
                            return;

                        br.ReadInt64(); // version + signature
                        long nextBlock = br.ReadInt64();
                        br.ReadInt32(); // block capacity
                        int count = br.ReadInt32();

                        if (idxLength > 0)
                        {
                            IdxLength = idxLength * 12;
                        }

                        Dictionary<ulong, int> hashes = new Dictionary<ulong, int>();

                        for (int i = 0; i < length; i++)
                        {
                            string entryName = string.Format("build/{0}/{1:D8}{2}", uopPattern, i, uopEntryExtension);
                            ulong hash = UOPHash.HashLittle2(entryName);

                            if (!hashes.ContainsKey(hash))
                            {
                                hashes.Add(hash, i);
                            }
                        }

                        br.BaseStream.Seek(nextBlock, SeekOrigin.Begin);

                        do
                        {
                            int filesCount = br.ReadInt32();
                            nextBlock = br.ReadInt64();

                            for (int i = 0; i < filesCount; i++)
                            {
                                long offset = br.ReadInt64();
                                int headerLength = br.ReadInt32();
                                int compressedLength = br.ReadInt32();
                                int decompressedLength = br.ReadInt32();
                                ulong hash = br.ReadUInt64();
                                br.ReadUInt32(); // Adler32
                                short flag = br.ReadInt16();

                                int entryLength = flag == 1 ? compressedLength : decompressedLength;

                                if (offset == 0)
                                {
                                    continue;
                                }

                                if (hashes.TryGetValue(hash, out int idx))
                                {
                                    if (idx < 0 || idx > Index.Length)
                                        return;

                                    Index[idx].lookup = (int)(offset + headerLength);
                                    Index[idx].length = entryLength;

                                    if (hasExtra)
                                    {
                                        long curPos = br.BaseStream.Position;

                                        br.BaseStream.Seek(offset + headerLength, SeekOrigin.Begin);

                                        byte[] extra = br.ReadBytes(8);

                                        ushort extra1 = (ushort)((extra[3] << 24) | (extra[2] << 16) | (extra[1] << 8) | extra[0]);
                                        ushort extra2 = (ushort)((extra[7] << 24) | (extra[6] << 16) | (extra[5] << 8) | extra[4]);

                                        Index[idx].lookup += 8;
                                        Index[idx].extra = extra1 << 16 | extra2;

                                        br.BaseStream.Seek(curPos, SeekOrigin.Begin);
                                    }
                                }
                            }
                        }
                        while (br.BaseStream.Seek(nextBlock, SeekOrigin.Begin) != 0);
                    }
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Entry3D
    {
        public int lookup;
        public int length;
        public int extra;
    }
}
