#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

using Server;

using PF = System.Drawing.Imaging.PixelFormat;
#endregion

namespace VitaNex.Text
{
	public enum UOEncoding : byte
	{
		Ascii,
		Unicode
	}

	public enum UOFontStyle
	{
		Big = 0,
		Normal = 1,
		Small = 2
	}

	public sealed class UOFonts
	{
		private static readonly UOFont[] _Ascii;
		private static readonly UOFont[] _Unicode;

		private static readonly UOChar[][][] _Chars;

		private static readonly byte[] _EmptyBuffer = new byte[0];

		static UOFonts()
		{
			_Ascii = new UOFont[10];
			_Unicode = new UOFont[13];

			_Chars = new UOChar[2][][];
		}

		private static string FindFontFile(string file, params object[] args)
		{
			if (!args.IsNullOrEmpty())
			{
				file = String.Format(file, args);
			}

			var path = Path.Combine(Core.BaseDirectory, "Data", "Fonts", file);

			if (!File.Exists(path))
			{
				path = Core.FindDataFile(file);
			}

			return path;
		}

		private static Bitmap NewEmptyImage()
		{
			return new Bitmap(UOFont.DefaultCharSize.Width, UOFont.DefaultCharSize.Height, UOFont.PixelFormat);
		}

		private static UOChar NewEmptyChar(UOEncoding enc)
		{
			return new UOChar(enc, 0, 0, NewEmptyImage());
		}

		private static UOFont Instantiate(UOEncoding enc, byte id)
		{
			int charsWidth = 0, charsHeight = 0;

			var list = _Chars[(byte)enc][id];

			var i = list.Length;

			while (--i >= 0)
			{
				charsWidth = Math.Max(charsWidth, list[i].XOffset + list[i].Width);
				charsHeight = Math.Max(charsHeight, list[i].YOffset + list[i].Height);
			}

			return new UOFont(enc, id, 1, 4, (byte)charsWidth, (byte)charsHeight, list);
		}

		private static UOFont LoadAscii(byte id)
		{
			if (id >= _Ascii.Length)
			{
				return null;
			}

			const UOEncoding enc = UOEncoding.Ascii;

			var idx = (byte)enc;

			if (_Chars.InBounds(idx, id) && _Chars[idx][id] != null)
			{
				return _Ascii[id] ?? (_Ascii[id] = Instantiate(enc, id));
			}

			var fonts = _Chars[idx] ?? (_Chars[idx] = new UOChar[_Ascii.Length][]);
			var chars = fonts[id] ?? (fonts[id] = new UOChar[256]);

			var path = FindFontFile("fonts.mul");

			if (path == null || !File.Exists(path))
			{
				chars.SetAll(NewEmptyChar(enc));

				return _Ascii[id] ?? (_Ascii[id] = Instantiate(enc, id));
			}

			using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (var bin = new BinaryReader(fs))
				{
					for (var i = 0; i <= id; i++)
					{
						bin.ReadByte(); // header

						if (i == id)
						{
							for (var c = 0; c < 32; c++)
							{
								chars[c] = NewEmptyChar(enc);
							}
						}

						for (var c = 32; c < chars.Length; c++)
						{
							var width = bin.ReadByte();
							var height = bin.ReadByte();

							bin.ReadByte(); // unk

							if (i == id)
							{
								var buffer = _EmptyBuffer;

								if (width * height > 0)
								{
									buffer = bin.ReadBytes((width * height) * 2);
								}

								chars[c] = new UOChar(enc, 0, 0, GetImage(width, height, buffer, enc));
							}
							else
							{
								bin.BaseStream.Seek((width * height) * 2, SeekOrigin.Current);
							}
						}
					}
				}
			}

			return _Ascii[id] ?? (_Ascii[id] = Instantiate(enc, id));
		}

