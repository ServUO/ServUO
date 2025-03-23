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

#if MUO
global using GenericWriter = Server.IGenericWriter;
global using GenericReader = Server.IGenericReader;
global using ScriptCompiler = Server.AssemblyHandler;
#endif

#region References
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#if MUO
using System.Diagnostics.CodeAnalysis;
#endif

using Server;

using VitaNex.IO;
#endregion

namespace VitaNex
{
	public static partial class VitaNexCore
	{
		private static readonly Queue<Tuple<string, string>> _INITQueue;
		private static readonly Dictionary<string, Action<string>> _INITHandlers;

		static VitaNexCore()
		{
			_INITQueue = new Queue<Tuple<string, string>>();
			_INITHandlers = new Dictionary<string, Action<string>>();

			var basePath = IOUtility.GetSafeDirectoryPath(Core.BaseDirectory + "/VitaNexCore");

			if (!Directory.Exists(basePath))
			{
				FirstBoot = true;
			}

			BaseDirectory = IOUtility.EnsureDirectory(basePath);

			var first = IOUtility.GetSafeFilePath(BaseDirectory + "/FirstBoot.vnc", true);

			if (!File.Exists(first))
			{
				FirstBoot = true;

				IOUtility.EnsureFile(first)
						 .AppendText(
							 true,
							 "This file serves no other purpose than to identify if",
							 "the software has been initialized for the first time. ",
							 "To re-initialize 'First-Boot' mode, simply delete this",
							 "file before starting the application.");
			}

			var root = FindRootDirectory(Core.BaseDirectory + "/Scripts/VitaNex");

			if (root != null && root.Exists)
			{
				RootDirectory = root;

				ParseINIT();

				RegisterINITHandler(
					"ROOT_DIR",
					path =>
					{
						root = FindRootDirectory(path);

						if (root != null && root.Exists)
						{
							RootDirectory = root;
						}
					});
			}

			BackupExpireAge = TimeSpan.FromDays(7);

			RegisterINITHandler(
				"BACKUP_EXPIRE",
				time =>
				{
					if (TimeSpan.TryParse(time, out var ts))
					{
						BackupExpireAge = ts;
					}
				});

#if MUO
			Core.LoopContext.Post(Slice);
#else
            Core.Slice += Slice;
#endif
		}

		private static void Slice()
		{
			_Tick = Ticks;

#if MUO
            Core.LoopContext.Post(Slice);
#endif
		}

		private static DirectoryInfo FindRootDirectory(string path)
		{
			if (String.IsNullOrWhiteSpace(path))
			{
				return null;
			}

			path = IOUtility.GetSafeDirectoryPath(path);

			var root = TryCatchGet(
				() =>
				{
					var dir = new DirectoryInfo(path);

					while (!dir.Exists && dir.Parent != null)
					{
						dir = dir.Parent;
					}

					return dir;
				},
				ToConsole);

			if (root == null || !root.Exists)
			{
				return null;
			}

			var files = root.GetFiles("VitaNex*.cs", SearchOption.AllDirectories);

			if (files.Length < 4)
			{
				return null;
			}

			if (!files.All(f => PathEquals(f.Directory, root)))
			{
				var file = files.FirstOrDefault(f => f.Directory != null && f.Directory.Exists);

				root = file != null && files.All(f => PathEquals(f.Directory, file.Directory)) ? file.Directory : null;
			}

			if (root == null)
			{
				return root;
			}

#if !MONO
			// Convert absolute path to relative path

			var corePath = IOUtility.GetSafeDirectoryPath(Core.BaseDirectory);
			var rootPath = IOUtility.GetSafeDirectoryPath(root.FullName.Replace(corePath, String.Empty));

			root = new DirectoryInfo(rootPath);
#endif

			return root;
		}

		private static bool PathEquals(DirectoryInfo l, DirectoryInfo r)
		{
			return l != null && r != null && l.FullName == r.FullName;
		}

		private static void ParseINIT()
		{
			var files = RootDirectory.GetFiles("VNC.cfg", SearchOption.AllDirectories);

			bool parse;
			var die = false;

			foreach (var file in files.Select(f => new ConfigFileInfo(f)))
			{
				parse = false;

				var lines = file.ReadAllLines();

				foreach (var line in lines)
				{
					if (!parse && line.StartsWith("[VNC_INIT]"))
					{
						parse = true;
						die = true;
					}

					if (parse && line.StartsWith("[VNC_EXIT]"))
					{
						parse = false;
					}

					if (!parse || String.IsNullOrWhiteSpace(line))
					{
						return;
					}

					var split = line.Split('=');

					var key = (split[0] ?? String.Empty).ToUpper();
					var value = String.Join(String.Empty, split, 1, split.Length - 1);

					if (!String.IsNullOrWhiteSpace(key))
					{
						_INITQueue.Enqueue(Tuple.Create(key, value));
					}
				}

				if (die)
				{
					break;
				}
			}
		}

		private static void ProcessINIT()
		{
			while (_INITQueue.Count > 0)
			{
				var instr = _INITQueue.Dequeue();

				if (_INITHandlers.ContainsKey(instr.Item1) && _INITHandlers[instr.Item1] != null)
				{
					_INITHandlers[instr.Item1](instr.Item2);
				}
			}
		}

		public static void RegisterINITHandler(string key, Action<string> callback)
		{
			if (String.IsNullOrWhiteSpace(key))
			{
				return;
			}

			key = key.ToUpper();

			if (_INITHandlers.ContainsKey(key))
			{
				if (callback != null)
				{
					_INITHandlers[key] = callback;
				}
				else
				{
					_INITHandlers.Remove(key);
				}
			}
			else if (callback != null)
			{
				_INITHandlers.Add(key, callback);
			}
		}
	}
}
