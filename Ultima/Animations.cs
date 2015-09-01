#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
#endregion

namespace Ultima
{
	/// <summary>
	///     Contains translation tables used for mapping body values to file subsets.
	///     <seealso cref="Animations" />
	/// </summary>
	public sealed class BodyConverter
	{
		public static int[] Table1 { get; private set; }
		public static int[] Table2 { get; private set; }
		public static int[] Table3 { get; private set; }
		public static int[] Table4 { get; private set; }

		private BodyConverter()
		{ }

		static BodyConverter()
		{
			Initialize();
		}

		/// <summary>
		///     Fills bodyconv.def Tables
		/// </summary>
		public static void Initialize()
		{
			string path = Files.GetFilePath("bodyconv.def");

			if (path == null)
			{
				return;
			}

			List<int> list1 = new List<int>(), list2 = new List<int>(), list3 = new List<int>(), list4 = new List<int>();
			int max1 = 0, max2 = 0, max3 = 0, max4 = 0;

			using (var ip = new StreamReader(path))
			{
				string line;

				while ((line = ip.ReadLine()) != null)
				{
					if ((line = line.Trim()).Length == 0 || line.StartsWith("#"))
					{
						continue;
					}

					try
					{
						string[] split = line.Split('\t');

						int original = System.Convert.ToInt32(split[0]);
						int anim2 = System.Convert.ToInt32(split[1]);
						int anim3;
						int anim4;
						int anim5;

						try
						{
							anim3 = System.Convert.ToInt32(split[2]);
						}
						catch
						{
							anim3 = -1;
						}

						try
						{
							anim4 = System.Convert.ToInt32(split[3]);
						}
						catch
						{
							anim4 = -1;
						}

						try
						{
							anim5 = System.Convert.ToInt32(split[4]);
						}
						catch
						{
							anim5 = -1;
						}

						if (anim2 != -1)
						{
							if (anim2 == 68)
							{
								anim2 = 122;
							}

							if (original > max1)
							{
								max1 = original;
							}

							list1.Add(original);
							list1.Add(anim2);
						}

						if (anim3 != -1)
						{
							if (original > max2)
							{
								max2 = original;
							}

							list2.Add(original);
							list2.Add(anim3);
						}

						if (anim4 != -1)
						{
							if (original > max3)
							{
								max3 = original;
							}

							list3.Add(original);
							list3.Add(anim4);
						}

						if (anim5 != -1)
						{
							if (original > max4)
							{
								max4 = original;
							}

							list4.Add(original);
							list4.Add(anim5);
						}
					}
					catch
					{ }
				}
			}

			Table1 = new int[max1 + 1];

			for (int i = 0; i < Table1.Length; ++i)
			{
				Table1[i] = -1;
			}

			for (int i = 0; i < list1.Count; i += 2)
			{
				Table1[list1[i]] = list1[i + 1];
			}

			Table2 = new int[max2 + 1];

			for (int i = 0; i < Table2.Length; ++i)
			{
				Table2[i] = -1;
			}

			for (int i = 0; i < list2.Count; i += 2)
			{
				Table2[list2[i]] = list2[i + 1];
			}

			Table3 = new int[max3 + 1];

			for (int i = 0; i < Table3.Length; ++i)
			{
				Table3[i] = -1;
			}

			for (int i = 0; i < list3.Count; i += 2)
			{
				Table3[list3[i]] = list3[i + 1];
			}

			Table4 = new int[max4 + 1];

			for (int i = 0; i < Table4.Length; ++i)
			{
				Table4[i] = -1;
			}

			for (int i = 0; i < list4.Count; i += 2)
			{
				Table4[list4[i]] = list4[i + 1];
			}
		}

