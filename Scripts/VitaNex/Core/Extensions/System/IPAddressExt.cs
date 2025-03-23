#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;

using Server;
#endregion

namespace System
{
	public static class IPAddressExtUtility
	{
		private static readonly Regex _AddressPattern = new Regex(@"([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3})");

		private static IPAddress _Public;

		public static IPAddress FindPublic()
		{
			if (_Public != null)
			{
				return _Public;
			}

			var data = String.Empty;

			var request = WebRequest.Create("https://api.ipify.org");

			using (var response = request.GetResponse())
			{
				var r = response.GetResponseStream();

				if (r != null)
				{
					using (var stream = new StreamReader(r))
					{
						data = stream.ReadToEnd();
					}
				}
			}

			var m = _AddressPattern.Match(data);

			return (_Public = m.Success ? IPAddress.Parse(m.Value) : null);
		}

		public static IEnumerable<IPAddress> FindInternal(this IPAddress address)
		{
			if (address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any))
			{
				return NetworkInterface.GetAllNetworkInterfaces()
									   .Select(a => a.GetIPProperties())
									   .SelectMany(p => p.UnicastAddresses.Where(u => address.AddressFamily == u.Address.AddressFamily))
									   .Select(uni => uni.Address);
			}

			return address.ToEnumerable();
		}

		public static bool IsPrivateNetwork(this IPAddress address)
		{
			// 10.0.0.0/8
			// 172.16.0.0/12
			// 192.168.0.0/16
			// 169.254.0.0/16
			// 100.64.0.0/10 RFC 6598

			if (address.AddressFamily == AddressFamily.InterNetworkV6)
			{
				return false;
			}

			if (Utility.IPMatch("192.168.*", address))
			{
				return true;
			}

			if (Utility.IPMatch("10.*", address))
			{
				return true;
			}

			if (Utility.IPMatch("172.16-31.*", address))
			{
				return true;
			}

			if (Utility.IPMatch("169.254.*", address))
			{
				return true;
			}

			if (Utility.IPMatch("100.64-127.*", address))
			{
				return true;
			}

			return false;
		}
	}
}