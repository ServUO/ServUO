using Server.Items;
using System;
using System.Collections.Generic;

namespace Server.Engines.BulkOrders
{
    public delegate Item ConstructCallback(int type);

    public sealed class RewardType
    {
        private readonly int m_Points;
        private readonly Type[] m_Types;

        public int Points => m_Points;
        public Type[] Types => m_Types;

        public RewardType(int points, params Type[] types)
        {
            m_Points = points;
            m_Types = types;
        }

        public bool Contains(Type type)
        {
            for (int i = 0; i < m_Types.Length; ++i)
            {
                if (m_Types[i] == type)
                    return true;
            }

            return false;
        }
    }

    public sealed class RewardItem
    {
        private readonly int m_Weight;
        private readonly ConstructCallback m_Constructor;
        private readonly int m_Type;

        public int Weight => m_Weight;
        public ConstructCallback Constructor => m_Constructor;
        public int Type => m_Type;

        public RewardItem(int weight, ConstructCallback constructor)
            : this(weight, constructor, 0)
        {
        }

        public RewardItem(int weight, ConstructCallback constructor, int type)
        {
            m_Weight = weight;
            m_Constructor = constructor;
            m_Type = type;
        }

        public Item Construct()
        {
            try
            {
                return m_Constructor(m_Type);
            }
            catch (Exception e)
            {
                Diagnostics.ExceptionLogging.LogException(e);
                return null;
            }
        }
    }

    public class BODCollectionItem : CollectionItem
    {
        public ConstructCallback Constructor { get; set; }
        public int RewardType { get; set; }

        public BODCollectionItem(int itemID, TextDefinition tooltip, int hue, double points, ConstructCallback constructor, int type = 0)
            : base(null, itemID, tooltip, hue, points, false)
        {
            Constructor = constructor;
            RewardType = type;
        }
    }

    public sealed class RewardGroup
    {
        private readonly int m_Points;
        private readonly RewardItem[] m_Items;

        public int Points => m_Points;
        public RewardItem[] Items => m_Items;

        public RewardGroup(int points, params RewardItem[] items)
        {
            m_Points = points;
            m_Items = items;
        }

        public RewardItem AcquireItem()
        {
            if (m_Items.Length == 0)
                return null;
            else if (m_Items.Length == 1)
                return m_Items[0];

            int totalWeight = 0;

            for (int i = 0; i < m_Items.Length; ++i)
                totalWeight += m_Items[i].Weight;

            int randomWeight = Utility.Random(totalWeight);

            for (int i = 0; i < m_Items.Length; ++i)
            {
                RewardItem item = m_Items[i];

                if (randomWeight < item.Weight)
                    return item;

                randomWeight -= item.Weight;
            }

            return null;
        }
    }

    public abstract class RewardCalculator
    {
        private RewardGroup[] m_Groups;

        public RewardGroup[] Groups
        {
            get
            {
                return m_Groups;
            }
            set
            {
                m_Groups = value;
            }
        }

        public List<CollectionItem> RewardCollection { get; set; }

        public abstract int ComputePoints(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type);

        public abstract int ComputeGold(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type);

        public virtual int ComputeFame(SmallBOD bod)
        {
            int points = ComputePoints(bod) / 50;

            return points * points;
        }

        public virtual int ComputeFame(LargeBOD bod)
        {
            int points = ComputePoints(bod) / 50;

            return points * points;
        }

        public virtual int ComputePoints(SmallBOD bod)
        {
            return ComputePoints(bod.AmountMax, bod.RequireExceptional, bod.Material, 1, bod.Type);
        }

        public virtual int ComputePoints(LargeBOD bod)
        {
            Type type = bod.Entries == null || bod.Entries.Length == 0 ? null : bod.Entries[0].Details.Type;

            return ComputePoints(bod.AmountMax, bod.RequireExceptional, bod.Material, bod.Entries.Length, type);
        }

        public virtual int ComputeGold(SmallBOD bod)
        {
            return ComputeGold(bod.AmountMax, bod.RequireExceptional, bod.Material, 1, bod.Type);
        }

        public virtual int ComputeGold(LargeBOD bod)
        {
            return ComputeGold(bod.AmountMax, bod.RequireExceptional, bod.Material, bod.Entries.Length, bod.Entries[0].Details.Type);
        }

        public virtual RewardGroup LookupRewards(int points)
        {
            for (int i = m_Groups.Length - 1; i >= 1; --i)
            {
                RewardGroup group = m_Groups[i];

                if (points >= group.Points)
                    return group;
            }

            return m_Groups[0];
        }

        public virtual int LookupTypePoints(RewardType[] types, Type type)
        {
            for (int i = 0; i < types.Length; ++i)
            {
                if (type == null || types[i].Contains(type))
                    return types[i].Points;
            }

            return 0;
        }

        protected static Item RewardTitle(int type)
        {
            return new BODRewardTitleDeed(type);
        }

        protected static Item NaturalDye(int type)
        {
            switch (type)
            {
                default:
                case 0: return new SpecialNaturalDye(DyeType.WindAzul);
                case 1: return new SpecialNaturalDye(DyeType.DullRuby);
                case 2: return new SpecialNaturalDye(DyeType.PoppieWhite);
                case 3: return new SpecialNaturalDye(DyeType.WindAzul, true);
                case 4: return new SpecialNaturalDye(DyeType.UmbranViolet, true);
                case 5: return new SpecialNaturalDye(DyeType.ZentoOrchid, true);
                case 6: return new SpecialNaturalDye(DyeType.DullRuby, true);
                case 7: return new SpecialNaturalDye(DyeType.PoppieWhite, true);
                case 8: return new SpecialNaturalDye(DyeType.UmbranViolet);
                case 9: return new SpecialNaturalDye(DyeType.ZentoOrchid);
            }
        }

        protected static Item RockHammer(int type)
        {
            return new RockHammer();
        }

        protected static Item HarvestMap(int type)
        {
            return new HarvestMap((CraftResource)type);
        }

        protected static Item Recipe(int type)
        {
            switch (type)
            {
                case 0: return new RecipeScroll(170);
                case 1: return new RecipeScroll(457);
                case 2: return new RecipeScroll(458);
                case 3: return new RecipeScroll(800);
                case 4: return new RecipeScroll(599);
            }

            return null;
        }

        protected static Item SmeltersTalisman(int type)
        {
            return new SmeltersTalisman((CraftResource)type);
        }

        protected static Item WoodsmansTalisman(int type)
        {
            return new WoodsmansTalisman((CraftResource)type);
        }
    }

