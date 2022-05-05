using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Server
{
	public class Firewall
	{
		#region Firewall Entries

		public interface IFirewallEntry
		{
			bool IsBlocked(IPAddress address);
		}

		public class IPFirewallEntry : IFirewallEntry
		{
			private readonly IPAddress m_Address;

			public IPFirewallEntry(IPAddress address)
			{
				m_Address = address;
			}

			public bool IsBlocked(IPAddress address)
			{
				return m_Address.Equals(address);
			}

			public override string ToString()
			{
				return m_Address.ToString();
			}

			public override bool Equals(object obj)
			{
				if (obj is IPAddress ip)
				{
					return ip.Equals(m_Address);
				}

				if (obj is IPFirewallEntry fwe)
				{
					return m_Address.Equals(fwe.m_Address);
				}

				if (obj is string s)
				{
					return IPAddress.TryParse(s, out var otherAddress) && m_Address.Equals(otherAddress);
				}

				return false;
			}

			public override int GetHashCode()
			{
				return m_Address.GetHashCode();
			}
		}

		public class CIDRFirewallEntry : IFirewallEntry
		{
			private readonly IPAddress m_CIDRPrefix;
			private readonly int m_CIDRLength;

			public CIDRFirewallEntry(IPAddress cidrPrefix, int cidrLength)
			{
				m_CIDRPrefix = cidrPrefix;
				m_CIDRLength = cidrLength;
			}

			public bool IsBlocked(IPAddress address)
			{
				return Utility.IPMatchCIDR(m_CIDRPrefix, address, m_CIDRLength);
			}

			public override string ToString()
			{
				return System.String.Format("{0}/{1}", m_CIDRPrefix, m_CIDRLength);
			}

			public override bool Equals(object obj)
			{
				if (obj is string entry)
				{
					var str = entry.Split('/');

					if (str.Length == 2)
					{

						if (IPAddress.TryParse(str[0], out var cidrPrefix))
						{

							if (System.Int32.TryParse(str[1], out var cidrLength))
							{
								return m_CIDRPrefix.Equals(cidrPrefix) && m_CIDRLength.Equals(cidrLength);
							}
						}
					}
				}
				else if (obj is CIDRFirewallEntry fwe)
				{
					return m_CIDRPrefix.Equals(fwe.m_CIDRPrefix) && m_CIDRLength.Equals(fwe.m_CIDRLength);
				}

				return false;
			}

			public override int GetHashCode()
			{
				return m_CIDRPrefix.GetHashCode() ^ m_CIDRLength.GetHashCode();
			}
		}

		public class WildcardIPFirewallEntry : IFirewallEntry
		{
			private readonly string m_Entry;
			private bool m_Valid = true;

			public WildcardIPFirewallEntry(string entry)
			{
				m_Entry = entry;
			}

			public bool IsBlocked(IPAddress address)
			{
				if (!m_Valid)
				{
					return false;   //Why process if it's invalid?  it'll return false anyway after processing it.
				}

				return Utility.IPMatch(m_Entry, address, ref m_Valid);
			}

			public override string ToString()
			{
				return m_Entry.ToString();
			}

			public override bool Equals(object obj)
			{
				if (obj is string)
				{
					return obj.Equals(m_Entry);
				}

				if (obj is WildcardIPFirewallEntry fwe)
				{
					return m_Entry.Equals(fwe.m_Entry);
				}

				return false;
			}

			public override int GetHashCode()
			{
				return m_Entry.GetHashCode();
			}
		}

		#endregion

		public static HashSet<IFirewallEntry> List { get; } = new HashSet<IFirewallEntry>();

		static Firewall()
		{
			var path = Path.Combine("firewall.cfg");

			if (File.Exists(path))
			{
				foreach (var line in File.ReadLines(path))
				{
					var entry = line.Trim();

					if (entry.Length > 0)
					{
						var fwe = ToFirewallEntry(entry);

						if (fwe != null)
						{
							List.Add(fwe);
						}
					}
				}
			}
		}

		public static IFirewallEntry ToFirewallEntry(object entry)
		{
			if (entry is IFirewallEntry fwe)
			{
				return fwe;
			}

			if (entry is IPAddress ip)
			{
				return new IPFirewallEntry(ip);
			}

			if (entry is string s)
			{
				return ToFirewallEntry(s);
			}

			return null;
		}

		public static IFirewallEntry ToFirewallEntry(string entry)
		{
			if (IPAddress.TryParse(entry, out var addr))
			{
				return new IPFirewallEntry(addr);
			}

			//Try CIDR parse
			var str = entry.Split('/');

			if (str.Length == 2 && IPAddress.TryParse(str[0], out var cidrPrefix) && System.Int32.TryParse(str[1], out var cidrLength))
			{
				return new CIDRFirewallEntry(cidrPrefix, cidrLength);
			}

			return new WildcardIPFirewallEntry(entry);
		}

		public static void RemoveAt(int index)
		{
			var i = -1;

			IFirewallEntry fwe = null;

			foreach (var o in List)
			{
				if (++i == index)
				{
					fwe = o;
					break;
				}
			}

			if (i == index && List.Remove(fwe))
			{
				Save();
			}
		}

		public static void Remove(object obj)
		{
			if (List.Remove(ToFirewallEntry(obj)))
			{
				Save();
			}
		}

		public static void Add(object obj)
		{
			if (obj is IPAddress ip)
			{
				Add(ip);
			}
			else if (obj is string s)
			{
				Add(s);
			}
			else if (obj is IFirewallEntry fwe)
			{
				Add(fwe);
			}
		}

		public static void Add(IFirewallEntry entry)
		{
			if (List.Add(entry))
			{
				Save();
			}
		}

		public static void Add(string pattern)
		{
			if (List.Add(ToFirewallEntry(pattern)))
			{
				Save();
			}
		}

		public static void Add(IPAddress ip)
		{
			if (List.Add(new IPFirewallEntry(ip)))
			{
				Save();
			}
		}

		public static void Save()
		{
			File.WriteAllText("firewall.cfg", String.Join(Environment.NewLine, List));
		}

		public static bool IsBlocked(IPAddress ip)
		{
			foreach (var fwe in List)
			{
				if (fwe.IsBlocked(ip))
				{
					return true;
				}
			}

			return false;
		}
	}
}
