using System;
using System.Linq;

using Server;
using Server.Items;
using Server.Engines.Points;
using Server.Mobiles;

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
            var fellowship = PointsSystem.FellowshipData;

            if (fellowship.Enabled && !fellowship.InSeason)
            {
                if (timed)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(30), () =>
                    {
                        Utility.WriteConsoleColor(ConsoleColor.Green, "Disabling Treasures of Khaldun");

                        Remove();
                        fellowship.Enabled = false;
                    });
                }
                else
                {
                    Utility.WriteConsoleColor(ConsoleColor.Green, "Auto Disabling Treasures of Khaldun");

                    Remove();
                    fellowship.Enabled = false;
                }
            }
            else if (!fellowship.Enabled && fellowship.InSeason)
            {
                if (timed)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(30), () =>
                    {
                        Utility.WriteConsoleColor(ConsoleColor.Green, "Enabling Treasures of Khaldun");

                        Generate();
                        fellowship.Enabled = true;

                        if (!fellowship.QuestContentGenerated)
                        {
                            GenerateQuestContent();
                            fellowship.QuestContentGenerated = true;
                        }
                    });
                }
                else
                {
                    Utility.WriteConsoleColor(ConsoleColor.Green, "Auto Enabling Treasures of Khaldun");

                    Generate();
                    fellowship.Enabled = true;

                    if (!fellowship.QuestContentGenerated)
                    {
                        GenerateQuestContent();
                        fellowship.QuestContentGenerated = true;
                    }
                }                
            }
        }

        public static void Generate()
        {
            XmlSpawner sp;

            if (!Siege.SiegeShard)
            {
                if (TheFellowshipHouse.InstanceTram == null)
                {
                    TheFellowshipHouse.InstanceTram = new TheFellowshipHouse();
                    TheFellowshipHouse.InstanceTram.MoveToWorld(new Point3D(1720, 1562, 17), Map.Trammel);
                }

                if (FellowshipAdept.InstanceTram == null)
                {
                    FellowshipAdept.InstanceTram = new FellowshipAdept();
                    FellowshipAdept.InstanceTram.MoveToWorld(new Point3D(1711, 1570, 44), Map.Trammel);
                }

                if (Follower.InstanceTram == null)
                {
                    Follower.InstanceTram = new Follower();
                    Follower.InstanceTram.MoveToWorld(new Point3D(1720, 1565, 49), Map.Trammel);
                }

                if (EtherealSoulcleanser.InstanceTram == null)
                {
                    EtherealSoulcleanser.InstanceTram = new EtherealSoulcleanser();
                    EtherealSoulcleanser.InstanceTram.MoveToWorld(new Point3D(1721, 1565, 50), Map.Trammel);
                }

                if (FellowshipDonationBox.InstanceTram == null)
                {
                    FellowshipDonationBox.InstanceTram = new FellowshipDonationBox();
                    FellowshipDonationBox.InstanceTram.MoveToWorld(new Point3D(1721, 1554, 49), Map.Trammel);
                }

                if (TheFellowshipStaff.InstanceTram == null)
                {
                    TheFellowshipStaff.InstanceTram = new TheFellowshipStaff();
                    TheFellowshipStaff.InstanceTram.MoveToWorld(new Point3D(1718, 1559, 55), Map.Trammel);
                }

                sp = new XmlSpawner("MiningCooperativeMerchant")
                {
                    SpawnRange = 1,
                    HomeRange = 5
                };

                sp.MoveToWorld(new Point3D(2497, 432, 15), Map.Trammel);
                sp.Respawn();

                OtherDecoration(Map.Trammel);
            }

            if (TheFellowshipHouse.InstanceFel == null)
            {
                TheFellowshipHouse.InstanceFel = new TheFellowshipHouse();
                TheFellowshipHouse.InstanceFel.MoveToWorld(new Point3D(1720, 1562, 17), Map.Felucca);
            }

            if (FellowshipAdept.InstanceFel == null)
            {
                FellowshipAdept.InstanceFel = new FellowshipAdept();
                FellowshipAdept.InstanceFel.MoveToWorld(new Point3D(1711, 1570, 44), Map.Felucca);
            }

            if (Follower.InstanceFel == null)
            {
                Follower.InstanceFel = new Follower();
                Follower.InstanceFel.MoveToWorld(new Point3D(1720, 1565, 49), Map.Felucca);
            }

            if (EtherealSoulcleanser.InstanceFel == null)
            {
                EtherealSoulcleanser.InstanceFel = new EtherealSoulcleanser();
                EtherealSoulcleanser.InstanceFel.MoveToWorld(new Point3D(1721, 1565, 50), Map.Felucca);
            }

            if (FellowshipDonationBox.InstanceFel == null)
            {
                FellowshipDonationBox.InstanceFel = new FellowshipDonationBox();
                FellowshipDonationBox.InstanceFel.MoveToWorld(new Point3D(1721, 1554, 49), Map.Felucca);
            }

            if (TheFellowshipStaff.InstanceFel == null)
            {
                TheFellowshipStaff.InstanceFel = new TheFellowshipStaff();
                TheFellowshipStaff.InstanceFel.MoveToWorld(new Point3D(1718, 1559, 55), Map.Felucca);
            }

            sp = new XmlSpawner("MiningCooperativeMerchant")
            {
                SpawnRange = 1,
                HomeRange = 5
            };

            sp.MoveToWorld(new Point3D(2497, 432, 15), Map.Felucca);
            sp.Respawn();

            OtherDecoration(Map.Felucca);
        }

        public static void OtherDecoration(Map map)
        {
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

        private static readonly Point3D[] blocker = new Point3D[]
        {
            new Point3D(6408, 2667, 0), new Point3D(6409, 2667, 0), new Point3D(6410, 2667, 0), new Point3D(6411, 2667, 0),
            new Point3D(6412, 2667, 0), new Point3D(6396, 2677, 0), new Point3D(6396, 2678, 0), new Point3D(6396, 2679, 0),
            new Point3D(6396, 2680, 0), new Point3D(6396, 2681, 0), new Point3D(6407, 2692, 0), new Point3D(6408, 2692, 0),
            new Point3D(6409, 2692, 0), new Point3D(6410, 2692, 0), new Point3D(6370, 2744, 0), new Point3D(6371, 2744, 0),
            new Point3D(6372, 2744, 0), new Point3D(6373, 2744, 0), new Point3D(6374, 2744, 0), new Point3D(6375, 2744, 0),
            new Point3D(6331, 2653, 0), new Point3D(6332, 2653, 0), new Point3D(6333, 2653, 0), new Point3D(6410, 2783, 0),
            new Point3D(6305, 2678, 0), new Point3D(6305, 2679, 0), new Point3D(6305, 2680, 0), new Point3D(6411, 2783, 0),
            new Point3D(6422, 2708, 0), new Point3D(6422, 2709, 0), new Point3D(6422, 2710, 0), new Point3D(6422, 2711, 0),
            new Point3D(6406, 2783, 0), new Point3D(6407, 2783, 0), new Point3D(6408, 2783, 0), new Point3D(6409, 2783, 0),
            new Point3D(6442, 2744, 0), new Point3D(6443, 2744, 0), new Point3D(6444, 2744, 0), new Point3D(6445, 2744, 0),
            new Point3D(6446, 2744, 0), new Point3D(6390, 2617, 0), new Point3D(6390, 2618, 0), new Point3D(6390, 2619, 0),
            new Point3D(6390, 2620, 0), new Point3D(6390, 2621, 0), new Point3D(6390, 2622, 0), new Point3D(6390, 2623, 0),
            new Point3D(6390, 2624, 0), new Point3D(6390, 2625, 0)
        };

        private static readonly Point3D[] Workers = new Point3D[]
        {
            new Point3D(6406, 2680, 0), new Point3D(6386, 2621, 0), new Point3D(6444, 2738, 0), new Point3D(6371, 2742, 0),
            new Point3D(6332, 2650, 0), new Point3D(6299, 2679, 0), new Point3D(6428, 2709, 0), new Point3D(6409, 2787, 0)
        };

        private static readonly Point3D[,] Teleporters = new Point3D[,]
        {
            {new Point3D(6405, 2679, 0), new Point3D(6387, 2620, 0)},
            {new Point3D(6387, 2620, 0), new Point3D(6405, 2679, 0)},
            {new Point3D(6385, 2620, 0), new Point3D(6445, 2737, 0)},
            {new Point3D(6445, 2737, 1), new Point3D(6385, 2620, 0)},
            {new Point3D(6443, 2737, 1), new Point3D(6372, 2741, 0)},
            {new Point3D(6372, 2741, 0), new Point3D(6443, 2737, 0)},
            {new Point3D(6370, 2741, 0), new Point3D(6333, 2649, 0)},
            {new Point3D(6333, 2649, 0), new Point3D(6370, 2741, 0)},
            {new Point3D(6331, 2649, 0), new Point3D(6300, 2678, 0)},
            {new Point3D(6300, 2678, 0), new Point3D(6331, 2649, 0)},
            {new Point3D(6298, 2678, 0), new Point3D(6429, 2708, 0)},
            {new Point3D(6429, 2708, 0), new Point3D(6298, 2678, 0)},
            {new Point3D(6427, 2708, 0), new Point3D(6410, 2786, 0)},
            {new Point3D(6410, 2786, 0), new Point3D(6427, 2708, 0)},
        };

        public static void GenerateQuestContent()
        {
            if (!Siege.SiegeShard)
            {
                if (BlackthornEntry.InstanceTram == null)
                {
                    BlackthornEntry.InstanceTram = new BlackthornEntry();
                    BlackthornEntry.InstanceTram.MoveToWorld(new Point3D(6409, 2677, 0), Map.Trammel);
                }

                if (BlackthornStep2.InstanceTram == null)
                {
                    BlackthornStep2.InstanceTram = new BlackthornStep2();
                    BlackthornStep2.InstanceTram.MoveToWorld(new Point3D(6378, 2612, 0), Map.Trammel);
                }

                if (BlackthornStep3.InstanceTram == null)
                {
                    BlackthornStep3.InstanceTram = new BlackthornStep3();
                    BlackthornStep3.InstanceTram.MoveToWorld(new Point3D(6454, 2741, 0), Map.Trammel);
                }

                if (BlackthornStep4.InstanceTram == null)
                {
                    BlackthornStep4.InstanceTram = new BlackthornStep4();
                    BlackthornStep4.InstanceTram.MoveToWorld(new Point3D(6363, 2739, 0), Map.Trammel);
                }

                if (BlackthornStep5.InstanceTram == null)
                {
                    BlackthornStep5.InstanceTram = new BlackthornStep5();
                    BlackthornStep5.InstanceTram.MoveToWorld(new Point3D(6327, 2647, 0), Map.Trammel);
                }

                if (BlackthornStep6.InstanceTram == null)
                {
                    BlackthornStep6.InstanceTram = new BlackthornStep6();
                    BlackthornStep6.InstanceTram.MoveToWorld(new Point3D(6297, 2677, -4), Map.Trammel);
                }

                if (BlackthornStep7.InstanceTram == null)
                {
                    BlackthornStep7.InstanceTram = new BlackthornStep7();
                    BlackthornStep7.InstanceTram.MoveToWorld(new Point3D(6429, 2709, 0), Map.Trammel);
                }

                if (BlackthornStep8.InstanceTram == null)
                {
                    BlackthornStep8.InstanceTram = new BlackthornStep8();
                    BlackthornStep8.InstanceTram.MoveToWorld(new Point3D(6408, 2792, 0), Map.Trammel);
                }

                OtherQuestContent(Map.Trammel);
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

            if (BlackthornStep3.InstanceFel == null)
            {
                BlackthornStep3.InstanceFel = new BlackthornStep3();
                BlackthornStep3.InstanceFel.MoveToWorld(new Point3D(6454, 2741, 0), Map.Felucca);
            }

            if (BlackthornStep4.InstanceFel == null)
            {
                BlackthornStep4.InstanceFel = new BlackthornStep4();
                BlackthornStep4.InstanceFel.MoveToWorld(new Point3D(6363, 2739, 0), Map.Felucca);
            }

            if (BlackthornStep5.InstanceFel == null)
            {
                BlackthornStep5.InstanceFel = new BlackthornStep5();
                BlackthornStep5.InstanceFel.MoveToWorld(new Point3D(6327, 2647, 0), Map.Felucca);
            }

            if (BlackthornStep6.InstanceFel == null)
            {
                BlackthornStep6.InstanceFel = new BlackthornStep6();
                BlackthornStep6.InstanceFel.MoveToWorld(new Point3D(6297, 2677, -4), Map.Felucca);
            }

            if (BlackthornStep7.InstanceFel == null)
            {
                BlackthornStep7.InstanceFel = new BlackthornStep7();
                BlackthornStep7.InstanceFel.MoveToWorld(new Point3D(6429, 2709, 0), Map.Felucca);
            }

            if (BlackthornStep8.InstanceFel == null)
            {
                BlackthornStep8.InstanceFel = new BlackthornStep8();
                BlackthornStep8.InstanceFel.MoveToWorld(new Point3D(6408, 2792, 0), Map.Felucca);
            }

            OtherQuestContent(Map.Felucca);
        }

        public static void OtherQuestContent(Map map)
        {
            blocker.ToList().ForEach(x =>
            {
                if (map.FindItem<Blocker>(new Point3D(x)) == null)
                {
                    Blocker bl = new Blocker();

                    bl.MoveToWorld(new Point3D(x), map);
                }
            });

            for (int i = 0; i < Workers.Length; i++)
            {
                Point3D p = Workers[i];

                if (map.FindItem<Blocker>(p) == null)
                {
                    Worker w = new Worker((FellowshipChain)(i + 1));

                    w.MoveToWorld(p, map);
                }
            }

            FellowshipChain c = FellowshipChain.One;

            for (int t = 0; t < Teleporters.Length / 2; t++)
            {
                Point3D p = Teleporters[t, 0];

                if (map.FindItem<BlackthornDungeonTeleporter>(p) == null)
                {
                    BlackthornDungeonTeleporter bl;

                    if (t % 2 == 0)
                    {
                        bl = new BlackthornDungeonTeleporter(c)
                        {
                            Dest = Teleporters[t, 1]
                        };

                        c++;
                    }
                    else
                    {
                        bl = new BlackthornDungeonTeleporter()
                        {
                            Dest = Teleporters[t, 1]
                        };
                    }

                    bl.MoveToWorld(p, map);
                }
            }
        }
    }
}
