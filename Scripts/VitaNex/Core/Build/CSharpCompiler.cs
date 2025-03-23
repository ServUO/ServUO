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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.CSharp;

using Server;

using VitaNex.IO;
#endregion

#if !MUO

namespace VitaNex.Build
{
	public enum CompileStatus
	{
		Initializing,
		Compiling,
		Completed,
		Aborted
	}

	public class CSharpCompiler
	{
		public static readonly string[] DefaultReferences =
		{
			"System.dll", "System.Core.dll", "System.Data.dll", "System.Drawing.dll", "System.Web.dll",
			"System.Windows.Forms.dll", "System.Xml.dll"
		};

		private static volatile string _DefaultInputPath = IOUtility.GetSafeDirectoryPath(Core.BaseDirectory + "/Scripts");

		private static volatile string _DefaultOutputPath =
			IOUtility.GetSafeDirectoryPath(Core.BaseDirectory + "/Scripts/Output");

		public static string DefaultInputPath
		{
			get => _DefaultInputPath;
			set => _DefaultInputPath = IOUtility.GetSafeDirectoryPath(value);
		}

		public static string DefaultOutputPath
		{
			get => _DefaultOutputPath;
			set => _DefaultOutputPath = IOUtility.GetSafeDirectoryPath(value);
		}

		public bool Debug { get; set; }

		public List<string> References { get; private set; }
		public List<string> FileMasks { get; private set; }
		public List<string> Arguments { get; private set; }

		public string[] Errors { get; private set; }

		public CompileStatus Status { get; private set; }
		public string StatusMessage { get; private set; }

		public CSharpCodeProvider Provider { get; private set; }
		public CompilerParameters Parameters { get; private set; }
		public CompilerResults Results { get; private set; }

		public DirectoryInfo InputDirectory { get; set; }
		public DirectoryInfo OutputDirectory { get; set; }
		public string OutputFileName { get; set; }

		public Action<CompilerResults> CompiledCallback { get; set; }

		public CSharpCompiler(
			DirectoryInfo input,
			DirectoryInfo output,
			string outputFileName,
			Action<CompilerResults> onCompiled = null)
		{
			References = new List<string>();
			FileMasks = new List<string>();
			Arguments = new List<string>();
			Errors = new string[0];

			Status = CompileStatus.Initializing;
			StatusMessage = String.Empty;

			Provider = new CSharpCodeProvider();

			InputDirectory = input;
			OutputDirectory = output.EnsureDirectory(false);
			OutputFileName = outputFileName;
			CompiledCallback = onCompiled;
		}

		public CSharpCompiler(
			string inputPath,
			string outputPath,
			string outputFileName,
			Action<CompilerResults> onCompiled = null)
			: this(
				new DirectoryInfo(IOUtility.GetSafeDirectoryPath(inputPath)),
				IOUtility.EnsureDirectory(outputPath, true),
				outputFileName,
				onCompiled)
		{ }

		public CSharpCompiler(string outputFileName, Action<CompilerResults> onCompiled = null)
			: this(new DirectoryInfo(_DefaultInputPath), new DirectoryInfo(_DefaultOutputPath), outputFileName, onCompiled)
		{ }

		public void AddReference(string name)
		{
			if (!References.Contains(name))
			{
				References.Add(name);
			}
		}

		public void AddFileMask(string mask)
		{
			if (!FileMasks.Contains(mask))
			{
				FileMasks.Add(mask);
			}
		}

		public void Compile(bool async = false)
		{
			if (Status == CompileStatus.Compiling)
			{
				return;
			}

			var pct = new Thread(Init)
			{
				Name = "VNCSharpCompiler"
			};
			pct.Start();

			if (async)
			{
				return;
			}

			VitaNexCore.WaitWhile(
				() => Status == CompileStatus.Initializing || Status == CompileStatus.Compiling,
				TimeSpan.FromMinutes(5.0));

			pct.Join();
		}

