using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Server
{
    public class Library
    {
        private string m_Name, m_FileExtension;
        private ICompiler m_Compiler;

        public Library(string name, string fileExtension, ICompiler compiler)
        {
            m_Name = name;
            m_FileExtension = fileExtension;
            m_Compiler = compiler;
        }

        public bool CompileScripts(out Assembly assembly)
        {
            return CompileScripts(false, out assembly);
        }

        public bool CompileScripts(bool debug, out Assembly assembly)
        {
            Console.Write("Scripts: Compiling {0} scripts...", m_Name);

            var files = GetScripts(string.Format("*.{0}", m_FileExtension));

            if (files.Length == 0)
            {
                Console.WriteLine("no files found.");
                assembly = null;
                return true;
            }

            assembly = m_Compiler.CompileImpl(files, debug);

            return assembly != null;
        }

        public static string[] GetScripts(string filter)
        {
            var list = new List<string>();

            GetScripts(list, ScriptCompiler.ScriptsDirectory, filter);

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
