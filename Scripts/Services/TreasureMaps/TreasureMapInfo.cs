using System;

using Server;

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

    public static class TreasureMapInfo
    {
        public static void Initialize()
        {
        }

        public static TreasureFacet GetFacet(IEntity e)
        {
            if (e.Map == Map.TerMur)
            {
                if (SpellHelper.IsEodon(e.Location, e.Map))
                {
                    return TreasureFacet.Eodon;
                }

                return TreasureFacet.TerMur;
            }

            if (e.Map == Map.Felucca)
            {
                return TreasureFacet.Felucca;
            }

            if (e.Map == Map.Trammel)
            {
                return TreasureFacet.Trammel;
            }

            if (e.Map == Map.Malas)
            {
                return TreasureFacet.Malas;
            }

            if (e.Map == Map.Ilshenar)
            {
                return TreasureFacet.Ilshenar;
            }

            if (e.Map == Map.Tokuno)
            {
                return TreasureFacet.Tokuno;
            }
        }

        public static Type[] GetWeaponList(TreasurePackage package, Map map)
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

        public static Type[] GetArmorList(TreasurePackage package, Map map)
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

            if (package == TreasurePackage.Ranger && facet == TreasureFacet.TerMur)
            {
                if (level >= TreasureLevel.Cache)
                {
                    return _JewelTable[3];
                }
                else
                {
                    return _JewelTable[2];
                }
            }
            else
            {
                if (level >= TreasureLevel.Cache)
                {
                    return _JewelTable[1];
                }
                else
                {
                    return _JewelTable[0];
                }
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

        public static Type[] GetDecorativeList(TreasureLevel level, TreasurePackage package)
        {
            if (level >= TreasureLevel.Cache)
            {
                return _DecorativeTable[(int)package];
            }

            return null;
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
                    new Type[] { typeof(Dagger), typeof(Kryss), typeof(Cleaver), typeof(Cutlass) },
                    new Type[] { typeof(Dagger), typeof(Kryss), typeof(Cleaver), typeof(Cutlass) },
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
                    new Type[] { typeof(Bow), typeof(Crossbow), typeof(HeavyCrossbow), typeof(CompositeBow), typeof(SkullGnarledStaff), typeof(ButcherKnife), typeof(SkinningKnife) },
                    new Type[] { typeof(Bow), typeof(Crossbow), typeof(HeavyCrossbow), typeof(CompositeBow), typeof(SkullGnarledStaff), typeof(ButcherKnife), typeof(SkinningKnife), typeof(SoulGlaive) },
                    new Type[] { typeof(Bow), typeof(Crossbow), typeof(HeavyCrossbow), typeof(CompositeBow), typeof(SkullGnarledStaff), typeof(ButcherKnife), typeof(SkinningKnife), typeof(ElvenCompositeLongbow) },
                    new Type[] { },
                    new Type[] { typeof(Bow), typeof(Crossbow), typeof(HeavyCrossbow), typeof(CompositeBow), typeof(SkullGnarledStaff), typeof(ButcherKnife), typeof(SkinningKnife), typeof(GargishButcherKnife), typeof(Cyclone), typeof(SoulGlaive) },
                    new Type[] { },
                },
            new Type[][] // Warrior
                {
                    new Type[] { typeof(Lance), typeof(Pike), typeof(Pitchfork), typeof(ShortSpear), typeof(WarFork), typeof(Club), typeof(Mace), typeof(Maul), typeof(SkullGnarledStaff), typeof(WarMaul), typeof(Bardiche), typeof(Broadsword), typeof(CrescentBlade), typeof(Halberd), typeof(Longsword), typeof(Scmimitar), typeof(VikingSword) },
                    new Type[] { },
                    new Type[] { },
                    new Type[] { typeof(Lance), typeof(Pike), typeof(Pitchfork), typeof(ShortSpear), typeof(WarFork), typeof(Club), typeof(Mace), typeof(Maul), typeof(SkullGnarledStaff), typeof(WarMaul), typeof(Bardiche), typeof(Broadsword), typeof(CrescentBlade), typeof(Halberd), typeof(Longsword), typeof(Scmimitar), typeof(VikingSword), typeof(Bokuto), typeof(Daisho) },
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

                new Type[] { typeof(GoldRing), typeof(GoldBracelet), typeof(SilverRing), typeof(SilverBracelet), typeof(OctopusNecklace) }, // cache+
                new Type[] { typeof(GoldRing), typeof(GoldBracelet), typeof(SilverRing), typeof(SilverBracelet), typeof(GargishBracelet), typeof(OctopusNecklace) } // Ranger/TerMur/cache+
            };

        public Type[][] _DecorationTable = new Type[][]
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

        public Type[][] _UtilityItemTable = new Type[][]
            {
                new Type[] { typeof(ShieldEngravingTool) },
                new Type[] { typeof(ForgedPardon), typeof(SkeletonKey) },
                new Type[] { },
                new Type[] { typeof(TastyTreat) },
                new Type[] { },
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

        /*private Type[][][] _ArmorTable = new Type[][][]
        {
            new Type[][] // Artisan
                {
                    new Type[] { }, // Trammel/Fel
                    new Type[] { }, // Ilshenar
                    new Type[] { }, // Malas
                    new Type[] { }, // Tokuno
                    new Type[] { }, // TerMur
                    new Type[] { }, // Eodon
                },
            new Type[][] // Assassin
                {
                    new Type[] { }, // Trammel/Fel
                    new Type[] { }, // Ilshenar
                    new Type[] { }, // Malas
                    new Type[] { }, // Tokuno
                    new Type[] { }, // TerMur
                    new Type[] { }, // Eodon
                },
            new Type[][] // Mage
                {
                    new Type[] { }, // Trammel/Fel
                    new Type[] { }, // Ilshenar
                    new Type[] { }, // Malas
                    new Type[] { }, // Tokuno
                    new Type[] { }, // TerMur
                    new Type[] { }, // Eodon
                },
            new Type[][] // Ranger
                {
                    new Type[] { }, // Trammel/Fel
                    new Type[] { }, // Ilshenar
                    new Type[] { }, // Malas
                    new Type[] { }, // Tokuno
                    new Type[] { }, // TerMur
                    new Type[] { }, // Eodon
                },
            new Type[][] // Warrior
                {
                    new Type[] { }, // Trammel/Fel
                    new Type[] { }, // Ilshenar
                    new Type[] { }, // Malas
                    new Type[] { }, // Tokuno
                    new Type[] { }, // TerMur
                    new Type[] { }, // Eodon
                }
        };*/
    }

    public class TreasureEntry
    {
        public Type[] Weapons { get; set; }
        public Type[] Armor { get; set; }
        public Type[] Jewels { get; set; }
        public SkillName[] AlacritySkills { get; set; }
        public SkillName[] TranscendenceSkills { get; set; }
        public Type[] Special { get; set; }
        public Type[] Resources { get; set; }
        public Type[] Decoration { get; set; }
        public Type[] Artifacts { get; set; }

        public TreasureEntry()
        {
        }
    }
}

