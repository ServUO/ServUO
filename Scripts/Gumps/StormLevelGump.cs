using Server.Commands;
using Server.Network;

using System;

namespace Server.Gumps
{
    public class StormLevelEntry
    {
        private readonly int m_Name;
        private readonly Point3D[] m_Locations;

        public StormLevelEntry(int name, Point3D[] locations)
        {
            m_Name = name;
            m_Locations = locations;
        }

        public int Name => m_Name;
        public Point3D[] Locations => m_Locations;
    }

    public class StormLevelGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("StormLevelGump", AccessLevel.Administrator, StormLevel_OnCommand);
            EventSink.Login += OnLogin;
        }

        private static void OnLogin(LoginEventArgs e)
        {
            var from = e.Mobile;

            if (((from.Map == Map.Trammel && from.Region.IsPartOf("Blackthorn Castle")) || Engines.Fellowship.ForsakenFoesEvent.Instance.Running && from.Region.IsPartOf("BlackthornDungeon") || from.Region.IsPartOf("Ver Lor Reg")) && from.Player && from.AccessLevel == AccessLevel.Player && from.CharacterOut)
            {
                var menu = new StormLevelGump(from);
                menu.BeginClose();
                from.SendGump(menu);
            }
        }

        [Usage("StormLevelGump")]
        public static void StormLevel_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (!from.HasGump(typeof(StormLevelGump)))
            {
                from.SendGump(new StormLevelGump(from));
            }
        }

        private static readonly StormLevelEntry[] m_Entries = new StormLevelEntry[]
        {
            // Britain
            new StormLevelEntry(1011028, new Point3D[]
            {
                new Point3D(1522, 1757, 28),
                new Point3D(1519, 1619, 10),
                new Point3D(1457, 1538, 30),
                new Point3D(1607, 1568, 20),
                new Point3D(1643, 1680, 18)
            }),

            // Trinsic
            new StormLevelEntry(1011029, new Point3D[]
            {
                new Point3D(2005, 2754, 30),
                new Point3D(1993, 2827, 0),
                new Point3D(2044, 2883, 0),
                new Point3D(1876, 2859, 20),
                new Point3D(1865, 2687, 0)
            }),

            // Vesper
            new StormLevelEntry(1011030, new Point3D[]
            {
                new Point3D(2973, 891, 0),
                new Point3D(3003, 776, 0),
                new Point3D(2910, 727, 0),
                new Point3D(2865, 804, 0),
                new Point3D(2832, 927, 0)
            }),

            // Minoc
            new StormLevelEntry(1011031, new Point3D[]
            {
                new Point3D(2498, 392, 0),
                new Point3D(2433, 541, 0),
                new Point3D(2445, 501, 15),
                new Point3D(2501, 469, 15),
                new Point3D(2444, 420, 15)
            }),

            // Jhelom ==
            new StormLevelEntry(1011343, new Point3D[]
            {
                new Point3D(490, 1166, 0),
                new Point3D(652, 1098, 0),
                new Point3D(650, 1013, 0),
                new Point3D(536, 979, 0),
                new Point3D(464, 970, 0)
            }),

            // Moonglow
            new StormLevelEntry(1011344, new Point3D[]
            {
                new Point3D(4444, 1061, 0),
                new Point3D(4443, 1066, 0),
                new Point3D(4443, 1050, 0),
                new Point3D(4444, 1071, 0),
                new Point3D(4444, 1052, 0)
            }),

            // Yew
            new StormLevelEntry(1011032, new Point3D[]
            {
                new Point3D(490, 1166, 0),
                new Point3D(652, 1098, 0),
                new Point3D(650, 1013, 0),
                new Point3D(536, 979, 0),
                new Point3D(464, 970, 0)
            }),

            // Magincia
            new StormLevelEntry(1016172, new Point3D[]
            {
                new Point3D(3669, 2099, 20),
                new Point3D(3689, 2235, 20),
                new Point3D(3680, 2067, 5),
                new Point3D(3772, 2116, 20),
                new Point3D(3660, 2122, 20),
            }),

            // Cove
            new StormLevelEntry(1011033, new Point3D[]
            {
                new Point3D(2230, 1159, 0),
                new Point3D(2218, 1203, 0),
                new Point3D(2247, 1194, 0),
                new Point3D(2236, 1224, 0),
                new Point3D(2273, 1231, 0)
            }),
            
            // Papua
            new StormLevelEntry(1011057, new Point3D[]
            {
                new Point3D(5720, 3109, -1),
                new Point3D(5677, 3176, -3),
                new Point3D(5678, 3227, 0),
                new Point3D(5769, 3206, -2),
                new Point3D(5777, 3270, -1)
            }),

            // Delucia
            new StormLevelEntry(1011058, new Point3D[]
            {
                new Point3D(5216, 4033, 37),
                new Point3D(5262, 4049, 37),
                new Point3D(5284, 4006, 37),
                new Point3D(5189, 3971, 39),
                new Point3D(5243, 3960, 37)
            }),

            // Skara Brae
            new StormLevelEntry(1011347, new Point3D[]
            {
                new Point3D(5216, 4033, 37),
                new Point3D(5262, 4049, 37),
                new Point3D(5284, 4006, 37),
                new Point3D(5189, 3971, 39),
                new Point3D(5243, 3960, 37)
            }),

            // Serpent's Hold
            new StormLevelEntry(1011348, new Point3D[]
            {
                new Point3D(5216, 4033, 37),
                new Point3D(5262, 4049, 37),
                new Point3D(5284, 4006, 37),
                new Point3D(5189, 3971, 39),
                new Point3D(5243, 3960, 37)
            })
        };

        private readonly Mobile m_Mobile;
        private Timer m_Timer;

        public StormLevelGump(Mobile m) : base(50, 50)
        {
            m_Mobile = m;

            Closable = false;
            Disposable = false;
            Dragable = true;

            AddPage(0);
            AddImage(0, 0, 206);
            AddImageTiled(44, 0, 292, 44, 201);
            AddImageTiled(0, 44, 42, 267, 202);
            AddImageTiled(337, 44, 42, 267, 203);
            AddImage(336, 0, 207);
            AddImage(0, 311, 204);
            AddImage(336, 311, 205);
            AddImageTiled(44, 311, 292, 44, 233);
            AddImageTiled(42, 44, 295, 267, 200);

            AddHtmlLocalized(49, 23, 200, 16, 1011043, false, false); // Magic Storm Level 5
            AddHtmlLocalized(23, 51, 338, 67, 1011044, true, true); // This area has become unstable and you must leave. If you do not select a town to go to one will be picked at random. Please select a town from the list below:

            StormLevelEntry[] entries = m_Entries;

            for (int i = 0; i < entries.Length; i++)
            {
                StormLevelEntry entry = entries[i];

                if (i < 8)
                {
                    AddButton(34, 125 + 20 * i, 208, 209, i + 1, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(65, 125 + 20 * i, 335, 40, entry.Name, false, false);
                }
                else
                {
                    AddButton(210, 125 + 20 * (i - 8), 208, 209, i + 1, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(241, 125 + 20 * (i - 8), 335, 40, entry.Name, false, false);
                }
            }
        }

        public void BeginClose()
        {
            StopClose();

            m_Timer = new CloseTimer(m_Mobile);
            m_Timer.Start();

            m_Mobile.Frozen = true;
        }

        public void StopClose()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            m_Mobile.Frozen = false;
        }

        private class CloseTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly DateTime m_End;
            public CloseTimer(Mobile m)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(1.0))
            {
                m_Mobile = m;
                m_End = DateTime.UtcNow + TimeSpan.FromSeconds(10.0);
            }

            protected override void OnTick()
            {
                if (m_Mobile.NetState == null || DateTime.UtcNow > m_End)
                {
                    m_Mobile.Frozen = false;
                    m_Mobile.CloseGump(typeof(StormLevelGump));

                    StormLevelEntry[] entries = m_Entries;
                    int id = Utility.Random(entries.Length);
                    int idx = Utility.Random(entries[id].Locations.Length);
                    Point3D dest = entries[id].Locations[idx];

                    GoTo(m_Mobile, dest);
                    Stop();
                }
                else
                {
                    m_Mobile.Frozen = true;
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            if (info.ButtonID != 0)
            {
                int index = info.ButtonID - 1;
                StormLevelEntry[] entries = m_Entries;

                if (index >= 0 && index < entries.Length)
                {
                    Point3D dest = entries[index].Locations[Utility.Random(entries[index].Locations.Length)];

                    GoTo(from, dest);
                    StopClose();
                }
            }
        }

        public static void GoTo(Mobile from, Point3D dest)
        {
            Map destMap;
            if (from.Map == Map.Trammel)
                destMap = Map.Trammel;
            else if (from.Map == Map.Felucca)
                destMap = Map.Felucca;
            else if (from.Map == Map.Internal)
                destMap = from.LogoutMap == Map.Felucca ? Map.Felucca : Map.Trammel;
            else
                destMap = from.Kills >= 5 ? Map.Felucca : Map.Trammel;

            Mobiles.BaseCreature.TeleportPets(from, dest, destMap);
            from.MoveToWorld(dest, destMap);
        }
    }
}
