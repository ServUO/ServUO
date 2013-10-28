using System; 
using Server.Items; 
using Server.Mobiles; 

namespace Server.Engines.Craft 
{ 
    public class DefMasonry : CraftSystem 
    { 
        public override SkillName MainSkill 
        { 
            get
            {
                return SkillName.Carpentry;
            }
        }

        public override int GumpTitleNumber 
        { 
            get
            {
                return 1044500;
            }// <CENTER>MASONRY MENU</CENTER> 
        }

        private static CraftSystem m_CraftSystem; 

        public static CraftSystem CraftSystem 
        { 
            get 
            { 
                if (m_CraftSystem == null) 
                    m_CraftSystem = new DefMasonry(); 

                return m_CraftSystem; 
            }
        }

        public override double GetChanceAtMin(CraftItem item) 
        { 
            return 0.0; // 0% 
        }

        private DefMasonry()
            : base(1, 1, 1.25)// base( 1, 2, 1.7 ) 
        { 
        }

        public override bool RetainsColorFrom(CraftItem item, Type type)
        {
            return true;
        }

        public override int CanCraft(Mobile from, BaseTool tool, Type itemType)
        {
            if (tool == null || tool.Deleted || tool.UsesRemaining < 0)
                return 1044038; // You have worn out your tool!
            else if (!BaseTool.CheckTool(tool, from))
                return 1048146; // If you have a tool equipped, you must use that tool.
            else if (!(from is PlayerMobile && ((PlayerMobile)from).Masonry && from.Skills[SkillName.Carpentry].Base >= 100.0))
                return 1044633; // You havent learned stonecraft.
            else if (!BaseTool.CheckAccessible(tool, from))
                return 1044263; // The tool must be on your person to use.

            return 0;
        }

        public override void PlayCraftEffect(Mobile from) 
        { 
            // no effects
            //if ( from.Body.Type == BodyType.Human && !from.Mounted ) 
            //	from.Animate( 9, 5, 1, true, false, 0 ); 
            //new InternalTimer( from ).Start(); 
        }

        // Delay to synchronize the sound with the hit on the anvil 
        private class InternalTimer : Timer 
        { 
            private readonly Mobile m_From; 

            public InternalTimer(Mobile from)
                : base(TimeSpan.FromSeconds(0.7))
            { 
                this.m_From = from; 
            }

            protected override void OnTick() 
            { 
                this.m_From.PlaySound(0x23D); 
            }
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
            // Decorations
            this.AddCraft(typeof(Vase), 1044501, 1022888, 52.5, 102.5, typeof(Granite), 1044514, 1, 1044513);
            this.AddCraft(typeof(LargeVase), 1044501, 1022887, 52.5, 102.5, typeof(Granite), 1044514, 3, 1044513);
			
            if (Core.SE)
            {
                int index = this.AddCraft(typeof(SmallUrn), 1044501, 1029244, 82.0, 132.0, typeof(Granite), 1044514, 3, 1044513);
                this.SetNeededExpansion(index, Expansion.SE);

                index = this.AddCraft(typeof(SmallTowerSculpture), 1044501, 1029242, 82.0, 132.0, typeof(Granite), 1044514, 3, 1044513);
                this.SetNeededExpansion(index, Expansion.SE);
            }

            if (Core.SA)
            {
                int index = this.AddCraft(typeof(GargoylePainting), 1044501, 1095317, 80.0, 130.0, typeof(Granite), 1044514, 3, 1044513);
                this.SetNeededExpansion(index, Expansion.SA);
 
                index = this.AddCraft(typeof(GargishSculpture), 1044501, 1095319, 82.0, 132.0, typeof(Granite), 1044514, 3, 1044513);
                this.SetNeededExpansion(index, Expansion.SA);
 
                index = this.AddCraft(typeof(GargoyleVase), 1044501, 1095322, 76.0, 126.0, typeof(Granite), 1044514, 3, 1044513);
                this.SetNeededExpansion(index, Expansion.SA);
 
            }            

            // Furniture
            this.AddCraft(typeof(StoneChair), 1044502, 1024635, 55.0, 105.0, typeof(Granite), 1044514, 4, 1044513);
            this.AddCraft(typeof(MediumStoneTableEastDeed), 1044502, 1044508, 65.0, 115.0, typeof(Granite), 1044514, 6, 1044513);
            this.AddCraft(typeof(MediumStoneTableSouthDeed), 1044502, 1044509, 65.0, 115.0, typeof(Granite), 1044514, 6, 1044513);
            this.AddCraft(typeof(LargeStoneTableEastDeed), 1044502, 1044511, 75.0, 125.0, typeof(Granite), 1044514, 9, 1044513);
            this.AddCraft(typeof(LargeStoneTableSouthDeed), 1044502, 1044512, 75.0, 125.0, typeof(Granite), 1044514, 9, 1044513);

