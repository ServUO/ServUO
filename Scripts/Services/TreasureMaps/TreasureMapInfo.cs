using System;
using System.Collections.Generic;

using Server;
using Server.Misc;
using Server.Engines.Craft;

namespace Server.Items
{
    public enum TreasureLevel
    {
        Stash,
        Supply,
        Cache,
        Hoard,
        Trove
    }

    public enum TreasurePackage
    {
        Artisan,
        Assassin,
        Mage,
        Ranger,
        Warrior
    }

    public enum TreasureFacet
    {
        Trammel,
        Felucca,
        Ilshenar,
        Malas,
        Tokuno,
        TerMur,
        Eodon
    }

    public enum ChestQuality
    {
        None,
        Rusty,
        Standard,
        Gold
    }

    public static class TreasureMapInfo
    {
        public static void Initialize()
        {
        }

        public static TreasureFacet GetFacet(IEntity e)
        {
            return GetFacet(e.Location, e.Map);
        }

        public static int PackageLocalization(TreasurePackage package)
        {
            switch (package)
            {
                case TreasurePackage.Artisian: return 1158989;
                case TreasurePackage.Assassin: return 1158987;
                case TreasurePackage.Mage: return 1158986;
                case TreasurePackage.Ranger: return 1158990;
                case TreasurePackage.Warrior: return 1158988;
            }

            return 0;
        }

        public static TreasureFacet GetFacet(IPoint2D p, Map map)
        {
            if (map == Map.TerMur)
            {
                if (SpellHelper.IsEodon(new Point3D(p.X, p.Y, 0), map))
                {
                    return TreasureFacet.Eodon;
                }

                return TreasureFacet.TerMur;
            }

            if (map == Map.Felucca)
            {
                return TreasureFacet.Felucca;
            }

            if (map == Map.Trammel)
            {
                return TreasureFacet.Trammel;
            }

            if (map == Map.Malas)
            {
                return TreasureFacet.Malas;
            }

            if (map == Map.Ilshenar)
            {
                return TreasureFacet.Ilshenar;
            }

            if (map == Map.Tokuno)
            {
                return TreasureFacet.Tokuno;
            }
        }

        public static Type[] GetWeaponList(TreasureLevel level, TreasurePackage package, Map map)
        {
            Type[] list = null;

            switch (GetMap(map))
            {
                case TreasureFacet.Trammel:
                case TreasureFacet.Felucca: list = _WeaponTable[(int)package][0]; break;
                case TreasureFacet.Ilshenar: list = _WeaponTable[(int)package][1]; break;
                case TreasureFacet.Malas: list = _WeaponTable[(int)package][2]; break;
                case TreasureFacet.Tokuno: list = _WeaponTable[(int)package][3]; break;
                case TreasureFacet.TerMur: list = _WeaponTable[(int)package][4]; break;
                case TreasureFacet.Eodon: list = _WeaponTable[(int)package][5]; break;
            }

            // tram/fel lists are always default
            if (list == null || list.Length == 0)
            {
                list = _WeaponTable[(int)package][0];
            }

            return list;
        }

        public static Type[] GetArmorList(TreasureLevel level, TreasurePackage package, Map map)
        {
            Type[] list = null;

            switch (GetMap(map))
            {
                case TreasureFacet.Trammel:
                case TreasureFacet.Felucca: list = _ArmorTable[(int)package][0]; break;
                case TreasureFacet.Ilshenar: list = _ArmorTable[(int)package][1]; break;
                case TreasureFacet.Malas: list = _ArmorTable[(int)package][2]; break;
                case TreasureFacet.Tokuno: list = _ArmorTable[(int)package][3]; break;
                case TreasureFacet.TerMur: list = _ArmorTable[(int)package][4]; break;
                case TreasureFacet.Eodon: list = _ArmorTable[(int)package][5]; break;
            }

            // tram/fel lists are always default
            if (list == null || list.Length == 0)
            {
                list = _ArmorTable[(int)package][0];
            }

            return null;
        }

        public static Type[] GetJewelList(TreasureLevel level, TreasurePackage package, Map map)
        {
            var facet = GetFacet(map);

            if (facet == TreasureFacet.TerMur)
            {
                return _JewelTable[1];
            }
            else
            {
                return _JewelTable[0];
            }

            return null;
        }

