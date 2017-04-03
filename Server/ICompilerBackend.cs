using System;
using System.Reflection;

namespace Server
{
    public interface ICompilerBackend
    {
        string OutputDirectory { get; }
        string AssemblyFileName { get; }
        string AssemblyPathPath { get; }
        string LanguageString { get; }

        Assembly CompileImpl(string[] files, bool debug);
    }
}
