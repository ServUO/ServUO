using System.IO;
using System.Text;

namespace System
{
    public class ConsoleHook : TextWriter
    {
#if DEBUG
        private static readonly bool _Enabled = false;
#else
        private static readonly bool _Enabled = true;
#endif

        private static Stream m_OldOutput;
        private static bool m_Newline;

        public override Encoding Encoding => Encoding.ASCII;

        private string Timestamp => string.Format("{0:D2}:{1:D2}:{2:D2} ", DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, DateTime.UtcNow.Second);

        public static void Initialize()
        {
            if (_Enabled)
            {
                m_OldOutput = Console.OpenStandardOutput();
                Console.SetOut(new ConsoleHook());
                m_Newline = true;
            }
        }

        public override void WriteLine(string value)
        {
            if (m_Newline)
            {
                value = Timestamp + value;
            }

            byte[] data = Encoding.GetBytes(value);
            m_OldOutput.Write(data, 0, data.Length);
            m_OldOutput.WriteByte(10);
            m_Newline = true;
        }

        public override void Write(string value)
        {
            if (m_Newline)
            {
                value = Timestamp + value;
            }

            byte[] data = Encoding.GetBytes(value);
            m_OldOutput.Write(data, 0, data.Length);
            m_Newline = false;
        }
    }
}