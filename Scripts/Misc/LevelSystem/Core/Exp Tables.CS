using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server
{
    //This File Is Used For ADVANCED Monster Exp
    //And The Armour Craft Settings.
    //It Is Not Recommended For Beginners
    //A Tutorial On How To Add Your Own Tables And
    //How To Add Monsters Will Be Created Latter.

    public class armours // This is for craft xp.
    {
        public static Type[] Ringmail = new Type[]
            {
                typeof(RingmailGloves),
                typeof(RingmailLegs),
                typeof(RingmailArms),
                typeof(RingmailChest)
            };

        public static Type[] Chainmail = new Type[]
            {
                typeof(ChainCoif),
                typeof(ChainChest),
                typeof(ChainLegs)
            };

        public static Type[] Platemail = new Type[]
            {
                typeof(PlateHelm),
                typeof(PlateArms),
                typeof(PlateGloves),
                typeof(PlateChest),
                typeof(PlateLegs),
                typeof(PlateGorget),
                typeof(FemalePlateChest)
            };
    }

    public class exp //this is for advanced monster exp.
    {
        #region No Exp
        public static Type[] NoExp = new Type[]
                {
                    //AosMonsters
                    typeof(AbysmalHorror),
                    typeof(BoneDemon),
                    typeof(CrystalElemental),
                    typeof(DarknightCreeper),
                    typeof(DemonKnight),
                    typeof(Devourer),
                    typeof(FleshGolem),
                    typeof(FleshRenderer),
                    typeof(Gibberling),
                    typeof(GoreFiend),
                    typeof(Impaler),
                    typeof(MoundOfMaggots),
                    typeof(PatchworkSkeleton),
                    typeof(Ravager),
                    typeof(Revenant),
                    typeof(ShadowKnight),
                    typeof(SkitteringHopper),
                    typeof(Treefellow),
                    typeof(VampireBat),
                    typeof(WailingBanshee),
                    typeof(WandererOfTheVoid),
                    typeof(IceElemental),
                    typeof(Efreet),
                    typeof(Bogle),
                    typeof(SeaHorse),
                    typeof(SilverSteed),
                    //Elementals
                    typeof(SummonedAirElemental),
                    typeof(SummonedDaemon),
                    typeof(SummonedEarthElemental),
                    typeof(SummonedFireElemental),
                    typeof(SummonedWaterElemental),
                    typeof(SpawnedOrcishLord),
                    typeof(KhaldunRevenant),
                    //Familiars
                    typeof(DarkWolfFamiliar),
                    typeof(DeathAdder),
                    typeof(HordeMinionFamiliar),
                    typeof(ShadowWispFamiliar),
                    typeof(VampireBatFamiliar)
                };

        #endregion

        #region Extremly Low Exp
        public static Type[] ExtLowExp = new Type[]
                {
                    typeof(Cow),
                    typeof(Rat),
                    typeof(Cat),
                    typeof(Dog),
                    typeof(Bird),
                    typeof(TropicalBird),
                    typeof(Eagle),
                    typeof(GreatHart),
                    typeof(Hind),
                    typeof(Llama),
                    typeof(MountainGoat),
                    typeof(PackHorse),
                    typeof(PackLlama),
                    typeof(Walrus),
                    typeof(Goat),
                    typeof(BullFrog),
                    typeof(Dolphin),
                    typeof(Snake),
                    typeof(JackRabbit),
                    typeof(Rabbit),
                    typeof(Sewerrat),
                    typeof(Jwilson),
                    typeof(Mongbat),
                    typeof(StrongMongbat),
                    typeof(Slime),
                    typeof(ShadowWisp),
                    typeof(FrostOoze)
                };

        #endregion

        #region Very Low Exp
        public static Type[] VeryLowExp = new Type[]
                {
                    //bears
                    typeof(BlackBear),
                    typeof(BrownBear),
                    typeof(GrizzlyBear),
                    typeof(PolarBear),
                    //birds
                    typeof(Crane),
                    typeof(Phoenix),
                    //wolfs
                    typeof(DireWolf),
                    typeof(GreyWolf),
                    typeof(TimberWolf),
                    typeof(WhiteWolf),
                    //felines
                    typeof(Cougar),
                    typeof(HellCat),
                    typeof(Panther),
                    typeof(PredatorHellCat),
                    typeof(SnowLeopard),
                    //Misc
                    typeof(GiantToad),
                    typeof(Gorilla),
                    //Reptiles
                    typeof(Alligator),
                    typeof(GiantSerpent),
                    typeof(IceSerpent),
                    typeof(IceSnake),
                    typeof(LavaLizard),
                    typeof(LavaSerpent),
                    //Rodents
                    typeof(GiantRat),
                    //other
                    typeof(Horse),
                    typeof(RidableLlama),
                    typeof(HeadlessOne),
                    typeof(HordeMinion),
                    typeof(BoneMagi),
                    typeof(Shade),
                    typeof(Spectre),
                    typeof(Wraith),
                    typeof(Zombie),
                    typeof(Imp),
                    typeof(Brigand),
                    typeof(DesertOstard),
                    typeof(ForestOstard),
                    typeof(FrenziedOstard),
                    typeof(Ridgeback),
                    typeof(SavageRidgeback),
                    typeof(SkeletalMount),
                    typeof(RedSolenWorker),
                    typeof(BlackSolenWorker),
                    typeof(EvilMage),
                    typeof(FrostSpider),
                    typeof(GiantSpider),
                    typeof(TerathanDrone),
                    typeof(GiantBlackWidow),
                    typeof(Cursed),
                    typeof(Ettin),
                    typeof(GazerLarva),
                    typeof(Ghoul),
                    typeof(Guardian),
                    typeof(Orc),
                    typeof(Ratman),
                    typeof(RestlessSoul),
                    typeof(Savage),
                    typeof(ShadowFiend),
                    typeof(Skeleton),
                    typeof(Troll),
                    typeof(HellHound),
                    typeof(DarkWisp),
                    typeof(Bogling),
                    typeof(SwampTentacle),
                    typeof(Harpy),
                    typeof(Lizardman),
                    typeof(Scorpion),
                    typeof(DeathwatchBeetleHatchling)
                };

        #endregion

        #region Low Exp
        public static Type[] LowExp = new Type[]
                {
                    typeof(AirElemental),
                    typeof(FireElemental),
                    typeof(WaterElemental),
                    typeof(EarthElemental)
                };

        #endregion

        #region Low Medium Exp
        public static Type[] LowMedExp = new Type[]
                {
                    typeof(Beetle),
                    typeof(HellSteed),
                    typeof(Kirin),
                    typeof(ScaledSwampDragon),
                    typeof(SwampDragon),
                    typeof(Unicorn),
                    //other
                    typeof(BlackSolenWarrior),
                    typeof(RedSolenWarrior),
                    typeof(RedSolenInfiltratorWarrior),
                    typeof(BlackSolenInfiltratorWarrior),
                    typeof(EvilMageLord),
                    typeof(Gargoyle),
                    typeof(Gazer),
                    typeof(GolemController),
                    typeof(Lich),
                    typeof(OrcishMage),
                    typeof(RatmanMage),
                    typeof(SavageShaman),
                    typeof(SkeletalMage),
                    typeof(BoneKnight),
                    typeof(Gaman),
                    typeof(TerathanWarrior),
                    typeof(ChaosDaemon),
                    typeof(Doppleganger),
                    typeof(FrostTroll),
                    typeof(Ogre),
                    typeof(OrcBomber),
                    typeof(OrcCaptain),
                    typeof(OrcishLord),
                    typeof(RatmanArcher),
                    typeof(SavageRider),
                    typeof(SkeletalKnight),
                    typeof(SpectralArmour),
                    typeof(StoneGargoyle),
                    typeof(MeerCaptain),
                    typeof(MeerWarrior),
                    typeof(Pixie),
                    typeof(BladeSpirits),
                    typeof(SandVortex),
                    typeof(Reaper),
                    typeof(Corpser),
                    typeof(Quagmire),
                    typeof(OphidianMage),
                    typeof(SeaSerpent),
                    typeof(DeathwatchBeetle),
                    typeof(Kappa)
                };

        #endregion

        #region Medium Exp
        public static Type[] MedExp = new Type[]
                {
                    typeof(AntLion),
                    typeof(BlackSolenInfiltratorQueen),
                    typeof(BlackSolenQueen),
                    typeof(RedSolenInfiltratorQueen),
                    typeof(RedSolenQueen),
                    //other
                    typeof(Nightmare),
                    typeof(FireSteed),
                    typeof(DreadSpider),
                    typeof(ElderGazer),
                    typeof(Cyclops),
                    typeof(EnslavedGargoyle),
                    typeof(Juggernaut),
                    typeof(Moloch),
                    typeof(Mummy),
                    //Jukas
                    typeof(JukaLord),
                    typeof(JukaMage),
                    typeof(JukaWarrior),
                    typeof(MeerMage),
                    typeof(ArcaneDaemon),
                    typeof(Centaur),
                    typeof(EnergyVortex),
                    typeof(PlagueSpawn),
                    //Eles
                    typeof(BronzeElemental),
                    typeof(CopperElemental),
                    typeof(DullCopperElemental),
                    typeof(GoldenElemental),
                    typeof(ShadowIronElemental),
                    typeof(VeriteElemental),
                    typeof(WhippingVine),
                    typeof(DeepSeaSerpent),
                    typeof(OphidianArchmage),
                    typeof(OphidianWarrior),
                    typeof(StoneHarpy),
                    typeof(Wyvern)
                };

        #endregion

        #region High Medium Exp
        public static Type[] HighMedExp = new Type[]
                {
                    //magic
                    typeof(TerathanAvenger),
                    typeof(TerathanMatriarch),
                    //other
                    typeof(AcidElemental),
                    typeof(SnowElemental),
                    typeof(Betrayer),
                    typeof(Daemon),
                    typeof(FireGargoyle),
                    typeof(Executioner),
                    typeof(Wisp),
                    typeof(Golem),
                    typeof(EliteNinja),
                    typeof(FanDancer),
                    typeof(FireBeetle),
                    typeof(KazeKemono),
                    typeof(RaiJu),
                    typeof(RevenantLion),
                    typeof(Ronin)
                };

        #endregion

        #region High Exp
        public static Type[] HighExp = new Type[]
                {
                    typeof(LesserHiryu),
                    typeof(BloodElemental),
                    typeof(PoisonElemental),
                    typeof(KhaldunZealot),
                    typeof(PlagueBeast),
                    typeof(AgapiteElemental),
                    typeof(ValoriteElemental),
                    typeof(OphidianMatriarch),
                    typeof(Drake),
                    typeof(YomotsuWarrior)
                };

        #endregion

        #region Very High Exp
        public static Type[] VeryHighExp = new Type[]
                {
                    typeof(ArcticOgreLord),
                    typeof(KhaldunSummoner),
                    typeof(OgreLord),
                    typeof(OrcBrute),
                    typeof(RottingCorpse),
                    //exodus
                    typeof(ExodusMinion),
                    typeof(ExodusOverseer),
                    typeof(LichLord),
                    typeof(IceFiend),
                    typeof(Succubus),
                    typeof(Titan),
                    typeof(GargoyleDestroyer),
                    typeof(GargoyleEnforcer),
                    typeof(EtherealWarrior),
                    typeof(BogThing),
                    typeof(Dragon),
                    typeof(SerpentineDragon),
                    typeof(OphidianKnight),
                    typeof(BakeKitsune),
                    typeof(TsukiWolf),
                    typeof(YomotsuPriest)
                };

        #endregion

        #region Extremly High Exp
        public static Type[] ExtHighExp = new Type[]
                {
                    //Meers
                    typeof(MeerEternal),
                    //other
                    typeof(AncientLich),
                    typeof(Balron),
                    typeof(Hiryu),
                    typeof(VorpalBunny),
                    typeof(WhiteWyrm),
                    typeof(Kraken),
                    typeof(LadyOfTheSnow),
                    typeof(Oni),
                    typeof(RuneBeetle),
                    typeof(YomotsuElder)
                };

        #endregion

        #region Extra High
        public static Type[] ExtHigh = new Type[]
                {
                    typeof(AncientWyrm),
                    typeof(Leviathan),
                    typeof(ShadowWyrm),
                    typeof(SkeletalDragon),
                    typeof(Yamandon)
                };

        #endregion
    }
}