        public static SkillName GetTranscendenceList(TreasurePackage package)
        {
            return _TranscendenceTable[(int)package];
        }

        public static SkillName GetAlacrityList(TreasureLevel level, TreasurePackage package)
        {
            if (level >= TreasureLevel.Supply)
            {
                return _AlacrityTable[(int)package];
            }

            return null;
        }

        public static SkillName GetPowerScrollList(TreasureLevel level, TreasurePackage package)
        {
            if (level >= TreasureLevel.Cache)
            {
                return _PowerScrollTable[(int)package];
            }

            return null;
        }

        public static Type[] GetCraftingMaterials(TreasureLevel level, TreasurePackage package)
        {
            if (package == TreasurePackage.Artisan && level <= TreasureLevel.Supply)
            {
                return _MaterialTable;
            }

            return null;
        }

        public static Type[] GetCraftingMaterials(TreasureLevel level, TreasurePackage package)
        {
            if (package == TreasurePackage.Artisan && level == TreasureLevel.Supply)
            {
                return _SpecialMaterialTable[(int)package];
            }

            return null;
        }

        public static Type[] GetDecorativeList(TreasureLevel level, TreasurePackage package, Map map)
        {
            Type[] list = null;
            var facet = GetFacet(map);

            if (level >= TreasureLevel.Cache)
            {
                list = _DecorativeTable[(int)package];

                if (facet == TreaureFacet.Malas)
                {
                    list.Concat(typeof(CoffinPiece));
                }
            }
            else if (level == TreasureLevel.Supply)
            {
                list = _DecorativeMinorArtifacts;
            }

            return list;
        }

        public static Type[] GetReagentList(TreasureLevel level, TreasurePackage package, Map map)
        {
            if (level != TreasureLevel.Stash || package != TreaurePackage.Mage)
                return null;

            var facet = GetFacet(map);

            switch (facet)
            {
                case TreasureFacet.Felucca:
                case TreasureFacet.Trammel: return Loot.RegTypes;
                case TreasureFacet.Malas: return Loot.NecroRegTypes;
                case TreasureFacet.TerMur: return Loot.MysticRegTypes;
            }

            return null;
        }

        public static Recipe[] GetRecipeList(TreasureLevel level, TreasurePackage package)
        {
            if (packet == TreasurePackage.Artisan && level == TreasureLevel.Supply)
            {
                return Recipe.Recipes.Values.ToArray();
            }

            return null;
        }

        public static Type[] GetSpecialLootList(TreasureLevel level, TreasurePackage package)
        {
            if (level == TreaureLevel.Stash)
                return null;

            Type[] list;

            if (level == TreasureLevel.Supply)
            {
                list = _SpecialSupplyLoot[(int)package];
            }
            else
            {
                list = _SpecialCacheHordeAndTrove;
            }

            if (package > TreasurePackage.Artisan)
            {
                list.Concat(_FunctionalMinorArtifacts);
            }

            return list;
        }