		[STAThread]
		private void Init()
		{
			VitaNexCore.TryCatch(
				() =>
				{
					Status = CompileStatus.Initializing;

					if (!InputDirectory.Exists)
					{
						Status = CompileStatus.Aborted;
						StatusMessage = "Input directory '" + InputDirectory + "' does not exist.";
						return;
					}

					var infos = new List<FileInfo>();

					foreach (var file in FileMasks.SelectMany(
						t => InputDirectory.GetFiles(t, SearchOption.AllDirectories).Where(file => !infos.Contains(file))))
					{
						infos.Add(file);
					}

					var files = infos.ToArray();
					infos.Clear();

					if (files.Length == 0)
					{
						Status = CompileStatus.Aborted;
						StatusMessage = "No files to compile.";
						return;
					}

					var refs = new List<string>();
					var fileNames = new List<string>();

					foreach (var fName in files.Select(t => t.FullName)
											   .Where(fName => !String.IsNullOrEmpty(fName))
											   .Where(fName => !fileNames.Contains(fName)))
					{
						fileNames.Add(fName);
					}

					foreach (var t in DefaultReferences.Where(t => !String.IsNullOrEmpty(t)).Where(t => !refs.Contains(t)))
					{
						refs.Add(t);
					}

					foreach (var t in References.Where(t => !String.IsNullOrEmpty(t)).Where(t => !refs.Contains(t)))
					{
						refs.Add(t);
					}

					var configs = GetConfigFiles();

					if (configs != null)
					{
						foreach (var t in configs.Select(GetConfigAssemblies)
												 .SelectMany(
													 asm => asm.Where(t => !String.IsNullOrEmpty(t))
															   .Where(t => File.Exists(IOUtility.GetSafeFilePath(IOUtility.GetBaseDirectory() + "/" + t, true)))
															   .Where(t => !refs.Contains(t))))
						{
							refs.Add(t);
						}
					}

					Status = CompileStatus.Compiling;
					Parameters = new CompilerParameters(
						refs.ToArray(),
						IOUtility.GetUnusedFilePath(OutputDirectory.FullName, OutputFileName),
						Debug)
					{
						GenerateExecutable = false,
						WarningLevel = 4,
						CompilerOptions = String.Empty
					};

					foreach (var arg in Arguments)
					{
						Parameters.CompilerOptions += arg + " ";
					}

					Results = Provider.CompileAssemblyFromFile(Parameters, fileNames.ToArray());

					if (Results.Errors.Count > 0)
					{
						int errorCount = 0, warningCount = 0;

						foreach (CompilerError e in Results.Errors)
						{
							if (e.IsWarning)
							{
								++warningCount;
							}
							else
							{
								++errorCount;
							}
						}

						Errors = new string[Results.Errors.Count];

						for (var e = 0; e < Results.Errors.Count; e++)
						{
							Errors[e] = String.Format(
								"[{0}][{1}][{2}]: Line {3}, Column {4}\n{5}",
								Results.Errors[e].IsWarning ? "Warning" : "Error",
								Results.Errors[e].FileName,
								Results.Errors[e].ErrorNumber,
								Results.Errors[e].Line,
								Results.Errors[e].Column,
								Results.Errors[e].ErrorText);
						}

						StatusMessage = String.Format(
							"Finished compiling with {0} error{1} and {2} warning{3}",
							errorCount,
							errorCount > 1 ? "s" : "",
							warningCount,
							warningCount > 1 ? "s" : "");

						Status = CompileStatus.Completed;
					}
					else
					{
						StatusMessage = "Finished compiling with no errors or warnings.";
						Status = CompileStatus.Completed;
					}
				},
				ex =>
				{
					Status = CompileStatus.Aborted;
					StatusMessage = ex.Message;
				});

			if (CompiledCallback != null)
			{
				CompiledCallback(Results);
			}
		}

		public FileInfo[] GetConfigFiles()
		{
			var configs = InputDirectory.GetFiles("*.cfg", SearchOption.AllDirectories);

			if (configs.Length > 0)
			{
				return configs;
			}

			return null;
		}

		private string[] GetConfigAssemblies(FileInfo file)
		{
			var list = new List<string>();

			if (file.Exists)
			{
				var lines = File.ReadAllLines(file.FullName, Encoding.Default);
				var content = String.Empty;
				var inTag = false;

				for (var i = 0; i < lines.Length; i++)
				{
					if (lines[i].StartsWith("[VNC]"))
					{
						inTag = true;
						lines[i] = lines[i].Replace("[VNC]", String.Empty);
					}
					else if (lines[i].StartsWith("["))
					{
						inTag = false;
						lines[i] = String.Empty;
					}

					if (inTag)
					{
						if (!String.IsNullOrEmpty(lines[i]))
						{
							content += lines[i].Trim();
						}
					}
				}

				var split = content.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

				list.AddRange(split.Where(assembly => !String.IsNullOrEmpty(assembly) && !assembly.StartsWith("#")));
			}

			return list.ToArray();
		}
	}
}

#endif