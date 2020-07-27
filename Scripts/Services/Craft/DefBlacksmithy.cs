#region References
using Server.Items;
using System;
#endregion

namespace Server.Engines.Craft
{

    #region Mondain's Legacy
    public enum SmithRecipes
    {
        // magical
        TrueSpellblade = 300,
        IcySpellblade = 301,
        FierySpellblade = 302,
        SpellbladeOfDefense = 303,
        TrueAssassinSpike = 304,
        ChargedAssassinSpike = 305,
        MagekillerAssassinSpike = 306,
        WoundingAssassinSpike = 307,
        TrueLeafblade = 308,
        Luckblade = 309,
        MagekillerLeafblade = 310,
        LeafbladeOfEase = 311,
        KnightsWarCleaver = 312,
        ButchersWarCleaver = 313,
        SerratedWarCleaver = 314,
        TrueWarCleaver = 315,
        AdventurersMachete = 316,
        OrcishMachete = 317,
        MacheteOfDefense = 318,
        DiseasedMachete = 319,
        Runesabre = 320,
        MagesRuneBlade = 321,
        RuneBladeOfKnowledge = 322,
        CorruptedRuneBlade = 323,
        TrueRadiantScimitar = 324,
        DarkglowScimitar = 325,
        IcyScimitar = 326,
        TwinklingScimitar = 327,
        GuardianAxe = 328,
        SingingAxe = 329,
        ThunderingAxe = 330,
        HeavyOrnateAxe = 331,
        RubyMace = 332, //good
        EmeraldMace = 333, //good
        SapphireMace = 334, //good
        SilverEtchedMace = 335, //good
        BoneMachete = 336,

        // arties
        RuneCarvingKnife = 350,
        ColdForgedBlade = 351,
        OverseerSunderedBlade = 352,
        LuminousRuneBlade = 353,
        ShardTrasher = 354, //good

        // doom 
        BritchesOfWarding = 355,
        GlovesOfFeudalGrip = 356
    }
    #endregion

    public class DefBlacksmithy : CraftSystem
    {
        public override SkillName MainSkill => SkillName.Blacksmith;

        public override int GumpTitleNumber => 1044002;

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem => m_CraftSystem ?? (m_CraftSystem = new DefBlacksmithy());

        public override CraftECA ECA => CraftECA.ChanceMinusSixtyToFourtyFive;

        public override double GetChanceAtMin(CraftItem item)
        {
            if (item.NameNumber == 1157349 || item.NameNumber == 1157345) // Gloves Of FeudalGrip and Britches Of Warding
                return 0.05; // 5%

            return 0.0; // 0%
        }

        private DefBlacksmithy()
            : base(1, 1, 1.25) // base( 1, 2, 1.7 )
        {
            /*
            base( MinCraftEffect, MaxCraftEffect, Delay )
            MinCraftEffect	: The minimum number of time the mobile will play the craft effect
            MaxCraftEffect	: The maximum number of time the mobile will play the craft effect
            Delay			: The delay between each craft effect
            Example: (3, 6, 1.7) would make the mobile do the PlayCraftEffect override
            function between 3 and 6 time, with a 1.7 second delay each time.
            */
        }

        private static readonly Type typeofAnvil = typeof(AnvilAttribute);
        private static readonly Type typeofForge = typeof(ForgeAttribute);

        public static void CheckAnvilAndForge(Mobile from, int range, out bool anvil, out bool forge)
        {
            anvil = false;
            forge = false;

            Map map = from.Map;

            if (map == null)
            {
                return;
            }

            IPooledEnumerable eable = map.GetItemsInRange(from.Location, range);

            foreach (Item item in eable)
            {
                Type type = item.GetType();

                bool isAnvil = (type.IsDefined(typeofAnvil, false) || item.ItemID == 4015 || item.ItemID == 4016 ||
                                item.ItemID == 0x2DD5 || item.ItemID == 0x2DD6);
                bool isForge = (type.IsDefined(typeofForge, false) || item.ItemID == 4017 ||
                                (item.ItemID >= 6522 && item.ItemID <= 6569) || item.ItemID == 0x2DD8) ||
                                item.ItemID == 0xA531 || item.ItemID == 0xA535;

                if (!isAnvil && !isForge)
                {
                    continue;
                }

                if ((from.Z + 16) < item.Z || (item.Z + 16) < from.Z || !from.InLOS(item))
                {
                    continue;
                }

                anvil = anvil || isAnvil;
                forge = forge || isForge;

                if (anvil && forge)
                {
                    break;
                }
            }

            eable.Free();

            for (int x = -range; (!anvil || !forge) && x <= range; ++x)
            {
                for (int y = -range; (!anvil || !forge) && y <= range; ++y)
                {
                    StaticTile[] tiles = map.Tiles.GetStaticTiles(from.X + x, from.Y + y, true);

                    for (int i = 0; (!anvil || !forge) && i < tiles.Length; ++i)
                    {
                        int id = tiles[i].ID;

                        bool isAnvil = (id == 4015 || id == 4016 || id == 0x2DD5 || id == 0x2DD6);
                        bool isForge = (id == 4017 || (id >= 6522 && id <= 6569) || id == 0x2DD8);

                        if (!isAnvil && !isForge)
                        {
                            continue;
                        }

                        if ((from.Z + 16) < tiles[i].Z || (tiles[i].Z + 16) < from.Z ||
                            !from.InLOS(new Point3D(from.X + x, from.Y + y, tiles[i].Z + (tiles[i].Height / 2) + 1)))
                        {
                            continue;
                        }

                        anvil = anvil || isAnvil;
                        forge = forge || isForge;
                    }
                }
            }
        }

        public override int CanCraft(Mobile from, ITool tool, Type itemType)
        {
            int num = 0;

            if (tool == null || tool.Deleted || tool.UsesRemaining <= 0)
            {
                return 1044038; // You have worn out your tool!
            }

            if (tool is Item && !BaseTool.CheckTool((Item)tool, from))
            {
                return 1048146; // If you have a tool equipped, you must use that tool.
            }

            else if (!tool.CheckAccessible(from, ref num))
            {
                return num; // The tool must be on your person to use.
            }

            if (tool is AddonToolComponent && from.InRange(((AddonToolComponent)tool).GetWorldLocation(), 2))
            {
                return 0;
            }

            bool anvil, forge;
            CheckAnvilAndForge(from, 2, out anvil, out forge);

            if (anvil && forge)
            {
                return 0;
            }

            return 1044267; // You must be near an anvil and a forge to smith items.
        }