		/// <summary>
		///     Checks to see if <paramref name="body" /> is contained within the mapping table.
		/// </summary>
		/// <returns>True if it is, false if not.</returns>
		public static bool Contains(int body)
		{
			if (Table1 != null && body >= 0 && body < Table1.Length && Table1[body] != -1)
			{
				return true;
			}

			if (Table2 != null && body >= 0 && body < Table2.Length && Table2[body] != -1)
			{
				return true;
			}

			if (Table3 != null && body >= 0 && body < Table3.Length && Table3[body] != -1)
			{
				return true;
			}

			if (Table4 != null && body >= 0 && body < Table4.Length && Table4[body] != -1)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		///     Attempts to convert <paramref name="body" /> to a body index relative to a file subset, specified by the return value.
		/// </summary>
		/// <returns>
		///     A value indicating a file subset:
		///     <list type="table">
		///         <listheader>
		///             <term>Return Value</term>
		///             <description>File Subset</description>
		///         </listheader>
		///         <item>
		///             <term>1</term>
		///             <description>Anim.mul, Anim.idx (Standard)</description>
		///         </item>
		///         <item>
		///             <term>2</term>
		///             <description>Anim2.mul, Anim2.idx (LBR)</description>
		///         </item>
		///         <item>
		///             <term>3</term>
		///             <description>Anim3.mul, Anim3.idx (AOS)</description>
		///         </item>
		///         <item>
		///             <term>4</term>
		///             <description>Anim4.mul, Anim4.idx (SE)</description>
		///         </item>
		///         <item>
		///             <term>5</term>
		///             <description>Anim5.mul, Anim5.idx (ML)</description>
		///         </item>
		///     </list>
		/// </returns>
		public static int Convert(ref int body)
		{
			if (Table1 != null && body >= 0 && body < Table1.Length)
			{
				int val = Table1[body];

				if (val != -1)
				{
					body = val;
					return 2;
				}
			}

			if (Table2 != null && body >= 0 && body < Table2.Length)
			{
				int val = Table2[body];

				if (val != -1)
				{
					body = val;
					return 3;
				}
			}

			if (Table3 != null && body >= 0 && body < Table3.Length)
			{
				int val = Table3[body];

				if (val != -1)
				{
					body = val;
					return 4;
				}
			}

			if (Table4 != null && body >= 0 && body < Table4.Length)
			{
				int val = Table4[body];

				if (val != -1)
				{
					body = val;
					return 5;
				}
			}

			return 1;
		}

		/// <summary>
		///     Converts backward
		/// </summary>
		/// <param name="FileType"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static int GetTrueBody(int FileType, int index)
		{
			switch (FileType)
			{
				default:
				case 1:
					return index;
				case 2:
					if (Table1 != null && index >= 0)
					{
						for (int i = 0; i < Table1.Length; ++i)
						{
							if (Table1[i] == index)
							{
								return i;
							}
						}
					}
					break;
				case 3:
					if (Table2 != null && index >= 0)
					{
						for (int i = 0; i < Table2.Length; ++i)
						{
							if (Table2[i] == index)
							{
								return i;
							}
						}
					}
					break;
				case 4:
					if (Table3 != null && index >= 0)
					{
						for (int i = 0; i < Table3.Length; ++i)
						{
							if (Table3[i] == index)
							{
								return i;
							}
						}
					}
					break;
				case 5:
					if (Table4 != null && index >= 0)
					{
						for (int i = 0; i < Table4.Length; ++i)
						{
							if (Table4[i] == index)
							{
								return i;
							}
						}
					}
					break;
			}
			return -1;
		}
	}

	public sealed class Animations
	{
		private static FileIndex m_FileIndex = new FileIndex("Anim.idx", "Anim.mul", 0x40000, 6);
		//public static FileIndex FileIndex{ get{ return m_FileIndex; } }

		private static FileIndex m_FileIndex2 = new FileIndex("Anim2.idx", "Anim2.mul", 0x10000, -1);
		//public static FileIndex FileIndex2{ get{ return m_FileIndex2; } }

		private static FileIndex m_FileIndex3 = new FileIndex("Anim3.idx", "Anim3.mul", 0x20000, -1);
		//public static FileIndex FileIndex3{ get{ return m_FileIndex3; } }

		private static FileIndex m_FileIndex4 = new FileIndex("Anim4.idx", "Anim4.mul", 0x20000, -1);
		//public static FileIndex FileIndex4{ get{ return m_FileIndex4; } }

		private static FileIndex m_FileIndex5 = new FileIndex("Anim5.idx", "Anim5.mul", 0x20000, -1);
		//public static FileIndex FileIndex5 { get { return m_FileIndex5; } }

		private static byte[] m_StreamBuffer;
		private static MemoryStream m_MemoryStream;

		/// <summary>
		///     Rereads AnimX files and bodyconv, body.def
		/// </summary>
		public static void Reload()
		{
			m_FileIndex = new FileIndex("Anim.idx", "Anim.mul", 0x40000, 6);
			m_FileIndex2 = new FileIndex("Anim2.idx", "Anim2.mul", 0x10000, -1);
			m_FileIndex3 = new FileIndex("Anim3.idx", "Anim3.mul", 0x20000, -1);
			m_FileIndex4 = new FileIndex("Anim4.idx", "Anim4.mul", 0x20000, -1);
			m_FileIndex5 = new FileIndex("Anim5.idx", "Anim5.mul", 0x20000, -1);

			BodyConverter.Initialize();
			BodyTable.Initialize();
		}

