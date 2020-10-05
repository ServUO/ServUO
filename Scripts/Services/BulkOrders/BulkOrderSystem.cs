using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Server.Engines.BulkOrders
{
    public enum PointsMode
    {
        Enabled,
        Disabled,
        Automatic
    }

    public interface IBOD
    {
        int AmountMax { get; set; }
        bool RequireExceptional { get; set; }
        bool Complete { get; }
        BulkMaterialType Material { get; set; }
        BODType BODType { get; }
    }

    public class BulkOrderSystem
    {
        // Logic (EA says 3 cached): 2 cached, 1 in the pipe if the last bod > 6 hours = 3
        public static readonly int MaxCachedDeeds = 2;
        public static readonly int Delay = 6;

        public static bool NewSystemEnabled = true;
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

        public static BODContext GetContext(Mobile m, bool create = true)
        {
            BODContext context = null;

            if (m is PlayerMobile)
            {
                PlayerMobile pm = m as PlayerMobile;

                if (Instance.BODPlayerData.ContainsKey(pm))
                {
                    context = Instance.BODPlayerData[pm];
                }
                else if (create)
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

        public static double GetBODSkill(Mobile m, SkillName skill)
        {
            return Math.Max(m.Skills[skill].Base, m.GetRacialSkillBonus(skill));
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
                BODEntry entry = context.Entries[type];

                if (entry != null)
                {
                    entry.CheckCache();

                    if (entry.LastBulkOrder + TimeSpan.FromHours(Delay) < DateTime.UtcNow)
                    {
                        entry.LastBulkOrder = DateTime.UtcNow;

                        return true;
                    }
                    else if (entry.CachedDeeds > 0)
                    {
                        entry.CachedDeeds--;

                        return true;
                    }
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
                        if (context.Entries[type].LastBulkOrder < DateTime.UtcNow - TimeSpan.FromHours(Delay * MaxCachedDeeds))
                            context.Entries[type].LastBulkOrder = DateTime.UtcNow - TimeSpan.FromHours(Delay * MaxCachedDeeds);
                        else
                            context.Entries[type].LastBulkOrder = (context.Entries[type].LastBulkOrder + ts) - TimeSpan.FromHours(Delay);
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
                    case BODType.Tailor:
                        if (doLarge) return new LargeTailorBOD();
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

        public static int GetBodHue(BODType type)
        {
            switch (type)
            {
                default:
                case BODType.Smith: return 0x44E;
                case BODType.Tailor: return 0x483;
                case BODType.Alchemy: return 2505;
                case BODType.Inscription: return 2598;
                case BODType.Tinkering: return 1109;
                case BODType.Cooking: return 1169;
                case BODType.Fletching: return 1425;
                case BODType.Carpentry: return 1512;
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

            banked = points * 0.02;
        }

        public static void ComputePoints(LargeBOD bod, out int points, out double banked)
        {
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

            banked = points * .2;
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

        public static bool CanClaimRewards(Mobile m)
        {
            BODContext context = GetContext(m);

            if (context != null)
            {
                return context.CanClaimRewards();
            }

            return true;
        }

        public static bool CanExchangeBOD(Mobile from, BaseVendor vendor, IBOD bod, int cost)
        {
            if (bod.BODType != vendor.BODType)
            {
                vendor.SayTo(from, 1152298, 0x3B2); // I don't deal in those goods.
                return false;
            }

            if ((bod is SmallBOD && ((SmallBOD)bod).AmountCur > 0) ||
                     (bod is LargeBOD && ((LargeBOD)bod).Entries != null &&
                     ((LargeBOD)bod).Entries.FirstOrDefault(e => e.Amount > 0) != null))
            {
                vendor.SayTo(from, 1152299, 0x3B2); // I am sorry to say I cannot work with a deed that is even partially filled.
                return false;
            }

            if (bod.AmountMax == 20 && (!CanBeExceptional(bod) || bod.RequireExceptional) &&
                     (!CanUseMaterial(bod) ||
                     (bod.Material == BulkMaterialType.Valorite ||
                      bod.Material == BulkMaterialType.Frostwood ||
                      bod.Material == BulkMaterialType.Barbed)))
            {
                vendor.SayTo(from, 1152291, 0x3B2); // I won't be able to replace that bulk order with a better one.
                return false;
            }

            if (cost > -1 && !Banker.Withdraw(from, cost, true))
            {
                vendor.SayTo(from, 1152302, 0x3B2); // I am afraid your bank box does not contain the funds needed to complete this transaction.
                return false;
            }

            return true;
        }

        public static Type GetTypeFromBOD(IBOD bod)
        {
            Type t = null;

            if (bod is SmallBOD)
            {
                t = ((SmallBOD)bod).Type;
            }
            else if (bod is LargeBOD && ((LargeBOD)bod).Entries != null && ((LargeBOD)bod).Entries.Length > 0)
            {
                t = ((LargeBOD)bod).Entries[0].Details.Type;
            }

            return t;
        }

        public static bool CanBeExceptional(IBOD bod)
        {
            switch (bod.BODType)
            {
                default: return true;
                case BODType.Alchemy:
                case BODType.Inscription: return false;
                case BODType.Tinkering:
                case BODType.Cooking:
                case BODType.Fletching:
                    return !IsInExceptionalExcludeList(bod);
            }
        }

        private static bool IsInExceptionalExcludeList(IBOD bod)
        {
            Type t = GetTypeFromBOD(bod);

            if (t == null)
                return false;

            return _ExceptionalExcluded.FirstOrDefault(type => type == t) != null;
        }

        public static bool CanUseMaterial(IBOD bod)
        {
            switch (bod.BODType)
            {
                default: return true;
                case BODType.Alchemy:
                case BODType.Inscription:
                case BODType.Cooking: return false;
                case BODType.Tinkering:
                case BODType.Fletching: return !IsInExceptionalExcludeList(bod);
                case BODType.Tailor: return BGTClassifier.Classify(BODType.Tailor, GetTypeFromBOD(bod)) == BulkGenericType.Leather;
            }
        }

        private static readonly Type[] _ExceptionalExcluded =
        {
            typeof(Arrow), typeof(Bolt), typeof(Kindling), typeof(Shaft),

            typeof(EnchantedApple), typeof(TribalPaint), typeof(GrapesOfWrath),
            typeof(EggBomb), typeof(CookedBird), typeof(FishSteak), typeof(FriedEggs),
            typeof(LambLeg), typeof(Ribs),

            typeof(Gears), typeof(Axle), typeof(Springs), typeof(AxleGears), typeof(ClockParts),
            typeof(Clock), typeof(PotionKeg), typeof(ClockFrame), typeof(MetalContainerEngraver)
        };

        public static void MutateBOD(IBOD bod)
        {
            List<int> picker = new List<int>();

            if (!bod.RequireExceptional && CanBeExceptional(bod))
            {
                picker.Add(0);
            }

            if (bod.AmountMax < 20)
            {
                picker.Add(1);
            }

            if (CanUseMaterial(bod) && bod.Material != BulkMaterialType.Frostwood && bod.Material != BulkMaterialType.Barbed && bod.Material != BulkMaterialType.Valorite)
            {
                picker.Add(2);
            }

            if (picker.Count == 0)
                return;

            switch (picker[Utility.Random(picker.Count)])
            {
                case 0: bod.RequireExceptional = true; break;
                case 1: bod.AmountMax += 5; break;
                case 2:
                    if (bod.Material == BulkMaterialType.None)
                    {
                        BulkGenericType type = BGTClassifier.Classify(bod.BODType, null);

                        switch (type)
                        {
                            case BulkGenericType.Iron: bod.Material = BulkMaterialType.DullCopper; break;
                            case BulkGenericType.Cloth: break;
                            case BulkGenericType.Leather: bod.Material = BulkMaterialType.Spined; break;
                            case BulkGenericType.Wood: bod.Material = BulkMaterialType.OakWood; break;
                        }
                    }
                    else
                    {
                        bod.Material++;
                    }
                    break;
            }

            picker.Clear();
        }

        public static int GetBribe(IBOD bod)
        {
            int worth = 0;

            if (bod.RequireExceptional)
            {
                worth += 400;
            }

            switch (bod.Material)
            {
                case BulkMaterialType.DullCopper: worth += 25; break;
                case BulkMaterialType.ShadowIron: worth += 50; break;
                case BulkMaterialType.Copper: worth += 100; break;
                case BulkMaterialType.Bronze: worth += 200; break;
                case BulkMaterialType.Gold: worth += 300; break;
                case BulkMaterialType.Agapite: worth += 400; break;
                case BulkMaterialType.Verite: worth += 500; break;
                case BulkMaterialType.Valorite: worth += 600; break;
                case BulkMaterialType.Spined: worth += 100; break;
                case BulkMaterialType.Horned: worth += 250; break;
                case BulkMaterialType.Barbed: worth += 500; break;
                case BulkMaterialType.OakWood: worth += 100; break;
                case BulkMaterialType.AshWood: worth += 200; break;
                case BulkMaterialType.YewWood: worth += 300; break;
                case BulkMaterialType.Heartwood: worth += 400; break;
                case BulkMaterialType.Bloodwood: worth += 500; break;
                case BulkMaterialType.Frostwood: worth += 600; break;
            }

            switch (bod.AmountMax)
            {
                default:
                case 10:
                case 15: worth += 100; break;
                case 20: worth += 250; break;
            }

            if (bod is LargeBOD && ((LargeBOD)bod).Entries != null)
            {
                worth *= Math.Min(4, ((LargeBOD)bod).Entries.Length);
            }

            return worth;
        }

        public static void OnTick()
        {
            foreach (KeyValuePair<PlayerMobile, BODContext> kvp in Instance.BODPlayerData)
            {
                kvp.Value.CheckCache();
            }
        }

        public static void OnLogin(LoginEventArgs e)
        {
        }

        public static string FilePath = Path.Combine("Saves/CraftContext", "BODs.bin");

        public static void Configure()
        {
            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;
        }

        public static void Initialize()
        {
            if (NewSystemEnabled)
            {
                Timer.DelayCall(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), OnTick);
            }
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

        public int GetPendingRewardFor(BODType type)
        {
            if (Entries.ContainsKey(type))
            {
                return Entries[type].PendingRewardPoints;
            }

            return 0;
        }

        public bool CanClaimRewards()
        {
            foreach (KeyValuePair<BODType, BODEntry> kvp in Entries)
            {
                if (kvp.Value.PendingRewardPoints > 0)
                {
                    return false;
                }
            }

            return true;
        }

        public BODContext(GenericReader reader)
        {
            int version = reader.ReadInt();
            ConfigEntries();

            PointsMode = (PointsMode)reader.ReadInt();
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

            writer.Write((int)PointsMode);
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

        public void CheckCache()
        {
            foreach (KeyValuePair<BODType, BODEntry> kvp in Entries)
            {
                kvp.Value.CheckCache();
            }
        }
    }

    [PropertyObject]
    public class BODEntry
    {
        private int _CachedDeeds;

        [CommandProperty(AccessLevel.GameMaster)]
        public int CachedDeeds
        {
            get { return _CachedDeeds; }
            set
            {
                _CachedDeeds = Math.Max(0, Math.Min(BulkOrderSystem.MaxCachedDeeds, value));
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastBulkOrder { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public double BankedPoints { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
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

        public override string ToString()
        {
            return "...";
        }

        public BODEntry()
        {
            _CachedDeeds = BulkOrderSystem.MaxCachedDeeds;
        }

        public void CheckCache()
        {
            if (_CachedDeeds >= BulkOrderSystem.MaxCachedDeeds)
            {
                return;
            }

            if (LastBulkOrder == DateTime.MinValue)
            {
                CachedDeeds = BulkOrderSystem.MaxCachedDeeds;
                LastBulkOrder = DateTime.UtcNow - TimeSpan.FromHours(BulkOrderSystem.Delay);
            }
            else if (LastBulkOrder + TimeSpan.FromHours(BulkOrderSystem.Delay) < DateTime.UtcNow)
            {
                CachedDeeds++;
                LastBulkOrder = LastBulkOrder + TimeSpan.FromHours(BulkOrderSystem.Delay);
            }
        }

        public BODEntry(GenericReader reader)
        {
            int version = reader.ReadInt();

            if (reader.ReadInt() == 0)
            {
                switch (version)
                {
                    case 1:
                    case 0:
                        _CachedDeeds = reader.ReadInt();
                        LastBulkOrder = reader.ReadDateTime();
                        BankedPoints = reader.ReadDouble();
                        PendingRewardPoints = reader.ReadInt();
                        break;
                }
            }
            else
            {
                _NextBulkOrder = reader.ReadDateTime();
            }

            if (version == 0 && _CachedDeeds > BulkOrderSystem.MaxCachedDeeds)
            {
                _CachedDeeds = BulkOrderSystem.MaxCachedDeeds;
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(1);

            if (BulkOrderSystem.NewSystemEnabled)
            {
                writer.Write(0);
                writer.Write(_CachedDeeds);
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

    public class PendingBribe
    {
        public IBOD BOD { get; set; }
        public int Amount { get; set; }

        public PendingBribe(IBOD bod, int amount)
        {
            BOD = bod;
            Amount = amount;
        }
    }
}
