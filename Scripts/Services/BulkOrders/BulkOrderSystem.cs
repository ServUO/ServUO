using System;
using Server;
using Server.Mobiles;
using Server.Items;
using System.IO;
using System.Collections.Generic;

namespace Server.Engines.BulkOrders
{
    public enum PointsMode
    {
        Enabled,
        Disabled,
        Automatic
    }

    public class BulkOrderSystem
    {
        public static readonly int MaxCachedDeeds = 3;
        public static readonly int Delay = 6;

        public static bool NewSystemEnabled = Core.TOL;
        public static BulkOrderSystem Instance { get; set; }

        public Dictionary<PlayerMobile, BODContext> BODPlayerData { get; set; }

        public BulkOrderSystem()
        {
            BODPlayerData = new Dictionary<PlayerMobile, BODContext>();
        }

        static BulkOrderSystem()
        {
            Instance = new BulkOrderSystem();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(BODPlayerData.Count);
            foreach (KeyValuePair<PlayerMobile, BODContext> kvp in BODPlayerData)
            {
                writer.Write(kvp.Key);
                kvp.Value.Serialize(writer);
            }
        }

        public void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                PlayerMobile player = reader.ReadMobile() as PlayerMobile;
                BODContext context = new BODContext(reader);

                if (player != null)
                    BODPlayerData[player] = context;
            }
        }

        public static BODContext GetContext(Mobile m)
        {
            BODContext context = null;

            if (m is PlayerMobile)
            {
                PlayerMobile pm = m as PlayerMobile;

                if (Instance.BODPlayerData.ContainsKey(pm))
                {
                    context = Instance.BODPlayerData[pm];
                }
                else
                {
                    Instance.BODPlayerData[pm] = context = new BODContext();
                }
            }

            return context;
        }

        public static double GetPoints(Mobile m, BODType type)
        {
            BODContext context = GetContext(m);

            if (context != null)
            {
                if (context.Entries.ContainsKey(type))
                {
                    return context.Entries[type].BankedPoints;
                }
            }

            return 0;
        }

        public static void SetPoints(Mobile m, BODType type, double points)
        {
            if (points < 0)
                points = 0.0;

            BODContext context = GetContext(m);

            if (context != null)
            {
                if (context.Entries.ContainsKey(type))
                {
                    context.Entries[type].BankedPoints += points;
                }
            }
        }

        public static void DeductPoints(Mobile m, BODType type, double points)
        {
            if (points < 0)
                points = 0.0;

            BODContext context = GetContext(m);

            if (context != null)
            {
                if (context.Entries.ContainsKey(type))
                {
                    context.Entries[type].BankedPoints -= points;

                    if (context.Entries[type].BankedPoints < 0.0)
                        context.Entries[type].BankedPoints = 0.0;
                }
            }
        }

        public static List<CollectionItem> GetRewardCollection(BODType type)
        {
            switch (type)
            {
                default:
                case BODType.Smith: return SmithRewardCalculator.Instance.RewardCollection;
                case BODType.Tailor: return TailorRewardCalculator.Instance.RewardCollection;
                case BODType.Alchemy: return AlchemyRewardCalculator.Instance.RewardCollection;
                case BODType.Inscription: return InscriptionRewardCalculator.Instance.RewardCollection;
                case BODType.Tinkering: return TinkeringRewardCalculator.Instance.RewardCollection;
                case BODType.Fletching: return FletchingRewardCalculator.Instance.RewardCollection;
                case BODType.Carpentry: return CarpentryRewardCalculator.Instance.RewardCollection;
                case BODType.Cooking: return CookingRewardCalculator.Instance.RewardCollection;
            }
        }

        public static bool CanGetBulkOrder(Mobile m, BODType type)
        {
            BODContext context = GetContext(m);

            if (context != null && context.Entries.ContainsKey(type))
            {
                DateTime last = context.Entries[type].LastBulkOrder;

                if (context.Entries[type].CachedDeeds == 0)
                {
                    int tocache = 0;

                    if (last + TimeSpan.FromHours(Delay) < DateTime.UtcNow)
                    {
                        int minutes = (int)(DateTime.UtcNow - last).TotalMinutes;

                        tocache = (int)(minutes /  ((double)Delay * 60));
                    }

                    if (tocache > 0)
                    {
                        context.Entries[type].CachedDeeds = Math.Min(3, context.Entries[type].CachedDeeds + tocache);
                    }
                }

                if (context.Entries[type].CachedDeeds > 0)
                {
                    context.Entries[type].CachedDeeds--;
                    context.Entries[type].LastBulkOrder = DateTime.UtcNow;

                    return true;
                }
            }

            return false;
        }

        public static TimeSpan GetNextBulkOrder(BODType type, PlayerMobile pm)
        {
            BODContext context = GetContext(pm);

            if (context != null)
            {
                if (NewSystemEnabled)
                {
                    DateTime last = context.Entries[type].LastBulkOrder;

                    return (last + TimeSpan.FromHours(Delay)) - DateTime.UtcNow;
                }
                else if (context.Entries.ContainsKey(type))
                {
                    DateTime dt = context.Entries[type].NextBulkOrder;

                    if (dt < DateTime.UtcNow)
                        return TimeSpan.Zero;

                    return DateTime.UtcNow - dt;
                }
            }

            return TimeSpan.MaxValue;
        }

        public static void SetNextBulkOrder(BODType type, PlayerMobile pm, TimeSpan ts)
        {
            BODContext context = GetContext(pm);

            if (context != null)
            {
                if (NewSystemEnabled)
                {
                    if (context.Entries.ContainsKey(type))
                    {
                        context.Entries[type].LastBulkOrder = (DateTime.UtcNow + ts) - TimeSpan.FromHours(Delay);
                    }
                }
                else if (context.Entries.ContainsKey(type))
                {
                    context.Entries[type].NextBulkOrder = DateTime.UtcNow + ts;
                }
            }
        }

        public static Item CreateBulkOrder(Mobile m, BODType type, bool fromContextMenu)
        {
            PlayerMobile pm = m as PlayerMobile;

            if (pm == null)
                return null;

            if (pm.AccessLevel > AccessLevel.Player || fromContextMenu || 0.2 > Utility.RandomDouble())
            {
                SkillName sk = GetSkillForBOD(type);
                double theirSkill = pm.Skills[sk].Base;
                bool doLarge = theirSkill >= 70.1 && ((theirSkill - 40.0) / 300.0) > Utility.RandomDouble();

                switch (type)
                {
                    case BODType.Smith:
                        if (doLarge) return new LargeSmithBOD();
                        else return SmallSmithBOD.CreateRandomFor(pm);
                    case BODType.Tailor: if (doLarge) return new LargeTailorBOD();
                        else return SmallTailorBOD.CreateRandomFor(pm);
                    case BODType.Alchemy:
                        if (doLarge) return new LargeAlchemyBOD();
                        else return SmallAlchemyBOD.CreateRandomFor(pm);
                    case BODType.Inscription:
                        if (doLarge) return new LargeInscriptionBOD();
                        else return SmallInscriptionBOD.CreateRandomFor(pm);
                    case BODType.Tinkering:
                        if (doLarge) return new LargeTinkerBOD();
                        else return SmallTinkerBOD.CreateRandomFor(pm);
                    case BODType.Cooking:
                        if (doLarge) return new LargeCookingBOD();
                        else return SmallCookingBOD.CreateRandomFor(pm);
                    case BODType.Fletching:
                        if (doLarge) return new LargeFletchingBOD();
                        else return SmallFletchingBOD.CreateRandomFor(pm);
                    case BODType.Carpentry:
                        if (doLarge) return new LargeCarpentryBOD();
                        else return SmallCarpentryBOD.CreateRandomFor(pm);
                }
            }

            return null;
        }

        public static SkillName GetSkillForBOD(BODType type)
        {
            switch (type)
            {
                default:
                case BODType.Smith: return SkillName.Blacksmith;
                case BODType.Tailor: return SkillName.Tailoring;
                case BODType.Alchemy: return SkillName.Alchemy;
                case BODType.Inscription: return SkillName.Inscribe;
                case BODType.Tinkering: return SkillName.Tinkering;
                case BODType.Cooking: return SkillName.Cooking;
                case BODType.Fletching: return SkillName.Fletching;
                case BODType.Carpentry: return SkillName.Carpentry;
            }
        }

        public static BOBFilter GetBOBFilter(PlayerMobile pm)
        {
            return GetContext(pm).BOBFilter;
        }

        public static void SetBOBFilter(PlayerMobile pm, BOBFilter filter)
        {
            BODContext context = GetContext(pm);
            context.BOBFilter = filter;
        }

        public static int ComputePoints(SmallBOD bod)
        {
            switch (bod.BODType)
            {
                default:
                case BODType.Smith: return SmithRewardCalculator.Instance.ComputePoints(bod);
                case BODType.Tailor: return TailorRewardCalculator.Instance.ComputePoints(bod);
                case BODType.Alchemy: return AlchemyRewardCalculator.Instance.ComputePoints(bod);
                case BODType.Inscription: return InscriptionRewardCalculator.Instance.ComputePoints(bod);
                case BODType.Tinkering: return TinkeringRewardCalculator.Instance.ComputePoints(bod);
                case BODType.Cooking: return CookingRewardCalculator.Instance.ComputePoints(bod);
                case BODType.Fletching: return FletchingRewardCalculator.Instance.ComputePoints(bod);
                case BODType.Carpentry: return CarpentryRewardCalculator.Instance.ComputePoints(bod);
            }
        }

        public static int ComputePoints(LargeBOD bod)
        {
            switch (bod.BODType)
            {
                default:
                case BODType.Smith: return SmithRewardCalculator.Instance.ComputePoints(bod);
                case BODType.Tailor: return TailorRewardCalculator.Instance.ComputePoints(bod);
                case BODType.Alchemy: return AlchemyRewardCalculator.Instance.ComputePoints(bod);
                case BODType.Inscription: return InscriptionRewardCalculator.Instance.ComputePoints(bod);
                case BODType.Tinkering: return TinkeringRewardCalculator.Instance.ComputePoints(bod);
                case BODType.Cooking: return CookingRewardCalculator.Instance.ComputePoints(bod);
                case BODType.Fletching: return FletchingRewardCalculator.Instance.ComputePoints(bod);
                case BODType.Carpentry: return CarpentryRewardCalculator.Instance.ComputePoints(bod);
            }
        }

        public static bool ComputeGold(Type type, int quantity, out int gold)
        {
            if (GenericBuyInfo.BuyPrices.ContainsKey(type))
            {
                gold = (quantity * GenericBuyInfo.BuyPrices[type]) / 2;
                return true;
            }

            gold = 0;
            return false;
        }

        public static void ComputePoints(SmallBOD bod, out int points, out double banked)
        {
            points = 0;
            banked = 0.0;

            switch (bod.BODType)
            {
                default:
                case BODType.Smith: points = SmithRewardCalculator.Instance.ComputePoints(bod); break;
                case BODType.Tailor: points = TailorRewardCalculator.Instance.ComputePoints(bod); break;
                case BODType.Alchemy: points = AlchemyRewardCalculator.Instance.ComputePoints(bod); break;
                case BODType.Inscription: points = InscriptionRewardCalculator.Instance.ComputePoints(bod); break;
                case BODType.Tinkering: points = TinkeringRewardCalculator.Instance.ComputePoints(bod); break;
                case BODType.Cooking: points = CookingRewardCalculator.Instance.ComputePoints(bod); break;
                case BODType.Fletching: points = FletchingRewardCalculator.Instance.ComputePoints(bod); break;
                case BODType.Carpentry: points = CarpentryRewardCalculator.Instance.ComputePoints(bod); break;
            }

            banked = (double)points * 0.02;
        }

        public static void ComputePoints(LargeBOD bod, out int points, out double banked)
        {
            points = 0;
            banked = 0.0;

            switch (bod.BODType)
            {
                default:
                case BODType.Smith: points = SmithRewardCalculator.Instance.ComputePoints(bod); break;
                case BODType.Tailor: points = TailorRewardCalculator.Instance.ComputePoints(bod); break;
                case BODType.Alchemy: points = AlchemyRewardCalculator.Instance.ComputePoints(bod); break;
                case BODType.Inscription: points = InscriptionRewardCalculator.Instance.ComputePoints(bod); break;
                case BODType.Tinkering: points = TinkeringRewardCalculator.Instance.ComputePoints(bod); break;
                case BODType.Cooking: points = CookingRewardCalculator.Instance.ComputePoints(bod); break;
                case BODType.Fletching: points = FletchingRewardCalculator.Instance.ComputePoints(bod); break;
                case BODType.Carpentry: points = CarpentryRewardCalculator.Instance.ComputePoints(bod); break;
            }

            banked = (double)points * .2;
        }

        public static void AddToPending(Mobile m, BODType type, int points)
        {
            BODContext context = GetContext(m);

            if (context != null)
            {
                context.AddPending(type, points);
            }
        }

        public static void RemovePending(Mobile m, BODType type)
        {
            BODContext context = GetContext(m);

            if (context != null)
            {
                context.RemovePending(type);
            }
        }

        public static bool CanClaimRewards(Mobile m, BODType type)
        {
            BODContext context = GetContext(m);

            if (context != null)
            {
                return context.CanClaimRewards(type);
            }

            return true;
        }

        public static int GetPendingRewardFor(Mobile m, BODType type)
        {
            BODContext context = GetContext(m);

            if (context != null && context.Entries.ContainsKey(type))
            {
                return context.Entries[type].PendingRewardPoints;
            }

            return 0;
        }

        /* Tinkering needs conditional check for combining:
        * SpoonLeft/SpoonRight, ForkLeft/ForkRight, KnifeLeft/KnifeRight, ClockRight/ClockLeft
         * TODO: Craft and make sure they show crafter/exceptional etc
        */
        private static Type[][] _TinkerTable =
        {
            new Type[] { typeof(Spoon), typeof(SpoonRight), typeof(SpoonLeft) },
            new Type[] { typeof(Fork), typeof(ForkRight), typeof(ForkLeft) },
            new Type[] { typeof(Knife), typeof(KnifeRight), typeof(KnifeLeft) },
            new Type[] { typeof(Clock), typeof(ClockRight), typeof(ClockLeft) },
            new Type[] { typeof(GoldRing), typeof(SilverRing) },
            new Type[] { typeof(GoldBracelet), typeof(SilverBracelet) },
        };

        public static bool CheckTinker(Type actual, Type lookingfor)
        {
            foreach (Type[] types in _TinkerTable)
            {
                if (types[0] == lookingfor)
                {
                    foreach (Type t in types)
                    {
                        if (actual == t)
                            return true;
                    }
                }
            }

            return false;
        }

        public static string FilePath = Path.Combine("Saves/CraftContext", "BODs.bin");

        public static void Configure()
        {
            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;
        }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write(0);

                    Instance.Serialize(writer);
                });
        }

        public static void OnLoad()
        {
            Persistence.Deserialize(
                FilePath,
                reader =>
                {
                    int version = reader.ReadInt();

                    Instance.Deserialize(reader);
                });
        }
    }

    public class BODContext
    {
        public Dictionary<BODType, BODEntry> Entries { get; set; }

        public BOBFilter BOBFilter { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public PointsMode PointsMode { get; set; }

        public BODContext()
        {
            BOBFilter = new BOBFilter();

            ConfigEntries();
        }

        public void AddPending(BODType type, int points)
        {
            if (Entries.ContainsKey(type))
            {
                Entries[type].PendingRewardPoints = points;
            }
        }

        public void RemovePending(BODType type)
        {
            if (Entries.ContainsKey(type))
            {
                Entries[type].PendingRewardPoints = 0;
            }
        }

        public bool CanClaimRewards(BODType type)
        {
            foreach (KeyValuePair<BODType, BODEntry> kvp in Entries)
            {
                if (kvp.Value.PendingRewardPoints > 0 && kvp.Key != type)
                    return false;
            }

            return true;
        }

        public BODContext(GenericReader reader)
        {
            int version = reader.ReadInt();
            ConfigEntries();

            this.PointsMode = (PointsMode)reader.ReadInt();
            BOBFilter = new BOBFilter(reader);

            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                BODType type = (BODType)reader.ReadInt();
                BODEntry entry = new BODEntry(reader);

                if (Entries.ContainsKey(type))
                    Entries[type] = entry;
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write((int)this.PointsMode);
            BOBFilter.Serialize(writer);

            // Lets do this dynamically incase we add new bods in the future
            writer.Write(Entries.Count);
            foreach (KeyValuePair<BODType, BODEntry> kvp in Entries)
            {
                writer.Write((int)kvp.Key);
                kvp.Value.Serialize(writer);
            }
        }

        private void ConfigEntries()
        {
            Entries = new Dictionary<BODType, BODEntry>();

            Entries[BODType.Smith] = new BODEntry();
            Entries[BODType.Tailor] = new BODEntry();
            Entries[BODType.Tinkering] = new BODEntry();
            Entries[BODType.Carpentry] = new BODEntry();
            Entries[BODType.Fletching] = new BODEntry();
            Entries[BODType.Cooking] = new BODEntry();
            Entries[BODType.Alchemy] = new BODEntry();
            Entries[BODType.Inscription] = new BODEntry();
        }
    }

    public class BODEntry
    {
        public int CachedDeeds { get; set; }
        public DateTime LastBulkOrder { get; set; }
        public double BankedPoints { get; set; }
        public int PendingRewardPoints { get; set; }

        // Legacy System
        private DateTime _NextBulkOrder;

        public DateTime NextBulkOrder
        {
            get
            {
                return _NextBulkOrder;
            }
            set
            {
                _NextBulkOrder = value;
            }
        }

        public BODEntry()
        {
            CachedDeeds = BulkOrderSystem.MaxCachedDeeds;
        }

        public BODEntry(GenericReader reader)
        {
            int version = reader.ReadInt();

            if (reader.ReadInt() == 0)
            {
                CachedDeeds = reader.ReadInt();
                LastBulkOrder = reader.ReadDateTime();
                BankedPoints = reader.ReadDouble();
                PendingRewardPoints = reader.ReadInt();
            }
            else
            {
                _NextBulkOrder = reader.ReadDateTime();
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            if (BulkOrderSystem.NewSystemEnabled)
            {
                writer.Write(0);
                writer.Write(CachedDeeds);
                writer.Write(LastBulkOrder);
                writer.Write(BankedPoints);
                writer.Write(PendingRewardPoints);
            }
            else
            {
                writer.Write(1);
                writer.Write(_NextBulkOrder);
            }
        }
    }
}