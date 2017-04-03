using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Server
{
    public class Compiler
    {
        private string m_LanguageName;
        private string m_LanguageExtension;
        private ICompilerBackend m_CompilerBackend;

        public Compiler(string languageName, string languageExtension, ICompilerBackend compilerBackend)
        {
            m_LanguageName = languageName;
            m_LanguageExtension = languageExtension;
            m_CompilerBackend = compilerBackend;
        }

        public bool CompileScripts(out Assembly assembly)
        {
            return CompileScripts(false, out assembly);
        }

        public bool CompileScripts(bool debug, out Assembly assembly)
        {
            Utility.PushColor(ConsoleColor.Green);
            Console.Write("Scripts: Compiling {0} scripts...", m_LanguageName);
            Utility.PopColor();

            var files = GetScripts(string.Format("*.{0}", m_LanguageExtension));

            if (files.Length == 0)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine("no files found.");
                Utility.PopColor();
                assembly = null;
                return true;
            }

            assembly = m_CompilerBackend.CompileImpl(files, debug);

            return assembly != null;
        }

        public static string[] GetScripts(string filter)
        {
            var list = new List<string>();

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
}