    #region Smith Rewards
    public sealed class SmithRewardCalculator : RewardCalculator
    {
        public SmithRewardCalculator()
        {
            if (BulkOrderSystem.NewSystemEnabled)
            {
                RewardCollection = new List<CollectionItem>();

                RewardCollection.Add(new BODCollectionItem(0x13E3, 1157219, 0, 10, SmithHammer));
                RewardCollection.Add(new BODCollectionItem(0xF39, 1157084, 0x973, 10, SturdyShovel));
                RewardCollection.Add(new BODCollectionItem(0xE86, 1157085, 0x973, 25, SturdyPickaxe));
                RewardCollection.Add(new BODCollectionItem(0x14F0, 1157181, 0, 25, RewardTitle, 0));
                RewardCollection.Add(new BODCollectionItem(0x14F0, 1157182, 0, 100, RewardTitle, 1));
                RewardCollection.Add(new BODCollectionItem(0x13C6, 1157086, 0, 100, MiningGloves, 1));
                RewardCollection.Add(new BODCollectionItem(0x13D5, 1157087, 0, 200, MiningGloves, 3));
                RewardCollection.Add(new BODCollectionItem(0xFB4, 1157090, 0, 200, ProspectorsTool));
                RewardCollection.Add(new BODCollectionItem(0xE86, 1157089, 0, 200, GargoylesPickaxe));
                RewardCollection.Add(new BODCollectionItem(0x2F5B, 1152674, CraftResources.GetHue(CraftResource.Gold), 350, SmeltersTalisman, (int)CraftResource.Gold));
                RewardCollection.Add(new BODCollectionItem(0x9E2A, 1157264, 0, 400, CraftsmanTalisman, 10));
                RewardCollection.Add(new BODCollectionItem(0x13EB, 1157088, 0, 450, MiningGloves, 5));
                RewardCollection.Add(new BODCollectionItem(4102, 1157091, 0, 450, PowderOfTemperament));
                RewardCollection.Add(new BODCollectionItem(0x2F5B, 1152675, CraftResources.GetHue(CraftResource.Agapite), 475, SmeltersTalisman, (int)CraftResource.Agapite));
                RewardCollection.Add(new BODCollectionItem(0x9E7E, 1157216, 0, 500, RockHammer));
                RewardCollection.Add(new BODCollectionItem(0x13E3, 1157092, CraftResources.GetHue(CraftResource.DullCopper), 500, RunicHammer, 1));
                RewardCollection.Add(new BODCollectionItem(0x2F5B, 1152676, CraftResources.GetHue(CraftResource.Verite), 525, SmeltersTalisman, (int)CraftResource.Verite));
                RewardCollection.Add(new BODCollectionItem(0x13E3, 1157093, CraftResources.GetHue(CraftResource.ShadowIron), 550, RunicHammer, 2));
                RewardCollection.Add(new BODCollectionItem(0x9E2A, 1157218, 0, 550, CraftsmanTalisman, 25));
                RewardCollection.Add(new BODCollectionItem(0x2F5B, 1152677, CraftResources.GetHue(CraftResource.Valorite), 575, SmeltersTalisman, (int)CraftResource.Valorite));
                RewardCollection.Add(new BODCollectionItem(0x14EC, 1152665, CraftResources.GetHue(CraftResource.Gold), 600, HarvestMap, (int)CraftResource.Gold));
                RewardCollection.Add(new BODCollectionItem(0xFAF, 1157100, 0, 625, ColoredAnvil));
                RewardCollection.Add(new BODCollectionItem(0x14F0, 1157105, 0x481, 625, PowerScroll, 5));
                RewardCollection.Add(new BODCollectionItem(0x13E3, 1157094, CraftResources.GetHue(CraftResource.Copper), 650, RunicHammer, 3));
                RewardCollection.Add(new BODCollectionItem(0x14F0, 1157106, 0x481, 675, PowerScroll, 10));
                RewardCollection.Add(new BODCollectionItem(0x13E3, 1157095, CraftResources.GetHue(CraftResource.Bronze), 700, RunicHammer, 4));
                RewardCollection.Add(new BODCollectionItem(0x13E3, 1157101, 0x482, 750, AncientHammer, 10));
                RewardCollection.Add(new BODCollectionItem(0x14F0, 1157107, 0x481, 800, PowerScroll, 15));
                RewardCollection.Add(new BODCollectionItem(0x13E3, 1157102, 0x482, 850, AncientHammer, 15));
                RewardCollection.Add(new BODCollectionItem(0x14EC, 1152666, CraftResources.GetHue(CraftResource.Agapite), 850, HarvestMap, (int)CraftResource.Agapite));
                RewardCollection.Add(new BODCollectionItem(0x14F0, 1157108, 0x481, 900, PowerScroll, 20));
                RewardCollection.Add(new BODCollectionItem(0x9E2A, 1157265, 0, 900, CraftsmanTalisman, 50));
                RewardCollection.Add(new BODCollectionItem(0x13E3, 1157096, CraftResources.GetHue(CraftResource.Gold), 950, RunicHammer, 5));
                RewardCollection.Add(new BODCollectionItem(0x14EC, 1152667, CraftResources.GetHue(CraftResource.Verite), 950, HarvestMap, (int)CraftResource.Verite));
                RewardCollection.Add(new BODCollectionItem(0x13E3, 1157103, 0x482, 1000, AncientHammer, 30));
                RewardCollection.Add(new BODCollectionItem(0x13E3, 1157097, CraftResources.GetHue(CraftResource.Agapite), 1050, RunicHammer, 6));
                RewardCollection.Add(new BODCollectionItem(0x14EC, 1152668, CraftResources.GetHue(CraftResource.Valorite), 1050, HarvestMap, (int)CraftResource.Valorite));
                RewardCollection.Add(new BODCollectionItem(0x13E3, 1157104, 0x482, 1100, AncientHammer, 60));
                RewardCollection.Add(new BODCollectionItem(0x13E3, 1157098, CraftResources.GetHue(CraftResource.Verite), 1150, RunicHammer, 7));
                RewardCollection.Add(new BODCollectionItem(0x13E3, 1157099, CraftResources.GetHue(CraftResource.Valorite), 1200, RunicHammer, 8));
            }
            else
            {
                Groups = new RewardGroup[]
                {
                    new RewardGroup(0, new RewardItem(1, SturdyShovel)),
                    new RewardGroup(25, new RewardItem(1, SturdyPickaxe)),
                    new RewardGroup(50, new RewardItem(45, SturdyShovel), new RewardItem(45, SturdyPickaxe), new RewardItem(10, MiningGloves, 1)),
                    new RewardGroup(200, new RewardItem(45, GargoylesPickaxe), new RewardItem(45, ProspectorsTool), new RewardItem(10, MiningGloves, 3)),
                    new RewardGroup(400, new RewardItem(2, GargoylesPickaxe), new RewardItem(2, ProspectorsTool), new RewardItem(1, PowderOfTemperament)),
                    new RewardGroup(450, new RewardItem(9, PowderOfTemperament), new RewardItem(1, MiningGloves, 5)),
                    new RewardGroup(500, new RewardItem(1, RunicHammer, 1)),
                    new RewardGroup(550, new RewardItem(3, RunicHammer, 1), new RewardItem(2, RunicHammer, 2)),
                    new RewardGroup(600, new RewardItem(1, RunicHammer, 2)),
                    new RewardGroup(625, new RewardItem(3, RunicHammer, 2), new RewardItem(6, PowerScroll, 5), new RewardItem(1, ColoredAnvil)),
                    new RewardGroup(650, new RewardItem(1, RunicHammer, 3)),
                    new RewardGroup(675, new RewardItem(1, ColoredAnvil), new RewardItem(6, PowerScroll, 10), new RewardItem(3, RunicHammer, 3)),
                    new RewardGroup(700, new RewardItem(1, RunicHammer, 4)),
                    new RewardGroup(750, new RewardItem(1, AncientHammer, 10)),
                    new RewardGroup(800, new RewardItem(1, PowerScroll, 15)),
                    new RewardGroup(850, new RewardItem(1, AncientHammer, 15)),
                    new RewardGroup(900, new RewardItem(1, PowerScroll, 20)),
                    new RewardGroup(950, new RewardItem(1, RunicHammer, 5)),
                    new RewardGroup(1000, new RewardItem(1, AncientHammer, 30)),
                    new RewardGroup(1050, new RewardItem(1, RunicHammer, 6)),
                    new RewardGroup(1100, new RewardItem(1, AncientHammer, 60)),
                    new RewardGroup(1150, new RewardItem(1, RunicHammer, 7)),
                    new RewardGroup(1200, new RewardItem(1, RunicHammer, 8))
                };
            }
        }

        #region Constructors
        private static readonly ConstructCallback SmithHammer = CreateSmithHammer;
        private static readonly ConstructCallback SturdyShovel = CreateSturdyShovel;
        private static readonly ConstructCallback SturdyPickaxe = CreateSturdyPickaxe;
        private static readonly ConstructCallback MiningGloves = CreateMiningGloves;
        private static readonly ConstructCallback GargoylesPickaxe = CreateGargoylesPickaxe;
        private static readonly ConstructCallback ProspectorsTool = CreateProspectorsTool;
        private static readonly ConstructCallback PowderOfTemperament = CreatePowderOfTemperament;
        private static readonly ConstructCallback RunicHammer = CreateRunicHammer;
        private static readonly ConstructCallback PowerScroll = CreatePowerScroll;
        private static readonly ConstructCallback ColoredAnvil = CreateColoredAnvil;
        private static readonly ConstructCallback AncientHammer = CreateAncientHammer;

        private static Item CreateSmithHammer(int type)
        {
            SmithHammer hammer = new SmithHammer
            {
                UsesRemaining = 250
            };

            return hammer;
        }

        private static Item CreateSturdyShovel(int type)
        {
            return new SturdyShovel();
        }

        private static Item CreateSturdyPickaxe(int type)
        {
            return new SturdyPickaxe();
        }

        private static Item CreateMiningGloves(int type)
        {
            if (type == 1)
                return new LeatherGlovesOfMining(1);
            else if (type == 3)
                return new StuddedGlovesOfMining(3);
            else if (type == 5)
                return new RingmailGlovesOfMining(5);

            throw new InvalidOperationException();
        }

        private static Item CreateGargoylesPickaxe(int type)
        {
            return new GargoylesPickaxe();
        }

        private static Item CreateProspectorsTool(int type)
        {
            return new ProspectorsTool();
        }

        private static Item CreatePowderOfTemperament(int type)
        {
            return new PowderOfTemperament();
        }

        private static Item CreateRunicHammer(int type)
        {
            if (type >= 1 && type <= 8)
                return new RunicHammer(CraftResource.Iron + type, 55 - (type * 5));

            throw new InvalidOperationException();
        }

        private static Item CreatePowerScroll(int type)
        {
            if (type == 5 || type == 10 || type == 15 || type == 20)
                return new PowerScroll(SkillName.Blacksmith, 100 + type);

            throw new InvalidOperationException();
        }

        private static Item CreateColoredAnvil(int type)
        {
            return new ColoredAnvil();
        }

        private static Item CreateAncientHammer(int type)
        {
            if (type == 10 || type == 15 || type == 30 || type == 60)
                return new AncientSmithyHammer(type);

            throw new InvalidOperationException();
        }

        private static Item CraftsmanTalisman(int type)
        {
            return new MasterCraftsmanTalisman(type, 0x9E2A, TalismanSkill.Blacksmithy);
        }
        #endregion

        public static readonly SmithRewardCalculator Instance = new SmithRewardCalculator();

        private readonly RewardType[] m_Types = new RewardType[]
        {
            // Armors
            new RewardType(200, typeof(RingmailGloves), typeof(RingmailChest), typeof(RingmailArms), typeof(RingmailLegs)),
            new RewardType(300, typeof(ChainCoif), typeof(ChainLegs), typeof(ChainChest)),
            new RewardType(400, typeof(PlateArms), typeof(PlateLegs), typeof(PlateHelm), typeof(PlateGorget), typeof(PlateGloves), typeof(PlateChest)),

            // Weapons
            new RewardType(200, typeof(Bardiche), typeof(Halberd)),
            new RewardType(300, typeof(Dagger), typeof(ShortSpear), typeof(Spear), typeof(WarFork), typeof(Kryss)), //OSI put the dagger in there.  Odd, ain't it.
            new RewardType(350, typeof(Axe), typeof(BattleAxe), typeof(DoubleAxe), typeof(ExecutionersAxe), typeof(LargeBattleAxe), typeof(TwoHandedAxe)),
            new RewardType(350, typeof(Broadsword), typeof(Cutlass), typeof(Katana), typeof(Longsword), typeof(Scimitar), /*typeof( ThinLongsword ),*/ typeof(VikingSword)),
            new RewardType(350, typeof(WarAxe), typeof(HammerPick), typeof(Mace), typeof(Maul), typeof(WarHammer), typeof(WarMace))
        };

        public override int ComputePoints(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type)
        {
            int points = 0;

            if (quantity == 10)
                points += 10;
            else if (quantity == 15)
                points += 25;
            else if (quantity == 20)
                points += 50;

            if (exceptional)
                points += 200;

            if (itemCount > 1)
                points += LookupTypePoints(m_Types, type);

            if (material >= BulkMaterialType.DullCopper && material <= BulkMaterialType.Valorite)
                points += 200 + (50 * (material - BulkMaterialType.DullCopper));

            return points;
        }

