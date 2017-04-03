using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Server
{
    public class CodeDomCompilerBackend : ICompilerBackend
    {
        private readonly string m_LanguageString;
        private readonly CodeDomProvider m_CodeDomProvider;

        public CodeDomCompilerBackend(string languageString, CodeDomProvider codeDomProvider)
        {
            m_LanguageString = languageString;
            m_CodeDomProvider = codeDomProvider;
        }

        public string OutputDirectory => "Scripts/Output";
        public string AssemblyFileName => string.Format("Scripts.{0}.dll", LanguageString);
        public string AssemblyPathPath => Path.Combine(OutputDirectory, AssemblyFileName);
        public string LanguageString => m_LanguageString;

        public Assembly CompileImpl(string[] files, bool debug)
        {
            CompilerResults results = null;

            DeleteFiles(string.Format("Scripts.{0}*.dll", LanguageString));

            using (CodeDomProvider provider = m_CodeDomProvider)
            {
                string path = GetUnusedPath(string.Format("Scripts.{0}", LanguageString));

                CompilerParameters parms = new CompilerParameters(ScriptCompiler.GetReferenceAssemblies(), path, debug);

                string options = GetCompilerOptions(debug);

                if (options != null)
                {
                    parms.CompilerOptions = options;
                }

                if (Core.HaltOnWarning)
                {
                    parms.WarningLevel = 4;
                }

#if !MONO
                results = provider.CompileAssemblyFromFile(parms, files);
#else
				parms.CompilerOptions = String.Format( "{0} /nowarn:169,219,414 /recurse:Scripts/*.cs", parms.CompilerOptions );
				results = provider.CompileAssemblyFromFile( parms, "" );
#endif
            }

            Display(results);

#if !MONO
            if (results.Errors.Count > 0)
            {
                return null;
            }
#else
            if( results.Errors.Count > 0 ) {
                foreach( CompilerError err in results.Errors ) {
                    if ( !err.IsWarning ) {
                        return null;
                    }
                }
            }
#endif

            return results.CompiledAssembly;
        }

        public static void Display(CompilerResults results)
        {
            if (results.Errors.Count > 0)
            {
                var errors = new Dictionary<string, List<CompilerError>>(results.Errors.Count, StringComparer.OrdinalIgnoreCase);
                var warnings = new Dictionary<string, List<CompilerError>>(results.Errors.Count, StringComparer.OrdinalIgnoreCase);

                foreach (CompilerError e in results.Errors)
                {
                    string file = e.FileName;

                    // Ridiculous. FileName is null if the warning/error is internally generated in csc.
                    if (string.IsNullOrEmpty(file))
                    {
                        Console.WriteLine("ScriptCompiler: {0}: {1}", e.ErrorNumber, e.ErrorText);
                        continue;
                    }

                    var table = (e.IsWarning ? warnings : errors);

                    List<CompilerError> list = null;
                    table.TryGetValue(file, out list);

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
                    Console.WriteLine("Finished with: {0} errors, {1} warnings", errors.Count, warnings.Count);
                    Utility.PopColor();
                }

                string scriptRoot = Path.GetFullPath(Path.Combine(Core.BaseDirectory, "Scripts" + Path.DirectorySeparatorChar));
                Uri scriptRootUri = new Uri(scriptRoot);

                Utility.PushColor(ConsoleColor.Yellow);

                if (warnings.Count > 0)
                {
                    Console.WriteLine("Warnings:");
                }

                foreach (var kvp in warnings)
                {
                    string fileName = kvp.Key;
                    var list = kvp.Value;

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

                foreach (var kvp in errors)
                {
                    string fileName = kvp.Key;
                    var list = kvp.Value;

                    string fullPath = Path.GetFullPath(fileName);
                    string usedPath = Uri.UnescapeDataString(scriptRootUri.MakeRelativeUri(new Uri(fullPath)).OriginalString);

                    Console.WriteLine(" + {0}:", usedPath);

                    Utility.PushColor(ConsoleColor.DarkRed);

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
                Console.WriteLine("Finished with: 0 errors, 0 warnings");
                Utility.PopColor();
            }
        }

        public string GetUnusedPath(string name)
        {
            string path = Path.Combine(Core.BaseDirectory, String.Format("{0}/{1}.dll", OutputDirectory, name));

            for (int i = 2; File.Exists(path) && i <= 1000; ++i)
            {
                path = Path.Combine(Core.BaseDirectory, String.Format("{0}/{1}.{2}.dll", OutputDirectory, name, i));
            }

            return path;
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

        public void DeleteFiles(string mask)
        {
            try
            {
                var files = Directory.GetFiles(Path.Combine(Core.BaseDirectory, OutputDirectory), mask);

                foreach (string file in files)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
        }
    }
}
