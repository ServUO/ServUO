using System;
using System.CodeDom.Compiler;

using Microsoft.VisualBasic;

namespace Server
{
    public class VBCompiler : CodeDomCompiler
    {
        protected override CodeDomProvider AcquireCodeDomProvider()
        {
            return new VBCodeProvider();
        }
    }
}