        private static readonly int[][][] m_GoldTable = new int[][][]
        {
            new int[][] // 1-part (regular)
            {
                new int[] { 150, 250, 250, 400, 400, 750, 750, 1200, 1200 },
                new int[] { 225, 375, 375, 600, 600, 1125, 1125, 1800, 1800 },
                new int[] { 300, 500, 750, 800, 1050, 1500, 2250, 2400, 4000 }
            },
            new int[][] // 1-part (exceptional)
            {
                new int[] { 250, 400, 400, 750, 750, 1500, 1500, 3000, 3000 },
                new int[] { 375, 600, 600, 1125, 1125, 2250, 2250, 4500, 4500 },
                new int[] { 500, 800, 1200, 1500, 2500, 3000, 6000, 6000, 12000 }
            },
            new int[][] // Ringmail (regular)
            {
                new int[] { 3000, 5000, 5000, 7500, 7500, 10000, 10000, 15000, 15000 },
                new int[] { 4500, 7500, 7500, 11250, 11500, 15000, 15000, 22500, 22500 },
                new int[] { 6000, 10000, 15000, 15000, 20000, 20000, 30000, 30000, 50000 }
            },
            new int[][] // Ringmail (exceptional)
            {
                new int[] { 5000, 10000, 10000, 15000, 15000, 25000, 25000, 50000, 50000 },
                new int[] { 7500, 15000, 15000, 22500, 22500, 37500, 37500, 75000, 75000 },
                new int[] { 10000, 20000, 30000, 30000, 50000, 50000, 100000, 100000, 200000 }
            },
            new int[][] // Chainmail (regular)
            {
                new int[] { 4000, 7500, 7500, 10000, 10000, 15000, 15000, 25000, 25000 },
                new int[] { 6000, 11250, 11250, 15000, 15000, 22500, 22500, 37500, 37500 },
                new int[] { 8000, 15000, 20000, 20000, 30000, 30000, 50000, 50000, 100000 }
            },
            new int[][] // Chainmail (exceptional)
            {
                new int[] { 7500, 15000, 15000, 25000, 25000, 50000, 50000, 100000, 100000 },
                new int[] { 11250, 22500, 22500, 37500, 37500, 75000, 75000, 150000, 150000 },
                new int[] { 15000, 30000, 50000, 50000, 100000, 100000, 200000, 200000, 200000 }
            },
            new int[][] // Platemail (regular)
            {
                new int[] { 5000, 10000, 10000, 15000, 15000, 25000, 25000, 50000, 50000 },
                new int[] { 7500, 15000, 15000, 22500, 22500, 37500, 37500, 75000, 75000 },
                new int[] { 10000, 20000, 30000, 30000, 50000, 50000, 100000, 100000, 200000 }
            },
            new int[][] // Platemail (exceptional)
            {
                new int[] { 10000, 25000, 25000, 50000, 50000, 100000, 100000, 100000, 100000 },
                new int[] { 15000, 37500, 37500, 75000, 75000, 150000, 150000, 150000, 150000 },
                new int[] { 20000, 50000, 100000, 100000, 200000, 200000, 200000, 200000, 200000 }
            },
            new int[][] // 2-part weapons (regular)
            {
                new int[] { 3000, 0, 0, 0, 0, 0, 0, 0, 0 },
                new int[] { 4500, 0, 0, 0, 0, 0, 0, 0, 0 },
                new int[] { 6000, 0, 0, 0, 0, 0, 0, 0, 0 }
            },
            new int[][] // 2-part weapons (exceptional)
            {
                new int[] { 5000, 0, 0, 0, 0, 0, 0, 0, 0 },
                new int[] { 7500, 0, 0, 0, 0, 0, 0, 0, 0 },
                new int[] { 10000, 0, 0, 0, 0, 0, 0, 0, 0 }
            },
            new int[][] // 5-part weapons (regular)
            {
                new int[] { 4000, 0, 0, 0, 0, 0, 0, 0, 0 },
                new int[] { 6000, 0, 0, 0, 0, 0, 0, 0, 0 },
                new int[] { 8000, 0, 0, 0, 0, 0, 0, 0, 0 }
            },
            new int[][] // 5-part weapons (exceptional)
            {
                new int[] { 7500, 0, 0, 0, 0, 0, 0, 0, 0 },
                new int[] { 11250, 0, 0, 0, 0, 0, 0, 0, 0 },
                new int[] { 15000, 0, 0, 0, 0, 0, 0, 0, 0 }
            },
            new int[][] // 6-part weapons (regular)
            {
                new int[] { 4000, 0, 0, 0, 0, 0, 0, 0, 0 },
                new int[] { 6000, 0, 0, 0, 0, 0, 0, 0, 0 },
                new int[] { 10000, 0, 0, 0, 0, 0, 0, 0, 0 }
            },
            new int[][] // 6-part weapons (exceptional)
            {
                new int[] { 7500, 0, 0, 0, 0, 0, 0, 0, 0 },
                new int[] { 11250, 0, 0, 0, 0, 0, 0, 0, 0 },
                new int[] { 15000, 0, 0, 0, 0, 0, 0, 0, 0 }
            }
        };

        private int ComputeType(Type type, int itemCount)
        {
            // Item count of 1 means it's a small BOD.
            if (itemCount == 1)
                return 0;

            int typeIdx;

            // Loop through the RewardTypes defined earlier and find the correct one.
            for (typeIdx = 0; typeIdx < 7; ++typeIdx)
            {
                if (m_Types[typeIdx].Contains(type))
                    break;
            }

            // Types 5, 6 and 7 are Large Weapon BODs with the same rewards.
            if (typeIdx > 5)
                typeIdx = 5;

            return (typeIdx + 1) * 2;
        }

        public override int ComputeGold(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type)
        {
            int gold = 0;

            if (itemCount == 1 && BulkOrderSystem.NewSystemEnabled && BulkOrderSystem.ComputeGold(type, quantity, out gold))
            {
                return gold;
            }

            int[][][] goldTable = m_GoldTable;

            int typeIndex = ComputeType(type, itemCount);
            int quanIndex = (quantity == 20 ? 2 : quantity == 15 ? 1 : 0);
            int mtrlIndex = (material >= BulkMaterialType.DullCopper && material <= BulkMaterialType.Valorite) ? 1 + (material - BulkMaterialType.DullCopper) : 0;

            if (exceptional)
                typeIndex++;

            gold = goldTable[typeIndex][quanIndex][mtrlIndex];

            int min = (gold * 9) / 10;
            int max = (gold * 10) / 9;

            return Utility.RandomMinMax(min, max);
        }
    }
    #endregion

    #region Tailor Rewards
    public sealed class TailorRewardCalculator : RewardCalculator
    {
        public TailorRewardCalculator()
        {
            if (BulkOrderSystem.NewSystemEnabled)
            {
                RewardCollection = new List<CollectionItem>();

                RewardCollection.Add(new BODCollectionItem(0xF9D, 1157219, 0, 10, SewingKit));
                RewardCollection.Add(new BODCollectionItem(0x14F0, 1157183, 0, 10, RewardTitle, 2));
                RewardCollection.Add(new BODCollectionItem(0x1765, 1157109, 0, 10, Cloth, 0));
                RewardCollection.Add(new BODCollectionItem(0x14F0, 1157184, 0, 25, RewardTitle, 3));
                RewardCollection.Add(new BODCollectionItem(0x1761, 1157109, 0, 50, Cloth, 1));
                RewardCollection.Add(new BODCollectionItem(0x14F0, 1157185, 0, 100, RewardTitle, 4));
                RewardCollection.Add(new BODCollectionItem(0x1765, 1157109, 0, 100, Cloth, 2));
                RewardCollection.Add(new BODCollectionItem(0x1765, 1157109, 0, 150, Cloth, 3));
                RewardCollection.Add(new BODCollectionItem(0x170D, 1157110, 0, 150, Sandals, 3));
                RewardCollection.Add(new BODCollectionItem(0x1765, 1157109, 0, 200, Cloth, 4));
                RewardCollection.Add(new BODCollectionItem(0x9E25, 1157264, 0, 200, CraftsmanTalisman, 10)); // todo: Get id
                RewardCollection.Add(new BODCollectionItem(0x14F0, 1157111, 0, 300, StretchedHide));
                RewardCollection.Add(new BODCollectionItem(0x1765, 1157109, 0, 300, Cloth, 5)); // TODO: Get other 4 colors
                RewardCollection.Add(new BODCollectionItem(0x9E25, 1157218, 0, 300, CraftsmanTalisman, 25)); // todo: Get id
                RewardCollection.Add(new BODCollectionItem(0xF9D, 1157115, CraftResources.GetHue(CraftResource.SpinedLeather), 350, RunicKit, 1));
                RewardCollection.Add(new BODCollectionItem(0x9E25, 1157265, 0, 350, CraftsmanTalisman, 50)); // todo: Get id
                RewardCollection.Add(new BODCollectionItem(0x14F0, 1157118, 0x481, 400, PowerScroll, 5));
                RewardCollection.Add(new BODCollectionItem(0x14F0, 1157112, 0, 400, Tapestry));
                RewardCollection.Add(new BODCollectionItem(0x14F0, 1157113, 0, 450, BearRug));
                RewardCollection.Add(new BODCollectionItem(0x14F0, 1157119, 0x481, 500, PowerScroll, 10));
                RewardCollection.Add(new BODCollectionItem(0x14F0, 1157114, 0, 550, ClothingBlessDeed));
                RewardCollection.Add(new BODCollectionItem(0x14F0, 1157120, 0x481, 575, PowerScroll, 15));
                RewardCollection.Add(new BODCollectionItem(0xF9D, 1157116, CraftResources.GetHue(CraftResource.HornedLeather), 600, RunicKit, 2));
                RewardCollection.Add(new BODCollectionItem(0x14F0, 1157121, 0x481, 650, PowerScroll, 20));
                RewardCollection.Add(new BODCollectionItem(0xF9D, 1157117, CraftResources.GetHue(CraftResource.BarbedLeather), 700, RunicKit, 3));
            }
            else
            {
                Groups = new RewardGroup[]
                {
                    new RewardGroup(0, new RewardItem(1, Cloth, 0)),
                    new RewardGroup(50, new RewardItem(1, Cloth, 1)),
                    new RewardGroup(100, new RewardItem(1, Cloth, 2)),
                    new RewardGroup(150, new RewardItem(9, Cloth, 3), new RewardItem(1, Sandals)),
                    new RewardGroup(200, new RewardItem(4, Cloth, 4), new RewardItem(1, Sandals)),
                    new RewardGroup(300, new RewardItem(1, StretchedHide)),
                    new RewardGroup(350, new RewardItem(1, RunicKit, 1)),
                    new RewardGroup(400, new RewardItem(2, PowerScroll, 5), new RewardItem(3, Tapestry)),
                    new RewardGroup(450, new RewardItem(1, BearRug)),
                    new RewardGroup(500, new RewardItem(1, PowerScroll, 10)),
                    new RewardGroup(550, new RewardItem(1, ClothingBlessDeed)),
                    new RewardGroup(575, new RewardItem(1, PowerScroll, 15)),
                    new RewardGroup(600, new RewardItem(1, RunicKit, 2)),
                    new RewardGroup(650, new RewardItem(1, PowerScroll, 20)),
                    new RewardGroup(700, new RewardItem(1, RunicKit, 3))
                };
            }
        }

        #region Constructors
        private static readonly ConstructCallback SewingKit = CreateSewingKit;
        private static readonly ConstructCallback Cloth = CreateCloth;
        private static readonly ConstructCallback Sandals = CreateSandals;
        private static readonly ConstructCallback StretchedHide = CreateStretchedHide;
        private static readonly ConstructCallback RunicKit = CreateRunicKit;
        private static readonly ConstructCallback Tapestry = CreateTapestry;
        private static readonly ConstructCallback PowerScroll = CreatePowerScroll;
        private static readonly ConstructCallback BearRug = CreateBearRug;
        private static readonly ConstructCallback ClothingBlessDeed = CreateCBD;
        private static readonly ConstructCallback CraftsmanTalisman = CreateCraftsmanTalisman;

