#region References
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
#endregion

namespace Ultima
{
	public sealed class Light
	{
		private static FileIndex m_FileIndex = new FileIndex("lightidx.mul", "light.mul", 100, -1);
		private static Bitmap[] m_Cache = new Bitmap[100];
		private static bool[] m_Removed = new bool[100];
		private static byte[] m_StreamBuffer;

		/// <summary>
		///     ReReads light.mul
		/// </summary>
		public static void Reload()
		{
			m_FileIndex = new FileIndex("lightidx.mul", "light.mul", 100, -1);
			m_Cache = new Bitmap[100];
			m_Removed = new bool[100];
		}

		/// <summary>
		///     Gets count of definied lights
		/// </summary>
		/// <returns></returns>
		public static int GetCount()
		{
			string idxPath = Files.GetFilePath("lightidx.mul");
			if (idxPath == null)
			{
				return 0;
			}
			using (var index = new FileStream(idxPath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				return (int)(index.Length / 12);
			}
		}

		/// <summary>
		///     Tests if given index is valid
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static bool TestLight(int index)
		{
			if (m_Removed[index])
			{
				return false;
			}
			if (m_Cache[index] != null)
			{
				return true;
			}

			int length, extra;
			bool patched;

			Stream stream = m_FileIndex.Seek(index, out length, out extra, out patched);

			if (stream == null)
			{
				return false;
			}
			stream.Close();
			int width = (extra & 0xFFFF);
			int height = ((extra >> 16) & 0xFFFF);
			if ((width > 0) && (height > 0))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		///     Removes Light <see cref="m_Removed" />
		/// </summary>
		/// <param name="index"></param>
		public static void Remove(int index)
		{
			m_Removed[index] = true;
		}

		/// <summary>
		///     Replaces Light
		/// </summary>
		/// <param name="index"></param>
		/// <param name="bmp"></param>
		public static void Replace(int index, Bitmap bmp)
		{
			m_Cache[index] = bmp;
			m_Removed[index] = false;
		}

		public static byte[] GetRawLight(int index, out int width, out int height)
		{
			width = 0;
			height = 0;
			if (m_Removed[index])
			{
				return null;
			}
			int length, extra;
			bool patched;

			Stream stream = m_FileIndex.Seek(index, out length, out extra, out patched);

			if (stream == null)
			{
				return null;
			}

			width = (extra & 0xFFFF);
			height = ((extra >> 16) & 0xFFFF);
			var buffer = new byte[length];
			stream.Read(buffer, 0, length);
			stream.Close();
			return buffer;
		}

		/// <summary>
		///     Returns Bitmap of given index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static unsafe Bitmap GetLight(int index)
		{
			if (m_Removed[index])
			{
				return null;
			}
			if (m_Cache[index] != null)
			{
				return m_Cache[index];
			}

			int length, extra;
			bool patched;

			Stream stream = m_FileIndex.Seek(index, out length, out extra, out patched);

			if (stream == null)
			{
				return null;
			}

			int width = (extra & 0xFFFF);
			int height = ((extra >> 16) & 0xFFFF);

			if (m_StreamBuffer == null || m_StreamBuffer.Length < length)
			{
				m_StreamBuffer = new byte[length];
			}
			stream.Read(m_StreamBuffer, 0, length);

			var bmp = new Bitmap(width, height, PixelFormat.Format16bppArgb1555);
			BitmapData bd = bmp.LockBits(
				new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format16bppArgb1555);

			var line = (ushort*)bd.Scan0;
			int delta = bd.Stride >> 1;

			fixed (byte* data = m_StreamBuffer)
			{
				var bindat = (sbyte*)data;
				for (int y = 0; y < height; ++y, line += delta)
				{
					ushort* cur = line;
					ushort* end = cur + width;

					while (cur < end)
					{
						sbyte value = *bindat++;
						*cur++ = (ushort)(((0x1f + value) << 10) + ((0x1F + value) << 5) + (0x1F + value));
					}
				}
			}

			bmp.UnlockBits(bd);
			stream.Close();
			if (!Files.CacheData)
			{
				return m_Cache[index] = bmp;
			}
			else
			{
				return bmp;
			}
		}

		public static unsafe void Save(string path)
		{
			string idx = Path.Combine(path, "lightidx.mul");
			string mul = Path.Combine(path, "light.mul");
			using (
				FileStream fsidx = new FileStream(idx, FileMode.Create, FileAccess.Write, FileShare.Write),
						   fsmul = new FileStream(mul, FileMode.Create, FileAccess.Write, FileShare.Write))
			{
				using (BinaryWriter binidx = new BinaryWriter(fsidx), binmul = new BinaryWriter(fsmul))
				{
					for (int index = 0; index < m_Cache.Length; index++)
					{
						if (m_Cache[index] == null)
						{
							m_Cache[index] = GetLight(index);
						}
						Bitmap bmp = m_Cache[index];

						if ((bmp == null) || (m_Removed[index]))
						{
							binidx.Write(-1); // lookup
							binidx.Write(-1); // length
							binidx.Write(-1); // extra
						}
						else
						{
							BitmapData bd = bmp.LockBits(
								new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format16bppArgb1555);
							var line = (ushort*)bd.Scan0;
							int delta = bd.Stride >> 1;

							binidx.Write((int)fsmul.Position); //lookup
							var length = (int)fsmul.Position;

							for (int Y = 0; Y < bmp.Height; ++Y, line += delta)
							{
								ushort* cur = line;
								ushort* end = cur + bmp.Width;
								while (cur < end)
								{
									var value = (sbyte)(((*cur++ >> 10) & 0xffff) - 0x1f);
									if (value > 0) // wtf? but it works...
									{
										--value;
									}
									binmul.Write(value);
								}
							}
							length = (int)fsmul.Position - length;
							binidx.Write(length);
							binidx.Write((bmp.Width << 16) + bmp.Height);
							bmp.UnlockBits(bd);
						}
					}
				}
			}
		}
	}
}