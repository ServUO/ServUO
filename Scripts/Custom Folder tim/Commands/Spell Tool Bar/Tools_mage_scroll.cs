using System;
using Server;
using Server.Network;
using Server.Gumps;

namespace Server.Items
{
	public class Tools_mage_scroll : Item
	{
			// First circle
		public int mW00_ClumsySpell = 0;
		public int mW01_CreateFoodSpell = 0;
		public int mW02_FeeblemindSpell = 0;
		public int mW03_HealSpell = 0;
		public int mW04_MagicArrowSpell = 0;
		public int mW05_NightSightSpell = 0;
		public int mW06_ReactiveArmorSpell = 0;
		public int mW07_WeakenSpell = 0;

			// Second circle
		public int mW08_AgilitySpell = 0;
		public int mW09_CunningSpell = 0;
		public int mW10_CureSpell = 0;
		public int mW11_HarmSpell = 0;
		public int mW12_MagicTrapSpell = 0;
		public int mW13_RemoveTrapSpell = 0;
		public int mW14_ProtectionSpell = 0;
		public int mW15_StrengthSpell = 0;

			// Third circle
		public int mW16_BlessSpell = 0;
		public int mW17_FireballSpell = 0;
		public int mW18_MagicLockSpell = 0;
		public int mW19_PoisonSpell = 0;
		public int mW20_TelekinesisSpell = 0;
		public int mW21_TeleportSpell = 0;
		public int mW22_UnlockSpell = 0;
		public int mW23_WallOfStoneSpell = 0;

			// Fourth circle
		public int mW24_ArchCureSpell = 0;
		public int mW25_ArchProtectionSpell = 0;
		public int mW26_CurseSpell = 0;
		public int mW27_FireFieldSpell = 0;
		public int mW28_GreaterHealSpell = 0;
		public int mW29_LightningSpell = 0;
		public int mW30_ManaDrainSpell = 0;
		public int mW31_RecallSpell = 0;

			// Fifth circle
		public int mW32_BladeSpiritsSpell = 0;
		public int mW33_DispelFieldSpell = 0;
		public int mW34_IncognitoSpell = 0;
		public int mW35_MagicReflectSpell = 0;
		public int mW36_MindBlastSpell = 0;
		public int mW37_ParalyzeSpell = 0;
		public int mW38_PoisonFieldSpell = 0;
		public int mW39_SummonCreatureSpell = 0;

			// Sixth circle
		public int mW40_DispelSpell = 0;
		public int mW41_EnergyBoltSpell = 0;
		public int mW42_ExplosionSpell = 0;
		public int mW43_InvisibilitySpell = 0;
		public int mW44_MarkSpell = 0;
		public int mW45_MassCurseSpell = 0;
		public int mW46_ParalyzeFieldSpell = 0;
		public int mW47_RevealSpell = 0;

			// Seventh circle
		public int mW48_ChainLightningSpell = 0;
		public int mW49_EnergyFieldSpell = 0;
		public int mW50_FlameStrikeSpell = 0;
		public int mW51_GateTravelSpell = 0;
		public int mW52_ManaVampireSpell = 0;
		public int mW53_MassDispelSpell = 0;
		public int mW54_MeteorSwarmSpell = 0;
		public int mW55_PolymorphSpell = 0;

			// Eighth circle
		public int mW56_EarthquakeSpell = 0;
		public int mW57_EnergyVortexSpell = 0;
		public int mW58_ResurrectionSpell = 0;
		public int mW59_AirElementalSpell = 0;
		public int mW60_SummonDaemonSpell = 0;
		public int mW61_EarthElementalSpell = 0;
		public int mW62_FireElementalSpell = 0;
		public int mW63_WaterElementalSpell = 0;

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

		[Constructable]
		public Tools_mage_scroll() : base( 0x14EF )
		{
			LootType = LootType.Blessed;
			Hue = 0x5B6;
			Name = "Mage Notes";
		}

		public Tools_mage_scroll( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if (!IsChildOf(from.Backpack))
			{
				from.SendLocalizedMessage(1042001);
			}
			else
			{
				from.CloseGump( typeof( Tools_mage_scrollGump ) );
				from.SendGump( new Tools_mage_scrollGump( from, this ) );
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
		}
	}
}
