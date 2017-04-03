using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Server
{
    public abstract class CodeDomCompiler : ICompiler
    {
        public CompilerWorkspace Workspace { get; }

        public CodeDomCompiler(CompilerWorkspace workspace)
        {
            Workspace = workspace;
        }

        public Assembly CompileImpl(string[] files, bool debug)
        {
            CompilerResults results = null;

            DeleteFiles(string.Format("Scripts.{0}*.dll", Workspace.LanguageString));

            using (CodeDomProvider provider = AcquireCodeDomProvider())
            {
                string path = GetUnusedPath(string.Format("Scripts.{0}", Workspace.LanguageString));

                CompilerParameters parms = new CompilerParameters(ScriptCompiler.GetReferenceAssemblies(), path, debug);

                string options = ScriptCompiler.GetCompilerOptions(debug);

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

        protected abstract CodeDomProvider AcquireCodeDomProvider();

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
                    Console.WriteLine("Failed with: {0} errors, {1} warnings", errors.Count, warnings.Count);
                }
                else
                {
                    Console.WriteLine("Finished with: {0} errors, {1} warnings", errors.Count, warnings.Count);
                }

                string scriptRoot = Path.GetFullPath(ScriptCompiler.ScriptsDirectory);
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
                Console.WriteLine("Finished with: 0 errors, 0 warnings");
            }
        }

        public string GetUnusedPath(string name)
        {
            string path = Path.Combine(string.Format("{0}/{1}.dll", Workspace.OutputDirectory, name));

            for (int i = 2; File.Exists(path) && i <= 1000; ++i)
            {
                path = Path.Combine(string.Format("{0}/{1}.{2}.dll", Workspace.OutputDirectory, name, i));
            }

            return path;
        }

        public void DeleteFiles(string mask)
        {
            try
            {
                var files = Directory.GetFiles(Workspace.OutputDirectory, mask);

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
