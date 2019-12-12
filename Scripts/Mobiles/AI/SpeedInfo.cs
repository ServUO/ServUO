#region References
using System;
using System.Collections;

using Server.Factions;
using Server.Mobiles;
#endregion

namespace Server
{
	public class SpeedInfo
	{
        public static readonly double MinDelay = 0.1;
        public static readonly double MaxDelay = 0.5;
        public static readonly double MinDelayWild = 0.4;
        public static readonly double MaxDelayWild = 0.8;

        public static bool GetSpeedsNew(BaseCreature bc, ref double activeSpeed, ref double passiveSpeed)
        {
            var maxDex = GetMaxMovementDex(bc);
            var dex = Math.Min(maxDex, Math.Max(25, bc.Dex));

            var min = bc.IsMonster || InActiveCombat(bc) ? MinDelayWild : MinDelay;
            var max = bc.IsMonster || InActiveCombat(bc) ? MaxDelayWild : MaxDelay;

            if (bc.IsParagon)
            {
                min /= 2;
                max = min + .4;
            }

            activeSpeed = max - ((max - min) * ((double)dex / maxDex));

            if (activeSpeed < min)
            {
                activeSpeed = min;
            }

            passiveSpeed = activeSpeed * 2;

            return true;
        }

        private static int GetMaxMovementDex(BaseCreature bc)
        {
            return bc.IsMonster ? 150 : 190;
        }

        public static bool InActiveCombat(BaseCreature bc)
        {
            return bc.Combatant != null && bc.ControlOrder != OrderType.Follow;
        }

        public static double TransformMoveDelay(BaseCreature bc, double delay)
        {
            var adjusted = bc.IsMonster ? MaxDelayWild : MaxDelay;

            if (!bc.IsDeadPet && (bc.ReduceSpeedWithDamage || bc.IsSubdued))
            {
                var offset = (double)bc.Stam / (double)bc.StamMax;

                if (offset < 1.0)
                {
                    delay = delay + ((adjusted - delay) * (1.0 - offset));
                }
            }

            if (delay > adjusted)
            {
                delay = adjusted;
            }

            return delay;
        }

        public static bool UseNewSpeeds { get { return true; } }
		public double ActiveSpeed { get; set; }
		public double PassiveSpeed { get; set; }

		public Type[] Types { get; set; }

		public SpeedInfo(double activeSpeed, double passiveSpeed, Type[] types)
		{
			ActiveSpeed = activeSpeed;
			PassiveSpeed = passiveSpeed;
			Types = types;
		}

		/*public static bool Contains(object obj)
		{
			if (UseNewSpeeds)
				return false;

			if (m_Table == null)
				LoadTable();

			var sp = (SpeedInfo)m_Table[obj.GetType()];

			return (sp != null);
		}*/

        public static bool GetSpeeds(BaseCreature bc, ref double activeSpeed, ref double passiveSpeed)
		{
            if (UseNewSpeeds)
            {
                GetSpeedsNew(bc, ref activeSpeed, ref passiveSpeed);
            }
            else
            {
                if (m_Table == null)
                    LoadTable();

                var sp = (SpeedInfo)m_Table[bc.GetType()];

                if (sp == null)
                    return false;

                activeSpeed = sp.ActiveSpeed;
                passiveSpeed = sp.PassiveSpeed;
            }

			return true;
		}

		private static void LoadTable()
		{
			m_Table = new Hashtable();

			for (var i = 0; i < m_Speeds.Length; ++i)
			{
				var info = m_Speeds[i];
				var types = info.Types;

				for (var j = 0; j < types.Length; ++j)
					m_Table[types[j]] = info;
			}
		}

		private static Hashtable m_Table;

