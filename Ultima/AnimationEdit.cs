#region References
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
#endregion

//using System.Windows.Media.Imaging;

namespace Ultima
{
	public sealed class AnimationEdit
	{
		private static FileIndex m_FileIndex = new FileIndex("Anim.idx", "Anim.mul", 6);
		private static FileIndex m_FileIndex2 = new FileIndex("Anim2.idx", "Anim2.mul", -1);
		private static FileIndex m_FileIndex3 = new FileIndex("Anim3.idx", "Anim3.mul", -1);
		private static FileIndex m_FileIndex4 = new FileIndex("Anim4.idx", "Anim4.mul", -1);
		private static FileIndex m_FileIndex5 = new FileIndex("Anim5.idx", "Anim5.mul", -1);

		private static AnimIdx[] animcache;
		private static readonly AnimIdx[] animcache2;
		private static readonly AnimIdx[] animcache3;
		private static readonly AnimIdx[] animcache4;
		private static readonly AnimIdx[] animcache5;

		static AnimationEdit()
		{
			if (m_FileIndex.IdxLength > 0)
			{
				animcache = new AnimIdx[m_FileIndex.IdxLength / 12];
			}
			if (m_FileIndex2.IdxLength > 0)
			{
				animcache2 = new AnimIdx[m_FileIndex2.IdxLength / 12];
			}
			if (m_FileIndex3.IdxLength > 0)
			{
				animcache3 = new AnimIdx[m_FileIndex3.IdxLength / 12];
			}
			if (m_FileIndex4.IdxLength > 0)
			{
				animcache4 = new AnimIdx[m_FileIndex4.IdxLength / 12];
			}
			if (m_FileIndex5.IdxLength > 0)
			{
				animcache5 = new AnimIdx[m_FileIndex5.IdxLength / 12];
			}
		}

		/// <summary>
		///     Rereads AnimX files
		/// </summary>
		public static void Reload()
		{
			m_FileIndex = new FileIndex("Anim.idx", "Anim.mul", 6);
			m_FileIndex2 = new FileIndex("Anim2.idx", "Anim2.mul", -1);
			m_FileIndex3 = new FileIndex("Anim3.idx", "Anim3.mul", -1);
			m_FileIndex4 = new FileIndex("Anim4.idx", "Anim4.mul", -1);
			m_FileIndex5 = new FileIndex("Anim5.idx", "Anim5.mul", -1);
			if (m_FileIndex.IdxLength > 0)
			{
				animcache = new AnimIdx[m_FileIndex.IdxLength / 12];
			}
			if (m_FileIndex2.IdxLength > 0)
			{
				animcache = new AnimIdx[m_FileIndex2.IdxLength / 12];
			}
			if (m_FileIndex3.IdxLength > 0)
			{
				animcache = new AnimIdx[m_FileIndex3.IdxLength / 12];
			}
			if (m_FileIndex4.IdxLength > 0)
			{
				animcache = new AnimIdx[m_FileIndex4.IdxLength / 12];
			}
			if (m_FileIndex5.IdxLength > 0)
			{
				animcache = new AnimIdx[m_FileIndex5.IdxLength / 12];
			}
		}

		private static void GetFileIndex(
			int body, int fileType, int action, int direction, out FileIndex fileIndex, out int index)
		{
			switch (fileType)
			{
				default:
				case 1:
					fileIndex = m_FileIndex;
					if (body < 200)
					{
						index = body * 110;
					}
					else if (body < 400)
					{
						index = 22000 + ((body - 200) * 65);
					}
					else
					{
						index = 35000 + ((body - 400) * 175);
					}
					break;
				case 2:
					fileIndex = m_FileIndex2;
					if (body < 200)
					{
						index = body * 110;
					}
					else
					{
						index = 22000 + ((body - 200) * 65);
					}
					break;
				case 3:
					fileIndex = m_FileIndex3;
					if (body < 300)
					{
						index = body * 65;
					}
					else if (body < 400)
					{
						index = 33000 + ((body - 300) * 110);
					}
					else
					{
						index = 35000 + ((body - 400) * 175);
					}
					break;
				case 4:
					fileIndex = m_FileIndex4;
					if (body < 200)
					{
						index = body * 110;
					}
					else if (body < 400)
					{
						index = 22000 + ((body - 200) * 65);
					}
					else
					{
						index = 35000 + ((body - 400) * 175);
					}
					break;
				case 5:
					fileIndex = m_FileIndex5;
					if ((body < 200) && (body != 34)) // looks strange, though it works.
					{
						index = body * 110;
					}
					else if (body < 400)
					{
						index = 22000 + ((body - 200) * 65);
					}
					else
					{
						index = 35000 + ((body - 400) * 175);
					}
					break;
			}

			index += action * 5;

			if (direction <= 4)
			{
				index += direction;
			}
			else
			{
				index += direction - (direction - 4) * 2;
			}
		}