		/// <summary>
		///     Returns Framelist
		/// </summary>
		/// <param name="body"></param>
		/// <param name="action"></param>
		/// <param name="direction"></param>
		/// <param name="hue"></param>
		/// <param name="preserveHue">
		///     No Hue override <see cref="bodydev" />
		/// </param>
		/// <param name="FirstFrame"></param>
		/// <returns></returns>
		public static Frame[] GetAnimation(
			int body, int action, int direction, ref int hue, bool preserveHue, bool FirstFrame)
		{
			if (preserveHue)
			{
				Translate(ref body);
			}
			else
			{
				Translate(ref body, ref hue);
			}

			int fileType = BodyConverter.Convert(ref body);

			FileIndex fileIndex;
			int index;
			GetFileIndex(body, action, direction, fileType, out fileIndex, out index);

			int length, extra;
			bool patched;
			Stream stream = fileIndex.Seek(index, out length, out extra, out patched);

			if (stream == null)
			{
				return null;
			}
			if (m_StreamBuffer == null || m_StreamBuffer.Length < length)
			{
				m_StreamBuffer = new byte[length];
			}
			stream.Read(m_StreamBuffer, 0, length);
			m_MemoryStream = new MemoryStream(m_StreamBuffer, false);

			bool flip = direction > 4;
			Frame[] frames;
			using (var bin = new BinaryReader(m_MemoryStream))
			{
				var palette = new ushort[0x100];

				for (int i = 0; i < 0x100; ++i)
				{
					palette[i] = (ushort)(bin.ReadUInt16() ^ 0x8000);
				}

				var start = (int)bin.BaseStream.Position;
				int frameCount = bin.ReadInt32();

				var lookups = new int[frameCount];

				for (int i = 0; i < frameCount; ++i)
				{
					lookups[i] = start + bin.ReadInt32();
				}

				bool onlyHueGrayPixels = (hue & 0x8000) != 0;

				hue = (hue & 0x3FFF) - 1;

				Hue hueObject;

				if (hue >= 0 && hue < Hues.List.Length)
				{
					hueObject = Hues.List[hue];
				}
				else
				{
					hueObject = null;
				}

				if (FirstFrame)
				{
					frameCount = 1;
				}
				frames = new Frame[frameCount];

				for (int i = 0; i < frameCount; ++i)
				{
					bin.BaseStream.Seek(lookups[i], SeekOrigin.Begin);
					frames[i] = new Frame(palette, bin, flip);

					if (hueObject != null)
					{
						if (frames[i] != null)
						{
							if (frames[i].Bitmap != null)
							{
								hueObject.ApplyTo(frames[i].Bitmap, onlyHueGrayPixels);
							}
						}
					}
				}
				bin.Close();
			}
			m_MemoryStream.Close();
			return frames;
		}

		public static Frame[] GetAnimation(int body, int action, int direction, int fileType)
		{
			FileIndex fileIndex;
			int index;
			GetFileIndex(body, action, direction, fileType, out fileIndex, out index);

			int length, extra;
			bool patched;

			Stream stream = fileIndex.Seek(index, out length, out extra, out patched);

			if (stream == null)
			{
				return null;
			}

			bool flip = direction > 4;

			using (var bin = new BinaryReader(stream))
			{
				var palette = new ushort[0x100];

				for (int i = 0; i < 0x100; ++i)
				{
					palette[i] = (ushort)(bin.ReadUInt16() ^ 0x8000);
				}

				var start = (int)bin.BaseStream.Position;
				int frameCount = bin.ReadInt32();

				var lookups = new int[frameCount];

				for (int i = 0; i < frameCount; ++i)
				{
					lookups[i] = start + bin.ReadInt32();
				}

				var frames = new Frame[frameCount];

				for (int i = 0; i < frameCount; ++i)
				{
					bin.BaseStream.Seek(lookups[i], SeekOrigin.Begin);
					frames[i] = new Frame(palette, bin, flip);
				}

				return frames;
			}
		}

		private static int[] m_Table;

		/// <summary>
		///     Translates body (body.def)
		/// </summary>
		/// <param name="body"></param>
		public static void Translate(ref int body)
		{
			if (m_Table == null)
			{
				LoadTable();
			}
			if (body <= 0 || body >= m_Table.Length)
			{
				body = 0;
				return;
			}

			body = m_Table[body] & 0x7FFF;
		}

