#region References
using Server.Items;
using System;
#endregion

namespace Server
{
    public class Loot
    {
        #region List definitions

        #region SA equipment
		private static readonly Type[] m_SAJewelryTypes =
        {
            typeof(GargishRing), typeof(GargishBracelet)
        };
        public static Type[] SAJewelryTypes => m_SAJewelryTypes;
		
		public static readonly Type[] m_SAShieldTypes =
        {
            typeof(GargishChaosShield), typeof(GargishKiteShield), typeof(GargishOrderShield), typeof(GargishWoodenShield),
            typeof(LargeStoneShield)
        };
        public static Type[] SAShieldTypes => m_SAShieldTypes;
		
        private static readonly Type[] m_SAWeaponTypes =
        {
            typeof(DiscMace), typeof(GargishTalwar), typeof(Shortblade), typeof(DualPointedSpear), typeof(GlassStaff),
            typeof(StoneWarSword), typeof(DualShortAxes), typeof(GlassSword), typeof(GargishDagger)
        };
        public static Type[] SAWeaponTypes => m_SAWeaponTypes;

        private static readonly Type[] m_SARangedWeaponTypes = { typeof(Boomerang), typeof(Cyclone), typeof(SoulGlaive)};
        public static Type[] SARangedWeaponTypes => m_SARangedWeaponTypes;

        private static readonly Type[] m_SAArmorTypes =
        {
            typeof(GargishLeatherChest), typeof(GargishLeatherLegs), typeof(GargishLeatherArms), typeof(GargishLeatherKilt),
            typeof(GargishStoneChest), typeof(GargishStoneLegs), typeof(GargishStoneArms),
            typeof(GargishStoneKilt), typeof(GargishPlateChest), typeof(GargishPlateLegs), typeof(GargishPlateArms),
            typeof(GargishPlateKilt), typeof(GargishNecklace), typeof( GargishEarrings )
        };
        public static Type[] SAArmorTypes => m_SAArmorTypes;

        private static readonly Type[] m_SAClothingTypes =
        {
            typeof(GargishClothChestArmor), typeof(GargishClothArmsArmor), typeof(GargishClothKiltArmor), typeof(GargishClothLegsArmor)
        };
        public static Type[] SAClothingTypes => m_SAClothingTypes;
        #endregion

        #region ML equipment
        private static readonly Type[] m_MLWeaponTypes =
        {
            typeof(AssassinSpike), typeof(DiamondMace), typeof(ElvenMachete), typeof(ElvenSpellblade), typeof(Leafblade),
            typeof(OrnateAxe), typeof(RadiantScimitar), typeof(RuneBlade), typeof(WarCleaver), typeof(WildStaff)
        };
        public static Type[] MLWeaponTypes => m_MLWeaponTypes;

        private static readonly Type[] m_MLRangedWeaponTypes = { typeof(ElvenCompositeLongbow), typeof(MagicalShortbow) };
        public static Type[] MLRangedWeaponTypes => m_MLRangedWeaponTypes;

        private static readonly Type[] m_MLArmorTypes =
        {
            typeof(Circlet), typeof(GemmedCirclet), typeof(LeafTonlet), typeof(RavenHelm), typeof(RoyalCirclet),
            typeof(VultureHelm), typeof(WingedHelm), typeof(LeafArms), typeof(LeafChest), typeof(LeafGloves), typeof(LeafGorget),
            typeof(LeafLegs), typeof(WoodlandArms), typeof(WoodlandChest), typeof(WoodlandGloves), typeof(WoodlandGorget),
            typeof(WoodlandLegs), typeof(HideChest), typeof(HideGloves), typeof(HideGorget), typeof(HidePants),
            typeof(HidePauldrons)
        };
        public static Type[] MLArmorTypes => m_MLArmorTypes;

        private static readonly Type[] m_MLClothingTypes =
        {
            typeof(MaleElvenRobe), typeof(FemaleElvenRobe), typeof(ElvenPants), typeof(ElvenShirt), typeof(ElvenDarkShirt),
            typeof(ElvenBoots), typeof(VultureHelm), typeof(WoodlandBelt)
        };
        public static Type[] MLClothingTypes => m_MLClothingTypes;
        #endregion