		private static AnimIdx[] GetCache(int filetype)
		{
			switch (filetype)
			{
				case 1:
					return animcache;
				case 2:
					return animcache2;
				case 3:
					return animcache3;
				case 4:
					return animcache4;
				case 5:
					return animcache5;
				default:
					return animcache;
			}
		}

		public static AnimIdx GetAnimation(int filetype, int body, int action, int dir)
		{
			AnimIdx[] cache = GetCache(filetype);
			FileIndex fileIndex;
			int index;
			GetFileIndex(body, filetype, action, dir, out fileIndex, out index);

			if (cache != null)
			{
				if (cache[index] != null)
				{
					return cache[index];
				}
			}
			return cache[index] = new AnimIdx(index, fileIndex, filetype);
		}

		public static bool IsActionDefinied(int filetype, int body, int action)
		{
			AnimIdx[] cache = GetCache(filetype);
			FileIndex fileIndex;
			int index;
			GetFileIndex(body, filetype, action, 0, out fileIndex, out index);

			if (cache != null)
			{
				if (cache[index] != null)
				{
					if ((cache[index].Frames != null) && (cache[index].Frames.Count > 0))
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}

			int AnimCount = Animations.GetAnimLength(body, filetype);
			if (AnimCount < action)
			{
				return false;
			}

			int length, extra;
			bool patched;
			bool valid = fileIndex.Valid(index, out length, out extra, out patched);
			if ((!valid) || (length < 1))
			{
				return false;
			}
			return true;
		}

		public static void LoadFromVD(int filetype, int body, BinaryReader bin)
		{
			AnimIdx[] cache = GetCache(filetype);
			FileIndex fileIndex;
			int index;
			GetFileIndex(body, filetype, 0, 0, out fileIndex, out index);
			int animlength = Animations.GetAnimLength(body, filetype) * 5;
			var entries = new Entry3D[animlength];

			for (int i = 0; i < animlength; ++i)
			{
				entries[i].lookup = bin.ReadInt32();
				entries[i].length = bin.ReadInt32();
				entries[i].extra = bin.ReadInt32();
			}
			foreach (Entry3D entry in entries)
			{
				if ((entry.lookup > 0) && (entry.lookup < bin.BaseStream.Length) && (entry.length > 0))
				{
					bin.BaseStream.Seek(entry.lookup, SeekOrigin.Begin);
					cache[index] = new AnimIdx(bin, entry.extra);
				}
				++index;
			}
		}

		public static void ExportToVD(int filetype, int body, string file)
		{
			AnimIdx[] cache = GetCache(filetype);
			FileIndex fileIndex;
			int index;
			GetFileIndex(body, filetype, 0, 0, out fileIndex, out index);
			using (var fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Write))
			{
				using (var bin = new BinaryWriter(fs))
				{
					bin.Write((short)6);
					int animlength = Animations.GetAnimLength(body, filetype);
					int currtype = animlength == 22 ? 0 : animlength == 13 ? 1 : 2;
					bin.Write((short)currtype);
					long indexpos = bin.BaseStream.Position;
					long animpos = bin.BaseStream.Position + 12 * animlength * 5;
					for (int i = index; i < index + animlength * 5; i++)
					{
						AnimIdx anim;
						if (cache != null)
						{
							if (cache[i] != null)
							{
								anim = cache[i];
							}
							else
							{
								anim = cache[i] = new AnimIdx(i, fileIndex, filetype);
							}
						}
						else
						{
							anim = cache[i] = new AnimIdx(i, fileIndex, filetype);
						}

						if (anim == null)
						{
							bin.BaseStream.Seek(indexpos, SeekOrigin.Begin);
							bin.Write(-1);
							bin.Write(-1);
							bin.Write(-1);
							indexpos = bin.BaseStream.Position;
						}
						else
						{
							anim.ExportToVD(bin, ref indexpos, ref animpos);
						}
					}
				}
			}
		}

