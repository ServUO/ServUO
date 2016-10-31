using System;
using Server;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Engines.ResortAndCasino;
using Server.Targeting;
using System.Linq;
using Server.Engines.CleanUpBritannia;

namespace Server.Engines.Points
{
    public class CleanUpBritanniaData : PointsSystem
    {
        public override PointsType Loyalty { get { return PointsType.CleanUpBritannia; } }
        public override TextDefinition Name { get { return m_Name; } }
        public override bool AutoAdd { get { return true; } }
        public override double MaxPoints { get { return int.MaxValue; } }
        public override bool ShowOnLoyaltyGump { get { return false; } }

        private TextDefinition m_Name = null;

        public CleanUpBritanniaData()
        {
            InitializeEntries();
        }

        public static double GetPoints(Item item)
        {
            Type type = item.GetType();

            if (Entries.ContainsKey(type))
            {
                double points = 0;

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
                else if (item is Bait)
                {
                    Bait sewing = (Bait)item;

                    points = 10 * sewing.UsesRemaining;
                }
                else
                {
                    points = Entries[type].Item1;

                    if (item is Bait)
                    {
                        Bait bait = (Bait)item;
                        points = points * bait.UsesRemaining;
                    }
                    else if (item.Stackable)
                        points = points * item.Amount;
                }

                return points;
            }

            return 0.0;
        }

        public override void SendMessage(PlayerMobile from, double old, double points, bool quest)
        {          
        }

        public static Dictionary<Type, Tuple<double>> Entries;