        private static Item CreateSewingKit(int type)
        {
            SewingKit kit = new SewingKit
            {
                UsesRemaining = 250
            };

            return kit;
        }

        private static readonly int[][] m_ClothHues = new int[][]
        {
            new int[] { 0x483, 0x48C, 0x488, 0x48A },
            new int[] { 0x495, 0x48B, 0x486, 0x485 },
            new int[] { 0x48D, 0x490, 0x48E, 0x491 },
            new int[] { 0x48F, 0x494, 0x484, 0x497 },
            new int[] { 0x489, 0x47F, 0x482, 0x47E },
            new int[] { 0xAAC, 0xAB4, 0xAAF, 0xAB5, 0xAAB },
        };

        private static Item CreateCloth(int type)
        {
            if (type >= 0 && type < m_ClothHues.Length)
            {
                UncutCloth cloth = new UncutCloth(100)
                {
                    Hue = m_ClothHues[type][Utility.Random(m_ClothHues[type].Length)]
                };
                return cloth;
            }

            throw new InvalidOperationException();
        }

        private static readonly int[] m_SandalHues = new int[]
        {
            0x489, 0x47F, 0x482,
            0x47E, 0x48F, 0x494,
            0x484, 0x497
        };

        private static Item CreateSandals(int type)
        {
            return new Sandals(m_SandalHues[Utility.Random(m_SandalHues.Length)]);
        }

        private static Item CreateStretchedHide(int type)
        {
            switch (Utility.Random(4))
            {
                default:
                case 0:
                    return new SmallStretchedHideEastDeed();
                case 1:
                    return new SmallStretchedHideSouthDeed();
                case 2:
                    return new MediumStretchedHideEastDeed();
                case 3:
                    return new MediumStretchedHideSouthDeed();
            }
        }

        private static Item CreateTapestry(int type)
        {
            switch (Utility.Random(4))
            {
                default:
                case 0:
                    return new LightFlowerTapestryEastDeed();
                case 1:
                    return new LightFlowerTapestrySouthDeed();
                case 2:
                    return new DarkFlowerTapestryEastDeed();
                case 3:
                    return new DarkFlowerTapestrySouthDeed();
            }
        }

        private static Item CreateBearRug(int type)
        {
            switch (Utility.Random(4))
            {
                default:
                case 0:
                    return new BrownBearRugEastDeed();
                case 1:
                    return new BrownBearRugSouthDeed();
                case 2:
                    return new PolarBearRugEastDeed();
                case 3:
                    return new PolarBearRugSouthDeed();
            }
        }

        private static Item CreateRunicKit(int type)
        {
            if (type >= 1 && type <= 3)
                return new RunicSewingKit(CraftResource.RegularLeather + type, 60 - (type * 15));

            throw new InvalidOperationException();
        }

        private static Item CreatePowerScroll(int type)
        {
            if (type == 5 || type == 10 || type == 15 || type == 20)
                return new PowerScroll(SkillName.Tailoring, 100 + type);

            throw new InvalidOperationException();
        }

        private static Item CreateCBD(int type)
        {
            return new ClothingBlessDeed();
        }

        private static Item CreateCraftsmanTalisman(int type)
        {
            return new MasterCraftsmanTalisman(type, 0x9E25, TalismanSkill.Tailoring);
        }

        #endregion

        public static readonly TailorRewardCalculator Instance = new TailorRewardCalculator();

        public override int ComputePoints(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type)
        {
            int points = 0;

            if (quantity == 10)
                points += 10;
            else if (quantity == 15)
                points += 25;
            else if (quantity == 20)
                points += 50;

            if (exceptional)
                points += 100;

            if (itemCount == 4)
                points += 300;
            else if (itemCount == 5)
                points += 400;
            else if (itemCount == 6)
                points += 500;

            if (material == BulkMaterialType.Spined)
                points += 50;
            else if (material == BulkMaterialType.Horned)
                points += 100;
            else if (material == BulkMaterialType.Barbed)
                points += 150;

            return points;
        }

        private static readonly int[][][] m_AosGoldTable = new int[][][]
        {
            new int[][] // 1-part (regular)
            {
                new int[] { 150, 150, 300, 300 },
                new int[] { 225, 225, 450, 450 },
                new int[] { 300, 400, 600, 750 }
            },
            new int[][] // 1-part (exceptional)
            {
                new int[] { 300, 300, 600, 600 },
                new int[] { 450, 450, 900, 900 },
                new int[] { 600, 750, 1200, 1800 }
            },
            new int[][] // 4-part (regular)
            {
                new int[] { 4000, 4000, 5000, 5000 },
                new int[] { 6000, 6000, 7500, 7500 },
                new int[] { 8000, 10000, 10000, 15000 }
            },
            new int[][] // 4-part (exceptional)
            {
                new int[] { 5000, 5000, 7500, 7500 },
                new int[] { 7500, 7500, 11250, 11250 },
                new int[] { 10000, 15000, 15000, 20000 }
            },
            new int[][] // 5-part (regular)
            {
                new int[] { 5000, 5000, 7500, 7500 },
                new int[] { 7500, 7500, 11250, 11250 },
                new int[] { 10000, 15000, 15000, 20000 }
            },
            new int[][] // 5-part (exceptional)
            {
                new int[] { 7500, 7500, 10000, 10000 },
                new int[] { 11250, 11250, 15000, 15000 },
                new int[] { 15000, 20000, 20000, 30000 }
            },
            new int[][] // 6-part (regular)
            {
                new int[] { 7500, 7500, 10000, 10000 },
                new int[] { 11250, 11250, 15000, 15000 },
                new int[] { 15000, 20000, 20000, 30000 }
            },
            new int[][] // 6-part (exceptional)
            {
                new int[] { 10000, 10000, 15000, 15000 },
                new int[] { 15000, 15000, 22500, 22500 },
                new int[] { 20000, 30000, 30000, 50000 }
            }
        };

        public override int ComputeGold(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type)
        {
            int gold = 0;

            if (itemCount == 1 && BulkOrderSystem.NewSystemEnabled && BulkOrderSystem.ComputeGold(type, quantity, out gold))
            {
                return gold;
            }

            int[][][] goldTable = m_AosGoldTable;

            int typeIndex = ((itemCount == 6 ? 3 : itemCount == 5 ? 2 : itemCount == 4 ? 1 : 0) * 2) + (exceptional ? 1 : 0);
            int quanIndex = (quantity == 20 ? 2 : quantity == 15 ? 1 : 0);
            int mtrlIndex = (material == BulkMaterialType.Barbed ? 3 : material == BulkMaterialType.Horned ? 2 : material == BulkMaterialType.Spined ? 1 : 0);

            gold = goldTable[typeIndex][quanIndex][mtrlIndex];

            int min = (gold * 9) / 10;
            int max = (gold * 10) / 9;

            return Utility.RandomMinMax(min, max);
        }
    }
    #endregion

    #region Tinkering Rewards
    public sealed class TinkeringRewardCalculator : RewardCalculator
    {
        public TinkeringRewardCalculator()
        {
            RewardCollection = new List<CollectionItem>();

            RewardCollection.Add(new BODCollectionItem(0x1EBC, 1157219, 0, 10, TinkerTools));
            RewardCollection.Add(new BODCollectionItem(0x14F0, 1157186, 0, 25, RewardTitle, 5));
            RewardCollection.Add(new BODCollectionItem(0x14F0, 1157187, 0, 50, RewardTitle, 6));
            RewardCollection.Add(new BODCollectionItem(0x14F0, 1157190, 0, 210, RewardTitle, 9));
            RewardCollection.Add(new BODCollectionItem(0x2831, 1157288, 0, 225, Recipe, 0));
            RewardCollection.Add(new BODCollectionItem(0x14F0, 1157188, 0, 250, RewardTitle, 7));
            RewardCollection.Add(new BODCollectionItem(0x2831, 1157287, 0, 310, Recipe, 1));
            RewardCollection.Add(new BODCollectionItem(0x14F0, 1157189, 0, 225, RewardTitle, 8));
            RewardCollection.Add(new BODCollectionItem(0x2831, 1157289, 0, 350, Recipe, 2));
            RewardCollection.Add(new BODCollectionItem(0x9E2B, 1157264, 0, 400, CraftsmanTalisman, 10));
            RewardCollection.Add(new BODCollectionItem(0x2F5B, 1152674, CraftResources.GetHue(CraftResource.Gold), 450, SmeltersTalisman, (int)CraftResource.Gold));
            RewardCollection.Add(new BODCollectionItem(0x14EC, 1152665, CraftResources.GetHue(CraftResource.Gold), 500, HarvestMap, (int)CraftResource.Gold));
            RewardCollection.Add(new BODCollectionItem(0x9E2B, 1157218, 0, 550, CraftsmanTalisman, 25)); // todo: Get id
            RewardCollection.Add(new BODCollectionItem(0x2F5B, 1152675, CraftResources.GetHue(CraftResource.Agapite), 600, SmeltersTalisman, (int)CraftResource.Agapite));
            RewardCollection.Add(new BODCollectionItem(0x14EC, 1152666, CraftResources.GetHue(CraftResource.Agapite), 650, HarvestMap, (int)CraftResource.Agapite));
            RewardCollection.Add(new BODCollectionItem(0x1940, 1157221, 0, 700, CreateItem, 0)); // powder of fort keg
            RewardCollection.Add(new BODCollectionItem(0x9CE9, 1157290, 0, 750, CreateItem, 1)); // automaton actuator
            RewardCollection.Add(new BODCollectionItem(0x2F5B, 1152676, CraftResources.GetHue(CraftResource.Verite), 800, SmeltersTalisman, (int)CraftResource.Verite));
            RewardCollection.Add(new BODCollectionItem(0x14EC, 1152667, CraftResources.GetHue(CraftResource.Verite), 850, HarvestMap, (int)CraftResource.Verite));
            RewardCollection.Add(new BODCollectionItem(0x9E2B, 1157265, 0, 900, CraftsmanTalisman, 50));
            RewardCollection.Add(new BODCollectionItem(0x9E7E, 1157216, 0, 950, RockHammer));
            RewardCollection.Add(new BODCollectionItem(0x9CAA, 1157286, 1175, 1000, CreateItem, 2));
            RewardCollection.Add(new BODCollectionItem(0x2F5B, 1152677, CraftResources.GetHue(CraftResource.Valorite), 1050, SmeltersTalisman, (int)CraftResource.Valorite));
            RewardCollection.Add(new BODCollectionItem(0x14EC, 1152668, CraftResources.GetHue(CraftResource.Valorite), 1100, HarvestMap, (int)CraftResource.Valorite));
            RewardCollection.Add(new BODCollectionItem(0x9DB1, 1157220, 1175, 1200, CreateItem, 3));
        }

