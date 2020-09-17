using Server.Engines.CityLoyalty;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using System;
using System.Collections.Generic;
using System.IO;

namespace Server.Engines.SeasonalEvents
{
    public class KrampusEvent : SeasonalEvent
    {
        public static KrampusEvent Instance { get; set; }
        public static readonly int MinComplete = 20;

        public KrampusEvent(EventType type, string name, EventStatus status)
            : base(type, name, status)
        {
            Instance = this;
        }

        public KrampusEvent(EventType type, string name, EventStatus status, int month, int day, int duration)
            : base(type, name, status, month, day, duration)
        {
            Instance = this;
        }

        public static bool KrampusSpawned()
        {
            return Instance.Krampus != null && !Instance.Krampus.Deleted;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TotalTradesComplete { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Wave => Math.Max(1, (int)Math.Min(6, TotalTradesComplete / 4.1));

        public Dictionary<PlayerMobile, int> CompleteTable { get; set; } = new Dictionary<PlayerMobile, int>();

        [CommandProperty(AccessLevel.GameMaster)]
        public Krampus Krampus { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D SpawnLocation { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool KrampusSpawning => SpawnMap != null && SpawnLocation != Point3D.Zero;

        [CommandProperty(AccessLevel.GameMaster)]
        public Map SpawnMap { get; set; }

        public void OnTradeComplete(Mobile m, TradeEntry entry)
        {
            bool distCheck = entry.Distance > 0;

            // 0 distance indicates they used a moongate
            if (m is PlayerMobile && distCheck)
            {
                PlayerMobile pm = (PlayerMobile)m;

                if (!CompleteTable.ContainsKey(pm))
                {
                    CompleteTable[pm] = 1;
                }
                else
                {
                    CompleteTable[pm]++;
                }
            }

            if (TotalTradesComplete > MinComplete || distCheck)
            {
                TotalTradesComplete++;
            }
        }

        public Type[] GetCreatureTypes(Mobile m, bool wet)
        {
            if (Krampus != null || KrampusSpawning)
            {
                return null;
            }

            if (wet)
            {
                return _WetSpawnTypes;
            }
            else
            {
                int wave = Math.Max(1, (int)Math.Min(6, TotalTradesComplete / 4.1)); // TODO: Is this right?

                if (wave == 6)
                {
                    if ((m.Map == Map.Trammel || (Siege.SiegeShard && m.Map == Map.Felucca)) && !SpellHelper.IsAnyT2A(m.Map, m.Location))
                    {
                        SpawnKrampus(m);

                        return null;
                    }

                    return _SpawnTypes[4];
                }

                return _SpawnTypes[wave - 1];
            }
        }

        private void SpawnKrampus(Mobile m)
        {
            SpawnMap = m.Map;
            Point3D p = m.Location;

            for (int i = 0; i < 25; i++)
            {
                int x = p.X + (Utility.RandomMinMax(-3, 3));
                int y = p.Y + (Utility.RandomMinMax(-3, 3));
                int z = m.Map.GetAverageZ(x, y);

                if (SpawnMap.CanSpawnMobile(x, y, z))
                {
                    p = new Point3D(x, y, z);
                    break;
                }
            }

            SpawnLocation = p;

            foreach (NetState ns in NetState.Instances)
            {
                Mobile mob = ns.Mobile;

                if (mob != null && CityTradeSystem.HasTrade(mob))
                {
                    mob.LocalOverheadMessage(MessageType.Regular, 1150, 1158832, string.Format("{0}\t{1}", WorldLocationInfo.GetLocationString(SpawnLocation, SpawnMap), Sextant.GetCoords(SpawnLocation, SpawnMap))); // *You sense Krampus has been spotted near ~2_where~ at ~1_coords~!*
                }
            }

            Timer.DelayCall(TimeSpan.FromMinutes(5), () =>
            {
                SpawnKrampus();
            });
        }

        private void SpawnKrampus()
        {
            Krampus = new Krampus
            {
                SpawnLocation = SpawnLocation,
                Home = SpawnLocation,
                RangeHome = 5
            };

            Krampus.MoveToWorld(SpawnLocation, SpawnMap);
            Krampus.Summon(Krampus, true);

            Rectangle2D rec = new Rectangle2D(SpawnLocation.X - 10, SpawnLocation.Y - 10, 20, 20);

            for (int i = 0; i < 2; i++)
            {
                FrostDrake drake = new FrostDrake();

                Point3D p = new Point3D(SpawnLocation);

                for (int j = 0; j < 10; j++)
                {
                    p = SpawnMap.GetRandomSpawnPoint(rec);

                    if (SpawnMap.CanSpawnMobile(p.X, p.Y, p.Z))
                    {
                        break;
                    }
                }

                drake.MoveToWorld(p, SpawnMap);
                drake.Home = p;
                drake.RangeHome = 15;
            }

            SpawnLocation = Point3D.Zero;
            SpawnMap = null;
        }

        public void OnKrampusKilled()
        {
            Krampus = null;

            CompleteTable.Clear();
            TotalTradesComplete = 0;
        }

        private readonly Type[][] _SpawnTypes =
        {
            new Type[] { typeof(FrostOoze), typeof(FrostSpider) },
            new Type[] { typeof(SnowElemental), typeof(IceElemental) },
            new Type[] { typeof(IceSerpent), typeof(FrostTroll) },
            new Type[] { typeof(IceFiend), typeof(WhiteWyrm) },
            new Type[] { typeof(KrampusMinion) },
            new Type[] { typeof(Krampus) }
        };

        private readonly Type[] _WetSpawnTypes =
        {
            typeof(SeaSerpent), typeof(DeepSeaSerpent), typeof(Kraken), typeof(WaterElemental)
        };

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write(Krampus);
            writer.Write(SpawnLocation);
            writer.Write(SpawnMap);

            writer.Write(TotalTradesComplete);

            writer.Write(CompleteTable.Count);

            foreach (KeyValuePair<PlayerMobile, int> kvp in CompleteTable)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            var version = InheritInsertion ? 0 : reader.ReadInt();

            Deserialize(reader, version); // version
        }

        public void Deserialize(GenericReader reader, int version)
        {
            switch (version)
            {
                case 1:
                    Krampus = reader.ReadMobile() as Krampus;
                    SpawnLocation = reader.ReadPoint3D();
                    SpawnMap = reader.ReadMap();

                    TotalTradesComplete = reader.ReadInt();

                    int count = reader.ReadInt();

                    for (int i = 0; i < count; i++)
                    {
                        PlayerMobile m = reader.ReadMobile() as PlayerMobile;
                        int c = reader.ReadInt();

                        if (m != null)
                        {
                            CompleteTable[m] = c;
                        }
                    }
                    break;
            }

            if (KrampusSpawning && Krampus == null)
            {
                Timer.DelayCall(TimeSpan.FromMinutes(2), SpawnKrampus);
            }
        }

        private static readonly string FilePath = Path.Combine("Saves/Misc", "KrampusEncounter.bin");

        public static void Configure()
        {
            if (File.Exists(FilePath))
            {
                EventSink.WorldLoad += OnLoad;
            }
        }

        public static void OnLoad()
        {
            Persistence.Deserialize(
                FilePath,
                reader =>
                {
                    reader.ReadInt(); // version

                    if (reader.ReadInt() == 1)
                    {
                        Timer.DelayCall(() =>
                        {
                            try
                            {
                                Instance.Deserialize(reader, 1);
                            }
                            catch (Exception e)
                            {
                                Diagnostics.ExceptionLogging.LogException(e);
                            }
                        });
                    }

                    Timer.DelayCall(TimeSpan.FromSeconds(10), () =>
                    {
                        try
                        {
                            File.Delete(FilePath);
                        }
                        catch (Exception e)
                        {
                            Diagnostics.ExceptionLogging.LogException(e);
                        }
                    });
                });
        }
    }
}