        /// <summary>
        /// Artisan
        /// Assassin
        /// Mage
        /// Ranger
        /// Warrior
        /// 
        /// Trammel, Felucca
        /// Ilshenar
        /// Malas
        /// Tokuno
        /// TerMur
        /// Eodon
        /// </summary>
        private static Type[][][] _WeaponTable = new Type[][][]
        {
            new Type[][] // Artisan
                {
                    new Type[] { typeof(HammerPick), typeof(SledgeHammerWeapon), typeof(SmithyHammer), typeof(WarAxe), typeof(WarHammer), typeof(Axe), typeof(BattleAxe), typeof(DoubleAxe), typeof(ExecutionersAxe), typeof(Hatchet), typeof(LargeBattleAxe), typeof(OrnateAxe), typeof(TwoHandedAxe), typeof(Pickaxe) }, // Trammel, Felucca
                    new Type[] { }, // Ilshenar
                    new Type[] { }, // Malas
                    new Type[] { }, // Tokuno
                    new Type[] { typeof(HammerPick), typeof(SledgeHammerWeapon), typeof(SmithyHammer), typeof(WarAxe), typeof(WarHammer), typeof(Axe), typeof(BattleAxe), typeof(DoubleAxe), typeof(ExecutionersAxe), typeof(Hatchet), typeof(LargeBattleAxe), typeof(OrnateAxe), typeof(TwoHandedAxe), typeof(Pickaxe), typeof(DualShortAxe) },  // TerMur
                    new Type[] {  }  // Eodon
                },
            new Type[][] // Assassin
                {
                    new Type[] { typeof(Dagger), typeof(Kryss), typeof(Cleaver), typeof(Cutlass), typeof(ElvenMachete) },
                    new Type[] { },
                    new Type[] { },
                    new Type[] { },
                    new Type[] { typeof(Dagger), typeof(Kryss), typeof(Cleaver), typeof(Cutlass) },
                    new Type[] { typeof(Dagger), typeof(Kryss), typeof(Cleaver), typeof(Cutlass), typeof(BladedWhip), typeof(BarbedWhip), typeof(SpikedWhip) },
                },
            new Type[][] // Mage
                {
                    new Type[] { typeof(BlackStaff), typeof(Crook), typeof(GnarledStaff), typeof(QuarterStaff) },
                    new Type[] { },
                    new Type[] { },
                    new Type[] { },
                    new Type[] { },
                    new Type[] { },
                },
            new Type[][] // Ranger
                {
                    new Type[] { typeof(Bow), typeof(Crossbow), typeof(HeavyCrossbow), typeof(CompositeBow), typeof(ButcherKnife), typeof(SkinningKnife) },
                    new Type[] { typeof(Bow), typeof(Crossbow), typeof(HeavyCrossbow), typeof(CompositeBow), typeof(ButcherKnife), typeof(SkinningKnife), typeof(SoulGlaive) },
                    new Type[] { typeof(Bow), typeof(Crossbow), typeof(HeavyCrossbow), typeof(CompositeBow), typeof(ButcherKnife), typeof(SkinningKnife), typeof(ElvenCompositeLongbow) },
                    new Type[] { },
                    new Type[] { typeof(Bow), typeof(Crossbow), typeof(HeavyCrossbow), typeof(CompositeBow), typeof(ButcherKnife), typeof(SkinningKnife), typeof(GargishButcherKnife), typeof(Cyclone), typeof(SoulGlaive) },
                    new Type[] { },
                },
            new Type[][] // Warrior
                {
                    new Type[] { typeof(Lance), typeof(Pike), typeof(Pitchfork), typeof(ShortSpear), typeof(WarFork), typeof(Club), typeof(Mace), typeof(Maul), typeof(WarMaul), typeof(Bardiche), typeof(Broadsword), typeof(CrescentBlade), typeof(Halberd), typeof(Longsword), typeof(Scmimitar), typeof(VikingSword) },
                    new Type[] { },
                    new Type[] { },
                    new Type[] { typeof(Lance), typeof(Pike), typeof(Pitchfork), typeof(ShortSpear), typeof(WarFork), typeof(Club), typeof(Mace), typeof(Maul), typeof(WarMaul), typeof(Bardiche), typeof(Broadsword), typeof(CrescentBlade), typeof(Halberd), typeof(Longsword), typeof(Scmimitar), typeof(VikingSword), typeof(Bokuto), typeof(Daisho) },
                    new Type[] { },
                    new Type[] { },
                },
        };