        public override void PlayCraftEffect(Mobile from)
        {
            // no animation, instant sound
            //if ( from.Body.Type == BodyType.Human && !from.Mounted )
            //	from.Animate( 9, 5, 1, true, false, 0 );
            //new InternalTimer( from ).Start();
            from.PlaySound(0x2A);
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

        public override int PlayEndingEffect(
            Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item)
        {
            if (toolBroken)
            {
                from.SendLocalizedMessage(1044038); // You have worn out your tool
            }

            if (failed)
            {
                if (lostMaterial)
                {
                    return 1044043; // You failed to create the item, and some of your materials are lost.
                }

                return 1044157; // You failed to create the item, but no materials were lost.
            }

            if (quality == 0)
            {
                return 502785; // You were barely able to make this item.  It's quality is below average.
            }

            if (makersMark && quality == 2)
            {
                return 1044156; // You create an exceptional quality item and affix your maker's mark.
            }

            if (quality == 2)
            {
                return 1044155; // You create an exceptional quality item.
            }

            return 1044154; // You create the item.
        }

        public override void InitCraftList()
        {
            /*
            Synthax for a SIMPLE craft item
            AddCraft( ObjectType, Group, MinSkill, MaxSkill, ResourceType, Amount, Message )
            ObjectType		: The type of the object you want to add to the build list.
            Group			: The group in wich the object will be showed in the craft menu.
            MinSkill		: The minimum of skill value
            MaxSkill		: The maximum of skill value
            ResourceType	: The type of the resource the mobile need to create the item
            Amount			: The amount of the ResourceType it need to create the item
            Message			: String or Int for Localized.  The message that will be sent to the mobile, if the specified resource is missing.
            Synthax for a COMPLEXE craft item.  A complexe item is an item that need either more than
            only one skill, or more than only one resource.
            Coming soon....
            */

            int index;

            #region Metal Armor

            #region Ringmail
            AddCraft(typeof(RingmailGloves), 1111704, 1025099, 12.0, 62.0, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(typeof(RingmailLegs), 1111704, 1025104, 19.4, 69.4, typeof(IronIngot), 1044036, 16, 1044037);
            AddCraft(typeof(RingmailArms), 1111704, 1025103, 16.9, 66.9, typeof(IronIngot), 1044036, 14, 1044037);
            AddCraft(typeof(RingmailChest), 1111704, 1025100, 21.9, 71.9, typeof(IronIngot), 1044036, 18, 1044037);
            #endregion

            #region Chainmail
            AddCraft(typeof(ChainCoif), 1111704, 1025051, 14.5, 64.5, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(typeof(ChainLegs), 1111704, 1025054, 36.7, 86.7, typeof(IronIngot), 1044036, 18, 1044037);
            AddCraft(typeof(ChainChest), 1111704, 1025055, 39.1, 89.1, typeof(IronIngot), 1044036, 20, 1044037);
            #endregion

            #region Platemail
            AddCraft(typeof(PlateArms), 1111704, 1025136, 66.3, 116.3, typeof(IronIngot), 1044036, 18, 1044037);
            AddCraft(typeof(PlateGloves), 1111704, 1025140, 58.9, 108.9, typeof(IronIngot), 1044036, 12, 1044037);
            AddCraft(typeof(PlateGorget), 1111704, 1025139, 56.4, 106.4, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(typeof(PlateLegs), 1111704, 1025137, 68.8, 118.8, typeof(IronIngot), 1044036, 20, 1044037);
            AddCraft(typeof(PlateChest), 1111704, 1046431, 75.0, 125.0, typeof(IronIngot), 1044036, 25, 1044037);
            AddCraft(typeof(FemalePlateChest), 1111704, 1046430, 44.1, 94.1, typeof(IronIngot), 1044036, 20, 1044037);

            AddCraft(typeof(DragonBardingDeed), 1111704, 1053012, 72.5, 122.5, typeof(IronIngot), 1044036, 750, 1044037);

            index = AddCraft(typeof(PlateMempo), 1111704, 1030180, 80.0, 130.0, typeof(IronIngot), 1044036, 18, 1044037);

            index = AddCraft(typeof(PlateDo), 1111704, 1030184, 80.0, 130.0, typeof(IronIngot), 1044036, 28, 1044037);
            //Double check skill

            index = AddCraft(typeof(PlateHiroSode), 1111704, 1030187, 80.0, 130.0, typeof(IronIngot), 1044036, 16, 1044037);

            index = AddCraft(typeof(PlateSuneate), 1111704, 1030195, 65.0, 115.0, typeof(IronIngot), 1044036, 20, 1044037);

            index = AddCraft(typeof(PlateHaidate), 1111704, 1030200, 65.0, 115.0, typeof(IronIngot), 1044036, 20, 1044037);

            index = AddCraft(typeof(FemaleGargishPlateArms), 1111704, 1095336, 66.3, 116.3, typeof(IronIngot), 1044036, 18, 1044037);

            index = AddCraft(typeof(FemaleGargishPlateChest), 1111704, 1095338, 75.0, 125.0, typeof(IronIngot), 1044036, 25, 1044037);

            index = AddCraft(typeof(FemaleGargishPlateLegs), 1111704, 1095342, 68.8, 118.8, typeof(IronIngot), 1044036, 20, 1044037);

            index = AddCraft(typeof(FemaleGargishPlateKilt), 1111704, 1095340, 58.9, 108.9, typeof(IronIngot), 1044036, 12, 1044037);

            index = AddCraft(typeof(GargishPlateArms), 1111704, 1095336, 66.3, 116.3, typeof(IronIngot), 1044036, 18, 1044037);

            index = AddCraft(typeof(GargishPlateChest), 1111704, 1095338, 75.0, 125.0, typeof(IronIngot), 1044036, 25, 1044037);

            index = AddCraft(typeof(GargishPlateLegs), 1111704, 1095342, 68.8, 118.8, typeof(IronIngot), 1044036, 20, 1044037);

            index = AddCraft(typeof(GargishPlateKilt), 1111704, 1095340, 58.9, 108.9, typeof(IronIngot), 1044036, 12, 1044037);

            index = AddCraft(typeof(GargishAmulet), 1111704, 1098595, 60.0, 110.0, typeof(IronIngot), 1044036, 3, 1044037);

            index = AddCraft(typeof(BritchesOfWarding), 1111704, 1157345, 120.0, 120.1, typeof(IronIngot), 1044036, 18, 1044037);
            AddRes(index, typeof(LeggingsOfBane), 1061100, 1, 1053098);
            AddRes(index, typeof(Turquoise), 1032691, 4, 1053098);
            AddRes(index, typeof(BloodOfTheDarkFather), 1157343, 5, 1053098);
            AddRecipe(index, (int)SmithRecipes.BritchesOfWarding);
            ForceNonExceptional(index);
            #endregion
            #endregion

            #region Helmets
            AddCraft(typeof(Bascinet), 1011079, 1025132, 8.3, 58.3, typeof(IronIngot), 1044036, 15, 1044037);
            AddCraft(typeof(CloseHelm), 1011079, 1025128, 37.9, 87.9, typeof(IronIngot), 1044036, 15, 1044037);
            AddCraft(typeof(Helmet), 1011079, 1025130, 37.9, 87.9, typeof(IronIngot), 1044036, 15, 1044037);
            AddCraft(typeof(NorseHelm), 1011079, 1025134, 37.9, 87.9, typeof(IronIngot), 1044036, 15, 1044037);
            AddCraft(typeof(PlateHelm), 1011079, 1025138, 62.6, 112.6, typeof(IronIngot), 1044036, 15, 1044037);

            index = AddCraft(typeof(ChainHatsuburi), 1011079, 1030175, 30.0, 80.0, typeof(IronIngot), 1044036, 20, 1044037);

            index = AddCraft(typeof(PlateHatsuburi), 1011079, 1030176, 45.0, 95.0, typeof(IronIngot), 1044036, 20, 1044037);

            index = AddCraft(typeof(HeavyPlateJingasa), 1011079, 1030178, 45.0, 95.0, typeof(IronIngot), 1044036, 20, 1044037);

            index = AddCraft(typeof(LightPlateJingasa), 1011079, 1030188, 45.0, 95.0, typeof(IronIngot), 1044036, 20, 1044037);

            index = AddCraft(typeof(SmallPlateJingasa), 1011079, 1030191, 45.0, 95.0, typeof(IronIngot), 1044036, 20, 1044037);

            index = AddCraft(typeof(DecorativePlateKabuto), 1011079, 1030179, 90.0, 140.0, typeof(IronIngot), 1044036, 25, 1044037);

            index = AddCraft(typeof(PlateBattleKabuto), 1011079, 1030192, 90.0, 140.0, typeof(IronIngot), 1044036, 25, 1044037);

            index = AddCraft(typeof(StandardPlateKabuto), 1011079, 1030196, 90.0, 140.0, typeof(IronIngot), 1044036, 25, 1044037);

            index = AddCraft(typeof(Circlet), 1011079, 1032645, 62.1, 112.1, typeof(IronIngot), 1044036, 6, 1044037);

            index = AddCraft(typeof(RoyalCirclet), 1011079, 1032646, 70.0, 120.0, typeof(IronIngot), 1044036, 6, 1044037);

            index = AddCraft(typeof(GemmedCirclet), 1011079, 1032647, 75.0, 125.0, typeof(IronIngot), 1044036, 6, 1044037);
            AddRes(index, typeof(Tourmaline), 1044237, 1, 1044240);
            AddRes(index, typeof(Amethyst), 1044236, 1, 1044240);
            AddRes(index, typeof(BlueDiamond), 1032696, 1, 1044240);
            #endregion

            #region Shields
            AddCraft(typeof(Buckler), 1011080, 1027027, -25.0, 25.0, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(typeof(BronzeShield), 1011080, 1027026, -15.2, 34.8, typeof(IronIngot), 1044036, 12, 1044037);
            AddCraft(typeof(HeaterShield), 1011080, 1027030, 24.3, 74.3, typeof(IronIngot), 1044036, 18, 1044037);
            AddCraft(typeof(MetalShield), 1011080, 1027035, -10.2, 39.8, typeof(IronIngot), 1044036, 14, 1044037);
            AddCraft(typeof(MetalKiteShield), 1011080, 1027028, 4.6, 54.6, typeof(IronIngot), 1044036, 16, 1044037);
            AddCraft(typeof(WoodenKiteShield), 1011080, 1027032, -15.2, 34.8, typeof(IronIngot), 1044036, 8, 1044037);

            AddCraft(typeof(ChaosShield), 1011080, 1027107, 85.0, 135.0, typeof(IronIngot), 1044036, 25, 1044037);
            AddCraft(typeof(OrderShield), 1011080, 1027108, 85.0, 135.0, typeof(IronIngot), 1044036, 25, 1044037);

            index = AddCraft(typeof(SmallPlateShield), 1011080, 1095770, -25.0, 25.0, typeof(IronIngot), 1044036, 12, 1044037);

            index = AddCraft(typeof(GargishKiteShield), 1011080, 1095774, 4.6, 54.6, typeof(IronIngot), 1044036, 16, 1044037);

            index = AddCraft(typeof(LargePlateShield), 1011080, 1095772, 24.3, 74.3, typeof(IronIngot), 1044036, 18, 1044037);

            index = AddCraft(typeof(MediumPlateShield), 1011080, 1095771, -10.2, 39.8, typeof(IronIngot), 1044036, 14, 1044037);

            index = AddCraft(typeof(GargishChaosShield), 1011080, 1095808, 85.0, 135.0, typeof(IronIngot), 1044036, 25, 1044037);

            index = AddCraft(typeof(GargishOrderShield), 1011080, 1095810, 85.0, 135.0, typeof(IronIngot), 1044036, 25, 1044037);
            #endregion

            #region Bladed
            AddCraft(typeof(BoneHarvester), 1011081, 1029915, 33.0, 83.0, typeof(IronIngot), 1044036, 10, 1044037);

            AddCraft(typeof(Broadsword), 1011081, 1023934, 35.4, 85.4, typeof(IronIngot), 1044036, 10, 1044037);

            AddCraft(typeof(CrescentBlade), 1011081, 1029921, 45.0, 95.0, typeof(IronIngot), 1044036, 14, 1044037);

            AddCraft(typeof(Cutlass), 1011081, 1025185, 24.3, 74.3, typeof(IronIngot), 1044036, 8, 1044037);
            AddCraft(typeof(Dagger), 1011081, 1023921, -0.4, 49.6, typeof(IronIngot), 1044036, 3, 1044037);
            AddCraft(typeof(Katana), 1011081, 1025119, 44.1, 94.1, typeof(IronIngot), 1044036, 8, 1044037);
            AddCraft(typeof(Kryss), 1011081, 1025121, 36.7, 86.7, typeof(IronIngot), 1044036, 8, 1044037);
            AddCraft(typeof(Longsword), 1011081, 1023937, 28.0, 78.0, typeof(IronIngot), 1044036, 12, 1044037);
            AddCraft(typeof(Scimitar), 1011081, 1025046, 31.7, 81.7, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(typeof(VikingSword), 1011081, 1025049, 24.3, 74.3, typeof(IronIngot), 1044036, 14, 1044037);

            index = AddCraft(typeof(NoDachi), 1011081, 1030221, 75.0, 125.0, typeof(IronIngot), 1044036, 18, 1044037);

            index = AddCraft(typeof(Wakizashi), 1011081, 1030223, 50.0, 100.0, typeof(IronIngot), 1044036, 8, 1044037);

            index = AddCraft(typeof(Lajatang), 1011081, 1030226, 80.0, 130.0, typeof(IronIngot), 1044036, 25, 1044037);

            index = AddCraft(typeof(Daisho), 1011081, 1030228, 60.0, 110.0, typeof(IronIngot), 1044036, 15, 1044037);

            index = AddCraft(typeof(Tekagi), 1011081, 1030230, 55.0, 105.0, typeof(IronIngot), 1044036, 12, 1044037);

            index = AddCraft(typeof(Shuriken), 1011081, 1030231, 45.0, 95.0, typeof(IronIngot), 1044036, 5, 1044037);

            index = AddCraft(typeof(Kama), 1011081, 1030232, 40.0, 90.0, typeof(IronIngot), 1044036, 14, 1044037);

            index = AddCraft(typeof(Sai), 1011081, 1030234, 50.0, 100.0, typeof(IronIngot), 1044036, 12, 1044037);

            index = AddCraft(typeof(RadiantScimitar), 1011081, 1031571, 75.0, 125.0, typeof(IronIngot), 1044036, 15, 1044037);

            index = AddCraft(typeof(WarCleaver), 1011081, 1031567, 70.0, 120.0, typeof(IronIngot), 1044036, 18, 1044037);

            index = AddCraft(typeof(ElvenSpellblade), 1011081, 1031564, 70.0, 120.0, typeof(IronIngot), 1044036, 14, 1044037);

            index = AddCraft(typeof(AssassinSpike), 1011081, 1031565, 70.0, 120.0, typeof(IronIngot), 1044036, 9, 1044037);

            index = AddCraft(typeof(Leafblade), 1011081, 1031566, 70.0, 120.0, typeof(IronIngot), 1044036, 12, 1044037);

            index = AddCraft(typeof(RuneBlade), 1011081, 1031570, 70.0, 120.0, typeof(IronIngot), 1044036, 15, 1044037);

            index = AddCraft(typeof(ElvenMachete), 1011081, 1031573, 70.0, 120.0, typeof(IronIngot), 1044036, 14, 1044037);

            index = AddCraft(typeof(RuneCarvingKnife), 1011081, 1072915, 70.0, 120.0, typeof(IronIngot), 1044036, 9, 1044037);
            AddRes(index, typeof(DreadHornMane), 1032682, 1, 1053098);
            AddRes(index, typeof(Putrefaction), 1032678, 10, 1053098);
            AddRes(index, typeof(Muculent), 1032680, 10, 1053098);
            AddRecipe(index, (int)SmithRecipes.RuneCarvingKnife);
            ForceNonExceptional(index);

            index = AddCraft(typeof(ColdForgedBlade), 1011081, 1072916, 70.0, 120.0, typeof(IronIngot), 1044036, 18, 1044037);
            AddRes(index, typeof(GrizzledBones), 1032684, 1, 1053098);
            AddRes(index, typeof(Taint), 1032684, 10, 1053098);
            AddRes(index, typeof(Blight), 1032675, 10, 1053098);
            AddRecipe(index, (int)SmithRecipes.ColdForgedBlade);
            ForceNonExceptional(index);

            index = AddCraft(typeof(OverseerSunderedBlade), 1011081, 1072920, 70.0, 120.0, typeof(IronIngot), 1044036, 15, 1044037);
            AddRes(index, typeof(GrizzledBones), 1032684, 1, 1053098);
            AddRes(index, typeof(Blight), 1032675, 10, 1053098);
            AddRes(index, typeof(Scourge), 1032677, 10, 1053098);
            AddRecipe(index, (int)SmithRecipes.OverseerSunderedBlade);
            ForceNonExceptional(index);

            index = AddCraft(typeof(LuminousRuneBlade), 1011081, 1072922, 70.0, 120.0, typeof(IronIngot), 1044036, 15, 1044037);
            AddRes(index, typeof(GrizzledBones), 1032684, 1, 1053098);
            AddRes(index, typeof(Corruption), 1032676, 10, 1053098);
            AddRes(index, typeof(Putrefaction), 1032678, 10, 1053098);
            AddRecipe(index, (int)SmithRecipes.LuminousRuneBlade);
            ForceNonExceptional(index);

            index = AddCraft(typeof(TrueSpellblade), 1011081, 1073513, 75.0, 125.0, typeof(IronIngot), 1044036, 14, 1044037);
            AddRes(index, typeof(BlueDiamond), 1032696, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.TrueSpellblade);

            index = AddCraft(typeof(IcySpellblade), 1011081, 1073514, 75.0, 125.0, typeof(IronIngot), 1044036, 14, 1044037);
            AddRes(index, typeof(Turquoise), 1032691, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.IcySpellblade);

            index = AddCraft(typeof(FierySpellblade), 1011081, 1073515, 75.0, 125.0, typeof(IronIngot), 1044036, 14, 1044037);
            AddRes(index, typeof(FireRuby), 1032695, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.FierySpellblade);

            index = AddCraft(typeof(SpellbladeOfDefense), 1011081, 1073516, 75.0, 125.0, typeof(IronIngot), 1044036, 18, 1044037);
            AddRes(index, typeof(WhitePearl), 1032694, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.SpellbladeOfDefense);

            index = AddCraft(typeof(TrueAssassinSpike), 1011081, 1073517, 75.0, 125.0, typeof(IronIngot), 1044036, 9, 1044037);
            AddRes(index, typeof(DarkSapphire), 1032690, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.TrueAssassinSpike);

            index = AddCraft(typeof(ChargedAssassinSpike), 1011081, 1073518, 75.0, 125.0, typeof(IronIngot), 1044036, 9, 1044037);
            AddRes(index, typeof(EcruCitrine), 1032693, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.ChargedAssassinSpike);

            index = AddCraft(typeof(MagekillerAssassinSpike), 1011081, 1073519, 75.0, 125.0, typeof(IronIngot), 1044036, 9, 1044037);
            AddRes(index, typeof(BrilliantAmber), 1032697, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.MagekillerAssassinSpike);

            index = AddCraft(typeof(WoundingAssassinSpike), 1011081, 1073520, 75.0, 125.0, typeof(IronIngot), 1044036, 9, 1044037);
            AddRes(index, typeof(PerfectEmerald), 1032692, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.WoundingAssassinSpike);

            index = AddCraft(typeof(TrueLeafblade), 1011081, 1073521, 75.0, 125.0, typeof(IronIngot), 1044036, 12, 1044037);
            AddRes(index, typeof(BlueDiamond), 1032696, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.TrueLeafblade);

            index = AddCraft(typeof(Luckblade), 1011081, 1073522, 75.0, 125.0, typeof(IronIngot), 1044036, 12, 1044037);
            AddRes(index, typeof(WhitePearl), 1032694, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.Luckblade);

            index = AddCraft(typeof(MagekillerLeafblade), 1011081, 1073523, 75.0, 125.0, typeof(IronIngot), 1044036, 12, 1044037);
            AddRes(index, typeof(FireRuby), 1032695, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.MagekillerLeafblade);

            index = AddCraft(typeof(LeafbladeOfEase), 1011081, 1073524, 75.0, 125.0, typeof(IronIngot), 1044036, 12, 1044037);
            AddRes(index, typeof(PerfectEmerald), 1032692, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.LeafbladeOfEase);

            index = AddCraft(typeof(KnightsWarCleaver), 1011081, 1073525, 75.0, 125.0, typeof(IronIngot), 1044036, 18, 1044037);
            AddRes(index, typeof(PerfectEmerald), 1032692, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.KnightsWarCleaver);

            index = AddCraft(typeof(ButchersWarCleaver), 1011081, 1073526, 75.0, 125.0, typeof(IronIngot), 1044036, 18, 1044037);
            AddRes(index, typeof(Turquoise), 1032691, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.ButchersWarCleaver);

            index = AddCraft(typeof(SerratedWarCleaver), 1011081, 1073527, 75.0, 125.0, typeof(IronIngot), 1044036, 18, 1044037);
            AddRes(index, typeof(EcruCitrine), 1032693, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.SerratedWarCleaver);

            index = AddCraft(typeof(TrueWarCleaver), 1011081, 1073528, 75.0, 125.0, typeof(IronIngot), 1044036, 18, 1044037);
            AddRes(index, typeof(BrilliantAmber), 1032697, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.TrueWarCleaver);

            index = AddCraft(typeof(AdventurersMachete), 1011081, 1073533, 75.0, 125.0, typeof(IronIngot), 1044036, 14, 1044037);
            AddRes(index, typeof(WhitePearl), 1032694, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.AdventurersMachete);

            index = AddCraft(typeof(OrcishMachete), 1011081, 1073534, 75.0, 125.0, typeof(IronIngot), 1044036, 14, 1044037);
            AddRes(index, typeof(Scourge), 1072136, 1, 1042081);
            AddRecipe(index, (int)SmithRecipes.OrcishMachete);

            index = AddCraft(typeof(MacheteOfDefense), 1011081, 1073535, 75.0, 125.0, typeof(IronIngot), 1044036, 14, 1044037);
            AddRes(index, typeof(BrilliantAmber), 1032697, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.MacheteOfDefense);

            index = AddCraft(typeof(DiseasedMachete), 1011081, 1073536, 75.0, 125.0, typeof(IronIngot), 1044036, 14, 1044037);
            AddRes(index, typeof(Blight), 1072134, 1, 1042081);
            AddRecipe(index, (int)SmithRecipes.DiseasedMachete);

            index = AddCraft(typeof(Runesabre), 1011081, 1073537, 75.0, 125.0, typeof(IronIngot), 1044036, 15, 1044037);
            AddRes(index, typeof(Turquoise), 1032691, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.Runesabre);

            index = AddCraft(typeof(MagesRuneBlade), 1011081, 1073538, 75.0, 125.0, typeof(IronIngot), 1044036, 15, 1044037);
            AddRes(index, typeof(BlueDiamond), 1032696, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.MagesRuneBlade);

            index = AddCraft(typeof(RuneBladeOfKnowledge), 1011081, 1073539, 75.0, 125.0, typeof(IronIngot), 1044036, 15, 1044037);
            AddRes(index, typeof(EcruCitrine), 1032693, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.RuneBladeOfKnowledge);

            index = AddCraft(typeof(CorruptedRuneBlade), 1011081, 1073540, 75.0, 125.0, typeof(IronIngot), 1044036, 15, 1044037);
            AddRes(index, typeof(Corruption), 1072135, 1, 1042081);
            AddRecipe(index, (int)SmithRecipes.CorruptedRuneBlade);

            index = AddCraft(typeof(TrueRadiantScimitar), 1011081, 1073541, 75.0, 125.0, typeof(IronIngot), 1044036, 15, 1044037);
            AddRes(index, typeof(BrilliantAmber), 1032697, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.TrueRadiantScimitar);

            index = AddCraft(typeof(DarkglowScimitar), 1011081, 1073542, 75.0, 125.0, typeof(IronIngot), 1044036, 15, 1044037);
            AddRes(index, typeof(DarkSapphire), 1032690, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.DarkglowScimitar);

            index = AddCraft(typeof(IcyScimitar), 1011081, 1073543, 75.0, 125.0, typeof(IronIngot), 1044036, 15, 1044037);
            AddRes(index, typeof(DarkSapphire), 1032690, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.IcyScimitar);

            index = AddCraft(typeof(TwinklingScimitar), 1011081, 1073544, 75.0, 125.0, typeof(IronIngot), 1044036, 15, 1044037);
            AddRes(index, typeof(DarkSapphire), 1032690, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.TwinklingScimitar);

            index = AddCraft(typeof(BoneMachete), 1011081, 1020526, 45.0, 95.0, typeof(IronIngot), 1044036, 20, 1044037);
            AddRes(index, typeof(Bone), 1049064, 6, 1049063);
            AddRecipe(index, (int)SmithRecipes.BoneMachete);

            index = AddCraft(typeof(GargishKatana), 1011081, 1097490, 44.1, 94.1, typeof(IronIngot), 1044036, 8, 1044037);

            index = AddCraft(typeof(GargishKryss), 1011081, 1097492, 36.7, 86.7, typeof(IronIngot), 1044036, 8, 1044037);

            index = AddCraft(typeof(GargishBoneHarvester), 1011081, 1097502, 33.0, 83.0, typeof(IronIngot), 1044036, 10, 1044037);

            index = AddCraft(typeof(GargishTekagi), 1011081, 1097510, 55.0, 105.0, typeof(IronIngot), 1044036, 12, 1044037);

            index = AddCraft(typeof(GargishDaisho), 1011081, 1097512, 60.0, 110.0, typeof(IronIngot), 1044036, 15, 1044037);

            index = AddCraft(typeof(DreadSword), 1011081, 1095372, 75.0, 125.0, typeof(IronIngot), 1044036, 14, 1044037);

            index = AddCraft(typeof(GargishTalwar), 1011081, 1095373, 75.0, 150.0, typeof(IronIngot), 1044036, 18, 1044037);

            index = AddCraft(typeof(GargishDagger), 1011081, 1095362, 0.0, 100.0, typeof(IronIngot), 1044036, 3, 1044037);

            index = AddCraft(typeof(BloodBlade), 1011081, 1095370, 44.1, 125.0, typeof(IronIngot), 1044036, 8, 1044037);

            index = AddCraft(typeof(Shortblade), 1011081, 1095374, 28.0, 100.0, typeof(IronIngot), 1044036, 12, 1044037);
            #endregion

            #region Axes
            AddCraft(typeof(Axe), 1011082, 1023913, 34.2, 84.2, typeof(IronIngot), 1044036, 14, 1044037);
            AddCraft(typeof(BattleAxe), 1011082, 1023911, 30.5, 80.5, typeof(IronIngot), 1044036, 14, 1044037);
            AddCraft(typeof(DoubleAxe), 1011082, 1023915, 29.3, 79.3, typeof(IronIngot), 1044036, 12, 1044037);
            AddCraft(typeof(ExecutionersAxe), 1011082, 1023909, 34.2, 84.2, typeof(IronIngot), 1044036, 14, 1044037);
            AddCraft(typeof(LargeBattleAxe), 1011082, 1025115, 28.0, 78.0, typeof(IronIngot), 1044036, 12, 1044037);
            AddCraft(typeof(TwoHandedAxe), 1011082, 1025187, 33.0, 83.0, typeof(IronIngot), 1044036, 16, 1044037);
            AddCraft(typeof(WarAxe), 1011082, 1025040, 39.1, 89.1, typeof(IronIngot), 1044036, 16, 1044037);

            index = AddCraft(typeof(OrnateAxe), 1011082, 1031572, 70.0, 120.0, typeof(IronIngot), 1044036, 18, 1044037);

            index = AddCraft(typeof(GuardianAxe), 1011082, 1073545, 75.0, 125.0, typeof(IronIngot), 1044036, 15, 1044037);
            AddRes(index, typeof(BlueDiamond), 1032696, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.GuardianAxe);

            index = AddCraft(typeof(SingingAxe), 1011082, 1073546, 75.0, 125.0, typeof(IronIngot), 1044036, 15, 1044037);
            AddRes(index, typeof(BrilliantAmber), 1032697, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.SingingAxe);

            index = AddCraft(typeof(ThunderingAxe), 1011082, 1073547, 75.0, 125.0, typeof(IronIngot), 1044036, 15, 1044037);
            AddRes(index, typeof(EcruCitrine), 1032693, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.ThunderingAxe);

            index = AddCraft(typeof(HeavyOrnateAxe), 1011082, 1073548, 75.0, 125.0, typeof(IronIngot), 1044036, 15, 1044037);
            AddRes(index, typeof(Turquoise), 1032691, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.HeavyOrnateAxe);

            index = AddCraft(typeof(GargishBattleAxe), 1011082, 1097480, 30.5, 80.5, typeof(IronIngot), 1044036, 14, 1044037);

            index = AddCraft(typeof(GargishAxe), 1011082, 1097482, 34.2, 84.2, typeof(IronIngot), 1044036, 14, 1044037);

            index = AddCraft(typeof(DualShortAxes), 1011082, 1095360, 75.0, 125.0, typeof(IronIngot), 1044036, 24, 1044037);
            #endregion

            #region Pole Arms

            AddCraft(typeof(Bardiche), 1011083, 1023917, 31.7, 81.7, typeof(IronIngot), 1044036, 18, 1044037);

            AddCraft(typeof(BladedStaff), 1011083, 1029917, 40.0, 90.0, typeof(IronIngot), 1044036, 12, 1044037);

            AddCraft(typeof(DoubleBladedStaff), 1011083, 1029919, 45.0, 95.0, typeof(IronIngot), 1044036, 16, 1044037);

            AddCraft(typeof(Halberd), 1011083, 1025183, 39.1, 89.1, typeof(IronIngot), 1044036, 20, 1044037);

            AddCraft(typeof(Lance), 1011083, 1029920, 48.0, 98.0, typeof(IronIngot), 1044036, 20, 1044037);

            AddCraft(typeof(Pike), 1011083, 1029918, 47.0, 97.0, typeof(IronIngot), 1044036, 12, 1044037);

            AddCraft(typeof(ShortSpear), 1011083, 1025123, 45.3, 95.3, typeof(IronIngot), 1044036, 6, 1044037);

            AddCraft(typeof(Scythe), 1011083, 1029914, 39.0, 89.0, typeof(IronIngot), 1044036, 14, 1044037);

            AddCraft(typeof(Spear), 1011083, 1023938, 49.0, 99.0, typeof(IronIngot), 1044036, 12, 1044037);
            AddCraft(typeof(WarFork), 1011083, 1025125, 42.9, 92.9, typeof(IronIngot), 1044036, 12, 1044037);

            index = AddCraft(typeof(GargishBardiche), 1011083, 1097484, 31.7, 81.7, typeof(IronIngot), 1044036, 18, 1044037);

            index = AddCraft(typeof(GargishWarFork), 1011083, 1097494, 42.9, 92.9, typeof(IronIngot), 1044036, 12, 1044037);

            index = AddCraft(typeof(GargishScythe), 1011083, 1097500, 39.0, 89.0, typeof(IronIngot), 1044036, 14, 1044037);

            index = AddCraft(typeof(GargishPike), 1011083, 1097504, 47.0, 97.0, typeof(IronIngot), 1044036, 12, 1044037);

            index = AddCraft(typeof(GargishLance), 1011083, 1097506, 48.0, 98.0, typeof(IronIngot), 1044036, 20, 1044037);

            index = AddCraft(typeof(DualPointedSpear), 1011083, 1095365, 47.0, 97.0, typeof(IronIngot), 1044036, 16, 1044037);

            #endregion

            #region Bashing
            AddCraft(typeof(HammerPick), 1011084, 1025181, 34.2, 84.2, typeof(IronIngot), 1044036, 16, 1044037);
            AddCraft(typeof(Mace), 1011084, 1023932, 14.5, 64.5, typeof(IronIngot), 1044036, 6, 1044037);
            AddCraft(typeof(Maul), 1011084, 1025179, 19.4, 69.4, typeof(IronIngot), 1044036, 10, 1044037);

            AddCraft(typeof(Scepter), 1011084, 1029916, 21.4, 71.4, typeof(IronIngot), 1044036, 10, 1044037);

            AddCraft(typeof(WarMace), 1011084, 1025127, 28.0, 78.0, typeof(IronIngot), 1044036, 14, 1044037);
            AddCraft(typeof(WarHammer), 1011084, 1025177, 34.2, 84.2, typeof(IronIngot), 1044036, 16, 1044037);

            index = AddCraft(typeof(Tessen), 1011084, 1030222, 85.0, 135.0, typeof(IronIngot), 1044036, 16, 1044037);
            AddSkill(index, SkillName.Tailoring, 50.0, 55.0);
            AddRes(index, typeof(Cloth), 1044286, 10, 1044287);

            index = AddCraft(typeof(DiamondMace), 1011084, 1031568, 70.0, 120.0, typeof(IronIngot), 1044036, 20, 1044037);

            index = AddCraft(typeof(ShardThrasher), 1011084, 1072918, 70.0, 120.0, typeof(IronIngot), 1044036, 20, 1044037);
            AddRes(index, typeof(EyeOfTheTravesty), 1073126, 1, 1042081);
            AddRes(index, typeof(Muculent), 1072139, 10, 1042081);
            AddRes(index, typeof(Corruption), 1072135, 10, 1042081);
            AddRecipe(index, (int)SmithRecipes.ShardTrasher);
            ForceNonExceptional(index);

            index = AddCraft(typeof(RubyMace), 1011084, 1073529, 75.0, 125.0, typeof(IronIngot), 1044036, 20, 1044037);
            AddRes(index, typeof(FireRuby), 1032695, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.RubyMace);

            index = AddCraft(typeof(EmeraldMace), 1011084, 1073530, 75.0, 125.0, typeof(IronIngot), 1044036, 20, 1044037);
            AddRes(index, typeof(PerfectEmerald), 1032692, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.EmeraldMace);

            index = AddCraft(typeof(SapphireMace), 1011084, 1073531, 75.0, 125.0, typeof(IronIngot), 1044036, 20, 1044037);
            AddRes(index, typeof(DarkSapphire), 1032690, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.SapphireMace);

            index = AddCraft(typeof(SilverEtchedMace), 1011084, 1073532, 75.0, 125.0, typeof(IronIngot), 1044036, 20, 1044037);
            AddRes(index, typeof(BlueDiamond), 1032696, 1, 1044240);
            AddRecipe(index, (int)SmithRecipes.SilverEtchedMace);

            index = AddCraft(typeof(GargishWarHammer), 1011084, 1097496, 34.2, 84.2, typeof(IronIngot), 1044036, 16, 1044037);

            index = AddCraft(typeof(GargishMaul), 1011084, 1097498, 19.4, 69.4, typeof(IronIngot), 1044036, 10, 1044037);

            index = AddCraft(typeof(GargishTessen), 1011084, 1097508, 85.0, 135.0, typeof(IronIngot), 1044036, 16, 1044037);
            AddSkill(index, SkillName.Tailoring, 50.0, 55.0);
            AddRes(index, typeof(Cloth), 1044286, 10, 1044287);

            index = AddCraft(typeof(DiscMace), 1011084, 1095366, 70.0, 120.0, typeof(IronIngot), 1044036, 20, 1044037);

            #endregion

            #region High Seas Cannons

            index = AddCraft(typeof(Cannonball), 1116354, 1116029, 10.0, 60.0, typeof(IronIngot), 1044036, 12, 1044037);
            SetUseAllRes(index, true);

            index = AddCraft(typeof(Grapeshot), 1116354, 1116030, 15.0, 70.0, typeof(IronIngot), 1044036, 12, 1044037);
            AddRes(index, typeof(Cloth), 1044286, 2, 1044287);
            SetUseAllRes(index, true);

            index = AddCraft(typeof(LightShipCannonDeed), 1116354, 1095790, 65.0, 120.0, typeof(IronIngot), 1044036, 900, 1044037);
            AddRes(index, typeof(Board), 1044041, 50, 1044351);
            AddSkill(index, SkillName.Carpentry, 65.0, 100.0);

            index = AddCraft(typeof(HeavyShipCannonDeed), 1116354, 1095794, 70.0, 120.0, typeof(IronIngot), 1044036, 1800, 1044037);
            AddRes(index, typeof(Board), 1044041, 75, 1044351);
            AddSkill(index, SkillName.Carpentry, 70.0, 100.0);

            #endregion

            #region Throwing

            index = AddCraft(typeof(Boomerang), 1079508, 1095359, 75.0, 125.0, typeof(IronIngot), 1044036, 5, 1044037);

            index = AddCraft(typeof(Cyclone), 1079508, 1095364, 75.0, 125.0, typeof(IronIngot), 1044036, 9, 1044037);

            index = AddCraft(typeof(SoulGlaive), 1079508, 1095363, 75.0, 125.0, typeof(IronIngot), 1044036, 9, 1044037);

            #endregion

            #region Miscellaneous

            index = AddCraft(typeof(DragonGloves), 1011173, 1029795, 68.9, 118.9, typeof(RedScales), 1060883, 16, 1060884);
            SetUseSubRes2(index, true);

            index = AddCraft(typeof(DragonHelm), 1011173, 1029797, 72.6, 122.6, typeof(RedScales), 1060883, 20, 1060884);
            SetUseSubRes2(index, true);

            index = AddCraft(typeof(DragonLegs), 1011173, 1029799, 78.8, 128.8, typeof(RedScales), 1060883, 28, 1060884);
            SetUseSubRes2(index, true);

            index = AddCraft(typeof(DragonArms), 1011173, 1029815, 76.3, 126.3, typeof(RedScales), 1060883, 24, 1060884);
            SetUseSubRes2(index, true);

            index = AddCraft(typeof(DragonChest), 1011173, 1029793, 85.0, 135.0, typeof(RedScales), 1060883, 36, 1060884);
            SetUseSubRes2(index, true);

            index = AddCraft(typeof(CrushedGlass), 1011173, 1113351, 110.0, 135.0, typeof(BlueDiamond), 1032696, 1, 1044253);
            AddRes(index, typeof(GlassSword), 1095371, 5, 1044253);

            index = AddCraft(typeof(PowderedIron), 1011173, 1113353, 110.0, 135.0, typeof(WhitePearl), 1026253, 1, 1044253);
            AddRes(index, typeof(IronIngot), 1044036, 20, 1044037);

            AddCraft(typeof(MetalKeg), 1011173, 1150675, 85.0, 100.0, typeof(IronIngot), 1044036, 25, 1044253);

            index = AddCraft(typeof(ExodusSacrificalDagger), 1011173, 1153500, 95.0, 120.0, typeof(IronIngot), 1044036, 12, 1044253);
            AddRes(index, typeof(BlueDiamond), 1032696, 2, 1044253);
            AddRes(index, typeof(FireRuby), 1032695, 2, 1044253);
            AddRes(index, typeof(SmallPieceofBlackrock), 1150016, 10, 1044253);
            ForceNonExceptional(index);

            index = AddCraft(typeof(GlovesOfFeudalGrip), 1011173, 1157349, 120.0, 120.1, typeof(RedScales), 1060883, 18, 1060884);
            SetUseSubRes2(index, true);
            AddRes(index, typeof(BlueDiamond), 1032696, 4, 1044253);
            AddRes(index, typeof(GauntletsOfNobility), 1061092, 1, 1053098);
            AddRes(index, typeof(BloodOfTheDarkFather), 1157343, 5, 1053098);
            AddRecipe(index, (int)SmithRecipes.GlovesOfFeudalGrip);
            ForceNonExceptional(index);

            #endregion

            // Set the overridable material
            SetSubRes(typeof(IronIngot), 1044022);

            // Add every material you want the player to be able to choose from
            // This will override the overridable material
            AddSubRes(typeof(IronIngot), 1044022, 00.0, 1044036, 1044269);
            AddSubRes(typeof(DullCopperIngot), 1044023, 65.0, 1044036, 1044269);
            AddSubRes(typeof(ShadowIronIngot), 1044024, 70.0, 1044036, 1044269);
            AddSubRes(typeof(CopperIngot), 1044025, 75.0, 1044036, 1044269);
            AddSubRes(typeof(BronzeIngot), 1044026, 80.0, 1044036, 1044269);
            AddSubRes(typeof(GoldIngot), 1044027, 85.0, 1044036, 1044269);
            AddSubRes(typeof(AgapiteIngot), 1044028, 90.0, 1044036, 1044269);
            AddSubRes(typeof(VeriteIngot), 1044029, 95.0, 1044036, 1044269);
            AddSubRes(typeof(ValoriteIngot), 1044030, 99.0, 1044036, 1044269);

            SetSubRes2(typeof(RedScales), 1060875);

            AddSubRes2(typeof(RedScales), 1060875, 0.0, 1053137, 1044268);
            AddSubRes2(typeof(YellowScales), 1060876, 0.0, 1053137, 1044268);
            AddSubRes2(typeof(BlackScales), 1060877, 0.0, 1053137, 1044268);
            AddSubRes2(typeof(GreenScales), 1060878, 0.0, 1053137, 1044268);
            AddSubRes2(typeof(WhiteScales), 1060879, 0.0, 1053137, 1044268);
            AddSubRes2(typeof(BlueScales), 1060880, 0.0, 1053137, 1044268);

            Resmelt = true;
            Repair = true;
            MarkOption = true;
            CanEnhance = true;
            CanAlter = true;
        }
    }

    public class ForgeAttribute : Attribute
    { }

    public class AnvilAttribute : Attribute
    { }
}
