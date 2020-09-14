using Server.Accounting;
using Server.Engines.Craft;
using Server.Engines.Quests.Doom;
using Server.Items;
using Server.Mobiles;
using Server.SkillHandlers;
using Server.Targeting;
using System;
using System.Collections.Generic;

namespace Server.Engines.Points
{
    public class CleanUpBritanniaData : PointsSystem
    {
        public override PointsType Loyalty => PointsType.CleanUpBritannia;
        public override TextDefinition Name => m_Name;
        public override bool AutoAdd => true;
        public override double MaxPoints => double.MaxValue;
        public override bool ShowOnLoyaltyGump => false;

        private readonly TextDefinition m_Name = null;

        public static bool Enabled { get; set; }

        public CleanUpBritanniaData()
        {
            Enabled = true;

            if (Enabled)
            {
                InitializeEntries();
                PointsExchange = new Dictionary<string, double>();
            }
        }

        public static double GetPoints(Item item)
        {
            if (item is IVvVItem && ((IVvVItem)item).IsVvVItem)
                return 0;

            double points = 0;

            Type type = item.GetType();

            if (Entries.ContainsKey(type))
            {
                points = Entries[type];

                // Kind of ametuar, but if this arrizes more, we'll make a seperate function
                if (item is SOS && ((SOS)item).IsAncient)
                    points = 2500;

                if (item.Stackable)
                    points = points * item.Amount;

                return points;
            }
            else
            {
                if (item is RunicHammer)
                {
                    RunicHammer hammer = (RunicHammer)item;

                    if (hammer.Resource == CraftResource.DullCopper)
                        points = 5 * hammer.UsesRemaining;
                    else if (hammer.Resource == CraftResource.ShadowIron)
                        points = 10 * hammer.UsesRemaining;
                    else if (hammer.Resource == CraftResource.Copper)
                        points = 25 * hammer.UsesRemaining;
                    else if (hammer.Resource == CraftResource.Bronze)
                        points = 100 * hammer.UsesRemaining;
                    else if (hammer.Resource == CraftResource.Gold)
                        points = 250 * hammer.UsesRemaining;
                    else if (hammer.Resource == CraftResource.Agapite)
                        points = 1000 * hammer.UsesRemaining;
                    else if (hammer.Resource == CraftResource.Verite)
                        points = 4000 * hammer.UsesRemaining;
                    else if (hammer.Resource == CraftResource.Valorite)
                        points = 8000 * hammer.UsesRemaining;
                }
                else if (item is RunicSewingKit)
                {
                    RunicSewingKit sewing = (RunicSewingKit)item;

                    if (sewing.Resource == CraftResource.SpinedLeather)
                        points = 10 * sewing.UsesRemaining;
                    else if (sewing.Resource == CraftResource.HornedLeather)
                        points = 100 * sewing.UsesRemaining;
                    else if (sewing.Resource == CraftResource.BarbedLeather)
                        points = 400 * sewing.UsesRemaining;
                }
                else if (item is PowerScroll)
                {
                    PowerScroll ps = (PowerScroll)item;

                    if (ps.Value == 105)
                        points = 50;
                    else if (ps.Value == 110)
                        points = 100;
                    else if (ps.Value == 115)
                        points = 500;
                    else if (ps.Value == 120)
                        points = 2500;
                }
                else if (item is ScrollOfTranscendence)
                {
                    SpecialScroll sot = (SpecialScroll)item;

                    points = sot.Value / 0.1 * 2;
                }
                else if (item is Bait)
                {
                    Bait bait = (Bait)item;

                    points = 10 * bait.UsesRemaining;
                }
                else if (item is TreasureMap)
                {
                    TreasureMap tmap = (TreasureMap)item;

                    switch (tmap.Level)
                    {
                        default:
                        case 0: return 50;
                        case 1: return 100;
                        case 2: return 250;
                        case 3: return 750;
                        case 4: return 1000;
                    }
                }
                else if (item is MonsterStatuette)
                {
                    MonsterStatuette ms = (MonsterStatuette)item;

                    if (ms.Type == MonsterStatuetteType.Slime)
                        points = 5000;
                }
                else if (item is PigmentsOfTokuno || item is LesserPigmentsOfTokuno)
                {
                    BasePigmentsOfTokuno pigments = (BasePigmentsOfTokuno)item;
                    points = 500 * pigments.UsesRemaining;
                }
                else if (item is ICombatEquipment)
                {
                    points = GetPointsForEquipment(item);
                }

                if (item.LootType != LootType.Blessed && points < 100 && item is IShipwreckedItem && ((IShipwreckedItem)item).IsShipwreckedItem)
                {
                    points = 100;
                }

                return points;
            }
        }

        public override void SendMessage(PlayerMobile from, double old, double points, bool quest)
        {
            from.SendLocalizedMessage(1151281, CleanUpBritannia.GetPoints(from).ToString()); // Your Clean Up Britannia point total is now ~1_VALUE~!
        }

        public static Dictionary<Type, double> Entries;

