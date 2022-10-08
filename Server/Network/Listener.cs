#region References
using System;
using System.Collections;
using System.Collections.Concurrent;
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
		public static IPEndPoint[] EndPoints { get; set; }

		private volatile Socket m_Listener;
		private volatile PingListener m_PingListener;

		private readonly ConcurrentQueue<Socket> m_Accepted;

		private readonly AsyncCallback m_OnAccept;

		public Listener(IPEndPoint ipep)
		{
			m_Accepted = new ConcurrentQueue<Socket>();

			m_Listener = Bind(ipep);

			if (m_Listener == null)
			{
				return;
			}

			DisplayListener();

			m_PingListener = new PingListener(ipep);

			m_OnAccept = OnAccept;

			try
			{
				m_Listener.BeginAccept(m_OnAccept, m_Listener);
			}
			catch (SocketException ex)
			{
				NetState.TraceException(ex);
			}
			catch (ObjectDisposedException)
			{ }
		}

		private static Socket Bind(IPEndPoint ipep)
		{
			var s = new Socket(ipep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

			try
			{
				s.NoDelay = true;
				s.LingerState.Enabled = false;

				// Default is 'false' starting Windows Vista and Server 2008. Source: https://msdn.microsoft.com/en-us/library/system.net.sockets.socket.exclusiveaddressuse(v=vs.110).aspx?f=255&MSPPError=-2147217396
				s.ExclusiveAddressUse = false;

				s.Bind(ipep);
				s.Listen(8);

				return s;
			}
			catch (Exception e)
			{
				if (e is SocketException se)
				{
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
			if (m_Listener.LocalEndPoint is IPEndPoint ipep)
			{
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
								Utility.WriteLine(ConsoleColor.Green, $"Listening: {unicast.Address}:{ipep.Port}");
							}
						}
					}
				}
				else
				{
					Utility.WriteLine(ConsoleColor.Green, $"Listening: {ipep.Address}:{ipep.Port}");
				}

				Utility.WriteLine(ConsoleColor.DarkGreen, new string('-', Console.BufferWidth));
			}
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
			m_Accepted.Enqueue(socket);

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

		public IEnumerable<Socket> Slice()
		{
			if (m_Accepted.Count == 0)
			{
				yield break;
			}

			var count = m_Accepted.Count;

			while (--count >= 0)
			{
				if (m_Accepted.TryDequeue(out var socket))
				{
					yield return socket;
				}
			}
		}

		public void Dispose()
		{
			var socket = Interlocked.Exchange(ref m_Listener, null);

			if (socket != null)
			{
				socket.Close();
			}

			var ping = Interlocked.Exchange(ref m_PingListener, null);

			if (ping != null)
			{
				ping.Dispose();
			}
		}
	}
}
