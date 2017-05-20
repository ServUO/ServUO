using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;

namespace Server
{
    public class CompilerResultsRenderer
    {
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

                if (warnings.Count > 0)
                {
                    Utility.PushColor(ConsoleColor.Yellow);

                    Console.WriteLine("Warnings:");

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
                }

                if (errors.Count > 0)
                {
                    Utility.PushColor(ConsoleColor.Red);

                    Console.WriteLine("Errors:");

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
            }
            else
            {
                Console.WriteLine("Finished with: 0 errors, 0 warnings");
            }
        }
    }
}
