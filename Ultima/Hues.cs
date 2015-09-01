#region References
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
#endregion

namespace Ultima
{
	public sealed class Hues
	{
		private static int[] m_Header;

		public static Hue[] List { get; private set; }

		static Hues()
		{
			Initialize();
		}

		/// <summary>
		///     Reads hues.mul and fills <see cref="List" />
		/// </summary>
		public static void Initialize()
		{
			string path = Files.GetFilePath("hues.mul");
			int index = 0;

			List = new Hue[3000];

			if (path != null)
			{
				using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					int blockCount = (int)fs.Length / 708;

					if (blockCount > 375)
					{
						blockCount = 375;
					}
					m_Header = new int[blockCount];
					int structsize = Marshal.SizeOf(typeof(HueDataMul));
					var buffer = new byte[blockCount * (4 + 8 * structsize)];
					GCHandle gc = GCHandle.Alloc(buffer, GCHandleType.Pinned);
					try
					{
						fs.Read(buffer, 0, buffer.Length);
						long currpos = 0;

						for (int i = 0; i < blockCount; ++i)
						{
							var ptrheader = new IntPtr((long)gc.AddrOfPinnedObject() + currpos);
							currpos += 4;
							m_Header[i] = (int)Marshal.PtrToStructure(ptrheader, typeof(int));

							for (int j = 0; j < 8; ++j, ++index)
							{
								var ptr = new IntPtr((long)gc.AddrOfPinnedObject() + currpos);
								currpos += structsize;
								var cur = (HueDataMul)Marshal.PtrToStructure(ptr, typeof(HueDataMul));
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

			for (; index < List.Length; ++index)
			{
				List[index] = new Hue(index);
			}
		}

		public static void Save(string path)
		{
			string mul = Path.Combine(path, "hues.mul");
			using (var fsmul = new FileStream(mul, FileMode.Create, FileAccess.Write, FileShare.Write))
			{
				using (var binmul = new BinaryWriter(fsmul))
				{
					int index = 0;
					for (int i = 0; i < m_Header.Length; ++i)
					{
						binmul.Write(m_Header[i]);
						for (int j = 0; j < 8; ++j, ++index)
						{
							for (int c = 0; c < 32; ++c)
							{
								binmul.Write((short)(List[index].Colors[c] ^ 0x8000));
							}

							binmul.Write((short)(List[index].TableStart ^ 0x8000));
							binmul.Write((short)(List[index].TableEnd ^ 0x8000));
							var b = new byte[20];
							if (List[index].Name != null)
							{
								byte[] bb = Encoding.Default.GetBytes(List[index].Name);
								if (bb.Length > 20)
								{
									Array.Resize(ref bb, 20);
								}
								bb.CopyTo(b, 0);
							}
							binmul.Write(b);
						}
					}
				}
			}
		}

		/// <summary>
		///     Returns <see cref="Hue" />
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static Hue GetHue(int index)
		{
			index &= 0x3FFF;

			if (index >= 0 && index < 3000)
			{
				return List[index];
			}

			return List[0];
		}

		/// <summary>
		///     Converts RGB value to Huecolor
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public static short ColorToHue(Color c)
		{
			ushort origred = c.R;
			ushort origgreen = c.G;
			ushort origblue = c.B;
			const double scale = 31.0 / 255;
			var newred = (ushort)(origred * scale);
			if (newred == 0 && origred != 0)
			{
				newred = 1;
			}
			var newgreen = (ushort)(origgreen * scale);
			if (newgreen == 0 && origgreen != 0)
			{
				newgreen = 1;
			}
			var newblue = (ushort)(origblue * scale);
			if (newblue == 0 && origblue != 0)
			{
				newblue = 1;
			}

			return (short)((newred << 10) | (newgreen << 5) | (newblue));
		}

		/// <summary>
		///     Converts Huecolor to RGBColor
		/// </summary>
		/// <param name="hue"></param>
		/// <returns></returns>
		public static Color HueToColor(short hue)
		{
			const int scale = 255 / 31;
			return Color.FromArgb((((hue & 0x7c00) >> 10) * scale), (((hue & 0x3e0) >> 5) * scale), ((hue & 0x1f) * scale));
		}

		public static int HueToColorR(short hue)
		{
			return (((hue & 0x7c00) >> 10) * (255 / 31));
		}

		public static int HueToColorG(short hue)
		{
			return (((hue & 0x3e0) >> 5) * (255 / 31));
		}

		public static int HueToColorB(short hue)
		{
			return ((hue & 0x1f) * (255 / 31));
		}

		public static unsafe void ApplyTo(Bitmap bmp, short[] Colors, bool onlyHueGrayPixels)
		{
			BitmapData bd = bmp.LockBits(
				new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format16bppArgb1555);

			int stride = bd.Stride >> 1;
			int width = bd.Width;
			int height = bd.Height;
			int delta = stride - width;

			var pBuffer = (ushort*)bd.Scan0;
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

	public sealed class Hue
	{
		public int Index { get; private set; }
		public short[] Colors { get; set; }
		public string Name { get; set; }
		public short TableStart { get; set; }
		public short TableEnd { get; set; }

		public Hue(int index)
		{
			Name = "Null";
			Index = index;
			Colors = new short[32];
			TableStart = 0;
			TableEnd = 0;
		}

		public Color GetColor(int index)
		{
			return Hues.HueToColor(Colors[index]);
		}

		private static readonly byte[] m_StringBuffer = new byte[20];
		private static byte[] m_Buffer = new byte[88];

		public Hue(int index, BinaryReader bin)
		{
			Index = index;
			Colors = new short[32];

			m_Buffer = bin.ReadBytes(88);
			unsafe
			{
				fixed (byte* buffer = m_Buffer)
				{
					var buf = (ushort*)buffer;
					for (int i = 0; i < 32; ++i)
					{
						Colors[i] = (short)(*buf++ | 0x8000);
					}
					TableStart = (short)(*buf++ | 0x8000);
					TableEnd = (short)(*buf++ | 0x8000);
					var sbuf = (byte*)buf;
					int count;
					for (count = 0; count < 20 && *sbuf != 0; ++count)
					{
						m_StringBuffer[count] = *sbuf++;
					}
					Name = Encoding.Default.GetString(m_StringBuffer, 0, count);
					Name = Name.Replace("\n", " ");
				}
			}
		}

		public Hue(int index, HueDataMul mulstruct)
		{
			Index = index;
			Colors = new short[32];
			for (int i = 0; i < 32; ++i)
			{
				Colors[i] = (short)(mulstruct.colors[i] | 0x8000);
			}
			TableStart = (short)(mulstruct.tablestart | 0x8000);
			TableEnd = (short)(mulstruct.tableend | 0x8000);
			Name = NativeMethods.ReadNameString(mulstruct.name, 20);
			Name = Name.Replace("\n", " ");
		}

		/// <summary>
		///     Applies Hue to Bitmap
		/// </summary>
		/// <param name="bmp"></param>
		/// <param name="onlyHueGrayPixels"></param>
		public unsafe void ApplyTo(Bitmap bmp, bool onlyHueGrayPixels)
		{
			BitmapData bd = bmp.LockBits(
				new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format16bppArgb1555);

			int stride = bd.Stride >> 1;
			int width = bd.Width;
			int height = bd.Height;
			int delta = stride - width;

			var pBuffer = (ushort*)bd.Scan0;
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

		public void Export(string FileName)
		{
			using (
				var Tex = new StreamWriter(
					new FileStream(FileName, FileMode.Create, FileAccess.ReadWrite), Encoding.GetEncoding(1252)))
			{
				Tex.WriteLine(Name);
				Tex.WriteLine(((short)(TableStart ^ 0x8000)).ToString());
				Tex.WriteLine(((short)(TableEnd ^ 0x8000)).ToString());
				for (int i = 0; i < Colors.Length; ++i)
				{
					Tex.WriteLine(((short)(Colors[i] ^ 0x8000)).ToString());
				}
			}
		}

		public void Import(string FileName)
		{
			if (!File.Exists(FileName))
			{
				return;
			}
			using (var sr = new StreamReader(FileName))
			{
				string line;
				int i = -3;
				while ((line = sr.ReadLine()) != null)
				{
					line = line.Trim();
					try
					{
						if (i >= Colors.Length)
						{
							break;
						}
						if (i == -3)
						{
							Name = line;
						}
						else if (i == -2)
						{
							TableStart = (short)(ushort.Parse(line) | 0x8000);
						}
						else if (i == -1)
						{
							TableEnd = (short)(ushort.Parse(line) | 0x8000);
						}
						else
						{
							Colors[i] = (short)(ushort.Parse(line) | 0x8000);
						}
						++i;
					}
					catch
					{ }
				}
			}
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct HueDataMul
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public ushort[] colors;

		public ushort tablestart;
		public ushort tableend;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
		public byte[] name;
	}
}