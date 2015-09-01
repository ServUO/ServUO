#region References
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
#endregion

namespace Ultima
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

			if ((e.length & (1 << 31)) != 0)
			{
				patched = true;
				Verdata.Seek(e.lookup);
				return Verdata.Stream;
			}

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

		public bool Valid(int index, out int length, out int extra, out bool patched)
		{
			if (index < 0 || index >= Index.Length)
			{
				length = extra = 0;
				patched = false;
				return false;
			}

			Entry3D e = Index[index];

			if (e.lookup < 0)
			{
				length = extra = 0;
				patched = false;
				return false;
			}

			length = e.length & 0x7FFFFFFF;
			extra = e.extra;

			if ((e.length & (1 << 31)) != 0)
			{
				patched = true;
				return true;
			}

			if (e.length < 0)
			{
				length = extra = 0;
				patched = false;
				return false;
			}

			if ((MulPath == null) || !File.Exists(MulPath))
			{
				length = extra = 0;
				patched = false;
				return false;
			}

			if ((Stream == null) || (!Stream.CanRead) || (!Stream.CanSeek))
			{
				Stream = new FileStream(MulPath, FileMode.Open, FileAccess.Read, FileShare.Read);
			}

			if (Stream.Length < e.lookup)
			{
				length = extra = 0;
				patched = false;
				return false;
			}

			patched = false;

			return true;
		}

		public FileIndex(string idxFile, string mulFile, int length, int file)
			: this(idxFile, mulFile, null, length, file, ".dat", -1, false)
		{ }

		public FileIndex(
			string idxFile,
			string mulFile,
			string uopFile,
			int length,
			int file,
			string uopEntryExtension,
			int idxLength,
			bool hasExtra)
		{
			Index = new Entry3D[length];

			string idxPath = null;
			MulPath = null;
			string uopPath = null;

			if (Files.MulPath == null)
			{
				Files.LoadMulPath();
			}

			if (Files.MulPath.Count > 0)
			{
				idxPath = Files.MulPath[idxFile.ToLower()];
				MulPath = Files.MulPath[mulFile.ToLower()];

				if (!String.IsNullOrEmpty(uopFile) && Files.MulPath.ContainsKey(uopFile.ToLower()))
				{
					uopPath = Files.MulPath[uopFile.ToLower()];
				}

				if (String.IsNullOrEmpty(idxPath))
				{
					idxPath = null;
				}
				else
				{
					if (String.IsNullOrEmpty(Path.GetDirectoryName(idxPath)))
					{
						idxPath = Path.Combine(Files.RootDir, idxPath);
					}

					if (!File.Exists(idxPath))
					{
						idxPath = null;
					}
				}

				if (String.IsNullOrEmpty(MulPath))
				{
					MulPath = null;
				}
				else
				{
					if (String.IsNullOrEmpty(Path.GetDirectoryName(MulPath)))
					{
						MulPath = Path.Combine(Files.RootDir, MulPath);
					}

					if (!File.Exists(MulPath))
					{
						MulPath = null;
					}
				}

				if (String.IsNullOrEmpty(uopPath))
				{
					uopPath = null;
				}
				else
				{
					if (String.IsNullOrEmpty(Path.GetDirectoryName(uopPath)))
					{
						uopPath = Path.Combine(Files.RootDir, uopPath);
					}

					if (!File.Exists(uopPath))
					{
						uopPath = null;
					}
					else
					{
						MulPath = uopPath;
					}
				}
			}

			/* UOP files support code, written by Wyatt (c) www.ruosi.org
			 * idxLength variable was added for compatibility with legacy code for art (see art.cs)
			 * At the moment the only UOP file having entries with extra field is gumpartlegacy.uop,
			 * and it's two dwords in the beginning of the entry.
			 * It's possible that UOP can include some entries with unknown hash: not really unknown for me, but
			 * not useful for reading legacy entries. That's why i removed unknown hash exception throwing from this code
			 */
			if (MulPath != null && MulPath.EndsWith(".uop"))
			{
				using (var index = new FileStream(MulPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
				{
					Stream = new FileStream(MulPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

					var fi = new FileInfo(MulPath);
					string uopPattern = fi.Name.Replace(fi.Extension, "").ToLowerInvariant();

					using (var br = new BinaryReader(Stream))
					{
						br.BaseStream.Seek(0, SeekOrigin.Begin);

						if (br.ReadInt32() != 0x50594D)
						{
							throw new ArgumentException("Bad UOP file.");
						}

						br.ReadInt64(); // version + signature
						long nextBlock = br.ReadInt64();
						br.ReadInt32(); // block capacity
						int count = br.ReadInt32();

						if (idxLength > 0)
						{
							IdxLength = idxLength * 12;
						}

						var hashes = new Dictionary<ulong, int>();

						for (int i = 0; i < length; i++)
						{
							string entryName = string.Format("build/{0}/{1:D8}{2}", uopPattern, i, uopEntryExtension);
							ulong hash = HashFileName(entryName);

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

								int idx;
								if (hashes.TryGetValue(hash, out idx))
								{
									if (idx < 0 || idx > Index.Length)
									{
										throw new IndexOutOfRangeException("hashes dictionary and files collection have different count of entries!");
									}

									Index[idx].lookup = (int)(offset + headerLength);
									Index[idx].length = entryLength;

									if (hasExtra)
									{
										long curPos = br.BaseStream.Position;

										br.BaseStream.Seek(offset + headerLength, SeekOrigin.Begin);

										byte[] extra = br.ReadBytes(8);

										var extra1 = (ushort)((extra[3] << 24) | (extra[2] << 16) | (extra[1] << 8) | extra[0]);
										var extra2 = (ushort)((extra[7] << 24) | (extra[6] << 16) | (extra[5] << 8) | extra[4]);

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
			else if ((idxPath != null) && (MulPath != null))
			{
				using (var index = new FileStream(idxPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
				{
					Stream = new FileStream(MulPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
					var count = (int)(index.Length / 12);
					IdxLength = index.Length;
					GCHandle gc = GCHandle.Alloc(Index, GCHandleType.Pinned);
					var buffer = new byte[index.Length];
					index.Read(buffer, 0, (int)index.Length);
					Marshal.Copy(buffer, 0, gc.AddrOfPinnedObject(), (int)Math.Min(IdxLength, length * 12));
					gc.Free();
					for (int i = count; i < length; ++i)
					{
						Index[i].lookup = -1;
						Index[i].length = -1;
						Index[i].extra = -1;
					}
				}
			}
			else
			{
				Stream = null;
				return;
			}

			Entry5D[] patches = Verdata.Patches;

			if (file > -1)
			{
				for (int i = 0; i < patches.Length; ++i)
				{
					Entry5D patch = patches[i];

					if (patch.file == file && patch.index >= 0 && patch.index < length)
					{
						Index[patch.index].lookup = patch.lookup;
						Index[patch.index].length = patch.length | (1 << 31);
						Index[patch.index].extra = patch.extra;
					}
				}
			}
		}

		public FileIndex(string idxFile, string mulFile, int file)
		{
			string idxPath = null;
			MulPath = null;
			if (Files.MulPath == null)
			{
				Files.LoadMulPath();
			}
			if (Files.MulPath.Count > 0)
			{
				idxPath = Files.MulPath[idxFile.ToLower()];
				MulPath = Files.MulPath[mulFile.ToLower()];
				if (String.IsNullOrEmpty(idxPath))
				{
					idxPath = null;
				}
				else
				{
					if (String.IsNullOrEmpty(Path.GetDirectoryName(idxPath)))
					{
						idxPath = Path.Combine(Files.RootDir, idxPath);
					}
					if (!File.Exists(idxPath))
					{
						idxPath = null;
					}
				}
				if (String.IsNullOrEmpty(MulPath))
				{
					MulPath = null;
				}
				else
				{
					if (String.IsNullOrEmpty(Path.GetDirectoryName(MulPath)))
					{
						MulPath = Path.Combine(Files.RootDir, MulPath);
					}
					if (!File.Exists(MulPath))
					{
						MulPath = null;
					}
				}
			}

			if ((idxPath != null) && (MulPath != null))
			{
				using (var index = new FileStream(idxPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
				{
					Stream = new FileStream(MulPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
					var count = (int)(index.Length / 12);
					IdxLength = index.Length;
					Index = new Entry3D[count];
					GCHandle gc = GCHandle.Alloc(Index, GCHandleType.Pinned);
					var buffer = new byte[index.Length];
					index.Read(buffer, 0, (int)index.Length);
					Marshal.Copy(buffer, 0, gc.AddrOfPinnedObject(), (int)index.Length);
					gc.Free();
				}
			}
			else
			{
				Stream = null;
				Index = new Entry3D[1];
				return;
			}
			Entry5D[] patches = Verdata.Patches;

			if (file > -1)
			{
				for (int i = 0; i < patches.Length; ++i)
				{
					Entry5D patch = patches[i];

					if (patch.file == file && patch.index >= 0 && patch.index < Index.Length)
					{
						Index[patch.index].lookup = patch.lookup;
						Index[patch.index].length = patch.length | (1 << 31);
						Index[patch.index].extra = patch.extra;
					}
				}
			}
		}

		/// <summary>
		///     Method for calculating entry hash by it's name.
		///     Taken from Mythic.Package.dll
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static ulong HashFileName(string s)
		{
			uint eax, ecx, edx, ebx, esi, edi;

			eax = ecx = edx = ebx = esi = edi = 0;
			ebx = edi = esi = (uint)s.Length + 0xDEADBEEF;

			int i = 0;

			for (i = 0; i + 12 < s.Length; i += 12)
			{
				edi = (uint)((s[i + 7] << 24) | (s[i + 6] << 16) | (s[i + 5] << 8) | s[i + 4]) + edi;
				esi = (uint)((s[i + 11] << 24) | (s[i + 10] << 16) | (s[i + 9] << 8) | s[i + 8]) + esi;
				edx = (uint)((s[i + 3] << 24) | (s[i + 2] << 16) | (s[i + 1] << 8) | s[i]) - esi;

				edx = (edx + ebx) ^ (esi >> 28) ^ (esi << 4);
				esi += edi;
				edi = (edi - edx) ^ (edx >> 26) ^ (edx << 6);
				edx += esi;
				esi = (esi - edi) ^ (edi >> 24) ^ (edi << 8);
				edi += edx;
				ebx = (edx - esi) ^ (esi >> 16) ^ (esi << 16);
				esi += edi;
				edi = (edi - ebx) ^ (ebx >> 13) ^ (ebx << 19);
				ebx += esi;
				esi = (esi - edi) ^ (edi >> 28) ^ (edi << 4);
				edi += ebx;
			}

			if (s.Length - i > 0)
			{
				switch (s.Length - i)
				{
					case 12:
						esi += (uint)s[i + 11] << 24;
						goto case 11;
					case 11:
						esi += (uint)s[i + 10] << 16;
						goto case 10;
					case 10:
						esi += (uint)s[i + 9] << 8;
						goto case 9;
					case 9:
						esi += s[i + 8];
						goto case 8;
					case 8:
						edi += (uint)s[i + 7] << 24;
						goto case 7;
					case 7:
						edi += (uint)s[i + 6] << 16;
						goto case 6;
					case 6:
						edi += (uint)s[i + 5] << 8;
						goto case 5;
					case 5:
						edi += s[i + 4];
						goto case 4;
					case 4:
						ebx += (uint)s[i + 3] << 24;
						goto case 3;
					case 3:
						ebx += (uint)s[i + 2] << 16;
						goto case 2;
					case 2:
						ebx += (uint)s[i + 1] << 8;
						goto case 1;
					case 1:
						ebx += s[i];
						break;
				}

				esi = (esi ^ edi) - ((edi >> 18) ^ (edi << 14));
				ecx = (esi ^ ebx) - ((esi >> 21) ^ (esi << 11));
				edi = (edi ^ ecx) - ((ecx >> 7) ^ (ecx << 25));
				esi = (esi ^ edi) - ((edi >> 16) ^ (edi << 16));
				edx = (esi ^ ecx) - ((esi >> 28) ^ (esi << 4));
				edi = (edi ^ edx) - ((edx >> 18) ^ (edx << 14));
				eax = (esi ^ edi) - ((edi >> 8) ^ (edi << 24));

				return ((ulong)edi << 32) | eax;
			}

			return ((ulong)esi << 32) | eax;
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