        public void InitializeEntries()
        {
            Entries = new Dictionary<Type, double>();

            Entries[typeof(DecorativeTopiary)] = 2.0;

            //Fishing
            Entries[typeof(LargeFishingNet)] = 500.0;
            Entries[typeof(AntiqueWeddingDress)] = 500.0;
            Entries[typeof(BronzedArmorValkyrie)] = 500.0;
            Entries[typeof(BaseCrabAndLobster)] = 1.0;
            Entries[typeof(KelpWovenLeggings)] = 500.0;
            Entries[typeof(FabledFishingNet)] = 2500.0;
            Entries[typeof(LavaRock)] = 500.0;
            Entries[typeof(SmugglersLiquor)] = 30.0;
            Entries[typeof(MessageInABottle)] = 100.0;
            Entries[typeof(SOS)] = 100.0;
            Entries[typeof(RunedDriftwoodBow)] = 500.0;
            Entries[typeof(Rope)] = 1600.0;
            Entries[typeof(SpecialFishingNet)] = 250.0;

            //Mining
            Entries[typeof(IronIngot)] = 0.10;
            Entries[typeof(DullCopperIngot)] = 0.50;
            Entries[typeof(ShadowIronIngot)] = 0.75;
            Entries[typeof(CopperIngot)] = 1.0;
            Entries[typeof(BronzeIngot)] = 1.50;
            Entries[typeof(GoldIngot)] = 2.50;
            Entries[typeof(AgapiteIngot)] = 5.0;
            Entries[typeof(VeriteIngot)] = 8.50;
            Entries[typeof(ValoriteIngot)] = 10.0;
            Entries[typeof(Amber)] = 0.30;
            Entries[typeof(Citrine)] = 0.30;
            Entries[typeof(Ruby)] = 0.30;
            Entries[typeof(Tourmaline)] = 0.30;
            Entries[typeof(Amethyst)] = 0.30;
            Entries[typeof(Emerald)] = 0.30;
            Entries[typeof(Sapphire)] = 0.30;
            Entries[typeof(StarSapphire)] = 0.30;
            Entries[typeof(Diamond)] = 0.30;
            Entries[typeof(BlueDiamond)] = 25.0;
            Entries[typeof(FireRuby)] = 25.0;
            Entries[typeof(PerfectEmerald)] = 25.0;
            Entries[typeof(DarkSapphire)] = 25.0;
            Entries[typeof(Turquoise)] = 25.0;
            Entries[typeof(EcruCitrine)] = 25.0;
            Entries[typeof(WhitePearl)] = 25.0;
            Entries[typeof(SmallPieceofBlackrock)] = 10.0;

            //Lumberjacking
            Entries[typeof(Board)] = 0.05;
            Entries[typeof(OakBoard)] = 0.10;
            Entries[typeof(AshBoard)] = 0.25;
            Entries[typeof(YewBoard)] = 0.50;
            Entries[typeof(HeartwoodBoard)] = 1.0;
            Entries[typeof(BloodwoodBoard)] = 2.0;
            Entries[typeof(FrostwoodBoard)] = 3.0;
            Entries[typeof(BarkFragment)] = 1.60;
            Entries[typeof(LuminescentFungi)] = 2.0;
            Entries[typeof(SwitchItem)] = 3.0;
            Entries[typeof(ParasiticPlant)] = 6.0;
            Entries[typeof(BrilliantAmber)] = 62.0;

            //Fletching
            Entries[typeof(Arrow)] = 0.05;
            Entries[typeof(Bolt)] = 0.05;

            //Tailoring
            Entries[typeof(Leather)] = 0.10;
            Entries[typeof(SpinedLeather)] = 0.50;
            Entries[typeof(HornedLeather)] = 1.0;
            Entries[typeof(BarbedLeather)] = 2.0;
            Entries[typeof(Fur)] = 0.10;


            //BOD Rewards
            Entries[typeof(Sandals)] = 2.0;
            Entries[typeof(LeatherGlovesOfMining)] = 50.0;
            Entries[typeof(StuddedGlovesOfMining)] = 100.0;
            Entries[typeof(RingmailGlovesOfMining)] = 500.0;

            //ArtifactRarity 1 Stealable Artifacts
            Entries[typeof(RockArtifact)] = 5.0;
            Entries[typeof(SkullCandleArtifact)] = 5.0;
            Entries[typeof(BottleArtifact)] = 5.0;
            Entries[typeof(DamagedBooksArtifact)] = 5.0;
            Entries[typeof(Basket1Artifact)] = 5.0;
            Entries[typeof(Basket2Artifact)] = 5.0;
            Entries[typeof(Basket3NorthArtifact)] = 5.0;
            Entries[typeof(Basket3WestArtifact)] = 5.0;

            //ArtifactRarity 2 Stealable Artifacts
            Entries[typeof(StretchedHideArtifact)] = 15.0;
            Entries[typeof(BrazierArtifact)] = 15.0;
            Entries[typeof(Basket4Artifact)] = 15.0;
            Entries[typeof(Basket5NorthArtifact)] = 15.0;
            Entries[typeof(Basket5WestArtifact)] = 15.0;
            Entries[typeof(Basket6Artifact)] = 15.0;
            Entries[typeof(ZenRock1Artifact)] = 15.0;

            //ArtifactRarity 3 Stealable Artifacts
            Entries[typeof(LampPostArtifact)] = 25.0;
            Entries[typeof(BooksNorthArtifact)] = 25.0;
            Entries[typeof(BooksWestArtifact)] = 25.0;
            Entries[typeof(BooksFaceDownArtifact)] = 25.0;
            Entries[typeof(BowlsVerticalArtifact)] = 25.0;
            Entries[typeof(FanWestArtifact)] = 25.0;
            Entries[typeof(FanNorthArtifact)] = 25.0;
            Entries[typeof(Sculpture1Artifact)] = 25.0;
            Entries[typeof(Sculpture2Artifact)] = 25.0;
            Entries[typeof(TeapotWestArtifact)] = 25.0;
            Entries[typeof(TeapotNorthArtifact)] = 25.0;
            Entries[typeof(TowerLanternArtifact)] = 25.0;
            Entries[typeof(Urn1Artifact)] = 25.0;
            Entries[typeof(Urn2Artifact)] = 25.0;
            Entries[typeof(ZenRock2Artifact)] = 25.0;
            Entries[typeof(ZenRock3Artifact)] = 25.0;
            Entries[typeof(JugsOfGoblinRotgutArtifact)] = 25.0;
            Entries[typeof(MysteriousSupperArtifact)] = 25.0;

            //ArtifactRarity 4 Stealable Artifacts
            Entries[typeof(BowlArtifact)] = 50.0;
            Entries[typeof(BowlsHorizontalArtifact)] = 50.0;
            Entries[typeof(CupsArtifact)] = 50.0;
            Entries[typeof(TripleFanWestArtifact)] = 50.0;
            Entries[typeof(TripleFanNorthArtifact)] = 50.0;
            Entries[typeof(Painting1WestArtifact)] = 50.0;
            Entries[typeof(Painting1NorthArtifact)] = 50.0;
            Entries[typeof(Painting2WestArtifact)] = 50.0;
            Entries[typeof(Painting2NorthArtifact)] = 50.0;
            Entries[typeof(SakeArtifact)] = 50.0;
            Entries[typeof(StolenBottlesOfLiquor1Artifact)] = 50.0;
            Entries[typeof(StolenBottlesOfLiquor2Artifact)] = 50.0;
            Entries[typeof(BottlesOfSpoiledWine1Artifact)] = 50.0;
            Entries[typeof(NaverysWeb1Artifact)] = 50.0;
            Entries[typeof(NaverysWeb2Artifact)] = 50.0;

            //ArtifactRarity 5 Stealable Artifacts
            Entries[typeof(Painting3Artifact)] = 100.0;
            Entries[typeof(SwordDisplay1WestArtifact)] = 100.0;
            Entries[typeof(SwordDisplay1NorthArtifact)] = 100.0;
            Entries[typeof(DyingPlantArtifact)] = 100.0;
            Entries[typeof(LargePewterBowlArtifact)] = 100.0;
            Entries[typeof(NaverysWeb3Artifact)] = 100.0;
            Entries[typeof(NaverysWeb4Artifact)] = 100.0;
            Entries[typeof(NaverysWeb5Artifact)] = 100.0;
            Entries[typeof(NaverysWeb6Artifact)] = 100.0;
            Entries[typeof(BloodySpoonArtifact)] = 100.0;
            Entries[typeof(RemnantsOfMeatLoafArtifact)] = 100.0;
            Entries[typeof(HalfEatenSupperArtifact)] = 100.0;
            Entries[typeof(BackpackArtifact)] = 100.0;
            Entries[typeof(BloodyWaterArtifact)] = 100.0;
            Entries[typeof(EggCaseArtifact)] = 100.0;
            Entries[typeof(GruesomeStandardArtifact)] = 100.0;
            Entries[typeof(SkinnedGoatArtifact)] = 100.0;
            Entries[typeof(StuddedLeggingsArtifact)] = 100.0;
            Entries[typeof(TarotCardsArtifact)] = 100.0;

            //ArtifactRarity 6 Stealable Artifacts
            Entries[typeof(Painting4WestArtifact)] = 200.0;
            Entries[typeof(Painting4NorthArtifact)] = 200.0;
            Entries[typeof(SwordDisplay2WestArtifact)] = 200.0;
            Entries[typeof(SwordDisplay2NorthArtifact)] = 200.0;
            Entries[typeof(LargeDyingPlantArtifact)] = 200.0;
            Entries[typeof(GargishLuckTotemArtifact)] = 200.0;
            Entries[typeof(BookOfTruthArtifact)] = 200.0;
            Entries[typeof(GargishTraditionalVaseArtifact)] = 200.0;
            Entries[typeof(GargishProtectiveTotemArtifact)] = 200.0;
            Entries[typeof(BottlesOfSpoiledWine2Artifact)] = 200.0;
            Entries[typeof(BatteredPanArtifact)] = 200.0;
            Entries[typeof(RustedPanArtifact)] = 200.0;

            //ArtifactRarity 7 Stealable Artifacts
            Entries[typeof(FlowersArtifact)] = 350.0;
            Entries[typeof(GargishBentasVaseArtifact)] = 350.0;
            Entries[typeof(GargishPortraitArtifact)] = 350.0;
            Entries[typeof(GargishKnowledgeTotemArtifact)] = 350.0;
            Entries[typeof(GargishMemorialStatueArtifact)] = 350.0;
            Entries[typeof(StolenBottlesOfLiquor3Artifact)] = 350.0;
            Entries[typeof(BottlesOfSpoiledWine3Artifact)] = 350.0;
            Entries[typeof(DriedUpInkWellArtifact)] = 350.0;
            Entries[typeof(FakeCopperIngotsArtifact)] = 350.0;
            Entries[typeof(CocoonArtifact)] = 350.0;
            Entries[typeof(StuddedTunicArtifact)] = 350.0;

            //ArtifactRarity 8 Stealable Artifacts
            Entries[typeof(Painting5WestArtifact)] = 750.0;
            Entries[typeof(Painting5NorthArtifact)] = 750.0;
            Entries[typeof(DolphinLeftArtifact)] = 750.0;
            Entries[typeof(DolphinRightArtifact)] = 750.0;
            Entries[typeof(SwordDisplay3SouthArtifact)] = 750.0;
            Entries[typeof(SwordDisplay3EastArtifact)] = 750.0;
            Entries[typeof(SwordDisplay4WestArtifact)] = 750.0;
            Entries[typeof(PushmePullyuArtifact)] = 750.0;
            Entries[typeof(StolenBottlesOfLiquor4Artifact)] = 750.0;
            Entries[typeof(RottedOarsArtifact)] = 750.0;
            Entries[typeof(PricelessTreasureArtifact)] = 750.0;
            Entries[typeof(SkinnedDeerArtifact)] = 750.0;

            //ArtifactRarity 9 Stealable Artifacts
            Entries[typeof(Painting6WestArtifact)] = 1400.0;
            Entries[typeof(Painting6NorthArtifact)] = 1400.0;
            Entries[typeof(ManStatuetteSouthArtifact)] = 1400.0;
            Entries[typeof(ManStatuetteEastArtifact)] = 1400.0;
            Entries[typeof(SwordDisplay4NorthArtifact)] = 1400.0;
            Entries[typeof(SwordDisplay5WestArtifact)] = 1400.0;
            Entries[typeof(SwordDisplay5NorthArtifact)] = 1400.0;
            Entries[typeof(TyballsFlaskStandArtifact)] = 1400.0;
            Entries[typeof(BlockAndTackleArtifact)] = 1400.0;
            Entries[typeof(LeatherTunicArtifact)] = 1400.0;
            Entries[typeof(SaddleArtifact)] = 1400.0;

            //ArtifactRarity 10
            Entries[typeof(TitansHammer)] = 2750.0;
            Entries[typeof(ZyronicClaw)] = 2750.0;
            Entries[typeof(InquisitorsResolution)] = 2750.0;
            Entries[typeof(BladeOfTheRighteous)] = 2750.0;
            Entries[typeof(LegacyOfTheDreadLord)] = 2750.0;
            Entries[typeof(TheTaskmaster)] = 2750.0;

            //Virtue Artifacts
            Entries[typeof(TenthAnniversarySculpture)] = 1500.0;
            Entries[typeof(MapOfTheKnownWorld)] = 1500.0;
            Entries[typeof(AnkhPendant)] = 1500.0;
            Entries[typeof(DragonsEnd)] = 1500.0;
            Entries[typeof(JaanasStaff)] = 1500.0;
            Entries[typeof(KatrinasCrook)] = 1500.0;
            Entries[typeof(LordBlackthornsExemplar)] = 1500.0;
            Entries[typeof(SentinelsGuard)] = 1500.0;
            Entries[typeof(CompassionArms)] = 1500.0;
            Entries[typeof(JusticeBreastplate)] = 1500.0;
            Entries[typeof(ValorGauntlets)] = 1500.0;
            Entries[typeof(HonestyGorget)] = 1500.0;
            Entries[typeof(SpiritualityHelm)] = 1500.0;
            Entries[typeof(HonorLegs)] = 1500.0;
            Entries[typeof(SacrificeSollerets)] = 1500.0;

            //Minor Artifacts (ML/Peerless/Tokuno)
            Entries[typeof(CandelabraOfSouls)] = 100.0;
            Entries[typeof(GhostShipAnchor)] = 100.0;
            Entries[typeof(GoldBricks)] = 100.0;
            Entries[typeof(PhillipsWoodenSteed)] = 100.0;
            Entries[typeof(SeahorseStatuette)] = 100.0;
            Entries[typeof(ShipModelOfTheHMSCape)] = 100.0;
            Entries[typeof(AdmiralsHeartyRum)] = 100.0;
            Entries[typeof(AlchemistsBauble)] = 100.0;
            Entries[typeof(ArcticDeathDealer)] = 100.0;
            Entries[typeof(BlazeOfDeath)] = 100.0;
            Entries[typeof(BurglarsBandana)] = 100.0;
            Entries[typeof(CaptainQuacklebushsCutlass)] = 100.0;
            Entries[typeof(CavortingClub)] = 100.0;
            Entries[typeof(DreadPirateHat)] = 100.0;
            Entries[typeof(EnchantedTitanLegBone)] = 100.0;
            Entries[typeof(GwennosHarp)] = 100.0;
            Entries[typeof(IolosLute)] = 100.0;
            Entries[typeof(LunaLance)] = 100.0;
            Entries[typeof(NightsKiss)] = 100.0;
            Entries[typeof(NoxRangersHeavyCrossbow)] = 100.0;
            Entries[typeof(PolarBearMask)] = 100.0;
            Entries[typeof(VioletCourage)] = 100.0;
            Entries[typeof(GlovesOfThePugilist)] = 100.0;
            Entries[typeof(PixieSwatter)] = 100.0;
            Entries[typeof(WrathOfTheDryad)] = 100.0;
            Entries[typeof(StaffOfPower)] = 100.0;
            Entries[typeof(OrcishVisage)] = 100.0;
            Entries[typeof(BowOfTheJukaKing)] = 100.0;
            Entries[typeof(ColdBlood)] = 100.0;
            Entries[typeof(CreepingVine)] = 100.0;
            Entries[typeof(ForgedPardon)] = 100.0;
            Entries[typeof(ManaPhasingOrb)] = 500.0;
            Entries[typeof(RunedSashOfWarding)] = 100.0;
            Entries[typeof(SurgeShield)] = 100.0;
            Entries[typeof(HeartOfTheLion)] = 100.0;
            Entries[typeof(ShieldOfInvulnerability)] = 100.0;
            Entries[typeof(AegisOfGrace)] = 100.0;
            Entries[typeof(BladeDance)] = 100.0;
            Entries[typeof(BloodwoodSpirit)] = 100.0;
            Entries[typeof(Bonesmasher)] = 100.0;
            Entries[typeof(Boomstick)] = 100.0;
            Entries[typeof(BrightsightLenses)] = 100.0;
            Entries[typeof(FeyLeggings)] = 100.0;
            Entries[typeof(FleshRipper)] = 100.0;
            Entries[typeof(HelmOfSwiftness)] = 100.0;
            Entries[typeof(PadsOfTheCuSidhe)] = 100.0;
            Entries[typeof(QuiverOfRage)] = 100.0;
            Entries[typeof(QuiverOfElements)] = 100.0;
            Entries[typeof(RaedsGlory)] = 100.0;
            Entries[typeof(RighteousAnger)] = 100.0;
            Entries[typeof(RobeOfTheEclipse)] = 100.0;
            Entries[typeof(RobeOfTheEquinox)] = 100.0;
            Entries[typeof(SoulSeeker)] = 100.0;
            Entries[typeof(TalonBite)] = 100.0;
            Entries[typeof(TotemOfVoid)] = 100.0;
            Entries[typeof(WildfireBow)] = 100.0;
            Entries[typeof(Windsong)] = 100.0;
            Entries[typeof(CrimsonCincture)] = 100.0;
            Entries[typeof(DreadFlute)] = 100.0;
            Entries[typeof(DreadsRevenge)] = 100.0;
            Entries[typeof(MelisandesCorrodedHatchet)] = 100.0;
            Entries[typeof(AlbinoSquirrelImprisonedInCrystal)] = 100.0;
            Entries[typeof(GrizzledMareStatuette)] = 100.0;
            Entries[typeof(GrizzleGauntlets)] = 100.0;
            Entries[typeof(GrizzleGreaves)] = 100.0;
            Entries[typeof(GrizzleHelm)] = 100.0;
            Entries[typeof(GrizzleTunic)] = 100.0;
            Entries[typeof(GrizzleVambraces)] = 100.0;
            Entries[typeof(ParoxysmusSwampDragonStatuette)] = 100.0;
            Entries[typeof(ScepterOfTheChief)] = 100.0;
            Entries[typeof(CrystallineRing)] = 100.0;
            Entries[typeof(MarkOfTravesty)] = 100.0;
            Entries[typeof(ImprisonedDog)] = 100.0;
            Entries[typeof(AncientFarmersKasa)] = 100.0;
            Entries[typeof(AncientSamuraiDo)] = 100.0;
            Entries[typeof(AncientUrn)] = 100.0;
            Entries[typeof(ArmsOfTacticalExcellence)] = 100.0;
            Entries[typeof(BlackLotusHood)] = 100.0;
            Entries[typeof(ChestOfHeirlooms)] = 100.0;
            Entries[typeof(DaimyosHelm)] = 100.0;
            Entries[typeof(DemonForks)] = 100.0;
            Entries[typeof(TheDestroyer)] = 100.0;
            Entries[typeof(DragonNunchaku)] = 100.0;
            Entries[typeof(Exiler)] = 100.0;
            Entries[typeof(FluteOfRenewal)] = 100.0;
            Entries[typeof(GlovesOfTheSun)] = 100.0;
            Entries[typeof(HanzosBow)] = 100.0;
            Entries[typeof(HonorableSwords)] = 100.0;
            Entries[typeof(LegsOfStability)] = 100.0;
            Entries[typeof(LeurociansMempoOfFortune)] = 100.0;
            Entries[typeof(PeasantsBokuto)] = 100.0;
            Entries[typeof(PilferedDancerFans)] = 100.0;
            Entries[typeof(TomeOfEnlightenment)] = 100.0;

            //Stygian Abyss Artifacts
            Entries[typeof(AbyssalBlade)] = 5000.0;
            Entries[typeof(AnimatedLegsoftheInsaneTinker)] = 5000.0;
            Entries[typeof(AxeOfAbandon)] = 5000.0;
            Entries[typeof(AxesOfFury)] = 5000.0;
            Entries[typeof(BansheesCall)] = 5000.0;
            Entries[typeof(BasiliskHideBreastplate)] = 5000.0;
            Entries[typeof(BladeOfBattle)] = 5000.0;
            Entries[typeof(BouraTailShield)] = 5000.0;
            Entries[typeof(BreastplateOfTheBerserker)] = 5000.0;
            Entries[typeof(BurningAmber)] = 5000.0;
            Entries[typeof(CastOffZombieSkin)] = 5000.0;
            Entries[typeof(CavalrysFolly)] = 5000.0;
            Entries[typeof(ChannelersDefender)] = 5000.0;
            Entries[typeof(ClawsOfTheBerserker)] = 5000.0;
            Entries[typeof(DeathsHead)] = 5000.0;
            Entries[typeof(DefenderOfTheMagus)] = 5000.0;
            Entries[typeof(DemonBridleRing)] = 5000.0;
            Entries[typeof(DemonHuntersStandard)] = 5000.0;
            Entries[typeof(DragonHideShield)] = 5000.0;
            Entries[typeof(DragonJadeEarrings)] = 5000.0;
            Entries[typeof(DraconisWrath)] = 5000.0;
            Entries[typeof(EternalGuardianStaff)] = 5000.0;
            Entries[typeof(FallenMysticsSpellbook)] = 5000.0;
            Entries[typeof(GiantSteps)] = 5000.0;
            Entries[typeof(IronwoodCompositeBow)] = 5000.0;
            Entries[typeof(JadeWarAxe)] = 5000.0;
            Entries[typeof(LegacyOfDespair)] = 5000.0;
            Entries[typeof(Lavaliere)] = 5000.0;
            Entries[typeof(LifeSyphon)] = 5000.0;
            Entries[typeof(Mangler)] = 5000.0;
            Entries[typeof(MantleOfTheFallen)] = 5000.0;
            Entries[typeof(MysticsGarb)] = 5000.0;
            Entries[typeof(NightEyes)] = 5000.0;
            Entries[typeof(ObsidianEarrings)] = 5000.0;
            Entries[typeof(PetrifiedSnake)] = 5000.0;
            Entries[typeof(PillarOfStrength)] = 5000.0;
            Entries[typeof(ProtectoroftheBattleMage)] = 5000.0;
            Entries[typeof(RaptorClaw)] = 5000.0;
            Entries[typeof(ResonantStaffofEnlightenment)] = 5000.0;
            Entries[typeof(ShroudOfTheCondemned)] = 500.0;
            Entries[typeof(GargishSignOfOrder)] = 5000.0;
            Entries[typeof(HumanSignOfOrder)] = 5000.0;
            Entries[typeof(GargishSignOfChaos)] = 5000.0;
            Entries[typeof(HumanSignOfChaos)] = 5000.0;
            Entries[typeof(Slither)] = 5000.0;
            Entries[typeof(SpinedBloodwormBracers)] = 5000.0;
            Entries[typeof(StandardOfChaos)] = 5000.0;
            Entries[typeof(StandardOfChaosG)] = 5000.0;
            Entries[typeof(StaffOfShatteredDreams)] = 5000.0;
            Entries[typeof(StoneDragonsTooth)] = 5000.0;
            Entries[typeof(StoneSlithClaw)] = 5000.0;
            Entries[typeof(StormCaller)] = 5000.0;
            Entries[typeof(SwordOfShatteredHopes)] = 5000.0;
            Entries[typeof(SummonersKilt)] = 5000.0;
            Entries[typeof(Tangle1)] = 5000.0;
            Entries[typeof(TheImpalersPick)] = 5000.0;
            Entries[typeof(TorcOfTheGuardians)] = 5000.0;
            Entries[typeof(TokenOfHolyFavor)] = 5000.0;
            Entries[typeof(VampiricEssence)] = 5000.0;
            Entries[typeof(Venom)] = 5000.0;
            Entries[typeof(VoidInfusedKilt)] = 5000.0;
            Entries[typeof(WallOfHungryMouths)] = 5000.0;

            //Tokuno Major Artifacts
            Entries[typeof(DarkenedSky)] = 2500.0;
            Entries[typeof(KasaOfTheRajin)] = 2500.0;
            Entries[typeof(RuneBeetleCarapace)] = 2500.0;
            Entries[typeof(Stormgrip)] = 2500.0;
            Entries[typeof(SwordOfTheStampede)] = 2500.0;
            Entries[typeof(SwordsOfProsperity)] = 2500.0;
            Entries[typeof(TheHorselord)] = 2500.0;
            Entries[typeof(TomeOfLostKnowledge)] = 2500.0;
            Entries[typeof(WindsEdge)] = 2500.0;

            //Major Artifacts
            Entries[typeof(TheDryadBow)] = 5500.0;
            Entries[typeof(RingOfTheElements)] = 5500.0;
            Entries[typeof(ArcaneShield)] = 5500.0;
            Entries[typeof(SerpentsFang)] = 5500.0;
            Entries[typeof(OrnamentOfTheMagician)] = 5500.0;
            Entries[typeof(BoneCrusher)] = 5500.0;
            Entries[typeof(OrnateCrownOfTheHarrower)] = 5500.0;
            Entries[typeof(HuntersHeaddress)] = 5500.0;
            Entries[typeof(DivineCountenance)] = 5500.0;
            Entries[typeof(BraceletOfHealth)] = 5500.0;
            Entries[typeof(Aegis)] = 5500.0;
            Entries[typeof(AxeOfTheHeavens)] = 5500.0;
            Entries[typeof(HelmOfInsight)] = 5500.0;
            Entries[typeof(Frostbringer)] = 5500.0;
            Entries[typeof(StaffOfTheMagi)] = 5500.0;
            Entries[typeof(TheDragonSlayer)] = 5500.0;
            Entries[typeof(BreathOfTheDead)] = 5500.0;
            Entries[typeof(HolyKnightsBreastplate)] = 5500.0;
            Entries[typeof(TunicOfFire)] = 5500.0;
            Entries[typeof(ShadowDancerLeggings)] = 5500.0;
            Entries[typeof(VoiceOfTheFallenKing)] = 5500.0;
            Entries[typeof(TheBeserkersMaul)] = 5500.0;
            Entries[typeof(HatOfTheMagi)] = 5500.0;
            Entries[typeof(BladeOfInsanity)] = 5500.0;
            Entries[typeof(JackalsCollar)] = 5500.0;
			Entries[typeof(SpiritOfTheTotem)] = 5500.0;

            //Artifacts
            Entries[typeof(PendantOfTheMagi)] = 35;

            //Replicas
            Entries[typeof(TatteredAncientMummyWrapping)] = 5000.0;
            Entries[typeof(WindSpirit)] = 5000.0;
            Entries[typeof(GauntletsOfAnger)] = 5000.0;
            Entries[typeof(GladiatorsCollar)] = 5000.0;
            Entries[typeof(OrcChieftainHelm)] = 5000.0;
            Entries[typeof(ShroudOfDeceit)] = 5000.0;
            Entries[typeof(AcidProofRobe)] = 5000.0;
            Entries[typeof(ANecromancerShroud)] = 5000.0;
            Entries[typeof(CaptainJohnsHat)] = 5000.0;
            Entries[typeof(CrownOfTalKeesh)] = 5000.0;

            Entries[typeof(DetectiveBoots)] = 5000.0;
            Entries[typeof(EmbroideredOakLeafCloak)] = 5000.0;
            Entries[typeof(JadeArmband)] = 5000.0;
            Entries[typeof(LieutenantOfTheBritannianRoyalGuard)] = 5000.0;
            Entries[typeof(MagicalDoor)] = 5000.0;
            Entries[typeof(RoyalGuardInvestigatorsCloak)] = 5000.0;
            Entries[typeof(SamaritanRobe)] = 5000.0;
            Entries[typeof(TheMostKnowledgePerson)] = 5000.0;
            Entries[typeof(TheRobeOfBritanniaAri)] = 5000.0;
            Entries[typeof(DjinnisRing)] = 5000.0;

            Entries[typeof(BraveKnightOfTheBritannia)] = 5000.0;
            Entries[typeof(Calm)] = 5000.0;
            Entries[typeof(FangOfRactus)] = 5000.0;
            Entries[typeof(OblivionsNeedle)] = 5000.0;
            Entries[typeof(Pacify)] = 5000.0;
            Entries[typeof(Quell)] = 5000.0;
            Entries[typeof(RoyalGuardSurvivalKnife)] = 5000.0;
            Entries[typeof(Subdue)] = 5000.0;
            Entries[typeof(Asclepius)] = 5000.0;
            Entries[typeof(BracersofAlchemicalDevastation)] = 5000.0;

            Entries[typeof(GargishAsclepius)] = 5000.0;
            Entries[typeof(GargishBracersofAlchemicalDevastation)] = 5000.0;
            Entries[typeof(HygieiasAmulet)] = 5000.0;
            Entries[typeof(ScrollofValiantCommendation)] = 5000.0;

            //Easter
            Entries[typeof(EasterEggs)] = 2.0;
            Entries[typeof(JellyBeans)] = 1.0;

            //Miscellaneous            
            Entries[typeof(ParrotItem)] = 25.0;
            Entries[typeof(Gold)] = 0.01;
            Entries[typeof(RedScales)] = 0.10;
            Entries[typeof(YellowScales)] = 0.10;
            Entries[typeof(BlackScales)] = 0.10;
            Entries[typeof(GreenScales)] = 0.10;
            Entries[typeof(WhiteScales)] = 0.10;
            Entries[typeof(BlueScales)] = 0.10;
            Entries[typeof(Bottle)] = 0.25;
            Entries[typeof(OrcishKinMask)] = 100.0;
            Entries[typeof(PottedPlantDeed)] = 15000.0;
            Entries[typeof(BagOfSending)] = 250.0;
            Entries[typeof(Cauldron)] = 200.0;
            Entries[typeof(ChampionSkull)] = 1000.0;
            Entries[typeof(ClockworkAssembly)] = 50.0;
            Entries[typeof(ConjurersTrinket)] = 10000.0;

            Entries[typeof(CorgulsHandbookOnMysticism)] = 250.0;
            Entries[typeof(CrownOfArcaneTemperament)] = 5000.0;
            Entries[typeof(DeadWood)] = 1.0;
            Entries[typeof(DustyPillow)] = 250.0;
            Entries[typeof(EndlessDecanter)] = 10.0;
            Entries[typeof(EternallyCorruptTree)] = 1000.0;
            Entries[typeof(ExcellentIronMaiden)] = 50.0;
            Entries[typeof(ExecutionersCap)] = 1.0;
            Entries[typeof(Flowstone)] = 250.0;
            Entries[typeof(GlacialStaff)] = 500.0;
            Entries[typeof(GrapeVine)] = 500.0;
            Entries[typeof(GrobusFur)] = 20.0;
            Entries[typeof(HorseShoes)] = 200.0;

            Entries[typeof(JocklesQuicksword)] = 2.0;
            Entries[typeof(MangledHeadOfDreadhorn)] = 1000.0;
            Entries[typeof(MedusaBlood)] = 1000.0;
            Entries[typeof(MedusaDarkScales)] = 200.0;
            Entries[typeof(MedusaLightScales)] = 200.0;
            Entries[typeof(ContestMiniHouseDeed)] = 6500.0;
            Entries[typeof(MysticsGuard)] = 2500.0;
            Entries[typeof(PowerCrystal)] = 100.0;
            Entries[typeof(PristineDreadHorn)] = 1000.0;
            Entries[typeof(ProspectorsTool)] = 3.0;
            Entries[typeof(RecipeScroll)] = 10.0;

            Entries[typeof(SwampTile)] = 5000.0;
            Entries[typeof(TastyTreat)] = 100.0;
            Entries[typeof(TatteredAncientScroll)] = 200.0;
            Entries[typeof(ThorvaldsMedallion)] = 250.0;
            Entries[typeof(TribalBerry)] = 10.0;
            Entries[typeof(TunicOfGuarding)] = 2.0;
            Entries[typeof(UndeadGargHorn)] = 1000.0;
            Entries[typeof(UntranslatedAncientTome)] = 200.0;
            Entries[typeof(WallBlood)] = 5000.0;
            Entries[typeof(Whip)] = 200.0;
            Entries[typeof(BalmOfSwiftness)] = 100.0;
            Entries[typeof(TaintedMushroom)] = 1000.0;
            Entries[typeof(GoldenSkull)] = 1000.0;
            Entries[typeof(RedSoulstone)] = 15000.0;
            Entries[typeof(BlueSoulstone)] = 15000.0;
            Entries[typeof(SoulStone)] = 15000.0;
            Entries[typeof(HornOfPlenty)] = 2500.0;
            Entries[typeof(KepetchWax)] = 500.0;
            Entries[typeof(SlithEye)] = 500.0;
            Entries[typeof(SoulstoneFragment)] = 500.0;
            Entries[typeof(WhiteClothDyeTub)] = 300.0;
            Entries[typeof(Lodestone)] = 75.0;
            Entries[typeof(FeyWings)] = 75.0;
            Entries[typeof(StoutWhip)] = 3.0;
            Entries[typeof(PlantClippings)] = 1.0;
            Entries[typeof(BasketOfRolls)] = 5.0;
            Entries[typeof(Yeast)] = 10.0;
            Entries[typeof(ValentinesCard)] = 50.0;
            Entries[typeof(MetallicClothDyeTub)] = 100.0;

            //Treasure Hunting
            Entries[typeof(Lockpick)] = 0.10;
        }

