using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Server.Network;

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

            PacketHandlers.RegisterThrottler(0x80, new ThrottlePacketCallback(Throttle_Callback));
            PacketHandlers.RegisterThrottler(0x91, new ThrottlePacketCallback(Throttle_Callback));
            PacketHandlers.RegisterThrottler(0xCF, new ThrottlePacketCallback(Throttle_Callback));
        }

        public static bool Throttle_Callback(NetState ns)
        {
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
                catch
                {
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
            this.m_Address = address;
            this.RefreshAccessTime();
        }

        public IPAddress Address
        {
            get
            {
                return this.m_Address;
            }
            set
            {
                this.m_Address = value;
            }
        }
        public DateTime LastAccessTime
        {
            get
            {
                return this.m_LastAccessTime;
            }
            set
            {
                this.m_LastAccessTime = value;
            }
        }
        public bool HasExpired
        {
            get
            {
                return (DateTime.UtcNow >= (this.m_LastAccessTime + TimeSpan.FromHours(1.0)));
            }
        }
        public int Counts
        {
            get
            {
                return this.m_Counts;
            }
            set
            {
                this.m_Counts = value;
            }
        }
        public void RefreshAccessTime()
        {
            this.m_LastAccessTime = DateTime.UtcNow;
        }
    }
}