using System;
using System.Net;
using System.Net.Sockets;
using Server.Network;

namespace Server
{
    public class SocketOptions
    {
	    public static readonly int Port = Config.Get("Server.Port", 2593);

        private static readonly IPEndPoint[] m_ListenerEndPoints = new IPEndPoint[]
        {
            new IPEndPoint(IPAddress.Any, Port), // Default: Listen on port 2593 on all IP addresses
 			
            // Examples:
            // new IPEndPoint( IPAddress.Any, 80 ), // Listen on port 80 on all IP addresses
            // new IPEndPoint( IPAddress.Parse( "1.2.3.4" ), 2593 ), // Listen on port 2593 on IP address 1.2.3.4
        };

        public static bool NagleEnabled = false;// Should the Nagle algorithm be enabled? This may reduce performance
		public static int CoalesceBufferSize = 512;// MSS that the core will use when buffering packets

        public static void Initialize()
        {
            SendQueue.CoalesceBufferSize = CoalesceBufferSize;

            EventSink.SocketConnect += new SocketConnectEventHandler(EventSink_SocketConnect);

            Listener.EndPoints = m_ListenerEndPoints;
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