		private static UOFont LoadUnicode(byte id)
		{
			if (id >= _Unicode.Length)
			{
				return null;
			}

			const UOEncoding enc = UOEncoding.Unicode;

			var idx = (byte)enc;

			if (_Chars.InBounds(idx, id) && _Chars[idx][id] != null)
			{
				return _Unicode[id] ?? (_Unicode[id] = Instantiate(enc, id));
			}

			var fonts = _Chars[idx] ?? (_Chars[idx] = new UOChar[_Unicode.Length][]);
			var chars = fonts[id] ?? (fonts[id] = new UOChar[65536]);

			var filePath = FindFontFile("unifont{0:#}.mul", id);

			if (filePath == null)
			{
				chars.SetAll(NewEmptyChar(enc));

				return _Unicode[id] ?? (_Unicode[id] = Instantiate(enc, id));
			}

			using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (var bin = new BinaryReader(fs))
				{
					for (int c = 0, o; c < chars.Length; c++)
					{
						fs.Seek(c * 4, SeekOrigin.Begin);

						o = bin.ReadInt32();

						if (o <= 0 || o >= fs.Length)
						{
							chars[c] = NewEmptyChar(enc);
							continue;
						}

						fs.Seek(o, SeekOrigin.Begin);

						var x = bin.ReadSByte(); // x-offset
						var y = bin.ReadSByte(); // y-offset

						var width = bin.ReadByte();
						var height = bin.ReadByte();

						var buffer = _EmptyBuffer;

						if (width * height > 0)
						{
							buffer = bin.ReadBytes(height * (((width - 1) / 8) + 1));
						}

						chars[c] = new UOChar(enc, x, y, GetImage(width, height, buffer, enc));
					}
				}
			}