		#region SE equipment
		private static readonly Type[] m_SEArmorTypes =
        {
            typeof(ChainHatsuburi), typeof(LeatherDo), typeof(LeatherHaidate), typeof(LeatherHiroSode), typeof(LeatherJingasa),
            typeof(LeatherMempo), typeof(LeatherNinjaHood), typeof(LeatherNinjaJacket), typeof(LeatherNinjaMitts),
            typeof(LeatherNinjaPants), typeof(LeatherSuneate), typeof(DecorativePlateKabuto), typeof(HeavyPlateJingasa),
            typeof(LightPlateJingasa), typeof(PlateBattleKabuto), typeof(PlateDo), typeof(PlateHaidate), typeof(PlateHatsuburi),
            typeof(PlateHiroSode), typeof(PlateMempo), typeof(PlateSuneate), typeof(SmallPlateJingasa),
            typeof(StandardPlateKabuto), typeof(StuddedDo), typeof(StuddedHaidate), typeof(StuddedHiroSode), typeof(StuddedMempo)
            , typeof(StuddedSuneate)
        };
        public static Type[] SEArmorTypes => m_SEArmorTypes;
		
		private static readonly Type[] m_SEClothingTypes =
        {
            typeof(ClothNinjaJacket), typeof(FemaleKimono), typeof(Hakama), typeof(HakamaShita), typeof(JinBaori),
            typeof(Kamishimo), typeof(MaleKimono), typeof(NinjaTabi), typeof(Obi), typeof(SamuraiTabi), typeof(TattsukeHakama),
            typeof(Waraji)
        };
        public static Type[] SEClothingTypes => m_SEClothingTypes;
		
		private static readonly Type[] m_SEHatTypes = { typeof(ClothNinjaHood), typeof(Kasa) };
        public static Type[] SEHatTypes => m_SEHatTypes;
		
        private static readonly Type[] m_SEWeaponTypes =
        {
            typeof(Bokuto), typeof(Daisho), typeof(Kama), typeof(Lajatang), typeof(NoDachi), typeof(Nunchaku), typeof(Sai),
            typeof(Tekagi), typeof(Tessen), typeof(Tetsubo), typeof(Wakizashi)
        };
        public static Type[] SEWeaponTypes => m_SEWeaponTypes;

        private static readonly Type[] m_SERangedWeaponTypes = { typeof(Yumi) };
        public static Type[] SERangedWeaponTypes => m_SERangedWeaponTypes;
		#endregion
		
		#region Normal equipment
		private static readonly Type[] m_ClothingTypes =
        {
            typeof(Cloak), typeof(Bonnet), typeof(Cap), typeof(FeatheredHat), typeof(FloppyHat), typeof(JesterHat),
            typeof(Surcoat), typeof(SkullCap), typeof(StrawHat), typeof(TallStrawHat), typeof(TricorneHat), typeof(WideBrimHat),
            typeof(WizardsHat), typeof(BodySash), typeof(Doublet), typeof(Boots), typeof(FullApron), typeof(JesterSuit),
            typeof(Sandals), typeof(Tunic), typeof(Shoes), typeof(Shirt), typeof(Kilt), typeof(Skirt), typeof(FancyShirt),
            typeof(FancyDress), typeof(ThighBoots), typeof(LongPants), typeof(PlainDress), typeof(Robe), typeof(ShortPants),
            typeof(HalfApron), typeof(FurSarong), typeof(FurCape), typeof(FlowerGarland), typeof(GildedDress), typeof(FurBoots), 
			typeof(FormalShirt)
        };
        public static Type[] ClothingTypes => m_ClothingTypes;
		
		private static readonly Type[] m_HatTypes =
        {
            typeof(SkullCap), typeof(Bandana), typeof(FloppyHat), typeof(Cap), typeof(WideBrimHat), typeof(StrawHat),
            typeof(TallStrawHat), typeof(WizardsHat), typeof(Bonnet), typeof(FeatheredHat), typeof(TricorneHat),
            typeof(JesterHat), typeof(OrcMask), typeof(TribalMask), typeof(FlowerGarland), typeof(BearMask), typeof(DeerMask)
        };
        public static Type[] HatTypes => m_HatTypes;
		
