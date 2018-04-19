#region Header
// **********
// ServUO - Listener.cs
// **********
#endregion

#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
#endregion

namespace Server.Network
{
	public class Listener : IDisposable
	{
		private Socket m_Listener;
        private PingListener _PingListener;

        private readonly Queue<Socket> m_Accepted;
		private readonly object m_AcceptedSyncRoot;

		private readonly AsyncCallback m_OnAccept;

		private static readonly Socket[] m_EmptySockets = new Socket[0];

		public static IPEndPoint[] EndPoints { get; set; }

		public Listener(IPEndPoint ipep)
		{
			m_Accepted = new Queue<Socket>();
			m_AcceptedSyncRoot = ((ICollection)m_Accepted).SyncRoot;

			m_Listener = Bind(ipep);

			if (m_Listener == null)
			{
				return;
			}

			DisplayListener();
            _PingListener = new PingListener(ipep);

            m_OnAccept = OnAccept;
			try
			{
				IAsyncResult res = m_Listener.BeginAccept(m_OnAccept, m_Listener);
			}
			catch (SocketException ex)
			{
				NetState.TraceException(ex);
			}
			catch (ObjectDisposedException)
			{ }
		}

		private Socket Bind(IPEndPoint ipep)
		{
			Socket s = new Socket(ipep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

			try
			{
				s.LingerState.Enabled = false;
				
				// Default is 'false' starting Windows Vista and Server 2008. Source: https://msdn.microsoft.com/en-us/library/system.net.sockets.socket.exclusiveaddressuse(v=vs.110).aspx?f=255&MSPPError=-2147217396
				s.ExclusiveAddressUse = false;

				s.Bind(ipep);
				s.Listen(8);

				return s;
			}
			catch (Exception e)
			{
				if (e is SocketException)
				{
					SocketException se = (SocketException)e;

					if (se.ErrorCode == 10048)
					{
						// WSAEADDRINUSE
						Utility.PushColor(ConsoleColor.Red);
						Console.WriteLine("Listener Failed: {0}:{1} (In Use)", ipep.Address, ipep.Port);
						Utility.PopColor();
					}
					else if (se.ErrorCode == 10049)
					{
						// WSAEADDRNOTAVAIL
						Utility.PushColor(ConsoleColor.Red);
						Console.WriteLine("Listener Failed: {0}:{1} (Unavailable)", ipep.Address, ipep.Port);
						Utility.PopColor();
					}
					else
					{
						Utility.PushColor(ConsoleColor.Red);
						Console.WriteLine("Listener Exception:");
						Console.WriteLine(e);
						Utility.PopColor();
					}
				}

				return null;
			}
		}

		private void DisplayListener()
		{
			IPEndPoint ipep = m_Listener.LocalEndPoint as IPEndPoint;

			if (ipep == null)
			{
				return;
			}

			if (ipep.Address.Equals(IPAddress.Any) || ipep.Address.Equals(IPAddress.IPv6Any))
			{
				var adapters = NetworkInterface.GetAllNetworkInterfaces();
				foreach (NetworkInterface adapter in adapters)
				{
					IPInterfaceProperties properties = adapter.GetIPProperties();
					foreach (IPAddressInformation unicast in properties.UnicastAddresses)
					{
						if (ipep.AddressFamily == unicast.Address.AddressFamily)
						{
							Utility.PushColor(ConsoleColor.Green);
							Console.WriteLine("Listening: {0}:{1}", unicast.Address, ipep.Port);
							Utility.PopColor();
						}
					}
				}
				/*
                try {
                Console.WriteLine( "Listening: {0}:{1}", IPAddress.Loopback, ipep.Port );
                IPHostEntry iphe = Dns.GetHostEntry( Dns.GetHostName() );
                IPAddress[] ip = iphe.AddressList;
                for ( int i = 0; i < ip.Length; ++i )
                Console.WriteLine( "Listening: {0}:{1}", ip[i], ipep.Port );
                }
                catch { }
                */
			}
			else
			{
				Utility.PushColor(ConsoleColor.Green);
				Console.WriteLine("Listening: {0}:{1}", ipep.Address, ipep.Port);
				Utility.PopColor();
			}

			Utility.PushColor(ConsoleColor.DarkGreen);
			Console.WriteLine(@"----------------------------------------------------------------------");
			Utility.PopColor();
		}
		
		private void OnAccept(IAsyncResult asyncResult)
		{
			Socket listener = (Socket)asyncResult.AsyncState;

			Socket accepted = null;

			try
			{
				accepted = listener.EndAccept(asyncResult);
			}
			catch (SocketException ex)
			{
				NetState.TraceException(ex);
			}
			catch (ObjectDisposedException)
			{
				return;
			}

			if (accepted != null)
			{
				if (VerifySocket(accepted))
				{
					Enqueue(accepted);
				}
				else
				{
					Release(accepted);
				}
			}

			try
			{
				listener.BeginAccept(m_OnAccept, listener);
			}
			catch (SocketException ex)
			{
				NetState.TraceException(ex);
			}
			catch (ObjectDisposedException)
			{ }
		}

		private bool VerifySocket(Socket socket)
		{
			try
			{
				SocketConnectEventArgs args = new SocketConnectEventArgs(socket);

				EventSink.InvokeSocketConnect(args);

				return args.AllowConnection;
			}
			catch (Exception ex)
			{
				NetState.TraceException(ex);

				return false;
			}
		}

		private void Enqueue(Socket socket)
		{
			lock (m_AcceptedSyncRoot)
			{
				m_Accepted.Enqueue(socket);
			}

			Core.Set();
		}

		private void Release(Socket socket)
		{
			try
			{
				socket.Shutdown(SocketShutdown.Both);
			}
			catch (SocketException ex)
			{
				NetState.TraceException(ex);
			}

			try
			{
				socket.Close();
			}
			catch (SocketException ex)
			{
				NetState.TraceException(ex);
			}
		}

		public Socket[] Slice()
		{
			Socket[] array;

			lock (m_AcceptedSyncRoot)
			{
				if (m_Accepted.Count == 0)
				{
					return m_EmptySockets;
				}

				array = m_Accepted.ToArray();
				m_Accepted.Clear();
			}

			return array;
		}

		public void Dispose()
		{
			Socket socket = Interlocked.Exchange(ref m_Listener, null);

			if (socket != null)
			{
				socket.Close();
			}

            if (_PingListener == null)
            {
                return;
            }

            _PingListener.Dispose();
            _PingListener = null;
        }
	}
}