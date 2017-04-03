using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace Server
{
    public class CachedCompilerBackend : ICompilerBackend
    {
        protected string HashFileName => string.Format("Scripts.{0}.hash", Workspace.LanguageString);
        protected string HashFilePath => Path.Combine(Workspace.OutputDirectory, HashFileName);

        private ICompilerBackend m_InnerCompiler;
        public CompilerWorkspace Workspace { get; }

        public CachedCompilerBackend(ICompilerBackend innerCompiler)
        {
            m_InnerCompiler = innerCompiler;
            Workspace = innerCompiler.Workspace;
        }

        public Assembly CompileImpl(string[] files, bool debug)
        {
            if (File.Exists(Workspace.AssemblyPathPath) && File.Exists(HashFilePath))
            {
                try
                {
                    var hashCode = CalculateHashCode(Workspace.AssemblyPathPath, files, debug);

                    if (VerifyHashCode(hashCode))
                    {
                        var cachedAssembly = Assembly.LoadFrom(Workspace.AssemblyPathPath);

                        Console.WriteLine("done (cached)");

                        return cachedAssembly;
                    }
                }
                catch
                {
                }
            }

            var assembly = m_InnerCompiler.CompileImpl(files, debug);

            if (assembly != null && Path.GetFileName(assembly.Location) == Workspace.AssemblyFileName)
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