        #region Constructors
        // Do I need these since they aren't era-specific???

        private static Item TinkerTools(int type)
        {
            BaseTool tool = new TinkerTools
            {
                UsesRemaining = 250
            };

            return tool;
        }

        public static Item CreateItem(int type)
        {
            switch (type)
            {
                case 0: return new PowderOfFortKeg();
                case 1: return new AutomatonActuator();
                case 2: return new BlackrockMoonstone();
                case 3: return new BlackrockAutomatonHead();
            }

            return null;
        }

        private static Item CraftsmanTalisman(int type)
        {
            return new MasterCraftsmanTalisman(type, 0x9E2B, TalismanSkill.Tinkering);
        }
        #endregion

        public static readonly TinkeringRewardCalculator Instance = new TinkeringRewardCalculator();

        public override int ComputePoints(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type)
        {
            int points = 0;

            if (quantity == 10)
                points += 10;
            else if (quantity == 15)
                points += 25;
            else if (quantity == 20)
                points += 50;

            switch (itemCount)
            {
                case 3: points += 200; break;
                case 4: points += 300; break;
                case 5: points += 350; break;
                case 6: points += 400; break;
            }

            if (material >= BulkMaterialType.DullCopper && material <= BulkMaterialType.Valorite)
                points += 200 + (50 * (material - BulkMaterialType.DullCopper));

            if (exceptional)
                points += 200;

            return points;
        }

        private static readonly int[][][] m_GoldTable = new int[][][]
        {
            new int[][] // 1-part (regular)
            {
                new int[] { 150, 150, 225, 225, 300, 300, 350, 350, 400 },
                new int[] { 225, 225, 325, 325, 450, 450, 450, 500, 500 },
                new int[] { 300, 400, 500, 500, 600, 750, 750, 900, 900 }
            },
            new int[][] // 1-part (exceptional)
            {
                new int[] { 300, 300, 400, 500, 600, 600, 600, 700, 700 },
                new int[] { 450, 450, 650, 750, 900, 900, 900, 1000, 1000 },
                new int[] { 600, 750, 850, 1000, 1200, 1800, 1800, 1800, 2000 }
            },
            new int[][] // 3-part (regular)
            {
                new int[] { 2500, 2500, 2500, 3500, 3500, 3500, 4500, 4500, 4500 },
                new int[] { 4000, 4000, 4000, 5500, 5500, 5500, 7000, 7000, 7000 },
                new int[] { 6000, 7000, 7500, 8000, 8000, 9000, 9000, 10000, 10000 }
            },
            new int[][] // 3-part (exceptional)
            {
                new int[] { 4000, 4000, 5000, 5750, 6500, 6500, 6500, 7500, 8500 },
                new int[] { 6500, 6500, 7500, 8500, 10000, 10000, 10000, 12500, 12500 },
                new int[] { 8000, 10000, 10000, 12500, 12500, 15000, 15000, 20000, 20000 }
            },
            new int[][] // 4-part (regular)
            {
                new int[] { 4000, 4000, 4000, 5000, 5000, 5000, 6000, 6000, 6000 },
                new int[] { 6000, 6000, 6000, 7500, 7500, 7500, 9000, 9000, 9000 },
                new int[] { 8000, 9000, 9500, 10000, 10000, 15000, 17500, 17500, 17500 }
            },
            new int[][] // 4-part (exceptional)
            {
                new int[] { 5000, 5000, 6000, 6750, 7500, 7500, 8500, 8500, 9500 },
                new int[] { 7500, 7500, 8500, 9500, 11250, 11250, 11250, 15000, 15000 },
                new int[] { 10000, 1250, 1250, 15000, 15000, 20000, 20000, 25000, 25000 }
            },
            new int[][] // 4-part (regular)
            {
                new int[] { 4000, 4000, 4000, 5000, 5000, 5000, 7000, 7000, 7000 },
                new int[] { 6000, 6000, 6000, 7500, 7500, 7500, 9000, 9000, 9000 },
                new int[] { 8000, 9000, 9500, 10000, 10000, 15000, 15000, 20000, 20000 }
            },
            new int[][] // 4-part (exceptional)
            {
                new int[] { 5000, 5000, 6000, 6750, 7500, 7500, 9000, 9000, 15000 },
                new int[] { 7500, 7500, 8500, 9500, 11250, 11250, 15000, 15000, 15000 },
                new int[] { 10000, 1250, 1250, 15000, 15000, 20000, 20000, 25000, 25000 }
            },
            new int[][] // 5-part (regular)
            {
                new int[] { 5000, 5000, 60000, 6000, 7500, 7500, 9000, 9000, 10500 },
                new int[] { 7500, 7500, 7500, 11250, 11250, 11250, 15000, 15000, 15000 },
                new int[] { 10000, 10000, 1250, 15000, 15000, 20000, 20000, 25000, 25000 }
            },
            new int[][] // 5-part (exceptional)
            {
                new int[] { 7500, 7500, 8500, 9500, 10000, 10000, 12500, 12500, 15000 },
                new int[] { 11250, 11250, 1250, 13500, 15000, 15000, 20000, 20000, 25000 },
                new int[] { 15000, 1750, 1750, 20000, 20000, 30000, 30000, 40000, 50000 }
            },
        };

        public override int ComputeGold(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type)
        {
            int gold = 0;

            if (itemCount == 1 && BulkOrderSystem.NewSystemEnabled && BulkOrderSystem.ComputeGold(type, quantity, out gold))
            {
                return gold;
            }

            int[][][] goldTable = m_GoldTable;

            int typeIndex = ((itemCount == 6 ? 3 : itemCount == 5 ? 2 : itemCount == 4 ? 1 : 0) * 2) + (exceptional ? 1 : 0);
            int quanIndex = (quantity == 20 ? 2 : quantity == 15 ? 1 : 0);
            int mtrlIndex = (material >= BulkMaterialType.DullCopper && material <= BulkMaterialType.Valorite) ? 1 + (material - BulkMaterialType.DullCopper) : 0;

            gold = goldTable[typeIndex][quanIndex][mtrlIndex];

            int min = (gold * 9) / 10;
            int max = (gold * 10) / 9;

            return Utility.RandomMinMax(min, max);
        }
    }
    #endregion

    #region Carpentry Rewards
    public sealed class CarpentryRewardCalculator : RewardCalculator
    {
        public CarpentryRewardCalculator()
        {
            RewardCollection = new List<CollectionItem>();

            RewardCollection.Add(new BODCollectionItem(0x1028, 1157219, 0, 10, DovetailSaw));
            RewardCollection.Add(new BODCollectionItem(0x14F0, 1157191, 0, 25, RewardTitle, 10));
            RewardCollection.Add(new BODCollectionItem(0x14F0, 1157192, 0, 50, RewardTitle, 11));
            RewardCollection.Add(new BODCollectionItem(0x14F0, 1157193, 0, 250, RewardTitle, 12));
            RewardCollection.Add(new BODCollectionItem(0x9E2C, 1157264, 0, 300, CraftsmanTalisman, 10));
            RewardCollection.Add(new BODCollectionItem(0x2F5A, 1152678, CraftResources.GetHue(CraftResource.YewWood), 350, WoodsmansTalisman, (int)CraftResource.YewWood));
            RewardCollection.Add(new BODCollectionItem(0x9E2C, 1157218, 0, 450, CraftsmanTalisman, 25));
            RewardCollection.Add(new BODCollectionItem(0x12B3, 1157293, CraftResources.GetHue(CraftResource.DullCopper), 450, RunicMalletAndChisel, 1));
            RewardCollection.Add(new BODCollectionItem(0x12B3, 1157294, CraftResources.GetHue(CraftResource.ShadowIron), 450, RunicMalletAndChisel, 2));
            RewardCollection.Add(new BODCollectionItem(0x14EC, 1152669, CraftResources.GetHue(CraftResource.YewWood), 500, HarvestMap, (int)CraftResource.YewWood));
            RewardCollection.Add(new BODCollectionItem(0x1029, 1157223, CraftResources.GetHue(CraftResource.OakWood), 550, RunicDovetailSaw, 0));
            RewardCollection.Add(new BODCollectionItem(0x12B3, 1157295, CraftResources.GetHue(CraftResource.Copper), 600, RunicMalletAndChisel, 3));
            RewardCollection.Add(new BODCollectionItem(0x12B3, 1157296, CraftResources.GetHue(CraftResource.Bronze), 650, RunicMalletAndChisel, 4));
            RewardCollection.Add(new BODCollectionItem(0x2F5A, 1152679, CraftResources.GetHue(CraftResource.Heartwood), 650, WoodsmansTalisman, (int)CraftResource.Heartwood));
            RewardCollection.Add(new BODCollectionItem(0x14EC, 1152670, CraftResources.GetHue(CraftResource.Heartwood), 700, HarvestMap, (int)CraftResource.Heartwood));
            RewardCollection.Add(new BODCollectionItem(0x1029, 1157224, CraftResources.GetHue(CraftResource.AshWood), 750, RunicDovetailSaw, 1));
            RewardCollection.Add(new BODCollectionItem(0x9E2C, 1157265, 0, 800, CraftsmanTalisman, 50));
            RewardCollection.Add(new BODCollectionItem(0x2F5A, 1152680, CraftResources.GetHue(CraftResource.Bloodwood), 850, WoodsmansTalisman, (int)CraftResource.Bloodwood));
            RewardCollection.Add(new BODCollectionItem(0x14EC, 1152671, CraftResources.GetHue(CraftResource.Bloodwood), 900, HarvestMap, (int)CraftResource.Bloodwood));
            RewardCollection.Add(new BODCollectionItem(0x12B3, 1157297, CraftResources.GetHue(CraftResource.Gold), 900, RunicMalletAndChisel, 5));
            RewardCollection.Add(new BODCollectionItem(0x1029, 1157225, CraftResources.GetHue(CraftResource.YewWood), 950, RunicDovetailSaw, 2));
            RewardCollection.Add(new BODCollectionItem(0x2F5A, 1152681, CraftResources.GetHue(CraftResource.Frostwood), 1000, WoodsmansTalisman, (int)CraftResource.Frostwood));
            RewardCollection.Add(new BODCollectionItem(0x12B3, 1157298, CraftResources.GetHue(CraftResource.Agapite), 1000, RunicMalletAndChisel, 6));
            RewardCollection.Add(new BODCollectionItem(0x14EC, 1152672, CraftResources.GetHue(CraftResource.Frostwood), 1050, HarvestMap, (int)CraftResource.Frostwood));
            RewardCollection.Add(new BODCollectionItem(0x12B3, 1157299, CraftResources.GetHue(CraftResource.Verite), 1100, RunicMalletAndChisel, 7));
            RewardCollection.Add(new BODCollectionItem(0x1029, 1157226, CraftResources.GetHue(CraftResource.Heartwood), 1150, RunicDovetailSaw, 3));
            RewardCollection.Add(new BODCollectionItem(0x12B3, 1157300, CraftResources.GetHue(CraftResource.Valorite), 1150, RunicMalletAndChisel, 8));
        }