		private static readonly Type[] m_WeaponTypes =
        {
            typeof(Axe), typeof(BattleAxe), typeof(DoubleAxe), typeof(ExecutionersAxe), typeof(Hatchet), typeof(LargeBattleAxe),
            typeof(TwoHandedAxe), typeof(WarAxe), typeof(Club), typeof(Mace), typeof(Maul), typeof(WarHammer), typeof(WarMace),
            typeof(Bardiche), typeof(Halberd), typeof(Spear), typeof(ShortSpear), typeof(Pitchfork), typeof(WarFork),
            typeof(BlackStaff), typeof(GnarledStaff), typeof(QuarterStaff), typeof(Broadsword), typeof(Cutlass), typeof(Katana),
            typeof(Kryss), typeof(Longsword), typeof(Scimitar), typeof(VikingSword), typeof(Pickaxe), typeof(HammerPick),
            typeof(ButcherKnife), typeof(Cleaver), typeof(Dagger), typeof(SkinningKnife), typeof(ShepherdsCrook),
			typeof(Scythe), typeof(BoneHarvester), typeof(Scepter), typeof(BladedStaff), typeof(Pike), typeof(DoubleBladedStaff),
            typeof(Lance), typeof(CrescentBlade), typeof(SmithyHammer), typeof(SledgeHammerWeapon)
        };	
        public static Type[] WeaponTypes => m_WeaponTypes;

        private static readonly Type[] m_RangedWeaponTypes =
        { 
			typeof(Bow), typeof(Crossbow), typeof(HeavyCrossbow), typeof(CompositeBow), typeof(RepeatingCrossbow)
		};
        public static Type[] RangedWeaponTypes => m_RangedWeaponTypes;

        private static readonly Type[] m_ArmorTypes =
        {
            typeof(BoneArms), typeof(BoneChest), typeof(BoneGloves), typeof(BoneLegs), typeof(BoneHelm), typeof(ChainChest),
            typeof(ChainLegs), typeof(ChainCoif), typeof(Bascinet), typeof(CloseHelm), typeof(Helmet), typeof(NorseHelm),
            typeof(OrcHelm), typeof(FemaleLeatherChest), typeof(LeatherArms), typeof(LeatherBustierArms), typeof(LeatherChest),
            typeof(LeatherGloves), typeof(LeatherGorget), typeof(LeatherLegs), typeof(LeatherShorts), typeof(LeatherSkirt),
            typeof(LeatherCap), typeof(FemalePlateChest), typeof(PlateArms), typeof(PlateChest), typeof(PlateGloves),
            typeof(PlateGorget), typeof(PlateHelm), typeof(PlateLegs), typeof(RingmailArms), typeof(RingmailChest),
            typeof(RingmailGloves), typeof(RingmailLegs), typeof(FemaleStuddedChest), typeof(StuddedArms),
            typeof(StuddedBustierArms), typeof(StuddedChest), typeof(StuddedGloves), typeof(StuddedGorget), typeof(StuddedLegs)
        };
        public static Type[] ArmorTypes => m_ArmorTypes;
		
		private static readonly Type[] m_JewelryTypes =
        {
            typeof(GoldRing), typeof(GoldBracelet), typeof(SilverRing), typeof(SilverBracelet)
        };
        public static Type[] JewelryTypes => m_JewelryTypes;

        private static readonly Type[] m_ShieldTypes =
        {
            typeof(BronzeShield), typeof(Buckler), typeof(HeaterShield), typeof(MetalShield), typeof(MetalKiteShield),
            typeof(WoodenKiteShield), typeof(WoodenShield), typeof(ChaosShield), typeof(OrderShield)
        };
        public static Type[] ShieldTypes => m_ShieldTypes;
		#endregion
		
        private static readonly Type[] m_GemTypes =
        {
            typeof(Amber), typeof(Amethyst), typeof(Citrine), typeof(Diamond), typeof(Emerald), typeof(Ruby), typeof(Sapphire),
            typeof(StarSapphire), typeof(Tourmaline)
        };

        public static Type[] GemTypes => m_GemTypes;

        private static readonly Type[] m_RareGemTypes =
        {
            typeof(BlueDiamond), typeof(DarkSapphire), typeof(EcruCitrine), typeof(FireRuby), typeof(PerfectEmerald), typeof(Turquoise), typeof(WhitePearl), typeof(BrilliantAmber)
        };

        public static Type[] RareGemTypes => m_RareGemTypes;

        private static readonly Type[] m_MLResources =
		{
            typeof(BlueDiamond), typeof(DarkSapphire), typeof(EcruCitrine), typeof(FireRuby), typeof(PerfectEmerald), typeof(Turquoise), typeof(WhitePearl), typeof(BrilliantAmber),
            typeof(LuminescentFungi), typeof(BarkFragment), typeof(SwitchItem), typeof(ParasiticPlant)
        };

        public static Type[] MLResources => m_MLResources;

