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
using System.Security.Cryptography;
using System.Text;
#endregion

namespace Server
{
    public static class ScriptCompiler
    {
        public static string ScriptsDirectory { get { return Path.Combine(Core.BaseDirectory, "Scripts"); } }

        public static string ScriptsOutputDirectory { get { return Path.Combine(Core.BaseDirectory, "Scripts", "Output"); } }

        private static Assembly[] m_Assemblies;

        public static Assembly[] Assemblies { get { return m_Assemblies; } set { m_Assemblies = value; } }

        private static readonly List<string> m_AdditionalReferences = new List<string>();

        public static string[] GetReferenceAssemblies()
        {
            var list = new List<string>();

            string path = Path.Combine(Core.BaseDirectory, "Data", "Assemblies.cfg");

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

            //These two defines are legacy, ie, deprecated.
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
            EnsureDirectory(ScriptsDirectory);
            EnsureDirectory(ScriptsOutputDirectory);

            m_AdditionalReferences.Clear();

            var assemblies = new List<Assembly>();

            Library library;
            Assembly assembly;

            library = new Library("C# scripts", "Scripts.CS", GetScripts(ScriptsDirectory, "*.cs"), ScriptsOutputDirectory, debug);

            if (CompileScripts(library, new CSharpCompiler(), cache, out assembly))
            {
                if (assembly != null)
                {
                    assemblies.Add(assembly);

                    if (!m_AdditionalReferences.Contains(assembly.Location))
                        m_AdditionalReferences.Add(assembly.Location);
                }
            }
            else
            {
                return false;
            }

            if (Core.VBdotNet)
            {
                library = new Library("VB.NET scripts", "Scripts.VB", GetScripts(ScriptsDirectory, "*.vb"), ScriptsOutputDirectory, debug);

                if (CompileScripts(library, new VBCompiler(), cache, out assembly))
                {
                    if (assembly != null)
                    {
                        assemblies.Add(assembly);

                        if (!m_AdditionalReferences.Contains(assembly.Location))
                            m_AdditionalReferences.Add(assembly.Location);
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

            Console.Write("Scripts: Verifying...");

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

        public static string[] GetScripts(string path, string filter)
        {
            var list = new List<string>();

            GetScripts(list, path, filter);

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

        private static bool CompileScripts(Library library, ICompiler compiler, bool cache, out Assembly assembly)
        {
            Console.Write("Scripts: Compiling {0}...", library.DisplayName);

            if (library.Files.Length == 0)
            {
                Console.WriteLine("no files found.");
                assembly = null;
                return true;
            }

            if (cache && File.Exists(library.AssemblyFilePath) && File.Exists(library.HashFilePath))
            {
                try
                {
                    var hashCode = CalculateHashCode(library, library.AssemblyFilePath);

                    if (VerifyHashCode(library, hashCode))
                    {
                        var cachedAssembly = Assembly.LoadFrom(library.AssemblyFilePath);

                        Console.WriteLine("done (cached)");

                        assembly = cachedAssembly;
                        return true;
                    }
                }
                catch
                {
                }
            }

            library.Clean();

            assembly = compiler.Compile(library);

            if (cache && assembly != null && Path.GetFileName(assembly.Location) == library.AssemblyFileName)
            {
                try
                {
                    var hashCode = CalculateHashCode(library, assembly.Location);

                    WriteHashCode(library, hashCode);
                }
                catch
                {
                }

                return true;
            }

            return false;
        }

        private static byte[] CalculateHashCode(Library library, string assemblyFile)
        {
            using (var ms = new MemoryStream())
            {
                using (var bin = new BinaryWriter(ms))
                {
                    var fileInfo = new FileInfo(assemblyFile);

                    bin.Write(fileInfo.LastWriteTimeUtc.Ticks);

                    foreach (var scriptFile in library.Files)
                    {
                        fileInfo = new FileInfo(scriptFile);

                        bin.Write(fileInfo.LastWriteTimeUtc.Ticks);
                    }

                    bin.Write(library.Debug);
                    bin.Write(Core.Version.ToString());

                    ms.Position = 0;

                    using (var sha1 = SHA1.Create())
                    {
                        return sha1.ComputeHash(ms);
                    }
                }
            }
        }

        private static bool VerifyHashCode(Library library, byte[] hashCode)
        {
            using (var fs = new FileStream(library.HashFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var bin = new BinaryReader(fs))
                {
                    var bytes = bin.ReadBytes(hashCode.Length);

                    if (bytes.Length != hashCode.Length)
                        return false;

                    for (int i = 0; i < bytes.Length; ++i)
                    {
                        if (bytes[i] != hashCode[i])
                            return false;
                    }

                    return true;
                }
            }
        }

        private static void WriteHashCode(Library library, byte[] hashCode)
        {
            using (var fs = new FileStream(library.HashFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (var bin = new BinaryWriter(fs))
                {
                    bin.Write(hashCode, 0, hashCode.Length);
                }
            }
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
