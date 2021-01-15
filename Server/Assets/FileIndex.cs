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
		public static readonly int EntryDataSize = Marshal.SizeOf<Entry3D>();

		public Entry3D[] Index { get; }

		public FileInfo File { get; }

		public int IdxLength { get; }
		public int IdxCount { get; }

		private readonly string _BinPath, _IdxPath;
		private readonly bool _BinLoaded, _IdxLoaded;

		public bool Loaded => _BinLoaded && _IdxLoaded;

		private FileIndex(int entryCount)
		{
			IdxLength = entryCount * EntryDataSize;
			IdxCount = entryCount;

			Index = new Entry3D[entryCount];
		}

		public FileIndex(string idxFile, string mulFile, int entryCount)
			: this(entryCount)
		{
			do
			{
				_IdxPath = Core.FindDataFile(idxFile);
				_BinPath = Core.FindDataFile(mulFile);

				try
				{
					if (_IdxPath != null && System.IO.File.Exists(_IdxPath))
					{
						FileStream index;

						try
						{ index = new FileStream(_IdxPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite); }
						catch { index = null; }

						if (index != null)
						{
							using (index)
							{
								IdxLength = (int)index.Length;
								IdxCount = IdxLength / EntryDataSize;

								if (Index.Length != IdxCount)
									Index = new Entry3D[IdxCount];

								unsafe
								{
									fixed (Entry3D* buffer = Index)
										NativeReader.Read(index, buffer, IdxLength);
								}
							}

							_IdxLoaded = true;
						}
					}
				}
				catch { }

				try
				{
					if (_BinPath != null)
					{
						try
						{ File = new FileInfo(_BinPath); }
						catch { File = null; }

						_BinLoaded = File?.Exists ?? false;
					}
				}
				catch { }
			}
			while (CheckRetry());
		}

		public FileIndex(string uopFile, int entryCount, string entryExt, bool extended)
			: this(entryCount)
		{
			do
			{
				_IdxPath = _BinPath = Core.FindDataFile(uopFile);

				try
				{
					if (_BinPath != null)
					{
						try
						{ File = new FileInfo(_BinPath); }
						catch { File = null; }

						_BinLoaded = File?.Exists ?? false;
					}
				}
				catch { }

				try
				{
					if (File != null)
					{
						using (var stream = new FileStream(File.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
						using (var reader = new BinaryReader(stream))
						{
							if (reader.ReadInt32() == 0x50594D)
							{
								reader.ReadInt64(); // version + signature

								var nextBlock = reader.ReadInt64();

								reader.ReadInt32(); // block capacity
								reader.ReadInt32(); // block count

								var hashes = new Dictionary<ulong, int>();

								var root = $"build/{Path.GetFileNameWithoutExtension(_IdxPath).ToLowerInvariant()}";

								for (var i = 0; i < entryCount; i++)
									hashes[UOPHash.HashLittle2($"{root}/{i:D8}{entryExt}")] = i;

								stream.Seek(nextBlock, SeekOrigin.Begin);

								do
								{
									var filesCount = reader.ReadInt32();

									nextBlock = reader.ReadInt64();

									for (var i = 0; i < filesCount; i++)
									{
										var offset = reader.ReadInt64();
										var headerLength = reader.ReadInt32();
										var compressedLength = reader.ReadInt32();
										var decompressedLength = reader.ReadInt32();
										var hash = reader.ReadUInt64();

										reader.ReadUInt32(); // Adler32

										var flag = reader.ReadInt16();

										var entryLength = flag == 1 ? compressedLength : decompressedLength;

										if (offset == 0 || !hashes.TryGetValue(hash, out var idx))
											continue;

										if (idx < 0 || idx > Index.Length)
											continue;

										Index[idx].Offset = (int)(offset + headerLength);
										Index[idx].Size = entryLength;

										if (!extended)
											continue;

										var curPos = stream.Position;

										stream.Seek(offset + headerLength, SeekOrigin.Begin);

										var extra = reader.ReadBytes(8);
										var extra1 = (ushort)((extra[3] << 24) | (extra[2] << 16) | (extra[1] << 8) | extra[0]);
										var extra2 = (ushort)((extra[7] << 24) | (extra[6] << 16) | (extra[5] << 8) | extra[4]);

										Index[idx].Offset += 8;
										Index[idx].Data = extra1 << 16 | extra2;

										stream.Seek(curPos, SeekOrigin.Begin);
									}
								}
								while (stream.Seek(nextBlock, SeekOrigin.Begin) != 0);

								_IdxLoaded = true;
							}
						}
					}
				}
				catch { }
			}
			while (CheckRetry());
		}

		private bool CheckRetry()
		{
			if (_IdxLoaded && _BinLoaded)
				return false;

			if (!_BinLoaded)
			{
				if (System.IO.File.Exists(_BinPath))
					Utility.WriteConsoleColor(ConsoleColor.Yellow, $"Warning: Could not load [bin] '{_BinPath}'\nThe file cannot be opened, close any applications that are using the file and try again.");
				else
					Utility.WriteConsoleColor(ConsoleColor.Yellow, $"Warning: Could not load [bin] '{_BinPath}'\nThe file cannot be opened, it does not exist.");
			}
			else if (!_IdxLoaded)
			{
				if (System.IO.File.Exists(_IdxPath))
					Utility.WriteConsoleColor(ConsoleColor.Yellow, $"Warning: Could not load [idx] '{_IdxPath}'\nThe file cannot be opened, close any applications that are using the file and try again.");
				else
					Utility.WriteConsoleColor(ConsoleColor.Yellow, $"Warning: Could not load [idx] '{_IdxPath}'\nThe file cannot be opened, it does not exist.");
			}

			if (Core.Service)
				return false;

			ConsoleKeyInfo key;
			bool retry;

			do
			{
				Utility.WriteConsoleColor(ConsoleColor.Yellow, "Retry? (Y/N)");

				key = Console.ReadKey();

				Console.WriteLine();

				retry = key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.Y;
			}
			while (!retry && key.Key != ConsoleKey.Escape && key.Key != ConsoleKey.N);

			return retry;
		}

		public bool Seek(int index, ref byte[] buffer, out int length, out int extra)
		{
			length = extra = 0;

			if (File == null)
				return false;

			if (index < 0 || index >= Index.Length)
				return false;

			var e = Index[index];

			if (e.Offset < 0 || e.Size < 0)
				return false;

			length = e.Size;
			extra = e.Data;

			try
			{
				using (var stream = new FileStream(File.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					unsafe
					{
						if (buffer == null || buffer.Length < length)
							buffer = new byte[length];

						fixed (byte* data = buffer)
							length = NativeReader.Read(stream, e.Offset, data, length);
					}
				}

				return true;
			}
			catch
			{
				length = extra = 0;
				return false;
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
	public struct Entry3D
	{
		public int Offset;
		public int Size;
		public int Data;
	}
}