        private Type[][][] _ArmorTable = new Type[][][]
        {
            new Type[][] // Artisan
                {
                    new Type[] { typeof(Bonnet), typeof(Cap), typeof(Circlet), typeof(ElvenGlasses), typeof(FeatheredHat), typeof(FlowerGarland), typeof(JesterHat), typeof(Skullcap), typeof(StrawHat), typeof(TallStrawHat), typeof(WideBrimHat) }, // Trammel/Fel
                    new Type[] { }, // Ilshenar
                    new Type[] { }, // Malas
                    new Type[] { }, // Tokuno
                    new Type[] { }, // TerMur
                    new Type[] { typeof(Bonnet), typeof(Cap), typeof(Circlet), typeof(ElvenGlasses), typeof(FeatheredHat), typeof(FlowerGarland), typeof(JesterHat), typeof(Skullcap), typeof(StrawHat), typeof(TallStrawHat), typeof(WideBrimHat), typeof(ChefsToque) }, // Eodon
                },
            new Type[][] // Assassin
                {
                    new Type[] { typeof(ChainLegs), typeof(ChainCoif), typeof(ChainChest), typeof(RingailLegs), typeof(RingmailGloves), typeof(RingailChest), typeof(RingailArms), typeof(Bandana) }, // Trammel/Fel
                    new Type[] { }, // Ilshenar
                    new Type[] { }, // Malas
                    new Type[] { typeof(ChainLegs), typeof(ChainCoif), typeof(ChainChest), typeof(RingailLegs), typeof(RingmailGloves), typeof(RingailChest), typeof(RingailArms), typeof(Bandana), typeof(LeatherSuneate), typeof(LeatherMempo), typeof(LeatherJingasa), typeof(LeatherHiroSode), typeof(LeatherHaidate), typeof(LeatherDo) }, // Tokuno
                    new Type[] { }, // TerMur
                    new Type[] { }, // Eodon
                },
            new Type[][] // Mage
                {
                    new Type[] { typeof(LeafGloves), typeof(LeafLegs), typeof(LeafTonlet), typeof(LeafGorget), typeof(LeafArms),typeof(LeafChest), typeof(LeatherArms), typeof(LeatherChest), typeof(LeatherLegs), typeof(LeatherGloves), typeof(LeatherGorget), typeof(WizardsHat) }, // Trammel/Fel
                    new Type[] { }, // Ilshenar
                    new Type[] { typeof(LeafGloves), typeof(LeafLegs), typeof(LeafTonlet), typeof(LeafGorget), typeof(LeafArms),typeof(LeafChest), typeof(LeatherArms), typeof(LeatherChest), typeof(LeatherLegs), typeof(LeatherGloves), typeof(LeatherGorget), typeof(WizardsHat), typeof(BoneLegs), typeof(BoneHelm), typeof(BoneGloves), typeof(BoneChest), typeof(BoneArms) }, // Malas
                    new Type[] { }, // Tokuno
                    new Type[] { typeof(LeatherArms), typeof(LeatherChest), typeof(LeatherLegs), typeof(LeatherGloves), typeof(LeatherGorget), typeof(WizardsHat) }, // TerMur
                    new Type[] { typeof(LeatherArms), typeof(LeatherChest), typeof(LeatherLegs), typeof(LeatherGloves), typeof(LeatherGorget), typeof(WizardsHat) }, // Eodon
                },
            new Type[][] // Ranger
                {
                    new Type[] { typeof(HidePants), typeof(HidePauldrons), typeof(HideGorget), typeof(HideFemaleChest), typeof(HideChest), typeof(HideGloves), typeof(StuddedLegs), typeof(StuddedGorget), typeof(StuddedGloves), typeof(StuddedChest), typeof(StuddedBustierArms), typeof(StuddedArms), typeof(RavenHelm), typeof(VultureHelm), typeof(WingedHelm) }, // Trammel/Fel
                    new Type[] { }, // Ilshenar
                    new Type[] { }, // Malas
                    new Type[] { typeof(StuddedLegs), typeof(StuddedGorget), typeof(StuddedGloves), typeof(StuddedChest), typeof(StuddedBustierArms), typeof(StuddedArms) }, // Tokuno
                    new Type[] { typeof(HidePants), typeof(HidePauldrons), typeof(HideGorget), typeof(HideFemaleChest), typeof(HideChest), typeof(HideGloves), typeof(StuddedLegs), typeof(StuddedGorget), typeof(StuddedGloves), typeof(StuddedChest), typeof(StuddedBustierArms), typeof(StuddedArms), typeof(GargishLeatherKilt), typeof(GargishLeatherLegs), typeof(GargishLeatherArms), typeof(GargishLeatherChest) }, // TerMur
                    new Type[] { typeof(StuddedLegs), typeof(StuddedGorget), typeof(StuddedGloves), typeof(StuddedChest), typeof(StuddedBustierArms), typeof(StuddedArms), typeof(TigerPeltSkirt), typeof(TigerPeltShorts), typeof(TigerPeltLegs), typeof(TigerPeltLongSkirt), typeof(TigerPeltHelm), typeof(TigerPeltChest), typeof(TigerPeltCollar), typeof(TigerPeltBustier), typeof(VultureHelm), typeof(TribalMask) }, // Eodon
                },
            new Type[][] // Warrior
                {
                    new Type[] { typeof(PlateLegs), typeof(PlateHelm), typeof(PlateGorget), typeof(PlateGloves), typeof(PlateChest), typeof(PlateArms), typeof(Bascinet), typeof(CloseHelm), typeof(Helmet), typeof(LeatherCap), typeof(NorseHelm), typeof(TricornHat), typeof(BronzeShield), typeof(Buckler), typeof(ChaosShield), typeof(HeaterShield), typeof(MetalKiteShield), typeof(MetalShield), typeof(OrderShield), typeof(TearKiteShield) }, // Trammel/Fel
                    new Type[] { }, // Ilshenar
                    new Type[] { typeof(PlateLegs), typeof(PlateHelm), typeof(PlateGorget), typeof(PlateGloves), typeof(PlateChest), typeof(PlateArms), typeof(Bascinet), typeof(CloseHelm), typeof(Helmet), typeof(LeatherCap), typeof(NorseHelm), typeof(TricornHat), typeof(BronzeShield), typeof(Buckler), typeof(ChaosShield), typeof(HeaterShield), typeof(MetalKiteShield), typeof(MetalShield), typeof(OrderShield), typeof(TearKiteShield), typeof(DragonHelm), typeof(DragonGloves), typeof(DragonChest), typeof(DragonArms), typeof(DragonLegs) }, // Malas
                    new Type[] { typeof(PlateLegs), typeof(PlateHelm), typeof(PlateGorget), typeof(PlateGloves), typeof(PlateChest), typeof(PlateArms), typeof(Bascinet), typeof(CloseHelm), typeof(Helmet), typeof(LeatherCap), typeof(NorseHelm), typeof(TricornHat), typeof(BronzeShield), typeof(Buckler), typeof(ChaosShield), typeof(HeaterShield), typeof(MetalKiteShield), typeof(MetalShield), typeof(OrderShield), typeof(TearKiteShield), typeof(PlateSuneate), typeof(PlateMempo), typeof(PlateHiroSode), typeof(PlateHatsuburi), typeof(Haidate), typeof(PlateDo), typeof(PlateBattleDo), typeof(DecorativePlateKabuto), typeof(LightPlateJingasa), typeof(SmallPlateJingasa)  }, // Tokuno
                    new Type[] { typeof(PlateLegs), typeof(PlateHelm), typeof(PlateGorget), typeof(PlateGloves), typeof(PlateChest), typeof(PlateArms), typeof(Bascinet), typeof(CloseHelm), typeof(Helmet), typeof(LeatherCap), typeof(NorseHelm), typeof(TricornHat), typeof(BronzeShield), typeof(Buckler), typeof(ChaosShield), typeof(HeaterShield), typeof(MetalKiteShield), typeof(MetalShield), typeof(OrderShield), typeof(TearKiteShield), typeof(GargishPlateArms), typeof(GargishPlateChest), typeof(GargishPlateKilt), typeof(GargishPlateLegs), typeof(GargishStoneKilt), typeof(GargishStoneLegs), typeof(GargishStoneArms), typeof(GargishStoneChest) }, // TerMur
                    new Type[] { typeof(PlateLegs), typeof(PlateHelm), typeof(PlateGorget), typeof(PlateGloves), typeof(PlateChest), typeof(PlateArms), typeof(Bascinet), typeof(CloseHelm), typeof(Helmet), typeof(LeatherCap), typeof(NorseHelm), typeof(TricornHat), typeof(BronzeShield), typeof(Buckler), typeof(ChaosShield), typeof(HeaterShield), typeof(MetalKiteShield), typeof(MetalShield), typeof(OrderShield), typeof(TearKiteShield), typeof(DragonTurtleHideHelm), typeof(DragonTurtleHideLegs), typeof(DragonTurtleHideChest), typeof(DragonTurtleHideBustier), typeof(DragonTurtleHideArms) }, // Eodon
                }
        };

