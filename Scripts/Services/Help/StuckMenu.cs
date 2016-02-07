using System;
using Server.Gumps;
using Server.Network;

namespace Server.Menus.Questions
{
    public class StuckMenuEntry
    {
        private readonly int m_Name;
        private readonly Point3D[] m_Locations;
        public StuckMenuEntry(int name, Point3D[] locations)
        {
            this.m_Name = name;
            this.m_Locations = locations;
        }

        public int Name
        {
            get
            {
                return this.m_Name;
            }
        }
        public Point3D[] Locations
        {
            get
            {
                return this.m_Locations;
            }
        }
    }

    public class StuckMenu : Gump
    {
        private static readonly StuckMenuEntry[] m_Entries = new StuckMenuEntry[]
        {
            // Britain
            new StuckMenuEntry(1011028, new Point3D[]
            {
                new Point3D(1522, 1757, 28),
                new Point3D(1519, 1619, 10),
                new Point3D(1457, 1538, 30),
                new Point3D(1607, 1568, 20),
                new Point3D(1643, 1680, 18)
            }),

            // Trinsic
            new StuckMenuEntry(1011029, new Point3D[]
            {
                new Point3D(2005, 2754, 30),
                new Point3D(1993, 2827, 0),
                new Point3D(2044, 2883, 0),
                new Point3D(1876, 2859, 20),
                new Point3D(1865, 2687, 0)
            }),

            // Vesper
            new StuckMenuEntry(1011030, new Point3D[]
            {
                new Point3D(2973, 891, 0),
                new Point3D(3003, 776, 0),
                new Point3D(2910, 727, 0),
                new Point3D(2865, 804, 0),
                new Point3D(2832, 927, 0)
            }),

            // Minoc
            new StuckMenuEntry(1011031, new Point3D[]
            {
                new Point3D(2498, 392, 0),
                new Point3D(2433, 541, 0),
                new Point3D(2445, 501, 15),
                new Point3D(2501, 469, 15),
                new Point3D(2444, 420, 15)
            }),

            // Yew
            new StuckMenuEntry(1011032, new Point3D[]
            {
                new Point3D(490, 1166, 0),
                new Point3D(652, 1098, 0),
                new Point3D(650, 1013, 0),
                new Point3D(536, 979, 0),
                new Point3D(464, 970, 0)
            }),

            // Cove
            new StuckMenuEntry(1011033, new Point3D[]
            {
                new Point3D(2230, 1159, 0),
                new Point3D(2218, 1203, 0),
                new Point3D(2247, 1194, 0),
                new Point3D(2236, 1224, 0),
                new Point3D(2273, 1231, 0)
            })
        };
        private static readonly StuckMenuEntry[] m_T2AEntries = new StuckMenuEntry[]
        {
            // Papua
            new StuckMenuEntry(1011057, new Point3D[]
            {
                new Point3D(5720, 3109, -1),
                new Point3D(5677, 3176, -3),
                new Point3D(5678, 3227, 0),
                new Point3D(5769, 3206, -2),
                new Point3D(5777, 3270, -1)
            }),

            // Delucia
            new StuckMenuEntry(1011058, new Point3D[]
            {
                new Point3D(5216, 4033, 37),
                new Point3D(5262, 4049, 37),
                new Point3D(5284, 4006, 37),
                new Point3D(5189, 3971, 39),
                new Point3D(5243, 3960, 37)
            })
        };
        private readonly Mobile m_Mobile;
        private readonly Mobile m_Sender;
        private readonly bool m_MarkUse;
        private Timer m_Timer;
        public StuckMenu(Mobile beholder, Mobile beheld, bool markUse)
            : base(150, 50)
        {
            this.m_Sender = beholder;
            this.m_Mobile = beheld;
            this.m_MarkUse = markUse;

            this.Closable = false; 
            this.Dragable = false; 
            this.Disposable = false;

            this.AddBackground(0, 0, 270, 320, 2600);

            this.AddHtmlLocalized(50, 20, 250, 35, 1011027, false, false); // Chose a town:

            StuckMenuEntry[] entries = IsInSecondAgeArea(beheld) ? m_T2AEntries : m_Entries;

            for (int i = 0; i < entries.Length; i++)
            {
                StuckMenuEntry entry = entries[i];

                this.AddButton(50, 55 + 35 * i, 208, 209, i + 1, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(75, 55 + 35 * i, 335, 40, entry.Name, false, false);
            }

            this.AddButton(55, 263, 4005, 4007, 0, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(90, 265, 200, 35, 1011012, false, false); // CANCEL
        }

        public void BeginClose()
        {
            this.StopClose();

            this.m_Timer = new CloseTimer(this.m_Mobile);
            this.m_Timer.Start();

            this.m_Mobile.Frozen = true;
        }

        public void StopClose()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Mobile.Frozen = false;
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            this.StopClose();

            if (Factions.Sigil.ExistsOn(this.m_Mobile))
            {
                this.m_Mobile.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
            }
            else if (info.ButtonID == 0)
            {
                if (this.m_Mobile == this.m_Sender)
                    this.m_Mobile.SendLocalizedMessage(1010588); // You choose not to go to any city.
            }
            else
            {
                int index = info.ButtonID - 1;
                StuckMenuEntry[] entries = IsInSecondAgeArea(this.m_Mobile) ? m_T2AEntries : m_Entries;

                if (index >= 0 && index < entries.Length)
                    this.Teleport(entries[index]);
            }
        }

        private static bool IsInSecondAgeArea(Mobile m)
        {
            if (m.Map != Map.Trammel && m.Map != Map.Felucca)
                return false;

            if (m.X >= 5120 && m.Y >= 2304)
                return true;

            if (m.Region.IsPartOf("Terathan Keep"))
                return true;

            return false;
        }

        private void Teleport(StuckMenuEntry entry)
        {
            if (this.m_MarkUse) 
            {
                this.m_Mobile.SendLocalizedMessage(1010589); // You will be teleported within the next two minutes.

                new TeleportTimer(this.m_Mobile, entry, TimeSpan.FromSeconds(10.0 + (Utility.RandomDouble() * 110.0))).Start();

                this.m_Mobile.UsedStuckMenu();
            }
            else
            {
                new TeleportTimer(this.m_Mobile, entry, TimeSpan.Zero).Start();
            }
        }

        private class CloseTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly DateTime m_End;
            public CloseTimer(Mobile m)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(1.0))
            {
                this.m_Mobile = m;
                this.m_End = DateTime.UtcNow + TimeSpan.FromMinutes(3.0);
            }

