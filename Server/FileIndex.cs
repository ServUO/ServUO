#region References
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

		private readonly string _Path;

		public Stream Seek(int index, out int length, out int extra, out bool patched)
		{
			if (index < 0 || index >= Index.Length)
			{
				length = extra = 0;

				patched = false;

				return null;
			}

			var e = Index[index];

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

			if (Stream == null || !Stream.CanRead || !Stream.CanSeek)
			{
				if (_Path == null)
				{
					Stream = null;
				}
				else
				{
					Stream = new FileStream(_Path, FileMode.Open, FileAccess.Read, FileShare.Read);
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

		public FileIndex(string idxFile, string mulFile, int length)
		{
			Index = new Entry3D[length];

			var idxPath = Core.FindDataFile(idxFile);

			_Path = Core.FindDataFile(mulFile);

			if (idxPath != null && _Path != null)
			{
				using (var index = new FileStream(idxPath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					Stream = new FileStream(_Path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
					var count = (int)(index.Length / 12);
					IdxLength = index.Length;
					Index = new Entry3D[count];
					var gc = GCHandle.Alloc(Index, GCHandleType.Pinned);
					var buffer = new byte[index.Length];
					index.Read(buffer, 0, (int)index.Length);
					Marshal.Copy(buffer, 0, gc.AddrOfPinnedObject(), (int)index.Length);
					gc.Free();
				}
			}
		}

		public FileIndex(string uopFile, int length, string uopEntryExtension, int idxLength, bool hasExtra)
		{
			Index = new Entry3D[length];

			_Path = Core.FindDataFile(uopFile);

			if (_Path != null && _Path.EndsWith(".uop"))
			{
				using (var index = new FileStream(_Path, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					Stream = new FileStream(_Path, FileMode.Open, FileAccess.Read, FileShare.Read);

					var fi = new FileInfo(_Path);

					var uopPattern = fi.Name.Replace(fi.Extension, "").ToLowerInvariant();

					using (var br = new BinaryReader(Stream))
					{
						br.BaseStream.Seek(0, SeekOrigin.Begin);

						if (br.ReadInt32() != 0x50594D)
							return;

						br.ReadInt64(); // version + signature

						var nextBlock = br.ReadInt64();

						br.ReadInt32(); // block capacity

						var count = br.ReadInt32();

						if (idxLength > 0)
						{
							IdxLength = idxLength * 12;
						}

						var hashes = new Dictionary<ulong, int>();

						for (var i = 0; i < length; i++)
						{
							var entryName = System.String.Format("build/{0}/{1:D8}{2}", uopPattern, i, uopEntryExtension);

							var hash = UOPHash.HashLittle2(entryName);

							if (!hashes.ContainsKey(hash))
							{
								hashes.Add(hash, i);
							}
						}

						br.BaseStream.Seek(nextBlock, SeekOrigin.Begin);

						do
						{
							var filesCount = br.ReadInt32();

							nextBlock = br.ReadInt64();

							for (var i = 0; i < filesCount; i++)
							{
								var offset = br.ReadInt64();
								var headerLength = br.ReadInt32();
								var compressedLength = br.ReadInt32();
								var decompressedLength = br.ReadInt32();
								var hash = br.ReadUInt64();

								br.ReadUInt32(); // Adler32

								var flag = br.ReadInt16();

								var entryLength = flag == 1 ? compressedLength : decompressedLength;

								if (offset == 0)
								{
									continue;
								}

								if (hashes.TryGetValue(hash, out var idx))
								{
									if (idx < 0 || idx > Index.Length)
										return;

									Index[idx].lookup = (int)(offset + headerLength);

									Index[idx].length = entryLength;

									if (hasExtra)
									{
										var curPos = br.BaseStream.Position;

										br.BaseStream.Seek(offset + headerLength, SeekOrigin.Begin);

										var extra = br.ReadBytes(8);
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