        public static int GetPointsForEquipment(Item item)
        {
            if (item is IEpiphanyArmor)
            {
                return 1000;
            }

            foreach (CraftSystem system in CraftSystem.Systems)
            {
                CraftItem crItem = null;

                if (system != null && system.CraftItems != null)
                {
                    Type type = item.GetType();

                    if (type == typeof(SilverRing))
                    {
                        type = typeof(GoldRing);
                    }
                    else if (type == typeof(SilverBracelet))
                    {
                        type = typeof(GoldBracelet);
                    }

                    crItem = system.CraftItems.SearchFor(type);

                    if (crItem != null && crItem.Resources != null)
                    {
                        CraftRes craftRes = crItem.Resources.GetAt(0);
                        double amount = 1;

                        if (craftRes != null)
                        {
                            amount = craftRes.Amount;
                        }

                        double award = 1;

                        if (item is IResource)
                        {
                            switch (((IResource)item).Resource)
                            {
                                default: award = amount * .1; break;
                                case CraftResource.DullCopper: award = amount * .47; break;
                                case CraftResource.ShadowIron: award = amount * .73; break;
                                case CraftResource.Copper: award = amount * 1.0; break;
                                case CraftResource.Bronze: award = amount * 1.47; break;
                                case CraftResource.Gold: award = amount * 2.5; break;
                                case CraftResource.Agapite: award = amount * 5.0; break;
                                case CraftResource.Verite: award = amount * 8.5; break;
                                case CraftResource.Valorite: award = amount * 10; break;
                                case CraftResource.SpinedLeather: award = amount * 0.5; break;
                                case CraftResource.HornedLeather: award = amount * 1.0; break;
                                case CraftResource.BarbedLeather: award = amount * 2.0; break;
                                case CraftResource.OakWood: award = amount * .17; break;
                                case CraftResource.AshWood: award = amount * .33; break;
                                case CraftResource.YewWood: award = amount * .67; break;
                                case CraftResource.Heartwood: award = amount * 1.0; break;
                                case CraftResource.Bloodwood: award = amount * 2.17; break;
                                case CraftResource.Frostwood: award = amount * 3.17; break;
                            }
                        }

                        int weight = item is BaseWeapon && !((BaseWeapon)item).DImodded ? Imbuing.GetTotalWeight(item, 12, false, true) : Imbuing.GetTotalWeight(item, -1, false, true);

                        if (weight > 0)
                        {
                            award += weight / 30;
                        }

                        return (int)award;
                    }
                }
            }

            return 0;
        }

