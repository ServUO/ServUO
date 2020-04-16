#region References
using Server.Commands;
using Server.Items;
using Server.Mobiles;
using Server.Regions;
using Server.Spells;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#endregion

namespace Server
{
    public static class Siege
    {
        public static bool SiegeShard = Config.Get("Siege.IsSiege", false);
        public static int CharacterSlots = Config.Get("Siege.CharacterSlots", 1);
        public static string FilePath = Path.Combine("Saves", "Siege.bin");

        public static int StatsPerDay = 15;

        public static Dictionary<PlayerMobile, Dictionary<SkillName, DateTime>> ROTTable { get; private set; }
        public static Dictionary<PlayerMobile, int> StatsTable { get; private set; }

        public static DateTime LastReset { get; private set; }

        static Siege()
        {
            ROTTable = new Dictionary<PlayerMobile, Dictionary<SkillName, DateTime>>();
            StatsTable = new Dictionary<PlayerMobile, int>();
        }

        public static void Configure()
        {
            if (SiegeShard)
            {
                EventSink.AfterWorldSave += OnAfterSave;
                EventSink.Login += OnLogin;

                EventSink.WorldSave += OnSave;
                EventSink.WorldLoad += OnLoad;
            }
        }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(FilePath, OnSerialize);
        }

        public static void OnLoad()
        {
            Persistence.Deserialize(FilePath, OnDeserialize);
        }

        private static void OnSerialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(LastReset);

            writer.Write(ROTTable.Count);

