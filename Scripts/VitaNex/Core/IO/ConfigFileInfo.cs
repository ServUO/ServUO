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
using System.Collections.Generic;
using System.IO;
using System.Linq;
#endregion

namespace VitaNex.IO
{
	public sealed class ConfigFileInfo
	{
		private static readonly string[] _CommentSymbols = { "##", "//" };
		private static readonly string[][] _MassCommentSymbols = { new[] { "/#", "#/" }, new[] { "/*", "*/" } };

		private static FileInfo GetFileInfo(string path)
		{
			path = path ?? String.Empty;

			return new FileInfo(IOUtility.GetSafeFilePath(path, true));
		}

		public FileInfo File { get; set; }

		public DirectoryInfo Directory => File != null ? File.Directory : null;

		public string Name => File != null ? File.Name : String.Empty;
		public string Extension => File != null ? File.Extension : String.Empty;

		public long Length => File != null ? File.Length : 0;

		public bool Exists => File != null && File.Exists;

		public FileAttributes Attributes
		{
			get => File != null ? File.Attributes : FileAttributes.Normal;
			set
			{
				if (File != null)
				{
					File.Attributes = value;
				}
			}
		}

		public ConfigFileInfo(string path)
			: this(GetFileInfo(path))
		{ }

		public ConfigFileInfo(FileInfo file)
		{
			File = file;
		}

		private static bool IsComment(string line, string symbol, out int idx)
		{
			idx = -1;

			if (String.IsNullOrWhiteSpace(line))
			{
				return false;
			}

			idx = line.IndexOf(symbol, StringComparison.Ordinal);

			return idx >= 0;
		}

		public string[] ReadAllLines()
		{
			var lines = new List<string>();

			if (File != null && File.Exists && File.Length > 0)
			{
				using (var stream = File.OpenText())
				{
					var idx = -1;
					var comment = false;

					while (!stream.EndOfStream)
					{
						var line = (stream.ReadLine() ?? String.Empty).Trim();

						if (_CommentSymbols.Any(
							symbol => IsComment(line, symbol, out idx) && idx > 0 && idx + symbol.Length < line.Length))
						{
							line = line.Substring(idx);
						}

						foreach (var symbols in _MassCommentSymbols)
						{
							if (!comment)
							{
								if (IsComment(line, symbols[0], out idx))
								{
									if (idx > 0 && idx + symbols[0].Length < line.Length)
									{
										line = line.Substring(0, idx);
									}

									comment = true;
								}
							}

							if (!comment || !IsComment(line, symbols[1], out var cIdx))
							{
								continue;
							}

							if (cIdx > idx && cIdx + symbols[1].Length < line.Length)
							{
								line = line.Substring(cIdx + symbols[1].Length);
							}

							comment = false;
						}

						if (!comment)
						{
							lines.Add(line);
						}
					}
				}
			}

			return lines.ToArray();
		}
	}
}