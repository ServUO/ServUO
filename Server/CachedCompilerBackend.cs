using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace Server
{
    public class CachedCompilerBackend : ICompilerBackend
    {
        public string OutputDirectory => m_InnerCompiler.OutputDirectory;
        public string AssemblyFileName => m_InnerCompiler.AssemblyFileName;
        public string AssemblyPathPath => m_InnerCompiler.AssemblyPathPath;
        public string LanguageString => m_InnerCompiler.LanguageString;

        protected string HashFileName => string.Format("Scripts.{0}.hash", LanguageString);
        protected string HashFilePath => Path.Combine(m_InnerCompiler.OutputDirectory, HashFileName);

        private ICompilerBackend m_InnerCompiler;

        public CachedCompilerBackend(ICompilerBackend innerCompiler)
        {
            m_InnerCompiler = innerCompiler;
        }

        public Assembly CompileImpl(string[] files, bool debug)
        {
            if (File.Exists(AssemblyPathPath) && File.Exists(HashFilePath))
            {
                try
                {
                    var hashCode = CalculateHashCode(AssemblyPathPath, files, debug);

                    if (VerifyHashCode(hashCode))
                    {
                        var cachedAssembly = Assembly.LoadFrom(AssemblyPathPath);

                        Utility.PushColor(ConsoleColor.Green);
                        Console.WriteLine("done (cached)");
                        Utility.PopColor();

                        return cachedAssembly;
                    }
                }
                catch
                {
                }
            }

            var assembly = m_InnerCompiler.CompileImpl(files, debug);

            if (assembly != null && Path.GetFileName(assembly.Location) == AssemblyFileName)
            {
                try
                {
                    var hashCode = CalculateHashCode(assembly.Location, files, debug);

                    WriteHashCode(hashCode);
                }
                catch
                {
                }
            }

            return assembly;
        }

        private static byte[] CalculateHashCode(string compiledFile, string[] scriptFiles, bool debug)
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

        private bool VerifyHashCode(byte[] hashCode)
        {
            using (var fs = new FileStream(HashFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
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

        private void WriteHashCode(byte[] hashCode)
        {
            using (FileStream fs = new FileStream(HashFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (BinaryWriter bin = new BinaryWriter(fs))
                {
                    bin.Write(hashCode, 0, hashCode.Length);
                }
            }
        }
    }
}
