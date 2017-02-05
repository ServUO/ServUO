#region References
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

using Microsoft.Win32;
#endregion

namespace Ultima
{
	public sealed class Files
	{
		public delegate void FileSaveHandler();

		public static event FileSaveHandler FileSaveEvent;

		public static void FireFileSaveEvent()
		{
			if (FileSaveEvent != null)
			{
				FileSaveEvent();
			}
		}

		private static bool m_CacheData = true;
		private static Dictionary<string, string> m_MulPath;
		private static string m_Directory;
		private static string m_RootDir;

		/// <summary>
		///     Should loaded Data be cached
		/// </summary>
		public static bool CacheData { get { return m_CacheData; } set { m_CacheData = value; } }

		/// <summary>
		///     Should a Hashfile be used to speed up loading
		/// </summary>
		public static bool UseHashFile { get; set; }

		/// <summary>
		///     Contains the path infos
		/// </summary>
		public static Dictionary<string, string> MulPath { get { return m_MulPath; } set { m_MulPath = value; } }

		/// <summary>
		///     Gets a list of paths to the Client's data files.
		/// </summary>
		public static string Directory { get { return m_Directory; } }

		/// <summary>
		///     Contains the rootDir (so relative values are possible for <see cref="MulPath" />
		/// </summary>
		public static string RootDir { get { return m_RootDir; } set { m_RootDir = value; } }

		private static readonly string[] m_Files = new[]
		{
			"anim.idx", "anim.mul", "anim2.idx", "anim2.mul", "anim3.idx", "anim3.mul", "anim4.idx", "anim4.mul", "anim5.idx",
			"anim5.mul", "animdata.mul", "art.mul", "artidx.mul", "artlegacymul.uop", "body.def", "bodyconv.def", "client.exe",
			"cliloc.custom1", "cliloc.custom2", "cliloc.deu", "cliloc.enu", "equipconv.def", "facet00.mul", "facet01.mul",
			"facet02.mul", "facet03.mul", "facet04.mul", "facet05.mul", "fonts.mul", "gump.def", "gumpart.mul", "gumpidx.mul",
			"gumpartlegacymul.uop", "hues.mul", "light.mul", "lightidx.mul", "map0.mul", "map1.mul", "map2.mul", "map3.mul",
			"map4.mul", "map5.mul", "map0legacymul.uop", "map1legacymul.uop", "map2legacymul.uop", "map3legacymul.uop",
			"map4legacymul.uop", "map5legacymul.uop", "mapdif0.mul", "mapdif1.mul", "mapdif2.mul", "mapdif3.mul", "mapdif4.mul",
			"mapdifl0.mul", "mapdifl1.mul", "mapdifl2.mul", "mapdifl3.mul", "mapdifl4.mul", "mobtypes.txt", "multi.idx",
			"multi.mul", "multimap.rle", "radarcol.mul", "skillgrp.mul", "skills.idx", "skills.mul", "sound.def", "sound.mul",
			"soundidx.mul", "soundlegacymul.uop", "speech.mul", "stadif0.mul", "stadif1.mul", "stadif2.mul", "stadif3.mul",
			"stadif4.mul", "stadifi0.mul", "stadifi1.mul", "stadifi2.mul", "stadifi3.mul", "stadifi4.mul", "stadifl0.mul",
			"stadifl1.mul", "stadifl2.mul", "stadifl3.mul", "stadifl4.mul", "staidx0.mul", "staidx1.mul", "staidx2.mul",
			"staidx3.mul", "staidx4.mul", "staidx5.mul", "statics0.mul", "statics1.mul", "statics2.mul", "statics3.mul",
			"statics4.mul", "statics5.mul", "texidx.mul", "texmaps.mul", "tiledata.mul", "unifont.mul", "unifont1.mul",
			"unifont2.mul", "unifont3.mul", "unifont4.mul", "unifont5.mul", "unifont6.mul", "unifont7.mul", "unifont8.mul",
			"unifont9.mul", "unifont10.mul", "unifont11.mul", "unifont12.mul", "uotd.exe", "verdata.mul"
		};

		static Files()
		{
			m_Directory = LoadDirectory();
			LoadMulPath();
		}

