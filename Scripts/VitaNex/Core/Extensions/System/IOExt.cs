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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VitaNex;
using VitaNex.IO;
#endregion

namespace System.IO
{
	public static class IOExtUtility
	{
		public static FileMime GetMimeType(this FileInfo file)
		{
			file.Refresh();

			return FileMime.Lookup(file);
		}

		public static byte[] ReadAllBytes(this FileInfo file)
		{
			file.Refresh();

			return File.ReadAllBytes(file.FullName);
		}

		public static string[] ReadAllLines(this FileInfo file)
		{
			file.Refresh();

			return File.ReadAllLines(file.FullName);
		}

		public static string ReadAllText(this FileInfo file)
		{
			file.Refresh();

			return File.ReadAllText(file.FullName);
		}

		public static void WriteAllBytes(this FileInfo file, byte[] bytes)
		{
			File.WriteAllBytes(file.FullName, bytes);

			file.Refresh();
		}

		public static void WriteAllLines(this FileInfo file, string[] contents)
		{
			File.WriteAllLines(file.FullName, contents);

			file.Refresh();
		}

		public static void WriteAllLines(this FileInfo file, string[] contents, Encoding encoding)
		{
			File.WriteAllLines(file.FullName, contents, encoding);

			file.Refresh();
		}

		public static void WriteAllLines(this FileInfo file, IEnumerable<string> contents)
		{
			File.WriteAllLines(file.FullName, contents);

			file.Refresh();
		}

		public static void WriteAllLines(this FileInfo file, IEnumerable<string> contents, Encoding encoding)
		{
			File.WriteAllLines(file.FullName, contents, encoding);

			file.Refresh();
		}

		public static void WriteAllText(this FileInfo file, string contents)
		{
			File.WriteAllText(file.FullName, contents);

			file.Refresh();
		}

		public static void WriteAllText(this FileInfo file, string contents, Encoding encoding)
		{
			File.WriteAllText(file.FullName, contents, encoding);

			file.Refresh();
		}

		public static bool GetAttribute(this FileInfo file, FileAttributes attr)
		{
			file.Refresh();

			if (file.Exists)
			{
				return file.Attributes.HasFlag(attr);
			}

			return false;
		}

		public static void SetAttribute(this FileInfo file, FileAttributes attr, bool value)
		{
			file.Refresh();

			if (!file.Exists)
			{
				return;
			}

			if (value)
			{
				file.Attributes |= attr;
			}
			else
			{
				file.Attributes &= ~attr;
			}
		}

		public static void SetHidden(this FileInfo file, bool value)
		{
			file.Refresh();

			SetAttribute(file, FileAttributes.Hidden, value);
		}

