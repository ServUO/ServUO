using Server.Network;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class SocketOptions
    {
	    private static readonly IPEndPoint[] _ListenerEndPoints = new IPEndPoint[]
        {
	        new IPEndPoint(Config.Get("Server.Address", IPAddress.Any), Config.Get("Server.Port", 2593))
 			
	        // Examples:
			// new IPEndPoint(IPAddress.Any, Port), // Listen on port 2593 on all IP addresses
	        // new IPEndPoint( IPAddress.Any, 80 ), // Listen on port 80 on all IP addresses
	        // new IPEndPoint( IPAddress.Parse( "1.2.3.4" ), 2593 ), // Listen on port 2593 on IP address 1.2.3.4
        };

        public static bool NagleEnabled = false;// Should the Nagle algorithm be enabled? This may reduce performance
        public static int CoalesceBufferSize = 512;// MSS that the core will use when buffering packets

        public static void Initialize()
        {
            SendQueue.CoalesceBufferSize = CoalesceBufferSize;

            EventSink.SocketConnect += EventSink_SocketConnect;

            Listener.EndPoints = _ListenerEndPoints;
        }

        private static void EventSink_SocketConnect(SocketConnectEventArgs e)
        {
            if (!e.AllowConnection)
                return;

            if (!NagleEnabled)
                e.Socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1); // RunUO uses its own algorithm
        }
    }
}
