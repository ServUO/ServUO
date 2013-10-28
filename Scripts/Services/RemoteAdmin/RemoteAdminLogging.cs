using System;
using System.IO;
using Server.Accounting;
using Server.Network;

namespace Server.RemoteAdmin
{
    public class RemoteAdminLogging
    {
        private static StreamWriter m_Output;
        private static bool m_Enabled = true;
        private static bool Initialized = false;
        const string LogBaseDirectory = "Logs";
        const string LogSubDirectory = "RemoteAdmin";
        public static bool Enabled
        {
            get
            {
                return m_Enabled;
            }
            set
            {
                m_Enabled = value;
            }
        }
        public static StreamWriter Output
        {
            get
            {
                return m_Output;
            }
        }
        public static void LazyInitialize()
        {
            if (Initialized || !m_Enabled)
                return;
            Initialized = true;

            if (!Directory.Exists(LogBaseDirectory))
                Directory.CreateDirectory(LogBaseDirectory);

            string directory = Path.Combine(LogBaseDirectory, LogSubDirectory);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            try
            {
                m_Output = new StreamWriter(Path.Combine(directory, String.Format(LogSubDirectory + "{0}.log", DateTime.UtcNow.ToString("yyyyMMdd"))), true);

                m_Output.AutoFlush = true;

                m_Output.WriteLine("##############################");
                m_Output.WriteLine("Log started on {0}", DateTime.UtcNow);
                m_Output.WriteLine();
            }
            catch
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine("RemoteAdminLogging: Failed to initialize LogWriter.");
                Utility.PopColor();
                m_Enabled = false;
            }
        }

        public static object Format(object o)
        {
            o = Commands.CommandLogging.Format(o);
            if (o == null)
                return "(null)";

            return o;
        }

        public static void WriteLine(NetState state, string format, params object[] args)
        {
            for (int i = 0; i < args.Length; i++)
                args[i] = Commands.CommandLogging.Format(args[i]);

            WriteLine(state, String.Format(format, args));
        }

        public static void WriteLine(NetState state, string text)
        {
            LazyInitialize();

            if (!m_Enabled)
                return;

            try
            {
                Account acct = state.Account as Account;
                string name = acct == null ? "(UNKNOWN)" : acct.Username;
                string accesslevel = acct == null ? "NoAccount" : acct.AccessLevel.ToString();
                string statestr = state == null ? "NULLSTATE" : state.ToString();

                m_Output.WriteLine("{0}: {1}: {2}: {3}", DateTime.UtcNow, statestr, name, text);

                string path = Core.BaseDirectory;

                Commands.CommandLogging.AppendPath(ref path, LogBaseDirectory);
                Commands.CommandLogging.AppendPath(ref path, LogSubDirectory);
                Commands.CommandLogging.AppendPath(ref path, accesslevel);
                path = Path.Combine(path, String.Format("{0}.log", name));

                using (StreamWriter sw = new StreamWriter(path, true))
                    sw.WriteLine("{0}: {1}: {2}", DateTime.UtcNow, statestr, text);
            }
            catch
            {
            }
        }
    }
}