		private static readonly SpeedInfo[] m_Speeds =
		{
			// Slow
			new SpeedInfo(
				0.3,
				0.6,
				new[]
				{
					typeof(AntLion), typeof(ArcticOgreLord), typeof(BogThing), typeof(Bogle), typeof(BoneKnight),
					typeof(EarthElemental), typeof(Ettin), typeof(FrostOoze), typeof(FrostTroll), typeof(GazerLarva), typeof(Ghoul),
					typeof(Golem), typeof(HeadlessOne), typeof(Jwilson), typeof(Mummy), typeof(Ogre), typeof(OgreLord),
					typeof(PlagueBeast), typeof(Quagmire), typeof(Rat), typeof(RottingCorpse), typeof(Sewerrat), typeof(Skeleton),
					typeof(Slime), typeof(Zombie), typeof(Walrus), typeof(RestlessSoul), typeof(CrystalElemental),
					typeof(DarknightCreeper), typeof(MoundOfMaggots), typeof(Juggernaut), typeof(Yamandon), typeof(Serado),
					typeof(RuddyBoura), typeof(LowlandBoura), typeof(HighPlainsBoura), typeof(Relanord), typeof(Ortanord),
					typeof(Korpre), typeof(Anzuanord), typeof(Anlorzen), typeof(UndeadGuardian), typeof(PutridUndeadGuardian),
					typeof(CorgulTheSoulBinder), typeof(GooeyMaggots), typeof(Fezzik), typeof(Ronin)
				}),
            // Fast
            new SpeedInfo(
				0.2,
				0.4,
				new[]
				{
					typeof(LordOaks), typeof(Silvani), typeof(AirElemental), typeof(AncientWyrm), typeof(Balron), typeof(BladeSpirits),
					typeof(DreadSpider), typeof(Efreet), typeof(EtherealWarrior), typeof(Lich), typeof(Nightmare),
					typeof(OphidianArchmage), typeof(OphidianMage), typeof(OphidianWarrior), typeof(OphidianMatriarch),
					typeof(OphidianKnight), typeof(PoisonElemental), typeof(Revenant), typeof(SandVortex), typeof(SavageRider),
					typeof(SavageShaman), typeof(SnowElemental), typeof(WhiteWyrm), typeof(Wisp), typeof(DemonKnight),
					typeof(GiantBlackWidow), typeof(SummonedAirElemental), typeof(LesserHiryu), typeof(Hiryu), typeof(LadyOfTheSnow),
					typeof(RaiJu), typeof(RuneBeetle), typeof(Changeling), typeof(SentinelSpider), typeof(Anlorvaglem), typeof(Medusa),
					typeof(PrimevalLich), typeof(StygianDragon), typeof(CoralSnake), typeof(DarkWisp), typeof(DreamWraith),
					typeof(FireAnt), typeof(KepetchAmbusher), typeof(LavaElemental), typeof(MaddeningHorror), typeof(Wight),
					typeof(WolfSpider), typeof(UndeadGargoyle), typeof(SlasherOfVeils), typeof(SavagePackWolf), typeof(DemonicJailor),
					typeof(SilverSerpent),

					#region ML named mobs - before Paragon speedboost
					typeof(LadyJennifyr), typeof(LadyMarai), typeof(MasterJonath), typeof(MasterMikael), typeof(MasterTheophilus),
					typeof(RedDeath), typeof(SirPatrick), typeof(Rend), typeof(Grobu), typeof(Gnaw), typeof(Guile), typeof(Irk),
					typeof(Spite), typeof(LadyLissith), typeof(LadySabrix), typeof(Malefic), typeof(Silk), typeof(Virulent)
					// TODO: Where to put Lurg, Putrefier, Swoop and Pyre? They seem slower.
					#endregion
				}),
			// Very Fast
			new SpeedInfo(
				0.175,
				0.350,
				new[]
				{
					typeof(Barracoon), typeof(Neira), typeof(Rikktor), typeof(EnergyVortex), typeof(EliteNinja), typeof(Pixie),
					typeof(FleshRenderer), typeof(KhaldunRevenant), typeof(FactionDragoon), typeof(FactionKnight),
					typeof(FactionPaladin), typeof(FactionHenchman), typeof(FactionMercenary), typeof(FactionNecromancer),
					typeof(FactionSorceress), typeof(FactionWizard), typeof(FactionBerserker), typeof(FactionPaladin),
					typeof(Leviathan), typeof(FireBeetle), typeof(FanDancer), typeof(FactionDeathKnight), typeof(ClockworkExodus),
					typeof(Navrey), typeof(Raptor), typeof(TrapdoorSpider)
				}),
			// Extremely Fast
            new SpeedInfo(0.08, 0.20, new[] {typeof(Miasma), typeof(Semidar), typeof(Mephitis)}),
			// Medium
			new SpeedInfo(
				0.25,
				0.5,
				new[]
				{
					typeof(ToxicElemental), typeof(AgapiteElemental), typeof(Alligator), typeof(AncientLich), typeof(Betrayer),
					typeof(Bird), typeof(BlackBear), typeof(BlackSolenInfiltratorQueen), typeof(BlackSolenInfiltratorWarrior),
					typeof(BlackSolenQueen), typeof(BlackSolenWarrior), typeof(BlackSolenWorker), typeof(BloodElemental), typeof(Boar),
					typeof(Bogling), typeof(BoneMagi), typeof(Brigand), typeof(BronzeElemental), typeof(BrownBear), typeof(Bull),
					typeof(BullFrog), typeof(Cat), typeof(Centaur), typeof(ChaosDaemon), typeof(Chicken), typeof(GolemController),
					typeof(CopperElemental), typeof(CopperElemental), typeof(Cougar), typeof(Cow), typeof(Cyclops), typeof(Daemon),
					typeof(DeepSeaSerpent), typeof(DesertOstard), typeof(DireWolf), typeof(Dog), typeof(Dolphin), typeof(Dragon),
					typeof(Drake), typeof(DullCopperElemental), typeof(Eagle), typeof(ElderGazer), typeof(EvilMage),
					typeof(EvilMageLord), typeof(Executioner), typeof(Savage), typeof(FireElemental), typeof(FireGargoyle),
					typeof(FireSteed), typeof(ForestOstard), typeof(FrenziedOstard), typeof(FrostSpider), typeof(Gargoyle),
					typeof(Gazer), typeof(IceSerpent), typeof(GiantRat), typeof(GiantSerpent), typeof(GiantSpider), typeof(GiantToad),
					typeof(Goat), typeof(GoldenElemental), typeof(Gorilla), typeof(GreatHart), typeof(GreyWolf), typeof(GrizzlyBear),
					typeof(Guardian), typeof(Harpy), typeof(Harrower), typeof(HellHound), typeof(Hind), typeof(HordeMinion),
					typeof(Horse), typeof(IceElemental), typeof(IceFiend), typeof(IceSnake), typeof(Imp), typeof(JackRabbit),
					typeof(Kirin), typeof(Kraken), typeof(PredatorHellCat), typeof(LavaLizard), typeof(LavaSerpent), typeof(LavaSnake),
					typeof(Lizardman), typeof(Llama), typeof(Mongbat), typeof(StrongMongbat), typeof(MountainGoat), typeof(Orc),
					typeof(OrcBomber), typeof(OrcBrute), typeof(OrcCaptain), typeof(OrcishLord), typeof(OrcishMage), typeof(PackHorse),
					typeof(PackLlama), typeof(Panther), typeof(Pig), typeof(PlagueSpawn), typeof(PolarBear), typeof(Rabbit),
					typeof(Ratman), typeof(RatmanArcher), typeof(RatmanMage), typeof(RedSolenInfiltratorQueen),
					typeof(RedSolenInfiltratorWarrior), typeof(RedSolenQueen), typeof(RedSolenWarrior), typeof(RedSolenWorker),
					typeof(RidableLlama), typeof(Ridgeback), typeof(Scorpion), typeof(SeaSerpent), typeof(SerpentineDragon),
					typeof(Shade), typeof(ShadowIronElemental), typeof(ShadowWisp), typeof(ShadowWyrm), typeof(Sheep),
					typeof(SilverSteed), typeof(SkeletalDragon), typeof(SkeletalMage), typeof(SkeletalMount), typeof(HellCat),
					typeof(Snake), typeof(SnowLeopard), typeof(SpectralArmour), typeof(Spectre), typeof(StoneGargoyle),
					typeof(StoneHarpy), typeof(SwampDragon), typeof(ScaledSwampDragon), typeof(SwampTentacle), typeof(TerathanAvenger),
					typeof(TerathanDrone), typeof(TerathanMatriarch), typeof(TerathanWarrior), typeof(TimberWolf), typeof(Titan),
					typeof(Troll), typeof(Unicorn), typeof(ValoriteElemental), typeof(VeriteElemental), typeof(CoMWarHorse),
					typeof(MinaxWarHorse), typeof(SLWarHorse), typeof(TBWarHorse), typeof(WaterElemental), typeof(WhippingVine),
					typeof(WhiteWolf), typeof(Wraith), typeof(Wyvern), typeof(KhaldunZealot), typeof(KhaldunSummoner),
					typeof(SavageRidgeback), typeof(LichLord), typeof(SkeletalKnight), typeof(SummonedDaemon),
					typeof(SummonedEarthElemental), typeof(SummonedWaterElemental), typeof(SummonedFireElemental), typeof(MeerWarrior),
					typeof(MeerEternal), typeof(MeerMage), typeof(MeerCaptain), typeof(JukaLord), typeof(JukaMage),
					typeof(JukaWarrior), typeof(AbysmalHorror), typeof(BoneDemon), typeof(Devourer), typeof(FleshGolem),
					typeof(Gibberling), typeof(GoreFiend), typeof(Impaler), typeof(PatchworkSkeleton), typeof(Ravager),
					typeof(ShadowKnight), typeof(SkitteringHopper), typeof(Treefellow), typeof(VampireBat), typeof(WailingBanshee),
					typeof(WandererOfTheVoid), typeof(Cursed), typeof(GrimmochDrummel), typeof(LysanderGathenwale), typeof(MorgBergen),
					typeof(ShadowFiend), typeof(SpectralArmour), typeof(TavaraSewel), typeof(ArcaneDaemon), typeof(Doppleganger),
					typeof(EnslavedGargoyle), typeof(ExodusMinion), typeof(ExodusOverseer), typeof(GargoyleDestroyer),
					typeof(GargoyleEnforcer), typeof(Moloch), typeof(BakeKitsune), typeof(DeathwatchBeetleHatchling), typeof(Kappa),
					typeof(KazeKemono), typeof(DeathwatchBeetle), typeof(TsukiWolf), typeof(YomotsuElder), typeof(YomotsuPriest),
					typeof(YomotsuWarrior), typeof(RevenantLion), typeof(Oni), typeof(Gaman), typeof(Crane), typeof(Beetle),
					typeof(ColdDrake), typeof(Vasanord), typeof(Ballem), typeof(Betballem), typeof(Anlorlem), typeof(BloodWorm),
					typeof(GreenGoblin), typeof(EnslavedGreenGoblin), typeof(GreenGoblinAlchemist), typeof(GreenGoblinScout),
					typeof(Kepetch), typeof(ClanRC), typeof(EnslavedGoblinKeeper), typeof(EnslavedGoblinMage),
					typeof(EnslavedGoblinScout), typeof(EnslavedGrayGoblin), typeof(GrayGoblin), typeof(GrayGoblinKeeper),
					typeof(GrayGoblinMage), typeof(Gremlin), typeof(SkeletalDrake), typeof(Slith), typeof(StoneSlith),
					typeof(ToxicSlith), typeof(WyvernRenowned), typeof(GrayGoblinMageRenowned), typeof(FireDaemon), typeof(AcidSlug)
				})
		};
	}
}
