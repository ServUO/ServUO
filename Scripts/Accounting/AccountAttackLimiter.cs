using Server.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Server.Accounting
{
    public class AccountAttackLimiter
    {
        public static bool Enabled = true;
        private static readonly List<InvalidAccountAccessLog> m_List = new List<InvalidAccountAccessLog>();
        public static void Initialize()
        {
            if (!Enabled)
                return;

            PacketHandlers.RegisterThrottler(0x80, Throttle_Callback);
            PacketHandlers.RegisterThrottler(0x91, Throttle_Callback);
            PacketHandlers.RegisterThrottler(0xCF, Throttle_Callback);
        }

        public static bool Throttle_Callback(NetState ns, out bool drop)
        {
            drop = false;

            InvalidAccountAccessLog accessLog = FindAccessLog(ns);

            if (accessLog == null)
                return true;

            return (DateTime.UtcNow >= (accessLog.LastAccessTime + ComputeThrottle(accessLog.Counts)));
        }

        public static InvalidAccountAccessLog FindAccessLog(NetState ns)
        {
            if (ns == null)
                return null;

            IPAddress ipAddress = ns.Address;

            for (int i = 0; i < m_List.Count; ++i)
            {
                InvalidAccountAccessLog accessLog = m_List[i];

                if (accessLog.HasExpired)
                    m_List.RemoveAt(i--);
                else if (accessLog.Address.Equals(ipAddress))
                    return accessLog;
            }

            return null;
        }

        public static void RegisterInvalidAccess(NetState ns)
        {
            if (ns == null || !Enabled)
                return;

            InvalidAccountAccessLog accessLog = FindAccessLog(ns);

            if (accessLog == null)
                m_List.Add(accessLog = new InvalidAccountAccessLog(ns.Address));

            accessLog.Counts += 1;
            accessLog.RefreshAccessTime();

            if (accessLog.Counts >= 3)
            {
                try
                {
                    using (StreamWriter op = new StreamWriter("throttle.log", true))
                    {
                        op.WriteLine(
                            "{0}\t{1}\t{2}",
                            DateTime.UtcNow,
                            ns,
                            accessLog.Counts);
                    }
                }
                catch (Exception e)
                {
                    Diagnostics.ExceptionLogging.LogException(e);
                }
            }
        }

        public static TimeSpan ComputeThrottle(int counts)
        {
            if (counts >= 15)
                return TimeSpan.FromMinutes(5.0);

            if (counts >= 10)
                return TimeSpan.FromMinutes(1.0);

            if (counts >= 5)
                return TimeSpan.FromSeconds(20.0);

            if (counts >= 3)
                return TimeSpan.FromSeconds(10.0);

            if (counts >= 1)
                return TimeSpan.FromSeconds(2.0);

            return TimeSpan.Zero;
        }
    }

    public class InvalidAccountAccessLog
    {
        private IPAddress m_Address;
        private DateTime m_LastAccessTime;
        private int m_Counts;
        public InvalidAccountAccessLog(IPAddress address)
        {
            m_Address = address;
            RefreshAccessTime();
        }

        public IPAddress Address
        {
            get
            {
                return m_Address;
            }
            set
            {
                m_Address = value;
            }
        }
        public DateTime LastAccessTime
        {
            get
            {
                return m_LastAccessTime;
            }
            set
            {
                m_LastAccessTime = value;
            }
        }
        public bool HasExpired => (DateTime.UtcNow >= (m_LastAccessTime + TimeSpan.FromHours(1.0)));
        public int Counts
        {
            get
            {
                return m_Counts;
            }
            set
            {
                m_Counts = value;
            }
        }
        public void RefreshAccessTime()
        {
            m_LastAccessTime = DateTime.UtcNow;
        }
    }
}