        public void InitializeEntries()
        {
            Entries = new Dictionary<Type, Tuple<double>>();

            Entries[typeof(DecorativeTopiary)] = new Tuple<double>(2.0);

            //Fishing
            Entries[typeof(LargeFishingNet)] = new Tuple<double>(500.0);
            Entries[typeof(AntiqueWeddingDress)] = new Tuple<double>(500.0);
            Entries[typeof(BronzedArmorValkyrie)] = new Tuple<double>(500.0);
            Entries[typeof(Bait)] = new Tuple<double>(10.0);
            Entries[typeof(BaseCrabAndLobster)] = new Tuple<double>(1.0);
            Entries[typeof(KelpWovenLeggings)] = new Tuple<double>(500.0);
            Entries[typeof(FabledFishingNet)] = new Tuple<double>(2500.0);
            Entries[typeof(LavaRock)] = new Tuple<double>(500.0);
            Entries[typeof(SmugglersLiquor)] = new Tuple<double>(30.0);
            Entries[typeof(MessageInABottle)] = new Tuple<double>(100.0);
            Entries[typeof(RunedDriftwoodBow)] = new Tuple<double>(500.0);
            Entries[typeof(Rope)] = new Tuple<double>(1600.0);
            Entries[typeof(ShipModelOfTheHMSCape)] = new Tuple<double>(100.0);
            Entries[typeof(SpecialFishingNet)] = new Tuple<double>(250.0);

            //Mining
            Entries[typeof(IronIngot)] = new Tuple<double>(0.10);
            Entries[typeof(DullCopperIngot)] = new Tuple<double>(0.50);
            Entries[typeof(ShadowIronIngot)] = new Tuple<double>(0.75);
            Entries[typeof(CopperIngot)] = new Tuple<double>(1.0);
            Entries[typeof(BronzeIngot)] = new Tuple<double>(1.50);
            Entries[typeof(GoldIngot)] = new Tuple<double>(2.50);
            Entries[typeof(AgapiteIngot)] = new Tuple<double>(5.0);
            Entries[typeof(VeriteIngot)] = new Tuple<double>(8.50);
            Entries[typeof(ValoriteIngot)] = new Tuple<double>(10.0);
            Entries[typeof(Amber)] = new Tuple<double>(0.30);
            Entries[typeof(Citrine)] = new Tuple<double>(0.30);
            Entries[typeof(Ruby)] = new Tuple<double>(0.30);
            Entries[typeof(Tourmaline)] = new Tuple<double>(0.30);
            Entries[typeof(Amethyst)] = new Tuple<double>(0.30);
            Entries[typeof(Emerald)] = new Tuple<double>(0.30);
            Entries[typeof(Sapphire)] = new Tuple<double>(0.30);
            Entries[typeof(StarSapphire)] = new Tuple<double>(0.30);
            Entries[typeof(Diamond)] = new Tuple<double>(0.30);
            Entries[typeof(BlueDiamond)] = new Tuple<double>(25.0);           
            Entries[typeof(FireRuby)] = new Tuple<double>(25.0);
            Entries[typeof(PerfectEmerald)] = new Tuple<double>(25.0);
            Entries[typeof(DarkSapphire)] = new Tuple<double>(25.0);
            Entries[typeof(Turquoise)] = new Tuple<double>(25.0);
            Entries[typeof(EcruCitrine)] = new Tuple<double>(25.0);
            Entries[typeof(WhitePearl)] = new Tuple<double>(25.0);
            Entries[typeof(SmallPieceofBlackrock)] = new Tuple<double>(10.0);

            //Lumberjacking
            Entries[typeof(Board)] = new Tuple<double>(0.05);
            Entries[typeof(OakBoard)] = new Tuple<double>(0.10);
            Entries[typeof(AshBoard)] = new Tuple<double>(0.25);
            Entries[typeof(YewBoard)] = new Tuple<double>(0.50);
            Entries[typeof(HeartwoodBoard)] = new Tuple<double>(1.0);
            Entries[typeof(BloodwoodBoard)] = new Tuple<double>(2.0);
            Entries[typeof(FrostwoodBoard)] = new Tuple<double>(3.0);
            Entries[typeof(BarkFragment)] = new Tuple<double>(1.60);
            Entries[typeof(LuminescentFungi)] = new Tuple<double>(2.0);
            Entries[typeof(SwitchItem)] = new Tuple<double>(3.0);
            Entries[typeof(ParasiticPlant)] = new Tuple<double>(6.25);
            Entries[typeof(BrilliantAmber)] = new Tuple<double>(62.50);

            //Fletching
            Entries[typeof(Arrow)] = new Tuple<double>(0.05);
            Entries[typeof(Bolt)] = new Tuple<double>(0.05);

            //Tailoring
            Entries[typeof(Leather)] = new Tuple<double>(0.10);
            Entries[typeof(SpinedLeather)] = new Tuple<double>(0.50);
            Entries[typeof(HornedLeather)] = new Tuple<double>(1.0);
            Entries[typeof(BarbedLeather)] = new Tuple<double>(2.0);

            
            Entries[typeof(StretchedHideArtifact)] = new Tuple<double>(2.0);
            Entries[typeof(Sandals)] = new Tuple<double>(2.0);

            //ArtifactRarity 1 Stealable Artifacts
            Entries[typeof(RockArtifact)] = new Tuple<double>(5.0);
            Entries[typeof(SkullCandleArtifact)] = new Tuple<double>(5.0);
            Entries[typeof(BottleArtifact)] = new Tuple<double>(5.0);
            Entries[typeof(DamagedBooksArtifact)] = new Tuple<double>(5.0);
            Entries[typeof(Basket1Artifact)] = new Tuple<double>(5.0);
            Entries[typeof(Basket2Artifact)] = new Tuple<double>(5.0);
            Entries[typeof(Basket3NorthArtifact)] = new Tuple<double>(5.0);
            Entries[typeof(Basket3WestArtifact)] = new Tuple<double>(5.0);

            //ArtifactRarity 2 Stealable Artifacts
            Entries[typeof(StretchedHideArtifact)] = new Tuple<double>(15.0);
            Entries[typeof(BrazierArtifact)] = new Tuple<double>(15.0);
            Entries[typeof(Basket4Artifact)] = new Tuple<double>(15.0);
            Entries[typeof(Basket5NorthArtifact)] = new Tuple<double>(15.0);
            Entries[typeof(Basket5WestArtifact)] = new Tuple<double>(15.0);
            Entries[typeof(Basket6Artifact)] = new Tuple<double>(15.0);
            Entries[typeof(ZenRock1Artifact)] = new Tuple<double>(15.0);

            //ArtifactRarity 3 Stealable Artifacts
            Entries[typeof(LampPostArtifact)] = new Tuple<double>(25.0);
            Entries[typeof(BooksNorthArtifact)] = new Tuple<double>(25.0);
            Entries[typeof(BooksWestArtifact)] = new Tuple<double>(25.0);
            Entries[typeof(BooksFaceDownArtifact)] = new Tuple<double>(25.0);
            Entries[typeof(BowlsVerticalArtifact)] = new Tuple<double>(25.0);
            Entries[typeof(FanWestArtifact)] = new Tuple<double>(25.0);
            Entries[typeof(FanNorthArtifact)] = new Tuple<double>(25.0);
            Entries[typeof(Sculpture1Artifact)] = new Tuple<double>(25.0);
            Entries[typeof(Sculpture2Artifact)] = new Tuple<double>(25.0);
            Entries[typeof(TeapotWestArtifact)] = new Tuple<double>(25.0);
            Entries[typeof(TeapotNorthArtifact)] = new Tuple<double>(25.0);
            Entries[typeof(TowerLanternArtifact)] = new Tuple<double>(25.0);
            Entries[typeof(Urn1Artifact)] = new Tuple<double>(25.0);
            Entries[typeof(Urn2Artifact)] = new Tuple<double>(25.0);
            Entries[typeof(ZenRock2Artifact)] = new Tuple<double>(25.0);
            Entries[typeof(ZenRock3Artifact)] = new Tuple<double>(25.0);
            Entries[typeof(JugsOfGoblinRotgutArtifact)] = new Tuple<double>(25.0);
            Entries[typeof(MysteriousSupperArtifact)] = new Tuple<double>(25.0);

            //ArtifactRarity 4 Stealable Artifacts
            Entries[typeof(BowlArtifact)] = new Tuple<double>(50.0);
            Entries[typeof(BowlsHorizontalArtifact)] = new Tuple<double>(50.0);
            Entries[typeof(CupsArtifact)] = new Tuple<double>(50.0);
            Entries[typeof(TripleFanWestArtifact)] = new Tuple<double>(50.0);
            Entries[typeof(TripleFanNorthArtifact)] = new Tuple<double>(50.0);
            Entries[typeof(Painting1WestArtifact)] = new Tuple<double>(50.0);
            Entries[typeof(Painting1NorthArtifact)] = new Tuple<double>(50.0);
            Entries[typeof(Painting2WestArtifact)] = new Tuple<double>(50.0);
            Entries[typeof(Painting2NorthArtifact)] = new Tuple<double>(50.0);
            Entries[typeof(SakeArtifact)] = new Tuple<double>(50.0);
            Entries[typeof(StolenBottlesOfLiquor1Artifact)] = new Tuple<double>(50.0);
            Entries[typeof(StolenBottlesOfLiquor2Artifact)] = new Tuple<double>(50.0);
            Entries[typeof(BottlesOfSpoiledWine1Artifact)] = new Tuple<double>(50.0);
            Entries[typeof(NaverysWeb1Artifact)] = new Tuple<double>(50.0);
            Entries[typeof(NaverysWeb2Artifact)] = new Tuple<double>(50.0);

            //ArtifactRarity 5 Stealable Artifacts
            Entries[typeof(Painting3Artifact)] = new Tuple<double>(100.0);
            Entries[typeof(SwordDisplay1WestArtifact)] = new Tuple<double>(100.0);
            Entries[typeof(SwordDisplay1NorthArtifact)] = new Tuple<double>(100.0);
            Entries[typeof(DyingPlantArtifact)] = new Tuple<double>(100.0);
            Entries[typeof(LargePewterBowlArtifact)] = new Tuple<double>(100.0);
            Entries[typeof(NaverysWeb3Artifact)] = new Tuple<double>(100.0);
            Entries[typeof(NaverysWeb4Artifact)] = new Tuple<double>(100.0);
            Entries[typeof(NaverysWeb5Artifact)] = new Tuple<double>(100.0);
            Entries[typeof(NaverysWeb6Artifact)] = new Tuple<double>(100.0);
            Entries[typeof(BloodySpoonArtifact)] = new Tuple<double>(100.0);
            Entries[typeof(RemnantsOfMeatLoafArtifact)] = new Tuple<double>(100.0);
            Entries[typeof(HalfEatenSupperArtifact)] = new Tuple<double>(100.0);
            Entries[typeof(BackpackArtifact)] = new Tuple<double>(100.0);
            Entries[typeof(BloodyWaterArtifact)] = new Tuple<double>(100.0);
            Entries[typeof(EggCaseArtifact)] = new Tuple<double>(100.0);
            Entries[typeof(GruesomeStandardArtifact)] = new Tuple<double>(100.0);
            Entries[typeof(SkinnedGoatArtifact)] = new Tuple<double>(100.0);
            Entries[typeof(StuddedLeggingsArtifact)] = new Tuple<double>(100.0);
            Entries[typeof(TarotCardsArtifact)] = new Tuple<double>(100.0);

            //ArtifactRarity 6 Stealable Artifacts
            Entries[typeof(Painting4WestArtifact)] = new Tuple<double>(200.0);
            Entries[typeof(Painting4NorthArtifact)] = new Tuple<double>(200.0);
            Entries[typeof(SwordDisplay2WestArtifact)] = new Tuple<double>(200.0);
            Entries[typeof(SwordDisplay2NorthArtifact)] = new Tuple<double>(200.0);
            Entries[typeof(LargeDyingPlantArtifact)] = new Tuple<double>(200.0);
            Entries[typeof(GargishLuckTotemArtifact)] = new Tuple<double>(200.0);
            Entries[typeof(BookOfTruthArtifact)] = new Tuple<double>(200.0);
            Entries[typeof(GargishTraditionalVaseArtifact)] = new Tuple<double>(200.0);
            Entries[typeof(GargishProtectiveTotemArtifact)] = new Tuple<double>(200.0);
            Entries[typeof(BottlesOfSpoiledWine2Artifact)] = new Tuple<double>(200.0);
            Entries[typeof(BatteredPanArtifact)] = new Tuple<double>(200.0);
            Entries[typeof(RustedPanArtifact)] = new Tuple<double>(200.0);

            //ArtifactRarity 7 Stealable Artifacts
            Entries[typeof(FlowersArtifact)] = new Tuple<double>(350.0);
            Entries[typeof(GargishBentasVaseArtifact)] = new Tuple<double>(350.0);
            Entries[typeof(GargishPortraitArtifact)] = new Tuple<double>(350.0);
            Entries[typeof(GargishKnowledgeTotemArtifact)] = new Tuple<double>(350.0);
            Entries[typeof(GargishMemorialStatueArtifact)] = new Tuple<double>(350.0);
            Entries[typeof(StolenBottlesOfLiquor3Artifact)] = new Tuple<double>(350.0);
            Entries[typeof(BottlesOfSpoiledWine3Artifact)] = new Tuple<double>(350.0);
            Entries[typeof(DriedUpInkWellArtifact)] = new Tuple<double>(350.0);
            Entries[typeof(FakeCopperIngotsArtifact)] = new Tuple<double>(350.0);
            Entries[typeof(CocoonArtifact)] = new Tuple<double>(350.0);
            Entries[typeof(StuddedTunicArtifact)] = new Tuple<double>(350.0);

            //ArtifactRarity 8 Stealable Artifacts
            Entries[typeof(Painting5WestArtifact)] = new Tuple<double>(750.0);
            Entries[typeof(Painting5NorthArtifact)] = new Tuple<double>(750.0);
            Entries[typeof(DolphinLeftArtifact)] = new Tuple<double>(750.0);
            Entries[typeof(DolphinRightArtifact)] = new Tuple<double>(750.0);
            Entries[typeof(SwordDisplay3SouthArtifact)] = new Tuple<double>(750.0);
            Entries[typeof(SwordDisplay3EastArtifact)] = new Tuple<double>(750.0);
            Entries[typeof(SwordDisplay4WestArtifact)] = new Tuple<double>(750.0);
            Entries[typeof(PushmePullyuArtifact)] = new Tuple<double>(750.0);
            Entries[typeof(StolenBottlesOfLiquor4Artifact)] = new Tuple<double>(750.0);
            Entries[typeof(RottedOarsArtifact)] = new Tuple<double>(750.0);
            Entries[typeof(PricelessTreasureArtifact)] = new Tuple<double>(750.0);
            Entries[typeof(SkinnedDeerArtifact)] = new Tuple<double>(750.0);

            //ArtifactRarity 9 Stealable Artifacts
            Entries[typeof(Painting6WestArtifact)] = new Tuple<double>(1400.0);
            Entries[typeof(Painting6NorthArtifact)] = new Tuple<double>(1400.0);
            Entries[typeof(ManStatuetteSouthArtifact)] = new Tuple<double>(1400.0);
            Entries[typeof(ManStatuetteEastArtifact)] = new Tuple<double>(1400.0);
            Entries[typeof(SwordDisplay4NorthArtifact)] = new Tuple<double>(1400.0);
            Entries[typeof(SwordDisplay5WestArtifact)] = new Tuple<double>(1400.0);
            Entries[typeof(SwordDisplay5NorthArtifact)] = new Tuple<double>(1400.0);
            Entries[typeof(TyballsFlaskStandArtifact)] = new Tuple<double>(1400.0);
            Entries[typeof(BlockAndTackleArtifact)] = new Tuple<double>(1400.0);
            Entries[typeof(LeatherTunicArtifact)] = new Tuple<double>(1400.0);
            Entries[typeof(SaddleArtifact)] = new Tuple<double>(1400.0);

            //ArtifactRarity 10
            Entries[typeof(TitansHammer)] = new Tuple<double>(2750.0);
            Entries[typeof(ZyronicClaw)] = new Tuple<double>(2750.0);
            Entries[typeof(InquisitorsResolution)] = new Tuple<double>(2750.0);
            Entries[typeof(BladeOfTheRighteous)] = new Tuple<double>(2750.0);

            //Virtue Artifacts
            Entries[typeof(TenthAnniversarySculpture)] = new Tuple<double>(1500.0);
            Entries[typeof(MapOfTheKnownWorld)] = new Tuple<double>(1500.0);
            Entries[typeof(AnkhPendant)] = new Tuple<double>(1500.0);
            Entries[typeof(DragonsEnd)] = new Tuple<double>(1500.0);
            Entries[typeof(JaanasStaff)] = new Tuple<double>(1500.0);
            Entries[typeof(KatrinasCrook)] = new Tuple<double>(1500.0);
            Entries[typeof(LordBlackthornsExemplar)] = new Tuple<double>(1500.0);
            Entries[typeof(SentinelsGuard)] = new Tuple<double>(1500.0);
            Entries[typeof(CompassionArms)] = new Tuple<double>(1500.0);
            Entries[typeof(JusticeBreastplate)] = new Tuple<double>(1500.0);
            Entries[typeof(ValorGauntlets)] = new Tuple<double>(1500.0);
            Entries[typeof(HonestyGorget)] = new Tuple<double>(1500.0);
            Entries[typeof(SpiritualityHelm)] = new Tuple<double>(1500.0);
            Entries[typeof(HonorLegs)] = new Tuple<double>(1500.0);
            Entries[typeof(SacrificeSollerets)] = new Tuple<double>(1500.0);

            //Minor Artifacts (ML/Peerless/Tokuno)
            Entries[typeof(CandelabraOfSouls)] = new Tuple<double>(100.0);
            Entries[typeof(GhostShipAnchor)] = new Tuple<double>(100.0);
            Entries[typeof(GoldBricks)] = new Tuple<double>(100.0);
            Entries[typeof(PhillipsWoodenSteed)] = new Tuple<double>(100.0);
            Entries[typeof(SeahorseStatuette)] = new Tuple<double>(100.0);
            Entries[typeof(ShipModelOfTheHMSCape)] = new Tuple<double>(100.0);
            Entries[typeof(AdmiralsHeartyRum)] = new Tuple<double>(100.0);
            Entries[typeof(AlchemistsBauble)] = new Tuple<double>(100.0);
            Entries[typeof(ArcticDeathDealer)] = new Tuple<double>(100.0);
            Entries[typeof(BlazeOfDeath)] = new Tuple<double>(100.0);
            Entries[typeof(BurglarsBandana)] = new Tuple<double>(100.0);
            Entries[typeof(CaptainQuacklebushsCutlass)] = new Tuple<double>(100.0);
            Entries[typeof(CavortingClub)] = new Tuple<double>(100.0);
            Entries[typeof(DreadPirateHat)] = new Tuple<double>(100.0);
            Entries[typeof(EnchantedTitanLegBone)] = new Tuple<double>(100.0);
            Entries[typeof(GwennosHarp)] = new Tuple<double>(100.0);
            Entries[typeof(IolosLute)] = new Tuple<double>(100.0);
            Entries[typeof(LunaLance)] = new Tuple<double>(100.0);
            Entries[typeof(NightsKiss)] = new Tuple<double>(100.0);
            Entries[typeof(NoxRangersHeavyCrossbow)] = new Tuple<double>(100.0);
            Entries[typeof(PolarBearMask)] = new Tuple<double>(100.0);
            Entries[typeof(VioletCourage)] = new Tuple<double>(100.0);
            Entries[typeof(GlovesOfThePugilist)] = new Tuple<double>(100.0);
            Entries[typeof(PixieSwatter)] = new Tuple<double>(100.0);
            Entries[typeof(WrathOfTheDryad)] = new Tuple<double>(100.0);
            Entries[typeof(StaffOfPower)] = new Tuple<double>(100.0);
            Entries[typeof(OrcishVisage)] = new Tuple<double>(100.0);
            Entries[typeof(BowOfTheJukaKing)] = new Tuple<double>(100.0);
            Entries[typeof(ColdBlood)] = new Tuple<double>(100.0);
            Entries[typeof(CreepingVine)] = new Tuple<double>(100.0);
            Entries[typeof(ForgedPardon)] = new Tuple<double>(100.0);
            Entries[typeof(ManaPhasingOrb)] = new Tuple<double>(100.0);
            Entries[typeof(RunedSashOfWarding)] = new Tuple<double>(100.0);
            Entries[typeof(SurgeShield)] = new Tuple<double>(100.0);
            Entries[typeof(HeartOfTheLion)] = new Tuple<double>(100.0);
            Entries[typeof(ShieldOfInvulnerability)] = new Tuple<double>(100.0);
            Entries[typeof(AegisOfGrace)] = new Tuple<double>(100.0);
            Entries[typeof(BladeDance)] = new Tuple<double>(100.0);
            Entries[typeof(BloodwoodSpirit)] = new Tuple<double>(100.0);
            Entries[typeof(Bonesmasher)] = new Tuple<double>(100.0);
            Entries[typeof(Boomstick)] = new Tuple<double>(100.0);
            Entries[typeof(BrightsightLenses)] = new Tuple<double>(100.0);
            Entries[typeof(FeyLeggings)] = new Tuple<double>(100.0);
            Entries[typeof(FleshRipper)] = new Tuple<double>(100.0);
            Entries[typeof(HelmOfSwiftness)] = new Tuple<double>(100.0);
            Entries[typeof(PadsOfTheCuSidhe)] = new Tuple<double>(100.0);
            Entries[typeof(QuiverOfRage)] = new Tuple<double>(100.0);
            Entries[typeof(QuiverOfElements)] = new Tuple<double>(100.0);
            Entries[typeof(RaedsGlory)] = new Tuple<double>(100.0);
            Entries[typeof(RighteousAnger)] = new Tuple<double>(100.0);
            Entries[typeof(RobeOfTheEclipse)] = new Tuple<double>(100.0);
            Entries[typeof(RobeOfTheEquinox)] = new Tuple<double>(100.0);
            Entries[typeof(SoulSeeker)] = new Tuple<double>(100.0);
            Entries[typeof(TalonBite)] = new Tuple<double>(100.0);
            Entries[typeof(TotemOfVoid)] = new Tuple<double>(100.0);
            Entries[typeof(WildfireBow)] = new Tuple<double>(100.0);
            Entries[typeof(Windsong)] = new Tuple<double>(100.0);
            Entries[typeof(CrimsonCincture)] = new Tuple<double>(100.0);
            Entries[typeof(DreadFlute)] = new Tuple<double>(100.0);
            Entries[typeof(DreadsRevenge)] = new Tuple<double>(100.0);
            Entries[typeof(MelisandesCorrodedHatchet)] = new Tuple<double>(100.0);
            Entries[typeof(AlbinoSquirrelImprisonedInCrystal)] = new Tuple<double>(100.0);
            Entries[typeof(GrizzledMareStatuette)] = new Tuple<double>(100.0);
            Entries[typeof(GrizzleGauntlets)] = new Tuple<double>(100.0);
            Entries[typeof(GrizzleGreaves)] = new Tuple<double>(100.0);
            Entries[typeof(GrizzleHelm)] = new Tuple<double>(100.0);
            Entries[typeof(GrizzleTunic)] = new Tuple<double>(100.0);
            Entries[typeof(GrizzleVambraces)] = new Tuple<double>(100.0);
            Entries[typeof(ParoxysmusSwampDragonStatuette)] = new Tuple<double>(100.0);
            Entries[typeof(ScepterOfTheChief)] = new Tuple<double>(100.0);
            Entries[typeof(CrystallineRing)] = new Tuple<double>(100.0);
            Entries[typeof(MarkOfTravesty)] = new Tuple<double>(100.0);
            Entries[typeof(ImprisonedDog)] = new Tuple<double>(100.0);
            Entries[typeof(AncientFarmersKasa)] = new Tuple<double>(100.0);
            Entries[typeof(AncientSamuraiDo)] = new Tuple<double>(100.0);
            Entries[typeof(AncientUrn)] = new Tuple<double>(100.0);
            Entries[typeof(ArmsOfTacticalExcellence)] = new Tuple<double>(100.0);
            Entries[typeof(BlackLotusHood)] = new Tuple<double>(100.0);
            Entries[typeof(ChestOfHeirlooms)] = new Tuple<double>(100.0);
            Entries[typeof(DaimyosHelm)] = new Tuple<double>(100.0);
            Entries[typeof(DemonForks)] = new Tuple<double>(100.0);
            Entries[typeof(TheDestroyer)] = new Tuple<double>(100.0);
            Entries[typeof(DragonNunchaku)] = new Tuple<double>(100.0);
            Entries[typeof(Exiler)] = new Tuple<double>(100.0);
            Entries[typeof(FluteOfRenewal)] = new Tuple<double>(100.0);
            Entries[typeof(GlovesOfTheSun)] = new Tuple<double>(100.0);
            Entries[typeof(HanzosBow)] = new Tuple<double>(100.0);
            Entries[typeof(HonorableSwords)] = new Tuple<double>(100.0);
            Entries[typeof(LegsOfStability)] = new Tuple<double>(100.0);
            Entries[typeof(LeurociansMempoOfFortune)] = new Tuple<double>(100.0);
            Entries[typeof(PeasantsBokuto)] = new Tuple<double>(100.0);
            Entries[typeof(LesserPigmentsOfTokuno)] = new Tuple<double>(100.0);
            Entries[typeof(PilferedDancerFans)] = new Tuple<double>(100.0);
            Entries[typeof(TomeOfEnlightenment)] = new Tuple<double>(100.0);

            //Stygian Abyss Artifacts
            Entries[typeof(AbyssalBlade)] = new Tuple<double>(5000.0);
            Entries[typeof(AnimatedLegsoftheInsaneTinker)] = new Tuple<double>(5000.0);
            Entries[typeof(AxeOfAbandon)] = new Tuple<double>(5000.0);
            Entries[typeof(AxesOfFury)] = new Tuple<double>(5000.0);
            Entries[typeof(BansheesCall)] = new Tuple<double>(5000.0);
            Entries[typeof(BasiliskHideBreastplate)] = new Tuple<double>(5000.0);
            Entries[typeof(BladeOfBattle)] = new Tuple<double>(5000.0);
            Entries[typeof(BouraTailShield)] = new Tuple<double>(5000.0);
            Entries[typeof(BreastplateOfTheBerserker)] = new Tuple<double>(5000.0);
            Entries[typeof(BurningAmber)] = new Tuple<double>(5000.0);
            Entries[typeof(CastOffZombieSkin)] = new Tuple<double>(5000.0);
            Entries[typeof(CavalrysFolly)] = new Tuple<double>(5000.0);
            Entries[typeof(ChannelersDefender)] = new Tuple<double>(5000.0);
            Entries[typeof(ClawsOfTheBerserker)] = new Tuple<double>(5000.0);
            Entries[typeof(DeathsHead)] = new Tuple<double>(5000.0);
            Entries[typeof(DefenderOfTheMagus)] = new Tuple<double>(5000.0);
            Entries[typeof(DemonBridleRing)] = new Tuple<double>(5000.0);
            Entries[typeof(DemonHuntersStandard)] = new Tuple<double>(5000.0);
            Entries[typeof(DragonHideShield)] = new Tuple<double>(5000.0);
            Entries[typeof(DragonJadeEarrings)] = new Tuple<double>(5000.0);
            Entries[typeof(DraconisWrath)] = new Tuple<double>(5000.0);
            Entries[typeof(EternalGuardianStaff)] = new Tuple<double>(5000.0);
            Entries[typeof(FallenMysticsSpellbook)] = new Tuple<double>(5000.0);
            Entries[typeof(GiantSteps)] = new Tuple<double>(5000.0);
            Entries[typeof(IronwoodCompositeBow)] = new Tuple<double>(5000.0);
            Entries[typeof(JadeWarAxe)] = new Tuple<double>(5000.0);
            Entries[typeof(LegacyOfDespair)] = new Tuple<double>(5000.0);
            Entries[typeof(Lavaliere)] = new Tuple<double>(5000.0);
            Entries[typeof(LifeSyphon)] = new Tuple<double>(5000.0);
            Entries[typeof(Mangler)] = new Tuple<double>(5000.0);
            Entries[typeof(MantleOfTheFallen)] = new Tuple<double>(5000.0);
            Entries[typeof(MysticsGarb)] = new Tuple<double>(5000.0);
            Entries[typeof(NightEyes)] = new Tuple<double>(5000.0);
            Entries[typeof(ObsidianEarrings)] = new Tuple<double>(5000.0);
            Entries[typeof(PetrifiedSnake)] = new Tuple<double>(5000.0);
            Entries[typeof(PillarOfStrength)] = new Tuple<double>(5000.0);
            Entries[typeof(ProtectoroftheBattleMage)] = new Tuple<double>(5000.0);
            Entries[typeof(RaptorClaw)] = new Tuple<double>(5000.0);
            Entries[typeof(ResonantStaffofEnlightenment)] = new Tuple<double>(5000.0);
            Entries[typeof(ShroudOfTheCondemned)] = new Tuple<double>(5000.0);
            Entries[typeof(GargishSignOfOrder)] = new Tuple<double>(5000.0);
            Entries[typeof(HumanSignOfOrder)] = new Tuple<double>(5000.0);
            Entries[typeof(GargishSignOfChaos)] = new Tuple<double>(5000.0);
            Entries[typeof(HumanSignOfChaos)] = new Tuple<double>(5000.0);
            Entries[typeof(Slither)] = new Tuple<double>(5000.0);
            Entries[typeof(SpinedBloodwormBracers)] = new Tuple<double>(5000.0);
            Entries[typeof(StandardOfChaos)] = new Tuple<double>(5000.0);
            Entries[typeof(StandardOfChaosG)] = new Tuple<double>(5000.0);
            Entries[typeof(StaffOfShatteredDreams)] = new Tuple<double>(5000.0);
            Entries[typeof(StoneDragonsTooth)] = new Tuple<double>(5000.0);
            Entries[typeof(StoneSlithClaw)] = new Tuple<double>(5000.0);
            Entries[typeof(StormCaller)] = new Tuple<double>(5000.0);
            Entries[typeof(SwordOfShatteredHopes)] = new Tuple<double>(5000.0);
            Entries[typeof(SummonersKilt)] = new Tuple<double>(5000.0);
            Entries[typeof(Tangle1)] = new Tuple<double>(5000.0);
            Entries[typeof(TheImpalersPick)] = new Tuple<double>(5000.0);
            Entries[typeof(TorcOfTheGuardians)] = new Tuple<double>(5000.0);
            Entries[typeof(TokenOfHolyFavor)] = new Tuple<double>(5000.0);
            Entries[typeof(VampiricEssence)] = new Tuple<double>(5000.0);
            Entries[typeof(Venom)] = new Tuple<double>(5000.0);
            Entries[typeof(VoidInfusedKilt)] = new Tuple<double>(5000.0);
            Entries[typeof(WallofHungryMouths)] = new Tuple<double>(5000.0);

            //Tokuno Major Artifacts
            Entries[typeof(DarkenedSky)] = new Tuple<double>(2500.0);
            Entries[typeof(KasaOfTheRajin)] = new Tuple<double>(2500.0);
            Entries[typeof(PigmentsOfTokuno)] = new Tuple<double>(2500.0);
            Entries[typeof(RuneBeetleCarapace)] = new Tuple<double>(2500.0);
            Entries[typeof(Stormgrip)] = new Tuple<double>(2500.0);
            Entries[typeof(SwordOfTheStampede)] = new Tuple<double>(2500.0);
            Entries[typeof(SwordsOfProsperity)] = new Tuple<double>(2500.0);
            Entries[typeof(TheHorselord)] = new Tuple<double>(2500.0);
            Entries[typeof(TomeOfLostKnowledge)] = new Tuple<double>(2500.0);
            Entries[typeof(WindsEdge)] = new Tuple<double>(2500.0);

            //Replicas
            Entries[typeof(TatteredAncientMummyWrapping)] = new Tuple<double>(5000.0);
            Entries[typeof(WindSpirit)] = new Tuple<double>(5000.0);
            Entries[typeof(GauntletsOfAnger)] = new Tuple<double>(5000.0);
            Entries[typeof(GladiatorsCollar)] = new Tuple<double>(5000.0);
            Entries[typeof(OrcChieftainHelm)] = new Tuple<double>(5000.0);
            Entries[typeof(ShroudOfDeceit)] = new Tuple<double>(5000.0);
            Entries[typeof(AcidProofRobe)] = new Tuple<double>(5000.0);
            Entries[typeof(ANecromancerShroud)] = new Tuple<double>(5000.0);
            Entries[typeof(CaptainJohnsHat)] = new Tuple<double>(5000.0);
            Entries[typeof(CrownOfTalKeesh)] = new Tuple<double>(5000.0);

            Entries[typeof(DetectiveBoots)] = new Tuple<double>(5000.0);
            Entries[typeof(EmbroideredOakLeafCloak)] = new Tuple<double>(5000.0);
            Entries[typeof(JadeArmband)] = new Tuple<double>(5000.0);
            Entries[typeof(LieutenantOfTheBritannianRoyalGuard)] = new Tuple<double>(5000.0);
            Entries[typeof(MagicalDoor)] = new Tuple<double>(5000.0);
            Entries[typeof(RoyalGuardInvestigatorsCloak)] = new Tuple<double>(5000.0);
            Entries[typeof(SamaritanRobe)] = new Tuple<double>(5000.0);
            Entries[typeof(TheMostKnowledgePerson)] = new Tuple<double>(5000.0);
            Entries[typeof(TheRobeOfBritanniaAri)] = new Tuple<double>(5000.0);
            Entries[typeof(DjinnisRing)] = new Tuple<double>(5000.0);

            Entries[typeof(BraveKnightOfTheBritannia)] = new Tuple<double>(5000.0);
            Entries[typeof(Calm)] = new Tuple<double>(5000.0);
            Entries[typeof(FangOfRactus)] = new Tuple<double>(5000.0);
            Entries[typeof(OblivionsNeedle)] = new Tuple<double>(5000.0);
            Entries[typeof(Pacify)] = new Tuple<double>(5000.0);
            Entries[typeof(Quell)] = new Tuple<double>(5000.0);
            Entries[typeof(RoyalGuardSurvivalKnife)] = new Tuple<double>(5000.0);
            Entries[typeof(Subdue)] = new Tuple<double>(5000.0);
            Entries[typeof(Asclepius)] = new Tuple<double>(5000.0);
            Entries[typeof(BracersofAlchemicalDevastation)] = new Tuple<double>(5000.0);

            Entries[typeof(GargishAsclepius)] = new Tuple<double>(5000.0);
            Entries[typeof(GargishBracersofAlchemicalDevastation)] = new Tuple<double>(5000.0);
            Entries[typeof(HygieiasAmulet)] = new Tuple<double>(5000.0);
            Entries[typeof(ScrollofValiantCommendation)] = new Tuple<double>(5000.0);

            //Easter
            Entries[typeof(EasterEggs)] = new Tuple<double>(2.0);
            Entries[typeof(JellyBeans)] = new Tuple<double>(2.0);

            //Miscellaneous
            Entries[typeof(ScrollofTranscendence)] = new Tuple<double>(2.0);

            //Miscellaneous
            Entries[typeof(Gold)] = new Tuple<double>(0.01);
            Entries[typeof(RedScales)] = new Tuple<double>(0.10);
            Entries[typeof(YellowScales)] = new Tuple<double>(0.10);
            Entries[typeof(BlackScales)] = new Tuple<double>(0.10);
            Entries[typeof(GreenScales)] = new Tuple<double>(0.10);
            Entries[typeof(WhiteScales)] = new Tuple<double>(0.10);
            Entries[typeof(BlueScales)] = new Tuple<double>(0.10);
            Entries[typeof(Bottle)] = new Tuple<double>(0.25);
            Entries[typeof(OrcishKinMask)] = new Tuple<double>(100.0);
            Entries[typeof(PottedPlantDeed)] = new Tuple<double>(15000.0);
            Entries[typeof(BagOfSending)] = new Tuple<double>(250.0);
            Entries[typeof(BookOfTruthArtifact)] = new Tuple<double>(1600.0);
            Entries[typeof(Cauldron)] = new Tuple<double>(200.0);
            Entries[typeof(ChampionSkull)] = new Tuple<double>(1000.0);
            Entries[typeof(ChaosShield)] = new Tuple<double>(2500.0);
            Entries[typeof(ClockworkAssembly)] = new Tuple<double>(50.0);
            Entries[typeof(ConjurersTrinket)] = new Tuple<double>(10000.0);

            Entries[typeof(CorgulsHandbookOnMysticism)] = new Tuple<double>(250.0);
            Entries[typeof(CrownOfArcaneTemperament)] = new Tuple<double>(5000.0);
            Entries[typeof(DeadWood)] = new Tuple<double>(1.0);
            Entries[typeof(DreadFlute)] = new Tuple<double>(1000.0);
            Entries[typeof(DustyPillow)] = new Tuple<double>(250.0);
            Entries[typeof(EndlessDecanter)] = new Tuple<double>(10.0);
            Entries[typeof(EternallyCorruptTree)] = new Tuple<double>(1000.0);
            Entries[typeof(ExcellentIronMaiden)] = new Tuple<double>(50.0);
            Entries[typeof(ExecutionersCap)] = new Tuple<double>(1.0);
            Entries[typeof(Flowstone)] = new Tuple<double>(250.0);
            Entries[typeof(ForgedPardon)] = new Tuple<double>(500.0);
            Entries[typeof(GlacialStaff)] = new Tuple<double>(500.0);
            Entries[typeof(GoldBricks)] = new Tuple<double>(100.0);
            Entries[typeof(GrapeVine)] = new Tuple<double>(500.0);
            Entries[typeof(GrobusFur)] = new Tuple<double>(20.0);
            Entries[typeof(HorseShoes)] = new Tuple<double>(200.0);

            Entries[typeof(JackalsCollar)] = new Tuple<double>(5500.0);
            Entries[typeof(JocklesQuicksword)] = new Tuple<double>(2.0);
            Entries[typeof(LavaRock)] = new Tuple<double>(500.0);
            Entries[typeof(ManaPhasingOrb)] = new Tuple<double>(500.0);
            Entries[typeof(MangledHeadOfDreadhorn)] = new Tuple<double>(1000.0);
            Entries[typeof(MedusaBlood)] = new Tuple<double>(1000.0);
            Entries[typeof(MedusaDarkScales)] = new Tuple<double>(200.0);
            Entries[typeof(MedusaLightScales)] = new Tuple<double>(200.0);
            Entries[typeof(MidnightBracers)] = new Tuple<double>(5000.0);
            Entries[typeof(MiniHouseDeed)] = new Tuple<double>(6500.0);
            Entries[typeof(Moonstone)] = new Tuple<double>(5000.0);
            Entries[typeof(MysticsGuard)] = new Tuple<double>(2500.0);
            Entries[typeof(PowerCrystal)] = new Tuple<double>(100.0);
            Entries[typeof(PristineDreadHorn)] = new Tuple<double>(1000.0);
            Entries[typeof(ProspectorsTool)] = new Tuple<double>(3.0);
            Entries[typeof(RecipeScroll)] = new Tuple<double>(10.0);
            Entries[typeof(ShroudOfTheCondemned)] = new Tuple<double>(500.0);

            Entries[typeof(SwampTile)] = new Tuple<double>(5000.0);
            Entries[typeof(TastyTreat)] = new Tuple<double>(100.0);
            Entries[typeof(TatteredAncientScroll)] = new Tuple<double>(200.0);
            Entries[typeof(ThorvaldsMedallion)] = new Tuple<double>(250.0);
            Entries[typeof(TribalBerry)] = new Tuple<double>(10.0);
            Entries[typeof(TunicOfGuarding)] = new Tuple<double>(2.0);
            Entries[typeof(UndeadGargHorn)] = new Tuple<double>(1000.0);
            Entries[typeof(UntransTome)] = new Tuple<double>(200.0);
            Entries[typeof(WallBlood)] = new Tuple<double>(5000.0);
            Entries[typeof(Whip)] = new Tuple<double>(200.0);
        }
    }

    public class AppraiseforCleanupTarget : Target
    {
        private Mobile m_Mobile;

        public AppraiseforCleanupTarget(Mobile from) : base(-1, true, TargetFlags.None)
        {
            m_Mobile = from;
        }

        protected override void OnTarget(Mobile m, object targeted)
        {
            if (targeted is Item)
            {
                Item item = (Item)targeted;
                
                double points = CleanUpBritanniaData.GetPoints(item);
                    
                if(points == 0)
                    m_Mobile.SendLocalizedMessage(1151271); // This item has no turn-in value for Clean Up Britannia.
                if (points < 1)
                    m_Mobile.SendLocalizedMessage(1151272, points.ToString()); // This item is worth less than one point for Clean Up Britannia.
                else if(points == 1)
                    m_Mobile.SendLocalizedMessage(1151273, points.ToString()); // This item is worth approximately one point for Clean Up Britannia.
                else
                    m_Mobile.SendLocalizedMessage(1151274, points.ToString()); //This item is worth approximately ~1_VALUE~ points for Clean Up Britannia.                
            }
            else
                m_Mobile.SendLocalizedMessage(1151271); // This item has no turn-in value for Clean Up Britannia.
        }
    }
}