            if (Core.SA)
            {
                int index = this.AddCraft(typeof(LargeGargoyleBedSouthDeed), 1044502, 1111761, 76.0, 126.0, typeof(Granite), 1044514, 3, 1044513);
                this.AddSkill(index, SkillName.Tailoring, 70.0, 75.0);
                this.AddRes(index, typeof(Cloth), 1044286, 100, 1044287);
                this.SetNeededExpansion(index, Expansion.SA);
 
                index = this.AddCraft(typeof(LargeGargoyleBedEastDeed), 1044502, 1111762, 76.0, 126.0, typeof(Granite), 1044514, 3, 1044513);
                this.AddSkill(index, SkillName.Tailoring, 70.0, 75.0);
                this.AddRes(index, typeof(Cloth), 1044286, 100, 1044287);
                this.SetNeededExpansion(index, Expansion.SA);
 
                index = this.AddCraft(typeof(GargishCotEastDeed), 1044502, 1111921, 76.0, 126.0, typeof(Granite), 1044514, 3, 1044513);
                this.AddSkill(index, SkillName.Tailoring, 70.0, 75.0);
                this.AddRes(index, typeof(Cloth), 1044286, 100, 1044287);
                this.SetNeededExpansion(index, Expansion.SA);
 
                index = this.AddCraft(typeof(GargishCotSouthDeed), 1044502, 1111920, 76.0, 126.0, typeof(Granite), 1044514, 3, 1044513);
                this.AddSkill(index, SkillName.Tailoring, 70.0, 75.0);
                this.AddRes(index, typeof(Cloth), 1044286, 100, 1044287);
                this.SetNeededExpansion(index, Expansion.SA);
            }            

            // Statues
            this.AddCraft(typeof(StatueSouth), 1044503, 1044505, 60.0, 120.0, typeof(Granite), 1044514, 3, 1044513);
            this.AddCraft(typeof(StatueNorth), 1044503, 1044506, 60.0, 120.0, typeof(Granite), 1044514, 3, 1044513);
            this.AddCraft(typeof(StatueEast), 1044503, 1044507, 60.0, 120.0, typeof(Granite), 1044514, 3, 1044513);
            this.AddCraft(typeof(StatuePegasus), 1044503, 1044510, 70.0, 130.0, typeof(Granite), 1044514, 4, 1044513);

            #region Mondain's Legacy
            // Misc Addons
            if (Core.ML)
            {
                int index = this.AddCraft(typeof(StoneAnvilSouthDeed), 1044290, 1072876, 78.0, 128.0, typeof(Granite), 1044514, 20, 1044513);
                this.AddRecipe(index, (int)CarpRecipes.StoneAnvilSouth);
                this.SetNeededExpansion(index, Expansion.ML);
				
                index = this.AddCraft(typeof(StoneAnvilEastDeed), 1044290, 1073392, 78.0, 128.0, typeof(Granite), 1044514, 20, 1044513);
                this.AddRecipe(index, (int)CarpRecipes.StoneAnvilEast);
                this.SetNeededExpansion(index, Expansion.ML);
            }
            #endregion

            //Armor
            #region Gargish Armor
            if (Core.SA)
            {
                int index = this.AddCraft(typeof(GargishStoneArms), 1111705, 1020643, 52.0, 102.0, typeof(Granite), 1044514, 8, 1044513);
                this.SetNeededExpansion(index, Expansion.SA);
 
                index = this.AddCraft(typeof(GargishStoneChest), 1111705, 1020645, 55.0, 105.0, typeof(Granite), 1044514, 12, 1044513);
                this.SetNeededExpansion(index, Expansion.SA);
 
                index = this.AddCraft(typeof(GargishStoneKilt), 1111705, 1020647, 50.0, 100.0, typeof(Granite), 1044514, 6, 1044513);
                this.SetNeededExpansion(index, Expansion.SA);
 
                index = this.AddCraft(typeof(GargishStoneLegs), 1111705, 1020649, 53.5, 103.5, typeof(Granite), 1044514, 10, 1044513);
                this.SetNeededExpansion(index, Expansion.SA);
 
                index = this.AddCraft(typeof(LargeStoneShield), 1111705, 1095773, 55.0, 105.0, typeof(Granite), 1044514, 16, 1044513);
                this.SetNeededExpansion(index, Expansion.SA);
 
                index = this.AddCraft(typeof(StoneWarSword), 1111705, 1112753, 55.0, 105.0, typeof(Granite), 1044514, 18, 1044513);
                this.SetNeededExpansion(index, Expansion.SA);
            }
            #endregion

            this.SetSubRes(typeof(Granite), 1044525);

            this.AddSubRes(typeof(Granite), 1044525, 00.0, 1044514, 1044526);
            this.AddSubRes(typeof(DullCopperGranite), 1044023, 65.0, 1044514, 1044527);
            this.AddSubRes(typeof(ShadowIronGranite), 1044024, 70.0, 1044514, 1044527);
            this.AddSubRes(typeof(CopperGranite), 1044025, 75.0, 1044514, 1044527);
            this.AddSubRes(typeof(BronzeGranite), 1044026, 80.0, 1044514, 1044527);
            this.AddSubRes(typeof(GoldGranite), 1044027, 85.0, 1044514, 1044527);
            this.AddSubRes(typeof(AgapiteGranite), 1044028, 90.0, 1044514, 1044527);
            this.AddSubRes(typeof(VeriteGranite), 1044029, 95.0, 1044514, 1044527);
            this.AddSubRes(typeof(ValoriteGranite), 1044030, 99.0, 1044514, 1044527);
        }
    }
}