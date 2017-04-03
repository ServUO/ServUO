#region Header
// **********
// ServUO - ScriptCompiler.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
#endregion

namespace Server
{
	public static class ScriptCompiler
	{
	    public static string ScriptsDirectory => Path.Combine( Core.BaseDirectory, "Scripts" );

		private static Assembly[] m_Assemblies;

		public static Assembly[] Assemblies { get { return m_Assemblies; } set { m_Assemblies = value; } }

		private static readonly List<string> m_AdditionalReferences = new List<string>();

		public static string[] GetReferenceAssemblies()
		{
			var list = new List<string>();

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

			if (!debug)
			{
				AppendCompilerOption(ref sb, "/optimize");
			}

#if MONO
			AppendCompilerOption( ref sb, "/d:MONO" );
            #endif

			//These two defines are legacy, ie, depreciated.
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

			var assemblies = new List<Assembly>();

			Assembly assembly;

		    ICompiler csCompiler = new CSharpCompiler(new CompilerWorkspace("cs"));
		    if (cache)
		        csCompiler = new CachedCompiler(csCompiler);

		    Library csLibrary = new Library("C#", "cs");

			if (csLibrary.CompileScripts(csCompiler, debug, out assembly))
			{
				if (assembly != null)
				{
					assemblies.Add(assembly);

				    if (!m_AdditionalReferences.Contains(assembly.Location))
				    {
				        m_AdditionalReferences.Add(assembly.Location);
				    }
				}
			}
			else
			{
				return false;
			}

			if (Core.VBdotNet)
			{
			    ICompiler vbCompiler = new VBCompiler(new CompilerWorkspace("vb"));
			    if (cache)
			        vbCompiler = new CachedCompiler(vbCompiler);

			    Library vbLibrary = new Library("VB.NET", "vb");

				if (vbLibrary.CompileScripts(vbCompiler, debug, out assembly))
				{
					if (assembly != null)
					{
						assemblies.Add(assembly);

					    if (!m_AdditionalReferences.Contains(assembly.Location))
					    {
					        m_AdditionalReferences.Add(assembly.Location);
					    }
					}
				}
				else
				{
					return false;
				}
			}
			else
			{
				Console.WriteLine("Scripts: Skipping VB.NET Scripts...done (use -vb to enable)");
			}

			if (assemblies.Count == 0)
			{
				return false;
			}

			m_Assemblies = assemblies.ToArray();

			Console.WriteLine("Scripts: Verifying...");

			Stopwatch watch = Stopwatch.StartNew();

			Core.VerifySerialization();

			watch.Stop();

			Console.WriteLine(
				"Finished ({0} items, {1} mobiles, {3} customs) ({2:F2} seconds)",
				Core.ScriptItems,
				Core.ScriptMobiles,
				watch.Elapsed.TotalSeconds,
				Core.ScriptCustoms);

			return true;
		}

		public static void Invoke(string method)
		{
			var invoke = new List<MethodInfo>();

			for (int a = 0; a < m_Assemblies.Length; ++a)
			{
				var types = m_Assemblies[a].GetTypes();

				for (int i = 0; i < types.Length; ++i)
				{
					MethodInfo m = types[i].GetMethod(method, BindingFlags.Static | BindingFlags.Public);

					if (m != null)
					{
						invoke.Add(m);
					}
				}
			}

			invoke.Sort(new CallPriorityComparer());

			for (int i = 0; i < invoke.Count; ++i)
			{
				invoke[i].Invoke(null, null);
			}
		}

		private static readonly Dictionary<Assembly, TypeCache> m_TypeCaches = new Dictionary<Assembly, TypeCache>();
		private static TypeCache m_NullCache;

		public static TypeCache GetTypeCache(Assembly asm)
		{
			if (asm == null)
			{
				if (m_NullCache == null)
				{
					m_NullCache = new TypeCache(null);
				}

				return m_NullCache;
			}

			TypeCache c = null;
			m_TypeCaches.TryGetValue(asm, out c);

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
			Type type = null;

			for (int i = 0; type == null && i < m_Assemblies.Length; ++i)
			{
				type = GetTypeCache(m_Assemblies[i]).GetTypeByFullName(fullName, ignoreCase);
			}

			if (type == null)
			{
				type = GetTypeCache(Core.Assembly).GetTypeByFullName(fullName, ignoreCase);
			}

			return type;
		}

		public static Type FindTypeByName(string name)
		{
			return FindTypeByName(name, true);
		}

		public static Type FindTypeByName(string name, bool ignoreCase)
		{
			Type type = null;

			for (int i = 0; type == null && i < m_Assemblies.Length; ++i)
			{
				type = GetTypeCache(m_Assemblies[i]).GetTypeByName(name, ignoreCase);
			}

			if (type == null)
			{
				type = GetTypeCache(Core.Assembly).GetTypeByName(name, ignoreCase);
			}

			return type;
		}

		public static void EnsureDirectory(string dir)
		{
			string path = Path.Combine(Core.BaseDirectory, dir);

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}
	}
}
