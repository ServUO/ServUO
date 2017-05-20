using System;
using System.IO;

namespace Server
{
    public class Library
    {
        public string DisplayName { get; private set; }
        public string BaseName { get; private set; }
        public string[] Files { get; private set; }
        public string OutputDirectory { get; private set; }
        public bool Debug { get; private set; }

        public string AssemblyFileName { get { return string.Format("{0}.dll", BaseName); } }
        public string AssemblyFilePath { get { return Path.Combine(OutputDirectory, AssemblyFileName); } }
        public string HashFileName { get { return string.Format("{0}.hash", BaseName); } }
        public string HashFilePath { get { return Path.Combine(OutputDirectory, HashFileName); } }

        public Library(string displayName, string baseName, string[] files, string outputDirectory, bool debug)
        {
            DisplayName = displayName;
            BaseName = baseName;
            Files = files;
            OutputDirectory = outputDirectory;
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
            DeleteFiles(string.Format("{0}*.dll", BaseName));
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
