using System;
using System.Reflection;

namespace Server
{
    public interface ICompiler
    {
        Assembly Compile(Library library);
    }
}