        public static Type[] RegTypes => m_RegTypes;
        private static readonly Type[] m_RegTypes =
        {
            typeof(BlackPearl), typeof(Bloodmoss), typeof(Garlic), typeof(Ginseng), typeof(MandrakeRoot), typeof(Nightshade),
            typeof(SulfurousAsh), typeof(SpidersSilk)
        };

        public static Type[] NecroRegTypes => m_NecroRegTypes;
        private static readonly Type[] m_NecroRegTypes = { typeof(BatWing), typeof(GraveDust), typeof(DaemonBlood), typeof(NoxCrystal), typeof(PigIron) };

        public static Type[] MysticRegTypes => m_MysticRegTypes;
        private static readonly Type[] m_MysticRegTypes = { typeof(Bone), typeof(DragonBlood), typeof(FertileDirt), typeof(DaemonBone) };

        private static readonly Type[] m_PotionTypes =
        {
            typeof(AgilityPotion), typeof(StrengthPotion), typeof(RefreshPotion), typeof(LesserCurePotion),
            typeof(LesserHealPotion), typeof(LesserPoisonPotion)
        };

        public static Type[] PotionTypes => m_PotionTypes;

        private static readonly Type[] m_ImbuingEssenceIngreds =
        {
            typeof(EssencePrecision), typeof(EssenceAchievement), typeof(EssenceBalance), typeof(EssenceControl), typeof(EssenceDiligence),
            typeof(EssenceDirection),   typeof(EssenceFeeling), typeof(EssenceOrder),   typeof(EssencePassion),   typeof(EssencePersistence),
            typeof(EssenceSingularity)
        };

        public static Type[] ImbuingEssenceIngreds => m_ImbuingEssenceIngreds;

        private static readonly Type[] m_SEInstrumentTypes = { typeof(BambooFlute) };

        public static Type[] SEInstrumentTypes => m_SEInstrumentTypes;

        private static readonly Type[] m_InstrumentTypes = { typeof(Drums), typeof(Harp), typeof(LapHarp), typeof(Lute), typeof(Tambourine), typeof(TambourineTassel) };

        public static Type[] InstrumentTypes => m_InstrumentTypes;

        private static readonly Type[] m_StatueTypes =
        {
            typeof(StatueSouth), typeof(StatueSouth2), typeof(StatueNorth), typeof(StatueWest), typeof(StatueEast),
            typeof(StatueEast2), typeof(StatueSouthEast), typeof(BustSouth), typeof(BustEast)
        };

        public static Type[] StatueTypes => m_StatueTypes;

        #region Spell Scrolls
        private static readonly Type[] m_MageryScrollTypes =
        {
            typeof(ReactiveArmorScroll), typeof(ClumsyScroll), typeof(CreateFoodScroll), typeof(FeeblemindScroll),
            typeof(HealScroll), typeof(MagicArrowScroll), typeof(NightSightScroll), typeof(WeakenScroll), typeof(AgilityScroll),
            typeof(CunningScroll), typeof(CureScroll), typeof(HarmScroll), typeof(MagicTrapScroll), typeof(MagicUnTrapScroll),
            typeof(ProtectionScroll), typeof(StrengthScroll), typeof(BlessScroll), typeof(FireballScroll),
            typeof(MagicLockScroll), typeof(PoisonScroll), typeof(TelekinisisScroll), typeof(TeleportScroll),
            typeof(UnlockScroll), typeof(WallOfStoneScroll), typeof(ArchCureScroll), typeof(ArchProtectionScroll),
            typeof(CurseScroll), typeof(FireFieldScroll), typeof(GreaterHealScroll), typeof(LightningScroll),
            typeof(ManaDrainScroll), typeof(RecallScroll), typeof(BladeSpiritsScroll), typeof(DispelFieldScroll),
            typeof(IncognitoScroll), typeof(MagicReflectScroll), typeof(MindBlastScroll), typeof(ParalyzeScroll),
            typeof(PoisonFieldScroll), typeof(SummonCreatureScroll), typeof(DispelScroll), typeof(EnergyBoltScroll),
            typeof(ExplosionScroll), typeof(InvisibilityScroll), typeof(MarkScroll), typeof(MassCurseScroll),
            typeof(ParalyzeFieldScroll), typeof(RevealScroll), typeof(ChainLightningScroll), typeof(EnergyFieldScroll),
            typeof(FlamestrikeScroll), typeof(GateTravelScroll), typeof(ManaVampireScroll), typeof(MassDispelScroll),
            typeof(MeteorSwarmScroll), typeof(PolymorphScroll), typeof(EarthquakeScroll), typeof(EnergyVortexScroll),
            typeof(ResurrectionScroll), typeof(SummonAirElementalScroll), typeof(SummonDaemonScroll),
            typeof(SummonEarthElementalScroll), typeof(SummonFireElementalScroll), typeof(SummonWaterElementalScroll)
        };

