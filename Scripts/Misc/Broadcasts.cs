using System;

namespace Server.Misc
{
    public class Broadcasts
    {
        public static void Initialize()
        {
            EventSink.Crashed += EventSink_Crashed;
            EventSink.Shutdown += EventSink_Shutdown;
        }

        public static void EventSink_Crashed(CrashedEventArgs e)
        {
            try
            {
                World.Broadcast(0x35, true, "The server has crashed.");
            }
            catch (Exception ex)
            {
                Diagnostics.ExceptionLogging.LogException(ex);
            }
        }

        public static void EventSink_Shutdown(ShutdownEventArgs e)
        {
            try
            {
                World.Broadcast(0x35, true, "The server has shut down.");
            }
            catch (Exception ex)
            {
                Diagnostics.ExceptionLogging.LogException(ex);
            }
        }
    }
}