		public static void Save(int filetype, string path)
		{
			string filename;
			AnimIdx[] cache;
			FileIndex fileindex;
			switch (filetype)
			{
				case 1:
					filename = "anim";
					cache = animcache;
					fileindex = m_FileIndex;
					break;
				case 2:
					filename = "anim2";
					cache = animcache2;
					fileindex = m_FileIndex2;
					break;
				case 3:
					filename = "anim3";
					cache = animcache3;
					fileindex = m_FileIndex3;
					break;
				case 4:
					filename = "anim4";
					cache = animcache4;
					fileindex = m_FileIndex4;
					break;
				case 5:
					filename = "anim5";
					cache = animcache5;
					fileindex = m_FileIndex5;
					break;
				default:
					filename = "anim";
					cache = animcache;
					fileindex = m_FileIndex;
					break;
			}
			string idx = Path.Combine(path, filename + ".idx");
			string mul = Path.Combine(path, filename + ".mul");
			using (
				FileStream fsidx = new FileStream(idx, FileMode.Create, FileAccess.Write, FileShare.Write),
						   fsmul = new FileStream(mul, FileMode.Create, FileAccess.Write, FileShare.Write))
			{
				using (BinaryWriter binidx = new BinaryWriter(fsidx), binmul = new BinaryWriter(fsmul))
				{
					for (int idxc = 0; idxc < cache.Length; ++idxc)
					{
						AnimIdx anim;
						if (cache != null)
						{
							if (cache[idxc] != null)
							{
								anim = cache[idxc];
							}
							else
							{
								anim = cache[idxc] = new AnimIdx(idxc, fileindex, filetype);
							}
						}
						else
						{
							anim = cache[idxc] = new AnimIdx(idxc, fileindex, filetype);
						}

						if (anim == null)
						{
							binidx.Write(-1);
							binidx.Write(-1);
							binidx.Write(-1);
						}
						else
						{
							anim.Save(binmul, binidx);
						}
					}
				}
			}
		}
	}

	public sealed class AnimIdx
	{
		public int idxextra;
		public ushort[] Palette { get; private set; }
		public List<FrameEdit> Frames { get; private set; }

		public AnimIdx(int index, FileIndex fileIndex, int filetype)
		{
			Palette = new ushort[0x100];
			int length, extra;
			bool patched;
			Stream stream = fileIndex.Seek(index, out length, out extra, out patched);
			if ((stream == null) || (length < 1))
			{
				return;
			}

			idxextra = extra;
			using (var bin = new BinaryReader(stream))
			{
				for (int i = 0; i < 0x100; ++i)
				{
					Palette[i] = (ushort)(bin.ReadUInt16() ^ 0x8000);
				}

				var start = (int)bin.BaseStream.Position;
				int frameCount = bin.ReadInt32();

				var lookups = new int[frameCount];

				for (int i = 0; i < frameCount; ++i)
				{
					lookups[i] = start + bin.ReadInt32();
				}

				Frames = new List<FrameEdit>();

				for (int i = 0; i < frameCount; ++i)
				{
					stream.Seek(lookups[i], SeekOrigin.Begin);
					Frames.Add(new FrameEdit(bin));
				}
			}
			stream.Close();
		}

		public AnimIdx(BinaryReader bin, int extra)
		{
			Palette = new ushort[0x100];
			idxextra = extra;
			for (int i = 0; i < 0x100; ++i)
			{
				Palette[i] = (ushort)(bin.ReadUInt16() ^ 0x8000);
			}

			var start = (int)bin.BaseStream.Position;
			int frameCount = bin.ReadInt32();

			var lookups = new int[frameCount];

			for (int i = 0; i < frameCount; ++i)
			{
				lookups[i] = start + bin.ReadInt32();
			}

			Frames = new List<FrameEdit>();

			for (int i = 0; i < frameCount; ++i)
			{
				bin.BaseStream.Seek(lookups[i], SeekOrigin.Begin);
				Frames.Add(new FrameEdit(bin));
			}
		}