        public Type[] _MaterialTable = new Type[] { typeof(BarbedLeather), typeof(BloodwoodBoard), typeof(FrostwoodBoard), typeof(ValoriteIngot), typeof(VeriteIngot) };

        public Type[][] _JewelTable = new Type[][]
            {
                new Type[] { typeof(GoldRing), typeof(GoldBracelet), typeof(SilverRing), typeof(SilverBracelet) }, // standard
                new Type[] { typeof(GoldRing), typeof(GoldBracelet), typeof(SilverRing), typeof(SilverBracelet), typeof(GargishBracelet) }, // Ranger/TerMur
            };

        public Type[][] _DecorativeTable = new Type[][]
            {
                new Type[] { typeof(SkullTiledFloorDeed) },
                new Type[] { typeof(AncientWeapon3) },
                new Type[] { typeof(DecorativeHourglass) },
                new Type[] { typeof(AncientWeapon1), typeof(CreepingVine) },
                new Type[] { typeof(AncientWeapon2) },
            };

        public Type[][] _SpecialMaterialTable = new Type[][]
            {
                new Type[] { },
                new Type[] { },
                new Type[] { },
                new Type[] { typeof(LuminescentFungi) },
                new Type[] { typeof(EssenceAchievement), typeof(EssenceBalance), typeof(EssenceControl), typeof(EssenceDiligence), typeof(EssenceDirection), typeof(EssenceFeeling), typeof(EssenceOrder), typeof(EssencePassion), typeof(EssencePrecision), typeof(EssenceSingularity) },
                new Type[] { },
                new Type[] { },
            };

