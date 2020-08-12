using System;
using System.Collections.Generic;
using System.IO;

namespace Server.Diagnostics
{
    public class ExceptionLogging
    {
        public static string LogDirectory { get; set; }

        private static StreamWriter _Output;

        public static StreamWriter Output
        {
            get
            {
                if (_Output == null)
                {
                    _Output = new StreamWriter(Path.Combine(LogDirectory, String.Format("{0}.log", DateTime.UtcNow.ToLongDateString())), true);

                    _Output.AutoFlush = true;

                    _Output.WriteLine("##############################");
                    _Output.WriteLine("Exception log started on {0}", DateTime.UtcNow);
                    _Output.WriteLine();
                }

                return _Output;
            }
        }

        static ExceptionLogging()
        {
            var directory = Path.Combine(Core.BaseDirectory, "Logs/Exceptions");

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            LogDirectory = directory;
        }

        public static void LogException(Exception e)
        {
            Utility.WriteConsoleColor(ConsoleColor.Red, "Caught Exception:");
            Utility.WriteConsoleColor(ConsoleColor.DarkRed, e.ToString());

            Output.WriteLine("Exception Caught: {0}", DateTime.UtcNow);
            Output.WriteLine(e);
            Output.WriteLine();
        }

        public static void LogException(Exception e, string arg)
        {
            Utility.WriteConsoleColor(ConsoleColor.Red, "Caught Exception: {0}", arg);
            Utility.WriteConsoleColor(ConsoleColor.DarkRed, e.ToString());

            Output.WriteLine("Exception Caught: {0}", DateTime.UtcNow);
            Output.WriteLine(e);
            Output.WriteLine();
        }
    }
}
