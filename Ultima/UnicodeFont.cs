#region References
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
#endregion

namespace Ultima
{
	public sealed class UnicodeFont
	{
		public UnicodeChar[] Chars { get; set; }

		public UnicodeFont()
		{
			Chars = new UnicodeChar[0x10000];
		}

		/// <summary>
		///     Returns width of text
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public int GetWidth(string text)
		{
			if (text == null || text.Length == 0)
			{
				return 0;
			}
			int width = 0;
			for (int i = 0; i < text.Length; ++i)
			{
				int c = text[i] % 0x10000;
				width += Chars[c].Width;
				width += Chars[c].XOffset;
			}
			return width;
		}

		/// <summary>
		///     Returns max height of text
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public int GetHeight(string text)
		{
			if (text == null || text.Length == 0)
			{
				return 0;
			}
			int height = 0;
			for (int i = 0; i < text.Length; ++i)
			{
				int c = text[i] % 0x10000;
				height = Math.Max(height, Chars[c].Height + Chars[c].YOffset);
			}
			return height;
		}
	}

	public sealed class UnicodeChar
	{
		public byte[] Bytes { get; set; }
		public sbyte XOffset { get; set; }
		public sbyte YOffset { get; set; }
		public int Height { get; set; }
		public int Width { get; set; }

		/// <summary>
		///     Gets Bitmap of Char
		/// </summary>
		/// <returns></returns>
		public Bitmap GetImage()
		{
			return GetImage(false);
		}

		/// <summary>
		///     Gets Bitmap of Char with Background -1
		/// </summary>
		/// <param name="fill"></param>
		/// <returns></returns>
		public unsafe Bitmap GetImage(bool fill)
		{
			if ((Width == 0) || (Height == 0))
			{
				return null;
			}
			var bmp = new Bitmap(Width, Height, PixelFormat.Format16bppArgb1555);
			BitmapData bd = bmp.LockBits(
				new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format16bppArgb1555);
			var line = (ushort*)bd.Scan0;
			int delta = bd.Stride >> 1;
			for (int y = 0; y < Height; ++y, line += delta)
			{
				ushort* cur = line;
				for (int x = 0; x < Width; ++x)
				{
					if (IsPixelSet(Bytes, Width, x, y))
					{
						cur[x] = 0x8000;
					}
					else if (fill)
					{
						cur[x] = 0xffff;
					}
				}
			}
			bmp.UnlockBits(bd);
			return bmp;
		}

		private static bool IsPixelSet(byte[] data, int width, int x, int y)
		{
			int offset = x / 8 + y * ((width + 7) / 8);
			if (offset > data.Length)
			{
				return false;
			}
			return (data[offset] & (1 << (7 - (x % 8)))) != 0;
		}

		/// <summary>
		///     Resets Buffer with Bitmap
		/// </summary>
		/// <param name="bmp"></param>
		public unsafe void SetBuffer(Bitmap bmp)
		{
			Bytes = new byte[bmp.Height * (((bmp.Width - 1) / 8) + 1)];
			BitmapData bd = bmp.LockBits(
				new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format16bppArgb1555);
			var line = (ushort*)bd.Scan0;
			//int delta = bd.Stride >> 1;
			for (int y = 0; y < bmp.Height; ++y)
			{
				ushort* cur = line;
				for (int x = 0; x < bmp.Width; ++x)
				{
					if (cur[x] == 0x8000)
					{
						int offset = x / 8 + y * ((bmp.Width + 7) / 8);
						Bytes[offset] |= (byte)(1 << (7 - (x % 8)));
					}
				}
			}
			bmp.UnlockBits(bd);
		}
	}

	public static class UnicodeFonts
	{
		private static readonly string[] m_files = new[]
		{
			"unifont.mul", "unifont1.mul", "unifont2.mul", "unifont3.mul", "unifont4.mul", "unifont5.mul", "unifont6.mul",
			"unifont7.mul", "unifont8.mul", "unifont9.mul", "unifont10.mul", "unifont11.mul", "unifont12.mul"
		};

		public static UnicodeFont[] Fonts = new UnicodeFont[13];

		static UnicodeFonts()
		{
			Initialize();
		}

		/// <summary>
		///     Reads unifont*.mul
		/// </summary>
		public static void Initialize()
		{
			for (int i = 0; i < m_files.Length; i++)
			{
				string filePath = Files.GetFilePath(m_files[i]);
				if (filePath == null)
				{
					continue;
				}
				Fonts[i] = new UnicodeFont();
				using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					using (var bin = new BinaryReader(fs))
					{
						for (int c = 0; c < 0x10000; ++c)
						{
							Fonts[i].Chars[c] = new UnicodeChar();
							fs.Seek(((c) * 4), SeekOrigin.Begin);
							int num2 = bin.ReadInt32();
							if ((num2 >= fs.Length) || (num2 <= 0))
							{
								continue;
							}
							fs.Seek(num2, SeekOrigin.Begin);
							sbyte xOffset = bin.ReadSByte();
							sbyte yOffset = bin.ReadSByte();
							int Width = bin.ReadByte();
							int Height = bin.ReadByte();
							Fonts[i].Chars[c].XOffset = xOffset;
							Fonts[i].Chars[c].YOffset = yOffset;
							Fonts[i].Chars[c].Width = Width;
							Fonts[i].Chars[c].Height = Height;
							if (!((Width == 0) || (Height == 0)))
							{
								Fonts[i].Chars[c].Bytes = bin.ReadBytes(Height * (((Width - 1) / 8) + 1));
							}
						}
					}
				}
			}
		}

		/// <summary>
		///     Draws Text with font in Bitmap and returns
		/// </summary>
		/// <param name="fontId"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		public static Bitmap WriteText(int fontId, string text)
		{
			var result = new Bitmap(Fonts[fontId].GetWidth(text) + 2, Fonts[fontId].GetHeight(text) + 2);

			int dx = 2;
			int dy = 2;
			using (Graphics graph = Graphics.FromImage(result))
			{
				for (int i = 0; i < text.Length; ++i)
				{
					int c = text[i] % 0x10000;
					Bitmap bmp = Fonts[fontId].Chars[c].GetImage();
					dx += Fonts[fontId].Chars[c].XOffset;
					graph.DrawImage(bmp, dx, dy + Fonts[fontId].Chars[c].YOffset);
					dx += bmp.Width;
				}
			}
			return result;
		}

		/// <summary>
		///     Saves Font and returns string Filename
		/// </summary>
		/// <param name="path"></param>
		/// <param name="filetype"></param>
		/// <returns></returns>
		public static string Save(string path, int filetype)
		{
			string FileName = Path.Combine(path, m_files[filetype]);
			using (var fs = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.Write))
			{
				using (var bin = new BinaryWriter(fs))
				{
					fs.Seek(0x10000 * 4, SeekOrigin.Begin);
					bin.Write(0);
					// Set first data
					for (int c = 0; c < 0x10000; ++c)
					{
						if (Fonts[filetype].Chars[c].Bytes == null)
						{
							continue;
						}
						fs.Seek(((c) * 4), SeekOrigin.Begin);
						bin.Write((int)fs.Length);
						fs.Seek(fs.Length, SeekOrigin.Begin);
						bin.Write(Fonts[filetype].Chars[c].XOffset);
						bin.Write(Fonts[filetype].Chars[c].YOffset);
						bin.Write((byte)Fonts[filetype].Chars[c].Width);
						bin.Write((byte)Fonts[filetype].Chars[c].Height);
						bin.Write(Fonts[filetype].Chars[c].Bytes);
					}
				}
			}
			return FileName;
		}
	}
}