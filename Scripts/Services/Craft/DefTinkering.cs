using System;
using Server.Factions;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Engines.Craft
{
    public enum TinkerRecipes
    {
        InvisibilityPotion = 400,
        DarkglowPotion = 401,
        ParasiticPotion = 402,

        EssenceOfBattle = 450,
        PendantOfTheMagi = 451,
        ResilientBracer = 452,
        ScrappersCompendium = 453,
        HoveringWisp = 454,

        KotlPowerCore = 455,

        // doom
        BraceletOfPrimalConsumption = 456,
        DrSpectorLenses = 457,
        KotlAutomatonHead = 458
    }

    public class DefTinkering : CraftSystem
    {
        #region Mondain's Legacy
        public override CraftECA ECA
        {
            get
            {
                return CraftECA.ChanceMinusSixtyToFourtyFive;
            }
        }
        #endregion

        public override SkillName MainSkill
        {
            get
            {
                return SkillName.Tinkering;
            }
        }

        public override int GumpTitleNumber
        {
            get
            {
                return 1044007;
            }// <CENTER>TINKERING MENU</CENTER>
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefTinkering();

                return m_CraftSystem;
            }
        }

        private DefTinkering()
            : base(1, 1, 1.25)// base( 1, 1, 3.0 )
        {
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            if (item.NameNumber == 1044258 || item.NameNumber == 1046445) // potion keg and faction trap removal kit
                return 0.5; // 50%

            return 0.0; // 0%
        }

        public override int CanCraft(Mobile from, BaseTool tool, Type itemType)
        {
            int num = 0;

            if (tool == null || tool.Deleted || tool.UsesRemaining <= 0)
                return 1044038; // You have worn out your tool!
            else if (!tool.CheckAccessible(from, ref num))
                return num; // The tool must be on your person to use.
            else if (itemType != null && (itemType.IsSubclassOf(typeof(BaseFactionTrapDeed)) || itemType == typeof(FactionTrapRemovalKit)) && Faction.Find(from) == null)
                return 1044573; // You have to be in a faction to do that.
            else if (itemType == typeof(ModifiedClockworkAssembly) && !(from is PlayerMobile && ((PlayerMobile)from).MechanicalLife))
                return 1113034; // You haven't read the Mechanical Life Manual. Talking to Sutek might help!

            return 0;
        }

        private static readonly Type[] m_TinkerColorables = new Type[]
        {
            typeof(ForkLeft), typeof(ForkRight),
            typeof(SpoonLeft), typeof(SpoonRight),
            typeof(KnifeLeft), typeof(KnifeRight),
            typeof(Plate),
            typeof(Goblet), typeof(PewterMug),
            typeof(KeyRing),
            typeof(Candelabra), typeof(Scales),
            typeof(Key), typeof(Globe),
            typeof(Spyglass), typeof(Lantern),
            typeof(HeatingStand),
            typeof(GorgonLense), typeof(MedusaLightScales), typeof(MedusaDarkScales), typeof(RedScales), 
            typeof(BlueScales), typeof(BlackScales), typeof(GreenScales), typeof(YellowScales), typeof(WhiteScales),
            typeof(PlantPigment), typeof(SoftenedReeds), typeof(DryReeds), typeof(PlantClippings),

            typeof(KotlAutomatonHead)
        };

        public override bool RetainsColorFrom(CraftItem item, Type type)
        {
            if (type == typeof(CrystalDust))
                return false;

            bool contains = false;
            type = item.ItemType;

            for (int i = 0; !contains && i < m_TinkerColorables.Length; ++i)
                contains = (m_TinkerColorables[i] == type);

            if (!contains && !type.IsSubclassOf(typeof(BaseIngot)))
                return false;

            return contains;
        }

        public override void PlayCraftEffect(Mobile from)
        {
            from.PlaySound( 0x23B );
        }

        public override int PlayEndingEffect(Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item)
        {
            if (toolBroken)
                from.SendLocalizedMessage(1044038); // You have worn out your tool

            if (failed)
            {
                if (lostMaterial)
                    return 1044043; // You failed to create the item, and some of your materials are lost.
                else
                    return 1044157; // You failed to create the item, but no materials were lost.
            }
            else
            {
                if (quality == 0)
                    return 502785; // You were barely able to make this item.  It's quality is below average.
                else if (makersMark && quality == 2)
                    return 1044156; // You create an exceptional quality item and affix your maker's mark.
                else if (quality == 2)
                    return 1044155; // You create an exceptional quality item.
                else 
                    return 1044154; // You create the item.
            }
        }

        public override bool ConsumeOnFailure(Mobile from, Type resourceType, CraftItem craftItem)
        {
            if (resourceType == typeof(Silver) || resourceType == typeof(RingOfTheElements) || resourceType == typeof(HatOfTheMagi) || resourceType == typeof(AutomatonActuator))
                return false;

            return base.ConsumeOnFailure(from, resourceType, craftItem);
        }

        public void AddJewelrySet(GemType gemType, Type itemType)
        {
            int offset = (int)gemType - 1;

            int index = this.AddCraft(typeof(GoldRing), 1044049, 1044176 + offset, 40.0, 90.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddRes(index, itemType, 1044231 + offset, 1, 1044240);

            index = this.AddCraft(typeof(SilverBeadNecklace), 1044049, 1044185 + offset, 40.0, 90.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddRes(index, itemType, 1044231 + offset, 1, 1044240);

            index = this.AddCraft(typeof(GoldNecklace), 1044049, 1044194 + offset, 40.0, 90.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddRes(index, itemType, 1044231 + offset, 1, 1044240);

            index = this.AddCraft(typeof(GoldEarrings), 1044049, 1044203 + offset, 40.0, 90.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddRes(index, itemType, 1044231 + offset, 1, 1044240);

            index = this.AddCraft(typeof(GoldBeadNecklace), 1044049, 1044212 + offset, 40.0, 90.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddRes(index, itemType, 1044231 + offset, 1, 1044240);

            index = this.AddCraft(typeof(GoldBracelet), 1044049, 1044221 + offset, 40.0, 90.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddRes(index, itemType, 1044231 + offset, 1, 1044240);     
             
        }

        public override void InitCraftList()
        {
            int index = -1;

            #region Jewelry
            this.AddCraft(typeof(GoldRing), 1044049, 1024234, 65.0, 115.0, typeof(IronIngot), 1044036, 3, 1044037);
            this.AddCraft(typeof(GoldBracelet), 1044049, 1024230, 55.0, 105.0, typeof(IronIngot), 1044036, 3, 1044037);

            index = this.AddCraft(typeof(GargishNecklace), 1044049, 1095784, 60.0, 110.0, typeof(IronIngot), 1044036, 3, 1044037);
            SetNeededExpansion(index, Expansion.SA);

            index = this.AddCraft(typeof(GargishBracelet), 1044049, 1095785, 55.0, 105.0, typeof(IronIngot), 1044036, 3, 1044037);
            SetNeededExpansion(index, Expansion.SA);

            index = this.AddCraft(typeof(GargishRing), 1044049, 1095786, 65.0, 115.0, typeof(IronIngot), 1044036, 3, 1044037);
            SetNeededExpansion(index, Expansion.SA);

            index = this.AddCraft(typeof(GargishEarrings), 1044049, 1095787, 55.0, 105.0, typeof(IronIngot), 1044036, 3, 1044037);
            SetNeededExpansion(index, Expansion.SA);

            this.AddJewelrySet(GemType.StarSapphire, typeof(StarSapphire));
            this.AddJewelrySet(GemType.Emerald, typeof(Emerald));
            this.AddJewelrySet(GemType.Sapphire, typeof(Sapphire));
            this.AddJewelrySet(GemType.Ruby, typeof(Ruby));
            this.AddJewelrySet(GemType.Citrine, typeof(Citrine));
            this.AddJewelrySet(GemType.Amethyst, typeof(Amethyst));
            this.AddJewelrySet(GemType.Tourmaline, typeof(Tourmaline));
            this.AddJewelrySet(GemType.Amber, typeof(Amber));
            this.AddJewelrySet(GemType.Diamond, typeof(Diamond));
            #endregion

            #region Wooden Items
            if (Core.SE)
            {
                index = this.AddCraft(typeof(Nunchaku), 1044042, 1030158, 70.0, 120.0, typeof(IronIngot), 1044036, 3, 1044037);
                this.AddRes(index, typeof(Board), 1044041, 8, 1044351);
                this.SetNeededExpansion(index, Expansion.SE);
            }

            this.AddCraft(typeof(JointingPlane), 1044042, 1024144, 0.0, 50.0, typeof(Board), 1044041, 4, 1044351);
            this.AddCraft(typeof(MouldingPlane), 1044042, 1024140, 0.0, 50.0, typeof(Board), 1044041, 4, 1044351);
            this.AddCraft(typeof(SmoothingPlane), 1044042, 1024146, 0.0, 50.0, typeof(Board), 1044041, 4, 1044351);
            this.AddCraft(typeof(ClockFrame), 1044042, 1024173, 0.0, 50.0, typeof(Board), 1044041, 6, 1044351);
            this.AddCraft(typeof(Axle), 1044042, 1024187, -25.0, 25.0, typeof(Board), 1044041, 2, 1044351);
            this.AddCraft(typeof(RollingPin), 1044042, 1024163, 0.0, 50.0, typeof(Board), 1044041, 5, 1044351);

            #region High Seas
            if (Core.HS)
            {
                AddCraft(typeof(Ramrod), 1044042, 1095839, 0.0, 50.0, typeof(Board), 1044041, 8, 1044253);
                SetNeededExpansion(index, Expansion.HS);

                index = AddCraft(typeof(Swab), 1044042, 1095840, 0.0, 50.0, typeof(Cloth), 1044286, 1, 1044253);
                AddRes(index, typeof(Board), 1044041, 4, 1044253);
                SetNeededExpansion(index, Expansion.HS);
            }
            #endregion

            #region SA
            if (Core.SA)
            {
                index = AddCraft(typeof(SoftenedReeds), 1044042, 1112249, 75.0, 100.0, typeof(DryReeds), 1112248, 1, 1112250);
                AddRes(index, typeof(ScouringToxin), 1112292, 2, 1112326);
                SetRequiresBasketWeaving(index);
                SetRequireResTarget(index);
                SetNeededExpansion(index, Expansion.SA);

                index = AddCraft(typeof(RoundBasket), 1044042, 1112293, 75.0, 100.0, typeof(SoftenedReeds), 1112249, 2, 1112251);
                AddRes(index, typeof(Shaft), 1027125, 3, 1044351);
                SetRequireResTarget(index);
                SetRequiresBasketWeaving(index);
                SetNeededExpansion(index, Expansion.SA);

                index = AddCraft(typeof(RoundBasketHandles), 1044042, 1112357, 75.0, 100.0, typeof(SoftenedReeds), 1112249, 2, 1112251);
                AddRes(index, typeof(Shaft), 1027125, 3, 1044351);
                SetRequireResTarget(index);
                SetRequiresBasketWeaving(index);
                SetNeededExpansion(index, Expansion.SA);

                index = AddCraft(typeof(SmallBushel), 1044042, 1112337, 75.0, 100.0, typeof(SoftenedReeds), 1112249, 1, 1112251);
                AddRes(index, typeof(Shaft), 1027125, 2, 1044351);
                SetRequireResTarget(index);
                SetRequiresBasketWeaving(index);
                SetNeededExpansion(index, Expansion.SA);

                index = AddCraft(typeof(PicnicBasket2), 1044042, 1023706, 75.0, 100.0, typeof(SoftenedReeds), 1112249, 1, 1112251);
                AddRes(index, typeof(Shaft), 1027125, 2, 1044351);
                SetRequireResTarget(index);
                SetRequiresBasketWeaving(index);
                SetNeededExpansion(index, Expansion.SA);

                index = AddCraft(typeof(WinnowingBasket), 1044042, 1026274, 75.0, 100.0, typeof(SoftenedReeds), 1112249, 2, 1112251);
                AddRes(index, typeof(Shaft), 1027125, 3, 1044351);
                SetRequireResTarget(index);
                SetRequiresBasketWeaving(index);
                SetNeededExpansion(index, Expansion.SA);

                index = AddCraft(typeof(SquareBasket), 1044042, 1112295, 75.0, 100.0, typeof(SoftenedReeds), 1112249, 2, 1112251);
                AddRes(index, typeof(Shaft), 1027125, 3, 1044351);
                SetRequireResTarget(index);
                SetRequiresBasketWeaving(index);
                SetNeededExpansion(index, Expansion.SA);

                index = AddCraft(typeof(BasketCraftable), 1044042, 1022448, 75.0, 100.0, typeof(SoftenedReeds), 1112249, 2, 1112251);
                AddRes(index, typeof(Shaft), 1027125, 3, 1044351);
                SetRequireResTarget(index);
                SetRequiresBasketWeaving(index);
                SetNeededExpansion(index, Expansion.SA);

                index = AddCraft(typeof(TallRoundBasket), 1044042, 1112297, 75.0, 100.0, typeof(SoftenedReeds), 1112249, 3, 1112251);
                AddRes(index, typeof(Shaft), 1027125, 4, 1044351);
                SetRequireResTarget(index);
                SetRequiresBasketWeaving(index);
                SetNeededExpansion(index, Expansion.SA);

                index = AddCraft(typeof(SmallSquareBasket), 1044042, 1112296, 75.0, 100.0, typeof(SoftenedReeds), 1112249, 1, 1112251);
                AddRes(index, typeof(Shaft), 1027125, 2, 1044351);
                SetRequireResTarget(index);
                SetRequiresBasketWeaving(index);
                SetNeededExpansion(index, Expansion.SA);

                index = AddCraft(typeof(TallBasket), 1044042, 1112299, 75.0, 100.0, typeof(SoftenedReeds), 1112249, 3, 1112251);
                AddRes(index, typeof(Shaft), 1027125, 4, 1044351);
                SetRequireResTarget(index);
                SetRequiresBasketWeaving(index);
                SetNeededExpansion(index, Expansion.SA);

                index = AddCraft(typeof(SmallRoundBasket), 1044042, 1112298, 75.0, 100.0, typeof(SoftenedReeds), 1112249, 1, 1112251);
                AddRes(index, typeof(Shaft), 1027125, 2, 1044351);
                SetRequireResTarget(index);
                SetRequiresBasketWeaving(index);
                SetNeededExpansion(index, Expansion.SA);
            }
            #endregion
            #endregion

            #region Tools
            this.AddCraft(typeof(Scissors), 1044046, 1023998, 5.0, 55.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddCraft(typeof(MortarPestle), 1044046, 1023739, 20.0, 70.0, typeof(IronIngot), 1044036, 3, 1044037);
            this.AddCraft(typeof(Scorp), 1044046, 1024327, 30.0, 80.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddCraft(typeof(TinkerTools), 1044046, 1044164, 10.0, 60.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddCraft(typeof(Hatchet), 1044046, 1023907, 30.0, 80.0, typeof(IronIngot), 1044036, 4, 1044037);
            this.AddCraft(typeof(DrawKnife), 1044046, 1024324, 30.0, 80.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddCraft(typeof(SewingKit), 1044046, 1023997, 10.0, 70.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddCraft(typeof(Saw), 1044046, 1024148, 30.0, 80.0, typeof(IronIngot), 1044036, 4, 1044037);
            this.AddCraft(typeof(DovetailSaw), 1044046, 1024136, 30.0, 80.0, typeof(IronIngot), 1044036, 4, 1044037);
            this.AddCraft(typeof(Froe), 1044046, 1024325, 30.0, 80.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddCraft(typeof(Shovel), 1044046, 1023898, 40.0, 90.0, typeof(IronIngot), 1044036, 4, 1044037);
            this.AddCraft(typeof(Hammer), 1044046, 1024138, 30.0, 80.0, typeof(IronIngot), 1044036, 1, 1044037);
            this.AddCraft(typeof(Tongs), 1044046, 1024028, 35.0, 85.0, typeof(IronIngot), 1044036, 1, 1044037);
            this.AddCraft(typeof(SmithHammer), 1044046, 1025091, 40.0, 90.0, typeof(IronIngot), 1044036, 4, 1044037);
            this.AddCraft(typeof(SledgeHammer), 1044046, 1024021, 40.0, 90.0, typeof(IronIngot), 1044036, 4, 1044037);
            this.AddCraft(typeof(Inshave), 1044046, 1024326, 30.0, 80.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddCraft(typeof(Pickaxe), 1044046, 1023718, 40.0, 90.0, typeof(IronIngot), 1044036, 4, 1044037);
            this.AddCraft(typeof(Pitchfork), 1044046, 1023719, 40.0, 90.0, typeof(IronIngot), 1044036, 4, 1044037);
            this.AddCraft(typeof(Lockpick), 1044046, 1025371, 45.0, 95.0, typeof(IronIngot), 1044036, 1, 1044037);
            this.AddCraft(typeof(Skillet), 1044046, 1044567, 30.0, 80.0, typeof(IronIngot), 1044036, 4, 1044037);
            this.AddCraft(typeof(FlourSifter), 1044046, 1024158, 50.0, 100.0, typeof(IronIngot), 1044036, 3, 1044037);
            this.AddCraft(typeof(FletcherTools), 1044046, 1044166, 35.0, 85.0, typeof(IronIngot), 1044036, 3, 1044037);
            this.AddCraft(typeof(MapmakersPen), 1044046, 1044167, 25.0, 75.0, typeof(IronIngot), 1044036, 1, 1044037);
            this.AddCraft(typeof(ScribesPen), 1044046, 1044168, 25.0, 75.0, typeof(IronIngot), 1044036, 1, 1044037);
            this.AddCraft(typeof(Clippers), 1044046, 1112117, 50.0, 50.0, typeof(IronIngot), 1044036, 4, 1044037);

            #region Mondain's Legacy
            if (Core.ML)
            {
                this.AddCraft(typeof(MetalContainerEngraver), 1044046, 1072154, 75.0, 100.0, typeof(IronIngot), 1044036, 4, 1044037);
                this.AddRes(index, typeof(Springs), 1044171, 1, 1044253);
                this.AddRes(index, typeof(Gears), 1044254, 2, 1044253);
                this.AddRes(index, typeof(Diamond), 1062608, 1, 1044240);
                this.SetNeededExpansion(index, Expansion.ML);
            }
            #endregion

            #endregion

            #region Parts
            this.AddCraft(typeof(Gears), 1044047, 1024179, 5.0, 55.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddCraft(typeof(ClockParts), 1044047, 1024175, 25.0, 75.0, typeof(IronIngot), 1044036, 1, 1044037);
            this.AddCraft(typeof(BarrelTap), 1044047, 1024100, 35.0, 85.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddCraft(typeof(Springs), 1044047, 1024189, 5.0, 55.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddCraft(typeof(SextantParts), 1044047, 1024185, 30.0, 80.0, typeof(IronIngot), 1044036, 4, 1044037);
            this.AddCraft(typeof(BarrelHoops), 1044047, 1024321, -15.0, 35.0, typeof(IronIngot), 1044036, 5, 1044037);
            this.AddCraft(typeof(Hinge), 1044047, 1024181, 5.0, 55.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddCraft(typeof(BolaBall), 1044047, 1023699, 45.0, 95.0, typeof(IronIngot), 1044036, 10, 1044037);
            
            if (Core.ML)
            {
                index = this.AddCraft(typeof(JeweledFiligree), 1044047, 1072894, 70.0, 110.0, typeof(IronIngot), 1044036, 2, 1044037);
                this.AddRes(index, typeof(StarSapphire), 1044231, 1, 1044253);
                this.AddRes(index, typeof(Ruby), 1044234, 1, 1044253);
                this.SetNeededExpansion(index, Expansion.ML);
            }
            
            #endregion

            #region Utensils
            this.AddCraft(typeof(ButcherKnife), 1044048, 1025110, 25.0, 75.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddCraft(typeof(SpoonLeft), 1044048, 1044158, 0.0, 50.0, typeof(IronIngot), 1044036, 1, 1044037);
            this.AddCraft(typeof(SpoonRight), 1044048, 1044159, 0.0, 50.0, typeof(IronIngot), 1044036, 1, 1044037);
            this.AddCraft(typeof(Plate), 1044048, 1022519, 0.0, 50.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddCraft(typeof(ForkLeft), 1044048, 1044160, 0.0, 50.0, typeof(IronIngot), 1044036, 1, 1044037);
            this.AddCraft(typeof(ForkRight), 1044048, 1044161, 0.0, 50.0, typeof(IronIngot), 1044036, 1, 1044037);
            this.AddCraft(typeof(Cleaver), 1044048, 1023778, 20.0, 70.0, typeof(IronIngot), 1044036, 3, 1044037);
            this.AddCraft(typeof(KnifeLeft), 1044048, 1044162, 0.0, 50.0, typeof(IronIngot), 1044036, 1, 1044037);
            this.AddCraft(typeof(KnifeRight), 1044048, 1044163, 0.0, 50.0, typeof(IronIngot), 1044036, 1, 1044037);
            this.AddCraft(typeof(Goblet), 1044048, 1022458, 10.0, 60.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddCraft(typeof(PewterMug), 1044048, 1024097, 10.0, 60.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddCraft(typeof(SkinningKnife), 1044048, 1023781, 25.0, 75.0, typeof(IronIngot), 1044036, 2, 1044037);
            #endregion

            #region Misc
            this.AddCraft(typeof(KeyRing), 1044050, 1024113, 10.0, 60.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddCraft(typeof(Candelabra), 1044050, 1022599, 55.0, 105.0, typeof(IronIngot), 1044036, 4, 1044037);
            this.AddCraft(typeof(Scales), 1044050, 1026225, 60.0, 110.0, typeof(IronIngot), 1044036, 4, 1044037);
            this.AddCraft(typeof(Key), 1044050, 1024112, 20.0, 70.0, typeof(IronIngot), 1044036, 3, 1044037);
            this.AddCraft(typeof(Globe), 1044050, 1024167, 55.0, 105.0, typeof(IronIngot), 1044036, 4, 1044037);
            this.AddCraft(typeof(Spyglass), 1044050, 1025365, 60.0, 110.0, typeof(IronIngot), 1044036, 4, 1044037);
            this.AddCraft(typeof(Lantern), 1044050, 1022597, 30.0, 80.0, typeof(IronIngot), 1044036, 2, 1044037);
            this.AddCraft(typeof(HeatingStand), 1044050, 1026217, 60.0, 110.0, typeof(IronIngot), 1044036, 4, 1044037);

            if (Core.SE)
            {
                index = this.AddCraft(typeof(ShojiLantern), 1044050, 1029404, 65.0, 115.0, typeof(IronIngot), 1044036, 10, 1044037);
                this.AddRes(index, typeof(Board), 1044041, 5, 1044351);
                this.SetNeededExpansion(index, Expansion.SE);

                index = this.AddCraft(typeof(PaperLantern), 1044050, 1029406, 65.0, 115.0, typeof(IronIngot), 1044036, 10, 1044037);
                this.AddRes(index, typeof(Board), 1044041, 5, 1044351);
                this.SetNeededExpansion(index, Expansion.SE);

                index = this.AddCraft(typeof(RoundPaperLantern), 1044050, 1029418, 65.0, 115.0, typeof(IronIngot), 1044036, 10, 1044037);
                this.AddRes(index, typeof(Board), 1044041, 5, 1044351);
                this.SetNeededExpansion(index, Expansion.SE);

                index = this.AddCraft(typeof(WindChimes), 1044050, 1030290, 80.0, 130.0, typeof(IronIngot), 1044036, 15, 1044037);
                this.SetNeededExpansion(index, Expansion.SE);

                index = this.AddCraft(typeof(FancyWindChimes), 1044050, 1030291, 80.0, 130.0, typeof(IronIngot), 1044036, 15, 1044037);
                this.SetNeededExpansion(index, Expansion.SE);
            }

            #region High Seas
            if (Core.HS)
            {
                index = AddCraft(typeof(Matches), 1044050, 1096648, 15.0, 70.0, typeof(Matchcord), 1095184, 10, 1044367);
                AddRes(index, typeof(Board), 1044041, 4, 1044351);
                SetNeededExpansion(index, Expansion.HS);
            }
            #endregion

            #region Stygian Abyss
            index = AddCraft(typeof(GorgonLense), 1044050, 1112625, 90.0, 120.0, typeof(RedScales), 1112626, 2, 1053097);
            AddRes(index, typeof(CrystalDust), 1112328, 3, 1044253);
            ForceNonExceptional(index);
            SetItemHue(index, 1266);

            index = AddCraft(typeof(ScaleCollar), 1044050, 1112625, 50.0, 100.0, typeof(RedScales), 1112626, 4, 1053097);
            AddRes(index, typeof(Scourge), 1032677, 1, 1044253);
            SetNeededExpansion(index, Expansion.SA);

            index = AddCraft(typeof(CraftableDragonLamp), 1044050, 1098404, 75.0, 125.0, typeof(IronIngot), 1044036, 8, 1044253);
            AddRes(index, typeof(Candelabra), 1011213, 1, 1154172);
            AddRes(index, typeof(WorkableGlass), 1154170, 1, 1154171);

            index = AddCraft(typeof(StainedGlassLamp), 1044050, 1098408, 75.0, 125.0, typeof(IronIngot), 1044036, 8, 1044253);
            AddRes(index, typeof(Candelabra), 1011213, 1, 1154172);
            AddRes(index, typeof(WorkableGlass), 1154170, 1, 1154171);

            index = AddCraft(typeof(TallDoubleLamp), 1044050, 1098414, 75.0, 125.0, typeof(IronIngot), 1044036, 8, 1044253);
            AddRes(index, typeof(Candelabra), 1011213, 1, 1154172);
            AddRes(index, typeof(WorkableGlass), 1154170, 1, 1154171);
            #endregion

            #region TOL
            index = AddCraft(typeof(CraftableHouseAddonDeed), 1044050, 1155851, 40.0, 90.0, typeof(IronIngot), 1044036, 8, 1044253);
            SetData(index, CraftableAddonType.CurledMetalSignHanger);
            SetDisplayID(index, 2971);
            SetNeededExpansion(index, Expansion.TOL);

            index = AddCraft(typeof(CraftableHouseAddonDeed), 1044050, 1155852, 40.0, 90.0, typeof(IronIngot), 1044036, 8, 1044253);
            SetData(index, CraftableAddonType.FlourishedMetalSignHanger);
            SetDisplayID(index, 2973);
            SetNeededExpansion(index, Expansion.TOL);

            index = AddCraft(typeof(CraftableHouseAddonDeed), 1044050, 1155853, 40.0, 90.0, typeof(IronIngot), 1044036, 8, 1044253);
            SetData(index, CraftableAddonType.InwardCurledMetalSignHanger);
            SetDisplayID(index, 2975);
            SetNeededExpansion(index, Expansion.TOL);

            index = AddCraft(typeof(CraftableHouseAddonDeed), 1044050, 1155854, 40.0, 90.0, typeof(IronIngot), 1044036, 8, 1044253);
            SetData(index, CraftableAddonType.EndCurledMetalSignHanger);
            SetDisplayID(index, 2977);
            SetNeededExpansion(index, Expansion.TOL);

            index = AddCraft(typeof(CraftableHouseDoorDeed), 1044050, 1156080, 85.0, 135.0, typeof(IronIngot), 1044036, 50, 1044253);
            SetData(index, DoorType.LeftMetalDoor_S_In);
            SetDisplayID(index, 1653);
            SetNeededExpansion(index, Expansion.TOL);

            index = AddCraft(typeof(CraftableHouseDoorDeed), 1044050, 1156081, 85.0, 135.0, typeof(IronIngot), 1044036, 50, 1044253);
            SetData(index, DoorType.RightMetalDoor_S_In);
            SetDisplayID(index, 1659);
            SetNeededExpansion(index, Expansion.TOL);

            index = AddCraft(typeof(CraftableHouseDoorDeed), 1044050, 1156082, 85.0, 135.0, typeof(IronIngot), 1044036, 50, 1044253);
            SetData(index, DoorType.LeftMetalDoor_E_Out);
            SetDisplayID(index, 1660);
            SetNeededExpansion(index, Expansion.TOL);

            index = AddCraft(typeof(CraftableHouseDoorDeed), 1044050, 1156083, 85.0, 135.0, typeof(IronIngot), 1044036, 50, 1044253);
            SetData(index, DoorType.RightMetalDoor_E_Out);
            SetDisplayID(index, 1663);
            SetNeededExpansion(index, Expansion.TOL);

            index = AddCraft(typeof(WallSafeDeed), 1044050, 1155860, 0.0, 0.0, typeof(IronIngot), 1044036, 20, 1044253);
            ForceNonExceptional(index);
            SetNeededExpansion(index, Expansion.TOL);

            index = AddCraft(typeof(CraftableHouseDoorDeed), 1044050, 1156352, 85.0, 135.0, typeof(IronIngot), 1044036, 50, 1044253);
            SetData(index, DoorType.LeftMetalDoor_E_In);
            SetDisplayID(index, 1660);
            SetNeededExpansion(index, Expansion.TOL);

            index = AddCraft(typeof(CraftableHouseDoorDeed), 1044050, 1156353, 85.0, 135.0, typeof(IronIngot), 1044036, 50, 1044253);
            SetData(index, DoorType.RightMetalDoor_E_In);
            SetDisplayID(index, 1663);
            SetNeededExpansion(index, Expansion.TOL);

            index = AddCraft(typeof(CraftableHouseDoorDeed), 1044050, 1156350, 85.0, 135.0, typeof(IronIngot), 1044036, 50, 1044253);
            SetData(index, DoorType.LeftMetalDoor_S_Out);
            SetDisplayID(index, 1653);
            SetNeededExpansion(index, Expansion.TOL);

            index = AddCraft(typeof(CraftableHouseDoorDeed), 1044050, 1156351, 85.0, 135.0, typeof(IronIngot), 1044036, 50, 1044253);
            SetData(index, DoorType.RightMetalDoor_S_Out);
            SetDisplayID(index, 1659);
            SetNeededExpansion(index, Expansion.TOL);

            index = AddCraft(typeof(KotlPowerCore), 1044050, 1124179, 85.0, 135.0, typeof(WorkableGlass), 1154170, 5, 1154171);
            AddRes(index, typeof(CopperWire), 1026265, 5, 1150700);
            AddRes(index, typeof(IronIngot), 1044036, 100, 1044253);
            AddRes(index, typeof(MoonstoneCrystalShard), 1124142, 5, 1156701);
            AddRecipe(index, (int)TinkerRecipes.KotlPowerCore);
            SetNeededExpansion(index, Expansion.TOL);
            #endregion

            #endregion

            #region Multi-Component Items
            index = this.AddCraft(typeof(AxleGears), 1044051, 1024177, 0.0, 0.0, typeof(Axle), 1044169, 1, 1044253);
            this.AddRes(index, typeof(Gears), 1044254, 1, 1044253);

            index = this.AddCraft(typeof(ClockParts), 1044051, 1024175, 0.0, 0.0, typeof(AxleGears), 1044170, 1, 1044253);
            this.AddRes(index, typeof(Springs), 1044171, 1, 1044253);

            index = this.AddCraft(typeof(SextantParts), 1044051, 1024185, 0.0, 0.0, typeof(AxleGears), 1044170, 1, 1044253);
            this.AddRes(index, typeof(Hinge), 1044172, 1, 1044253);

            index = this.AddCraft(typeof(ClockRight), 1044051, 1044257, 0.0, 0.0, typeof(ClockFrame), 1044174, 1, 1044253);
            this.AddRes(index, typeof(ClockParts), 1044173, 1, 1044253);

            index = this.AddCraft(typeof(ClockLeft), 1044051, 1044256, 0.0, 0.0, typeof(ClockFrame), 1044174, 1, 1044253);
            this.AddRes(index, typeof(ClockParts), 1044173, 1, 1044253);

            this.AddCraft(typeof(Sextant), 1044051, 1024183, 0.0, 0.0, typeof(SextantParts), 1044175, 1, 1044253);

            index = this.AddCraft(typeof(Bola), 1044051, 1046441, 60.0, 80.0, typeof(BolaBall), 1046440, 4, 1042613);
            this.AddRes(index, typeof(Leather), 1044462, 3, 1044463);

            index = this.AddCraft(typeof(PotionKeg), 1044051, 1044258, 75.0, 100.0, typeof(Keg), 1044255, 1, 1044253);
            this.AddRes(index, typeof(Bottle), 1044250, 10, 1044253);
            this.AddRes(index, typeof(BarrelLid), 1044251, 1, 1044253);
            this.AddRes(index, typeof(BarrelTap), 1044252, 1, 1044253);
            
            if (Core.SA)
            {
                index = this.AddCraft(typeof(ModifiedClockworkAssembly), 1044051, 1113031, 65.0, 115.0, typeof(ClockworkAssembly), 1073426, 1, 502910);
                this.AddRes(index, typeof(PowerCrystal), 1112811, 1, 502910);
                this.AddRes(index, typeof(VoidEssence), 1112327, 1, 502910);
                this.SetNeededExpansion(index, Expansion.SA);
                this.ForceNonExceptional(index);

                index = this.AddCraft(typeof(ModifiedClockworkAssembly), 1044051, 1113032, 65.0, 115.0, typeof(ClockworkAssembly), 1073426, 1, 502910);
                this.AddRes(index, typeof(PowerCrystal), 1112811, 1, 502910);
                this.AddRes(index, typeof(VoidEssence), 1112327, 2, 502910);
                this.SetNeededExpansion(index, Expansion.SA);
                this.ForceNonExceptional(index);

                index = this.AddCraft(typeof(ModifiedClockworkAssembly), 1044051, 1113033, 65.0, 115.0, typeof(ClockworkAssembly), 1073426, 1, 502910);
                this.AddRes(index, typeof(PowerCrystal), 1112811, 1, 502910);
                this.AddRes(index, typeof(VoidEssence), 1112327, 3, 502910);
                this.SetNeededExpansion(index, Expansion.SA);
                this.ForceNonExceptional(index);

                index = this.AddCraft(typeof(ArcanicRuneStone), 1044051, 1113352, 90.0, 140.0, typeof(CrystalShards), 1073161, 1, 1044253);
                this.AddRes(index, typeof(PowerCrystal), 1112811, 5, 502910);
                this.AddSkill(index, SkillName.Magery, 80.0, 85.0);
                this.SetNeededExpansion(index, Expansion.SA);
                this.ForceNonExceptional(index);
            }

            #region Hitching Post
            if (Core.ML)
            {
                index = this.AddCraft(typeof(HitchingRope), 1044051, 1071124, 60.0, 120.0, typeof(Rope), 1020934, 1, 1044253);
                this.AddSkill(index, SkillName.AnimalLore, 15.0, 100.0);
                this.AddRes(index, typeof(ResolvesBridle), 1074761, 1, 1044253);

                index = this.AddCraft(typeof(HitchingPost), 1044051, 1071127, 90.0, 160.0, typeof(IronIngot), 1044036, 50, 1044253);
                this.AddRes(index, typeof(AnimalPheromone), 1071200, 1, 1044253);
                this.AddRes(index, typeof(HitchingRope), 1071124, 2, 1044253);
                this.AddRes(index, typeof(PhillipsWoodenSteed), 1063488, 1, 1044253);
            }
            #endregion

            if (Core.SA)
            {
                index = this.AddCraft(typeof(VoidOrb), 1044051, 1113354, 90.0, 104.3, typeof(DarkSapphire), 1032690, 1, 1044253);
                this.AddSkill(index, SkillName.Magery, 80.0, 100.0);
                this.AddRes(index, typeof(BlackPearl), 1015001, 50, 1044253);
                this.SetNeededExpansion(index, Expansion.SA);
                this.ForceNonExceptional(index);
            }

            index = AddCraft(typeof(AdvancedTrainingDummyEastDeed), 1044051, 1150596, 90.0, 120.0, typeof(TrainingDummyEastDeed), 1044335, 1, 1044253);
            AddRes(index, typeof(PlateChest), 1025141, 1, 1044253);
            AddRes(index, typeof(CloseHelm), 1025128, 1, 1044253);
            AddRes(index, typeof(Broadsword), 1015055, 1, 1044253);
            ForceNonExceptional(index);

            index = AddCraft(typeof(AdvancedTrainingDummySouthDeed), 1044051, 1150595, 90.0, 120.0, typeof(TrainingDummySouthDeed), 1044336, 1, 1044253);
            AddRes(index, typeof(PlateChest), 1025141, 1, 1044253);
            AddRes(index, typeof(CloseHelm), 1025128, 1, 1044253);
            AddRes(index, typeof(Broadsword), 1015055, 1, 1044253);
            ForceNonExceptional(index);

            index = AddCraft(typeof(DistilleryEastAddonDeed), 1044051, 1150664, 90.0, 120.0, typeof(MetalKeg), 1150675, 2, 1044253);
            AddRes(index, typeof(HeatingStand), 1011224, 4, 1044253);
            AddRes(index, typeof(CopperWire), 1026265, 1, 1044253);
            ForceNonExceptional(index);

            index = AddCraft(typeof(DistillerySouthAddonDeed), 1044051, 1150663, 90.0, 120.0, typeof(MetalKeg), 1150675, 2, 1044253);
            AddRes(index, typeof(HeatingStand), 1011224, 4, 1044253);
            AddRes(index, typeof(CopperWire), 1026265, 1, 1044253);
            ForceNonExceptional(index);

            index = AddCraft(typeof(KotlAutomatonHead), 1044051, 1156998, 100.0, 580.0, typeof(IronIngot), 1044036, 300, 1044037);
            SetMinSkillOffset(index, 25.0);
            AddRes(index, typeof(AutomatonActuator), 1156997, 1, 1156999);
            AddRes(index, typeof(StasisChamberPowerCore), 1156623, 1, 1157000);
            AddRes(index, typeof(InoperativeAutomatonHead), 1157002, 1, 1157001);
            SetNeededExpansion(index, Expansion.TOL);
            AddRecipe(index, (int)TinkerRecipes.KotlAutomatonHead);

            #endregion

            #region Traps
            // Dart Trap
            index = this.AddCraft(typeof(DartTrapCraft), 1044052, 1024396, 30.0, 80.0, typeof(IronIngot), 1044036, 1, 1044037);
            this.AddRes(index, typeof(Bolt), 1044570, 1, 1044253);

            // Poison Trap
            index = this.AddCraft(typeof(PoisonTrapCraft), 1044052, 1044593, 30.0, 80.0, typeof(IronIngot), 1044036, 1, 1044037);
            this.AddRes(index, typeof(BasePoisonPotion), 1044571, 1, 1044253);

            // Explosion Trap
            index = this.AddCraft(typeof(ExplosionTrapCraft), 1044052, 1044597, 55.0, 105.0, typeof(IronIngot), 1044036, 1, 1044037);
            this.AddRes(index, typeof(BaseExplosionPotion), 1044569, 1, 1044253);

            // Faction Gas Trap
            index = this.AddCraft(typeof(FactionGasTrapDeed), 1044052, 1044598, 65.0, 115.0, typeof(Silver), 1044572, Core.AOS ? 250 : 1000, 1044253);
            this.AddRes(index, typeof(IronIngot), 1044036, 10, 1044037);
            this.AddRes(index, typeof(BasePoisonPotion), 1044571, 1, 1044253);

            // Faction explosion Trap
            index = this.AddCraft(typeof(FactionExplosionTrapDeed), 1044052, 1044599, 65.0, 115.0, typeof(Silver), 1044572, Core.AOS ? 250 : 1000, 1044253);
            this.AddRes(index, typeof(IronIngot), 1044036, 10, 1044037);
            this.AddRes(index, typeof(BaseExplosionPotion), 1044569, 1, 1044253);

            // Faction Saw Trap
            index = this.AddCraft(typeof(FactionSawTrapDeed), 1044052, 1044600, 65.0, 115.0, typeof(Silver), 1044572, Core.AOS ? 250 : 1000, 1044253);
            this.AddRes(index, typeof(IronIngot), 1044036, 10, 1044037);
            this.AddRes(index, typeof(Gears), 1044254, 1, 1044253);

            // Faction Spike Trap           
            index = this.AddCraft(typeof(FactionSpikeTrapDeed), 1044052, 1044601, 65.0, 115.0, typeof(Silver), 1044572, Core.AOS ? 250 : 1000, 1044253);
            this.AddRes(index, typeof(IronIngot), 1044036, 10, 1044037);
            this.AddRes(index, typeof(Springs), 1044171, 1, 1044253);

            // Faction trap removal kit
            index = this.AddCraft(typeof(FactionTrapRemovalKit), 1044052, 1046445, 90.0, 115.0, typeof(Silver), 1044572, 500, 1044253);
            this.AddRes(index, typeof(IronIngot), 1044036, 10, 1044037);
            #endregion

            #region Mondain's Legacy Magic Jewlery
            if (Core.ML)
            {
                index = this.AddCraft(typeof(BrilliantAmberBracelet), 1073107, 1073453, 75.0, 125.0, typeof(IronIngot), 1044036, 5, 1044037);
                this.AddRes(index, typeof(Amber), 1062607, 20, 1044240);
                this.AddRes(index, typeof(BrilliantAmber), 1032697, 10, 1044240);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(FireRubyBracelet), 1073107, 1073454, 75.0, 125.0, typeof(IronIngot), 1044036, 5, 1044037);
                this.AddRes(index, typeof(Ruby), 1062603, 20, 1044240);
                this.AddRes(index, typeof(FireRuby), 1032695, 10, 1044240);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(DarkSapphireBracelet), 1073107, 1073455, 75.0, 125.0, typeof(IronIngot), 1044036, 5, 1044037);
                this.AddRes(index, typeof(Sapphire), 1062602, 20, 1044240);
                this.AddRes(index, typeof(DarkSapphire), 1032690, 10, 1044240);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(WhitePearlBracelet), 1073107, 1073456, 75.0, 125.0, typeof(IronIngot), 1044036, 5, 1044037);
                this.AddRes(index, typeof(Tourmaline), 1062606, 20, 1044240);
                this.AddRes(index, typeof(WhitePearl), 1032694, 10, 1044240);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(EcruCitrineRing), 1073107, 1073457, 75.0, 125.0, typeof(IronIngot), 1044036, 5, 1044037);
                this.AddRes(index, typeof(Citrine), 1062604, 20, 1044240);
                this.AddRes(index, typeof(EcruCitrine), 1032693, 10, 1044240);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(BlueDiamondRing), 1073107, 1073458, 75.0, 125.0, typeof(IronIngot), 1044036, 5, 1044037);
                this.AddRes(index, typeof(Diamond), 1062608, 20, 1044240);
                this.AddRes(index, typeof(BlueDiamond), 1032696, 10, 1044240);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(PerfectEmeraldRing), 1073107, 1073459, 75.0, 125.0, typeof(IronIngot), 1044036, 5, 1044037);
                this.AddRes(index, typeof(Emerald), 1062601, 20, 1044240);
                this.AddRes(index, typeof(PerfectEmerald), 1032692, 10, 1044240);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(TurqouiseRing), 1073107, 1073460, 75.0, 125.0, typeof(IronIngot), 1044036, 5, 1044037);
                this.AddRes(index, typeof(Amethyst), 1062605, 20, 1044240);
                this.AddRes(index, typeof(Turquoise), 1032691, 10, 1044240);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(ResilientBracer), 1073107, 1072933, 100.0, 125.0, typeof(IronIngot), 1044036, 2, 1044037);
                this.SetMinSkillOffset(index, 25.0);
                this.AddRes(index, typeof(CapturedEssence), 1032686, 1, 1044253);
                this.AddRes(index, typeof(BlueDiamond), 1032696, 10, 1044253);
                this.AddRes(index, typeof(Diamond), 1062608, 50, 1044253);
                this.AddRecipe(index, (int)TinkerRecipes.ResilientBracer);
                this.ForceNonExceptional(index);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(EssenceOfBattle), 1073107, 1072935, 100.0, 125.0, typeof(IronIngot), 1044036, 2, 1044037);
                this.SetMinSkillOffset(index, 25.0);
                this.AddRes(index, typeof(CapturedEssence), 1032686, 1, 1044253);
                this.AddRes(index, typeof(FireRuby), 1032695, 10, 1044253);
                this.AddRes(index, typeof(Ruby), 1062603, 50, 1044253);
                this.AddRecipe(index, (int)TinkerRecipes.EssenceOfBattle);
                this.ForceNonExceptional(index);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(PendantOfTheMagi), 1073107, 1072937, 100.0, 125.0, typeof(IronIngot), 1044036, 2, 1044037);
                this.SetMinSkillOffset(index, 25.0);
                this.AddRes(index, typeof(EyeOfTheTravesty), 1032685, 1, 1044253);
                this.AddRes(index, typeof(WhitePearl), 1032694, 5, 1044253);
                this.AddRes(index, typeof(StarSapphire), 1062600, 50, 1044253);
                this.AddRecipe(index, (int)TinkerRecipes.PendantOfTheMagi);
                this.ForceNonExceptional(index);
                this.SetNeededExpansion(index, Expansion.ML);
            }
            if (Core.SA)
            {
                index = this.AddCraft(typeof(GargishBracelet), 1044049, 1095785, 65.0, 115.0, typeof(IronIngot), 1044036, 6, 1044037);
                this.AddRes(index, typeof(BrilliantAmber), 1032697, 1, 1044240);
                this.SetNeededExpansion(index, Expansion.SA);
     
                index = this.AddCraft(typeof(GargishRing), 1044049, 1095786, 65.0, 115.0, typeof(IronIngot), 1044036, 3, 1044037);
                this.AddRes(index, typeof(StarSapphire), 1062600, 1, 1044253);
                this.SetNeededExpansion(index, Expansion.SA);
     
                index = this.AddCraft(typeof(GargishNecklace), 1044049, 1095784, 65.0, 115.0, typeof(IronIngot), 1044036, 6, 1044037);
                this.AddRes(index, typeof(FireRuby), 1032695, 1, 1044253);
                this.SetNeededExpansion(index, Expansion.SA);
     
                index = this.AddCraft(typeof(GargishEarrings), 1044049, 1095787, 65.0, 115.0, typeof(IronIngot), 1044036, 4, 1044037);
                this.AddRes(index, typeof(BlueDiamond), 1032696, 1, 1044253);
                this.SetNeededExpansion(index, Expansion.SA);

                index = this.AddCraft(typeof(DrSpectorsLenses), 1073107, 1156991, 100.0, 580.0, typeof(IronIngot), 1044036, 20, 1044037);
                this.SetMinSkillOffset(index, 25.0);
                this.AddRes(index, typeof(BlackrockMoonstone), 1156993, 1, 1156992);
                this.AddRes(index, typeof(HatOfTheMagi), 1061597, 1, 1044253);
                this.AddRecipe(index, (int)TinkerRecipes.DrSpectorLenses);
                this.ForceNonExceptional(index);
                this.SetNeededExpansion(index, Expansion.TOL);

                index = this.AddCraft(typeof(BraceletOfPrimalConsumption), 1073107, 1157350, 100.0, 580.0, typeof(IronIngot), 1044036, 3, 1044037);
                this.SetMinSkillOffset(index, 25.0);
                this.AddRes(index, typeof(RingOfTheElements), 1061104, 1, 1044253);
                this.AddRes(index, typeof(BloodOfTheDarkFather), 1157343, 5, 1044253);
                this.AddRes(index, typeof(WhitePearl), 1032694, 10, 1044240);
                this.AddRecipe(index, (int)TinkerRecipes.BraceletOfPrimalConsumption);
                this.ForceNonExceptional(index);
                this.SetNeededExpansion(index, Expansion.TOL);
            }
            #endregion

            // Set the overridable material
            this.SetSubRes(typeof(IronIngot), 1044022);

            // Add every material you want the player to be able to choose from
            // This will override the overridable material
            this.AddSubRes(typeof(IronIngot), 1044022, 00.0, 1044036, 1044267);
            this.AddSubRes(typeof(DullCopperIngot), 1044023, 65.0, 1044036, 1044268);
            this.AddSubRes(typeof(ShadowIronIngot), 1044024, 70.0, 1044036, 1044268);
            this.AddSubRes(typeof(CopperIngot), 1044025, 75.0, 1044036, 1044268);
            this.AddSubRes(typeof(BronzeIngot), 1044026, 80.0, 1044036, 1044268);
            this.AddSubRes(typeof(GoldIngot), 1044027, 85.0, 1044036, 1044268);
            this.AddSubRes(typeof(AgapiteIngot), 1044028, 90.0, 1044036, 1044268);
            this.AddSubRes(typeof(VeriteIngot), 1044029, 95.0, 1044036, 1044268);
            this.AddSubRes(typeof(ValoriteIngot), 1044030, 99.0, 1044036, 1044268);

            this.MarkOption = true;
            this.Repair = true;
            this.CanEnhance = Core.AOS;
            this.CanAlter = Core.SA;
        }
    }

    public abstract class TrapCraft : CustomCraft
    {
        private LockableContainer m_Container;

        public LockableContainer Container
        {
            get
            {
                return this.m_Container;
            }
        }

        public abstract TrapType TrapType { get; }

        public TrapCraft(Mobile from, CraftItem craftItem, CraftSystem craftSystem, Type typeRes, BaseTool tool, int quality)
            : base(from, craftItem, craftSystem, typeRes, tool, quality)
        {
        }

        private int Verify(LockableContainer container)
        {
            if (container == null || container.KeyValue == 0)
                return 1005638; // You can only trap lockable chests.
            if (this.From.Map != container.Map || !this.From.InRange(container.GetWorldLocation(), 2))
                return 500446; // That is too far away.
            if (!container.Movable)
                return 502944; // You cannot trap this item because it is locked down.
            if (!container.IsAccessibleTo(this.From))
                return 502946; // That belongs to someone else.
            if (container.Locked)
                return 502943; // You can only trap an unlocked object.
            if (container.TrapType != TrapType.None)
                return 502945; // You can only place one trap on an object at a time.

            return 0;
        }

        private bool Acquire(object target, out int message)
        {
            LockableContainer container = target as LockableContainer;

            message = this.Verify(container);

            if (message > 0)
            {
                return false;
            }
            else
            {
                this.m_Container = container;
                return true;
            }
        }

        public override void EndCraftAction()
        {
            this.From.SendLocalizedMessage(502921); // What would you like to set a trap on?
            this.From.Target = new ContainerTarget(this);
        }

        private class ContainerTarget : Target
        {
            private readonly TrapCraft m_TrapCraft;

            public ContainerTarget(TrapCraft trapCraft)
                : base(-1, false, TargetFlags.None)
            {
                this.m_TrapCraft = trapCraft;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                int message;

                if (this.m_TrapCraft.Acquire(targeted, out message))
                    this.m_TrapCraft.CraftItem.CompleteCraft(this.m_TrapCraft.Quality, false, this.m_TrapCraft.From, this.m_TrapCraft.CraftSystem, this.m_TrapCraft.TypeRes, this.m_TrapCraft.Tool, this.m_TrapCraft);
                else
                    this.Failure(message);
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                if (cancelType == TargetCancelType.Canceled)
                    this.Failure(0);
            }

            private void Failure(int message)
            {
                Mobile from = this.m_TrapCraft.From;
                BaseTool tool = this.m_TrapCraft.Tool;

                if (Siege.SiegeShard)
                {
                    AOS.Damage(from, Utility.RandomMinMax(80, 120), 50, 50, 0, 0, 0);
                    message = 502902; // You fail to set the trap, and inadvertantly hurt yourself in the process.
                }

                if (tool != null && !tool.Deleted && tool.UsesRemaining > 0)
                    from.SendGump(new CraftGump(from, this.m_TrapCraft.CraftSystem, tool, message));
                else if (message > 0)
                    from.SendLocalizedMessage(message);
            }
        }

        public override Item CompleteCraft(out int message)
        {
            message = this.Verify(this.Container);

            if (message == 0)
            {
                int trapLevel = (int)(this.From.Skills.Tinkering.Value / 10);

                this.Container.TrapType = this.TrapType;
                this.Container.TrapPower = trapLevel * 9;
                this.Container.TrapLevel = trapLevel;
                this.Container.TrapOnLockpick = true;

                message = 1005639; // Trap is disabled until you lock the chest.
            }

            return null;
        }
    }

    [CraftItemID(0x1BFC)]
    public class DartTrapCraft : TrapCraft
    {
        public override TrapType TrapType
        {
            get
            {
                return TrapType.DartTrap;
            }
        }

        public DartTrapCraft(Mobile from, CraftItem craftItem, CraftSystem craftSystem, Type typeRes, BaseTool tool, int quality)
            : base(from, craftItem, craftSystem, typeRes, tool, quality)
        {
        }
    }

    [CraftItemID(0x113E)]
    public class PoisonTrapCraft : TrapCraft
    {
        public override TrapType TrapType
        {
            get
            {
                return TrapType.PoisonTrap;
            }
        }

        public PoisonTrapCraft(Mobile from, CraftItem craftItem, CraftSystem craftSystem, Type typeRes, BaseTool tool, int quality)
            : base(from, craftItem, craftSystem, typeRes, tool, quality)
        {
        }
    }

    [CraftItemID(0x370C)]
    public class ExplosionTrapCraft : TrapCraft
    {
        public override TrapType TrapType
        {
            get
            {
                return TrapType.ExplosionTrap;
            }
        }

        public ExplosionTrapCraft(Mobile from, CraftItem craftItem, CraftSystem craftSystem, Type typeRes, BaseTool tool, int quality)
            : base(from, craftItem, craftSystem, typeRes, tool, quality)
        {
        }
    }
}
