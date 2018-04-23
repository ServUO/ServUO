using System;
using Server;
using Server.Network;
using Server.Gumps;

namespace Server.Items
{
    public class SpellBarScroll : Item
    {
        // MAGERY
        public int mW00_ClumsySpell = 0;
        public int mW01_CreateFoodSpell = 0;
        public int mW02_FeeblemindSpell = 0;
        public int mW03_HealSpell = 0;
        public int mW04_MagicArrowSpell = 0;
        public int mW05_NightSightSpell = 0;
        public int mW06_ReactiveArmorSpell = 0;
        public int mW07_WeakenSpell = 0;
        public int mW08_AgilitySpell = 0;
        public int mW09_CunningSpell = 0;
        public int mW10_CureSpell = 0;
        public int mW11_HarmSpell = 0;
        public int mW12_MagicTrapSpell = 0;
        public int mW13_RemoveTrapSpell = 0;
        public int mW14_ProtectionSpell = 0;
        public int mW15_StrengthSpell = 0;
        public int mW16_BlessSpell = 0;
        public int mW17_FireballSpell = 0;
        public int mW18_MagicLockSpell = 0;
        public int mW19_PoisonSpell = 0;
        public int mW20_TelekinesisSpell = 0;
        public int mW21_TeleportSpell = 0;
        public int mW22_UnlockSpell = 0;
        public int mW23_WallOfStoneSpell = 0;
        public int mW24_ArchCureSpell = 0;
        public int mW25_ArchProtectionSpell = 0;
        public int mW26_CurseSpell = 0;
        public int mW27_FireFieldSpell = 0;
        public int mW28_GreaterHealSpell = 0;
        public int mW29_LightningSpell = 0;
        public int mW30_ManaDrainSpell = 0;
        public int mW31_RecallSpell = 0;
        public int mW32_BladeSpiritsSpell = 0;
        public int mW33_DispelFieldSpell = 0;
        public int mW34_IncognitoSpell = 0;
        public int mW35_MagicReflectSpell = 0;
        public int mW36_MindBlastSpell = 0;
        public int mW37_ParalyzeSpell = 0;
        public int mW38_PoisonFieldSpell = 0;
        public int mW39_SummonCreatureSpell = 0;
        public int mW40_DispelSpell = 0;
        public int mW41_EnergyBoltSpell = 0;
        public int mW42_ExplosionSpell = 0;
        public int mW43_InvisibilitySpell = 0;
        public int mW44_MarkSpell = 0;
        public int mW45_MassCurseSpell = 0;
        public int mW46_ParalyzeFieldSpell = 0;
        public int mW47_RevealSpell = 0;
        public int mW48_ChainLightningSpell = 0;
        public int mW49_EnergyFieldSpell = 0;
        public int mW50_FlameStrikeSpell = 0;
        public int mW51_GateTravelSpell = 0;
        public int mW52_ManaVampireSpell = 0;
        public int mW53_MassDispelSpell = 0;
        public int mW54_MeteorSwarmSpell = 0;
        public int mW55_PolymorphSpell = 0;
        public int mW56_EarthquakeSpell = 0;
        public int mW57_EnergyVortexSpell = 0;
        public int mW58_ResurrectionSpell = 0;
        public int mW59_AirElementalSpell = 0;
        public int mW60_SummonDaemonSpell = 0;
        public int mW61_EarthElementalSpell = 0;
        public int mW62_FireElementalSpell = 0;
        public int mW63_WaterElementalSpell = 0;

            //Necro spells
        public int mN01AnimateDeadSpell = 0;
        public int mN02BloodOathSpell = 0;
        public int mN03CorpseSkinSpell = 0;
        public int mN04CurseWeaponSpell = 0;
        public int mN05EvilOmenSpell = 0;
        public int mN06HorrificBeastSpell = 0;
        public int mN07LichFormSpell = 0;
        public int mN08MindRotSpell = 0;
        public int mN09PainSpikeSpell = 0;
        public int mN10PoisonStrikeSpell = 0;
        public int mN11StrangleSpell = 0;
        public int mN12SummonFamiliarSpell = 0;
        public int mN13VampiricEmbraceSpell = 0;
        public int mN14VengefulSpiritSpell = 0;
        public int mN15WitherSpell = 0;
        public int mN16WraithFormSpell = 0;
        public int mN17ExorcismSpell = 0;
       
        //chivalry spells
        public int mC01CleanseByFireSpell = 0;
        public int mC02CloseWoundsSpell = 0;
        public int mC03ConsecrateWeaponSpell = 0;
        public int mC04DispelEvilSpell = 0;
        public int mC05DivineFurySpell = 0;
        public int mC06EnemyOfOneSpell = 0;
        public int mC07HolyLightSpell = 0;
        public int mC08NobleSacrificeSpell = 0;
        public int mC09RemoveCurseSpell = 0;
        public int mC10SacredJourneySpell = 0;

		//bushido spells
		public int mB01Confidence = 0;
		public int mB02CounterAttack = 0;
		public int mB03Evasion = 0;
		public int mB04LightningStrike = 0;
		public int mB05MomentumStrike = 0;
		public int mB06HonorableExecution =0;

		//ninjitsu
		
