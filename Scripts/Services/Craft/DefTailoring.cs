using System;
using Server.Items;

namespace Server.Engines.Craft
{
    public enum TailorRecipe
    {
        ElvenQuiver = 501,
        QuiverOfFire = 502,
        QuiverOfIce = 503,
        QuiverOfBlight = 504,
        QuiverOfLightning = 505,

        SongWovenMantle = 550,
        SpellWovenBritches = 551,
        StitchersMittens = 552,
    }

    public class DefTailoring : CraftSystem
    {
        public override SkillName MainSkill
        {
            get
            {
                return SkillName.Tailoring;
            }
        }

        public override int GumpTitleNumber
        {
            get
            {
                return 1044005;
            }// <CENTER>TAILORING MENU</CENTER>
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefTailoring();

                return m_CraftSystem;
            }
        }

        public override CraftECA ECA
        {
            get
            {
                return CraftECA.ChanceMinusSixtyToFourtyFive;
            }
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.5; // 50%
        }

        private DefTailoring()
            : base(1, 1, 1.25)// base( 1, 1, 4.5 )
        {
        }

        public override int CanCraft(Mobile from, BaseTool tool, Type itemType)
        {
            if (tool == null || tool.Deleted || tool.UsesRemaining < 0)
                return 1044038; // You have worn out your tool!
            else if (!BaseTool.CheckAccessible(tool, from))
                return 1044263; // The tool must be on your person to use.

            return 0;
        }

        private static readonly Type[] m_TailorColorables = new Type[]
        {
            typeof(GozaMatEastDeed), typeof(GozaMatSouthDeed),
            typeof(SquareGozaMatEastDeed), typeof(SquareGozaMatSouthDeed),
            typeof(BrocadeGozaMatEastDeed), typeof(BrocadeGozaMatSouthDeed),
            typeof(BrocadeSquareGozaMatEastDeed), typeof(BrocadeSquareGozaMatSouthDeed)
        };

        public override bool RetainsColorFrom(CraftItem item, Type type)
        {
            if (type != typeof(Cloth) && type != typeof(UncutCloth) && type != typeof(AbyssalCloth))
                return false;

            type = item.ItemType;

            bool contains = false;

            for (int i = 0; !contains && i < m_TailorColorables.Length; ++i)
                contains = (m_TailorColorables[i] == type);

            return contains;
        }

