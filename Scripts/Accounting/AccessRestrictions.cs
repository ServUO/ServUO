using System;
using System.IO;
using System.Net;

using Server.Misc;

namespace Server
{
    public class AccessRestrictions
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Initialize()
        {
            EventSink.SocketConnect += EventSink_SocketConnect;
        }

        private static void EventSink_SocketConnect(SocketConnectEventArgs e)
        {
            try
            {
                IPAddress ip = ((IPEndPoint)e.Socket.RemoteEndPoint).Address;

                if (Firewall.IsBlocked(ip))
                {
                    log.Warning("Client: {0}: Firewall blocked connection attempt.", ip);
                    e.AllowConnection = false;
                }
                else if (IPLimiter.SocketBlock && !IPLimiter.Verify(ip))
                {
                    log.Warning("Client: {0}: Past IP limit threshold", ip);

                    using (StreamWriter op = new StreamWriter("ipLimits.log", true))
                        op.WriteLine("{0}\tPast IP limit threshold\t{1}", ip, DateTime.UtcNow);

                    e.AllowConnection = false;
                }
            }
            catch
            {
                e.AllowConnection = false;
            }
        }
    }
}