        public int mI01DeathStrike = 0;
        public int mI02AnimalForm = 0;
        public int mI03KiAttack = 0;
        public int mI04SurpriseAttack = 0;
        public int mI05Backstab = 0;
        public int mI06Shadowjump = 0;
        public int mI07MirrorImage = 0;
		public int mI08FocusAttack = 0;

		// SPELLWEAVING
        public int mS01ArcaneCircleSpell = 0;
        public int mS02GiftOfRenewalSpell = 0;
        public int mS03ImmolatingWeaponSpell = 0;
        public int mS04AttuneWeaponSpell = 0;
        public int mS05ThunderstormSpell = 0;
        public int mS06NatureFurySpell = 0;
        public int mS07SummonFeySpell = 0;
        public int mS08SummonFiendSpell = 0;
        public int mS09ReaperFormSpell = 0;
        public int mS10WildfireSpell = 0;
        public int mS11EssenceOfWindSpell = 0;
        public int mS12DryadAllureSpell = 0;
        public int mS13EtherealVoyageSpell = 0;
        public int mS14WordOfDeathSpell = 0;
        public int mS15GiftOfLifeSpell = 0;
        public int mS16ArcaneEmpowermentSpell = 0;
		
		// mysticism
        public int mM01NetherBoltSpell = 0;
        public int mM02HealingStoneSpell = 0;
        public int mM03PurgeMagicSpell = 0;
        public int mM04EnchantSpell = 0;
        public int mM05SleepSpell = 0;
        public int mM06EagleStrikeSpell = 0;
        public int mM07AnimatedWeaponSpell = 0;
        public int mM08SpellTriggerSpell = 0;
        public int mM09MassSleepSpell = 0;
        public int mM10CleansingWindsSpell = 0;
        public int mM11BombardSpell = 0;
        public int mM12SpellPlagueSpell = 0;
        public int mM13HailStormSpell = 0;
        public int mM14NetherCycloneSpell = 0;
        public int mM15RisingColossusSpell = 0;
		public int mM16StoneFormSpell = 0;
		
		public int mSwitch = 0;
		public int mLock = 0;
		public int mCount =0;
		
		public int mXselect_10 =0;
		public int mXselect_15 =1;
		public int mXselect_20 =0;
		public int mXselect_30 =0;
		
	
		public int mYselect_1 = 0;
		public int mYselect_2 = 1;
		public int mYselect_3 = 0;
		public int mYselect_4 = 0;
		//public int mYselect_var = 0;