		public unsafe Bitmap[] GetFrames()
		{
			if ((Frames == null) || (Frames.Count == 0))
			{
				return null;
			}
			var bits = new Bitmap[Frames.Count];
			for (int i = 0; i < bits.Length; ++i)
			{
				FrameEdit frame = Frames[i];
				int width = frame.width;
				int height = frame.height;
				if (height == 0 || width == 0)
				{
					continue;
				}
				var bmp = new Bitmap(width, height, PixelFormat.Format16bppArgb1555);
				BitmapData bd = bmp.LockBits(
					new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format16bppArgb1555);
				var line = (ushort*)bd.Scan0;
				int delta = bd.Stride >> 1;

				int xBase = frame.Center.X - 0x200;
				int yBase = frame.Center.Y + height - 0x200;

				line += xBase;
				line += yBase * delta;
				for (int j = 0; j < frame.RawData.Length; ++j)
				{
					FrameEdit.Raw raw = frame.RawData[j];

					ushort* cur = line + (((raw.offy) * delta) + ((raw.offx) & 0x3FF));
					ushort* end = cur + (raw.run);

					int ii = 0;
					while (cur < end)
					{
						*cur++ = Palette[raw.data[ii++]];
					}
				}
				bmp.UnlockBits(bd);
				bits[i] = bmp;
			}
			return bits;
		}

		public void AddFrame(Bitmap bit)
		{
			if (Frames == null)
			{
				Frames = new List<FrameEdit>();
			}
			Frames.Add(new FrameEdit(bit, Palette, 0, 0));
		}

		public void ReplaceFrame(Bitmap bit, int index)
		{
			if ((Frames == null) || (Frames.Count == 0))
			{
				return;
			}
			if (index > Frames.Count)
			{
				return;
			}
			Frames[index] = new FrameEdit(bit, Palette, (Frames[index]).Center.X, (Frames[index]).Center.Y);
		}

		public void RemoveFrame(int index)
		{
			if (Frames == null)
			{
				return;
			}
			if (index > Frames.Count)
			{
				return;
			}
			Frames.RemoveAt(index);
		}

		public void ClearFrames()
		{
			if (Frames == null)
			{
				return;
			}
			Frames.Clear();
		}

#if false
	//Soulblighter Modification
		public void GetGifPalette(Bitmap bit)
		{
			using (MemoryStream imageStreamSource = new MemoryStream())
			{
				System.Drawing.ImageConverter ic = new System.Drawing.ImageConverter();
				byte[] btImage = (byte[])ic.ConvertTo(bit, typeof(byte[]));
				imageStreamSource.Write(btImage, 0, btImage.Length);
				GifBitmapDecoder decoder = new GifBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
				BitmapPalette pal = decoder.Palette;
				int i;
				for (i = 0; i < 0x100; i++)
				{
					this.Palette[i] = 0;
				}
				try
				{
					i = 0;
					while (i < 0x100)//&& i < pal.Colors.Count)
					{

						int Red = pal.Colors[i].R / 8;
						int Green = pal.Colors[i].G / 8;
						int Blue = pal.Colors[i].B / 8;
						int contaFinal = (((0x400 * Red) + (0x20 * Green)) + Blue) + 0x8000;
						if (contaFinal == 0x8000)
							contaFinal = 0x8001;
						this.Palette[i] = (ushort)contaFinal;
						i++;
					}
				}
				catch (System.IndexOutOfRangeException)
				{ }
				catch (System.ArgumentOutOfRangeException)
				{ }
				for (i = 0; i < 0x100; i++)
				{
					if (this.Palette[i] < 0x8000)
						this.Palette[i] = 0x8000;
				}
			}
		}
#endif

		public unsafe void GetImagePalette(Bitmap bit)
		{
			int count = 0;
			var bmp = new Bitmap(bit);
			BitmapData bd = bmp.LockBits(
				new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format16bppArgb1555);
			var line = (ushort*)bd.Scan0;
			int delta = bd.Stride >> 1;
			ushort* cur = line;
			int i = 0;
			while (i < 0x100)
			{
				Palette[i] = 0;
				i++;
			}
			int y = 0;
			while (y < bmp.Height)
			{
				cur = line;
				for (int x = 0; x < bmp.Width; x++)
				{
					ushort c = cur[x];
					if (c != 0)
					{
						bool found = false;
						i = 0;
						while (i < Palette.Length)
						{
							if (Palette[i] == c)
							{
								found = true;
								break;
							}
							i++;
						}
						if (!found)
						{
							Palette[count++] = c;
						}
						if (count >= 0x100)
						{
							break;
						}
					}
				}
				for (i = 0; i < 0x100; i++)
				{
					if (Palette[i] < 0x8000)
					{
						Palette[i] = 0x8000;
					}
				}
				if (count >= 0x100)
				{
					break;
				}
				y++;
				line += delta;
			}
		}

