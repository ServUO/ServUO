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
            readonly IPAddress m_Address;
            public IPFirewallEntry(IPAddress address)
            {
                this.m_Address = address;
            }

            public bool IsBlocked(IPAddress address)
            {
                return this.m_Address.Equals(address);
            }

            public override string ToString()
            {
                return this.m_Address.ToString();
            }

            public override bool Equals(object obj)
            {
                if (obj is IPAddress)
                {
                    return obj.Equals(this.m_Address);
                }
                else if (obj is string)
                {
                    IPAddress otherAddress;

                    if (IPAddress.TryParse((string)obj, out otherAddress))
                        return otherAddress.Equals(this.m_Address);
                }
                else if (obj is IPFirewallEntry)
                {
                    return this.m_Address.Equals(((IPFirewallEntry)obj).m_Address);
                }

                return false;
            }

            public override int GetHashCode()
            {
                return this.m_Address.GetHashCode();
            }
        }

        public class CIDRFirewallEntry : IFirewallEntry
        {
            readonly IPAddress m_CIDRPrefix;
            readonly int m_CIDRLength;

            public CIDRFirewallEntry(IPAddress cidrPrefix, int cidrLength)
            {
                this.m_CIDRPrefix = cidrPrefix;
                this.m_CIDRLength = cidrLength;
            }

            public bool IsBlocked(IPAddress address)
            {
                return Utility.IPMatchCIDR(this.m_CIDRPrefix, address, this.m_CIDRLength);
            }

            public override string ToString()
            {
                return String.Format("{0}/{1}", this.m_CIDRPrefix, this.m_CIDRLength);
            }

            public override bool Equals(object obj)
            {
                if (obj is string)
                {
                    string entry = (string)obj;

                    string[] str = entry.Split('/');

                    if (str.Length == 2)
                    {
                        IPAddress cidrPrefix;

                        if (IPAddress.TryParse(str[0], out cidrPrefix))
                        {
                            int cidrLength;

                            if (int.TryParse(str[1], out cidrLength))
                                return this.m_CIDRPrefix.Equals(cidrPrefix) && this.m_CIDRLength.Equals(cidrLength);
                        }
                    }
                }
                else if (obj is CIDRFirewallEntry)
                {
                    CIDRFirewallEntry entry = obj as CIDRFirewallEntry;

                    return this.m_CIDRPrefix.Equals(entry.m_CIDRPrefix) && this.m_CIDRLength.Equals(entry.m_CIDRLength);
                }

                return false;
            }

            public override int GetHashCode()
            {
                return this.m_CIDRPrefix.GetHashCode() ^ this.m_CIDRLength.GetHashCode();
            }
        }

        public class WildcardIPFirewallEntry : IFirewallEntry
        {
            readonly string m_Entry;

            bool m_Valid = true;

            public WildcardIPFirewallEntry(string entry)
            {
                this.m_Entry = entry;
            }

            public bool IsBlocked(IPAddress address)
            {
                if (!this.m_Valid)
                    return false;	//Why process if it's invalid?  it'll return false anyway after processing it.

                return Utility.IPMatch(this.m_Entry, address, ref this.m_Valid);
            }

            public override string ToString()
            {
                return this.m_Entry.ToString();
            }

            public override bool Equals(object obj)
            {
                if (obj is string)
                    return obj.Equals(this.m_Entry);
                else if (obj is WildcardIPFirewallEntry)
                    return this.m_Entry.Equals(((WildcardIPFirewallEntry)obj).m_Entry);

                return false;
            }

            public override int GetHashCode()
            {
                return this.m_Entry.GetHashCode();
            }
        }
        #endregion

        private static List<IFirewallEntry> m_Blocked;

        static Firewall()
        {
            m_Blocked = new List<IFirewallEntry>();

            string path = "firewall.cfg";

            if (File.Exists(path))
            {
                using (StreamReader ip = new StreamReader(path))
                {
                    string line;

                    while ((line = ip.ReadLine()) != null)
                    {
                        line = line.Trim();

                        if (line.Length == 0)
                            continue;

                        m_Blocked.Add(ToFirewallEntry(line));
                        /*
                        object toAdd;
                        IPAddress addr;
                        if( IPAddress.TryParse( line, out addr ) )
                        toAdd = addr;
                        else
                        toAdd = line;
                        m_Blocked.Add( toAdd.ToString() );
                        * */
                    }
                }
            }
        }

        public static List<IFirewallEntry> List
        {
            get
            {
                return m_Blocked;
            }
        }

        public static IFirewallEntry ToFirewallEntry(object entry)
        {
            if (entry is IFirewallEntry)
                return (IFirewallEntry)entry;
            else if (entry is IPAddress)
                return new IPFirewallEntry((IPAddress)entry);
            else if (entry is string)
                return ToFirewallEntry((string)entry);

            return null;
        }

        public static IFirewallEntry ToFirewallEntry(string entry)
        {
            IPAddress addr;

            if (IPAddress.TryParse(entry, out addr))
                return new IPFirewallEntry(addr);

            //Try CIDR parse
            string[] str = entry.Split('/');

            if (str.Length == 2)
            {
                IPAddress cidrPrefix;

                if (IPAddress.TryParse(str[0], out cidrPrefix))
                {
                    int cidrLength;

                    if (int.TryParse(str[1], out cidrLength))
                        return new CIDRFirewallEntry(cidrPrefix, cidrLength);
                }
            }

            return new WildcardIPFirewallEntry(entry);
        }

        public static void RemoveAt(int index)
        {
            m_Blocked.RemoveAt(index);
            Save();
        }

        public static void Remove(object obj)
        {
            IFirewallEntry entry = ToFirewallEntry(obj);

            if (entry != null)
            {
                m_Blocked.Remove(entry);
                Save();
            }
        }

        public static void Add(object obj)
        {
            if (obj is IPAddress)
                Add((IPAddress)obj);
            else if (obj is string)
                Add((string)obj);
            else if (obj is IFirewallEntry)
                Add((IFirewallEntry)obj);
        }

        public static void Add(IFirewallEntry entry)
        {
            if (!m_Blocked.Contains(entry))
                m_Blocked.Add(entry);

            Save();
        }

        public static void Add(string pattern)
        {
            IFirewallEntry entry = ToFirewallEntry(pattern);

            if (!m_Blocked.Contains(entry))
                m_Blocked.Add(entry);

            Save();
        }

        public static void Add(IPAddress ip)
        {
            IFirewallEntry entry = new IPFirewallEntry(ip);

            if (!m_Blocked.Contains(entry))
                m_Blocked.Add(entry);

            Save();
        }

        public static void Save()
        {
            string path = "firewall.cfg";

            using (StreamWriter op = new StreamWriter(path))
            {
                for (int i = 0; i < m_Blocked.Count; ++i)
                    op.WriteLine(m_Blocked[i]);
            }
        }

        public static bool IsBlocked(IPAddress ip)
        {
            for (int i = 0; i < m_Blocked.Count; i++)
            {
                if (m_Blocked[i].IsBlocked(ip))
                    return true;
            }

            return false;
            /*
            bool contains = false;
            for ( int i = 0; !contains && i < m_Blocked.Count; ++i )
            {
            if ( m_Blocked[i] is IPAddress )
            contains = ip.Equals( m_Blocked[i] );
            else if ( m_Blocked[i] is String )
            {
            string s = (string)m_Blocked[i];
            contains = Utility.IPMatchCIDR( s, ip );
            if( !contains )
            contains = Utility.IPMatch( s, ip );
            }
            }
            return contains;
            * */
        }
    }
}