using System;
using System.CodeDom.Compiler;

using Microsoft.CSharp;

namespace Server
{
    public class CSharpCompiler : CodeDomCompiler
    {
        public CSharpCompiler( CompilerWorkspace workspace )
            : base( workspace )
        {
        }

        protected override CodeDomProvider AcquireCodeDomProvider()
        {
            return new CSharpCodeProvider();
        }
    }
}