        private static readonly Type[] m_NecromancyScrollTypes =
        {
            typeof(AnimateDeadScroll), typeof(BloodOathScroll), typeof(CorpseSkinScroll), typeof(CurseWeaponScroll),
            typeof(EvilOmenScroll), typeof(HorrificBeastScroll), typeof(LichFormScroll), typeof(MindRotScroll),
            typeof(PainSpikeScroll), typeof(PoisonStrikeScroll), typeof(StrangleScroll), typeof(SummonFamiliarScroll),
            typeof(VampiricEmbraceScroll), typeof(VengefulSpiritScroll), typeof(WitherScroll), typeof(WraithFormScroll),
            typeof(ExorcismScroll)
        };

        private static readonly Type[] m_ArcanistScrollTypes =
        {
            typeof(ArcaneCircleScroll), typeof(GiftOfRenewalScroll), typeof(ImmolatingWeaponScroll), typeof(AttuneWeaponScroll),
            typeof(ThunderstormScroll), typeof(NatureFuryScroll),
			typeof(ReaperFormScroll), typeof(WildfireScroll), typeof(EssenceOfWindScroll), typeof(DryadAllureScroll),
            typeof(EtherealVoyageScroll), typeof(WordOfDeathScroll), typeof(GiftOfLifeScroll), typeof(ArcaneEmpowermentScroll)
        };

        private static readonly Type[] m_MysticismScrollTypes =
        {
            typeof(NetherBoltScroll), typeof(HealingStoneScroll), typeof(PurgeMagicScroll), typeof(EagleStrikeScroll),
            typeof(AnimatedWeaponScroll), typeof(StoneFormScroll), typeof(SpellTriggerScroll), typeof(CleansingWindsScroll),
            typeof(BombardScroll), typeof(SpellPlagueScroll), typeof(HailStormScroll), typeof(NetherCycloneScroll),
            typeof(RisingColossusScroll), typeof(SleepScroll), typeof(MassSleepScroll), typeof(EnchantScroll)
        };

        public static Type[] MageryScrollTypes => m_MageryScrollTypes;
        public static Type[] NecromancyScrollTypes => m_NecromancyScrollTypes;
        public static Type[] MysticismScrollTypes => m_MysticismScrollTypes;
        public static Type[] ArcanistScrollTypes => m_ArcanistScrollTypes;
        #endregion

        #region Journals/Books
        private static readonly Type[] m_GrimmochJournalTypes =
        {
            typeof(GrimmochJournal1), typeof(GrimmochJournal2), typeof(GrimmochJournal3), typeof(GrimmochJournal6),
            typeof(GrimmochJournal7), typeof(GrimmochJournal11), typeof(GrimmochJournal14), typeof(GrimmochJournal17),
            typeof(GrimmochJournal23)
        };

        public static Type[] GrimmochJournalTypes => m_GrimmochJournalTypes;

        private static readonly Type[] m_LysanderNotebookTypes =
        {
            typeof(LysanderNotebook1), typeof(LysanderNotebook2), typeof(LysanderNotebook3), typeof(LysanderNotebook7),
            typeof(LysanderNotebook8), typeof(LysanderNotebook11)
        };

        public static Type[] LysanderNotebookTypes => m_LysanderNotebookTypes;

        private static readonly Type[] m_TavarasJournalTypes =
        {
            typeof(TavarasJournal1), typeof(TavarasJournal2), typeof(TavarasJournal3), typeof(TavarasJournal6),
            typeof(TavarasJournal7), typeof(TavarasJournal8), typeof(TavarasJournal9), typeof(TavarasJournal11),
            typeof(TavarasJournal14), typeof(TavarasJournal16), typeof(TavarasJournal16b), typeof(TavarasJournal17),
            typeof(TavarasJournal19)
        };

        public static Type[] TavarasJournalTypes => m_TavarasJournalTypes;
		#endregion
		