        #region Points Exchange

        public Dictionary<string, double> PointsExchange { get; private set; }

        public double GetPointsFromExchange(Mobile m)
        {
            Account a = m.Account as Account;

            if (a != null && !PointsExchange.ContainsKey(a.Username))
            {
                PointsExchange[a.Username] = 0.0;
            }

            return a == null ? 0.0 : PointsExchange[a.Username];
        }

        public bool AddPointsToExchange(Mobile m)
        {
            Account a = m.Account as Account;

            if (a == null)
            {
                return false;
            }

            double points = GetPoints(m);

            if (points <= 0)
            {
                m.SendLocalizedMessage(1158451); // This account has no points to deposit.
            }
            else if (DeductPoints(m, points))
            {
                if (!PointsExchange.ContainsKey(a.Username))
                {
                    PointsExchange[a.Username] = points;
                }
                else
                {
                    PointsExchange[a.Username] += points;
                }

                m.SendLocalizedMessage(1158452, points.ToString("N0")); // You have deposited ~1_VALUE~ Cleanup Britannia Points.
                return true;
            }

            return false;
        }

        public bool RemovePointsFromExchange(Mobile m)
        {
            Account a = m.Account as Account;

            if (a == null)
            {
                return false;
            }

            double points = GetPointsFromExchange(m);

            if (points <= 0)
            {
                m.SendLocalizedMessage(1158457); // This account has no points to withdraw.
            }
            else if (PointsExchange.ContainsKey(a.Username))
            {
                PointsExchange[a.Username] = 0;
                AwardPoints(m, points, false, false);

                m.SendLocalizedMessage(1158453, string.Format("{0}\t{1}", points.ToString("N0"), ((int)GetPoints(m)).ToString("N0"))); // You have withdrawn ~1_VALUE~ Cleanup Britannia Points.  You now have ~2_VALUE~ points.
                return true;
            }

            return false;
        }
        #endregion

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(PointsExchange == null ? 0 : PointsExchange.Count);

