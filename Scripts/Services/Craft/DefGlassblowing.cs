using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Craft
{
    public class DefGlassblowing : CraftSystem
    {
        private static CraftSystem m_CraftSystem;
        private DefGlassblowing()
            : base(1, 1, 1.25)// base( 1, 2, 1.7 )
        {
        }

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefGlassblowing();

                return m_CraftSystem;
            }
        }
        public override SkillName MainSkill => SkillName.Alchemy;
        public override int GumpTitleNumber => 1044622;
        public override double GetChanceAtMin(CraftItem item)
        {
            if (item.ItemType == typeof(HollowPrism))
                return 0.5; // 50%

            if (item.ItemType == typeof(EtherealSoulbinder))
                return 0.1; // 10%

            return 0.0; // 0%
        }

        public override int CanCraft(Mobile from, ITool tool, Type itemType)
        {
            int num = 0;

            if (tool == null || tool.Deleted || tool.UsesRemaining <= 0)
                return 1044038; // You have worn out your tool!
            else if (tool is Item && !BaseTool.CheckTool((Item)tool, from))
                return 1048146; // If you have a tool equipped, you must use that tool.
            else if (!(from is PlayerMobile && ((PlayerMobile)from).Glassblowing && from.Skills[SkillName.Alchemy].Base >= 100.0))
                return 1044634; // You havent learned glassblowing.
            else if (!tool.CheckAccessible(from, ref num))
                return num; // The tool must be on your person to use.

            bool anvil, forge;

            DefBlacksmithy.CheckAnvilAndForge(from, 2, out anvil, out forge);

            if (forge)
                return 0;

            return 1044628; // You must be near a forge to blow glass.
        }

        public override void PlayCraftEffect(Mobile from)
        {
            from.PlaySound(0x2B); // bellows
            //if ( from.Body.Type == BodyType.Human && !from.Mounted )
            //	from.Animate( 9, 5, 1, true, false, 0 );
            //new InternalTimer( from ).Start();
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
                from.PlaySound(0x41); // glass breaking

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
            int index = AddCraft(typeof(Bottle), 1044050, 1023854, 52.5, 102.5, typeof(Sand), 1044625, 1, 1044627);
            SetUseAllRes(index, true);

            AddCraft(typeof(SmallFlask), 1044050, 1044610, 52.5, 102.5, typeof(Sand), 1044625, 2, 1044627);
            AddCraft(typeof(MediumFlask), 1044050, 1044611, 52.5, 102.5, typeof(Sand), 1044625, 3, 1044627);
            AddCraft(typeof(CurvedFlask), 1044050, 1044612, 55.0, 105.0, typeof(Sand), 1044625, 2, 1044627);
            AddCraft(typeof(LongFlask), 1044050, 1044613, 57.5, 107.5, typeof(Sand), 1044625, 4, 1044627);
            AddCraft(typeof(LargeFlask), 1044050, 1044623, 60.0, 110.0, typeof(Sand), 1044625, 5, 1044627);
            AddCraft(typeof(AniSmallBlueFlask), 1044050, 1044614, 60.0, 110.0, typeof(Sand), 1044625, 5, 1044627);
            AddCraft(typeof(AniLargeVioletFlask), 1044050, 1044615, 60.0, 110.0, typeof(Sand), 1044625, 5, 1044627);
            AddCraft(typeof(AniRedRibbedFlask), 1044050, 1044624, 60.0, 110.0, typeof(Sand), 1044625, 7, 1044627);
            AddCraft(typeof(EmptyVialsWRack), 1044050, 1044616, 65.0, 115.0, typeof(Sand), 1044625, 8, 1044627);
            AddCraft(typeof(FullVialsWRack), 1044050, 1044617, 65.0, 115.0, typeof(Sand), 1044625, 9, 1044627);
            AddCraft(typeof(SpinningHourglass), 1044050, 1044618, 75.0, 125.0, typeof(Sand), 1044625, 10, 1044627);

            AddCraft(typeof(HollowPrism), 1044050, 1072895, 100.0, 150.0, typeof(Sand), 1044625, 8, 1044627);

            AddCraft(typeof(GargoyleFloorMirror), 1044050, 1095314, 75.0, 125.0, typeof(Sand), 1044625, 20, 1044627);

            AddCraft(typeof(GargoyleWallMirror), 1044050, 1095324, 70.0, 120.0, typeof(Sand), 1044625, 10, 1044627);

            index = AddCraft(typeof(SoulstoneFragment), 1044050, 1071000, 70.0, 120.0, typeof(CrystalGranules), 1112329, 2, 1044253);
            AddRes(index, typeof(VoidEssence), 1112327, 2, 1044253);
            SetItemHue(index, 1150);

            AddCraft(typeof(EmptyVenomVial), 1044050, 1112215, 52.5, 102.5, typeof(Sand), 1044625, 1, 1044627);

            AddCraft(typeof(EmptyOilFlask), 1044050, 1150866, 60.0, 110.0, typeof(Sand), 1044625, 5, 1044627);

            AddCraft(typeof(WorkableGlass), 1044050, 1154170, 55.0, 105.0, typeof(Sand), 1044625, 10, 1044627);

            index = AddCraft(typeof(EtherealSoulbinder), 1044050, 1159167, 100.0, 190.0, typeof(Sand), 1044625, 20, 1044627);
            AddRes(index, typeof(EtherealSand), 1125984, 5, 1159169);

            //Glass Weapons
            AddCraft(typeof(GlassSword), 1111745, 1022316, 55.0, 105.0, typeof(Sand), 1044625, 14, 1044627);

            AddCraft(typeof(GlassStaff), 1111745, 1095368, 53.6, 103.6, typeof(Sand), 1044625, 10, 1044627);

            Repair = true;
            MarkOption = true;
        }

        // Delay to synchronize the sound with the hit on the anvil
        private class InternalTimer : Timer
        {
            private readonly Mobile m_From;
            public InternalTimer(Mobile from)
                : base(TimeSpan.FromSeconds(0.7))
            {
                m_From = from;
            }

            protected override void OnTick()
            {
                m_From.PlaySound(0x2A);
            }
        }
    }
}