        public override void PlayCraftEffect(Mobile from)
        {
            from.PlaySound(0x248);
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

        public override void InitCraftList()
        {
            int index = -1;

            if (Core.HS)
            {
                #region High Seas
                index = AddCraft(typeof(LightPowderCharge), 1044457, 1116159, 0.0, 50.0, typeof(Cloth), 1044286, 1, 1044253);
                AddRes(index, typeof(BlackPowder), 1095826, 1, 1044253);
                SetNeededExpansion(index, Expansion.HS);

                index = AddCraft(typeof(HeavyPowderCharge), 1044457, 1116160, 0.0, 50.0, typeof(Cloth), 1044286, 1, 1044253);
                AddRes(index, typeof(BlackPowder), 1095826, 4, 1044253);
                SetNeededExpansion(index, Expansion.HS);
            }
            #endregion

            #region Hats
            this.AddCraft(typeof(SkullCap), 1011375, 1025444, 0.0, 25.0, typeof(Cloth), 1044286, 2, 1044287);
            this.AddCraft(typeof(Bandana), 1011375, 1025440, 0.0, 25.0, typeof(Cloth), 1044286, 2, 1044287);
            this.AddCraft(typeof(FloppyHat), 1011375, 1025907, 6.2, 31.2, typeof(Cloth), 1044286, 11, 1044287);
            this.AddCraft(typeof(Cap), 1011375, 1025909, 6.2, 31.2, typeof(Cloth), 1044286, 11, 1044287);
            this.AddCraft(typeof(WideBrimHat), 1011375, 1025908, 6.2, 31.2, typeof(Cloth), 1044286, 12, 1044287);
            this.AddCraft(typeof(StrawHat), 1011375, 1025911, 6.2, 31.2, typeof(Cloth), 1044286, 10, 1044287);
            this.AddCraft(typeof(TallStrawHat), 1011375, 1025910, 6.7, 31.7, typeof(Cloth), 1044286, 13, 1044287);
            this.AddCraft(typeof(WizardsHat), 1011375, 1025912, 7.2, 32.2, typeof(Cloth), 1044286, 15, 1044287);
            this.AddCraft(typeof(Bonnet), 1011375, 1025913, 6.2, 31.2, typeof(Cloth), 1044286, 11, 1044287);
            this.AddCraft(typeof(FeatheredHat), 1011375, 1025914, 6.2, 31.2, typeof(Cloth), 1044286, 12, 1044287);
            this.AddCraft(typeof(TricorneHat), 1011375, 1025915, 6.2, 31.2, typeof(Cloth), 1044286, 12, 1044287);
            this.AddCraft(typeof(JesterHat), 1011375, 1025916, 7.2, 32.2, typeof(Cloth), 1044286, 15, 1044287);

            if (Core.AOS)
                this.AddCraft(typeof(FlowerGarland), 1011375, 1028965, 10.0, 35.0, typeof(Cloth), 1044286, 5, 1044287);

            if (Core.SE)
            {
                index = this.AddCraft(typeof(ClothNinjaHood), 1011375, 1030202, 80.0, 105.0, typeof(Cloth), 1044286, 13, 1044287);
                this.SetNeededExpansion(index, Expansion.SE);

                index = this.AddCraft(typeof(Kasa), 1011375, 1030211, 60.0, 85.0, typeof(Cloth), 1044286, 12, 1044287);	
                this.SetNeededExpansion(index, Expansion.SE);
            }
            #endregion

            #region Shirts/Pants
            this.AddCraft(typeof(Doublet), 1111747, 1028059, 0, 25.0, typeof(Cloth), 1044286, 8, 1044287);
            this.AddCraft(typeof(Shirt), 1111747, 1025399, 20.7, 45.7, typeof(Cloth), 1044286, 8, 1044287);
            this.AddCraft(typeof(FancyShirt), 1111747, 1027933, 24.8, 49.8, typeof(Cloth), 1044286, 8, 1044287);
            this.AddCraft(typeof(Tunic), 1111747, 1028097, 00.0, 25.0, typeof(Cloth), 1044286, 12, 1044287);
            this.AddCraft(typeof(Surcoat), 1111747, 1028189, 8.2, 33.2, typeof(Cloth), 1044286, 14, 1044287);
            this.AddCraft(typeof(PlainDress), 1111747, 1027937, 12.4, 37.4, typeof(Cloth), 1044286, 10, 1044287);
            this.AddCraft(typeof(FancyDress), 1111747, 1027935, 33.1, 58.1, typeof(Cloth), 1044286, 12, 1044287);
            this.AddCraft(typeof(Cloak), 1111747, 1025397, 41.4, 66.4, typeof(Cloth), 1044286, 14, 1044287);
            this.AddCraft(typeof(Robe), 1111747, 1027939, 53.9, 78.9, typeof(Cloth), 1044286, 16, 1044287);
            this.AddCraft(typeof(JesterSuit), 1111747, 1028095, 8.2, 33.2, typeof(Cloth), 1044286, 24, 1044287);

            if (Core.AOS)
            {
                this.AddCraft(typeof(FurCape), 1111747, 1028969, 35.0, 60.0, typeof(Cloth), 1044286, 13, 1044287);
                this.AddCraft(typeof(GildedDress), 1111747, 1028973, 37.5, 62.5, typeof(Cloth), 1044286, 16, 1044287);
                this.AddCraft(typeof(FormalShirt), 1111747, 1028975, 26.0, 51.0, typeof(Cloth), 1044286, 16, 1044287);
            }

            if (Core.SE)
            {
                index = this.AddCraft(typeof(ClothNinjaJacket), 1111747, 1030207, 75.0, 100.0, typeof(Cloth), 1044286, 12, 1044287);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(Kamishimo), 1111747, 1030212, 75.0, 100.0, typeof(Cloth), 1044286, 15, 1044287);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(HakamaShita), 1111747, 1030215, 40.0, 65.0, typeof(Cloth), 1044286, 14, 1044287);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(MaleKimono), 1111747, 1030189, 50.0, 75.0, typeof(Cloth), 1044286, 16, 1044287);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(FemaleKimono), 1111747, 1030190, 50.0, 75.0, typeof(Cloth), 1044286, 16, 1044287);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(JinBaori), 1111747, 1030220, 30.0, 55.0, typeof(Cloth), 1044286, 12, 1044287);
                this.SetNeededExpansion(index, Expansion.SE);
            }

            #region Mondain's Legacy
            if (Core.ML)
            {
                index = this.AddCraft(typeof(ElvenShirt), 1111747, 1032661, 80.0, 105.0, typeof(Cloth), 1044286, 10, 1044287);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(ElvenDarkShirt), 1111747, 1032662, 80.0, 105.0, typeof(Cloth), 1044286, 10, 1044287);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(MaleElvenRobe), 1111747, 1032659, 80.0, 105.0, typeof(Cloth), 1044286, 30, 1044287);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(FemaleElvenRobe), 1111747, 1032660, 80.0, 105.0, typeof(Cloth), 1044286, 30, 1044287);
                this.SetNeededExpansion(index, Expansion.ML);
            }
            #endregion

            this.AddCraft(typeof(ShortPants), 1111747, 1025422, 24.8, 49.8, typeof(Cloth), 1044286, 6, 1044287);
            this.AddCraft(typeof(LongPants), 1111747, 1025433, 24.8, 49.8, typeof(Cloth), 1044286, 8, 1044287);
            this.AddCraft(typeof(Kilt), 1111747, 1025431, 20.7, 45.7, typeof(Cloth), 1044286, 8, 1044287);
            this.AddCraft(typeof(Skirt), 1111747, 1025398, 29.0, 54.0, typeof(Cloth), 1044286, 10, 1044287);

            if (Core.AOS)
                this.AddCraft(typeof(FurSarong), 1111747, 1028971, 35.0, 60.0, typeof(Cloth), 1044286, 12, 1044287);

            if (Core.SE)
            {
                index = this.AddCraft(typeof(Hakama), 1111747, 1030213, 50.0, 75.0, typeof(Cloth), 1044286, 16, 1044287);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(TattsukeHakama), 1111747, 1030214, 50.0, 75.0, typeof(Cloth), 1044286, 16, 1044287);
                this.SetNeededExpansion(index, Expansion.SE);
            }

            #region Mondain's Legacy
            if (Core.ML)
            {
                index = this.AddCraft(typeof(ElvenPants), 1111747, 1032665, 80.0, 105.0, typeof(Cloth), 1044286, 12, 1044287);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(WoodlandBelt), 1111747, 1032639, 80.0, 105.0, typeof(Cloth), 1044286, 10, 1044287);
                this.SetNeededExpansion(index, Expansion.ML);
            }
            #endregion

            if (Core.SA)
            {
                index = AddCraft(typeof(RobeofRite), 1111747, 1153510, 101.5, 120.0, typeof(Leather), 1044462, 6, 1044253);
                AddRes(index, typeof(FireRuby), 1032695, 1, 1044253);
                AddRes(index, typeof(GoldDust), 1098337, 5, 1044253);
                AddRes(index, typeof(AbyssalCloth), 1113350, 6, 1044253);
                this.ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.SA);
            }

            #endregion

            #region Misc
            this.AddCraft(typeof(BodySash), 1015283, 1025441, 4.1, 29.1, typeof(Cloth), 1044286, 4, 1044287);
            this.AddCraft(typeof(HalfApron), 1015283, 1025435, 20.7, 45.7, typeof(Cloth), 1044286, 6, 1044287);
            this.AddCraft(typeof(FullApron), 1015283, 1025437, 29.0, 54.0, typeof(Cloth), 1044286, 10, 1044287);

            if (Core.SE)
            {
                index = this.AddCraft(typeof(Obi), 1015283, 1030219, 20.0, 45.0, typeof(Cloth), 1044286, 6, 1044287);
                this.SetNeededExpansion(index, Expansion.SE);
            }

            if (Core.ML)
            {
                index = this.AddCraft(typeof(ElvenQuiver), 1015283, 1032657, 65.0, 115.0, typeof(Leather), 1044462, 28, 1044463);
                this.AddRecipe(index, (int)TailorRecipe.ElvenQuiver);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(QuiverOfFire), 1015283, 1073109, 65.0, 115.0, typeof(Leather), 1044462, 28, 1044463);
                this.AddRes(index, typeof(FireRuby), 1032695, 15, 1042081);
                this.AddRecipe(index, (int)TailorRecipe.QuiverOfFire);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(QuiverOfIce), 1015283, 1073110, 65.0, 115.0, typeof(Leather), 1044462, 28, 1044463);
                this.AddRes(index, typeof(WhitePearl), 1032694, 15, 1042081);
                this.AddRecipe(index, (int)TailorRecipe.QuiverOfIce);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(QuiverOfBlight), 1015283, 1073111, 65.0, 115.0, typeof(Leather), 1044462, 28, 1044463);
                this.AddRes(index, typeof(Blight), 1032675, 10, 1042081);
                this.AddRecipe(index, (int)TailorRecipe.QuiverOfBlight);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(QuiverOfLightning), 1015283, 1073112, 65.0, 115.0, typeof(Leather), 1044462, 28, 1044463);
                this.AddRes(index, typeof(Corruption), 1032676, 10, 1042081);
                this.AddRecipe(index, (int)TailorRecipe.QuiverOfLightning);
                this.SetNeededExpansion(index, Expansion.ML);

                #region Mondain's Legacy
                index = this.AddCraft(typeof(LeatherContainerEngraver), 1015283, 1072152, 75.0, 100.0, typeof(Bone), 1049064, 1, 1049063);
                this.AddRes(index, typeof(Leather), 1044462, 6, 1044463);
                this.AddRes(index, typeof(SpoolOfThread), 1073462, 2, 1073463);
                this.AddRes(index, typeof(Dyes), 1024009, 6, 1044253);
                this.SetNeededExpansion(index, Expansion.ML);
                #endregion
            }

            this.AddCraft(typeof(OilCloth), 1015283, 1041498, 74.6, 99.6, typeof(Cloth), 1044286, 1, 1044287);

            if (Core.SE)
            {
                index = this.AddCraft(typeof(GozaMatEastDeed), 1015283, 1030404, 55.0, 80.0, typeof(Cloth), 1044286, 25, 1044287);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(GozaMatSouthDeed), 1015283, 1030405, 55.0, 80.0, typeof(Cloth), 1044286, 25, 1044287);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(SquareGozaMatEastDeed), 1015283, 1030407, 55.0, 80.0, typeof(Cloth), 1044286, 25, 1044287);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(SquareGozaMatSouthDeed), 1015283, 1030406, 55.0, 80.0, typeof(Cloth), 1044286, 25, 1044287);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(BrocadeGozaMatEastDeed), 1015283, 1030408, 55.0, 80.0, typeof(Cloth), 1044286, 25, 1044287);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(BrocadeGozaMatSouthDeed), 1015283, 1030409, 55.0, 80.0, typeof(Cloth), 1044286, 25, 1044287);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(BrocadeSquareGozaMatEastDeed), 1015283, 1030411, 55.0, 80.0, typeof(Cloth), 1044286, 25, 1044287);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(BrocadeSquareGozaMatSouthDeed), 1015283, 1030410, 55.0, 80.0, typeof(Cloth), 1044286, 25, 1044287);
                this.SetNeededExpansion(index, Expansion.SE);
            }
            #region SA
            if (Core.SA)
            {
                this.AddCraft(typeof(GargishSash), 1015283, 1115388, 4.1, 29.1, typeof(Cloth), 1044286, 4, 1044287);
                this.AddCraft(typeof(GargishApron), 1015283, 1099568, 20.7, 45.7, typeof(Cloth), 1044286, 6, 1044287);
            }
            
            #endregion
            #endregion

            #region Footwear

            #region Mondain's Legacy
            if (Core.ML)
            {
                index = this.AddCraft(typeof(ElvenBoots), 1015283, 1072902, 80.0, 105.0, typeof(Leather), 1044462, 15, 1044463);
                this.SetNeededExpansion(index, Expansion.ML);
            }
            #endregion

            if (Core.AOS)
                this.AddCraft(typeof(FurBoots), 1015288, 1028967, 50.0, 75.0, typeof(Cloth), 1044286, 12, 1044287);

            if (Core.SE)
            {
                index = this.AddCraft(typeof(NinjaTabi), 1015288, 1030210, 70.0, 95.0, typeof(Cloth), 1044286, 10, 1044287);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(SamuraiTabi), 1015288, 1030209, 20.0, 45.0, typeof(Cloth), 1044286, 6, 1044287);
                this.SetNeededExpansion(index, Expansion.SE);
            }

            this.AddCraft(typeof(Sandals), 1015288, 1025901, 12.4, 37.4, typeof(Leather), 1044462, 4, 1044463);
            this.AddCraft(typeof(Shoes), 1015288, 1025904, 16.5, 41.5, typeof(Leather), 1044462, 6, 1044463);
            this.AddCraft(typeof(Boots), 1015288, 1025899, 33.1, 58.1, typeof(Leather), 1044462, 8, 1044463);
            this.AddCraft(typeof(ThighBoots), 1015288, 1025906, 41.4, 66.4, typeof(Leather), 1044462, 10, 1044463);
            #region SA
            if (Core.SA)
            {
                this.AddCraft(typeof(LeatherTalons), 1015288, 1095728, 40.4, 65.4, typeof(Leather), 1044462, 6, 1044453);
            }
            #endregion
            #endregion

            #region Leather Armor

            #region Mondain's Legacy
            if (Core.ML)
            {
                index = this.AddCraft(typeof(SpellWovenBritches), 1015293, 1072929, 92.5, 117.5, typeof(Leather), 1044462, 15, 1044463);
                this.AddRes(index, typeof(EyeOfTheTravesty), 1032685, 1, 1044253);
                this.AddRes(index, typeof(Putrefication), 1032678, 10, 1044253);
                this.AddRes(index, typeof(Scourge), 1032677, 10, 1044253);
                this.AddRecipe(index, (int)TailorRecipe.SpellWovenBritches);
                this.ForceNonExceptional(index);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(SongWovenMantle), 1015293, 1072931, 92.5, 117.5, typeof(Leather), 1044462, 15, 1044463);
                this.AddRes(index, typeof(EyeOfTheTravesty), 1032685, 1, 1044253);
                this.AddRes(index, typeof(Blight), 1032675, 10, 1044253);
                this.AddRes(index, typeof(Muculent), 1032680, 10, 1044253);
                this.AddRecipe(index, (int)TailorRecipe.SongWovenMantle);
                this.ForceNonExceptional(index);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(StitchersMittens), 1015293, 1072932, 92.5, 117.5, typeof(Leather), 1044462, 15, 1044463);
                this.AddRes(index, typeof(CapturedEssence), 1032686, 1, 1044253);
                this.AddRes(index, typeof(Corruption), 1032676, 10, 1044253);
                this.AddRes(index, typeof(Taint), 1032679, 10, 1044253);
                this.AddRecipe(index, (int)TailorRecipe.StitchersMittens);
                this.ForceNonExceptional(index);
                this.SetNeededExpansion(index, Expansion.ML);
            }
            #endregion

            this.AddCraft(typeof(LeatherGorget), 1015293, 1025063, 53.9, 78.9, typeof(Leather), 1044462, 4, 1044463);
            this.AddCraft(typeof(LeatherCap), 1015293, 1027609, 6.2, 31.2, typeof(Leather), 1044462, 2, 1044463);
            this.AddCraft(typeof(LeatherGloves), 1015293, 1025062, 51.8, 76.8, typeof(Leather), 1044462, 3, 1044463);
            this.AddCraft(typeof(LeatherArms), 1015293, 1025061, 53.9, 78.9, typeof(Leather), 1044462, 4, 1044463);
            this.AddCraft(typeof(LeatherLegs), 1015293, 1025067, 66.3, 91.3, typeof(Leather), 1044462, 10, 1044463);
            this.AddCraft(typeof(LeatherChest), 1015293, 1025068, 70.5, 95.5, typeof(Leather), 1044462, 12, 1044463);

            if (Core.SE)
            {
                index = this.AddCraft(typeof(LeatherJingasa), 1015293, 1030177, 45.0, 70.0, typeof(Leather), 1044462, 4, 1044463);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(LeatherMempo), 1015293, 1030181, 80.0, 105.0, typeof(Leather), 1044462, 8, 1044463);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(LeatherDo), 1015293, 1030182, 75.0, 100.0, typeof(Leather), 1044462, 12, 1044463);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(LeatherHiroSode), 1015293, 1030185, 55.0, 80.0, typeof(Leather), 1044462, 5, 1044463);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(LeatherSuneate), 1015293, 1030193, 68.0, 93.0, typeof(Leather), 1044462, 12, 1044463);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(LeatherHaidate), 1015293, 1030197, 68.0, 93.0, typeof(Leather), 1044462, 12, 1044463);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(LeatherNinjaPants), 1015293, 1030204, 80.0, 105.0, typeof(Leather), 1044462, 13, 1044463);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(LeatherNinjaJacket), 1015293, 1030206, 85.0, 110.0, typeof(Leather), 1044462, 13, 1044463);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(LeatherNinjaBelt), 1015293, 1030203, 50.0, 75.0, typeof(Leather), 1044462, 5, 1044463);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(LeatherNinjaMitts), 1015293, 1030205, 65.0, 90.0, typeof(Leather), 1044462, 12, 1044463);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(LeatherNinjaHood), 1015293, 1030201, 90.0, 115.0, typeof(Leather), 1044462, 14, 1044463);
                this.SetNeededExpansion(index, Expansion.SE);
            }
            #region SA
            if (Core.SA)
            {
                this.AddCraft(typeof(GargishLeatherArms), 1015293, 1095327, 53.9, 78.9, typeof(Leather), 1044462, 8, 1044463);
                this.AddCraft(typeof(GargishLeatherChest), 1015293, 1095329, 70.5, 95.5, typeof(Leather), 1044462, 8, 1044463);
                this.AddCraft(typeof(GargishLeatherKilt), 1015293, 1095331, 58.0, 83.0, typeof(Leather), 1044462, 6, 1044463);
                this.AddCraft(typeof(GargishLeatherLegs), 1015293, 1095333, 66.3, 91.3, typeof(Leather), 1044462, 10, 1044463);
                this.AddCraft(typeof(GargishLeatherWingArmor), 1015293, 1096662, 65.0, 90.0, typeof(Leather), 1044462, 12, 1044463);
            }
            #endregion
            #region Mondain's Legacy
            if (Core.ML)
            {
                index = this.AddCraft(typeof(LeafChest), 1015293, 1032667, 75.0, 100.0, typeof(Leather), 1044462, 15, 1044463);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(LeafArms), 1015293, 1032670, 60.0, 85.0, typeof(Leather), 1044462, 12, 1044463);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(LeafGloves), 1015293, 1032668, 60.0, 85.0, typeof(Leather), 1044462, 10, 1044463);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(LeafLegs), 1015293, 1032671, 75.0, 100.0, typeof(Leather), 1044462, 15, 1044463);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(LeafGorget), 1015293, 1032669, 65.0, 90.0, typeof(Leather), 1044462, 12, 1044463);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(LeafTonlet), 1015293, 1032672, 70.0, 95.0, typeof(Leather), 1044462, 12, 1044463);
                this.SetNeededExpansion(index, Expansion.ML);
            }
            #endregion

            #endregion

            #region Cloth Armor
            if (Core.SA)
            {
                index = this.AddCraft(typeof(GargishClothArmsArmor), 1111748, 1021027, 87.1, 137.1, typeof(Cloth), 1044286, 8, 1044463);
                this.SetNeededExpansion(index, Expansion.SA);

                index = this.AddCraft(typeof(GargishClothChestArmor), 1111748, 1021029, 94.0, 144.0, typeof(Cloth), 1044286, 8, 1044463);
                this.SetNeededExpansion(index, Expansion.SA);

                index = this.AddCraft(typeof(GargishClothKiltArmor), 1111748, 1021031, 82.9, 132.9, typeof(Cloth), 1044286, 6, 1044463);
                this.SetNeededExpansion(index, Expansion.SA);

                index = this.AddCraft(typeof(GargishClothLegsArmor), 1111748, 1021033, 91.2, 141.2, typeof(Cloth), 1044286, 10, 1044463);
                this.SetNeededExpansion(index, Expansion.SA);

                index = this.AddCraft(typeof(GargishClothWingArmor), 1111748, 1115393, 65.0, 90.0, typeof(Cloth), 1044286, 12, 1044463);
                this.SetNeededExpansion(index, Expansion.SA);
            }
            #endregion

            #region Studded Armor
            this.AddCraft(typeof(StuddedGorget), 1015300, 1025078, 78.8, 103.8, typeof(Leather), 1044462, 6, 1044463);
            this.AddCraft(typeof(StuddedGloves), 1015300, 1025077, 82.9, 107.9, typeof(Leather), 1044462, 8, 1044463);
            this.AddCraft(typeof(StuddedArms), 1015300, 1025076, 87.1, 112.1, typeof(Leather), 1044462, 10, 1044463);
            this.AddCraft(typeof(StuddedLegs), 1015300, 1025082, 91.2, 116.2, typeof(Leather), 1044462, 12, 1044463);
            this.AddCraft(typeof(StuddedChest), 1015300, 1025083, 94.0, 119.0, typeof(Leather), 1044462, 14, 1044463);

            if (Core.SE)
            {
                index = this.AddCraft(typeof(StuddedMempo), 1015300, 1030216, 80.0, 105.0, typeof(Leather), 1044462, 8, 1044463);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(StuddedDo), 1015300, 1030183, 95.0, 120.0, typeof(Leather), 1044462, 14, 1044463);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(StuddedHiroSode), 1015300, 1030186, 85.0, 110.0, typeof(Leather), 1044462, 8, 1044463);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(StuddedSuneate), 1015300, 1030194, 92.0, 117.0, typeof(Leather), 1044462, 14, 1044463);
                this.SetNeededExpansion(index, Expansion.SE);
                index = this.AddCraft(typeof(StuddedHaidate), 1015300, 1030198, 92.0, 117.0, typeof(Leather), 1044462, 14, 1044463);
                this.SetNeededExpansion(index, Expansion.SE);
            }

            #region Mondain's Legacy
            if (Core.ML)
            {
                index = this.AddCraft(typeof(HideChest), 1015300, 1032651, 85.0, 110.0, typeof(Leather), 1044462, 15, 1044463);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(HidePauldrons), 1015300, 1032654, 75.0, 100.0, typeof(Leather), 1044462, 12, 1044463);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(HideGloves), 1015300, 1032652, 75.0, 100.0, typeof(Leather), 1044462, 10, 1044463);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(HidePants), 1015300, 1032655, 92.0, 117.0, typeof(Leather), 1044462, 15, 1044463);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(HideGorget), 1015300, 1032653, 90.0, 115.0, typeof(Leather), 1044462, 12, 1044463);
                this.SetNeededExpansion(index, Expansion.ML);
            }
            #endregion

            #endregion

            #region Female Armor
            this.AddCraft(typeof(LeatherShorts), 1015306, 1027168, 62.2, 87.2, typeof(Leather), 1044462, 8, 1044463);
            this.AddCraft(typeof(LeatherSkirt), 1015306, 1027176, 58.0, 83.0, typeof(Leather), 1044462, 6, 1044463);
            this.AddCraft(typeof(LeatherBustierArms), 1015306, 1027178, 58.0, 83.0, typeof(Leather), 1044462, 6, 1044463);
            this.AddCraft(typeof(StuddedBustierArms), 1015306, 1027180, 82.9, 107.9, typeof(Leather), 1044462, 8, 1044463);
            this.AddCraft(typeof(FemaleLeatherChest), 1015306, 1027174, 62.2, 87.2, typeof(Leather), 1044462, 8, 1044463);
            this.AddCraft(typeof(FemaleStuddedChest), 1015306, 1027170, 87.1, 112.1, typeof(Leather), 1044462, 10, 1044463);
            #endregion

            #region Bone Armor
            index = this.AddCraft(typeof(BoneHelm), 1049149, 1025206, 85.0, 110.0, typeof(Leather), 1044462, 4, 1044463);
            this.AddRes(index, typeof(Bone), 1049064, 2, 1049063);
			
            index = this.AddCraft(typeof(BoneGloves), 1049149, 1025205, 89.0, 114.0, typeof(Leather), 1044462, 6, 1044463);
            this.AddRes(index, typeof(Bone), 1049064, 2, 1049063);

            index = this.AddCraft(typeof(BoneArms), 1049149, 1025203, 92.0, 117.0, typeof(Leather), 1044462, 8, 1044463);
            this.AddRes(index, typeof(Bone), 1049064, 4, 1049063);

            index = this.AddCraft(typeof(BoneLegs), 1049149, 1025202, 95.0, 120.0, typeof(Leather), 1044462, 10, 1044463);
            this.AddRes(index, typeof(Bone), 1049064, 6, 1049063);
		
            index = this.AddCraft(typeof(BoneChest), 1049149, 1025199, 96.0, 121.0, typeof(Leather), 1044462, 12, 1044463);
            this.AddRes(index, typeof(Bone), 1049064, 10, 1049063);
            #endregion

            // Set the overridable material
            this.SetSubRes(typeof(Leather), 1049150);

            // Add every material you want the player to be able to choose from
            // This will override the overridable material
            this.AddSubRes(typeof(Leather), 1049150, 00.0, 1044462, 1049311);
            this.AddSubRes(typeof(SpinedLeather), 1049151, 65.0, 1044462, 1049311);
            this.AddSubRes(typeof(HornedLeather), 1049152, 80.0, 1044462, 1049311);
            this.AddSubRes(typeof(BarbedLeather), 1049153, 99.0, 1044462, 1049311);

            this.MarkOption = true;
            this.Repair = Core.AOS;
            this.CanEnhance = Core.ML;
			this.CanAlter = Core.SA;
        }
    }
}