		public static FileStream OpenRead(this FileInfo file, bool create = false, bool replace = false)
		{
			file.Refresh();

			if (file.Exists)
			{
				if (replace)
				{
					file = EnsureFile(file, true);
				}
			}
			else if (create)
			{
				file = EnsureFile(file, replace);
			}

			return file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		public static FileStream OpenWrite(this FileInfo file, bool create = false, bool replace = false)
		{
			file.Refresh();

			if (file.Exists)
			{
				if (replace)
				{
					file = EnsureFile(file, true);
				}
			}
			else if (create)
			{
				file = EnsureFile(file, replace);
			}

			return file.Open(FileMode.Open, FileAccess.Write, FileShare.Write);
		}

		public static FileStream OpenAppend(this FileInfo file, bool create = false, bool replace = false)
		{
			file.Refresh();

			if (file.Exists)
			{
				if (replace)
				{
					file = EnsureFile(file, true);
				}
			}
			else if (create)
			{
				file = EnsureFile(file, replace);
			}

			return file.Open(FileMode.Append, FileAccess.Write, FileShare.Write);
		}

		public static FileStream Open(this FileInfo file, bool create = false, bool replace = false)
		{
			file.Refresh();

			if (file.Exists)
			{
				if (replace)
				{
					file = EnsureFile(file, true);
				}
			}
			else if (create)
			{
				file = EnsureFile(file, replace);
			}

			return file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
		}

		public static void AppendText(this FileInfo file, bool truncate, params string[] lines)
		{
			if (lines == null || lines.Length == 0)
			{
				return;
			}

			file.Refresh();

			if (!file.Exists)
			{
				file = EnsureFile(file, false);
			}
			else if (truncate)
			{
				file = EnsureFile(file, true);
			}

			using (var fs = OpenAppend(file))
			{
				var data = String.Join(Environment.NewLine, lines) + Environment.NewLine;
				var buffer = Encoding.UTF8.GetBytes(data);

				fs.Write(buffer, 0, buffer.Length);
				fs.Flush();
			}

			file.Refresh();
		}

		/// <summary>
		///     Ensures a files' existence
		/// </summary>
		/// <returns>FileInfo representing the file ensured for 'info'</returns>
		public static FileInfo EnsureFile(this FileInfo file)
		{
			return EnsureFile(file, false);
		}

		/// <summary>
		///     Ensures a files' existence
		/// </summary>
		/// <param name="file"></param>
		/// <param name="replace">True: replace the file if it exists</param>
		/// <returns>FileInfo representing the file ensured for 'info'</returns>
		public static FileInfo EnsureFile(this FileInfo file, bool replace)
		{
			file.Refresh();

			EnsureDirectory(file.Directory, false);

			if (!file.Exists)
			{
				using (var fs = file.Create())
				{
					fs.Close();
				}
			}
			else if (replace)
			{
				VitaNexCore.TryCatch(file.Delete);

				using (var fs = file.Create())
				{
					fs.Close();
				}
			}

			file.Refresh();

			return file;
		}

		/// <summary>
		///     Ensures a directories' existence
		/// </summary>
		/// <returns>DirectoryInfo representing the directory ensured for 'info'</returns>
		public static DirectoryInfo EnsureDirectory(this DirectoryInfo dir)
		{
			return EnsureDirectory(dir, false);
		}

		/// <summary>
		///     Ensures a directories' existence
		/// </summary>
		/// <param name="dir"></param>
		/// <param name="replace">True: replace the directory if it exists</param>
		/// <returns>DirectoryInfo representing the directory ensured for 'info'</returns>
		public static DirectoryInfo EnsureDirectory(this DirectoryInfo dir, bool replace)
		{
			dir.Refresh();

			if (!dir.Exists)
			{
				dir.Create();
			}
			else if (replace)
			{
				EmptyDirectory(dir, true);

				VitaNexCore.TryCatch(dir.Delete, true);

				dir.Create();
			}

			dir.Refresh();

			return dir;
		}

		/// <summary>
		///     Empties the contents of the specified directory with the option to include sub directories
		/// </summary>
		/// <param name="dir">Directory to empty</param>
		/// <param name="incDirs">True: includes sub directories</param>
		public static void EmptyDirectory(this DirectoryInfo dir, bool incDirs)
		{
			dir.Refresh();

			if (!dir.Exists)
			{
				return;
			}

			foreach (var f in dir.EnumerateFiles())
			{
				VitaNexCore.TryCatch(f.Delete);
			}

			if (incDirs)
			{
				foreach (var d in dir.EnumerateDirectories())
				{
					VitaNexCore.TryCatch(d.Delete, true);
				}
			}

			dir.Refresh();
		}

		/// <summary>
		///     Empties the contents of the specified directory, including all sub-directories and files that are older than the
		///     given age.
		/// </summary>
		/// <param name="dir">Directory to empty</param>
		/// <param name="age">Age at which a directory or file is considered old enough to be deleted</param>
		public static void EmptyDirectory(this DirectoryInfo dir, TimeSpan age)
		{
			EmptyDirectory(dir, age, "*", SearchOption.AllDirectories);
		}

		/// <summary>
		///     Empties the contents of the specified directory, only deleting files that meet the mask criteria and are older than
		///     the given age.
		/// </summary>
		/// <param name="dir">Directory to empty</param>
		/// <param name="age">Age at which a directory or file is considered old enough to be deleted</param>
		/// <param name="mask">String mask to use to filter file names</param>
		/// <param name="option">Search options</param>
		public static void EmptyDirectory(this DirectoryInfo dir, TimeSpan age, string mask, SearchOption option)
		{
			dir.Refresh();

			if (!dir.Exists)
			{
				return;
			}

			var expire = DateTime.UtcNow.Subtract(age);

			foreach (var d in AllDirectories(dir, mask, option).Where(d => d.CreationTimeUtc < expire))
			{
				VitaNexCore.TryCatch(d.Delete, true);
			}

			foreach (var f in AllFiles(dir, mask, option).Where(f => f.CreationTimeUtc < expire))
			{
				VitaNexCore.TryCatch(f.Delete);
			}

			dir.Refresh();
		}

		/// <summary>
		///     Empties the contents of the specified directory, only deleting files that meet the mask criteria
		/// </summary>
		/// <param name="dir">Directory to empty</param>
		/// <param name="mask">String mask to use to filter file names</param>
		/// <param name="option">Search options</param>
		public static void EmptyDirectory(this DirectoryInfo dir, string mask, SearchOption option)
		{
			dir.Refresh();

			if (!dir.Exists)
			{
				return;
			}

			foreach (var d in AllDirectories(dir, mask, option))
			{
				VitaNexCore.TryCatch(d.Delete, true);
			}

			foreach (var f in AllFiles(dir, mask, option))
			{
				VitaNexCore.TryCatch(f.Delete);
			}

			dir.Refresh();
		}

		/// <summary>
		///     Copies the contents of the specified directory to the specified target directory, only including files that meet
		///     the mask criteria
		/// </summary>
		/// <param name="source">Directory to copy</param>
		/// <param name="dest">Directory to copy to</param>
		public static void CopyDirectory(this DirectoryInfo source, DirectoryInfo dest)
		{
			CopyDirectory(source, dest, "*", SearchOption.AllDirectories);
		}

		/// <summary>
		///     Copies the contents of the specified directory to the specified target directory, only including files that meet
		///     the mask criteria
		/// </summary>
		/// <param name="source">Directory to copy</param>
		/// <param name="dest">Directory to copy to</param>
		/// <param name="mask">String mask to use to filter file names</param>
		/// <param name="option">Search options</param>
		public static void CopyDirectory(this DirectoryInfo source, DirectoryInfo dest, string mask, SearchOption option)
		{
			source.Refresh();

			if (!source.Exists)
			{
				return;
			}

			EnsureDirectory(dest, false);

			foreach (var f in AllFiles(source, mask, option))
			{
				VitaNexCore.TryCatch(
					() =>
					{
						var t = new FileInfo(f.FullName.Replace(source.FullName, dest.FullName));

						EnsureDirectory(t.Directory);

						f.CopyTo(t.FullName, true);
					});
			}

			source.Refresh();
			dest.Refresh();
		}

		public static IEnumerable<DirectoryInfo> AllDirectories(this DirectoryInfo dir, string mask, SearchOption option)
		{
			foreach (var d in dir.EnumerateDirectories(mask).Where(d => d != dir))
			{
				if (option == SearchOption.AllDirectories)
				{
					foreach (var s in AllDirectories(d, mask, SearchOption.AllDirectories).Where(s => s != d))
					{
						yield return s;
					}
				}

				yield return d;
			}
		}

		public static IEnumerable<FileInfo> AllFiles(this DirectoryInfo dir, string mask, SearchOption option)
		{
			if (option == SearchOption.AllDirectories)
			{
				foreach (var f in dir.EnumerateDirectories()
									 .Where(d => d != dir)
									 .SelectMany(d => AllFiles(d, mask, SearchOption.AllDirectories)))
				{
					yield return f;
				}
			}

			foreach (var f in dir.EnumerateFiles(mask))
			{
				yield return f;
			}
		}
	}
}