using System;

namespace Server
{
    public abstract class BaseLogger : ILog
    {
        public void Debug(string msg, params object[] args)
        {
            Log(LogLevel.Debug, msg, args);
        }

        public void Info(string msg, params object[] args)
        {
            Log(LogLevel.Info, msg, args);
        }

        public void Warning(string msg, params object[] args)
        {
            Log(LogLevel.Warning, msg, args);
        }

        public void Error(string msg, params object[] args)
        {
            Log(LogLevel.Error, msg, args);
        }

        public void Fatal(string msg, params object[] args)
        {
            Log(LogLevel.Fatal, msg, args);
        }

        public abstract void Log(LogLevel level, string msg, params object[] args);
    }
}