        private static readonly Type[] m_NewWandTypes =
        {
            typeof(FireballWand), typeof(LightningWand), typeof(MagicArrowWand), typeof(GreaterHealWand), typeof(HarmWand),
            typeof(HealWand)
        };
        public static Type[] NewWandTypes => m_NewWandTypes;

        private static readonly Type[] m_WandTypes = { typeof(ClumsyWand), typeof(FeebleWand), typeof(ManaDrainWand), typeof(WeaknessWand) };
        public static Type[] WandTypes => m_WandTypes;

        private static readonly Type[] m_LibraryBookTypes =
        {
            typeof(GrammarOfOrcish), typeof(CallToAnarchy), typeof(ArmsAndWeaponsPrimer), typeof(SongOfSamlethe),
            typeof(TaleOfThreeTribes), typeof(GuideToGuilds), typeof(BirdsOfBritannia), typeof(BritannianFlora),
            typeof(ChildrenTalesVol2), typeof(TalesOfVesperVol1), typeof(DeceitDungeonOfHorror), typeof(DimensionalTravel),
            typeof(EthicalHedonism), typeof(MyStory), typeof(DiversityOfOurLand), typeof(QuestOfVirtues), typeof(RegardingLlamas),
            typeof(TalkingToWisps), typeof(TamingDragons), typeof(BoldStranger), typeof(BurningOfTrinsic), typeof(TheFight),
            typeof(LifeOfATravellingMinstrel), typeof(MajorTradeAssociation), typeof(RankingsOfTrades),
            typeof(WildGirlOfTheForest), typeof(TreatiseOnAlchemy), typeof(VirtueBook)
        };
        public static Type[] LibraryBookTypes => m_LibraryBookTypes;
        #endregion

        public static Item RandomEssence()
        {
            return Construct(m_ImbuingEssenceIngreds);
        }

        #region Accessors
        public static BaseWand RandomWand()
        {
            return Construct(m_NewWandTypes) as BaseWand;
        }

        public static BaseClothing RandomClothing(bool inTokuno = false, bool isMondain = false, bool isStygian = false)
        {
            if (isStygian)
            {
                return Construct(m_SAClothingTypes, m_ClothingTypes) as BaseClothing;
            }

            if (isMondain)
            {
                return Construct(m_MLClothingTypes, m_ClothingTypes) as BaseClothing;
            }

            if (inTokuno)
            {
                return Construct(m_SEClothingTypes, m_ClothingTypes) as BaseClothing;
            }

            return Construct(m_ClothingTypes) as BaseClothing;
        }

        public static BaseWeapon RandomRangedWeapon(bool inTokuno = false, bool isMondain = false, bool isStygian = false)
        {
            if (isStygian)
            {
                return Construct(m_SARangedWeaponTypes, m_RangedWeaponTypes) as BaseWeapon;
            }

            if (isMondain)
            {
                return Construct(m_MLRangedWeaponTypes, m_RangedWeaponTypes) as BaseWeapon;
            }

            if (inTokuno)
            {
                return Construct(m_SERangedWeaponTypes, m_RangedWeaponTypes) as BaseWeapon;
            }

            return Construct(m_RangedWeaponTypes) as BaseWeapon;
        }

        public static BaseWeapon RandomWeapon(bool inTokuno = false, bool isMondain = false, bool isStygian = false)
        {
            if (isStygian)
            {
                return Construct(m_SAWeaponTypes, m_WeaponTypes) as BaseWeapon;
            }

            if (isMondain)
            {
                return Construct(m_MLWeaponTypes, m_WeaponTypes) as BaseWeapon;
            }

            if (inTokuno)
            {
                return Construct(m_SEWeaponTypes, m_WeaponTypes) as BaseWeapon;
            }

            return Construct(m_WeaponTypes) as BaseWeapon;
        }

        public static BaseJewel RandomJewelry(bool isStygian = false)
        {
            if (isStygian)
            {
                return Construct(m_SAJewelryTypes, m_JewelryTypes) as BaseJewel;
            }

            return Construct(m_JewelryTypes) as BaseJewel;
        }

        public static BaseArmor RandomArmor(bool inTokuno = false, bool isMondain = false, bool isStygian = false)
        {
            if (isStygian)
            {
                return Construct(m_SAArmorTypes, m_ArmorTypes) as BaseArmor;
            }

            if (isMondain)
            {
                return Construct(m_MLArmorTypes, m_ArmorTypes) as BaseArmor;
            }

            if (inTokuno)
            {
                return Construct(m_SEArmorTypes, m_ArmorTypes) as BaseArmor;
            }

            return Construct(m_ArmorTypes) as BaseArmor;
        }