            protected override void OnTick()
            {
                if (this.m_Mobile.NetState == null || DateTime.UtcNow > this.m_End)
                {
                    this.m_Mobile.Frozen = false;
                    this.m_Mobile.CloseGump(typeof(StuckMenu));

                    this.Stop();
                }
                else
                {
                    this.m_Mobile.Frozen = true;
                }
            }
        }

        private class TeleportTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly StuckMenuEntry m_Destination;
            private readonly DateTime m_End;
            public TeleportTimer(Mobile mobile, StuckMenuEntry destination, TimeSpan delay)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(1.0))
            {
                this.Priority = TimerPriority.TwoFiftyMS;

                this.m_Mobile = mobile;
                this.m_Destination = destination;
                this.m_End = DateTime.UtcNow + delay;
            }

            protected override void OnTick()
            {
                if (DateTime.UtcNow < this.m_End)
                {
                    this.m_Mobile.Frozen = true;
                }
                else
                {
                    this.m_Mobile.Frozen = false;
                    this.Stop();

                    if (Factions.Sigil.ExistsOn(this.m_Mobile))
                    {
                        this.m_Mobile.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
                        return;
                    }

                    int idx = Utility.Random(this.m_Destination.Locations.Length);
                    Point3D dest = this.m_Destination.Locations[idx];

                    Map destMap;
                    if (this.m_Mobile.Map == Map.Trammel)
                        destMap = Map.Trammel;
                    else if (this.m_Mobile.Map == Map.Felucca)
                        destMap = Map.Felucca;
                    else
                        destMap = this.m_Mobile.Kills >= 5 ? Map.Felucca : Map.Trammel;

                    Mobiles.BaseCreature.TeleportPets(this.m_Mobile, dest, destMap);
                    this.m_Mobile.MoveToWorld(dest, destMap);
                }
            }
        }
    }
}