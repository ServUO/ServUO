using System.IO;

namespace Server
{
    public class CompilerWorkspace
    {
        private string m_LanguageString;

        public CompilerWorkspace( string mLanguageString )
        {
            m_LanguageString = mLanguageString;
        }

        public string OutputDirectory => Path.Combine(ScriptCompiler.ScriptsDirectory, "Output");
        public string AssemblyFileName => string.Format("Scripts.{0}.dll", LanguageString);
        public string AssemblyPathPath => Path.Combine(OutputDirectory, AssemblyFileName);
        public string LanguageString => m_LanguageString;
    }
}