        #region Constructors

        private static Item DovetailSaw(int type)
        {
            BaseTool tool = new DovetailSaw
            {
                UsesRemaining = 250
            };

            return tool;
        }

        private static Item RunicMalletAndChisel(int type)
        {
            if (type >= 1 && type <= 8)
                return new RunicMalletAndChisel(CraftResource.Iron + type, 55 - (type * 5));

            return null;
        }

        private static Item RunicDovetailSaw(int type)
        {
            switch (type)
            {
                default:
                case 0: return new RunicDovetailSaw(CraftResource.OakWood, 45);
                case 1: return new RunicDovetailSaw(CraftResource.AshWood, 35);
                case 2: return new RunicDovetailSaw(CraftResource.YewWood, 25);
                case 3: return new RunicDovetailSaw(CraftResource.Heartwood, 15);
            }
        }

        private static Item CraftsmanTalisman(int type)
        {
            return new MasterCraftsmanTalisman(type, 0x9E2C, TalismanSkill.Carpentry);
        }
        #endregion

        public static readonly CarpentryRewardCalculator Instance = new CarpentryRewardCalculator();

        public override int ComputePoints(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type)
        {
            int points = 0;

            if (quantity == 10)
                points += 10;
            else if (quantity == 15)
                points += 25;
            else if (quantity == 20)
                points += 50;

            if (exceptional)
                points += 200;

            switch (material)
            {
                case BulkMaterialType.None: break;
                case BulkMaterialType.OakWood: points += 300; break;
                case BulkMaterialType.AshWood: points += 350; break;
                case BulkMaterialType.YewWood: points += 400; break;
                case BulkMaterialType.Heartwood: points += 450; break;
                case BulkMaterialType.Bloodwood: points += 500; break;
                case BulkMaterialType.Frostwood: points += 550; break;
            }

            if (itemCount > 1)
                points += LookupTypePoints(m_Types, type);

            return points;
        }

        private readonly RewardType[] m_Types =
        {
            new RewardType(250, typeof(TallCabinet), typeof(ShortCabinet)),
            new RewardType(250, typeof(RedArmoire), typeof(ElegantArmoire), typeof(MapleArmoire), typeof(CherryArmoire)),
            new RewardType(300, typeof(PlainWoodenChest), typeof(OrnateWoodenChest), typeof(GildedWoodenChest), typeof(WoodenFootLocker), typeof(FinishedWoodenChest)),
            new RewardType(350, typeof(WildStaff), typeof(ArcanistsWildStaff), typeof(AncientWildStaff), typeof(ThornedWildStaff), typeof(HardenedWildStaff)),
            new RewardType(250, typeof(LapHarp), typeof(Lute), typeof(Drums), typeof(Harp)),
            new RewardType(200, typeof(GnarledStaff), typeof(QuarterStaff), typeof(ShepherdsCrook), typeof(Tetsubo), typeof(Bokuto)),
            new RewardType(300, typeof(WoodenBox), typeof(EmptyBookcase), typeof(WoodenBench), typeof(WoodenThrone)),
        };

        private static readonly int[][][] m_GoldTable = new int[][][]
        {
            new int[][] // 1-part (regular)
            {
                new int[] { 150, 150, 225, 225, 300, 300 },
                new int[] { 225, 225, 325, 325, 450, 450 },
                new int[] { 300, 400, 500, 500, 600, 750 }
            },
            new int[][] // 1-part (exceptional)
            {
                new int[] { 300, 300, 400, 500, 600, 600 },
                new int[] { 450, 450, 650, 750, 900, 900 },
                new int[] { 600, 750, 850, 1000, 1200, 1800 }
            },
            new int[][] // 2-part (regular)
            {
                new int[] { 2000, 2000, 2000, 2500, 2500, 2500 },
                new int[] { 3000, 3000, 3000, 3750, 3750, 3750 },
                new int[] { 4000, 4500, 4500, 5000, 5000, 7500 }
            },
            new int[][] // 2-part (exceptional)
            {
                new int[] { 2500, 2500, 3000, 3350, 3350, 4000 },
                new int[] { 3750, 3750, 4250, 4750, 5200, 5200 },
                new int[] { 5000, 6100, 6100, 7000, 7000, 10000 }
            },
            new int[][] // 4-part (regular)
            {
                new int[] { 4000, 4000, 4000, 5000, 5000, 5000 },
                new int[] { 6000, 6000, 6000, 7500, 7500, 7500 },
                new int[] { 8000, 9000, 9500, 10000, 10000, 15000 }
            },
            new int[][] // 4-part (exceptional)
            {
                new int[] { 5000, 5000, 6000, 6750, 7500, 7500 },
                new int[] { 7500, 7500, 8500, 9500, 11250, 11250 },
                new int[] { 10000, 1250, 1250, 15000, 15000, 20000 }
            },
            new int[][] // 5-part (regular)
            {
                new int[] { 5000, 5000, 60000, 6000, 7500, 7500 },
                new int[] { 7500, 7500, 7500, 11250, 11250, 11250 },
                new int[] { 10000, 10000, 1250, 15000, 15000, 20000 }
            },
            new int[][] // 5-part (exceptional)
            {
                new int[] { 7500, 7500, 8500, 9500, 10000, 10000 },
                new int[] { 11250, 11250, 1250, 1350, 15000, 15000 },
                new int[] { 15000, 1750, 1750, 20000, 20000, 30000 }
            },
        };

        public override int ComputeGold(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type)
        {
            int gold = 0;

            if (itemCount == 1 && BulkOrderSystem.NewSystemEnabled && BulkOrderSystem.ComputeGold(type, quantity, out gold))
            {
                return gold;
            }

            int[][][] goldTable = m_GoldTable;

            int typeIndex = ((itemCount == 5 ? 2 : itemCount == 4 ? 1 : 0) * 2) + (exceptional ? 1 : 0);
            int quanIndex = (quantity == 20 ? 2 : quantity == 15 ? 1 : 0);
            int mtrlIndex = (material == BulkMaterialType.Frostwood ? 5 : material == BulkMaterialType.Bloodwood ? 4 : material == BulkMaterialType.Heartwood ? 3 : material == BulkMaterialType.YewWood ? 2 : material == BulkMaterialType.AshWood ? 1 : 0);

            gold = goldTable[typeIndex][quanIndex][mtrlIndex];

            int min = (gold * 9) / 10;
            int max = (gold * 10) / 9;

            return Utility.RandomMinMax(min, max);
        }
    }
    #endregion

    #region Inscription Rewards
    public sealed class InscriptionRewardCalculator : RewardCalculator
    {
        public InscriptionRewardCalculator()
        {
            RewardCollection = new List<CollectionItem>();

            RewardCollection.Add(new BODCollectionItem(0x0FBF, 1157219, 0, 10, ScribesPen));
            RewardCollection.Add(new BODCollectionItem(0x14F0, 1157194, 0, 25, RewardTitle, 13));
            RewardCollection.Add(new BODCollectionItem(0x14F0, 1157195, 0, 50, RewardTitle, 14));
            RewardCollection.Add(new BODCollectionItem(0x14F0, 1157196, 0, 210, RewardTitle, 15));
            RewardCollection.Add(new BODCollectionItem(0x2831, 1156443, 0, 210, Recipe, 3));
            RewardCollection.Add(new BODCollectionItem(0x182B, 1157205, 2741, 250, NaturalDye, 3));
            RewardCollection.Add(new BODCollectionItem(0x9E28, 1157264, 0, 275, CraftsmanTalisman, 10));
            RewardCollection.Add(new BODCollectionItem(0x182B, 1157205, 2740, 310, NaturalDye, 4));
            RewardCollection.Add(new BODCollectionItem(0x9E28, 1157218, 0, 350, CraftsmanTalisman, 25));
            RewardCollection.Add(new BODCollectionItem(0x182B, 1157205, 2732, 375, NaturalDye, 5));
            RewardCollection.Add(new BODCollectionItem(0x9E28, 1157265, 0, 410, CraftsmanTalisman, 50));
            RewardCollection.Add(new BODCollectionItem(0x182B, 1157205, 2731, 450, NaturalDye, 6));
            RewardCollection.Add(new BODCollectionItem(0x182B, 1157205, 2735, 475, NaturalDye, 7));
            RewardCollection.Add(new BODCollectionItem(0x9E28, 1157291, 0, 500, ImprovementTalisman, 10));
        }

        #region Constructors

        private static Item ScribesPen(int type)
        {
            BaseTool tool = new ScribesPen
            {
                UsesRemaining = 250
            };

            return tool;
        }

        private static Item CraftsmanTalisman(int type)
        {
            return new MasterCraftsmanTalisman(type, 0x9E28, TalismanSkill.Inscription);
        }

        private static Item ImprovementTalisman(int type)
        {
            return new GuaranteedSpellbookImprovementTalisman(type);
        }

        #endregion

        public static readonly InscriptionRewardCalculator Instance = new InscriptionRewardCalculator();

        public override int ComputePoints(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type)
        {
            int points = 0;

            if (quantity == 10)
                points += 10;
            else if (quantity == 15)
                points += 25;
            else if (quantity == 20)
                points += 50;

            if (itemCount > 1)
                points += LookupTypePoints(m_Types, type);

            return points;
        }