        [CommandProperty(AccessLevel.GameMaster)]
        public int W00_ClumsySpell { get { return mW00_ClumsySpell; } set { mW00_ClumsySpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W01_CreateFoodSpell { get { return mW01_CreateFoodSpell; } set { mW01_CreateFoodSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W02_FeeblemindSpell { get { return mW02_FeeblemindSpell; } set { mW02_FeeblemindSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W03_HealSpell { get { return mW03_HealSpell; } set { mW03_HealSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W04_MagicArrowSpell { get { return mW04_MagicArrowSpell; } set { mW04_MagicArrowSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W05_NightSightSpell { get { return mW05_NightSightSpell; } set { mW05_NightSightSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W06_ReactiveArmorSpell { get { return mW06_ReactiveArmorSpell; } set { mW06_ReactiveArmorSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W07_WeakenSpell { get { return mW07_WeakenSpell; } set { mW07_WeakenSpell = value; } }    
        [CommandProperty(AccessLevel.GameMaster)]
        public int W08_AgilitySpell { get { return mW08_AgilitySpell; } set { mW08_AgilitySpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W09_CunningSpell { get { return mW09_CunningSpell; } set { mW09_CunningSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W10_CureSpell { get { return mW10_CureSpell; } set { mW10_CureSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W11_HarmSpell { get { return mW11_HarmSpell; } set { mW11_HarmSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W12_MagicTrapSpell { get { return mW12_MagicTrapSpell; } set { mW12_MagicTrapSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W13_RemoveTrapSpell { get { return mW13_RemoveTrapSpell; } set { mW13_RemoveTrapSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W14_ProtectionSpell { get { return mW14_ProtectionSpell; } set { mW14_ProtectionSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W15_StrengthSpell { get { return mW15_StrengthSpell; } set { mW15_StrengthSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W16_BlessSpell { get { return mW16_BlessSpell; } set { mW16_BlessSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W17_FireballSpell { get { return mW17_FireballSpell; } set { mW17_FireballSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W18_MagicLockSpell { get { return mW18_MagicLockSpell; } set { mW18_MagicLockSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W19_PoisonSpell { get { return mW19_PoisonSpell; } set { mW19_PoisonSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W20_TelekinesisSpell { get { return mW20_TelekinesisSpell; } set { mW20_TelekinesisSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W21_TeleportSpell { get { return mW21_TeleportSpell; } set { mW21_TeleportSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W22_UnlockSpell { get { return mW22_UnlockSpell; } set { mW22_UnlockSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W23_WallOfStoneSpell { get { return mW23_WallOfStoneSpell; } set { mW23_WallOfStoneSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W24_ArchCureSpell { get { return mW24_ArchCureSpell; } set { mW24_ArchCureSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W25_ArchProtectionSpell { get { return mW25_ArchProtectionSpell; } set { mW25_ArchProtectionSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W26_CurseSpell { get { return mW26_CurseSpell; } set { mW26_CurseSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W27_FireFieldSpell { get { return mW27_FireFieldSpell; } set { mW27_FireFieldSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W28_GreaterHealSpell { get { return mW28_GreaterHealSpell; } set { mW28_GreaterHealSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W29_LightningSpell { get { return mW29_LightningSpell; } set { mW29_LightningSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W30_ManaDrainSpell { get { return mW30_ManaDrainSpell; } set { mW30_ManaDrainSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W31_RecallSpell { get { return mW31_RecallSpell; } set { mW31_RecallSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W32_BladeSpiritsSpell { get { return mW32_BladeSpiritsSpell; } set { mW32_BladeSpiritsSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W33_DispelFieldSpell { get { return mW33_DispelFieldSpell; } set { mW33_DispelFieldSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W34_IncognitoSpell { get { return mW34_IncognitoSpell; } set { mW34_IncognitoSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W35_MagicReflectSpell { get { return mW35_MagicReflectSpell; } set { mW35_MagicReflectSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W36_MindBlastSpell { get { return mW36_MindBlastSpell; } set { mW36_MindBlastSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W37_ParalyzeSpell { get { return mW37_ParalyzeSpell; } set { mW37_ParalyzeSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W38_PoisonFieldSpell { get { return mW38_PoisonFieldSpell; } set { mW38_PoisonFieldSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W39_SummonCreatureSpell { get { return mW39_SummonCreatureSpell; } set { mW39_SummonCreatureSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W40_DispelSpell { get { return mW40_DispelSpell; } set { mW40_DispelSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W41_EnergyBoltSpell { get { return mW41_EnergyBoltSpell; } set { mW41_EnergyBoltSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W42_ExplosionSpell { get { return mW42_ExplosionSpell; } set { mW42_ExplosionSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W43_InvisibilitySpell { get { return mW43_InvisibilitySpell; } set { mW43_InvisibilitySpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W44_MarkSpell { get { return mW44_MarkSpell; } set { mW44_MarkSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W45_MassCurseSpell { get { return mW45_MassCurseSpell; } set { mW45_MassCurseSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W46_ParalyzeFieldSpell { get { return mW46_ParalyzeFieldSpell; } set { mW46_ParalyzeFieldSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W47_RevealSpell { get { return mW47_RevealSpell; } set { mW47_RevealSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W48_ChainLightningSpell { get { return mW48_ChainLightningSpell; } set { mW48_ChainLightningSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W49_EnergyFieldSpell { get { return mW49_EnergyFieldSpell; } set { mW49_EnergyFieldSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W50_FlameStrikeSpell { get { return mW50_FlameStrikeSpell; } set { mW50_FlameStrikeSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W51_GateTravelSpell { get { return mW51_GateTravelSpell; } set { mW51_GateTravelSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W52_ManaVampireSpell { get { return mW52_ManaVampireSpell; } set { mW52_ManaVampireSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W53_MassDispelSpell { get { return mW53_MassDispelSpell; } set { mW53_MassDispelSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W54_MeteorSwarmSpell { get { return mW54_MeteorSwarmSpell; } set { mW54_MeteorSwarmSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W55_PolymorphSpell { get { return mW55_PolymorphSpell; } set { mW55_PolymorphSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W56_EarthquakeSpell { get { return mW56_EarthquakeSpell; } set { mW56_EarthquakeSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W57_EnergyVortexSpell { get { return mW57_EnergyVortexSpell; } set { mW57_EnergyVortexSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W58_ResurrectionSpell { get { return mW58_ResurrectionSpell; } set { mW58_ResurrectionSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W59_AirElementalSpell { get { return mW59_AirElementalSpell; } set { mW59_AirElementalSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W60_SummonDaemonSpell { get { return mW60_SummonDaemonSpell; } set { mW60_SummonDaemonSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W61_EarthElementalSpell { get { return mW61_EarthElementalSpell; } set { mW61_EarthElementalSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W62_FireElementalSpell { get { return mW62_FireElementalSpell; } set { mW62_FireElementalSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int W63_WaterElementalSpell { get { return mW63_WaterElementalSpell; } set { mW63_WaterElementalSpell = value; } }

// NECROMANCY
        [CommandProperty(AccessLevel.GameMaster)]
        public int N01AnimateDeadSpell { get { return mN01AnimateDeadSpell; } set { mN01AnimateDeadSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int N02BloodOathSpell { get { return mN02BloodOathSpell; } set { mN02BloodOathSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int N03CorpseSkinSpell { get { return mN03CorpseSkinSpell; } set { mN03CorpseSkinSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int N04CurseWeaponSpell { get { return mN04CurseWeaponSpell; } set { mN04CurseWeaponSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int N05EvilOmenSpell { get { return mN05EvilOmenSpell; } set { mN05EvilOmenSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int N06HorrificBeastSpell { get { return mN06HorrificBeastSpell; } set { mN06HorrificBeastSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int N07LichFormSpell { get { return mN07LichFormSpell; } set { mN07LichFormSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int N08MindRotSpell { get { return mN08MindRotSpell; } set { mN08MindRotSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int N09PainSpikeSpell { get { return mN09PainSpikeSpell; } set { mN09PainSpikeSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int N10PoisonStrikeSpell { get { return mN10PoisonStrikeSpell; } set { mN10PoisonStrikeSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int N11StrangleSpell { get { return mN11StrangleSpell; } set { mN11StrangleSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int N12SummonFamiliarSpell { get { return mN12SummonFamiliarSpell; } set { mN12SummonFamiliarSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int N13VampiricEmbraceSpell { get { return mN13VampiricEmbraceSpell; } set { mN13VampiricEmbraceSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int N14VengefulSpiritSpell { get { return mN14VengefulSpiritSpell; } set { mN14VengefulSpiritSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int N15WitherSpell { get { return mN15WitherSpell; } set { mN15WitherSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int N16WraithFormSpell { get { return mN16WraithFormSpell; } set { mN16WraithFormSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int N17ExorcismSpell { get { return mN17ExorcismSpell; } set { mN17ExorcismSpell = value; } }
   
 //CIVALRY    

        [CommandProperty(AccessLevel.GameMaster)]
        public int C01CleanseByFireSpell { get { return mC01CleanseByFireSpell; } set { mC01CleanseByFireSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int C02CloseWoundsSpell { get { return mC02CloseWoundsSpell; } set { mC02CloseWoundsSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int C03ConsecrateWeaponSpell { get { return mC03ConsecrateWeaponSpell; } set { mC03ConsecrateWeaponSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int C04DispelEvilSpell { get { return mC04DispelEvilSpell; } set { mC04DispelEvilSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int C05DivineFurySpell { get { return mC05DivineFurySpell; } set { mC05DivineFurySpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int C06EnemyOfOneSpell { get { return mC06EnemyOfOneSpell; } set { mC06EnemyOfOneSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int C07HolyLightSpell { get { return mC07HolyLightSpell; } set { mC07HolyLightSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int C08NobleSacrificeSpell { get { return mC08NobleSacrificeSpell; } set { mC08NobleSacrificeSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int C09RemoveCurseSpell { get { return mC09RemoveCurseSpell; } set { mC09RemoveCurseSpell = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int C10SacredJourneySpell { get { return mC10SacredJourneySpell; } set { mC10SacredJourneySpell = value; } }


// BUSHIDO


		

        [CommandProperty(AccessLevel.GameMaster)]
        public int B01Confidence { get { return mB01Confidence; } set { mB01Confidence = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int B02CounterAttack { get { return mB02CounterAttack; } set { mB02CounterAttack = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int B03Evasion { get { return mB03Evasion; } set { mB03Evasion = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int B04LightningStrike { get { return mB04LightningStrike; } set { mB04LightningStrike = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int B05MomentumStrike { get { return mB05MomentumStrike; } set { mB05MomentumStrike = value; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
        public int B06HonorableExecution { get { return mB06HonorableExecution; } set { mB06HonorableExecution = value; } }

// NINJITSU

	//mI06FocusAttack
		

		[CommandProperty(AccessLevel.GameMaster)]
				public int I01DeathStrike { get { return mI01DeathStrike; } set { mI01DeathStrike = value; } }
		 [CommandProperty(AccessLevel.GameMaster)]
				public int I02AnimalForm { get { return mI02AnimalForm; } set { mI02AnimalForm = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int I03KiAttack { get { return mI03KiAttack; } set { mI03KiAttack = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int I04SurpriseAttack { get { return mI04SurpriseAttack; } set { mI04SurpriseAttack = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int I05Backstab { get { return mI05Backstab; } set { mI05Backstab = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int I06Shadowjump { get { return mI06Shadowjump; } set { mI06Shadowjump = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int I07MirrorImage { get { return mI07MirrorImage; } set { mI07MirrorImage = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int I08FocusAttack { get { return mI08FocusAttack; } set { mI08FocusAttack = value; } }


// SPELLWEAVING

		[CommandProperty(AccessLevel.GameMaster)]
				public int S01ArcaneCircleSpell { get { return mS01ArcaneCircleSpell; } set { mS01ArcaneCircleSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int S02GiftOfRenewalSpell { get { return mS02GiftOfRenewalSpell; } set { mS02GiftOfRenewalSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int S03ImmolatingWeaponSpell { get { return mS03ImmolatingWeaponSpell; } set { mS03ImmolatingWeaponSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int S04AttuneWeaponSpell { get { return mS04AttuneWeaponSpell; } set { mS04AttuneWeaponSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int S05ThunderstormSpell { get { return mS05ThunderstormSpell; } set { mS05ThunderstormSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int S06NatureFurySpell { get { return mS06NatureFurySpell; } set { mS06NatureFurySpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int S07SummonFeySpell { get { return mS07SummonFeySpell; } set { mS07SummonFeySpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int S08SummonFiendSpell { get { return mS08SummonFiendSpell; } set { mS08SummonFiendSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int S09ReaperFormSpell { get { return mS09ReaperFormSpell; } set { mS09ReaperFormSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int S10WildfireSpell { get { return mS10WildfireSpell; } set { mS10WildfireSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int S11EssenceOfWindSpell { get { return mS11EssenceOfWindSpell; } set { mS11EssenceOfWindSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int S12DryadAllureSpell { get { return mS12DryadAllureSpell; } set { mS12DryadAllureSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int S13EtherealVoyageSpell { get { return mS13EtherealVoyageSpell; } set { mS13EtherealVoyageSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int S14WordOfDeathSpell { get { return mS14WordOfDeathSpell; } set { mS14WordOfDeathSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int S15GiftOfLifeSpell { get { return mS15GiftOfLifeSpell; } set { mS15GiftOfLifeSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int S16ArcaneEmpowermentSpell { get { return mS16ArcaneEmpowermentSpell; } set { mS16ArcaneEmpowermentSpell = value; } }
				
				
      // MYSTICISM
	  
	  [CommandProperty(AccessLevel.GameMaster)]
				public int M01NetherBoltSpell { get { return mM01NetherBoltSpell; } set { mM01NetherBoltSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int M02HealingStoneSpell { get { return mM02HealingStoneSpell; } set { mM02HealingStoneSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int M03PurgeMagicSpell { get { return mM03PurgeMagicSpell; } set { mM03PurgeMagicSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int M04EnchantSpell { get { return mM04EnchantSpell; } set { mM04EnchantSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int M05SleepSpell { get { return mM05SleepSpell; } set { mM05SleepSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int M06EagleStrikeSpell { get { return mM06EagleStrikeSpell; } set { mM06EagleStrikeSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int M07AnimatedWeaponSpell { get { return mM07AnimatedWeaponSpell; } set { mM07AnimatedWeaponSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int M08SpellTriggerSpell { get { return mM08SpellTriggerSpell; } set { mM08SpellTriggerSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int M09MassSleepSpell { get { return mM09MassSleepSpell; } set { mM09MassSleepSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int M10CleansingWindsSpell { get { return mM10CleansingWindsSpell; } set { mM10CleansingWindsSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int M11BombardSpell { get { return mM11BombardSpell; } set { mM11BombardSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int M12SpellPlagueSpell { get { return mM12SpellPlagueSpell; } set { mM12SpellPlagueSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int M13HailStormSpell { get { return mM13HailStormSpell; } set { mM13HailStormSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int M14NetherCycloneSpell { get { return mM14NetherCycloneSpell; } set { mM14NetherCycloneSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int M15RisingColossusSpell { get { return mM15RisingColossusSpell; } set { mM15RisingColossusSpell = value; } }
		[CommandProperty(AccessLevel.GameMaster)]
				public int M16StoneFormSpell { get { return mM16StoneFormSpell; } set { mM16StoneFormSpell = value; } }
				
				
	///
		[CommandProperty(AccessLevel.GameMaster)]
				public int Switch { get { return mSwitch; } set { mSwitch = value; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
				public int Lock { get { return mLock; } set { mLock = value; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
				public int Count { get { return mCount; } set { mCount = value; } }

	///
	
		[CommandProperty(AccessLevel.GameMaster)]
        public int Xselect_10 { get { return mXselect_10; } set { mXselect_10 = value; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
        public int Xselect_15  { get { return mXselect_15; } set { mXselect_15 = value; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
        public int Xselect_20 { get { return mXselect_20; } set { mXselect_20 = value; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
        public int Xselect_30 { get { return mXselect_30; } set { mXselect_30 = value; } }
	///	
		[CommandProperty(AccessLevel.GameMaster)]
        public int Yselect_1 { get { return mYselect_1; } set { mYselect_1 = value; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
        public int Yselect_2 { get { return mYselect_2; } set { mYselect_2 = value; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
        public int Yselect_3 { get { return mYselect_3; } set { mYselect_3 = value; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
        public int Yselect_4 { get { return mYselect_4; } set { mYselect_4 = value; } }
	
		
        [Constructable]
        public SpellBarScroll() : base( 0x14EF )
        {
            LootType = LootType.Blessed;
            Hue = 0x5B6;
            Name = "Mage Notes";
        }

        public SpellBarScroll( Serial serial ) : base( serial )
        {
        }

        public override void OnDoubleClick( Mobile from )
        {
            //Mobile from = e.Mobile;
            int page = 0;
            page = 0;
       
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
            }
            else
            {
                from.CloseGump( typeof( SpellBarGump ) );
        //        from.CloseGump( typeof( SpellBar_BarGump ) );
                from.SendGump( new SpellBarGump( from, page, this ) );
            }
        }

		
		
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int) 0 ); // version
            writer.Write(mW00_ClumsySpell);
            writer.Write(mW01_CreateFoodSpell);
            writer.Write(mW02_FeeblemindSpell);
            writer.Write(mW03_HealSpell);
            writer.Write(mW04_MagicArrowSpell);
            writer.Write(mW05_NightSightSpell);
            writer.Write(mW06_ReactiveArmorSpell);
            writer.Write(mW07_WeakenSpell);
            writer.Write(mW08_AgilitySpell);
            writer.Write(mW09_CunningSpell);
            writer.Write(mW10_CureSpell);
            writer.Write(mW11_HarmSpell);
            writer.Write(mW12_MagicTrapSpell);
            writer.Write(mW13_RemoveTrapSpell);
            writer.Write(mW14_ProtectionSpell);
            writer.Write(mW15_StrengthSpell);
            writer.Write(mW16_BlessSpell);
            writer.Write(mW17_FireballSpell);
            writer.Write(mW18_MagicLockSpell);
            writer.Write(mW19_PoisonSpell);
            writer.Write(mW20_TelekinesisSpell);
            writer.Write(mW21_TeleportSpell);
            writer.Write(mW22_UnlockSpell);
            writer.Write(mW23_WallOfStoneSpell);
            writer.Write(mW24_ArchCureSpell);
            writer.Write(mW25_ArchProtectionSpell);
            writer.Write(mW26_CurseSpell);
            writer.Write(mW27_FireFieldSpell);
            writer.Write(mW28_GreaterHealSpell);
            writer.Write(mW29_LightningSpell);
            writer.Write(mW30_ManaDrainSpell);
            writer.Write(mW31_RecallSpell);
            writer.Write(mW32_BladeSpiritsSpell);
            writer.Write(mW33_DispelFieldSpell);
            writer.Write(mW34_IncognitoSpell);
            writer.Write(mW35_MagicReflectSpell);
            writer.Write(mW36_MindBlastSpell);
            writer.Write(mW37_ParalyzeSpell);
            writer.Write(mW38_PoisonFieldSpell);
            writer.Write(mW39_SummonCreatureSpell);
            writer.Write(mW40_DispelSpell);
            writer.Write(mW41_EnergyBoltSpell);
            writer.Write(mW42_ExplosionSpell);
            writer.Write(mW43_InvisibilitySpell);
            writer.Write(mW44_MarkSpell);
            writer.Write(mW45_MassCurseSpell);
            writer.Write(mW46_ParalyzeFieldSpell);
            writer.Write(mW47_RevealSpell);
            writer.Write(mW48_ChainLightningSpell);
            writer.Write(mW49_EnergyFieldSpell);
            writer.Write(mW50_FlameStrikeSpell);
            writer.Write(mW51_GateTravelSpell);
            writer.Write(mW52_ManaVampireSpell);
            writer.Write(mW53_MassDispelSpell);
            writer.Write(mW54_MeteorSwarmSpell);
            writer.Write(mW55_PolymorphSpell);
            writer.Write(mW56_EarthquakeSpell);
            writer.Write(mW57_EnergyVortexSpell);
            writer.Write(mW58_ResurrectionSpell);
            writer.Write(mW59_AirElementalSpell);
            writer.Write(mW60_SummonDaemonSpell);
            writer.Write(mW61_EarthElementalSpell);
            writer.Write(mW62_FireElementalSpell);
            writer.Write(mW63_WaterElementalSpell);
            writer.Write(mN01AnimateDeadSpell);
            writer.Write(mN02BloodOathSpell);
            writer.Write(mN03CorpseSkinSpell);
            writer.Write(mN04CurseWeaponSpell);
            writer.Write(mN05EvilOmenSpell);
            writer.Write(mN06HorrificBeastSpell);
            writer.Write(mN07LichFormSpell);
            writer.Write(mN08MindRotSpell);
            writer.Write(mN09PainSpikeSpell);
            writer.Write(mN10PoisonStrikeSpell);
            writer.Write(mN11StrangleSpell);
            writer.Write(mN12SummonFamiliarSpell);
            writer.Write(mN13VampiricEmbraceSpell);
            writer.Write(mN14VengefulSpiritSpell);
            writer.Write(mN15WitherSpell);
            writer.Write(mN16WraithFormSpell);
            writer.Write(mN17ExorcismSpell);
            writer.Write(mC01CleanseByFireSpell);
            writer.Write(mC02CloseWoundsSpell);
            writer.Write(mC03ConsecrateWeaponSpell);
            writer.Write(mC04DispelEvilSpell);
            writer.Write(mC05DivineFurySpell);
            writer.Write(mC06EnemyOfOneSpell);
            writer.Write(mC07HolyLightSpell);
            writer.Write(mC08NobleSacrificeSpell);
            writer.Write(mC09RemoveCurseSpell);
            writer.Write(mC10SacredJourneySpell);
            writer.Write(mB01Confidence);
			writer.Write(mB02CounterAttack);
			writer.Write(mB03Evasion);
			writer.Write(mB04LightningStrike);
			writer.Write(mB05MomentumStrike); 
			writer.Write(mB06HonorableExecution);
			writer.Write(mI01DeathStrike);
            writer.Write(mI02AnimalForm);
            writer.Write(mI03KiAttack);
            writer.Write(mI04SurpriseAttack);
            writer.Write(mI05Backstab);
            writer.Write(mI06Shadowjump);
            writer.Write(mI07MirrorImage);
			writer.Write(mI08FocusAttack);
            writer.Write(mS01ArcaneCircleSpell);
            writer.Write(mS02GiftOfRenewalSpell);
            writer.Write(mS03ImmolatingWeaponSpell);
            writer.Write(mS04AttuneWeaponSpell);
            writer.Write(mS05ThunderstormSpell);
            writer.Write(mS06NatureFurySpell);
            writer.Write(mS07SummonFeySpell);
            writer.Write(mS08SummonFiendSpell);
            writer.Write(mS09ReaperFormSpell);
            writer.Write(mS10WildfireSpell);
            writer.Write(mS11EssenceOfWindSpell);
            writer.Write(mS12DryadAllureSpell);
            writer.Write(mS13EtherealVoyageSpell);
            writer.Write(mS14WordOfDeathSpell);
            writer.Write(mS15GiftOfLifeSpell);
            writer.Write(mS16ArcaneEmpowermentSpell);
			writer.Write(mM01NetherBoltSpell);
			writer.Write(mM02HealingStoneSpell);
			writer.Write(mM03PurgeMagicSpell);
			writer.Write(mM04EnchantSpell);
			writer.Write(mM05SleepSpell);
			writer.Write(mM06EagleStrikeSpell);
			writer.Write(mM07AnimatedWeaponSpell);
			writer.Write(mM08SpellTriggerSpell);
			writer.Write(mM09MassSleepSpell);
			writer.Write(mM10CleansingWindsSpell);
			writer.Write(mM11BombardSpell);
			writer.Write(mM12SpellPlagueSpell);
			writer.Write(mM13HailStormSpell);
			writer.Write(mM14NetherCycloneSpell);
			writer.Write(mM15RisingColossusSpell);
			writer.Write(mM16StoneFormSpell);
			writer.Write(mSwitch);
			writer.Write(mLock);
			writer.Write(mCount);
			writer.Write(mXselect_10);
			writer.Write(mXselect_15);
			writer.Write(mXselect_20);
			writer.Write(mXselect_30);
			writer.Write(mYselect_1);
			writer.Write(mYselect_2);
			writer.Write(mYselect_3);
			writer.Write(mYselect_4);
			
        }
	
        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
            mW00_ClumsySpell = reader.ReadInt();
            mW01_CreateFoodSpell = reader.ReadInt();
            mW02_FeeblemindSpell = reader.ReadInt();
            mW03_HealSpell = reader.ReadInt();
            mW04_MagicArrowSpell = reader.ReadInt();
            mW05_NightSightSpell = reader.ReadInt();
            mW06_ReactiveArmorSpell = reader.ReadInt();
            mW07_WeakenSpell = reader.ReadInt();
            mW08_AgilitySpell = reader.ReadInt();
            mW09_CunningSpell = reader.ReadInt();
            mW10_CureSpell = reader.ReadInt();
            mW11_HarmSpell = reader.ReadInt();
            mW12_MagicTrapSpell = reader.ReadInt();
            mW13_RemoveTrapSpell = reader.ReadInt();
            mW14_ProtectionSpell = reader.ReadInt();
            mW15_StrengthSpell = reader.ReadInt();
            mW16_BlessSpell = reader.ReadInt();
            mW17_FireballSpell = reader.ReadInt();
            mW18_MagicLockSpell = reader.ReadInt();
            mW19_PoisonSpell = reader.ReadInt();
            mW20_TelekinesisSpell = reader.ReadInt();
            mW21_TeleportSpell = reader.ReadInt();
            mW22_UnlockSpell = reader.ReadInt();
            mW23_WallOfStoneSpell = reader.ReadInt();
            mW24_ArchCureSpell = reader.ReadInt();
            mW25_ArchProtectionSpell = reader.ReadInt();
            mW26_CurseSpell = reader.ReadInt();
            mW27_FireFieldSpell = reader.ReadInt();
            mW28_GreaterHealSpell = reader.ReadInt();
            mW29_LightningSpell = reader.ReadInt();
            mW30_ManaDrainSpell = reader.ReadInt();
            mW31_RecallSpell = reader.ReadInt();
            mW32_BladeSpiritsSpell = reader.ReadInt();
            mW33_DispelFieldSpell = reader.ReadInt();
            mW34_IncognitoSpell = reader.ReadInt();
            mW35_MagicReflectSpell = reader.ReadInt();
            mW36_MindBlastSpell = reader.ReadInt();
            mW37_ParalyzeSpell = reader.ReadInt();
            mW38_PoisonFieldSpell = reader.ReadInt();
            mW39_SummonCreatureSpell = reader.ReadInt();
            mW40_DispelSpell = reader.ReadInt();
            mW41_EnergyBoltSpell = reader.ReadInt();
            mW42_ExplosionSpell = reader.ReadInt();
            mW43_InvisibilitySpell = reader.ReadInt();
            mW44_MarkSpell = reader.ReadInt();
            mW45_MassCurseSpell = reader.ReadInt();
            mW46_ParalyzeFieldSpell = reader.ReadInt();
            mW47_RevealSpell = reader.ReadInt();
            mW48_ChainLightningSpell = reader.ReadInt();
            mW49_EnergyFieldSpell = reader.ReadInt();
            mW50_FlameStrikeSpell = reader.ReadInt();
            mW51_GateTravelSpell = reader.ReadInt();
            mW52_ManaVampireSpell = reader.ReadInt();
            mW53_MassDispelSpell = reader.ReadInt();
            mW54_MeteorSwarmSpell = reader.ReadInt();
            mW55_PolymorphSpell = reader.ReadInt();
            mW56_EarthquakeSpell = reader.ReadInt();
            mW57_EnergyVortexSpell = reader.ReadInt();
            mW58_ResurrectionSpell = reader.ReadInt();
            mW59_AirElementalSpell = reader.ReadInt();
            mW60_SummonDaemonSpell = reader.ReadInt();
            mW61_EarthElementalSpell = reader.ReadInt();
            mW62_FireElementalSpell = reader.ReadInt();
            mW63_WaterElementalSpell = reader.ReadInt();
            mN01AnimateDeadSpell = reader.ReadInt();
            mN02BloodOathSpell = reader.ReadInt();
            mN03CorpseSkinSpell = reader.ReadInt();
            mN04CurseWeaponSpell = reader.ReadInt();
            mN05EvilOmenSpell = reader.ReadInt();
            mN06HorrificBeastSpell = reader.ReadInt();
            mN07LichFormSpell = reader.ReadInt();
            mN08MindRotSpell = reader.ReadInt();
            mN09PainSpikeSpell = reader.ReadInt();
            mN10PoisonStrikeSpell = reader.ReadInt();
            mN11StrangleSpell = reader.ReadInt();
            mN12SummonFamiliarSpell = reader.ReadInt();
            mN13VampiricEmbraceSpell = reader.ReadInt();
            mN14VengefulSpiritSpell = reader.ReadInt();
            mN15WitherSpell = reader.ReadInt();
            mN16WraithFormSpell = reader.ReadInt();
            mN17ExorcismSpell = reader.ReadInt();
            mC01CleanseByFireSpell = reader.ReadInt();
            mC02CloseWoundsSpell = reader.ReadInt();
            mC03ConsecrateWeaponSpell = reader.ReadInt();
            mC04DispelEvilSpell = reader.ReadInt();
            mC05DivineFurySpell = reader.ReadInt();
            mC06EnemyOfOneSpell = reader.ReadInt();
            mC07HolyLightSpell = reader.ReadInt();
            mC08NobleSacrificeSpell = reader.ReadInt();
            mC09RemoveCurseSpell = reader.ReadInt();
            mC10SacredJourneySpell = reader.ReadInt();
			mB01Confidence = reader.ReadInt();
			mB02CounterAttack = reader.ReadInt();
			mB03Evasion = reader.ReadInt();
			mB04LightningStrike = reader.ReadInt();
			mB05MomentumStrike = reader.ReadInt();
			mB06HonorableExecution = reader.ReadInt();
			mI01DeathStrike = reader.ReadInt();
           mI02AnimalForm = reader.ReadInt();
           mI03KiAttack = reader.ReadInt();
           mI04SurpriseAttack = reader.ReadInt();
           mI05Backstab = reader.ReadInt();
           mI06Shadowjump = reader.ReadInt();
           mI07MirrorImage = reader.ReadInt();
		   mI08FocusAttack = reader.ReadInt();
           mS01ArcaneCircleSpell = reader.ReadInt();
           mS02GiftOfRenewalSpell = reader.ReadInt();
           mS03ImmolatingWeaponSpell = reader.ReadInt();
           mS04AttuneWeaponSpell = reader.ReadInt();
           mS05ThunderstormSpell = reader.ReadInt();
           mS06NatureFurySpell = reader.ReadInt();
           mS07SummonFeySpell = reader.ReadInt();
           mS08SummonFiendSpell = reader.ReadInt();
           mS09ReaperFormSpell = reader.ReadInt();
           mS10WildfireSpell = reader.ReadInt();
           mS11EssenceOfWindSpell = reader.ReadInt();
           mS12DryadAllureSpell = reader.ReadInt();
           mS13EtherealVoyageSpell = reader.ReadInt();
           mS14WordOfDeathSpell = reader.ReadInt();
           mS15GiftOfLifeSpell = reader.ReadInt();
           mS16ArcaneEmpowermentSpell = reader.ReadInt();
		   mM01NetherBoltSpell = reader.ReadInt();            
		   mM02HealingStoneSpell = reader.ReadInt();
		   mM03PurgeMagicSpell = reader.ReadInt();
		   mM04EnchantSpell = reader.ReadInt();
		   mM05SleepSpell = reader.ReadInt();
		   mM06EagleStrikeSpell = reader.ReadInt();
		   mM07AnimatedWeaponSpell = reader.ReadInt();
		   mM08SpellTriggerSpell = reader.ReadInt();
		   mM09MassSleepSpell = reader.ReadInt();
		   mM10CleansingWindsSpell = reader.ReadInt();
		   mM11BombardSpell = reader.ReadInt();
		   mM12SpellPlagueSpell = reader.ReadInt();
		   mM13HailStormSpell = reader.ReadInt();
		   mM14NetherCycloneSpell = reader.ReadInt();
		   mM15RisingColossusSpell = reader.ReadInt();
		   mM16StoneFormSpell = reader.ReadInt();
		   mSwitch = reader.ReadInt();
		   mLock = reader.ReadInt();
		   mCount = reader.ReadInt();
		   mXselect_10 = reader.ReadInt();
		   mXselect_15 = reader.ReadInt();
		   mXselect_20 = reader.ReadInt();
		   mXselect_30 = reader.ReadInt();
		   mYselect_1 = reader.ReadInt();
		   mYselect_2 = reader.ReadInt();
		   mYselect_3 = reader.ReadInt();
		   mYselect_4 = reader.ReadInt();
		   
		   /*
			writer.Write(mYselect_1);
			writer.Write(mYselect_2);
			writer.Write(mYselect_3);
			writer.Write(mYselect_4);
		   */
		  
		   
		 
        }
    }
}