		public void PaletteConversor(int seletor)
		{
			int i;
			for (i = 0; i < 0x100; i++)
			{
				int BlueTemp = (Palette[i] - 0x8000) / 0x20;
				BlueTemp *= 0x20;
				BlueTemp = (Palette[i] - 0x8000) - BlueTemp;
				int GreenTemp = (Palette[i] - 0x8000) / 0x400;
				GreenTemp *= 0x400;
				GreenTemp = ((Palette[i] - 0x8000) - GreenTemp) - BlueTemp;
				GreenTemp /= 0x20;
				int RedTemp = (Palette[i] - 0x8000) / 0x400;
				int contaFinal = 0;
				switch (seletor)
				{
					case 1:
						contaFinal = (((0x400 * RedTemp) + (0x20 * GreenTemp)) + BlueTemp) + 0x8000;
						break;
					case 2:
						contaFinal = (((0x400 * RedTemp) + (0x20 * BlueTemp)) + GreenTemp) + 0x8000;
						break;
					case 3:
						contaFinal = (((0x400 * GreenTemp) + (0x20 * RedTemp)) + BlueTemp) + 0x8000;
						break;
					case 4:
						contaFinal = (((0x400 * GreenTemp) + (0x20 * BlueTemp)) + RedTemp) + 0x8000;
						break;
					case 5:
						contaFinal = (((0x400 * BlueTemp) + (0x20 * GreenTemp)) + RedTemp) + 0x8000;
						break;
					case 6:
						contaFinal = (((0x400 * BlueTemp) + (0x20 * RedTemp)) + GreenTemp) + 0x8000;
						break;
				}
				if (contaFinal == 0x8000)
				{
					contaFinal = 0x8001;
				}
				Palette[i] = (ushort)contaFinal;
			}
			for (i = 0; i < 0x100; i++)
			{
				if (Palette[i] < 0x8000)
				{
					Palette[i] = 0x8000;
				}
			}
		}

		public void PaletteReductor(int Redp, int Greenp, int Bluep)
		{
			int i;
			Redp /= 8;
			Greenp /= 8;
			Bluep /= 8;
			for (i = 0; i < 0x100; i++)
			{
				int BlueTemp = (Palette[i] - 0x8000) / 0x20;
				BlueTemp *= 0x20;
				BlueTemp = (Palette[i] - 0x8000) - BlueTemp;
				int GreenTemp = (Palette[i] - 0x8000) / 0x400;
				GreenTemp *= 0x400;
				GreenTemp = ((Palette[i] - 0x8000) - GreenTemp) - BlueTemp;
				GreenTemp /= 0x20;
				int RedTemp = (Palette[i] - 0x8000) / 0x400;
				RedTemp += Redp;
				GreenTemp += Greenp;
				BlueTemp += Bluep;
				if (RedTemp < 0)
				{
					RedTemp = 0;
				}
				if (RedTemp > 0x1f)
				{
					RedTemp = 0x1f;
				}
				if (GreenTemp < 0)
				{
					GreenTemp = 0;
				}
				if (GreenTemp > 0x1f)
				{
					GreenTemp = 0x1f;
				}
				if (BlueTemp < 0)
				{
					BlueTemp = 0;
				}
				if (BlueTemp > 0x1f)
				{
					BlueTemp = 0x1f;
				}
				int contaFinal = (((0x400 * RedTemp) + (0x20 * GreenTemp)) + BlueTemp) + 0x8000;
				if (contaFinal == 0x8000)
				{
					contaFinal = 0x8001;
				}
				Palette[i] = (ushort)contaFinal;
			}
			for (i = 0; i < 0x100; i++)
			{
				if (Palette[i] < 0x8000)
				{
					Palette[i] = 0x8000;
				}
			}
		}

		//End of Soulblighter Modification