            if (PointsExchange != null)
            {
                foreach (KeyValuePair<string, double> kvp in PointsExchange)
                {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value);
                }
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            if (Version >= 2)
            {
                int version = reader.ReadInt();

                int count = reader.ReadInt();

                for (int i = 0; i < count; i++)
                {
                    string accountName = reader.ReadString();
                    double points = reader.ReadDouble();

                    PointsExchange[accountName] = points;
                }
            }
        }
    }

    public class AppraiseforCleanupTarget : Target
    {
        private readonly Mobile m_Mobile;

        public AppraiseforCleanupTarget(Mobile from) : base(-1, true, TargetFlags.None)
        {
            m_Mobile = from;
        }

        protected override void OnTarget(Mobile m, object targeted)
        {
            if (targeted is Item)
            {
                Item item = (Item)targeted;

                if (!item.IsChildOf(m_Mobile))
                    return;

                double points = CleanUpBritanniaData.GetPoints(item);

                if (points == 0)
                    m_Mobile.SendLocalizedMessage(1151271); // This item has no turn-in value for Clean Up Britannia.
                else if (points < 1)
                    m_Mobile.SendLocalizedMessage(1151272); // This item is worth less than one point for Clean Up Britannia.
                else if (points == 1)
                    m_Mobile.SendLocalizedMessage(1151273); // This item is worth approximately one point for Clean Up Britannia.
                else
                    m_Mobile.SendLocalizedMessage(1151274, points.ToString()); //This item is worth approximately ~1_VALUE~ points for Clean Up Britannia.

                m_Mobile.Target = new AppraiseforCleanupTarget(m_Mobile);
            }
            else
            {
                m_Mobile.SendLocalizedMessage(1151271); // This item has no turn-in value for Clean Up Britannia.
                m_Mobile.Target = new AppraiseforCleanupTarget(m_Mobile);
            }
        }
    }
}
