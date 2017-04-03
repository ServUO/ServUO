using System.IO;

namespace Server
{
    public class CompilerWorkspace
    {
        private string m_FileExtension;

        public CompilerWorkspace( string fileExtension )
        {
            m_FileExtension = fileExtension;
        }

        public string OutputDirectory => Path.Combine(ScriptCompiler.ScriptsDirectory, "Output");
        public string AssemblyFileName => string.Format("Scripts.{0}.dll", FileExtension.ToUpper());
        public string AssemblyFilePath => Path.Combine(OutputDirectory, AssemblyFileName);
        public string FileExtension => m_FileExtension;
    }
}