        public Type[][] _SpecialSupplyLoot = new Type[][]
            {
                new Type[] { typeof(LegendaryMapMakersGlasses), typeof(ManaPhasingOrb), typeof(RunedSashOfWarding), typeof(ShieldEngravingTool) },
                new Type[] { typeof(ForgedPardon), typeof(LegendaryMapMakersGlasses), typeof(ManaPhasingOrb), typeof(RunedSashOfWarding), typeof(SkeletonKey), typeof(MasterSkeletonKey), typeof(SurgeShield) },
                new Type[] { typeof(LegendaryMapMakersGlasses), typeof(ManaPhasingOrb), typeof(RunedSashOfWarding) },
                new Type[] { typeof(LegendaryMapMakersGlasses), typeof(ManaPhasingOrb), typeof(RunedSashOfWarding), typeof(TastyTreat) },
                new Type[] { typeof(LegendaryMapMakersGlasses), typeof(ManaPhasingOrb), typeof(RunedSashOfWarding) },
            };

        public Type[] _SpecialCacheHordeAndTrove = new Type[]
            {
                typeof(OctopusNecklace), typeof(SkullGnarledStaff), typeof(SkullLongsword)
            };

        public Type[] _DecorativeMinorArtifacts = new Type[]
            {
                typeof(CandelabraOfSouls), typeof(GoldBricks), typeof(PhillipsWoodenSteed), typeof(AncientShipModelOfTheHMSCape), typeof(AdmiralsHeartyRum)
            };

        public Type[] _FunctionalMinorArtifacts = new Type[]
            {
                typeof(ArcticDeathDealer), typeof(BlazeOfDeath), typeof(BurglarsBandana),
                typeof(CavortingClub), typeof(DreadPirateHat),
                typeof(EnchantedTitanLegBone), typeof(GwennosHarp), typeof(IolosLute),
                typeof(LunaLance), typeof(NightsKiss), typeof(NoxRangersHeavyCrossbow),
                typeof(PolarBearMask), typeof(VioletCourage), typeof(HeartOfTheLion),
                typeof(ColdBlood), typeof(AlchemistsBauble), typeof(CaptainQuacklebushsCutlass),
                typeof(ShieldOfInvulnerability),
            };

        public SkillName[][] _TranscendenceTable = new Type[][]
            {
                new SkillName[] { SkillName.ArmsLore, SkillName.BlackSmithy, SkillName.Carpentry, SkillName.Cartography, SkillName.Cooking, SkillName.Cooking, SkillName.Fletching, SkillName.Mining, SkillName.Tailoring },
                new SkillName[] { SkillName.Anatomy, SkillName.DetectHidden, SkillName.Fencing, SkillName.Poisoning, SkillName.RemoveTrap, SkillName.Snooping, SkillName.Stealth },
                new SkillName[] { SkillName.Magery, SkillName.Meditation, SkillName.ResistingSpells, SkillName.Spellweaving },
                new SkillName[] { SkillName.Alchemy, SkillName.AnimalLore, SkillName.AnimalTaming, SkillName.Archery, },
                new SkillName[] { SkillName.Chivalry, SkillName.Focus, SkillName.Parry, SkillName.Swords, SkillName.Tactics, SkillName.Wrestling },
            };