		public unsafe void ExportPalette(string filename, int type)
		{
			switch (type)
			{
				case 0:
					using (var Tex = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.ReadWrite)))
					{
						for (int i = 0; i < 0x100; ++i)
						{
							Tex.WriteLine(Palette[i]);
						}
					}
					break;
				case 1:
					{
						var bmp = new Bitmap(0x100, 20, PixelFormat.Format16bppArgb1555);
						BitmapData bd = bmp.LockBits(
							new Rectangle(0, 0, 0x100, 20), ImageLockMode.WriteOnly, PixelFormat.Format16bppArgb1555);
						var line = (ushort*)bd.Scan0;
						int delta = bd.Stride >> 1;
						for (int y = 0; y < bd.Height; ++y, line += delta)
						{
							ushort* cur = line;
							for (int i = 0; i < 0x100; ++i)
							{
								*cur++ = Palette[i];
							}
						}
						bmp.UnlockBits(bd);
						var b = new Bitmap(bmp);
						b.Save(filename, ImageFormat.Bmp);
						b.Dispose();
						bmp.Dispose();
						break;
					}
				case 2:
					{
						var bmp = new Bitmap(0x100, 20, PixelFormat.Format16bppArgb1555);
						BitmapData bd = bmp.LockBits(
							new Rectangle(0, 0, 0x100, 20), ImageLockMode.WriteOnly, PixelFormat.Format16bppArgb1555);
						var line = (ushort*)bd.Scan0;
						int delta = bd.Stride >> 1;
						for (int y = 0; y < bd.Height; ++y, line += delta)
						{
							ushort* cur = line;
							for (int i = 0; i < 0x100; ++i)
							{
								*cur++ = Palette[i];
							}
						}
						bmp.UnlockBits(bd);
						var b = new Bitmap(bmp);
						b.Save(filename, ImageFormat.Tiff);
						b.Dispose();
						bmp.Dispose();
						break;
					}
			}
		}

		public void ReplacePalette(ushort[] palette)
		{
			Palette = palette;
		}

		public void Save(BinaryWriter bin, BinaryWriter idx)
		{
			if ((Frames == null) || (Frames.Count == 0))
			{
				idx.Write(-1);
				idx.Write(-1);
				idx.Write(-1);
				return;
			}
			long start = bin.BaseStream.Position;
			idx.Write((int)start);

			for (int i = 0; i < 0x100; ++i)
			{
				bin.Write((ushort)(Palette[i] ^ 0x8000));
			}
			long startpos = bin.BaseStream.Position;
			bin.Write(Frames.Count);
			long seek = bin.BaseStream.Position;
			long curr = bin.BaseStream.Position + 4 * Frames.Count;
			foreach (FrameEdit frame in Frames)
			{
				bin.BaseStream.Seek(seek, SeekOrigin.Begin);
				bin.Write((int)(curr - startpos));
				seek = bin.BaseStream.Position;
				bin.BaseStream.Seek(curr, SeekOrigin.Begin);
				frame.Save(bin);
				curr = bin.BaseStream.Position;
			}

			start = bin.BaseStream.Position - start;
			idx.Write((int)start);
			idx.Write(idxextra);
		}

		public void ExportToVD(BinaryWriter bin, ref long indexpos, ref long animpos)
		{
			bin.BaseStream.Seek(indexpos, SeekOrigin.Begin);
			if ((Frames == null) || (Frames.Count == 0))
			{
				bin.Write(-1);
				bin.Write(-1);
				bin.Write(-1);
				indexpos = bin.BaseStream.Position;
				return;
			}
			bin.Write((int)animpos);
			indexpos = bin.BaseStream.Position;
			bin.BaseStream.Seek(animpos, SeekOrigin.Begin);

			for (int i = 0; i < 0x100; ++i)
			{
				bin.Write((ushort)(Palette[i] ^ 0x8000));
			}
			long startpos = (int)bin.BaseStream.Position;
			bin.Write(Frames.Count);
			long seek = (int)bin.BaseStream.Position;
			long curr = bin.BaseStream.Position + 4 * Frames.Count;
			foreach (FrameEdit frame in Frames)
			{
				bin.BaseStream.Seek(seek, SeekOrigin.Begin);
				bin.Write((int)(curr - startpos));
				seek = bin.BaseStream.Position;
				bin.BaseStream.Seek(curr, SeekOrigin.Begin);
				frame.Save(bin);
				curr = bin.BaseStream.Position;
			}

			long length = bin.BaseStream.Position - animpos;
			animpos = bin.BaseStream.Position;
			bin.BaseStream.Seek(indexpos, SeekOrigin.Begin);
			bin.Write((int)length);
			bin.Write(idxextra);
			indexpos = bin.BaseStream.Position;
		}
	}

	public sealed class FrameEdit
	{
		private const int DoubleXor = (0x200 << 22) | (0x200 << 12);

		public struct Raw
		{
			public int run;
			public int offx;
			public int offy;
			public byte[] data;
		}

		public Raw[] RawData { get; private set; }
		public Point Center { get; set; }
		public int width;
		public int height;

		public FrameEdit(BinaryReader bin)
		{
			int xCenter = bin.ReadInt16();
			int yCenter = bin.ReadInt16();

			width = bin.ReadUInt16();
			height = bin.ReadUInt16();
			if (height == 0 || width == 0)
			{
				return;
			}
			int header;

			var tmp = new List<Raw>();
			while ((header = bin.ReadInt32()) != 0x7FFF7FFF)
			{
				var raw = new Raw();
				header ^= DoubleXor;
				raw.run = (header & 0xFFF);
				raw.offy = ((header >> 12) & 0x3FF);
				raw.offx = ((header >> 22) & 0x3FF);

				int i = 0;
				raw.data = new byte[raw.run];
				while (i < raw.run)
				{
					raw.data[i++] = bin.ReadByte();
				}
				tmp.Add(raw);
			}
			RawData = tmp.ToArray();
			Center = new Point(xCenter, yCenter);
		}

		public unsafe FrameEdit(Bitmap bit, ushort[] palette, int centerx, int centery)
		{
			Center = new Point(centerx, centery);
			width = bit.Width;
			height = bit.Height;
			BitmapData bd = bit.LockBits(
				new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format16bppArgb1555);
			var line = (ushort*)bd.Scan0;
			int delta = bd.Stride >> 1;
			var tmp = new List<Raw>();

			int X = 0;
			for (int Y = 0; Y < bit.Height; ++Y, line += delta)
			{
				ushort* cur = line;
				int i = 0;
				int j = 0;
				X = 0;
				while (i < bit.Width)
				{
					i = X;
					for (i = X; i <= bit.Width; ++i)
					{
						//first pixel set
						if (i < bit.Width)
						{
							if (cur[i] != 0)
							{
								break;
							}
						}
					}
					if (i < bit.Width)
					{
						for (j = (i + 1); j < bit.Width; ++j)
						{
							//next non set pixel
							if (cur[j] == 0)
							{
								break;
							}
						}
						var raw = new Raw();
						raw.run = j - i;
						raw.offx = j - raw.run - centerx;
						raw.offx += 512;
						raw.offy = Y - centery - bit.Height;
						raw.offy += 512;

						int r = 0;
						raw.data = new byte[raw.run];
						while (r < raw.run)
						{
							ushort col = (cur[r + i]);
							raw.data[r++] = GetPaletteIndex(palette, col);
						}
						tmp.Add(raw);
						X = j + 1;
						i = X;
					}
				}
			}

			RawData = tmp.ToArray();
			bit.UnlockBits(bd);
		}

		public void ChangeCenter(int x, int y)
		{
			for (int i = 0; i < RawData.Length; i++)
			{
				RawData[i].offx += Center.X;
				RawData[i].offx -= x;
				RawData[i].offy += Center.Y;
				RawData[i].offy -= y;
			}
			Center = new Point(x, y);
		}

		private static byte GetPaletteIndex(ushort[] palette, ushort col)
		{
			for (int i = 0; i < palette.Length; i++)
			{
				if (palette[i] == col)
				{
					return (byte)i;
				}
			}
			return 0;
		}

		public void Save(BinaryWriter bin)
		{
			bin.Write((short)Center.X);
			bin.Write((short)Center.Y);
			bin.Write((ushort)width);
			bin.Write((ushort)height);
			if (RawData != null)
			{
				for (int j = 0; j < RawData.Length; j++)
				{
					int newHeader = RawData[j].run | (RawData[j].offy << 12) | (RawData[j].offx << 22);
					newHeader ^= DoubleXor;
					bin.Write(newHeader);
					foreach (byte b in RawData[j].data)
					{
						bin.Write(b);
					}
				}
			}
			bin.Write(0x7FFF7FFF);
		}
	}
}