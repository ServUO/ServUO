using System;
using System.Reflection;

namespace Server
{
    public interface ICompilerBackend
    {
        CompilerWorkspace Workspace { get; }
        Assembly CompileImpl(string[] files, bool debug);
    }
}