        private readonly RewardType[] m_Types =
        {
            new RewardType(200, typeof(ClumsyScroll), typeof(FeeblemindScroll), typeof(WeakenScroll)),
            new RewardType(300, typeof(CurseScroll), typeof(GreaterHealScroll), typeof(RecallScroll)),
            new RewardType(300, typeof(PoisonStrikeScroll), typeof(WitherScroll), typeof(StrangleScroll)),
            new RewardType(250, typeof(MindRotScroll), typeof(SummonFamiliarScroll), typeof(AnimateDeadScroll), typeof(HorrificBeastScroll)),
            new RewardType(200, typeof(HealScroll), typeof(AgilityScroll), typeof(CunningScroll), typeof(CureScroll), typeof(StrengthScroll)),
            new RewardType(250, typeof(BloodOathScroll), typeof(CorpseSkinScroll), typeof(CurseWeaponScroll), typeof(EvilOmenScroll), typeof(PainSpikeScroll)),
            new RewardType(300, typeof(BladeSpiritsScroll), typeof(DispelFieldScroll), typeof(MagicReflectScroll), typeof(ParalyzeScroll), typeof(SummonCreatureScroll)),
            new RewardType(350, typeof(ChainLightningScroll), typeof(FlamestrikeScroll), typeof(ManaVampireScroll), typeof(MeteorSwarmScroll), typeof(PolymorphScroll)),
            new RewardType(400, typeof(SummonAirElementalScroll), typeof(SummonDaemonScroll), typeof(SummonEarthElementalScroll), typeof(SummonFireElementalScroll), typeof(SummonWaterElementalScroll)),
            new RewardType(450, typeof(Spellbook), typeof(NecromancerSpellbook), typeof(Runebook), typeof(RunicAtlas))
        };

        private static readonly int[][] m_GoldTable = new int[][]
        {
            new int[] // singles
            {
                150, 225, 300
            },
            new int[] // no 2 piece
            {
            },
            new int[] // 3-part
            {
                4000, 6000, 8000
            },
            new int[] // 4-part
            {
                5000, 7500, 10000
            },
            new int[] // 5-part
            {
                7500, 11250, 15000
            },
        };

        public override int ComputeGold(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type)
        {
            int gold = 0;

            if (itemCount == 1 && BulkOrderSystem.NewSystemEnabled && BulkOrderSystem.ComputeGold(type, quantity, out gold))
            {
                return gold;
            }

            int[][] goldTable = m_GoldTable;

            int quanIndex = (quantity == 20 ? 2 : quantity == 15 ? 1 : 0);

            gold = goldTable[itemCount - 1][quanIndex];

            int min = (gold * 9) / 10;
            int max = (gold * 10) / 9;

            return Utility.RandomMinMax(min, max);
        }
    }
    #endregion

    #region Cooking Rewards
    public sealed class CookingRewardCalculator : RewardCalculator
    {
        public CookingRewardCalculator()
        {
            RewardCollection = new List<CollectionItem>();

            RewardCollection.Add(new BODCollectionItem(0x97F, 1157219, 0, 10, Skillet));
            RewardCollection.Add(new BODCollectionItem(0x14F0, 1157197, 0, 25, RewardTitle, 13));
            RewardCollection.Add(new BODCollectionItem(0x2831, 1031233, 0, 25, Recipe, 4));
            RewardCollection.Add(new BODCollectionItem(0x14F0, 1157198, 0, 50, RewardTitle, 14));
            RewardCollection.Add(new BODCollectionItem(0x14F0, 1157199, 0, 210, RewardTitle, 15));
            RewardCollection.Add(new BODCollectionItem(0x9E27, 1157264, 0, 250, CraftsmanTalisman, 10));
            RewardCollection.Add(new BODCollectionItem(0x9E27, 1157218, 0, 300, CraftsmanTalisman, 25));
            RewardCollection.Add(new BODCollectionItem(0x9E27, 1157265, 0, 350, CraftsmanTalisman, 50));
            RewardCollection.Add(new BODCollectionItem(0x153D, 1157227, 1990, 410, CreateItem, 0));
            RewardCollection.Add(new BODCollectionItem(0x14F0, 1076605, 0, 475, CreateItem, 1));
            RewardCollection.Add(new BODCollectionItem(0x182B, 1157278, 2740, 525, NaturalDye, 8));
            RewardCollection.Add(new BODCollectionItem(0x182B, 1157278, 2732, 625, NaturalDye, 9));
            RewardCollection.Add(new BODCollectionItem(0x9E36, 1157229, 0, 625, CreateItem, 2));
        }

        #region Constructors

        private static Item Skillet(int type)
        {
            BaseTool tool = new Skillet
            {
                UsesRemaining = 250
            };

            return tool;
        }

        private static Item CraftsmanTalisman(int type)
        {
            return new MasterCraftsmanTalisman(type, 0x9E27, TalismanSkill.Cooking);
        }

        private static Item CreateItem(int type)
        {
            switch (type)
            {
                case 0: return new MasterChefsApron();
                case 1: return new PlumTreeAddonDeed();
                case 2: return new FermentationBarrel();
            }

            return null;
        }

        #endregion

        public static readonly CookingRewardCalculator Instance = new CookingRewardCalculator();

        public override int ComputePoints(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type)
        {
            int points = 0;

            if (quantity == 10)
                points += 10;
            else if (quantity == 15)
                points += 25;
            else if (quantity == 20)
                points += 50;

            if (exceptional)
                points += 200;

            if (itemCount > 1)
                points += LookupTypePoints(m_Types, type);

            return points;
        }

        private readonly RewardType[] m_Types =
        {
            new RewardType(200, typeof(SweetCocoaButter), typeof(SackFlour), typeof(Dough)),
            new RewardType(250, typeof(UnbakedFruitPie), typeof(UnbakedPeachCobbler), typeof(UnbakedApplePie), typeof(UnbakedPumpkinPie)),
            new RewardType(300, typeof(CookedBird), typeof(FishSteak), typeof(FriedEggs), typeof(LambLeg), typeof(Ribs)),
            new RewardType(350, typeof(Cookies), typeof(Cake), typeof(Muffins), typeof(ThreeTieredCake)),
            new RewardType(400, typeof(EnchantedApple), typeof(TribalPaint), typeof(GrapesOfWrath), typeof(EggBomb)),
            new RewardType(450, typeof(MisoSoup), typeof(WhiteMisoSoup), typeof(RedMisoSoup), typeof(AwaseMisoSoup)),
            new RewardType(500, typeof(WasabiClumps), typeof(SushiRolls), typeof(SushiPlatter), typeof(GreenTea)),
        };

        private static readonly int[][] m_GoldTable = new int[][]
        {
            new int[] // singles
            {
                150, 225, 300
            },
            new int[] // no 2 piece
            {
            },
            new int[] // 3-part
            {
                4000, 6000, 8000
            },
            new int[] // 4-part
            {
                5000, 7500, 10000
            },
            new int[] // 5-part
            {
                7500, 11250, 15000
            },
            new int[] // 6-part (regular)
            {
                7500, 11250, 15000
            },
        };

        public override int ComputeGold(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type)
        {
            int gold = 0;

            if (itemCount == 1 && BulkOrderSystem.NewSystemEnabled && BulkOrderSystem.ComputeGold(type, quantity, out gold))
            {
                return gold;
            }

            int[][] goldTable = m_GoldTable;

            int quanIndex = (quantity == 20 ? 2 : quantity == 15 ? 1 : 0);

            gold = goldTable[itemCount - 1][quanIndex];

            int min = (gold * 9) / 10;
            int max = (gold * 10) / 9;

            return Utility.RandomMinMax(min, max);
        }
    }
    #endregion

    #region Fletching Rewards
    public sealed class FletchingRewardCalculator : RewardCalculator
    {
        public FletchingRewardCalculator()
        {
            RewardCollection = new List<CollectionItem>();

            RewardCollection.Add(new BODCollectionItem(0x1022, 1157219, 0, 10, FletcherTools));
            RewardCollection.Add(new BODCollectionItem(0x14F0, 1157200, 0, 25, RewardTitle, 17));
            RewardCollection.Add(new BODCollectionItem(0x9E29, 1157264, 0, 210, CraftsmanTalisman, 10));
            RewardCollection.Add(new BODCollectionItem(0x2F5A, 1152678, CraftResources.GetHue(CraftResource.YewWood), 225, WoodsmansTalisman, (int)CraftResource.YewWood));
            RewardCollection.Add(new BODCollectionItem(0x14EC, 1152669, CraftResources.GetHue(CraftResource.YewWood), 310, HarvestMap, (int)CraftResource.YewWood));
            RewardCollection.Add(new BODCollectionItem(0x9E29, 1157218, 0, 325, CraftsmanTalisman, 25));
            RewardCollection.Add(new BODCollectionItem(0x2F5A, 1152679, CraftResources.GetHue(CraftResource.Heartwood), 360, WoodsmansTalisman, (int)CraftResource.Heartwood));
            RewardCollection.Add(new BODCollectionItem(0x14EC, 1152670, CraftResources.GetHue(CraftResource.Heartwood), 375, HarvestMap, (int)CraftResource.Heartwood));
            RewardCollection.Add(new BODCollectionItem(0x9E29, 1157265, 0, 410, CraftsmanTalisman, 50));
            RewardCollection.Add(new BODCollectionItem(0x1022, 1157223, CraftResources.GetHue(CraftResource.OakWood), 425, CreateRunicFletcherTools, 0));
            RewardCollection.Add(new BODCollectionItem(0x2F5A, 1152680, CraftResources.GetHue(CraftResource.Bloodwood), 510, WoodsmansTalisman, (int)CraftResource.Bloodwood));
            RewardCollection.Add(new BODCollectionItem(0x14EC, 1152671, CraftResources.GetHue(CraftResource.Bloodwood), 525, HarvestMap, (int)CraftResource.Bloodwood));
            RewardCollection.Add(new BODCollectionItem(0x1022, 1157224, CraftResources.GetHue(CraftResource.AshWood), 650, CreateRunicFletcherTools, 1));
            RewardCollection.Add(new BODCollectionItem(0x2F5A, 1152681, CraftResources.GetHue(CraftResource.Frostwood), 750, WoodsmansTalisman, (int)CraftResource.Frostwood));
            RewardCollection.Add(new BODCollectionItem(0x14EC, 1152672, CraftResources.GetHue(CraftResource.Frostwood), 950, HarvestMap, (int)CraftResource.Frostwood));
            RewardCollection.Add(new BODCollectionItem(0x1022, 1157225, CraftResources.GetHue(CraftResource.YewWood), 1000, CreateRunicFletcherTools, 2));
            RewardCollection.Add(new BODCollectionItem(0x1022, 1157226, CraftResources.GetHue(CraftResource.Heartwood), 1100, CreateRunicFletcherTools, 3));
        }

        #region Constructors

        private static Item FletcherTools(int type)
        {
            BaseTool tool = new FletcherTools
            {
                UsesRemaining = 250
            };

            return tool;
        }

        private static Item CraftsmanTalisman(int type)
        {
            return new MasterCraftsmanTalisman(type, 0x9E29, TalismanSkill.Fletching);
        }

