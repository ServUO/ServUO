#region References
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
#endregion

namespace Server
{
	public static class ScriptCompiler
	{
		public static Assembly[] Assemblies { get; set; }

		private static readonly List<string> m_AdditionalReferences = new List<string>();

		public static string[] GetReferenceAssemblies()
		{
			List<string> list = new List<string>();

			string path = Path.Combine(Core.BaseDirectory, "Data/Assemblies.cfg");

			if (File.Exists(path))
			{
				using (StreamReader ip = new StreamReader(path))
				{
					string line;

					while ((line = ip.ReadLine()) != null)
					{
						if (line.Length > 0 && !line.StartsWith("#"))
						{
							list.Add(line);
						}
					}
				}
			}

			list.Add(Core.ExePath);

			list.AddRange(m_AdditionalReferences);

			return list.ToArray();
		}

		public static string GetCompilerOptions(bool debug)
		{
			StringBuilder sb = null;

			AppendCompilerOption(ref sb, "/d:ServUO");

			AppendCompilerOption(ref sb, "/unsafe");

			if (!debug)
			{
				AppendCompilerOption(ref sb, "/optimize");
			}
			else
			{
				AppendCompilerOption(ref sb, "/debug");
				AppendCompilerOption(ref sb, "/d:DEBUG");
				AppendCompilerOption(ref sb, "/d:TRACE");
			}

#if MONO
			AppendCompilerOption( ref sb, "/d:MONO" );
#endif

			if (Core.Is64Bit)
			{
				AppendCompilerOption(ref sb, "/d:x64");
			}

#if NEWTIMERS
			AppendCompilerOption(ref sb, "/d:NEWTIMERS");
#endif

#if NEWPARENT
			AppendCompilerOption(ref sb, "/d:NEWPARENT");
#endif

			return (sb == null ? null : sb.ToString());
		}

		private static void AppendCompilerOption(ref StringBuilder sb, string define)
		{
			if (sb == null)
			{
				sb = new StringBuilder();
			}
			else
			{
				sb.Append(' ');
			}

			sb.Append(define);
		}