		/// <summary>
		///     Translates body and hue (body.def)
		/// </summary>
		/// <param name="body"></param>
		/// <param name="hue"></param>
		public static void Translate(ref int body, ref int hue)
		{
			if (m_Table == null)
			{
				LoadTable();
			}
			if (body <= 0 || body >= m_Table.Length)
			{
				body = 0;
				return;
			}

			int table = m_Table[body];

			if ((table & (1 << 31)) != 0)
			{
				body = table & 0x7FFF;

				int vhue = (hue & 0x3FFF) - 1;

				if (vhue < 0 || vhue >= Hues.List.Length)
				{
					hue = (table >> 15) & 0xFFFF;
				}
			}
		}

		private static void LoadTable()
		{
			int count = 400 + ((m_FileIndex.Index.Length - 35000) / 175);

			m_Table = new int[count];

			for (int i = 0; i < count; ++i)
			{
				object o = BodyTable.m_Entries[i];

				if (o == null || BodyConverter.Contains(i))
				{
					m_Table[i] = i;
				}
				else
				{
					var bte = (BodyTableEntry)o;

					m_Table[i] = bte.OldID | (1 << 31) | ((bte.NewHue & 0xFFFF) << 15);
				}
			}
		}

		/// <summary>
		///     Is Body with action and direction definied
		/// </summary>
		/// <param name="body"></param>
		/// <param name="action"></param>
		/// <param name="direction"></param>
		/// <returns></returns>
		public static bool IsActionDefined(int body, int action, int direction)
		{
			Translate(ref body);
			int fileType = BodyConverter.Convert(ref body);
			FileIndex fileIndex;
			int index;
			GetFileIndex(body, action, direction, fileType, out fileIndex, out index);

			int length, extra;
			bool patched;
			bool valid = fileIndex.Valid(index, out length, out extra, out patched);
			if ((!valid) || (length < 1))
			{
				return false;
			}
			return true;
		}

		/// <summary>
		///     Is Animation in given animfile definied
		/// </summary>
		/// <param name="body"></param>
		/// <param name="action"></param>
		/// <param name="dir"></param>
		/// <param name="fileType"></param>
		/// <returns></returns>
		public static bool IsAnimDefinied(int body, int action, int dir, int fileType)
		{
			FileIndex fileIndex;
			int index;
			GetFileIndex(body, action, dir, fileType, out fileIndex, out index);

			int length, extra;
			bool patched;
			Stream stream = fileIndex.Seek(index, out length, out extra, out patched);
			bool def = true;
			if ((stream == null) || (length == 0))
			{
				def = false;
			}
			if (stream != null)
			{
				stream.Close();
			}
			return def;
		}

		/// <summary>
		///     Returns Animationcount in given animfile
		/// </summary>
		/// <param name="fileType"></param>
		/// <returns></returns>
		public static int GetAnimCount(int fileType)
		{
			int count;
			switch (fileType)
			{
				default:
				case 1:
					count = 400 + (int)(m_FileIndex.IdxLength - 35000 * 12) / (12 * 175);
					break;
				case 2:
					count = 200 + (int)(m_FileIndex2.IdxLength - 22000 * 12) / (12 * 65);
					break;
				case 3:
					count = 400 + (int)(m_FileIndex3.IdxLength - 35000 * 12) / (12 * 175);
					break;
				case 4:
					count = 400 + (int)(m_FileIndex4.IdxLength - 35000 * 12) / (12 * 175);
					break;
				case 5:
					count = 400 + (int)(m_FileIndex5.IdxLength - 35000 * 12) / (12 * 175);
					break;
			}
			return count;
		}

		/// <summary>
		///     Actioncount of given Body in given anim file
		/// </summary>
		/// <param name="body"></param>
		/// <param name="fileType"></param>
		/// <returns></returns>
		public static int GetAnimLength(int body, int fileType)
		{
			int length = 0;
			switch (fileType)
			{
				default:
				case 1:
					if (body < 200)
					{
						length = 22; //high
					}
					else if (body < 400)
					{
						length = 13; //low
					}
					else
					{
						length = 35; //people
					}
					break;
				case 2:
					if (body < 200)
					{
						length = 22; //high
					}
					else
					{
						length = 13; //low
					}
					break;
				case 3:
					if (body < 300)
					{
						length = 13;
					}
					else if (body < 400)
					{
						length = 22;
					}
					else
					{
						length = 35;
					}
					break;
				case 4:
					if (body < 200)
					{
						length = 22;
					}
					else if (body < 400)
					{
						length = 13;
					}
					else
					{
						length = 35;
					}
					break;
				case 5:
					if (body < 200)
					{
						length = 22;
					}
					else if (body < 400)
					{
						length = 13;
					}
					else
					{
						length = 35;
					}
					break;
			}
			return length;
		}

