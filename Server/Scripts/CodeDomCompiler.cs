using System;
using System.CodeDom.Compiler;
using System.Reflection;

namespace Server
{
    public abstract class CodeDomCompiler : ICompiler
    {
        public Assembly Compile(Library library)
        {
            CompilerResults results;

            using (CodeDomProvider provider = AcquireCodeDomProvider())
            {
                string path = library.GetUnusedPath();

                CompilerParameters parms = new CompilerParameters(ScriptCompiler.GetReferenceAssemblies(), path, library.Debug);

                string options = ScriptCompiler.GetCompilerOptions(library.Debug);

                if (options != null)
                {
                    parms.CompilerOptions = options;
                }

                if (Core.HaltOnWarning)
                {
                    parms.WarningLevel = 4;
                }

#if !MONO
                results = provider.CompileAssemblyFromFile(parms, library.Files);
#else
				parms.CompilerOptions = String.Format( "{0} /nowarn:169,219,414 /recurse:Scripts/*.cs", parms.CompilerOptions );
				results = provider.CompileAssemblyFromFile( parms, "" );
#endif
            }

            CompilerResultsRenderer.Display(results);

#if !MONO
            if (results.Errors.Count > 0)
            {
                return null;
            }
#else
            if( results.Errors.Count > 0 ) {
                foreach( CompilerError err in results.Errors ) {
                    if ( !err.IsWarning ) {
                        return null;
                    }
                }
            }
#endif

            return results.CompiledAssembly;
        }

        protected abstract CodeDomProvider AcquireCodeDomProvider();
    }
}