		/// <summary>
		///     ReReads Registry Client dir
		/// </summary>
		public static void ReLoadDirectory()
		{
			m_Directory = LoadDirectory();
		}

		/// <summary>
		///     Fills <see cref="MulPath" /> with <see cref="Files.Directory" />
		/// </summary>
		public static void LoadMulPath()
		{
			m_MulPath = new Dictionary<string, string>();
			m_RootDir = Directory;
			if (m_RootDir == null)
			{
				m_RootDir = "";
			}
			foreach (string file in m_Files)
			{
				string filePath = Path.Combine(m_RootDir, file);
				if (File.Exists(filePath))
				{
					m_MulPath[file] = file;
				}
				else
				{
					m_MulPath[file] = "";
				}
			}
		}

		/// <summary>
		///     ReSets <see cref="MulPath" /> with given path
		/// </summary>
		/// <param name="path"></param>
		public static void SetMulPath(string path)
		{
			m_RootDir = path;
			foreach (string file in m_Files)
			{
				string filePath;
				if (!String.IsNullOrEmpty(m_MulPath[file])) //file was set
				{
					if (String.IsNullOrEmpty(Path.GetDirectoryName(m_MulPath[file]))) //and relative
					{
						filePath = Path.Combine(m_RootDir, m_MulPath[file]);
						if (File.Exists(filePath)) // exists in new Root?
						{
							m_MulPath[file] = filePath;
							continue;
						}
					}
					else // absolut dir ignore
					{
						continue;
					}
				}
				filePath = Path.Combine(m_RootDir, file); //file was not set, or relative and non existent
				if (File.Exists(filePath))
				{
					m_MulPath[file] = file;
				}
				else
				{
					m_MulPath[file] = "";
				}
			}
		}

		/// <summary>
		///     Sets <see cref="MulPath" /> key to path
		/// </summary>
		/// <param name="path"></param>
		/// <param name="key"></param>
		public static void SetMulPath(string path, string key)
		{
			MulPath[key] = path;
		}

		/// <summary>
		///     Looks up a given <paramref name="file" /> in <see cref="Files.MulPath" />
		/// </summary>
		/// <returns>
		///     The absolute path to <paramref name="file" /> -or- <c>null</c> if <paramref name="file" /> was not found.
		/// </returns>
		public static string GetFilePath(string file)
		{
			if (MulPath.Count > 0)
			{
				string path = "";
				if (MulPath.ContainsKey(file.ToLower()))
				{
					path = MulPath[file.ToLower()];
				}
				if (String.IsNullOrEmpty(path))
				{
					return null;
				}
				if (String.IsNullOrEmpty(Path.GetDirectoryName(path)))
				{
					path = Path.Combine(m_RootDir, path);
				}
				if (File.Exists(path))
				{
					return path;
				}
			}

			return null;
		}

		internal static string GetFilePath(string format, params object[] args)
		{
			return GetFilePath(String.Format(format, args));
		}

		private static readonly string[] knownRegkeys = new[]
		{
			@"Electronic Arts\EA Games\Ultima Online Classic", @"Electronic Arts\EA Games\Ultima Online Stygian Abyss Classic",
			@"Origin Worlds Online\Ultima Online\KR Legacy Beta", @"Origin Worlds Online\Ultima Online Samurai Empire\3d\1.0",
			@"Origin Worlds Online\Ultima Online Samurai Empire\2d\1.0",
			@"Origin Worlds Online\Ultima Online Samurai Empire BETA\3d\1.0",
			@"Origin Worlds Online\Ultima Online Samurai Empire BETA\2d\1.0",
			@"EA Games\Ultima Online: Mondain's Legacy\1.0", @"EA Games\Ultima Online: Mondain's Legacy\1.00.0000",
			@"EA GAMES\Ultima Online: Samurai Empire\1.00.0000", @"EA Games\Ultima Online: Mondain's Legacy",
			@"EA GAMES\Ultima Online Samurai Empire\1.00.0000", @"EA GAMES\Ultima Online: Samurai Empire\1.0",
			@"EA GAMES\Ultima Online Samurai Empire", @"EA GAMES\Ultima Online Samurai Empire\1.0",
			@"Origin Worlds Online\Ultima Online\1.0", @"Origin Worlds Online\Ultima Online Third Dawn\1.0",
		};

