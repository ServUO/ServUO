using System;

namespace Server
{
    public interface ILog
    {
        void Debug(string msg, params object[] args);
        void Info(string msg, params object[] args);
        void Warning(string msg, params object[] args);
        void Error(string msg, params object[] args);
        void Fatal(string msg, params object[] args);

        void Log(LogLevel level, string msg, params object[] args);
    }
}
