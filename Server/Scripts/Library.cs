using System;
using System.IO;

namespace Server
{
    public class Library
    {
        public string DisplayName { get; }
        public string BaseName { get; }
        public string[] Files { get; }
        public bool Debug { get; }

        private string OutputDirectory => ScriptCompiler.ScriptsOutputDirectory;

        public string AssemblyFileName => $"{BaseName}.dll";
        public string AssemblyFilePath => Path.Combine(OutputDirectory, AssemblyFileName);
        public string HashFileName => $"{BaseName}.hash";
        public string HashFilePath => Path.Combine(OutputDirectory, HashFileName);

        public Library(string displayName, string baseName, string[] files, bool debug)
        {
            DisplayName = displayName;
            BaseName = baseName;
            Files = files;
            Debug = debug;
        }

        public string GetUnusedPath()
        {
            var path = Path.Combine(string.Format("{0}/{1}.dll", OutputDirectory, BaseName));

            for (int i = 2; File.Exists(path) && i <= 1000; ++i)
            {
                path = Path.Combine(string.Format("{0}/{1}.{2}.dll", OutputDirectory, BaseName, i));
            }

            return path;
        }

        public void Clean()
        {
            DeleteFiles($"{BaseName}*.dll");
        }

        private void DeleteFiles(string mask)
        {
            try
            {
                var files = Directory.GetFiles(OutputDirectory, mask);

                foreach (var file in files)
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
