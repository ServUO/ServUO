#region References
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
#endregion

namespace Server.Misc
{
	public class ServerList
	{
		[ConfigProperty("Server.Address")]
		public static string Address
		{
			get => Config.Get("Server.Address", default(string)); 
			set
			{
				Config.Set("Server.Address", value);

				Invalidate();
			}
		}

		[ConfigProperty("Server.AutoDetect")]
		public static bool AutoDetect
		{
			get => Config.Get("Server.AutoDetect", true);
			set
			{
				Config.Set("Server.AutoDetect", value);

				Invalidate();
			}
		}

		[ConfigProperty("Server.Name")]
		public static string ServerName { get => Config.Get("Server.Name", "My Shard"); set => Config.Set("Server.Name", value); }

		private static IPAddress _PublicAddress;

		public static IPAddress PublicAddress => _PublicAddress;

		private static readonly Regex _AddressPattern = new Regex(@"([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3})");

		public static void Configure()
		{
			EventSink.ServerList += EventSink_ServerList;
		}

		public static void Initialize()
		{
			Invalidate();
		}

		private static void Invalidate()
		{
			if (Address == null)
			{
				if (AutoDetect)
				{
					AutoDetection();
				}
			}
			else
			{
				Resolve(Address, out _PublicAddress);
			}
		}

		private static void EventSink_ServerList(ServerListEventArgs e)
		{
			try
			{
				var ns = e.State;
				var s = ns.Socket;

				var ipep = (IPEndPoint)s.LocalEndPoint;

				var localAddress = ipep.Address;
				var localPort = ipep.Port;

				if (IsPrivateNetwork(localAddress))
				{
					ipep = (IPEndPoint)s.RemoteEndPoint;

					if (!IsPrivateNetwork(ipep.Address) && _PublicAddress != null)
					{
						localAddress = _PublicAddress;
					}
				}

				e.AddServer(ServerName, new IPEndPoint(localAddress, localPort));
			}
			catch
			{
				e.Rejected = true;
			}
		}

		public static string[] IPServices =
		{
			"http://api.ipify.org",
			"http://checkip.dyndns.org/"
		};

		private static void AutoDetection()
		{
			if (!HasPublicIPAddress())
			{
				Utility.PushColor(ConsoleColor.Yellow);
				Console.WriteLine("ServerList: Auto-detecting public IP address...");

				_PublicAddress = FindPublicAddress(IPServices);

				if (_PublicAddress != null)
				{
					Console.WriteLine("ServerList: Done: '{0}'", _PublicAddress);
				}
				else
				{
					_PublicAddress = IPAddress.Any;

					Console.WriteLine("ServerList: Failed: reverting to private IP address...");
				}

				Utility.PopColor();
			}
		}

		private static void Resolve(string addr, out IPAddress outValue)
		{
			if (IPAddress.TryParse(addr, out outValue))
			{
				return;
			}

			try
			{
				var iphe = Dns.GetHostEntry(addr);

				if (iphe.AddressList.Length > 0)
				{
					outValue = iphe.AddressList[iphe.AddressList.Length - 1];
				}
			}
			catch (Exception e)
			{
				Diagnostics.ExceptionLogging.LogException(e);
			}
		}

		private static bool HasPublicIPAddress()
		{
			var adapters = NetworkInterface.GetAllNetworkInterfaces();

			var uips = adapters.SelectMany(a => a.GetIPProperties().UnicastAddresses.Cast<IPAddressInformation>(), (p, u) => u.Address);

			return uips.Any(ip => !IPAddress.IsLoopback(ip) && ip.AddressFamily != AddressFamily.InterNetworkV6 && !IsPrivateNetwork(ip));
		}

		public static bool IsPrivateNetwork(IPAddress ip)
		{
			// 10.0.0.0/8
			// 172.16.0.0/12
			// 192.168.0.0/16
			// 169.254.0.0/16
			// 100.64.0.0/10 RFC 6598

			if (ip.AddressFamily == AddressFamily.InterNetworkV6)
			{
				return false;
			}

			if (Utility.IPMatch("192.168.*", ip))
			{
				return true;
			}

			if (Utility.IPMatch("10.*", ip))
			{
				return true;
			}

			if (Utility.IPMatch("172.16-31.*", ip))
			{
				return true;
			}

			if (Utility.IPMatch("169.254.*", ip))
			{
				return true;
			}

			if (Utility.IPMatch("100.64-127.*", ip))
			{
				return true;
			}

			return false;
		}

		public static IPAddress FindPublicAddress(params string[] services)
		{
			if (services == null || services.Length == 0)
			{
				services = IPServices;
			}

			if (services == null || services.Length == 0)
			{
				return null;
			}

			IPAddress ip = null;

			Uri uri;
			string data;
			Match match;

			foreach (var service in services.Where(s => !String.IsNullOrWhiteSpace(s)))
			{
				try
				{
					uri = new Uri(service);

					Console.WriteLine("ServerList: >>> {0}", uri.Host);

					using (var client = new WebClient())
					{
						data = client.DownloadString(uri);
					}

					Console.WriteLine("ServerList: <<< {0}", data);

					match = _AddressPattern.Match(data);

					if (!match.Success || !IPAddress.TryParse(match.Value, out ip))
					{
						ip = null;
					}
				}
				catch (UriFormatException)
				{
					Console.WriteLine("ServerList: Invalid IP service Uri '{0}'", service);

					ip = null;
				}
				catch
				{
				}

				if (ip != null)
				{
					break;
				}
			}

			return Utility.Intern(ip);
		}
	}
}