			return _Unicode[id] ?? (_Unicode[id] = Instantiate(enc, id));
		}

		private static unsafe Bitmap GetImage(int width, int height, byte[] buffer, UOEncoding enc)
		{
			if (width * height <= 0 || buffer.IsNullOrEmpty())
			{
				return NewEmptyImage();
			}

			var image = new Bitmap(width, height, UOFont.PixelFormat);
			var bound = new Rectangle(0, 0, width, height);
			var data = image.LockBits(bound, ImageLockMode.WriteOnly, UOFont.PixelFormat);

			var index = 0;

			var line = (ushort*)data.Scan0;
			var delta = data.Stride >> 1;

			int x, y;
			ushort pixel;

			for (y = 0; y < height; y++, line += delta)
			{
				var cur = line;

				if (cur == null)
				{
					continue;
				}

				for (x = 0; x < width; x++)
				{
					pixel = 0;

					if (enc > 0)
					{
						index = x / 8 + y * ((width + 7) / 8);
					}

					if (index < buffer.Length)
					{
						if (enc > 0)
						{
							pixel = buffer[index];
						}
						else
						{
							pixel = (ushort)(buffer[index++] | (buffer[index++] << 8));
						}
					}

					if (enc > 0)
					{
						pixel &= (ushort)(1 << (7 - (x % 8)));
					}

					if (pixel == 0)
					{
						cur[x] = 0;
					}
					else if (enc > 0)
					{
						cur[x] = 0x8000;
					}
					else
					{
						cur[x] = (ushort)(pixel ^ 0x8000);
					}
				}
			}

			image.UnlockBits(data);

			return image;
		}

		public static UOFont GetFont(UOEncoding enc, byte id)
		{
			switch (enc)
			{
				case UOEncoding.Ascii:
					return LoadAscii(id);
				case UOEncoding.Unicode:
					return LoadUnicode(id);
			}

			return null;
		}

		public static UOFont GetAscii(byte id)
		{
			return GetFont(UOEncoding.Ascii, id);
		}

		public static UOFont GetUnicode(byte id)
		{
			return GetFont(UOEncoding.Unicode, id);
		}

		public static Bitmap GetImage(UOFont font, char c)
		{
			return font[c].GetImage();
		}

		public UOEncoding Encoding { get; private set; }

		public UOFont this[int id] => GetFont(Encoding, (byte)id);

		public int Count { get; private set; }

		public byte DefaultID { get; private set; }

		public UOFonts(UOEncoding enc)
		{
			Encoding = enc;

			switch (Encoding)
			{
				case UOEncoding.Ascii:
				{
					Count = _Ascii.Length;
					DefaultID = 3;
				}
				break;
				case UOEncoding.Unicode:
				{
					Count = _Unicode.Length;
					DefaultID = 1;
				}
				break;
			}
		}

		public Bitmap GetImage(byte font, char c)
		{
			if (font >= Count)
			{
				font = DefaultID;
			}

			return GetImage(this[font], c);
		}

		public int GetWidth(byte font, string text)
		{
			if (font >= Count)
			{
				font = DefaultID;
			}

			return this[font].GetWidth(text);
		}

		public int GetHeight(byte font, string text)
		{
			if (font >= Count)
			{
				font = DefaultID;
			}

			return this[font].GetHeight(text);
		}

		public Size GetSize(byte font, string text)
		{
			if (font >= Count)
			{
				font = DefaultID;
			}

			return this[font].GetSize(text);
		}

		public int GetWidth(byte font, params string[] lines)
		{
			if (font >= Count)
			{
				font = DefaultID;
			}

			return this[font].GetWidth(lines);
		}

		public int GetHeight(byte font, params string[] lines)
		{
			if (font >= Count)
			{
				font = DefaultID;
			}

			return this[font].GetHeight(lines);
		}

		public Size GetSize(byte font, params string[] lines)
		{
			if (font >= Count)
			{
				font = DefaultID;
			}

			return this[font].GetSize(lines);
		}
	}

	public sealed class UOFont
	{
#if MONO
		public const PF PixelFormat = PF.Format32bppArgb;
#else
		public const PF PixelFormat = PF.Format16bppArgb1555;
#endif

		public static Size DefaultCharSize = new Size(8, 10);

		public static UOFonts Ascii { get; private set; }
		public static UOFonts Unicode { get; private set; }

		static UOFont()
		{
			Ascii = new UOFonts(UOEncoding.Ascii);
			Unicode = new UOFonts(UOEncoding.Unicode);

			for (var i = 0; i <= 1; i++)
			{
				VitaNexCore.ToConsole("[UOFont]: Preloaded {0}", Unicode[i]);
			}
		}

		public static void Configure()
		{ }

		public static UOFont GetFont(UOEncoding enc, byte id)
		{
			return UOFonts.GetFont(enc, id);
		}

		public static UOFont GetAscii(byte id)
		{
			return UOFonts.GetFont(UOEncoding.Ascii, id);
		}

		public static UOFont GetUnicode(byte id)
		{
			return UOFonts.GetFont(UOEncoding.Unicode, id);
		}

		public static Bitmap GetImage(UOFont font, char c)
		{
			return UOFonts.GetImage(font, c);
		}

		public static Bitmap GetAsciiImage(byte font, char c)
		{
			return Ascii.GetImage(font, c);
		}

		public static Bitmap GetUnicodeImage(byte font, char c)
		{
			return Unicode.GetImage(font, c);
		}

		public static int GetAsciiWidth(byte font, string text)
		{
			return Ascii.GetWidth(font, text);
		}

		public static int GetAsciiHeight(byte font, string text)
		{
			return Ascii.GetHeight(font, text);
		}

		public static Size GetAsciiSize(byte font, string text)
		{
			return Ascii.GetSize(font, text);
		}

		public static int GetAsciiWidth(byte font, params string[] lines)
		{
			return Ascii.GetWidth(font, lines);
		}

		public static int GetAsciiHeight(byte font, params string[] lines)
		{
			return Ascii.GetHeight(font, lines);
		}

		public static Size GetAsciiSize(byte font, params string[] lines)
		{
			return Ascii.GetSize(font, lines);
		}

		public static int GetUnicodeWidth(byte font, string text)
		{
			return Unicode.GetWidth(font, text);
		}

		public static int GetUnicodeHeight(byte font, string text)
		{
			return Unicode.GetHeight(font, text);
		}

		public static Size GetUnicodeSize(byte font, string text)
		{
			return Unicode.GetSize(font, text);
		}

		public static int GetUnicodeWidth(byte font, params string[] lines)
		{
			return Unicode.GetWidth(font, lines);
		}

		public static int GetUnicodeHeight(byte font, params string[] lines)
		{
			return Unicode.GetHeight(font, lines);
		}

		public static Size GetUnicodeSize(byte font, params string[] lines)
		{
			return Unicode.GetSize(font, lines);
		}

		public UOEncoding Encoding { get; private set; }

		public byte ID { get; private set; }

		public byte MaxCharWidth { get; private set; }
		public byte MaxCharHeight { get; private set; }

		public byte CharSpacing { get; private set; }
		public byte LineSpacing { get; private set; }

		public byte LineHeight { get; private set; }

		public UOChar[] Chars { get; private set; }

		public int Length => Chars.Length;

		public UOChar this[char c] => Chars[c % Length];
		public UOChar this[int i] => Chars[i % Length];

		public UOFont(
			UOEncoding enc,
			byte id,
			byte charSpacing,
			byte lineSpacing,
			byte charsWidth,
			byte charsHeight,
			UOChar[] chars)
		{
			Encoding = enc;

			ID = id;

			CharSpacing = charSpacing;
			LineSpacing = lineSpacing;
			MaxCharWidth = charsWidth;
			MaxCharHeight = charsHeight;

			Chars = chars;
		}

		public int GetWidth(string value)
		{
			return GetSize(value).Width;
		}

		public int GetHeight(string value)
		{
			return GetSize(value).Height;
		}

		public Size GetSize(string value)
		{
			var lines = value.Split('\n');

			if (lines.Length == 0)
			{
				lines = new[] { value };
			}

			return GetSize(lines);
		}

		public int GetWidth(params string[] lines)
		{
			return GetSize(lines).Width;
		}

		public int GetHeight(params string[] lines)
		{
			return GetSize(lines).Height;
		}

		public Size GetSize(params string[] lines)
		{
			var w = 0;
			var h = 0;

			var space = Chars[' '];

			UOChar ci;

			foreach (var line in lines.SelectMany(o => o.Contains('\n') ? o.Split('\n') : o.ToEnumerable()))
			{
				var lw = 0;
				var lh = 0;

				foreach (var c in line)
				{
					if (c == '\t')
					{
						lw += (CharSpacing + space.Width) * 4;
						continue;
					}

					ci = this[c];

					if (ci == null)
					{
						lw += (CharSpacing + space.Width);
						continue;
					}

					lw += (CharSpacing + ci.XOffset + ci.Width);
					lh = Math.Max(lh, ci.YOffset + ci.Height);
				}

				w = Math.Max(w, lw);
				h += lh + LineSpacing;
			}

			return new Size(w, h);
		}

		public override string ToString()
		{
			return String.Format("({0}, {1}, {2})", Encoding, ID, Length);
		}
	}

	public sealed class UOChar
	{
		public Bitmap Image { get; private set; }

		public UOEncoding Encoding { get; private set; }

		public sbyte XOffset { get; private set; }
		public sbyte YOffset { get; private set; }

		public byte Width { get; private set; }
		public byte Height { get; private set; }

		public UOChar(UOEncoding enc, sbyte ox, sbyte oy, Bitmap image)
		{
			Encoding = enc;

			XOffset = ox;
			YOffset = oy;

			Image = image;

			Width = (byte)Image.Width;
			Height = (byte)Image.Height;
		}

		public Bitmap GetImage()
		{
			return GetImage(false);
		}

		public Bitmap GetImage(bool fill)
		{
			return GetImage(fill, Color555.White);
		}

		public Bitmap GetImage(bool fill, Color555 bgColor)
		{
			return GetImage(fill, bgColor, Color555.Black);
		}

		public unsafe Bitmap GetImage(bool fill, Color555 bgColor, Color555 textColor)
		{
			if (Width * Height <= 0)
			{
				return null;
			}

			var image = new Bitmap(Width, Height, UOFont.PixelFormat);

			var bound = new Rectangle(0, 0, Width, Height);

			var dataSrc = Image.LockBits(bound, ImageLockMode.ReadOnly, UOFont.PixelFormat);
			var lineSrc = (ushort*)dataSrc.Scan0;
			var deltaSrc = dataSrc.Stride >> 1;

			var dataTrg = image.LockBits(bound, ImageLockMode.WriteOnly, UOFont.PixelFormat);
			var lineTrg = (ushort*)dataTrg.Scan0;
			var deltaTrg = dataTrg.Stride >> 1;

			int x, y;

			for (y = 0; y < Height; y++, lineSrc += deltaSrc, lineTrg += deltaTrg)
			{
				var source = lineSrc;
				var target = lineTrg;

				if (source == null || target == null)
				{
					continue;
				}

				for (x = 0; x < Width; x++)
				{
					if (source[x] != 0)
					{
						target[x] = textColor;
					}
					else if (fill)
					{
						target[x] = bgColor;
					}
				}
			}

			Image.UnlockBits(dataSrc);
			image.UnlockBits(dataTrg);

			return image;
		}
	}
}