        public static BaseHat RandomHat(bool inTokuno = false)
        {
            if (inTokuno)
            {
                return Construct(m_SEHatTypes, m_HatTypes) as BaseHat;
            }

            return Construct(m_HatTypes) as BaseHat;
        }

        public static BaseShield RandomShield(bool isStygian = false)
        {
            if (isStygian)
            {
                return Construct(m_ShieldTypes, m_SAShieldTypes) as BaseShield;
            }

            return Construct(m_ShieldTypes) as BaseShield;
        }

        public static BaseArmor RandomArmorOrShield(bool inTokuno = false, bool isMondain = false, bool isStygian = false)
        {
            if (isStygian)
            {
                return Construct(m_SAArmorTypes, m_ArmorTypes, m_ShieldTypes, m_SAShieldTypes) as BaseArmor;
            }

            if (isMondain)
            {
                return Construct(m_MLArmorTypes, m_ArmorTypes, m_ShieldTypes) as BaseArmor;
            }

            if (inTokuno)
            {
                return Construct(m_SEArmorTypes, m_ArmorTypes, m_ShieldTypes) as BaseArmor;
            }

            return Construct(m_ArmorTypes, m_ShieldTypes) as BaseArmor;
        }

        public static Item RandomArmorOrShieldOrJewelry(bool inTokuno = false, bool isMondain = false, bool isStygian = false)
        {
            if (isStygian)
            {
                return Construct(m_SAArmorTypes, m_ArmorTypes, m_HatTypes, m_ShieldTypes, m_JewelryTypes, m_SAJewelryTypes, m_SAShieldTypes);
            }

            if (isMondain)
            {
                return Construct(m_MLArmorTypes, m_ArmorTypes, m_HatTypes, m_ShieldTypes, m_JewelryTypes);
            }

            if (inTokuno)
            {
                return Construct(
                    m_SEArmorTypes,
                    m_ArmorTypes,
                    m_SEHatTypes,
                    m_HatTypes,
                    m_ShieldTypes,
                    m_JewelryTypes);
            }

            return Construct(m_ArmorTypes, m_HatTypes, m_ShieldTypes, m_JewelryTypes);
        }

        public static Item RandomArmorOrShieldOrWeapon(bool inTokuno = false, bool isMondain = false, bool isStygian = false)
        {
            if (isStygian)
                return Construct(
                    m_SAWeaponTypes,
                    m_WeaponTypes,
                    m_SARangedWeaponTypes,
                    m_RangedWeaponTypes,
                    m_SAArmorTypes,
                    m_ArmorTypes,
                    m_HatTypes,
                    m_ShieldTypes,
                    m_SAShieldTypes);

            if (isMondain)
            {
                return Construct(
                    m_MLWeaponTypes,
                    m_WeaponTypes,
                    m_MLRangedWeaponTypes,
                    m_RangedWeaponTypes,
                    m_MLArmorTypes,
                    m_ArmorTypes,
                    m_HatTypes,
                    m_ShieldTypes);
            }

            if (inTokuno)
            {
                return Construct(
                    m_SEWeaponTypes,
                    m_WeaponTypes,
                    m_SERangedWeaponTypes,
                    m_RangedWeaponTypes,
                    m_SEArmorTypes,
                    m_ArmorTypes,
                    m_SEHatTypes,
                    m_HatTypes,
                    m_ShieldTypes);
            }

            return Construct(
                m_WeaponTypes,
                m_RangedWeaponTypes,
                m_ArmorTypes,
                m_HatTypes,
                m_ShieldTypes);
        }