		private static readonly string[] knownRegPathkeys = new[] {"ExePath", "Install Dir", "InstallDir"};

		public static string LoadDirectory()
		{
			string dir = null;
			for (int i = knownRegkeys.Length - 1; i >= 0; i--)
			{
				string exePath;

				if (Environment.Is64BitOperatingSystem)
				{
					exePath = GetPath(string.Format(@"Wow6432Node\{0}", knownRegkeys[i]));
				}
				else
				{
					exePath = GetPath(knownRegkeys[i]);
				}

				if (exePath != null)
				{
					dir = exePath;
					break;
				}
			}
			return dir;
		}

		private static string GetPath(string regkey)
		{
			try
			{
				RegistryKey key = Registry.LocalMachine.OpenSubKey(string.Format(@"SOFTWARE\{0}", regkey));

				if (key == null)
				{
					key = Registry.CurrentUser.OpenSubKey(string.Format(@"SOFTWARE\{0}", regkey));

					if (key == null)
					{
						return null;
					}
				}

				string path = null;
				foreach (string pathkey in knownRegPathkeys)
				{
					path = key.GetValue(pathkey) as string;

					if ((path == null) || (path.Length <= 0))
					{
						continue;
					}

					if (pathkey == "InstallDir")
					{
						path = path + @"\";
					}

					if (!System.IO.Directory.Exists(path) && !File.Exists(path))
					{
						continue;
					}

					break;
				}

				if (path == null)
				{
					return null;
				}

				if (!System.IO.Directory.Exists(path))
				{
					path = Path.GetDirectoryName(path);
				}

				if ((path == null) || (!System.IO.Directory.Exists(path)))
				{
					return null;
				}

				return path;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		///     Compares given MD5 hash with hash of given file
		/// </summary>
		/// <param name="file"></param>
		/// <param name="hash"></param>
		/// <returns></returns>
		public static bool CompareMD5(string file, string hash)
		{
			if (file == null)
			{
				return false;
			}
			FileStream FileCheck = File.OpenRead(file);
			using (MD5 md5 = new MD5CryptoServiceProvider())
			{
				byte[] md5Hash = md5.ComputeHash(FileCheck);
				FileCheck.Close();
				string md5string = BitConverter.ToString(md5Hash).Replace("-", "").ToLower();
				if (md5string == hash)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		///     Returns MD5 hash from given file
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public static byte[] GetMD5(string file)
		{
			if (file == null)
			{
				return null;
			}
			FileStream FileCheck = File.OpenRead(file);
			using (MD5 md5 = new MD5CryptoServiceProvider())
			{
				byte[] md5Hash = md5.ComputeHash(FileCheck);
				FileCheck.Close();
				return md5Hash;
			}
		}

		/// <summary>
		///     Compares MD5 hash from given mul file with hash in responsible hash-file
		/// </summary>
		/// <param name="what"></param>
		/// <returns></returns>
		public static bool CompareHashFile(string what, string path)
		{
			string FileName = Path.Combine(path, String.Format("UOFiddler{0}.hash", what));
			if (File.Exists(FileName))
			{
				try
				{
					using (var bin = new BinaryReader(new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read)))
					{
						int length = bin.ReadInt32();
						var buffer = new byte[length];
						bin.Read(buffer, 0, length);
						string hashold = BitConverter.ToString(buffer).Replace("-", "").ToLower();
						return CompareMD5(GetFilePath(String.Format("{0}.mul", what)), hashold);
					}
				}
				catch
				{
					return false;
				}
			}
			return false;
		}

		/// <summary>
		///     Checks if map1.mul exists and sets <see cref="Ultima.Map" />
		/// </summary>
		public static void CheckForNewMapSize()
		{
			if (GetFilePath("map1.mul") != null)
			{
				if (Map.Trammel.Width == 7168)
				{
					Map.Trammel = new Map(1, 1, 7168, 4096);
				}
				else
				{
					Map.Trammel = new Map(1, 1, 6144, 4096);
				}
			}
			else
			{
				if (Map.Trammel.Width == 7168)
				{
					Map.Trammel = new Map(0, 1, 7168, 4096);
				}
				else
				{
					Map.Trammel = new Map(0, 1, 6144, 4096);
				}
			}
		}
	}
}