        public SkillName[][] _AlacrityTable = new Type[][]
           {
                new SkillName[] { SkillName.ArmsLore, SkillName.BlackSmithy, SkillName.Carpentry, SkillName.Cartography, SkillName.Cooking, SkillName.Cooking, SkillName.Fletching, SkillName.Mining, SkillName.Tailoring, SkillName.Lumberjacking },
                new SkillName[] { SkillName.DetectHidden, SkillName.Fencing, SkillName.Hiding, SkillName.Lockpicking, SkillName.Poisoning, SkillName.RemoveTrap, SkillName.Snooping, SkillName.Stealing, SkillName.Stealth },
                new SkillName[] { SkillName.Alchemy, SkillName.EvalInt, SkillName.Inscription, SkillName.Magery, SkillName.Meditation, SkillName.Spellweaving, SkillName.SpiritSpeak },
                new SkillName[] { SkillName.AnimalLore, SkillName.AnimalTaming, SkillName.Archery, SkillName.Musicianship, SkillName.Peacemaking, SkillName.Provocation, SkillName.Tinkering, SkillName.Tracking, SkillName.Veterinary },
                new SkillName[] { SkillName.Chivalry, SkillName.Focus, SkillName.MaceFighting, SkillName.Parry, SkillName.Swords, SkillName.Wrestling },
           };

        public SkillName[][] _PowerscrollTable = new Typep[][]
            {
                new SkillName[] { },
                new SkillName[] { SkillName.Ninjitsu },
                new SkillName[] { SkillName.Magery, SkillName.Meditation, SkillName.Mysticism, SkillName.Spellweaving, SkillName.SpiritSpeak },
                new SkillName[] { SkillName.AnimalTaming, SkillName.Discordance, SkillName.Provocation, SkillName.Veterinary },
                new SkillName[] { SkillName.Bushido, SkillName.Chivalry, SkillName.Focus, SkillName.Healing, SkillName.Parry, SkillName.Swords, SkillName.Tactics },
            };

        public static void Fill(Mobile from, TreasureMapChest chest, TreasureMap map)
        {
            chest.Movable = false;
            chest.Locked = true;

            chest.TrapType = TrapType.ExplosionTrap;
            chest.TrapPower = level * 25;
            chest.TrapLevel = level;

            switch (level)
            {
                case 1: chest.RequiredSkill = 36; break;
                case 2: chest.RequiredSkill = 60; break;
                case 3: chest.RequiredSkill = 76; break;
                case 4: chest.RequiredSkill = 75; break;
                case 5: chest.RequiredSkill = 80; break;
            }

            chest.LockLevel = cont.RequiredSkill - 10;
            chest.MaxLockLevel = cont.RequiredSkill + 40;

            #region Gold
            // TODO Confirm
            chest.DropItem(new Gold(isSos ? level * 10000 : level * 5500));
            #endregion

            #region Refinements
            if (map.TreasureLevel == TreasureLevel.Stash)
            {
                RefinementComponent.Roll(chest, rolls, 0.10);
            }
            #endregion

            #region Cartography Glasses
            if (map.TreasureLevel == TreasureLevel.Supply && 0.05 > Utility.RandomDouble())
            {
                chest.DropItem(new LegendaryMapmakersGlasses());
            }
            #endregion

            #region TMaps
            if (map.TreasureLevel < TreasureLevel.Trove && 0.1 > Utility.RandomDouble())
            {
                chest.DropItem(new TreasureMap(map.Level + 1, map));
            }
            #endregion
        }
    }

    public class RemoveTrapTimer : Timer
    {
        public Mobile From { get; set; }
        public TreaureMapChest Chest { get; set; }
        public DateTime EndTime { get; set; }

        public RemoveTrapTimer(Mobile from, TreasureMapChest chest, TimeSpan duration)
            : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
        {
            From = from;
            Chest = chest;
            EndTime = TimeSpan.UtcNow + duration;
        }

        protected override void OnTick()
        {
            if (EndTime < DateTime.UtcNow)
            {
            }
            else
            {

            }
        }
    }
}

