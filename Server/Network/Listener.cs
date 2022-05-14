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
		private Socket _Listener;
		private PingListener _PingListener;

		private readonly Queue<Socket> _Accepted;
		private readonly object _AcceptedSyncRoot;

		private readonly AsyncCallback _OnAccept;

		private static readonly Socket[] _EmptySockets = new Socket[0];

		public static IPEndPoint[] EndPoints { get; set; } = new IPEndPoint[]
		{
			new IPEndPoint(Config.Get("Server.Address", IPAddress.Any), Config.Get("Server.Port", 2593))
		};

		public Listener(IPEndPoint ipep)
		{
			_Accepted = new Queue<Socket>();
			_AcceptedSyncRoot = ((ICollection)_Accepted).SyncRoot;

			_Listener = Bind(ipep);

			if (_Listener == null)
			{
				return;
			}

			DisplayListener();
			_PingListener = new PingListener(ipep);

			_OnAccept = OnAccept;
			try
			{
				var res = _Listener.BeginAccept(_OnAccept, _Listener);
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
			var s = new Socket(ipep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

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
					var se = (SocketException)e;

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
			var ipep = _Listener.LocalEndPoint as IPEndPoint;

			if (ipep == null)
			{
				return;
			}

			if (ipep.Address.Equals(IPAddress.Any) || ipep.Address.Equals(IPAddress.IPv6Any))
			{
				var adapters = NetworkInterface.GetAllNetworkInterfaces();
				foreach (var adapter in adapters)
				{
					var properties = adapter.GetIPProperties();
					foreach (var unicast in properties.UnicastAddresses)
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
			Console.WriteLine("----------------------------------------------------------------------");
			Utility.PopColor();
		}

		private void OnAccept(IAsyncResult asyncResult)
		{
			var listener = (Socket)asyncResult.AsyncState;

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
				listener.BeginAccept(_OnAccept, listener);
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
				var args = new SocketConnectEventArgs(socket);

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
			lock (_AcceptedSyncRoot)
			{
				_Accepted.Enqueue(socket);
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

			lock (_AcceptedSyncRoot)
			{
				if (_Accepted.Count == 0)
				{
					return _EmptySockets;
				}

				array = _Accepted.ToArray();
				_Accepted.Clear();
			}

			return array;
		}

		public void Dispose()
		{
			var socket = Interlocked.Exchange(ref _Listener, null);

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
