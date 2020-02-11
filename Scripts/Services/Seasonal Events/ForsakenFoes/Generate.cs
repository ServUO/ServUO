using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Engines.CannedEvil;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Engines.Points;

namespace Server.Engines.Fellowship
{
	public static class ForsakenFoesGeneration
	{
        public static void Initialize()
        {
            if (Core.EJ)
            {
                EventSink.WorldSave += OnWorldSave;
            }
        }

        private static void OnWorldSave(WorldSaveEventArgs e)
        {
            CheckEnabled(true);
        }

        public static void CheckEnabled(bool timed = false)
        {
            var khaldun = PointsSystem.Khaldun;

            if (khaldun.Enabled && !khaldun.InSeason)
            {
                if (timed)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(30), () =>
                    {
                        Utility.WriteConsoleColor(ConsoleColor.Green, "Disabling Treasures of Khaldun");

                        Remove();
                        khaldun.Enabled = false;
                    });
                }
                else
                {
                    Utility.WriteConsoleColor(ConsoleColor.Green, "Auto Disabling Treasures of Khaldun");

                    Remove();
                    khaldun.Enabled = false;
                }
            }
            else if (!khaldun.Enabled && khaldun.InSeason)
            {
                if (timed)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(30), () =>
                    {
                        Utility.WriteConsoleColor(ConsoleColor.Green, "Enabling Treasures of Khaldun");

                        Generate();
                        khaldun.Enabled = true;
                    });
                }
                else
                {
                    Utility.WriteConsoleColor(ConsoleColor.Green, "Auto Enabling Treasures of Khaldun");

                    Generate();
                    khaldun.Enabled = true;
                }

                if(!khaldun.QuestContentGenerated)
                {
                    GenerateQuestContent();
                    khaldun.QuestContentGenerated = true;
                }
            }
        }

        public static void Generate()
        {
            if (TheFellowshipHouse.InstanceTram == null && !Siege.SiegeShard)
            {
                TheFellowshipHouse.InstanceTram = new TheFellowshipHouse();
                TheFellowshipHouse.InstanceTram.MoveToWorld(new Point3D(1720, 1562, 17), Map.Trammel);
            }

            if (TheFellowshipHouse.InstanceFel == null)
            {
                TheFellowshipHouse.InstanceFel = new TheFellowshipHouse();
                TheFellowshipHouse.InstanceFel.MoveToWorld(new Point3D(1720, 1562, 17), Map.Felucca);
            }

            if (FellowshipAdept.InstanceTram == null && !Siege.SiegeShard)
            {
                FellowshipAdept.InstanceTram = new FellowshipAdept();
                FellowshipAdept.InstanceTram.MoveToWorld(new Point3D(1711, 1570, 44), Map.Trammel);
            }

            if (FellowshipAdept.InstanceFel == null)
            {
                FellowshipAdept.InstanceFel = new FellowshipAdept();
                FellowshipAdept.InstanceFel.MoveToWorld(new Point3D(1711, 1570, 44), Map.Felucca);
            }

            if (Follower.InstanceTram == null && !Siege.SiegeShard)
            {
                Follower.InstanceTram = new Follower();
                Follower.InstanceTram.MoveToWorld(new Point3D(1720, 1565, 49), Map.Trammel);
            }

            if (Follower.InstanceFel == null)
            {
                Follower.InstanceFel = new Follower();
                Follower.InstanceFel.MoveToWorld(new Point3D(1720, 1565, 49), Map.Felucca);
            }

            if (EtherealSoulcleanser.InstanceTram == null && !Siege.SiegeShard)
            {
                EtherealSoulcleanser.InstanceTram = new EtherealSoulcleanser();
                EtherealSoulcleanser.InstanceTram.MoveToWorld(new Point3D(1721, 1565, 50), Map.Trammel);
            }

            if (EtherealSoulcleanser.InstanceFel == null)
            {
                EtherealSoulcleanser.InstanceFel = new EtherealSoulcleanser();
                EtherealSoulcleanser.InstanceFel.MoveToWorld(new Point3D(1721, 1565, 50), Map.Felucca);
            }

            if (FellowshipDonationBox.InstanceTram == null && !Siege.SiegeShard)
            {
                FellowshipDonationBox.InstanceTram = new FellowshipDonationBox();
                FellowshipDonationBox.InstanceTram.MoveToWorld(new Point3D(1721, 1554, 49), Map.Trammel);
            }

            if (FellowshipDonationBox.InstanceFel == null)
            {
                FellowshipDonationBox.InstanceFel = new FellowshipDonationBox();
                FellowshipDonationBox.InstanceFel.MoveToWorld(new Point3D(1721, 1554, 49), Map.Felucca);
            }

            if (TheFellowshipStaff.InstanceTram == null && !Siege.SiegeShard)
            {
                TheFellowshipStaff.InstanceTram = new TheFellowshipStaff();
                TheFellowshipStaff.InstanceTram.MoveToWorld(new Point3D(1718, 1559, 55), Map.Trammel);
            }

            if (TheFellowshipStaff.InstanceFel == null)
            {
                TheFellowshipStaff.InstanceFel = new TheFellowshipStaff();
                TheFellowshipStaff.InstanceFel.MoveToWorld(new Point3D(1718, 1559, 55), Map.Felucca);
            }

            Map map = !Siege.SiegeShard ? Map.Trammel : Map.Felucca;

            if (map.FindItem<Static>(new Point3D(1721, 1554, 54)) == null)
            {
                Static st = new Static(0x42BF)
                {
                    Hue = 1910,
                    Name = "The Fellowship Tome"
                };

                st.MoveToWorld(new Point3D(1721, 1554, 54), map);
            }

            if (map.FindItem<Static>(new Point3D(1721, 1555, 59)) == null)
            {
                Static st = new Static(0x2F60)
                {
                    Hue = 1912,
                    Name = "The Fellowship Coin"
                };

                st.MoveToWorld(new Point3D(1721, 1555, 59), map);
            }

            if (map.FindItem<Static>(new Point3D(1721, 1555, 49)) == null)
            {
                Static st = new Static(0x3155)
                {
                    Hue = 1912,
                    Name = "The Fellowship Icon"
                };

                st.MoveToWorld(new Point3D(1721, 1555, 49), map);
            }

            if (map.FindItem<Static>(new Point3D(1719, 1554, 49)) == null)
            {
                Candelabra st = new Candelabra()
                {
                    Hue = 1105
                };

                st.MoveToWorld(new Point3D(1719, 1554, 49), map);
            }

            if (map.FindItem<Static>(new Point3D(1722, 1554, 49)) == null)
            {
                Candelabra st = new Candelabra()
                {
                    Hue = 1105
                };

                st.MoveToWorld(new Point3D(1722, 1554, 49), map);
            }
        }

        public static void Remove()
        {
        }

        private static readonly Point3D[] blocker = new Point3D[] { new Point3D(6408, 2667, 0), new Point3D(6409, 2667, 0), new Point3D(6410, 2667, 0), new Point3D(6411, 2667, 0),
                                            new Point3D(6412, 2667, 0), new Point3D(6396, 2677, 0), new Point3D(6396, 2678, 0), new Point3D(6396, 2679, 0),
                                            new Point3D(6396, 2680, 0), new Point3D(6396, 2681, 0), new Point3D(6407, 2692, 0), new Point3D(6408, 2692, 0),
                                            new Point3D(6409, 2692, 0), new Point3D(6410, 2692, 0)};

        public static void GenerateQuestContent()
        {
            if (BlackthornEntry.InstanceTram == null && !Siege.SiegeShard)
            {
                BlackthornEntry.InstanceTram = new BlackthornEntry();
                BlackthornEntry.InstanceTram.MoveToWorld(new Point3D(6409, 2677, 0), Map.Trammel);
            }

            if (BlackthornStep2.InstanceTram == null && !Siege.SiegeShard)
            {
                BlackthornStep2.InstanceTram = new BlackthornStep2();
                BlackthornStep2.InstanceTram.MoveToWorld(new Point3D(6378, 2612, 0), Map.Trammel);
            }

            if (BlackthornEntry.InstanceFel == null)
            {
                BlackthornEntry.InstanceFel = new BlackthornEntry();
                BlackthornEntry.InstanceFel.MoveToWorld(new Point3D(6409, 2677, 0), Map.Felucca);
            }

            if (BlackthornStep2.InstanceFel == null)
            {
                BlackthornStep2.InstanceFel = new BlackthornStep2();
                BlackthornStep2.InstanceFel.MoveToWorld(new Point3D(6378, 2612, 0), Map.Felucca);
            }

            Map map = Map.Felucca;

            blocker.ToList().ForEach(x =>
            {
                if (map.FindItem<Blocker>(new Point3D(x)) == null)
                {
                    Blocker bl = new Blocker();

                    bl.MoveToWorld(new Point3D(x), map);
                }
            });

            if (Worker.InstanceFel == null)
            {
                Worker.InstanceFel = new Worker();
                Worker.InstanceFel.MoveToWorld(new Point3D(6406, 2680, 0), map);
            }

            map = Map.Trammel;

            blocker.ToList().ForEach(x =>
            {
                if (map.FindItem<Blocker>(new Point3D(x)) == null)
                {
                    Blocker bl = new Blocker();

                    bl.MoveToWorld(new Point3D(x), map);
                }
            });

            if (Worker.InstanceTram == null)
            {
                Worker.InstanceTram = new Worker();
                Worker.InstanceTram.MoveToWorld(new Point3D(6406, 2680, 0), map);
            }
        }
    }
}
