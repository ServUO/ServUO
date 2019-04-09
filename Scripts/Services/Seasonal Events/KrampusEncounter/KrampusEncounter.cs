using System;
using System.IO;
using System.Collections.Generic;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Engines.CityLoyalty;

// TODO: Addto Seasonal Event Controller + EventType.KrampusEncounter
// SeasonalEventEntry.OnStatusChange() 
// TradeOrderCrate.cs needs a way to add the ID when encounter is active
// TradeSystem.SpawnCreatures needs a way to have creatures spawned overriden, ie its own function. line 407. needs to have check for null list
// Krampus will also need to bypass difficulty modifiers

// No Trades while krampus is spawned

namespace Server.Engines.SeasonalEvents
{
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
                        var encounter = new KrampusEncounter();
                        encounter.Deserialize(reader);

                        if (Enabled)
                        {
                            Encoutner = encounter;
                        }
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

        public int TotalTrades { get; set; }
        public Dictionary<PlayerMobile, int> CompleteTable { get; set; } = new CompleteTable<PlayerMobile, int>();

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
            if (wet)
            {
                return _WetSpawnTypes;
            }
            else
            {
                var wave = Math.Max(1, Math.Min(6, TotalTradesComplete / 20)); // TODO: Is this right?

                if (wave == 6)
                {
                    if ((m.Map == Map.Trammel || (Siege.IsSiegeShard && m.Map == Map.Felucca)) && !SpellHelper.IsAnyT2A(m))
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
            Krampus = new Krampus(this);

            var map = m.Map;
            var p = m.Location;

            for (int i = 0; i < 25; i++)
            {
                int x = p.X + (Utility.RandomMinMax(-3, 3));
                int y = p.Y + (Utility.RandomMinMax(-3, 3));
                int z = m.Map.GetAverageZ(x, y);

                if (map.CanSpawnMobile(x, y, z))
                {
                    p = new Point3D(x, y, z);
                    break;
                }
            }

            Krampus.MoveToWorld(p, map);
            Krampus.SpawnMinions();

            foreach (var ns in NetState.Instances)
            {
                var m = ns.Mobile;

                if (m != null && CityTradeSystem.HasTrade(m))
                {
                    m.SendLocalizedMessage(1158832, String.Format("{0}\t{1}", WorldLocationInfo.GetLocationString(Krampus.Location, Krampus.Map), Sextant.GetCoords(Krampus)), 1150); // *You sense Krampus has been spotted near ~2_where~ at ~1_coords~!*
                }
            }
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
            new Type[] { typeof(IceFiend), typeof(WhiteWyrm) }, // spawns w/ 2600+ health
			new Type[] { typeof(KrampusMinion) }, // ~3500 health
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
        }
    }
}