		private static byte[] GetHashCode(string compiledFile, string[] scriptFiles, bool debug)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				using (BinaryWriter bin = new BinaryWriter(ms))
				{
					FileInfo fileInfo = new FileInfo(compiledFile);

					bin.Write(fileInfo.LastWriteTimeUtc.Ticks);

					foreach (string scriptFile in scriptFiles)
					{
						fileInfo = new FileInfo(scriptFile);

						bin.Write(fileInfo.LastWriteTimeUtc.Ticks);
					}

					bin.Write(debug);
					bin.Write(Core.Version.ToString());

					ms.Position = 0;

					using (SHA1 sha1 = SHA1.Create())
					{
						return sha1.ComputeHash(ms);
					}
				}
			}
		}

		public static bool CompileCSScripts(out Assembly assembly)
		{
			return CompileCSScripts(false, true, out assembly);
		}

		public static bool CompileCSScripts(bool debug, out Assembly assembly)
		{
			return CompileCSScripts(debug, true, out assembly);
		}

		public static bool CompileCSScripts(bool debug, bool cache, out Assembly assembly)
		{
			long startTime = Core.TickCount;

			Utility.PushColor(ConsoleColor.Yellow);
			Console.Write("Scripts: Compiling C# scripts...");
			Utility.PopColor();
			string[] files = GetScripts("*.cs");

			if (files.Length == 0)
			{
				Utility.PushColor(ConsoleColor.Red);
				Console.WriteLine("no files found.");
				Utility.PopColor();
				assembly = null;
				return true;
			}

			if (File.Exists("Scripts/Output/Scripts.CS.dll"))
			{
				if (cache && File.Exists("Scripts/Output/Scripts.CS.hash"))
				{
					try
					{
						byte[] hashCode = GetHashCode("Scripts/Output/Scripts.CS.dll", files, debug);

						using (FileStream fs = new FileStream("Scripts/Output/Scripts.CS.hash", FileMode.Open, FileAccess.Read, FileShare.Read))
						{
							using (BinaryReader bin = new BinaryReader(fs))
							{
								byte[] bytes = bin.ReadBytes(hashCode.Length);

								if (bytes.Length == hashCode.Length)
								{
									bool valid = true;

									for (int i = 0; i < bytes.Length; ++i)
									{
										if (bytes[i] != hashCode[i])
										{
											valid = false;
											break;
										}
									}

									if (valid)
									{
										assembly = Assembly.LoadFrom("Scripts/Output/Scripts.CS.dll");

										if (!m_AdditionalReferences.Contains(assembly.Location))
										{
											m_AdditionalReferences.Add(assembly.Location);
										}

										Utility.PushColor(ConsoleColor.Green);
										Console.WriteLine("done (cached)");
										Utility.PopColor();

										return true;
									}
								}
							}
						}
					}
					catch (Exception e)
					{
						Server.Diagnostics.ExceptionLogging.LogException(e);
					}
				}
			}

			DeleteFiles("Scripts.CS*.dll");

#if !MONO
			using (CodeDomProvider provider = new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider())
#else
            using (CSharpCodeProvider provider = new CSharpCodeProvider())
#endif
			{
				string path = GetUnusedPath("Scripts.CS");

				CompilerParameters parms = new CompilerParameters(GetReferenceAssemblies(), path, debug);

				string options = GetCompilerOptions(debug);

				if (options != null)
				{
					parms.CompilerOptions = options;
				}

				if (Core.HaltOnWarning)
				{
					parms.WarningLevel = 4;
				}

				if (Core.Unix)
				{
					parms.CompilerOptions = string.Format("{0} /nowarn:169,219,414 /recurse:Scripts/*.cs", parms.CompilerOptions);
					files = new string[0];
				}

				CompilerResults results = provider.CompileAssemblyFromFile(parms, files);

				m_AdditionalReferences.Add(path);

				Display(results, startTime);

				if (results.Errors.Count > 0 && !Core.Unix)
				{
					assembly = null;
					return false;
				}

				if (results.Errors.Count > 0 && Core.Unix)
				{
					foreach (CompilerError err in results.Errors)
					{
						if (!err.IsWarning)
						{
							assembly = null;
							return false;
						}
					}
				}

				if (cache && Path.GetFileName(path) == "Scripts.CS.dll")
				{
					try
					{
						byte[] hashCode = GetHashCode(path, files, debug);

						using (
							FileStream fs = new FileStream("Scripts/Output/Scripts.CS.hash", FileMode.Create, FileAccess.Write, FileShare.None))
						{
							using (BinaryWriter bin = new BinaryWriter(fs))
							{
								bin.Write(hashCode, 0, hashCode.Length);
							}
						}
					}
					catch (Exception ex)
					{
						Server.Diagnostics.ExceptionLogging.LogException(ex);
					}
				}

				assembly = results.CompiledAssembly;
				return true;
			}
		}

		public static void Display(CompilerResults results, long startTime)
		{
			if (results.Errors.Count > 0)
			{
				Dictionary<string, List<CompilerError>> errors = new Dictionary<string, List<CompilerError>>(results.Errors.Count, StringComparer.OrdinalIgnoreCase);
				Dictionary<string, List<CompilerError>> warnings = new Dictionary<string, List<CompilerError>>(results.Errors.Count, StringComparer.OrdinalIgnoreCase);

				foreach (CompilerError e in results.Errors)
				{
					string file = e.FileName;

					// Ridiculous. FileName is null if the warning/error is internally generated in csc.
					if (string.IsNullOrEmpty(file))
					{
						Console.WriteLine("ScriptCompiler: {0}: {1}", e.ErrorNumber, e.ErrorText);
						continue;
					}

					Dictionary<string, List<CompilerError>> table = (e.IsWarning ? warnings : errors);

					table.TryGetValue(file, out List<CompilerError> list);

					if (list == null)
					{
						table[file] = list = new List<CompilerError>();
					}

					list.Add(e);
				}

				if (errors.Count > 0)
				{
					Utility.PushColor(ConsoleColor.Red);
					Console.WriteLine("Failed with: {0} errors, {1} warnings", errors.Count, warnings.Count);
					Utility.PopColor();
				}
				else
				{
					Utility.PushColor(ConsoleColor.Green);
					Console.WriteLine("Finished with: {0} errors, {1} warnings [{2} seconds]", errors.Count, warnings.Count, (int)((Core.TickCount - startTime) / 1000));
					Utility.PopColor();
				}

				string scriptRoot = Path.GetFullPath(Path.Combine(Core.BaseDirectory, "Scripts" + Path.DirectorySeparatorChar));
				Uri scriptRootUri = new Uri(scriptRoot);

				Utility.PushColor(ConsoleColor.Yellow);

				if (warnings.Count > 0)
				{
					Console.WriteLine("Warnings:");
				}

				foreach (KeyValuePair<string, List<CompilerError>> kvp in warnings)
				{
					string fileName = kvp.Key;
					List<CompilerError> list = kvp.Value;

					string fullPath = Path.GetFullPath(fileName);
					string usedPath = Uri.UnescapeDataString(scriptRootUri.MakeRelativeUri(new Uri(fullPath)).OriginalString);

					Console.WriteLine(" + {0}:", usedPath);

					Utility.PushColor(ConsoleColor.DarkYellow);

					foreach (CompilerError e in list)
					{
						Console.WriteLine("    {0}: Line {1}: {2}", e.ErrorNumber, e.Line, e.ErrorText);
					}

					Utility.PopColor();
				}

				Utility.PopColor();

				Utility.PushColor(ConsoleColor.Red);

				if (errors.Count > 0)
				{
					Console.WriteLine("Errors:");
				}

				foreach (KeyValuePair<string, List<CompilerError>> kvp in errors)
				{
					string fileName = kvp.Key;
					List<CompilerError> list = kvp.Value;

					string fullPath = Path.GetFullPath(fileName);
					string usedPath = Uri.UnescapeDataString(scriptRootUri.MakeRelativeUri(new Uri(fullPath)).OriginalString);

					Console.WriteLine(" + {0}:", usedPath);

					Utility.PushColor(ConsoleColor.Red);

					foreach (CompilerError e in list)
					{
						Console.WriteLine("    {0}: Line {1}: {2}", e.ErrorNumber, e.Line, e.ErrorText);
					}

					Utility.PopColor();
				}

				Utility.PopColor();
			}
			else
			{
				Utility.PushColor(ConsoleColor.Green);
				Console.WriteLine("Finished with: 0 errors, 0 warnings [{0} seconds]", (int)((Core.TickCount - startTime) / 1000));
				Utility.PopColor();
			}
		}

		public static string GetUnusedPath(string name)
		{
			string path = Path.Combine(Core.BaseDirectory, string.Format("Scripts/Output/{0}.dll", name));

			for (int i = 2; File.Exists(path) && i <= 1000; ++i)
			{
				path = Path.Combine(Core.BaseDirectory, string.Format("Scripts/Output/{0}.{1}.dll", name, i));
			}

			return path;
		}

		public static void DeleteFiles(string mask)
		{
			try
			{
				string[] files = Directory.GetFiles(Path.Combine(Core.BaseDirectory, "Scripts/Output"), mask);

				foreach (string file in files)
				{
					try
					{
						File.Delete(file);
					}
					catch (Exception ex)
					{
						Server.Diagnostics.ExceptionLogging.LogException(ex);
					}
				}
			}
			catch (Exception ex)
			{
				Server.Diagnostics.ExceptionLogging.LogException(ex);
			}
		}

		private delegate CompilerResults Compiler(bool debug);

		public static bool Compile()
		{
			return Compile(false);
		}

		public static bool Compile(bool debug)
		{
			return Compile(debug, true);
		}

		public static bool Compile(bool debug, bool cache)
		{
			EnsureDirectory("Scripts/");
			EnsureDirectory("Scripts/Output/");

			if (m_AdditionalReferences.Count > 0)
			{
				m_AdditionalReferences.Clear();
			}

			List<Assembly> assemblies = new List<Assembly>();


			if (CompileCSScripts(debug, cache, out Assembly assembly))
			{
				if (assembly != null)
				{
					assemblies.Add(assembly);
				}
			}
			else
			{
				return false;
			}

			if (assemblies.Count == 0)
			{
				return false;
			}

			Assemblies = assemblies.ToArray();

			Utility.PushColor(ConsoleColor.Yellow);
			Console.WriteLine("Scripts: Verifying...");
			Utility.PopColor();

			Stopwatch watch = Stopwatch.StartNew();

			Core.VerifySerialization();

			watch.Stop();

			Utility.PushColor(ConsoleColor.Green);
			Console.WriteLine(
				"Finished ({0} items, {1} mobiles, {2} customs) ({3:F2} seconds)",
				Core.ScriptItems,
				Core.ScriptMobiles,
				Core.ScriptCustoms,
				watch.Elapsed.TotalSeconds);
			Utility.PopColor();

			return true;
		}

		public static void Invoke(string method)
		{
			List<MethodInfo> invoke = new List<MethodInfo>();

			foreach (Assembly a in Assemblies)
			{
				Type[] types = a.GetTypes();

				foreach (Type t in types)
				{
					MethodInfo m = t.GetMethod(method, BindingFlags.Static | BindingFlags.Public);

					if (m != null)
					{
						invoke.Add(m);
					}
				}
			}

			invoke.Sort(new CallPriorityComparer());

			foreach (MethodInfo m in invoke)
			{
				m.Invoke(null, null);
			}
		}

		private static readonly Dictionary<Assembly, TypeCache> m_TypeCaches = new Dictionary<Assembly, TypeCache>();
		private static TypeCache m_NullCache;

		public static TypeCache GetTypeCache(Assembly asm)
		{
			if (asm == null)
			{
				return m_NullCache ?? (m_NullCache = new TypeCache(null));
			}


			m_TypeCaches.TryGetValue(asm, out TypeCache c);

			if (c == null)
			{
				m_TypeCaches[asm] = c = new TypeCache(asm);
			}

			return c;
		}

		public static Type FindTypeByFullName(string fullName)
		{
			return FindTypeByFullName(fullName, true);
		}

		public static Type FindTypeByFullName(string fullName, bool ignoreCase)
		{
			if (string.IsNullOrWhiteSpace(fullName))
			{
				return null;
			}

			Type type = null;

			for (int i = 0; type == null && i < Assemblies.Length; ++i)
			{
				type = GetTypeCache(Assemblies[i]).GetTypeByFullName(fullName, ignoreCase);
			}

			return type ?? GetTypeCache(Core.Assembly).GetTypeByFullName(fullName, ignoreCase);
		}

		public static IEnumerable<Type> FindTypesByFullName(string name)
		{
			return FindTypesByFullName(name, true);
		}

		public static IEnumerable<Type> FindTypesByFullName(string name, bool ignoreCase)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				yield break;
			}

			for (int i = 0; i < Assemblies.Length; ++i)
			{
				foreach (Type t in GetTypeCache(Assemblies[i]).GetTypesByFullName(name, ignoreCase))
				{
					yield return t;
				}
			}

			foreach (Type t in GetTypeCache(Core.Assembly).GetTypesByFullName(name, ignoreCase))
			{
				yield return t;
			}
		}

		public static Type FindTypeByName(string name)
		{
			return FindTypeByName(name, true);
		}

		public static Type FindTypeByName(string name, bool ignoreCase)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				return null;
			}

			Type type = null;

			for (int i = 0; type == null && i < Assemblies.Length; ++i)
			{
				type = GetTypeCache(Assemblies[i]).GetTypeByName(name, ignoreCase);
			}

			return type ?? GetTypeCache(Core.Assembly).GetTypeByName(name, ignoreCase);
		}

		public static IEnumerable<Type> FindTypesByName(string name)
		{
			return FindTypesByName(name, true);
		}

		public static IEnumerable<Type> FindTypesByName(string name, bool ignoreCase)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				yield break;
			}

			for (int i = 0; i < Assemblies.Length; ++i)
			{
				foreach (Type t in GetTypeCache(Assemblies[i]).GetTypesByName(name, ignoreCase))
				{
					yield return t;
				}
			}

			foreach (Type t in GetTypeCache(Core.Assembly).GetTypesByName(name, ignoreCase))
			{
				yield return t;
			}
		}

		public static void EnsureDirectory(string dir)
		{
			string path = Path.Combine(Core.BaseDirectory, dir);

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		public static string[] GetScripts(string filter)
		{
			List<string> list = new List<string>();

			GetScripts(list, Path.Combine(Core.BaseDirectory, "Scripts"), filter);

			return list.ToArray();
		}

		public static void GetScripts(List<string> list, string path, string filter)
		{
			foreach (string dir in Directory.GetDirectories(path))
			{
				GetScripts(list, dir, filter);
			}

			list.AddRange(Directory.GetFiles(path, filter));
		}
	}

	public class TypeCache
	{
		private readonly Type[] m_Types;
		private readonly TypeTable m_Names;
		private readonly TypeTable m_FullNames;

		public Type[] Types => m_Types;
		public TypeTable Names => m_Names;
		public TypeTable FullNames => m_FullNames;

		public Type GetTypeByName(string name, bool ignoreCase)
		{
			return GetTypesByName(name, ignoreCase).FirstOrDefault(t => t != null);
		}

		public IEnumerable<Type> GetTypesByName(string name, bool ignoreCase)
		{
			return m_Names.Get(name, ignoreCase);
		}

		public Type GetTypeByFullName(string fullName, bool ignoreCase)
		{
			return GetTypesByFullName(fullName, ignoreCase).FirstOrDefault(t => t != null);
		}

		public IEnumerable<Type> GetTypesByFullName(string fullName, bool ignoreCase)
		{
			return m_FullNames.Get(fullName, ignoreCase);
		}

		public TypeCache(Assembly asm)
		{
			if (asm == null)
			{
				m_Types = Type.EmptyTypes;
			}
			else
			{
				m_Types = asm.GetTypes();
			}

			m_Names = new TypeTable(m_Types.Length);
			m_FullNames = new TypeTable(m_Types.Length);

			foreach (IGrouping<string, Type> g in m_Types.ToLookup(t => t.Name))
			{
				m_Names.Add(g.Key, g);

				foreach (Type type in g)
				{
					m_FullNames.Add(type.FullName, type);

					TypeAliasAttribute attr = type.GetCustomAttribute<TypeAliasAttribute>(false);

					if (attr != null)
					{
						foreach (string a in attr.Aliases)
						{
							m_FullNames.Add(a, type);
						}
					}
				}
			}

			m_Names.Prune();
			m_FullNames.Prune();

			m_Names.Sort();
			m_FullNames.Sort();
		}
	}

	public class TypeTable
	{
		private readonly Dictionary<string, List<Type>> m_Sensitive;
		private readonly Dictionary<string, List<Type>> m_Insensitive;

		public void Prune()
		{
			Prune(m_Sensitive);
			Prune(m_Insensitive);
		}

		private static void Prune(Dictionary<string, List<Type>> types)
		{
			List<Type> buffer = new List<Type>();

			foreach (List<Type> list in types.Values)
			{
				if (list.Count == 1)
				{
					continue;
				}

				buffer.AddRange(list.Distinct());

				list.Clear();
				list.AddRange(buffer);

				buffer.Clear();
			}

			buffer.TrimExcess();
		}

		public void Sort()
		{
			Sort(m_Sensitive);
			Sort(m_Insensitive);
		}

		private static void Sort(Dictionary<string, List<Type>> types)
		{
			foreach (List<Type> list in types.Values)
			{
				list.Sort(InternalSort);
			}
		}

		private static int InternalSort(Type l, Type r)
		{
			if (l == r)
			{
				return 0;
			}

			if (l != null && r == null)
			{
				return -1;
			}

			if (l == null && r != null)
			{
				return 1;
			}

			bool a = IsEntity(l);
			bool b = IsEntity(r);

			if (a && b)
			{
				a = IsConstructable(l, out AccessLevel x);
				b = IsConstructable(r, out AccessLevel y);

				if (a && !b)
				{
					return -1;
				}

				if (!a && b)
				{
					return 1;
				}

				return x > y ? -1 : x < y ? 1 : 0;
			}

			return a ? -1 : b ? 1 : 0;
		}

		private static bool IsEntity(Type type)
		{
			return type.GetInterface("IEntity") != null;
		}

		private static bool IsConstructable(Type type, out AccessLevel access)
		{
			foreach (ConstructorInfo ctor in type.GetConstructors().OrderBy(o => o.GetParameters().Length))
			{
				ConstructableAttribute attr = ctor.GetCustomAttribute<ConstructableAttribute>(false);

				if (attr != null)
				{
					access = attr.AccessLevel;
					return true;
				}
			}

			access = 0;
			return false;
		}

		public void Add(string key, IEnumerable<Type> types)
		{
			if (!string.IsNullOrWhiteSpace(key) && types != null)
			{
				Add(key, types.ToArray());
			}
		}

		public void Add(string key, params Type[] types)
		{
			if (string.IsNullOrWhiteSpace(key) || types == null || types.Length == 0)
			{
				return;
			}

			if (!m_Sensitive.TryGetValue(key, out List<Type> sensitive) || sensitive == null)
			{
				m_Sensitive[key] = new List<Type>(types);
			}
			else if (types.Length == 1)
			{
				sensitive.Add(types[0]);
			}
			else
			{
				sensitive.AddRange(types);
			}

			if (!m_Insensitive.TryGetValue(key, out List<Type> insensitive) || insensitive == null)
			{
				m_Insensitive[key] = new List<Type>(types);
			}
			else if (types.Length == 1)
			{
				insensitive.Add(types[0]);
			}
			else
			{
				insensitive.AddRange(types);
			}
		}

		public IEnumerable<Type> Get(string key, bool ignoreCase)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				return Type.EmptyTypes;
			}

			List<Type> t;

			if (ignoreCase)
			{
				m_Insensitive.TryGetValue(key, out t);
			}
			else
			{
				m_Sensitive.TryGetValue(key, out t);
			}

			if (t == null)
			{
				return Type.EmptyTypes;
			}

			return t.AsEnumerable();
		}

		public TypeTable(int capacity)
		{
			m_Sensitive = new Dictionary<string, List<Type>>(capacity);
			m_Insensitive = new Dictionary<string, List<Type>>(capacity, StringComparer.OrdinalIgnoreCase);
		}
	}
}
