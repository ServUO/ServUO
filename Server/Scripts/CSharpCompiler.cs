using System;
using System.CodeDom.Compiler;

using Microsoft.CSharp;

namespace Server
{
    public class CSharpCompiler : CodeDomCompiler
    {
        protected override CodeDomProvider AcquireCodeDomProvider()
        {
            return new CSharpCodeProvider();
        }
    }
}