        private static Item CreateRunicFletcherTools(int type)
        {
            switch (type)
            {
                default:
                case 0: return new RunicFletcherTool(CraftResource.OakWood, 45);
                case 1: return new RunicFletcherTool(CraftResource.AshWood, 35);
                case 2: return new RunicFletcherTool(CraftResource.YewWood, 25);
                case 3: return new RunicFletcherTool(CraftResource.Heartwood, 15);
            }
        }

        #endregion

        public static readonly FletchingRewardCalculator Instance = new FletchingRewardCalculator();

        public override int ComputePoints(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type)
        {
            int points = 0;

            if (quantity == 10)
                points += 10;
            else if (quantity == 15)
                points += 25;
            else if (quantity == 20)
                points += 50;

            if (exceptional)
                points += 200;

            switch (material)
            {
                case BulkMaterialType.None: break;
                case BulkMaterialType.OakWood: points += 300; break;
                case BulkMaterialType.AshWood: points += 350; break;
                case BulkMaterialType.YewWood: points += 400; break;
                case BulkMaterialType.Heartwood: points += 450; break;
                case BulkMaterialType.Bloodwood: points += 500; break;
                case BulkMaterialType.Frostwood: points += 550; break;
            }

            if (itemCount > 1)
                points += LookupTypePoints(m_Types, type);

            return points;
        }

        private readonly RewardType[] m_Types =
        {
            new RewardType(200, typeof(Arrow), typeof(Bolt)),
            new RewardType(300, typeof(Bow), typeof(CompositeBow), typeof(Yumi)),
            new RewardType(300, typeof(Crossbow), typeof(HeavyCrossbow), typeof(RepeatingCrossbow)),
            new RewardType(350, typeof(MagicalShortbow), typeof(RangersShortbow), typeof(LightweightShortbow), typeof(MysticalShortbow), typeof(AssassinsShortbow)),
            new RewardType(250, typeof(ElvenCompositeLongbow), typeof(BarbedLongbow), typeof(SlayerLongbow), typeof(FrozenLongbow), typeof(LongbowOfMight)),
        };

        private static readonly int[][][] m_GoldTable = new int[][][]
        {
            new int[][] // 1-part (regular)
            {
                new int[] { 150, 150, 225, 225, 300, 300 },
                new int[] { 225, 225, 325, 325, 450, 450 },
                new int[] { 300, 400, 500, 500, 600, 750 }
            },
            new int[][] // 1-part (exceptional)
            {
                new int[] { 300, 300, 400, 500, 600, 600 },
                new int[] { 450, 450, 650, 750, 900, 900 },
                new int[] { 600, 750, 850, 1000, 1200, 1800 }
            },
            new int[][] // 4-part (regular)
            {
                new int[] { 4000, 4000, 4000, 5000, 5000, 5000 },
                new int[] { 6000, 6000, 6000, 7500, 7500, 7500 },
                new int[] { 8000, 9000, 9500, 10000, 10000, 15000 }
            },
            new int[][] // 4-part (exceptional)
            {
                new int[] { 5000, 5000, 6000, 6750, 7500, 7500 },
                new int[] { 7500, 7500, 8500, 9500, 11250, 11250 },
                new int[] { 10000, 1250, 1250, 15000, 15000, 20000 }
            },
            new int[][] // 5-part (regular)
            {
                new int[] { 5000, 5000, 60000, 6000, 7500, 7500 },
                new int[] { 7500, 7500, 7500, 11250, 11250, 11250 },
                new int[] { 10000, 10000, 1250, 15000, 15000, 20000 }
            },
            new int[][] // 5-part (exceptional)
            {
                new int[] { 7500, 7500, 8500, 9500, 10000, 10000 },
                new int[] { 11250, 11250, 1250, 1350, 15000, 15000 },
                new int[] { 15000, 1750, 1750, 20000, 20000, 30000 }
            },
        };

        public override int ComputeGold(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type)
        {
            int gold = 0;

            if (itemCount == 1 && BulkOrderSystem.NewSystemEnabled && BulkOrderSystem.ComputeGold(type, quantity, out gold))
            {
                return gold;
            }

            int[][][] goldTable = m_GoldTable;

            int typeIndex = ((itemCount == 5 ? 2 : itemCount == 4 ? 1 : 0) * 2) + (exceptional ? 1 : 0);
            int quanIndex = (quantity == 20 ? 2 : quantity == 15 ? 1 : 0);
            int mtrlIndex = (material == BulkMaterialType.Frostwood ? 5 : material == BulkMaterialType.Bloodwood ? 4 : material == BulkMaterialType.Heartwood ? 3 : material == BulkMaterialType.YewWood ? 2 : material == BulkMaterialType.AshWood ? 1 : 0);

            gold = goldTable[typeIndex][quanIndex][mtrlIndex];

            int min = (gold * 9) / 10;
            int max = (gold * 10) / 9;

            return Utility.RandomMinMax(min, max);
        }
    }
    #endregion

    #region Alchemy Rewards
    public sealed class AlchemyRewardCalculator : RewardCalculator
    {
        public AlchemyRewardCalculator()
        {
            RewardCollection = new List<CollectionItem>();

            RewardCollection.Add(new BODCollectionItem(0xE9B, 1157219, 0, 10, MortarAndPestle));
            RewardCollection.Add(new BODCollectionItem(0x14F0, 1157183, 0, 25, RewardTitle, 20));
            RewardCollection.Add(new BODCollectionItem(0x14F0, 1157202, 0, 50, RewardTitle, 21));
            RewardCollection.Add(new BODCollectionItem(0x14F0, 1157203, 0, 210, RewardTitle, 22));
            RewardCollection.Add(new BODCollectionItem(0x182B, 1157278, 2741, 225, NaturalDye, 0));
            RewardCollection.Add(new BODCollectionItem(0x975, 1152660, CraftResources.GetHue(CraftResource.AshWood), 250, Cauldron, 0));
            RewardCollection.Add(new BODCollectionItem(0x975, 1152656, CraftResources.GetHue(CraftResource.Bronze), 260, Cauldron, 1));
            RewardCollection.Add(new BODCollectionItem(0x9E26, 1157264, 0, 275, CraftsmanTalisman, 10)); // todo: Get id
            RewardCollection.Add(new BODCollectionItem(0x975, 1152661, CraftResources.GetHue(CraftResource.YewWood), 300, Cauldron, 2));
            RewardCollection.Add(new BODCollectionItem(0x975, 1152657, CraftResources.GetHue(CraftResource.Gold), 310, Cauldron, 3));
            RewardCollection.Add(new BODCollectionItem(0x9E26, 1157218, 0, 325, CraftsmanTalisman, 25)); // todo: Get id
            RewardCollection.Add(new BODCollectionItem(0x975, 1152658, CraftResources.GetHue(CraftResource.Agapite), 350, Cauldron, 4));
            RewardCollection.Add(new BODCollectionItem(0x975, 1152662, CraftResources.GetHue(CraftResource.Heartwood), 360, Cauldron, 5));
            RewardCollection.Add(new BODCollectionItem(0x9E26, 1157265, 0, 375, CraftsmanTalisman, 50)); // todo: Get id
            RewardCollection.Add(new BODCollectionItem(0x182B, 1157278, 2731, 400, NaturalDye, 1));
            RewardCollection.Add(new BODCollectionItem(0x975, 1152663, CraftResources.GetHue(CraftResource.Bloodwood), 410, Cauldron, 6));
            RewardCollection.Add(new BODCollectionItem(0x182B, 1157278, 2735, 425, NaturalDye, 2));
            RewardCollection.Add(new BODCollectionItem(0x975, 1152659, CraftResources.GetHue(CraftResource.Verite), 450, Cauldron, 7));
        }

        #region Constructors
        private static Item MortarAndPestle(int type)
        {
            BaseTool tool = new MortarPestle
            {
                UsesRemaining = 250
            };

            return tool;
        }

        private static Item Cauldron(int type)
        {
            switch (type)
            {
                default:
                case 0: return new CauldronOfTransmutationDeed(CraftResource.AshWood);
                case 1: return new CauldronOfTransmutationDeed(CraftResource.Bronze);
                case 2: return new CauldronOfTransmutationDeed(CraftResource.YewWood);
                case 3: return new CauldronOfTransmutationDeed(CraftResource.Gold);
                case 4: return new CauldronOfTransmutationDeed(CraftResource.Agapite);
                case 5: return new CauldronOfTransmutationDeed(CraftResource.Heartwood);
                case 6: return new CauldronOfTransmutationDeed(CraftResource.Bloodwood);
                case 7: return new CauldronOfTransmutationDeed(CraftResource.Verite);
            }
        }

        private static Item CraftsmanTalisman(int type)
        {
            return new MasterCraftsmanTalisman(type, 0x9E26, TalismanSkill.Alchemy);
        }
        #endregion

        public static readonly AlchemyRewardCalculator Instance = new AlchemyRewardCalculator();

        public override int ComputePoints(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type)
        {
            int points = 0;

            if (quantity == 10)
                points += 10;
            else if (quantity == 15)
                points += 25;
            else if (quantity == 20)
                points += 50;

            if (itemCount == 3)
            {
                if (type == typeof(RefreshPotion) || type == typeof(HealPotion) || type == typeof(CurePotion))
                    points += 250;
                else
                    points += 300;
            }
            else if (itemCount == 4)
                points += 200;
            else if (itemCount == 5)
                points += 400;
            else if (itemCount == 6)
                points += 350;

            return points;
        }

        private static readonly int[][] m_GoldTable = new int[][]
        {
            new int[] // singles
            {
                150, 225, 300
            },
            new int[] // no 2 piece
            {
            },
            new int[] // 3-part
            {
                4000, 6000, 8000
            },
            new int[] // 4-part
            {
                5000, 7500, 10000
            },
            new int[] // 5-part
            {
                7500, 11250, 15000
            },
            new int[] // 6-part (regular)
            {
                7500, 11250, 15000
            },
        };

        public override int ComputeGold(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type)
        {
            int gold = 0;

            if (itemCount == 1 && BulkOrderSystem.NewSystemEnabled && BulkOrderSystem.ComputeGold(type, quantity, out gold))
            {
                return gold;
            }

            int[][] goldTable = m_GoldTable;

            int quanIndex = (quantity == 20 ? 2 : quantity == 15 ? 1 : 0);

            gold = goldTable[itemCount - 1][quanIndex];

            int min = (gold * 9) / 10;
            int max = (gold * 10) / 9;

            return Utility.RandomMinMax(min, max);
        }
    }
    #endregion


}
