using System;
using System.IO;
using System.Collections.Generic;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Engines.CityLoyalty;
using Server.Spells;
using Server.Network;
using Server.Commands;
using Server.Gumps;

namespace Server.Engines.SeasonalEvents
{
    [PropertyObject]
    public class KrampusEncounter
    {
        public static string FilePath = Path.Combine("Saves/Misc", "KrampusEncounter.bin");

        public static bool Enabled { get { return SeasonalEventSystem.IsActive(EventType.KrampusEncounter); } }
        public static KrampusEncounter Encounter { get; set; }

        public static readonly int MinComplete = 20;

        public static void Configure()
        {
            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;
        }

        public static void Initialize()
        {
            CommandSystem.Register("KrampusEncounter", AccessLevel.Administrator, e =>
            {
                if (Encounter != null)
                {
                    e.Mobile.SendGump(new PropertiesGump(e.Mobile, Encounter));
                }
                else
                {
                    e.Mobile.SendMessage("Encounter null");
                }
            });
        }

        public static void OnSave(WorldSaveEventArgs e)
        {
            CheckEnabled();

            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write(0);

                    if (Encounter != null)
                    {
                        writer.Write(1);
                        Encounter.Serialize(writer);
                    }
                    else
                    {
                        writer.Write(0);
                    }
                });
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
                        Encounter = new KrampusEncounter();
                        Encounter.Deserialize(reader);
                    }
                });
        }

        public static void CheckEnabled()
        {
            if (Enabled)
            {
                if (Encounter == null)
                {
                    Encounter = new KrampusEncounter();
                }
            }
            else
            {
                if (Encounter != null && Encounter.Krampus == null)
                {
                    Encounter = null;
                }
            }
        }

        public static bool KrampusSpawned()
        {
            return Enabled && Encounter != null && Encounter.Krampus != null && !Encounter.Krampus.Deleted;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TotalTradesComplete { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Wave { get { return (int)Math.Max(1, (int)Math.Min(6, (double)TotalTradesComplete / 4.1)); } }

        public Dictionary<PlayerMobile, int> CompleteTable { get; set; } = new Dictionary<PlayerMobile, int>();

        [CommandProperty(AccessLevel.GameMaster)]
        public Krampus Krampus { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D SpawnLocation { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool KrampusSpawning { get { return SpawnMap != null && SpawnLocation != Point3D.Zero; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map SpawnMap { get; set; }

        public KrampusEncounter()
        {
        }

        public void OnTradeComplete(Mobile m, TradeEntry entry)
        {
            bool distCheck = entry.Distance > 0;

            // 0 distance indicates they used a moongate
            if (m is PlayerMobile && distCheck)
            {
                var pm = (PlayerMobile)m;

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
                var wave = (int)Math.Max(1, (int)Math.Min(6, (double)TotalTradesComplete / 4.1)); // TODO: Is this right?

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
            var p = m.Location;

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

            foreach (var ns in NetState.Instances)
            {
                var mob = ns.Mobile;

                if (mob != null && CityTradeSystem.HasTrade(mob))
                {
                    mob.LocalOverheadMessage(MessageType.Regular, 1150, 1158832, String.Format("{0}\t{1}", WorldLocationInfo.GetLocationString(SpawnLocation, SpawnMap), Sextant.GetCoords(SpawnLocation, SpawnMap))); // *You sense Krampus has been spotted near ~2_where~ at ~1_coords~!*
                }
            }

            Timer.DelayCall(TimeSpan.FromMinutes(5), () =>
            {
                SpawnKrampus();
            });
        }

        private void SpawnKrampus()
        {
            Krampus = new Krampus();
            Krampus.SpawnLocation = SpawnLocation;
            Krampus.Home = SpawnLocation;
            Krampus.RangeHome = 5;

            Krampus.MoveToWorld(SpawnLocation, SpawnMap);
            Krampus.Summon(Krampus, true);

            var rec = new Rectangle2D(SpawnLocation.X - 10, SpawnLocation.Y - 10, 20, 20);

            for (var i = 0; i < 2; i++)
            {
                var drake = new FrostDrake();

                Point3D p = new Point3D(SpawnLocation);

                for (int j = 0; i < 10; j++)
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

            if (!Enabled)
            {
                Encounter = null;
            }
        }

        public Type[][] _SpawnTypes =
        {
            new Type[] { typeof(FrostOoze), typeof(FrostSpider) },
            new Type[] { typeof(SnowElemental), typeof(IceElemental) },
            new Type[] { typeof(IceSerpent), typeof(FrostTroll) },
            new Type[] { typeof(IceFiend), typeof(WhiteWyrm) },
			new Type[] { typeof(KrampusMinion) },
			new Type[] { typeof(Krampus) }
        };

        public Type[] _WetSpawnTypes =
        {
            typeof(SeaSerpent), typeof(DeepSeaSerpent), typeof(Kraken), typeof(WaterElemental)
        };

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(Krampus);
            writer.Write(SpawnLocation);
            writer.Write(SpawnMap);

            writer.Write(TotalTradesComplete);

            writer.Write(CompleteTable.Count);

            foreach (var kvp in CompleteTable)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }
        }

        public void Deserialize(GenericReader reader)
        {
            reader.ReadInt();

            Krampus = reader.ReadMobile() as Krampus;
            SpawnLocation = reader.ReadPoint3D();
            SpawnMap = reader.ReadMap();

            TotalTradesComplete = reader.ReadInt();

            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                var m = reader.ReadMobile() as PlayerMobile;
                var c = reader.ReadInt();

                if (m != null)
                {
                    CompleteTable[m] = c;
                }
            }

            if (KrampusSpawning && Krampus == null)
            {
                Timer.DelayCall(TimeSpan.FromMinutes(2), SpawnKrampus);
            }
        }
    }
}