            foreach (KeyValuePair<PlayerMobile, Dictionary<SkillName, DateTime>> kvp in ROTTable)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value.Count);

                foreach (KeyValuePair<SkillName, DateTime> kvp2 in kvp.Value)
                {
                    writer.Write((int)kvp2.Key);
                    writer.Write(kvp2.Value);
                }
            }

            writer.Write(StatsTable.Count);

            foreach (KeyValuePair<PlayerMobile, int> kvp in StatsTable)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }
        }

        private static void OnDeserialize(GenericReader reader)
        {
            reader.ReadInt();

            LastReset = reader.ReadDateTime();

            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                PlayerMobile pm = reader.ReadMobile<PlayerMobile>();
                Dictionary<SkillName, DateTime> dict = new Dictionary<SkillName, DateTime>();

                int c = reader.ReadInt();

                for (int j = 0; j < c; j++)
                {
                    SkillName sk = (SkillName)reader.ReadInt();
                    DateTime next = reader.ReadDateTime();

                    dict[sk] = next;
                }

                if (pm != null)
                {
                    ROTTable[pm] = dict;
                }
            }

            count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                PlayerMobile pm = reader.ReadMobile<PlayerMobile>();
                int total = reader.ReadInt();

                if (pm != null)
                {
                    StatsTable[pm] = total;
                }
            }

            CheckTime();
        }

        public static void OnLogin(LoginEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm != null && pm.Map == Map.Trammel && pm.AccessLevel == AccessLevel.Player)
            {
                pm.MoveToWorld(new Point3D(989, 519, -50), Map.Malas);
                pm.SendMessage("You have been removed from Trammel.");
            }
        }

        public static void Initialize()
        {
            if (SiegeShard)
            {
                CommandSystem.Register(
                    "ResetROT",
                    AccessLevel.GameMaster,
                    e =>
                    {
                        LastReset = DateTime.Now;

                        e.Mobile.SendMessage("Rate over Time reset!");
                    });

                CommandSystem.Register(
                    "GetROTInfo",
                    AccessLevel.GameMaster,
                    e =>
                    {
                        foreach (KeyValuePair<PlayerMobile, Dictionary<SkillName, DateTime>> kvp in ROTTable)
                        {
                            Console.WriteLine("Player: {0}", kvp.Key.Name);

                            int stats = 0;

                            if (StatsTable.ContainsKey(kvp.Key))
                            {
                                stats = StatsTable[kvp.Key];
                            }

                            Console.WriteLine("Stats gained today: {0} of {1}", stats, StatsPerDay.ToString());

                            Utility.PushColor(ConsoleColor.Magenta);

                            foreach (KeyValuePair<SkillName, DateTime> kvp2 in kvp.Value)
                            {
                                int pergain = MinutesPerGain(kvp.Key, kvp.Key.Skills[kvp2.Key]);
                                DateTime last = kvp2.Value;
                                DateTime next = last.AddMinutes(pergain);

                                string nextg = next < DateTime.Now
                                    ? "now"
                                    : "in " + ((int)(next - DateTime.Now).TotalMinutes).ToString() + " minutes";

                                Console.WriteLine(
                                    "   {0}: last gained {1}, can gain {2} (every {3} minutes)",
                                    kvp2.Key.ToString(),
                                    last.ToShortTimeString(),
                                    nextg,
                                    pergain.ToString());
                            }

                            Utility.PopColor();
                        }

                        Console.WriteLine("---");
                        Console.WriteLine(
                            "Next Reset: {0} minutes",
                            ((LastReset + TimeSpan.FromHours(24) - DateTime.Now)).TotalMinutes.ToString());
                    });

                Utility.PushColor(ConsoleColor.Red);
                Console.Write("Initializing Siege Perilous Shard...");

                long tick = Core.TickCount;

                List<XmlSpawner> toReset = new List<XmlSpawner>();

                foreach (XmlSpawner item in World.Items.Values.OfType<XmlSpawner>().Where(sp => sp.Map == Map.Trammel && sp.Running))
                {
                    toReset.Add(item);
                }

                foreach (XmlSpawner item in toReset)
                {
                    item.DoReset = true;
                }

                Console.WriteLine("Reset {1} trammel spawners in {0} milliseconds!", Core.TickCount - tick, toReset.Count);
                Utility.PopColor();

                ColUtility.Free(toReset);

                EventSink.ContainerDroppedTo += OnDropped;
            }
        }

        public static void OnDropped(ContainerDroppedToEventArgs e)
        {
            if (!SiegeShard)
                return;

            Item item = e.Dropped;
            Mobile from = e.Mobile;
            Container cont = e.Container;

            if (item != null)
            {
                if (cont != from.Backpack && from is PlayerMobile && ((PlayerMobile)from).BlessedItem != null && ((PlayerMobile)from).BlessedItem == item)
                {
                    ((PlayerMobile)from).BlessedItem = null;
                    item.LootType = LootType.Regular;

                    from.SendLocalizedMessage(1075292, item.Name != null ? item.Name : "#" + item.LabelNumber.ToString()); // ~1_NAME~ has been unblessed.
                }
            }
        }

        /// <summary>
        ///     Called in SpellHelper.cs CheckTravel method
        /// </summary>
        /// <param name="m"></param>
		/// <param name="p"></param>
		/// <param name="map"></param>
        /// <param name="type"></param>
        /// <returns>False fails travel check. True must pass other travel checks in SpellHelper.cs</returns>
        public static bool CheckTravel(Mobile m, Point3D p, Map map, TravelCheckType type)
        {
            if (m.AccessLevel > AccessLevel.Player)
                return true;

            switch (type)
            {
                case TravelCheckType.RecallFrom:
                case TravelCheckType.RecallTo:
                    {
                        return false;
                    }
                case TravelCheckType.GateFrom:
                case TravelCheckType.GateTo:
                case TravelCheckType.Mark:
                    {
                        return CanTravelTo(m, p, map);
                    }
                case TravelCheckType.TeleportFrom:
                case TravelCheckType.TeleportTo:
                    {
                        return true;
                    }
            }

            return true;
        }

        public static bool CanTravelTo(Mobile m, Point3D p, Map map)
        {
            return !(Region.Find(p, map) is DungeonRegion) && !SpellHelper.IsAnyT2A(map, p) && !SpellHelper.IsIlshenar(map, p);
        }

        public static void OnAfterSave(AfterWorldSaveEventArgs e)
        {
            CheckTime();
        }

        public static void CheckTime()
        {
            DateTime now = DateTime.Now;

            if (LastReset.AddHours(24) < now)
            {
                DateTime reset = new DateTime(now.Year, now.Month, now.Day, 20, 0, 0);

                if (now < reset)
                {
                    LastReset = reset - TimeSpan.FromHours(24);
                }
                else
                {
                    ROTTable.Clear();
                    StatsTable.Clear();

                    LastReset = reset;
                }
            }
        }

        public static bool CheckSkillGain(PlayerMobile pm, int minutesPerSkill, Skill skill)
        {
            if (minutesPerSkill == 0)
            {
                return true;
            }

            SkillName sk = skill.SkillName;

            if (ROTTable.ContainsKey(pm))
            {
                if (ROTTable[pm].ContainsKey(sk))
                {
                    DateTime lastGain = ROTTable[pm][sk];

                    if (lastGain + TimeSpan.FromMinutes(minutesPerSkill) < DateTime.Now)
                    {
                        ROTTable[pm][sk] = DateTime.Now;

                        return true;
                    }

                    return false;
                }

                ROTTable[pm][sk] = DateTime.Now;

                return true;
            }

            ROTTable[pm] = new Dictionary<SkillName, DateTime>();
            ROTTable[pm][sk] = DateTime.Now;

            return true;
        }

        public static int MinutesPerGain(Mobile m, Skill skill)
        {
            double value = skill.Base;

            if (value < 70.0)
            {
                return 0;
            }

            if (value <= 79.9)
            {
                return 5;
            }

            if (value <= 89.9)
            {
                return 8;
            }

            if (value <= 99.9)
            {
                return 12;
            }

            return 15;
        }

        public static bool CanGainStat(PlayerMobile m)
        {
            if (!StatsTable.ContainsKey(m))
            {
                return true;
            }

            return StatsTable[m] < StatsPerDay;
        }

        public static void IncreaseStat(PlayerMobile m)
        {
            if (!StatsTable.ContainsKey(m))
            {
                StatsTable[m] = 1;
            }
            else
            {
                StatsTable[m]++;
            }
        }

        public static bool VendorCanSell(Type t)
        {
            if (t == null)
            {
                return false;
            }

            foreach (Type type in _NoSellList)
            {
                if (t == type || t.IsSubclassOf(type))
                {
                    return false;
                }
            }

            return true;
        }

        private static readonly Type[] _NoSellList =
        {
            typeof(BaseIngot), typeof(BaseWoodBoard), typeof(BaseLog), typeof(BaseLeather), typeof(BaseHides), typeof(Cloth),
            typeof(BoltOfCloth), typeof(UncutCloth), typeof(Wool), typeof(Cotton), typeof(Flax), typeof(SpoolOfThread),
            typeof(Feather), typeof(Shaft), typeof(Arrow), typeof(Bolt)
        };

        public static void TryBlessItem(PlayerMobile pm, object targeted)
        {
            Item item = targeted as Item;

            if (item != null)
            {
                if (CanBlessItem(pm, item))
                {
                    if (pm.BlessedItem != null && pm.BlessedItem == item)
                    {
                        pm.BlessedItem.LootType = LootType.Regular;

                        pm.SendLocalizedMessage(
                            1075292,
                            pm.BlessedItem.Name ?? "#" + pm.BlessedItem.LabelNumber); // ~1_NAME~ has been unblessed.

                        pm.BlessedItem = null;
                    }
                    else if (item.LootType == LootType.Regular && !(item is Container))
                    {
                        Item old = pm.BlessedItem;

                        pm.BlessedItem = item;
                        pm.BlessedItem.LootType = LootType.Blessed;

                        pm.SendLocalizedMessage(
                            1075293,
                            pm.BlessedItem.Name ?? "#" + pm.BlessedItem.LabelNumber); // ~1_NAME~ has been blessed.

                        if (old != null)
                        {
                            old.LootType = LootType.Regular;

                            pm.SendLocalizedMessage(1075292, old.Name ?? "#" + old.LabelNumber); // ~1_NAME~ has been unblessed.
                        }
                    }
                }
                else
                {
                    pm.SendLocalizedMessage(1045114); // You cannot bless that item
                }
            }
        }

        public static bool CanBlessItem(PlayerMobile pm, Item item)
        {
            return (pm.Items.Contains(item) || (pm.Backpack != null && pm.Backpack.Items.Contains(item)) && !item.Stackable &&
                    (item is BaseArmor || item is BaseJewel || item is BaseClothing || item is BaseWeapon));
        }

        public static void CheckUsesRemaining(Mobile from, Item item)
        {
            IUsesRemaining uses = item as IUsesRemaining;

            if (uses != null)
            {
                uses.ShowUsesRemaining = true;
                uses.UsesRemaining--;

                if (uses.UsesRemaining <= 0)
                {
                    item.Delete();

                    from.SendLocalizedMessage(1044038); // You have worn out your tool!
                }
            }
        }
    }
}
