using Server.Engines.Quests;
using Server.Items;
using Server.Mobiles;
using Server.Engines.SeasonalEvents;
using System.Linq;

namespace Server.Engines.Fellowship
{
    public class ForsakenFoesEvent : SeasonalEvent
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public bool QuestContentGenerated { get; set; }

        public static ForsakenFoesEvent Instance { get; set; }

        public ForsakenFoesEvent(EventType type, string name, EventStatus status)
            : base(type, name, status)
        {
            Instance = this;
        }

        public ForsakenFoesEvent(EventType type, string name, EventStatus status, int month, int day, int duration)
            : base(type, name, status, month, day, duration)
        {
            Instance = this;
        }

        public override void CheckEnabled()
        {
            base.CheckEnabled();

            if (Running && IsActive() && !QuestContentGenerated)
            {
                GenerateQuestContent();
                QuestContentGenerated = true;
            }
        }

        public static void GenerateQuestContent()
        {
            if (!Siege.SiegeShard)
            {
                if (TheFellowshipHouse.InstanceTram == null)
                {
                    TheFellowshipHouse.InstanceTram = new TheFellowshipHouse();
                    TheFellowshipHouse.InstanceTram.MoveToWorld(new Point3D(1720, 1562, 17), Map.Trammel);
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

                OtherQuestContent(Map.Trammel);
            }

            if (TheFellowshipHouse.InstanceFel == null)
            {
                TheFellowshipHouse.InstanceFel = new TheFellowshipHouse();
                TheFellowshipHouse.InstanceFel.MoveToWorld(new Point3D(1720, 1562, 17), Map.Felucca);
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

            OtherQuestContent(Map.Felucca);
        }

        #region Add/Remove decoration
        protected override void Generate()
        {
            string filename = XmlSpawner.LocateFile("RevampedSpawns/BlackthornDungeonCreature.xml");
            XmlSpawner.XmlUnLoadFromFile(filename, string.Empty, null, out int processedmaps, out int processedspawners);

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

                GenerateMapDecoration(Map.Trammel);
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

            GenerateMapDecoration(Map.Felucca);
        }

        protected override void Remove()
        {
            string filename = XmlSpawner.LocateFile("RevampedSpawns/BlackthornDungeonCreature.xml");
            XmlSpawner.XmlLoadFromFile(filename, string.Empty, null, Point3D.Zero, Map.Internal, false, 0, false, out int processedmaps, out int processedspawners);

            RemoveDecoration();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write(QuestContentGenerated);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = InheritInsertion ? 0 : reader.ReadInt();

            switch (v)
            {
                case 1:
                    QuestContentGenerated = reader.ReadBool();
                    break;
            }
        }

        public static void RemoveDecoration()
        {
            if (BlackthornEntry.InstanceTram != null)
            {
                BlackthornEntry.InstanceTram.Delete();
                BlackthornEntry.InstanceTram = null;
            }

            if (BlackthornStep2.InstanceTram != null)
            {
                BlackthornStep2.InstanceTram.Delete();
                BlackthornStep2.InstanceTram = null;
            }

            if (BlackthornStep3.InstanceTram != null)
            {
                BlackthornStep3.InstanceTram.Delete();
                BlackthornStep3.InstanceTram = null;
            }

            if (BlackthornStep4.InstanceTram != null)
            {
                BlackthornStep4.InstanceTram.Delete();
                BlackthornStep4.InstanceTram = null;
            }

            if (BlackthornStep5.InstanceTram != null)
            {
                BlackthornStep5.InstanceTram.Delete();
                BlackthornStep5.InstanceTram = null;
            }

            if (BlackthornStep6.InstanceTram != null)
            {
                BlackthornStep6.InstanceTram.Delete();
                BlackthornStep6.InstanceTram = null;
            }

            if (BlackthornStep7.InstanceTram != null)
            {
                BlackthornStep7.InstanceTram.Delete();
                BlackthornStep7.InstanceTram = null;
            }

            if (BlackthornStep8.InstanceTram != null)
            {
                BlackthornStep8.InstanceTram.Delete();
                BlackthornStep8.InstanceTram = null;
            }

            RemoveOtherdecoration(Map.Trammel);

            if (BlackthornEntry.InstanceFel != null)
            {
                BlackthornEntry.InstanceFel.Delete();
                BlackthornEntry.InstanceFel = null;
            }

            if (BlackthornStep2.InstanceFel != null)
            {
                BlackthornStep2.InstanceFel.Delete();
                BlackthornStep2.InstanceFel = null;
            }

            if (BlackthornStep3.InstanceFel != null)
            {
                BlackthornStep3.InstanceFel.Delete();
                BlackthornStep3.InstanceFel = null;
            }

            if (BlackthornStep4.InstanceFel != null)
            {
                BlackthornStep4.InstanceFel.Delete();
                BlackthornStep4.InstanceFel = null;
            }

            if (BlackthornStep5.InstanceFel != null)
            {
                BlackthornStep5.InstanceFel.Delete();
                BlackthornStep5.InstanceFel = null;
            }

            if (BlackthornStep6.InstanceFel != null)
            {
                BlackthornStep6.InstanceFel.Delete();
                BlackthornStep6.InstanceFel = null;
            }

            if (BlackthornStep7.InstanceFel != null)
            {
                BlackthornStep7.InstanceFel.Delete();
                BlackthornStep7.InstanceFel = null;
            }

            if (BlackthornStep8.InstanceFel != null)
            {
                BlackthornStep8.InstanceFel.Delete();
                BlackthornStep8.InstanceFel = null;
            }

            RemoveOtherdecoration(Map.Felucca);
        }

        public static void OtherQuestContent(Map map)
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

        public static void RemoveOtherdecoration(Map map)
        {
            blocker.ToList().ForEach(x =>
            {
                Item b = map.FindItem<Blocker>(new Point3D(x));

                if (b != null)
                {
                    b.Delete();
                }

                Item lb = map.FindItem<LOSBlocker>(new Point3D(x));

                if (lb != null)
                {
                    lb.Delete();
                }
            });

            for (int i = 0; i < Workers.Length; i++)
            {
                Point3D p = Workers[i];

                Mobile w = map.FindMobile<Worker>(p);

                if (w != null)
                {
                    w.Delete();
                }
            }

            for (int t = 0; t < Teleporters.Length / 2; t++)
            {
                Point3D p = Teleporters[t, 0];

                Item tele = map.FindItem<BlackthornDungeonTeleporter>(p);

                if (tele != null)
                {
                    tele.Delete();
                }
            }
        }
        #endregion

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
            new Point3D(6390, 2624, 0), new Point3D(6390, 2625, 0), new Point3D(6434, 2708, 0), new Point3D(6434, 2709, 0),
            new Point3D(6434, 2710, 0), new Point3D(6434, 2711, 0)
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

        public static void GenerateMapDecoration(Map map)
        {
            blocker.ToList().ForEach(x =>
            {
                if (map.FindItem<Blocker>(new Point3D(x)) == null)
                {
                    Blocker bl = new Blocker();

                    bl.MoveToWorld(new Point3D(x), map);
                }

                if (map.FindItem<LOSBlocker>(new Point3D(x)) == null)
                {
                    LOSBlocker lb = new LOSBlocker();

                    lb.MoveToWorld(new Point3D(x), map);
                }
            });

            for (int i = 0; i < Workers.Length; i++)
            {
                Point3D p = Workers[i];

                if (map.FindMobile<Worker>(p) == null)
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

        public static void Initialize()
        {
            if (MiningCooperativeMerchant.InstanceTram == null && !Siege.SiegeShard)
            {
                MiningCooperativeMerchant.InstanceTram = new MiningCooperativeMerchant();
                MiningCooperativeMerchant.InstanceTram.MoveToWorld(new Point3D(2497, 432, 15), Map.Trammel);

                MiningCooperativeMerchant.InstanceTram.Home = MiningCooperativeMerchant.InstanceTram.Location;
                MiningCooperativeMerchant.InstanceTram.RangeHome = 5;
            }

            if (MiningCooperativeMerchant.InstanceFel == null)
            {
                MiningCooperativeMerchant.InstanceFel = new MiningCooperativeMerchant();
                MiningCooperativeMerchant.InstanceFel.MoveToWorld(new Point3D(2497, 432, 15), Map.Felucca);

                MiningCooperativeMerchant.InstanceFel.Home = MiningCooperativeMerchant.InstanceFel.Location;
                MiningCooperativeMerchant.InstanceFel.RangeHome = 5;
            }

            if (FellowshipAdept.InstanceTram == null && !Siege.SiegeShard)
            {
                FellowshipAdept.InstanceTram = new FellowshipAdept();
                FellowshipAdept.InstanceTram.MoveToWorld(new Point3D(1711, 1570, 44), Map.Trammel);

                FellowshipAdept.InstanceTram.Home = FellowshipAdept.InstanceTram.Location;
                FellowshipAdept.InstanceTram.RangeHome = 5;
            }

            if (FellowshipAdept.InstanceFel == null)
            {
                FellowshipAdept.InstanceFel = new FellowshipAdept();
                FellowshipAdept.InstanceFel.MoveToWorld(new Point3D(1711, 1570, 44), Map.Felucca);

                FellowshipAdept.InstanceFel.Home = FellowshipAdept.InstanceFel.Location;
                FellowshipAdept.InstanceFel.RangeHome = 5;
            }
        }
    }
}