        public static Item RandomArmorOrShieldOrWeaponOrJewelry(bool inTokuno = false, bool isMondain = false, bool isStygian = false)
        {
            if (isStygian)
            {
                return Construct(

                    m_SAWeaponTypes,
                    m_WeaponTypes,
                    m_SARangedWeaponTypes,
                    m_RangedWeaponTypes,
                    m_SAArmorTypes,
                    m_ArmorTypes,
                    m_HatTypes,
                    m_ShieldTypes,
                    m_JewelryTypes,
                    m_SAJewelryTypes,
                    m_SAShieldTypes);
            }

            if (isMondain)
            {
                return Construct(

                    m_MLWeaponTypes,
                    m_WeaponTypes,
                    m_MLRangedWeaponTypes,
                    m_RangedWeaponTypes,
                    m_MLArmorTypes,
                    m_ArmorTypes,
                    m_HatTypes,
                    m_ShieldTypes,
                    m_JewelryTypes);
            }

            if (inTokuno)
            {
                return Construct(

                    m_SEWeaponTypes,
                    m_WeaponTypes,
                    m_SERangedWeaponTypes,
                    m_RangedWeaponTypes,
                    m_SEArmorTypes,
                    m_ArmorTypes,
                    m_SEHatTypes,
                    m_HatTypes,
                    m_ShieldTypes,
                    m_JewelryTypes);
            }

            return Construct(
                m_WeaponTypes,
                m_RangedWeaponTypes,
                m_ArmorTypes,
                m_HatTypes,
                m_ShieldTypes,
                m_JewelryTypes);
        }

        #region Chest of Heirlooms
        public static Item ChestOfHeirloomsContains()
        {
            return Construct(m_SEArmorTypes, m_SEHatTypes, m_SEWeaponTypes, m_SERangedWeaponTypes, m_JewelryTypes);
        }
        #endregion

        public static Item RandomGem()
        {
            return Construct(m_GemTypes);
        }

        public static Item RandomRareGem()
        {
            return Construct(m_RareGemTypes);
        }

        public static Item RandomMLResource()
        {
            return Construct(m_MLResources);
        }

        public static Item RandomReagent()
        {
            return Construct(m_RegTypes);
        }

        public static Item RandomNecromancyReagent()
        {
            return Construct(m_NecroRegTypes);
        }

        public static Item RandomPossibleReagent()
        {
            return Construct(m_RegTypes, m_NecroRegTypes);
        }

        public static Item RandomPotion()
        {
            return Construct(m_PotionTypes);
        }

        public static BaseInstrument RandomInstrument()
        {
            return Construct(m_InstrumentTypes, m_SEInstrumentTypes) as BaseInstrument;
        }

        public static Item RandomStatue()
        {
            return Construct(m_StatueTypes);
        }

        public static SpellScroll RandomScroll(int minIndex, int maxIndex, SpellbookType type)
        {
            Type[] types;

            switch (type)
            {
                default:
                    types = m_MageryScrollTypes;
                    break;
                case SpellbookType.Necromancer:
                    types = m_NecromancyScrollTypes;
                    break;
                case SpellbookType.Arcanist:
                    types = m_ArcanistScrollTypes;
                    break;
                case SpellbookType.Mystic:
                    types = m_MysticismScrollTypes;
                    break;
            }

            return Construct(types, Utility.RandomMinMax(minIndex, maxIndex)) as SpellScroll;
        }

        public static BaseBook RandomGrimmochJournal()
        {
            return Construct(m_GrimmochJournalTypes) as BaseBook;
        }

        public static BaseBook RandomLysanderNotebook()
        {
            return Construct(m_LysanderNotebookTypes) as BaseBook;
        }

        public static BaseBook RandomTavarasJournal()
        {
            return Construct(m_TavarasJournalTypes) as BaseBook;
        }

        public static BaseBook RandomLibraryBook()
        {
            return Construct(m_LibraryBookTypes) as BaseBook;
        }

        public static BaseTalisman RandomTalisman()
        {
            return new RandomTalisman();
        }
        #endregion

        #region Construction methods
        public static Item Construct(Type type)
        {
            Item item;

            try
            {
                item = Activator.CreateInstance(type) as Item;
            }
            catch (Exception e)
            {
                Diagnostics.ExceptionLogging.LogException(e);
                return null;
            }

            return item;
        }

        public static Item Construct(Type[] types)
        {
            if (types.Length > 0)
            {
                return Construct(types, Utility.Random(types.Length));
            }

            return null;
        }

        public static Item Construct(Type[] types, int index)
        {
            if (index >= 0 && index < types.Length)
            {
                return Construct(types[index]);
            }

            return null;
        }

        public static Item Construct(params Type[][] types)
        {
            int totalLength = 0;

            for (int i = 0; i < types.Length; ++i)
            {
                totalLength += types[i].Length;
            }

            if (totalLength > 0)
            {
                int index = Utility.Random(totalLength);

                for (int i = 0; i < types.Length; ++i)
                {
                    if (index >= 0 && index < types[i].Length)
                    {
                        return Construct(types[i][index]);
                    }

                    index -= types[i].Length;
                }
            }

            return null;
        }
        #endregion
    }
}
