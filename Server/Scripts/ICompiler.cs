using System;
using System.Reflection;

namespace Server
{
    public interface ICompiler
    {
        CompilerWorkspace Workspace { get; }
        Assembly CompileImpl(string[] files, bool debug);
    }
}
