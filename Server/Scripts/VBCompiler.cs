using System;
using System.CodeDom.Compiler;

using Microsoft.VisualBasic;

namespace Server
{
    public class VBCompiler : CodeDomCompiler
    {
        public VBCompiler( CompilerWorkspace workspace )
            : base( workspace )
        {
        }

        protected override CodeDomProvider AcquireCodeDomProvider()
        {
            return new VBCodeProvider();
        }
    }
}