		/// <summary>
		///     Gets Fileseek index based on fileType,body,action,direction
		/// </summary>
		/// <param name="body"></param>
		/// <param name="action"></param>
		/// <param name="direction"></param>
		/// <param name="fileType">animX</param>
		/// <param name="fileIndex"></param>
		/// <param name="index"></param>
		private static void GetFileIndex(
			int body, int action, int direction, int fileType, out FileIndex fileIndex, out int index)
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

		/// <summary>
		///     Returns Filename body is in
		/// </summary>
		/// <param name="body"></param>
		/// <returns>anim{0}.mul</returns>
		public static string GetFileName(int body)
		{
			Translate(ref body);
			int fileType = BodyConverter.Convert(ref body);

			if (fileType == 1)
			{
				return "anim.mul";
			}
			else
			{
				return String.Format("anim{0}.mul", fileType);
			}
		}
	}

	public sealed class Frame
	{
		public Point Center { get; set; }
		public Bitmap Bitmap { get; set; }

		private const int DoubleXor = (0x200 << 22) | (0x200 << 12);

		public static readonly Frame Empty = new Frame();
		//public static readonly Frame[] EmptyFrames = new Frame[1] { Empty };

		private Frame()
		{
			Bitmap = new Bitmap(1, 1);
		}

		public unsafe Frame(ushort[] palette, BinaryReader bin, bool flip)
		{
			int xCenter = bin.ReadInt16();
			int yCenter = bin.ReadInt16();

			int width = bin.ReadUInt16();
			int height = bin.ReadUInt16();
			if (height == 0 || width == 0)
			{
				return;
			}
			var bmp = new Bitmap(width, height, PixelFormat.Format16bppArgb1555);
			BitmapData bd = bmp.LockBits(
				new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format16bppArgb1555);
			var line = (ushort*)bd.Scan0;
			int delta = bd.Stride >> 1;

			int header;

			int xBase = xCenter - 0x200;
			int yBase = (yCenter + height) - 0x200;

			if (!flip)
			{
				line += xBase;
				line += yBase * delta;

				while ((header = bin.ReadInt32()) != 0x7FFF7FFF)
				{
					header ^= DoubleXor;

					ushort* cur = line + ((((header >> 12) & 0x3FF) * delta) + ((header >> 22) & 0x3FF));
					ushort* end = cur + (header & 0xFFF);
					while (cur < end)
					{
						*cur++ = palette[bin.ReadByte()];
					}
				}
			}
			else
			{
				line -= xBase - width + 1;
				line += yBase * delta;

				while ((header = bin.ReadInt32()) != 0x7FFF7FFF)
				{
					header ^= DoubleXor;

					ushort* cur = line + ((((header >> 12) & 0x3FF) * delta) - ((header >> 22) & 0x3FF));
					ushort* end = cur - (header & 0xFFF);

					while (cur > end)
					{
						*cur-- = palette[bin.ReadByte()];
					}
				}

				xCenter = width - xCenter;
			}

			bmp.UnlockBits(bd);

			Center = new Point(xCenter, yCenter);
			Bitmap = bmp;
		}
	}

	public sealed class BodyTableEntry
	{
		public int OldID { get; set; }
		public int NewID { get; set; }
		public int NewHue { get; set; }

		public BodyTableEntry(int oldID, int newID, int newHue)
		{
			OldID = oldID;
			NewID = newID;
			NewHue = newHue;
		}
	}

	public sealed class BodyTable
	{
		public static Hashtable m_Entries;

		static BodyTable()
		{
			Initialize();
		}

		public static void Initialize()
		{
			m_Entries = new Hashtable();

			string filePath = Files.GetFilePath("body.def");

			if (filePath == null)
			{
				return;
			}

			using (var def = new StreamReader(filePath))
			{
				string line;

				while ((line = def.ReadLine()) != null)
				{
					if ((line = line.Trim()).Length == 0 || line.StartsWith("#"))
					{
						continue;
					}

					try
					{
						int index1 = line.IndexOf("{");
						int index2 = line.IndexOf("}");

						string param1 = line.Substring(0, index1);
						string param2 = line.Substring(index1 + 1, index2 - index1 - 1);
						string param3 = line.Substring(index2 + 1);

						int indexOf = param2.IndexOf(',');

						if (indexOf > -1)
						{
							param2 = param2.Substring(0, indexOf).Trim();
						}

						int iParam1 = Convert.ToInt32(param1.Trim());
						int iParam2 = Convert.ToInt32(param2.Trim());
						int iParam3 = Convert.ToInt32(param3.Trim());

						m_Entries[iParam1] = new BodyTableEntry(iParam2, iParam1, iParam3);
					}
					catch
					{ }
				}
			}
		}
	}
}