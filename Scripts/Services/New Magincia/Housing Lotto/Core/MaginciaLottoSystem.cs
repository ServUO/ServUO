using System;
using Server;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Commands;
using Server.Accounting;
using Server.Multis;
using System.Linq;

namespace Server.Engines.NewMagincia
{
    public class MaginciaLottoSystem : Item
    {
        public static readonly TimeSpan DefaultLottoDuration = TimeSpan.FromDays(30);
        public static readonly int WritExpirePeriod = 30;
        public static readonly bool AutoResetLotto = false;

        private static int m_GoldSink;
        private static MaginciaLottoSystem m_Instance;
        public static MaginciaLottoSystem Instance { get { return m_Instance; } set { m_Instance = value; } }

        private static List<MaginciaHousingPlot> m_Plots = new List<MaginciaHousingPlot>();
        public static List<MaginciaHousingPlot> Plots { get { return m_Plots; } }

        private static Dictionary<Map, List<Rectangle2D>> m_FreeHousingZones;
        public static Dictionary<Map, List<Rectangle2D>> FreeHousingZones { get { return m_FreeHousingZones; } }

        private static Dictionary<Mobile, List<NewMaginciaMessage>> m_MessageQueue = new Dictionary<Mobile, List<NewMaginciaMessage>>();
        public static Dictionary<Mobile, List<NewMaginciaMessage>> MessageQueue { get { return m_MessageQueue; } }

        private Timer m_Timer;
        private TimeSpan m_LottoDuration;
        private bool m_Enabled;

