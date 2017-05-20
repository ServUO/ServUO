using System;
using System.CodeDom.Compiler;
using System.IO;
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
                var tempFile = Path.GetTempFileName();

                // To prevent an "argument list too long" error, we write a list of file names to a temporary file and add them with @filename
                var writer = new StreamWriter( tempFile, false );
                foreach (string file in library.Files)
                {
                    writer.Write( "\"" + file + "\" " );
                }
                writer.Close();

                parms.CompilerOptions += " @" + tempFile;

                results = provider.CompileAssemblyFromFile(parms);

                File.Delete(tempFile);
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
