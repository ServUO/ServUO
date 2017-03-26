using System;

namespace Server
{
    public class ConsoleLogger : BaseLogger
    {
        private string m_Category;

        public ConsoleLogger(string category)
        {
            m_Category = category;
        }

        public override void Log(LogLevel level, string msg, params object[] args)
        {
            var color = GetColorForLevel(level);

            if (color != null)
                Utility.PushColor(color.Value);

            Console.WriteLine(Format(level, msg, args));

            if (color != null)
                Utility.PopColor();
        }

        private static ConsoleColor? GetColorForLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return ConsoleColor.Gray;

                case LogLevel.Warning:
                    return ConsoleColor.DarkYellow;

                case LogLevel.Error:
                    return ConsoleColor.DarkRed;

                case LogLevel.Fatal:
                    return ConsoleColor.Red;
            }

            return null;
        }

        private string Format(LogLevel level, string msg, params object[] args)
        {
            if (level >= LogLevel.Warning)
                msg = string.Format("{0}: {1}", level, msg);

            return string.Format("{0}: {1}", m_Category, string.Format(msg, args));
        }
    }
}