        public static void Initialize()
        {
            EventSink.Login += new LoginEventHandler(OnLogin);

            if (m_Instance != null)
                m_Instance.CheckMessages();
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ResetAuctions
        {
            get { return false; }
            set
            {
                if (value == true)
                {
                    foreach (MaginciaHousingPlot plot in m_Plots)
                    {
                        if(plot.IsAvailable)
                            plot.LottoEnds = DateTime.UtcNow + m_LottoDuration;
                    }
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Enabled
        {
            get { return m_Enabled; }
            set
            {
                if (m_Enabled != value)
                {
                    if (value)
                    {
                        StartTimer();
                    }
                    else
                    {
                        EndTimer();
                    }
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public static int GoldSink { get { return m_GoldSink; } set { m_GoldSink = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan LottoDuration { get { return m_LottoDuration; } set { m_LottoDuration = value; } }

        public MaginciaLottoSystem() : base(3240)
        {
            Movable = false;
            m_Enabled = true;
            m_LottoDuration = DefaultLottoDuration;

            m_FreeHousingZones = new Dictionary<Map, List<Rectangle2D>>();
            m_FreeHousingZones[Map.Trammel] = new List<Rectangle2D>();
            m_FreeHousingZones[Map.Felucca] = new List<Rectangle2D>();

            if(m_Enabled)
                StartTimer();

            LoadPlots();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player)
            {
                from.CloseGump(typeof(LottoTrackingGump));
                from.CloseGump(typeof(PlotTrackingGump));
                from.SendGump(new LottoTrackingGump());
            }
        }

        public void StartTimer()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), new TimerCallback(ProcessTick));
            m_Timer.Priority = TimerPriority.OneMinute;
            m_Timer.Start();
        }

        public void EndTimer()
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }
        }

        public void ProcessTick()
        {
            List<MaginciaHousingPlot> plots = new List<MaginciaHousingPlot>(m_Plots);

            foreach (MaginciaHousingPlot plot in plots)
            {
                if (plot.IsAvailable && plot.LottoEnds != DateTime.MinValue && DateTime.UtcNow > plot.LottoEnds)
                    plot.EndLotto();

                if (plot.Expires != DateTime.MinValue && plot.Expires < DateTime.UtcNow)
                {
                    if (plot.Writ != null)
                        plot.Writ.OnExpired();
                    else
                        UnregisterPlot(plot);
                }
            }

            ColUtility.Free(plots);

            if (m_Plots.Count == 0)
                EndTimer();
        }

        public void CheckMessages()
        {
            List<Mobile> mobiles = new List<Mobile>(m_MessageQueue.Keys);

            foreach (Mobile m in mobiles)
            {
                List<NewMaginciaMessage> messages = new List<NewMaginciaMessage>(m_MessageQueue[m]);

                foreach (NewMaginciaMessage message in messages)
                {
                    if (m_MessageQueue.ContainsKey(m) && m_MessageQueue[m].Contains(message) && message.Expired)
                        m_MessageQueue[m].Remove(message);
                }
            }

            ColUtility.Free(mobiles);
        }

        public override void Delete()
        {
        }

        public static void RegisterPlot(MaginciaHousingPlot plot)
        {
            m_Plots.Add(plot);
        }

        public static void UnregisterPlot(MaginciaHousingPlot plot)
        {
            if (plot == null)
                return;

            if (plot.Stone != null && !plot.Stone.Deleted)
                plot.Stone.Delete();

            if (m_Plots.Contains(plot))
                m_Plots.Remove(plot);

            if (plot.Map != null && m_FreeHousingZones.ContainsKey(plot.Map) && !m_FreeHousingZones[plot.Map].Contains(plot.Bounds))
                m_FreeHousingZones[plot.Map].Add(plot.Bounds);
        }

        public static bool IsRegisteredPlot(MaginciaHousingPlot plot)
        {
            return m_Plots.Contains(plot);
        }

        public static bool IsFreeHousingZone(Point3D p, Map map)
        {
            if (!m_FreeHousingZones.ContainsKey(map))
                return false;

            foreach (Rectangle2D rec in m_FreeHousingZones[map])
            {
                if (rec.Contains(p))
                    return true;
            }

            return false;
        }

        public static void CheckHousePlacement(Mobile from, Point3D center)
        {
            MaginciaLottoSystem system = MaginciaLottoSystem.Instance;

            if (system != null && system.Enabled && from.Backpack != null && IsInMagincia(center.X, center.Y, from.Map))
            {
                List<Item> items = new List<Item>();

                Item[] packItems = from.Backpack.FindItemsByType(typeof(WritOfLease));
                Item[] bankItems = from.BankBox.FindItemsByType(typeof(WritOfLease));

                if (packItems != null && packItems.Length > 0)
                    items.AddRange(packItems);

                if (bankItems != null && bankItems.Length > 0)
                    items.AddRange(bankItems);

                foreach (Item item in items)
                {
                    WritOfLease lease = item as WritOfLease;

                    if (lease != null && !lease.Expired && lease.Plot != null && lease.Plot.Bounds.Contains(center) && from.Map == lease.Plot.Map)
                    {
                        lease.OnExpired();
                        return;
                    }
                }
            }
        }

        public static bool IsInMagincia(int x, int y, Map map)
        {
            return x > 3614 && x < 3817 && y > 2031 && y < 2274 && (map == Map.Trammel || map == Map.Felucca);
        }

        private void LoadPlots()
        {
            for (int i = 0; i < m_MagHousingZones.Length; i++)
            {
                bool prime = (i > 0 && i < 6) || i > 14;

                MaginciaHousingPlot tramplot = new MaginciaHousingPlot(m_Identifiers[i], m_MagHousingZones[i], prime, Map.Trammel);
                MaginciaHousingPlot felplot = new MaginciaHousingPlot(m_Identifiers[i], m_MagHousingZones[i], prime, Map.Felucca);

                RegisterPlot(tramplot);
                RegisterPlot(felplot);

                tramplot.AddPlotStone(m_StoneLocs[i]);
                tramplot.LottoEnds = DateTime.UtcNow + m_LottoDuration;

                felplot.AddPlotStone(m_StoneLocs[i]);
                felplot.LottoEnds = DateTime.UtcNow + m_LottoDuration;
            }
        }

        public static Rectangle2D[] MagHousingZones { get { return m_MagHousingZones; } }
        private static Rectangle2D[] m_MagHousingZones = new Rectangle2D[]
        {
            new Rectangle2D(3686, 2125, 18, 18), // C1
            new Rectangle2D(3686, 2086, 18, 18), // C2 / Prime
            new Rectangle2D(3686, 2063, 18, 18), // C3 / Prime

            new Rectangle2D(3657, 2036, 18, 18), // N1 / Prime 
            new Rectangle2D(3648, 2058, 18, 18), // N2 / Prime
            new Rectangle2D(3636, 2081, 18, 18), // N3 / Prime

            new Rectangle2D(3712, 2123, 16, 16), // SE3
            new Rectangle2D(3712, 2151, 18, 16), // SE2
            new Rectangle2D(3712, 2172, 18, 16), // SE1
            new Rectangle2D(3729, 2135, 16, 16), // SE4

            new Rectangle2D(3655, 2213, 18, 18), // SW1        
            new Rectangle2D(3656, 2191, 18, 16), // SW2
            new Rectangle2D(3628, 2197, 20, 20), // SW3        
            new Rectangle2D(3628, 2175, 18, 18), // SW4
            new Rectangle2D(3657, 2165, 18, 18), // SW5   

            new Rectangle2D(3745, 2122, 16, 18), // E1 / Prime       
            new Rectangle2D(3765, 2122, 18, 18), // E2 / Prime
            new Rectangle2D(3787, 2130, 18, 18), // E3 / Prime       
            new Rectangle2D(3784, 2108, 18, 17), // E4 / Prime
            new Rectangle2D(3765, 2086, 18, 18), // E5 / Prime      
            new Rectangle2D(3749, 2065, 18, 18), // E6 / Prime
            new Rectangle2D(3715, 2090, 18, 18), // E7 / Prime            
        };

        private static Point3D[] m_StoneLocs = new Point3D[]
        {
            new Point3D(3683, 2134, 20),
            new Point3D(3704, 2092, 5),
            new Point3D(3704, 2069, 5),

            new Point3D(3677, 2045, 20),
            new Point3D(3667, 2065, 20),
            new Point3D(3644, 2099, 20),

            new Point3D(3711, 2131, 20),
            new Point3D(3711, 2160, 20),
            new Point3D(3711, 2180, 20),
            new Point3D(3735, 2133, 20),

            new Point3D(3676, 2220, 20),
            new Point3D(3675, 2198, 20),
            new Point3D(3647, 2205, 22),
            new Point3D(3647, 2184, 21),
            new Point3D(3665, 2183, 22),

            new Point3D(3753, 2119, 21),
            new Point3D(3772, 2119, 21),
            new Point3D(3785, 2127, 25),
            new Point3D(3790, 2106, 30),
            new Point3D(3761, 2090, 20),
            new Point3D(3746, 2064, 23),
            new Point3D(3711, 2087, 5)
        };

        private static string[] m_Identifiers = new string[]
        {
            "C-1",
            "C-2",
            "C-3",

            "N-1",
            "N-2",
            "N-3",

            "SE-1",
            "SE-2",
            "SE-3",
            "SE-4",

            "SW-1",
            "SW-2",
            "SW-3",
            "SW-4",
            "SW-5",

            "E-1",
            "E-2",
            "E-3",
            "E-4",
            "E-5",
            "E-6",
            "E-7"
        };

        public static Point3D GetPlotStoneLoc(MaginciaHousingPlot plot)
        {
            if (plot == null)
                return Point3D.Zero;

            for (int i = 0; i < m_Identifiers.Length; i++)
            {
                if (m_Identifiers[i] == plot.Identifier)
                    return m_StoneLocs[i];
            }

            int z = plot.Map.GetAverageZ(plot.Bounds.X - 1, plot.Bounds.Y - 1);
            return new Point3D(plot.Bounds.X - 1, plot.Bounds.Y - 1, z);
        }

        public static string FormatSextant(MaginciaHousingPlot plot)
        {
            int z = plot.Map.GetAverageZ(plot.Bounds.X, plot.Bounds.Y);
            Point3D p = new Point3D(plot.Bounds.X, plot.Bounds.Y, z);

            return FormatSextant(p, plot.Map);
        }

        public static string FormatSextant(Point3D p, Map map)
        {
            int xLong = 0, yLat = 0;
            int xMins = 0, yMins = 0;
            bool xEast = false, ySouth = false;

            if (Sextant.Format(p, map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth))
            {
                return String.Format("{0}° {1}'{2}, {3}° {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W");
            }

            return p.ToString();
        }

        #region Messages
        public static void SendMessageTo(Mobile from, NewMaginciaMessage message)
        {
            if (from == null || message == null)
                return;

            AddMessageToQueue(from, message);

            if (from.NetState != null)
            {
                if(from.HasGump(typeof(NewMaginciaMessageGump)))
                    from.CloseGump(typeof(NewMaginciaMessageGump));

                if (HasMessageInQueue(from))
                    from.SendGump(new NewMaginciaMessageGump(from, m_MessageQueue[from][0]));
            }
        }

        public static void AddMessageToQueue(Mobile from, NewMaginciaMessage message)
        {
            if (!MessageQueue.ContainsKey(from) || m_MessageQueue[from] == null)
                m_MessageQueue[from] = new List<NewMaginciaMessage>();

            m_MessageQueue[from].Add(message);
        }

        public static void RemoveMessageFromQueue(Mobile from, NewMaginciaMessage message)
        {
            if (from == null || message == null)
                return;

            if (m_MessageQueue.ContainsKey(from) && m_MessageQueue[from].Contains(message))
            {
                m_MessageQueue[from].Remove(message);

                if (m_MessageQueue[from].Count == 0)
                    m_MessageQueue.Remove(from);
            }
        }

        public static bool HasMessageInQueue(Mobile from)
        {
            return from != null && m_MessageQueue.ContainsKey(from) && m_MessageQueue[from] != null && m_MessageQueue[from].Count > 0;
        }

        public static void OnLogin(LoginEventArgs e)
        {
            Mobile from = e.Mobile;
            Account acct = from.Account as Account;
            CheckMessages(from);

            if (acct == null)
                return;

            for (int i = 0; i < acct.Length; i++)
            {
                Mobile m = acct[i];

                if (m == null)
                    continue;

                if (m_MessageQueue.ContainsKey(m))
                {
                    if (m_MessageQueue[m] == null || m_MessageQueue[m].Count == 0)
                        m_MessageQueue.Remove(m);
                    else if (m_MessageQueue[m].Count > 0)
                    {
                        from.CloseGump(typeof(NewMaginciaMessageGump));
                        from.SendGump(new NewMaginciaMessageGump(m, m_MessageQueue[m][0]));
                    }
                }
            }

            GetWinnerGump(from);
        }

        public static void GetWinnerGump(Mobile from)
        {
            Account acct = from.Account as Account;

            if (acct == null)
                return;

            for (int i = 0; i < acct.Length; i++)
            {
                Mobile m = acct[i];

                if (m == null)
                    continue;

                foreach (MaginciaHousingPlot plot in m_Plots)
                {
                    if (plot.Expires != DateTime.MinValue && plot.Winner == m)
                    {
                        from.SendGump(new PlotWinnerGump(plot));
                        return;
                    }
                }
            }
        }

        public static void CheckMessages(Mobile from)
        {
            if (!m_MessageQueue.ContainsKey(from) || m_MessageQueue[from] == null || m_MessageQueue[from].Count == 0)
                return;

            List<NewMaginciaMessage> list = new List<NewMaginciaMessage>(m_MessageQueue[from]);

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Expired)
                    m_MessageQueue[from].Remove(list[i]);
            }
        }

        #endregion

        public MaginciaLottoSystem(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(m_GoldSink);
            writer.Write(m_Enabled);
            writer.Write(m_LottoDuration);

            writer.Write(m_Plots.Count);
            for (int i = 0; i < m_Plots.Count; i++)
                m_Plots[i].Serialize(writer);

            writer.Write(m_FreeHousingZones[Map.Trammel].Count);
            foreach(Rectangle2D rec in m_FreeHousingZones[Map.Trammel])
                writer.Write(rec);

            writer.Write(m_FreeHousingZones[Map.Felucca].Count);
            foreach (Rectangle2D rec in m_FreeHousingZones[Map.Felucca])
                writer.Write(rec);

            writer.Write(m_MessageQueue.Count);
            foreach(KeyValuePair<Mobile, List<NewMaginciaMessage>> kvp in m_MessageQueue)
            {
                writer.Write(kvp.Key);

                writer.Write(kvp.Value.Count);
                foreach(NewMaginciaMessage message in kvp.Value)
                    message.Serialize(writer);
            }

            Timer.DelayCall(TimeSpan.FromSeconds(30), new TimerCallback(CheckMessages));
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_FreeHousingZones = new Dictionary<Map, List<Rectangle2D>>();
            m_FreeHousingZones[Map.Trammel] = new List<Rectangle2D>();
            m_FreeHousingZones[Map.Felucca] = new List<Rectangle2D>();

            m_GoldSink = reader.ReadInt();
            m_Enabled = reader.ReadBool();
            m_LottoDuration = reader.ReadTimeSpan();

            int c = reader.ReadInt();
            for (int i = 0; i < c; i++)
                RegisterPlot(new MaginciaHousingPlot(reader));

            c = reader.ReadInt();
            for (int i = 0; i < c; i++)
                m_FreeHousingZones[Map.Trammel].Add(reader.ReadRect2D());

            c = reader.ReadInt();
            for (int i = 0; i < c; i++)
                m_FreeHousingZones[Map.Felucca].Add(reader.ReadRect2D());

            c = reader.ReadInt();
            for (int i = 0; i < c; i++)
            {
                Mobile m = reader.ReadMobile();
                List<NewMaginciaMessage> messages = new List<NewMaginciaMessage>();

                int count = reader.ReadInt();
                for(int j = 0; j < count; j++)
                    messages.Add(new NewMaginciaMessage(reader));

                if (m != null && messages.Count > 0)
                    m_MessageQueue[m] = messages;
            }

            if (m_Enabled)
                StartTimer();

            m_Instance = this;

            Timer.DelayCall(ValidatePlots);
        }

        public void ValidatePlots()
        {
            for(int i = 0; i < m_Identifiers.Length; i++)
            {
                var rec = m_MagHousingZones[i];
                var id = m_Identifiers[i];

                var plotTram = m_Plots.FirstOrDefault(p => p.Identifier == id && p.Map == Map.Trammel);
                var plotFel = m_Plots.FirstOrDefault(p => p.Identifier == id && p.Map == Map.Felucca);

                if (plotTram == null && !m_FreeHousingZones[Map.Trammel].Contains(rec))
                {
                    Console.WriteLine("Adding {0} to Magincia Free Housing Zone.[{1}]", rec, "Plot non-existent");
                    m_FreeHousingZones[Map.Trammel].Add(rec);
                }
                else if (plotTram != null && plotTram.Stone == null && (plotTram.Writ == null || plotTram.Writ.Expired))
                {
                    Console.WriteLine("Adding {0} to Magincia Free Housing Zone.[{1}]", rec, "Plot existed, writ expired");
                    UnregisterPlot(plotTram);
                }

                if (plotFel == null && !m_FreeHousingZones[Map.Felucca].Contains(rec))
                {
                    Console.WriteLine("Adding {0} to Magincia Free Housing Zone.[{1}]", rec, "Plot non-existent");
                    m_FreeHousingZones[Map.Felucca].Add(rec);
                }
                else if (plotFel != null && plotFel.Stone == null && (plotFel.Writ == null || plotFel.Writ.Expired))
                {
                    Console.WriteLine("Adding {0} to Magincia Free Housing Zone.[{1}]", rec, "Plot existed, writ expired");
                    UnregisterPlot(plotFel);
                }
            }
        }
    }
}
