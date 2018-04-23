using System;
using System.IO;
using Server;

namespace Knives.Chat3
{
	public class Logging
	{
        public static void LogChat(string msg)
        {
            if (!Directory.Exists("Logs"))
                Directory.CreateDirectory("Logs");

            string directory = "Logs/Chat";

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            try
            {
                StreamWriter writer = new StreamWriter(Path.Combine(directory, String.Format("Chat-{0}.log", DateTime.Now.ToLongDateString())), true);

                writer.AutoFlush = true;
                writer.WriteLine(msg);
            }
            catch
            {
            }
        }

        public static void LogPm(string msg)
        {
            if (!Directory.Exists("Logs"))
                Directory.CreateDirectory("Logs");

            string directory = "Logs/Chat";

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            try
            {
                StreamWriter writer = new StreamWriter(Path.Combine(directory, String.Format("Pm-{0}.log", DateTime.Now.ToLongDateString())), true);

                writer.AutoFlush = true;
                writer.WriteLine(msg);
            }
            catch
            {
            }
        }
    }
}