using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Network;
using Server.Spells;
using Server.Spells.First;
using Server.Spells.Second;
using Server.Spells.Third;
using Server.Spells.Fourth;
using Server.Spells.Fifth;
using Server.Spells.Sixth;
using Server.Spells.Seventh;
using Server.Spells.Eighth;
using Server.Prompts;

namespace Server.Gumps
{
	public class Tools_sorcery_scrollGump : Gump
	{
		private Tools_sorcery_scroll m_Scroll;

		public Tools_sorcery_scrollGump( Mobile from, Tools_sorcery_scroll scroll ) : base( 0, 0 )
		{
			m_Scroll = scroll;

			int mW00_ClumsySpell = m_Scroll.mW00_ClumsySpell;
			int mW01_CreateFoodSpell = m_Scroll.mW01_CreateFoodSpell;
			int mW02_FeeblemindSpell = m_Scroll.mW02_FeeblemindSpell;
			int mW03_HealSpell = m_Scroll.mW03_HealSpell;
			int mW04_MagicArrowSpell = m_Scroll.mW04_MagicArrowSpell;
			int mW05_NightSightSpell = m_Scroll.mW05_NightSightSpell;
			int mW06_ReactiveArmorSpell = m_Scroll.mW06_ReactiveArmorSpell;
			int mW07_WeakenSpell = m_Scroll.mW07_WeakenSpell;
			int mW08_AgilitySpell = m_Scroll.mW08_AgilitySpell;
			int mW09_CunningSpell = m_Scroll.mW09_CunningSpell;
			int mW10_CureSpell = m_Scroll.mW10_CureSpell;
			int mW11_HarmSpell = m_Scroll.mW11_HarmSpell;
			int mW12_MagicTrapSpell = m_Scroll.mW12_MagicTrapSpell;
			int mW13_RemoveTrapSpell = m_Scroll.mW13_RemoveTrapSpell;
			int mW14_ProtectionSpell = m_Scroll.mW14_ProtectionSpell;
			int mW15_StrengthSpell = m_Scroll.mW15_StrengthSpell;
			int mW16_BlessSpell = m_Scroll.mW16_BlessSpell;
			int mW17_FireballSpell = m_Scroll.mW17_FireballSpell;
			int mW18_MagicLockSpell = m_Scroll.mW18_MagicLockSpell;
			int mW19_PoisonSpell = m_Scroll.mW19_PoisonSpell;
			int mW20_TelekinesisSpell = m_Scroll.mW20_TelekinesisSpell;
			int mW21_TeleportSpell = m_Scroll.mW21_TeleportSpell;
			int mW22_UnlockSpell = m_Scroll.mW22_UnlockSpell;
			int mW23_WallOfStoneSpell = m_Scroll.mW23_WallOfStoneSpell;
			int mW24_ArchCureSpell = m_Scroll.mW24_ArchCureSpell;
			int mW25_ArchProtectionSpell = m_Scroll.mW25_ArchProtectionSpell;
			int mW26_CurseSpell = m_Scroll.mW26_CurseSpell;
			int mW27_FireFieldSpell = m_Scroll.mW27_FireFieldSpell;
			int mW28_GreaterHealSpell = m_Scroll.mW28_GreaterHealSpell;
			int mW29_LightningSpell = m_Scroll.mW29_LightningSpell;
			int mW30_ManaDrainSpell = m_Scroll.mW30_ManaDrainSpell;
			int mW31_RecallSpell = m_Scroll.mW31_RecallSpell;
			int mW32_BladeSpiritsSpell = m_Scroll.mW32_BladeSpiritsSpell;
			int mW33_DispelFieldSpell = m_Scroll.mW33_DispelFieldSpell;
			int mW34_IncognitoSpell = m_Scroll.mW34_IncognitoSpell;
			int mW35_MagicReflectSpell = m_Scroll.mW35_MagicReflectSpell;
			int mW36_MindBlastSpell = m_Scroll.mW36_MindBlastSpell;
			int mW37_ParalyzeSpell = m_Scroll.mW37_ParalyzeSpell;
			int mW38_PoisonFieldSpell = m_Scroll.mW38_PoisonFieldSpell;
			int mW39_SummonCreatureSpell = m_Scroll.mW39_SummonCreatureSpell;
			int mW40_DispelSpell = m_Scroll.mW40_DispelSpell;
			int mW41_EnergyBoltSpell = m_Scroll.mW41_EnergyBoltSpell;
			int mW42_ExplosionSpell = m_Scroll.mW42_ExplosionSpell;
			int mW43_InvisibilitySpell = m_Scroll.mW43_InvisibilitySpell;
			int mW44_MarkSpell = m_Scroll.mW44_MarkSpell;
			int mW45_MassCurseSpell = m_Scroll.mW45_MassCurseSpell;
			int mW46_ParalyzeFieldSpell = m_Scroll.mW46_ParalyzeFieldSpell;
			int mW47_RevealSpell = m_Scroll.mW47_RevealSpell;
			int mW48_ChainLightningSpell = m_Scroll.mW48_ChainLightningSpell;
			int mW49_EnergyFieldSpell = m_Scroll.mW49_EnergyFieldSpell;
			int mW50_FlameStrikeSpell = m_Scroll.mW50_FlameStrikeSpell;
			int mW51_GateTravelSpell = m_Scroll.mW51_GateTravelSpell;
			int mW52_ManaVampireSpell = m_Scroll.mW52_ManaVampireSpell;
			int mW53_MassDispelSpell = m_Scroll.mW53_MassDispelSpell;
			int mW54_MeteorSwarmSpell = m_Scroll.mW54_MeteorSwarmSpell;
			int mW55_PolymorphSpell = m_Scroll.mW55_PolymorphSpell;
			int mW56_EarthquakeSpell = m_Scroll.mW56_EarthquakeSpell;
			int mW57_EnergyVortexSpell = m_Scroll.mW57_EnergyVortexSpell;
			int mW58_ResurrectionSpell = m_Scroll.mW58_ResurrectionSpell;
			int mW59_AirElementalSpell = m_Scroll.mW59_AirElementalSpell;
			int mW60_SummonDaemonSpell = m_Scroll.mW60_SummonDaemonSpell;
			int mW61_EarthElementalSpell = m_Scroll.mW61_EarthElementalSpell;
			int mW62_FireElementalSpell = m_Scroll.mW62_FireElementalSpell;
			int mW63_WaterElementalSpell = m_Scroll.mW63_WaterElementalSpell;

			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;
			this.AddPage(0);
			this.AddBackground(52, 34, 668, 411, 9200);

			this.AddImage(60, 45, 2240);
			this.AddImage(60, 90, 2241);
			this.AddImage(60, 135, 2242);
			this.AddImage(60, 180, 2243);
			this.AddImage(60, 225, 2244);
			this.AddImage(60, 270, 2245);
			this.AddImage(60, 315, 2246);
			this.AddImage(60, 360, 2247);
			this.AddImage(142, 45, 2248);
			this.AddImage(142, 90, 2249);
			this.AddImage(142, 135, 2250);
			this.AddImage(142, 180, 2251);
			this.AddImage(142, 225, 2252);
			this.AddImage(142, 270, 2253);
			this.AddImage(142, 315, 2254);
			this.AddImage(142, 360, 2255);
			this.AddImage(225, 45, 2256);
			this.AddImage(225, 90, 2257);
			this.AddImage(225, 135, 2258);
			this.AddImage(225, 180, 2259);
			this.AddImage(225, 225, 2260);
			this.AddImage(225, 270, 2261);
			this.AddImage(225, 315, 2262);
			this.AddImage(225, 360, 2263);
			this.AddImage(308, 45, 2264);
			this.AddImage(308, 90, 2265);
			this.AddImage(308, 135, 2266);
			this.AddImage(308, 180, 2267);
			this.AddImage(308, 225, 2268);
			this.AddImage(308, 270, 2269);
			this.AddImage(308, 315, 2270);
			this.AddImage(308, 360, 2271);
			this.AddImage(392, 44, 2272);
			this.AddImage(392, 89, 2273);
			this.AddImage(392, 134, 2274);
			this.AddImage(392, 179, 2275);
			this.AddImage(392, 224, 2276);
			this.AddImage(392, 269, 2277);
			this.AddImage(392, 314, 2278);
			this.AddImage(392, 359, 2279);
			this.AddImage(474, 44, 2280);
			this.AddImage(474, 89, 2281);
			this.AddImage(474, 134, 2282);
			this.AddImage(474, 179, 2283);
			this.AddImage(474, 224, 2284);
			this.AddImage(474, 269, 2285);
			this.AddImage(474, 314, 2286);
			this.AddImage(474, 359, 2287);
			this.AddImage(557, 44, 2288);
			this.AddImage(557, 89, 2289);
			this.AddImage(557, 134, 2290);
			this.AddImage(557, 179, 2291);
			this.AddImage(557, 224, 2292);
			this.AddImage(557, 269, 2293);
			this.AddImage(557, 314, 2294);
			this.AddImage(557, 359, 2295);
			this.AddImage(640, 44, 2296);
			this.AddImage(640, 89, 2297);
			this.AddImage(640, 134, 2298);
			this.AddImage(640, 179, 2299);
			this.AddImage(640, 224, 2300);
			this.AddImage(640, 269, 2301);
			this.AddImage(640, 314, 2302);
			this.AddImage(640, 359, 2303);

			if ( mW00_ClumsySpell == 1 ) { this.AddButton(110, 55,  2361, 2361, 99, GumpButtonType.Reply, 1); }
			if ( mW01_CreateFoodSpell == 1 ) { this.AddButton(110, 100, 2361, 2361, 1, GumpButtonType.Reply, 1); }
			if ( mW02_FeeblemindSpell == 1 ) { this.AddButton(110, 145, 2361, 2361, 2, GumpButtonType.Reply, 1); }
			if ( mW03_HealSpell == 1 ) { this.AddButton(110, 190, 2361, 2361, 3, GumpButtonType.Reply, 1); }
			if ( mW04_MagicArrowSpell == 1 ) { this.AddButton(110, 235, 2361, 2361, 4, GumpButtonType.Reply, 1); }
			if ( mW05_NightSightSpell == 1 ) { this.AddButton(110, 280, 2361, 2361, 5, GumpButtonType.Reply, 1); }
			if ( mW06_ReactiveArmorSpell == 1 ) { this.AddButton(110, 325, 2361, 2361, 6, GumpButtonType.Reply, 1); }
			if ( mW07_WeakenSpell == 1 ) { this.AddButton(110, 370, 2361, 2361, 7, GumpButtonType.Reply, 1); }
			if ( mW08_AgilitySpell == 1 ) { this.AddButton(192, 55,  2361, 2361, 8, GumpButtonType.Reply, 1); }
			if ( mW09_CunningSpell == 1 ) { this.AddButton(192, 100, 2361, 2361, 9, GumpButtonType.Reply, 1); }
			if ( mW10_CureSpell == 1 ) { this.AddButton(192, 145, 2361, 2361, 10, GumpButtonType.Reply, 1); }
			if ( mW11_HarmSpell == 1 ) { this.AddButton(192, 190, 2361, 2361, 11, GumpButtonType.Reply, 1); }
			if ( mW12_MagicTrapSpell == 1 ) { this.AddButton(192, 235, 2361, 2361, 12, GumpButtonType.Reply, 1); }
			if ( mW13_RemoveTrapSpell == 1 ) { this.AddButton(192, 280, 2361, 2361, 13, GumpButtonType.Reply, 1); }
			if ( mW14_ProtectionSpell == 1 ) { this.AddButton(192, 325, 2361, 2361, 14, GumpButtonType.Reply, 1); }
			if ( mW15_StrengthSpell == 1 ) { this.AddButton(192, 370, 2361, 2361, 15, GumpButtonType.Reply, 1); }
			if ( mW16_BlessSpell == 1 ) { this.AddButton(275, 55,  2361, 2361, 16, GumpButtonType.Reply, 1); }
			if ( mW17_FireballSpell == 1 ) { this.AddButton(275, 100, 2361, 2361, 17, GumpButtonType.Reply, 1); }
			if ( mW18_MagicLockSpell == 1 ) { this.AddButton(275, 145, 2361, 2361, 18, GumpButtonType.Reply, 1); }
			if ( mW19_PoisonSpell == 1 ) { this.AddButton(275, 190, 2361, 2361, 19, GumpButtonType.Reply, 1); }
			if ( mW20_TelekinesisSpell == 1 ) { this.AddButton(275, 235, 2361, 2361, 20, GumpButtonType.Reply, 1); }
			if ( mW21_TeleportSpell == 1 ) { this.AddButton(275, 280, 2361, 2361, 21, GumpButtonType.Reply, 1); }
			if ( mW22_UnlockSpell == 1 ) { this.AddButton(275, 325, 2361, 2361, 22, GumpButtonType.Reply, 1); }
			if ( mW23_WallOfStoneSpell == 1 ) { this.AddButton(275, 370, 2361, 2361, 23, GumpButtonType.Reply, 1); }
			if ( mW24_ArchCureSpell == 1 ) { this.AddButton(358, 55,  2361, 2361, 24, GumpButtonType.Reply, 1); }
			if ( mW25_ArchProtectionSpell == 1 ) { this.AddButton(358, 100, 2361, 2361, 25, GumpButtonType.Reply, 1); }
			if ( mW26_CurseSpell == 1 ) { this.AddButton(358, 145, 2361, 2361, 26, GumpButtonType.Reply, 1); }
			if ( mW27_FireFieldSpell == 1 ) { this.AddButton(358, 190, 2361, 2361, 27, GumpButtonType.Reply, 1); }
			if ( mW28_GreaterHealSpell == 1 ) { this.AddButton(358, 235, 2361, 2361, 28, GumpButtonType.Reply, 1); }
			if ( mW29_LightningSpell == 1 ) { this.AddButton(358, 280, 2361, 2361, 29, GumpButtonType.Reply, 1); }
			if ( mW30_ManaDrainSpell == 1 ) { this.AddButton(358, 325, 2361, 2361, 30, GumpButtonType.Reply, 1); }
			if ( mW31_RecallSpell == 1 ) { this.AddButton(358, 370, 2361, 2361, 31, GumpButtonType.Reply, 1); }
			if ( mW32_BladeSpiritsSpell == 1 ) { this.AddButton(442, 54,  2361, 2361, 32, GumpButtonType.Reply, 1); }
			if ( mW33_DispelFieldSpell == 1 ) { this.AddButton(442, 99,  2361, 2361, 33, GumpButtonType.Reply, 1); }
			if ( mW34_IncognitoSpell == 1 ) { this.AddButton(442, 144, 2361, 2361, 34, GumpButtonType.Reply, 1); }
			if ( mW35_MagicReflectSpell == 1 ) { this.AddButton(442, 189, 2361, 2361, 35, GumpButtonType.Reply, 1); }
			if ( mW36_MindBlastSpell == 1 ) { this.AddButton(442, 234, 2361, 2361, 36, GumpButtonType.Reply, 1); }
			if ( mW37_ParalyzeSpell == 1 ) { this.AddButton(442, 279, 2361, 2361, 37, GumpButtonType.Reply, 1); }
			if ( mW38_PoisonFieldSpell == 1 ) { this.AddButton(442, 324, 2361, 2361, 38, GumpButtonType.Reply, 1); }
			if ( mW39_SummonCreatureSpell == 1 ) { this.AddButton(442, 369, 2361, 2361, 39, GumpButtonType.Reply, 1); }
			if ( mW40_DispelSpell == 1 ) { this.AddButton(524, 54,  2361, 2361, 40, GumpButtonType.Reply, 1); }
			if ( mW41_EnergyBoltSpell == 1 ) { this.AddButton(524, 99,  2361, 2361, 41, GumpButtonType.Reply, 1); }
			if ( mW42_ExplosionSpell == 1 ) { this.AddButton(524, 144, 2361, 2361, 42, GumpButtonType.Reply, 1); }
			if ( mW43_InvisibilitySpell == 1 ) { this.AddButton(524, 189, 2361, 2361, 43, GumpButtonType.Reply, 1); }
			if ( mW44_MarkSpell == 1 ) { this.AddButton(524, 234, 2361, 2361, 44, GumpButtonType.Reply, 1); }
			if ( mW45_MassCurseSpell == 1 ) { this.AddButton(524, 279, 2361, 2361, 45, GumpButtonType.Reply, 1); }
			if ( mW46_ParalyzeFieldSpell == 1 ) { this.AddButton(524, 324, 2361, 2361, 46, GumpButtonType.Reply, 1); }
			if ( mW47_RevealSpell == 1 ) { this.AddButton(524, 369, 2361, 2361, 47, GumpButtonType.Reply, 1); }
			if ( mW48_ChainLightningSpell == 1 ) { this.AddButton(607, 54,  2361, 2361, 48, GumpButtonType.Reply, 1); }
			if ( mW49_EnergyFieldSpell == 1 ) { this.AddButton(607, 99,  2361, 2361, 49, GumpButtonType.Reply, 1); }
			if ( mW50_FlameStrikeSpell == 1 ) { this.AddButton(607, 144, 2361, 2361, 50, GumpButtonType.Reply, 1); }
			if ( mW51_GateTravelSpell == 1 ) { this.AddButton(607, 189, 2361, 2361, 51, GumpButtonType.Reply, 1); }
			if ( mW52_ManaVampireSpell == 1 ) { this.AddButton(607, 234, 2361, 2361, 52, GumpButtonType.Reply, 1); }
			if ( mW53_MassDispelSpell == 1 ) { this.AddButton(607, 279, 2361, 2361, 53, GumpButtonType.Reply, 1); }
			if ( mW54_MeteorSwarmSpell == 1 ) { this.AddButton(607, 324, 2361, 2361, 54, GumpButtonType.Reply, 1); }
			if ( mW55_PolymorphSpell == 1 ) { this.AddButton(607, 369, 2361, 2361, 55, GumpButtonType.Reply, 1); }
			if ( mW56_EarthquakeSpell == 1 ) { this.AddButton(690, 54,  2361, 2361, 56, GumpButtonType.Reply, 1); }
			if ( mW57_EnergyVortexSpell == 1 ) { this.AddButton(690, 99,  2361, 2361, 57, GumpButtonType.Reply, 1); }
			if ( mW58_ResurrectionSpell == 1 ) { this.AddButton(690, 144, 2361, 2361, 58, GumpButtonType.Reply, 1); }
			if ( mW59_AirElementalSpell == 1 ) { this.AddButton(690, 189, 2361, 2361, 59, GumpButtonType.Reply, 1); }
			if ( mW60_SummonDaemonSpell == 1 ) { this.AddButton(690, 234, 2361, 2361, 60, GumpButtonType.Reply, 1); }
			if ( mW61_EarthElementalSpell == 1 ) { this.AddButton(690, 279, 2361, 2361, 61, GumpButtonType.Reply, 1); }
			if ( mW62_FireElementalSpell == 1 ) { this.AddButton(690, 324, 2361, 2361, 62, GumpButtonType.Reply, 1); }
			if ( mW63_WaterElementalSpell == 1 ) { this.AddButton(690, 369, 2361, 2361, 63, GumpButtonType.Reply, 1); }

			if ( mW00_ClumsySpell == 0 ) { this.AddButton(110, 55,  2360, 2360, 99, GumpButtonType.Reply, 1); }
			if ( mW01_CreateFoodSpell == 0 ) { this.AddButton(110, 100, 2360, 2360, 1, GumpButtonType.Reply, 1); }
			if ( mW02_FeeblemindSpell == 0 ) { this.AddButton(110, 145, 2360, 2360, 2, GumpButtonType.Reply, 1); }
			if ( mW03_HealSpell == 0 ) { this.AddButton(110, 190, 2360, 2360, 3, GumpButtonType.Reply, 1); }
			if ( mW04_MagicArrowSpell == 0 ) { this.AddButton(110, 235, 2360, 2360, 4, GumpButtonType.Reply, 1); }
			if ( mW05_NightSightSpell == 0 ) { this.AddButton(110, 280, 2360, 2360, 5, GumpButtonType.Reply, 1); }
			if ( mW06_ReactiveArmorSpell == 0 ) { this.AddButton(110, 325, 2360, 2360, 6, GumpButtonType.Reply, 1); }
			if ( mW07_WeakenSpell == 0 ) { this.AddButton(110, 370, 2360, 2360, 7, GumpButtonType.Reply, 1); }
			if ( mW08_AgilitySpell == 0 ) { this.AddButton(192, 55,  2360, 2360, 8, GumpButtonType.Reply, 1); }
			if ( mW09_CunningSpell == 0 ) { this.AddButton(192, 100, 2360, 2360, 9, GumpButtonType.Reply, 1); }
			if ( mW10_CureSpell == 0 ) { this.AddButton(192, 145, 2360, 2360, 10, GumpButtonType.Reply, 1); }
			if ( mW11_HarmSpell == 0 ) { this.AddButton(192, 190, 2360, 2360, 11, GumpButtonType.Reply, 1); }
			if ( mW12_MagicTrapSpell == 0 ) { this.AddButton(192, 235, 2360, 2360, 12, GumpButtonType.Reply, 1); }
			if ( mW13_RemoveTrapSpell == 0 ) { this.AddButton(192, 280, 2360, 2360, 13, GumpButtonType.Reply, 1); }
			if ( mW14_ProtectionSpell == 0 ) { this.AddButton(192, 325, 2360, 2360, 14, GumpButtonType.Reply, 1); }
			if ( mW15_StrengthSpell == 0 ) { this.AddButton(192, 370, 2360, 2360, 15, GumpButtonType.Reply, 1); }
			if ( mW16_BlessSpell == 0 ) { this.AddButton(275, 55,  2360, 2360, 16, GumpButtonType.Reply, 1); }
			if ( mW17_FireballSpell == 0 ) { this.AddButton(275, 100, 2360, 2360, 17, GumpButtonType.Reply, 1); }
			if ( mW18_MagicLockSpell == 0 ) { this.AddButton(275, 145, 2360, 2360, 18, GumpButtonType.Reply, 1); }
			if ( mW19_PoisonSpell == 0 ) { this.AddButton(275, 190, 2360, 2360, 19, GumpButtonType.Reply, 1); }
			if ( mW20_TelekinesisSpell == 0 ) { this.AddButton(275, 235, 2360, 2360, 20, GumpButtonType.Reply, 1); }
			if ( mW21_TeleportSpell == 0 ) { this.AddButton(275, 280, 2360, 2360, 21, GumpButtonType.Reply, 1); }
			if ( mW22_UnlockSpell == 0 ) { this.AddButton(275, 325, 2360, 2360, 22, GumpButtonType.Reply, 1); }
			if ( mW23_WallOfStoneSpell == 0 ) { this.AddButton(275, 370, 2360, 2360, 23, GumpButtonType.Reply, 1); }
			if ( mW24_ArchCureSpell == 0 ) { this.AddButton(358, 55,  2360, 2360, 24, GumpButtonType.Reply, 1); }
			if ( mW25_ArchProtectionSpell == 0 ) { this.AddButton(358, 100, 2360, 2360, 25, GumpButtonType.Reply, 1); }
			if ( mW26_CurseSpell == 0 ) { this.AddButton(358, 145, 2360, 2360, 26, GumpButtonType.Reply, 1); }
			if ( mW27_FireFieldSpell == 0 ) { this.AddButton(358, 190, 2360, 2360, 27, GumpButtonType.Reply, 1); }
			if ( mW28_GreaterHealSpell == 0 ) { this.AddButton(358, 235, 2360, 2360, 28, GumpButtonType.Reply, 1); }
			if ( mW29_LightningSpell == 0 ) { this.AddButton(358, 280, 2360, 2360, 29, GumpButtonType.Reply, 1); }
			if ( mW30_ManaDrainSpell == 0 ) { this.AddButton(358, 325, 2360, 2360, 30, GumpButtonType.Reply, 1); }
			if ( mW31_RecallSpell == 0 ) { this.AddButton(358, 370, 2360, 2360, 31, GumpButtonType.Reply, 1); }
			if ( mW32_BladeSpiritsSpell == 0 ) { this.AddButton(442, 54,  2360, 2360, 32, GumpButtonType.Reply, 1); }
			if ( mW33_DispelFieldSpell == 0 ) { this.AddButton(442, 99,  2360, 2360, 33, GumpButtonType.Reply, 1); }
			if ( mW34_IncognitoSpell == 0 ) { this.AddButton(442, 144, 2360, 2360, 34, GumpButtonType.Reply, 1); }
			if ( mW35_MagicReflectSpell == 0 ) { this.AddButton(442, 189, 2360, 2360, 35, GumpButtonType.Reply, 1); }
			if ( mW36_MindBlastSpell == 0 ) { this.AddButton(442, 234, 2360, 2360, 36, GumpButtonType.Reply, 1); }
			if ( mW37_ParalyzeSpell == 0 ) { this.AddButton(442, 279, 2360, 2360, 37, GumpButtonType.Reply, 1); }
			if ( mW38_PoisonFieldSpell == 0 ) { this.AddButton(442, 324, 2360, 2360, 38, GumpButtonType.Reply, 1); }
			if ( mW39_SummonCreatureSpell == 0 ) { this.AddButton(442, 369, 2360, 2360, 39, GumpButtonType.Reply, 1); }
			if ( mW40_DispelSpell == 0 ) { this.AddButton(524, 54,  2360, 2360, 40, GumpButtonType.Reply, 1); }
			if ( mW41_EnergyBoltSpell == 0 ) { this.AddButton(524, 99,  2360, 2360, 41, GumpButtonType.Reply, 1); }
			if ( mW42_ExplosionSpell == 0 ) { this.AddButton(524, 144, 2360, 2360, 42, GumpButtonType.Reply, 1); }
			if ( mW43_InvisibilitySpell == 0 ) { this.AddButton(524, 189, 2360, 2360, 43, GumpButtonType.Reply, 1); }
			if ( mW44_MarkSpell == 0 ) { this.AddButton(524, 234, 2360, 2360, 44, GumpButtonType.Reply, 1); }
			if ( mW45_MassCurseSpell == 0 ) { this.AddButton(524, 279, 2360, 2360, 45, GumpButtonType.Reply, 1); }
			if ( mW46_ParalyzeFieldSpell == 0 ) { this.AddButton(524, 324, 2360, 2360, 46, GumpButtonType.Reply, 1); }
			if ( mW47_RevealSpell == 0 ) { this.AddButton(524, 369, 2360, 2360, 47, GumpButtonType.Reply, 1); }
			if ( mW48_ChainLightningSpell == 0 ) { this.AddButton(607, 54,  2360, 2360, 48, GumpButtonType.Reply, 1); }
			if ( mW49_EnergyFieldSpell == 0 ) { this.AddButton(607, 99,  2360, 2360, 49, GumpButtonType.Reply, 1); }
			if ( mW50_FlameStrikeSpell == 0 ) { this.AddButton(607, 144, 2360, 2360, 50, GumpButtonType.Reply, 1); }
			if ( mW51_GateTravelSpell == 0 ) { this.AddButton(607, 189, 2360, 2360, 51, GumpButtonType.Reply, 1); }
			if ( mW52_ManaVampireSpell == 0 ) { this.AddButton(607, 234, 2360, 2360, 52, GumpButtonType.Reply, 1); }
			if ( mW53_MassDispelSpell == 0 ) { this.AddButton(607, 279, 2360, 2360, 53, GumpButtonType.Reply, 1); }
			if ( mW54_MeteorSwarmSpell == 0 ) { this.AddButton(607, 324, 2360, 2360, 54, GumpButtonType.Reply, 1); }
			if ( mW55_PolymorphSpell == 0 ) { this.AddButton(607, 369, 2360, 2360, 55, GumpButtonType.Reply, 1); }
			if ( mW56_EarthquakeSpell == 0 ) { this.AddButton(690, 54,  2360, 2360, 56, GumpButtonType.Reply, 1); }
			if ( mW57_EnergyVortexSpell == 0 ) { this.AddButton(690, 99,  2360, 2360, 57, GumpButtonType.Reply, 1); }
			if ( mW58_ResurrectionSpell == 0 ) { this.AddButton(690, 144, 2360, 2360, 58, GumpButtonType.Reply, 1); }
			if ( mW59_AirElementalSpell == 0 ) { this.AddButton(690, 189, 2360, 2360, 59, GumpButtonType.Reply, 1); }
			if ( mW60_SummonDaemonSpell == 0 ) { this.AddButton(690, 234, 2360, 2360, 60, GumpButtonType.Reply, 1); }
			if ( mW61_EarthElementalSpell == 0 ) { this.AddButton(690, 279, 2360, 2360, 61, GumpButtonType.Reply, 1); }
			if ( mW62_FireElementalSpell == 0 ) { this.AddButton(690, 324, 2360, 2360, 62, GumpButtonType.Reply, 1); }
			if ( mW63_WaterElementalSpell == 0 ) { this.AddButton(690, 369, 2360, 2360, 63, GumpButtonType.Reply, 1); }

			this.AddButton(149, 408, 2152, 2152, 64, GumpButtonType.Reply, 1); // TOOLBAR
			this.AddLabel(60, 412, 52, @"Open Toolbar");
		}

	public override void OnResponse( NetState state, RelayInfo info )
	{
		Mobile from = state.Mobile;

		switch ( info.ButtonID )
		{
			case 0:
			{
				break;
			}
			case 99:
			{
				if ( m_Scroll.mW00_ClumsySpell == 0 ) { m_Scroll.mW00_ClumsySpell = 1; }
				else { m_Scroll.mW00_ClumsySpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 1:
			{
				if ( m_Scroll.mW01_CreateFoodSpell == 0 ) { m_Scroll.mW01_CreateFoodSpell = 1; }
				else { m_Scroll.mW01_CreateFoodSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 2:
			{
				if ( m_Scroll.mW02_FeeblemindSpell == 0 ) { m_Scroll.mW02_FeeblemindSpell = 1; }
				else { m_Scroll.mW02_FeeblemindSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 3:
			{
				if ( m_Scroll.mW03_HealSpell == 0 ) { m_Scroll.mW03_HealSpell = 1; }
				else { m_Scroll.mW03_HealSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 4:
			{
				if ( m_Scroll.mW04_MagicArrowSpell == 0 ) { m_Scroll.mW04_MagicArrowSpell = 1; }
				else { m_Scroll.mW04_MagicArrowSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 5:
			{
				if ( m_Scroll.mW05_NightSightSpell == 0 ) { m_Scroll.mW05_NightSightSpell = 1; }
				else { m_Scroll.mW05_NightSightSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 6:
			{
				if ( m_Scroll.mW06_ReactiveArmorSpell == 0 ) { m_Scroll.mW06_ReactiveArmorSpell = 1; }
				else { m_Scroll.mW06_ReactiveArmorSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 7:
			{
				if ( m_Scroll.mW07_WeakenSpell == 0 ) { m_Scroll.mW07_WeakenSpell = 1; }
				else { m_Scroll.mW07_WeakenSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 8:
			{
				if ( m_Scroll.mW08_AgilitySpell == 0 ) { m_Scroll.mW08_AgilitySpell = 1; }
				else { m_Scroll.mW08_AgilitySpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 9:
			{
				if ( m_Scroll.mW09_CunningSpell == 0 ) { m_Scroll.mW09_CunningSpell = 1; }
				else { m_Scroll.mW09_CunningSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 10:
			{
				if ( m_Scroll.mW10_CureSpell == 0 ) { m_Scroll.mW10_CureSpell = 1; }
				else { m_Scroll.mW10_CureSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 11:
			{
				if ( m_Scroll.mW11_HarmSpell == 0 ) { m_Scroll.mW11_HarmSpell = 1; }
				else { m_Scroll.mW11_HarmSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 12:
			{
				if ( m_Scroll.mW12_MagicTrapSpell == 0 ) { m_Scroll.mW12_MagicTrapSpell = 1; }
				else { m_Scroll.mW12_MagicTrapSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 13:
			{
				if ( m_Scroll.mW13_RemoveTrapSpell == 0 ) { m_Scroll.mW13_RemoveTrapSpell = 1; }
				else { m_Scroll.mW13_RemoveTrapSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 14:
			{
				if ( m_Scroll.mW14_ProtectionSpell == 0 ) { m_Scroll.mW14_ProtectionSpell = 1; }
				else { m_Scroll.mW14_ProtectionSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 15:
			{
				if ( m_Scroll.mW15_StrengthSpell == 0 ) { m_Scroll.mW15_StrengthSpell = 1; }
				else { m_Scroll.mW15_StrengthSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 16:
			{
				if ( m_Scroll.mW16_BlessSpell == 0 ) { m_Scroll.mW16_BlessSpell = 1; }
				else { m_Scroll.mW16_BlessSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 17:
			{
				if ( m_Scroll.mW17_FireballSpell == 0 ) { m_Scroll.mW17_FireballSpell = 1; }
				else { m_Scroll.mW17_FireballSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 18:
			{
				if ( m_Scroll.mW18_MagicLockSpell == 0 ) { m_Scroll.mW18_MagicLockSpell = 1; }
				else { m_Scroll.mW18_MagicLockSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 19:
			{
				if ( m_Scroll.mW19_PoisonSpell == 0 ) { m_Scroll.mW19_PoisonSpell = 1; }
				else { m_Scroll.mW19_PoisonSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 20:
			{
				if ( m_Scroll.mW20_TelekinesisSpell == 0 ) { m_Scroll.mW20_TelekinesisSpell = 1; }
				else { m_Scroll.mW20_TelekinesisSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 21:
			{
				if ( m_Scroll.mW21_TeleportSpell == 0 ) { m_Scroll.mW21_TeleportSpell = 1; }
				else { m_Scroll.mW21_TeleportSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 22:
			{
				if ( m_Scroll.mW22_UnlockSpell == 0 ) { m_Scroll.mW22_UnlockSpell = 1; }
				else { m_Scroll.mW22_UnlockSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 23:
			{
				if ( m_Scroll.mW23_WallOfStoneSpell == 0 ) { m_Scroll.mW23_WallOfStoneSpell = 1; }
				else { m_Scroll.mW23_WallOfStoneSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 24:
			{
				if ( m_Scroll.mW24_ArchCureSpell == 0 ) { m_Scroll.mW24_ArchCureSpell = 1; }
				else { m_Scroll.mW24_ArchCureSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 25:
			{
				if ( m_Scroll.mW25_ArchProtectionSpell == 0 ) { m_Scroll.mW25_ArchProtectionSpell = 1; }
				else { m_Scroll.mW25_ArchProtectionSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 26:
			{
				if ( m_Scroll.mW26_CurseSpell == 0 ) { m_Scroll.mW26_CurseSpell = 1; }
				else { m_Scroll.mW26_CurseSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 27:
			{
				if ( m_Scroll.mW27_FireFieldSpell == 0 ) { m_Scroll.mW27_FireFieldSpell = 1; }
				else { m_Scroll.mW27_FireFieldSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 28:
			{
				if ( m_Scroll.mW28_GreaterHealSpell == 0 ) { m_Scroll.mW28_GreaterHealSpell = 1; }
				else { m_Scroll.mW28_GreaterHealSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 29:
			{
				if ( m_Scroll.mW29_LightningSpell == 0 ) { m_Scroll.mW29_LightningSpell = 1; }
				else { m_Scroll.mW29_LightningSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 30:
			{
				if ( m_Scroll.mW30_ManaDrainSpell == 0 ) { m_Scroll.mW30_ManaDrainSpell = 1; }
				else { m_Scroll.mW30_ManaDrainSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 31:
			{
				if ( m_Scroll.mW31_RecallSpell == 0 ) { m_Scroll.mW31_RecallSpell = 1; }
				else { m_Scroll.mW31_RecallSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 32:
			{
				if ( m_Scroll.mW32_BladeSpiritsSpell == 0 ) { m_Scroll.mW32_BladeSpiritsSpell = 1; }
				else { m_Scroll.mW32_BladeSpiritsSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 33:
			{
				if ( m_Scroll.mW33_DispelFieldSpell == 0 ) { m_Scroll.mW33_DispelFieldSpell = 1; }
				else { m_Scroll.mW33_DispelFieldSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 34:
			{
				if ( m_Scroll.mW34_IncognitoSpell == 0 ) { m_Scroll.mW34_IncognitoSpell = 1; }
				else { m_Scroll.mW34_IncognitoSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 35:
			{
				if ( m_Scroll.mW35_MagicReflectSpell == 0 ) { m_Scroll.mW35_MagicReflectSpell = 1; }
				else { m_Scroll.mW35_MagicReflectSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 36:
			{
				if ( m_Scroll.mW36_MindBlastSpell == 0 ) { m_Scroll.mW36_MindBlastSpell = 1; }
				else { m_Scroll.mW36_MindBlastSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 37:
			{
				if ( m_Scroll.mW37_ParalyzeSpell == 0 ) { m_Scroll.mW37_ParalyzeSpell = 1; }
				else { m_Scroll.mW37_ParalyzeSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 38:
			{
				if ( m_Scroll.mW38_PoisonFieldSpell == 0 ) { m_Scroll.mW38_PoisonFieldSpell = 1; }
				else { m_Scroll.mW38_PoisonFieldSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 39:
			{
				if ( m_Scroll.mW39_SummonCreatureSpell == 0 ) { m_Scroll.mW39_SummonCreatureSpell = 1; }
				else { m_Scroll.mW39_SummonCreatureSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 40:
			{
				if ( m_Scroll.mW40_DispelSpell == 0 ) { m_Scroll.mW40_DispelSpell = 1; }
				else { m_Scroll.mW40_DispelSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 41:
			{
				if ( m_Scroll.mW41_EnergyBoltSpell == 0 ) { m_Scroll.mW41_EnergyBoltSpell = 1; }
				else { m_Scroll.mW41_EnergyBoltSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 42:
			{
				if ( m_Scroll.mW42_ExplosionSpell == 0 ) { m_Scroll.mW42_ExplosionSpell = 1; }
				else { m_Scroll.mW42_ExplosionSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 43:
			{
				if ( m_Scroll.mW43_InvisibilitySpell == 0 ) { m_Scroll.mW43_InvisibilitySpell = 1; }
				else { m_Scroll.mW43_InvisibilitySpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 44:
			{
				if ( m_Scroll.mW44_MarkSpell == 0 ) { m_Scroll.mW44_MarkSpell = 1; }
				else { m_Scroll.mW44_MarkSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 45:
			{
				if ( m_Scroll.mW45_MassCurseSpell == 0 ) { m_Scroll.mW45_MassCurseSpell = 1; }
				else { m_Scroll.mW45_MassCurseSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 46:
			{
				if ( m_Scroll.mW46_ParalyzeFieldSpell == 0 ) { m_Scroll.mW46_ParalyzeFieldSpell = 1; }
				else { m_Scroll.mW46_ParalyzeFieldSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 47:
			{
				if ( m_Scroll.mW47_RevealSpell == 0 ) { m_Scroll.mW47_RevealSpell = 1; }
				else { m_Scroll.mW47_RevealSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 48:
			{
				if ( m_Scroll.mW48_ChainLightningSpell == 0 ) { m_Scroll.mW48_ChainLightningSpell = 1; }
				else { m_Scroll.mW48_ChainLightningSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 49:
			{
				if ( m_Scroll.mW49_EnergyFieldSpell == 0 ) { m_Scroll.mW49_EnergyFieldSpell = 1; }
				else { m_Scroll.mW49_EnergyFieldSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 50:
			{
				if ( m_Scroll.mW50_FlameStrikeSpell == 0 ) { m_Scroll.mW50_FlameStrikeSpell = 1; }
				else { m_Scroll.mW50_FlameStrikeSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 51:
			{
				if ( m_Scroll.mW51_GateTravelSpell == 0 ) { m_Scroll.mW51_GateTravelSpell = 1; }
				else { m_Scroll.mW51_GateTravelSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 52:
			{
				if ( m_Scroll.mW52_ManaVampireSpell == 0 ) { m_Scroll.mW52_ManaVampireSpell = 1; }
				else { m_Scroll.mW52_ManaVampireSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 53:
			{
				if ( m_Scroll.mW53_MassDispelSpell == 0 ) { m_Scroll.mW53_MassDispelSpell = 1; }
				else { m_Scroll.mW53_MassDispelSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 54:
			{
				if ( m_Scroll.mW54_MeteorSwarmSpell == 0 ) { m_Scroll.mW54_MeteorSwarmSpell = 1; }
				else { m_Scroll.mW54_MeteorSwarmSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 55:
			{
				if ( m_Scroll.mW55_PolymorphSpell == 0 ) { m_Scroll.mW55_PolymorphSpell = 1; }
				else { m_Scroll.mW55_PolymorphSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 56:
			{
				if ( m_Scroll.mW56_EarthquakeSpell == 0 ) { m_Scroll.mW56_EarthquakeSpell = 1; }
				else { m_Scroll.mW56_EarthquakeSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 57:
			{
				if ( m_Scroll.mW57_EnergyVortexSpell == 0 ) { m_Scroll.mW57_EnergyVortexSpell = 1; }
				else { m_Scroll.mW57_EnergyVortexSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 58:
			{
				if ( m_Scroll.mW58_ResurrectionSpell == 0 ) { m_Scroll.mW58_ResurrectionSpell = 1; }
				else { m_Scroll.mW58_ResurrectionSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 59:
			{
				if ( m_Scroll.mW59_AirElementalSpell == 0 ) { m_Scroll.mW59_AirElementalSpell = 1; }
				else { m_Scroll.mW59_AirElementalSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 60:
			{
				if ( m_Scroll.mW60_SummonDaemonSpell == 0 ) { m_Scroll.mW60_SummonDaemonSpell = 1; }
				else { m_Scroll.mW60_SummonDaemonSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 61:
			{
				if ( m_Scroll.mW61_EarthElementalSpell == 0 ) { m_Scroll.mW61_EarthElementalSpell = 1; }
				else { m_Scroll.mW61_EarthElementalSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 62:
			{
				if ( m_Scroll.mW62_FireElementalSpell == 0 ) { m_Scroll.mW62_FireElementalSpell = 1; }
				else { m_Scroll.mW62_FireElementalSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 63:
			{
				if ( m_Scroll.mW63_WaterElementalSpell == 0 ) { m_Scroll.mW63_WaterElementalSpell = 1; }
				else { m_Scroll.mW63_WaterElementalSpell = 0; }
				from.SendGump( new Tools_sorcery_scrollGump( from, m_Scroll ) );
				break;
			}
			case 64:
			{
				from.CloseGump( typeof( Tools_tools_sorcery ) );
				from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) );
				break;
			}
		}
	}}

	public class Tools_tools_sorcery : Gump
	{
		public static bool HasSpell( Mobile from, int spellID )
		{
			Spellbook book = Spellbook.Find( from, spellID );
			return ( book != null && book.HasSpell( spellID ) );
		}

		private Tools_sorcery_scroll m_Scroll;

		public Tools_tools_sorcery( Mobile from, Tools_sorcery_scroll scroll ) : base( 0, 0 )
		{
			m_Scroll = scroll;
			this.Closable=false;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;
			this.AddPage(0);
			this.AddImage(0, 0, 2234, 0);
			int dby = 50;

			if ( HasSpell( from, 0 ) && m_Scroll.mW00_ClumsySpell == 1){this.AddButton(dby, 5, 2240, 2240, 99, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 1 ) && m_Scroll.mW01_CreateFoodSpell == 1){this.AddButton(dby, 5, 2241, 2241, 1, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 2 ) && m_Scroll.mW02_FeeblemindSpell == 1){this.AddButton(dby, 5, 2242, 2242, 2, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 3 ) && m_Scroll.mW03_HealSpell == 1){this.AddButton(dby, 5, 2243, 2243, 3, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 4 ) && m_Scroll.mW04_MagicArrowSpell == 1){this.AddButton(dby, 5, 2244, 2244, 4, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 5 ) && m_Scroll.mW05_NightSightSpell == 1){this.AddButton(dby, 5, 2245, 2245, 5, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 6 ) && m_Scroll.mW06_ReactiveArmorSpell == 1){this.AddButton(dby, 5, 2246, 2246, 6, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 7 ) && m_Scroll.mW07_WeakenSpell == 1){this.AddButton(dby, 5, 2247, 2247, 7, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 8 ) && m_Scroll.mW08_AgilitySpell == 1){this.AddButton(dby, 5, 2248, 2248, 8, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 9 ) && m_Scroll.mW09_CunningSpell == 1){this.AddButton(dby, 5, 2249, 2249, 9, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 10 ) && m_Scroll.mW10_CureSpell == 1){this.AddButton(dby, 5, 2250, 2250, 10, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 11 ) && m_Scroll.mW11_HarmSpell == 1){this.AddButton(dby, 5, 2251, 2251, 11, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 12 ) && m_Scroll.mW12_MagicTrapSpell == 1){this.AddButton(dby, 5, 2252, 2252, 12, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 13 ) && m_Scroll.mW13_RemoveTrapSpell == 1){this.AddButton(dby, 5, 2253, 2253, 13, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 14 ) && m_Scroll.mW14_ProtectionSpell == 1){this.AddButton(dby, 5, 2254, 2254, 14, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 15 ) && m_Scroll.mW15_StrengthSpell == 1){this.AddButton(dby, 5, 2255, 2255, 15, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 16 ) && m_Scroll.mW16_BlessSpell == 1){this.AddButton(dby, 5, 2256, 2256, 16, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 17 ) && m_Scroll.mW17_FireballSpell == 1){this.AddButton(dby, 5, 2257, 2257, 17, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 18 ) && m_Scroll.mW18_MagicLockSpell == 1){this.AddButton(dby, 5, 2258, 2258, 18, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 19 ) && m_Scroll.mW19_PoisonSpell == 1){this.AddButton(dby, 5, 2259, 2259, 19, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 20 ) && m_Scroll.mW20_TelekinesisSpell == 1){this.AddButton(dby, 5, 2260, 2260, 20, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 21 ) && m_Scroll.mW21_TeleportSpell == 1){this.AddButton(dby, 5, 2261, 2261, 21, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 22 ) && m_Scroll.mW22_UnlockSpell == 1){this.AddButton(dby, 5, 2262, 2262, 22, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 23 ) && m_Scroll.mW23_WallOfStoneSpell == 1){this.AddButton(dby, 5, 2263, 2263, 23, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 24 ) && m_Scroll.mW24_ArchCureSpell == 1){this.AddButton(dby, 5, 2264, 2264, 24, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 25 ) && m_Scroll.mW25_ArchProtectionSpell == 1){this.AddButton(dby, 5, 2265, 2265, 25, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 26 ) && m_Scroll.mW26_CurseSpell == 1){this.AddButton(dby, 5, 2266, 2266, 26, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 27 ) && m_Scroll.mW27_FireFieldSpell == 1){this.AddButton(dby, 5, 2267, 2267, 27, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 28 ) && m_Scroll.mW28_GreaterHealSpell == 1){this.AddButton(dby, 5, 2268, 2268, 28, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 29 ) && m_Scroll.mW29_LightningSpell == 1){this.AddButton(dby, 5, 2269, 2269, 29, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 30 ) && m_Scroll.mW30_ManaDrainSpell == 1){this.AddButton(dby, 5, 2270, 2270, 30, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 31 ) && m_Scroll.mW31_RecallSpell == 1){this.AddButton(dby, 5, 2271, 2271, 31, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 32 ) && m_Scroll.mW32_BladeSpiritsSpell == 1){this.AddButton(dby, 5, 2272, 2272, 32, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 33 ) && m_Scroll.mW33_DispelFieldSpell == 1){this.AddButton(dby, 5, 2273, 2273, 33, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 34 ) && m_Scroll.mW34_IncognitoSpell == 1){this.AddButton(dby, 5, 2274, 2274, 34, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 35 ) && m_Scroll.mW35_MagicReflectSpell == 1){this.AddButton(dby, 5, 2275, 2275, 35, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 36 ) && m_Scroll.mW36_MindBlastSpell == 1){this.AddButton(dby, 5, 2276, 2276, 36, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 37 ) && m_Scroll.mW37_ParalyzeSpell == 1){this.AddButton(dby, 5, 2277, 2277, 37, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 38 ) && m_Scroll.mW38_PoisonFieldSpell == 1){this.AddButton(dby, 5, 2278, 2278, 38, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 39 ) && m_Scroll.mW39_SummonCreatureSpell == 1){this.AddButton(dby, 5, 2279, 2279, 39, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 40 ) && m_Scroll.mW40_DispelSpell == 1){this.AddButton(dby, 5, 2280, 2280, 40, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 41 ) && m_Scroll.mW41_EnergyBoltSpell == 1){this.AddButton(dby, 5, 2281, 2281, 41, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 42 ) && m_Scroll.mW42_ExplosionSpell == 1){this.AddButton(dby, 5, 2282, 2282, 42, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 43 ) && m_Scroll.mW43_InvisibilitySpell == 1){this.AddButton(dby, 5, 2283, 2283, 43, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 44 ) && m_Scroll.mW44_MarkSpell == 1){this.AddButton(dby, 5, 2284, 2284, 44, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 45 ) && m_Scroll.mW45_MassCurseSpell == 1){this.AddButton(dby, 5, 2285, 2285, 45, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 46 ) && m_Scroll.mW46_ParalyzeFieldSpell == 1){this.AddButton(dby, 5, 2286, 2286, 46, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 47 ) && m_Scroll.mW47_RevealSpell == 1){this.AddButton(dby, 5, 2287, 2287, 47, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 48 ) && m_Scroll.mW48_ChainLightningSpell == 1){this.AddButton(dby, 5, 2288, 2288, 48, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 49 ) && m_Scroll.mW49_EnergyFieldSpell == 1){this.AddButton(dby, 5, 2289, 2289, 49, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 50 ) && m_Scroll.mW50_FlameStrikeSpell == 1){this.AddButton(dby, 5, 2290, 2290, 50, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 51 ) && m_Scroll.mW51_GateTravelSpell == 1){this.AddButton(dby, 5, 2291, 2291, 51, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 52 ) && m_Scroll.mW52_ManaVampireSpell == 1){this.AddButton(dby, 5, 2292, 2292, 52, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 53 ) && m_Scroll.mW53_MassDispelSpell == 1){this.AddButton(dby, 5, 2293, 2293, 53, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 54 ) && m_Scroll.mW54_MeteorSwarmSpell == 1){this.AddButton(dby, 5, 2294, 2294, 54, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 55 ) && m_Scroll.mW55_PolymorphSpell == 1){this.AddButton(dby, 5, 2295, 2295, 55, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 56 ) && m_Scroll.mW56_EarthquakeSpell == 1){this.AddButton(dby, 5, 2296, 2296, 56, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 57 ) && m_Scroll.mW57_EnergyVortexSpell == 1){this.AddButton(dby, 5, 2297, 2297, 57, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 58 ) && m_Scroll.mW58_ResurrectionSpell == 1){this.AddButton(dby, 5, 2298, 2298, 58, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 59 ) && m_Scroll.mW59_AirElementalSpell == 1){this.AddButton(dby, 5, 2299, 2299, 59, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 60 ) && m_Scroll.mW60_SummonDaemonSpell == 1){this.AddButton(dby, 5, 2300, 2300, 60, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 61 ) && m_Scroll.mW61_EarthElementalSpell == 1){this.AddButton(dby, 5, 2301, 2301, 61, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 62 ) && m_Scroll.mW62_FireElementalSpell == 1){this.AddButton(dby, 5, 2302, 2302, 62, GumpButtonType.Reply, 1); dby = dby + 45;}
			if ( HasSpell( from, 63 ) && m_Scroll.mW63_WaterElementalSpell == 1){this.AddButton(dby, 5, 2303, 2303, 63, GumpButtonType.Reply, 1); dby = dby + 45;}
		}
		
		public override void OnResponse( NetState state, RelayInfo info ) 
		{ 
			Mobile from = state.Mobile; 
			switch ( info.ButtonID ) 
			{
				case 0: { break; }
				case 99: { if ( HasSpell( from, 0 ) ) { new ClumsySpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 1: { if ( HasSpell( from, 1 ) ) { new CreateFoodSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 2: { if ( HasSpell( from, 2 ) ) { new FeeblemindSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 3: { if ( HasSpell( from, 3 ) ) { new HealSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 4: { if ( HasSpell( from, 4 ) ) { new MagicArrowSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 5: { if ( HasSpell( from, 5 ) ) { new NightSightSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 6: { if ( HasSpell( from, 6 ) ) { new ReactiveArmorSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 7: { if ( HasSpell( from, 7 ) ) { new WeakenSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 8: { if ( HasSpell( from, 8 ) ) { new AgilitySpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 9: { if ( HasSpell( from, 9 ) ) { new CunningSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 10: { if ( HasSpell( from, 10 ) ) { new CureSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 11: { if ( HasSpell( from, 11 ) ) { new HarmSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 12: { if ( HasSpell( from, 12 ) ) { new MagicTrapSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 13: { if ( HasSpell( from, 13 ) ) { new RemoveTrapSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 14: { if ( HasSpell( from, 14 ) ) { new ProtectionSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 15: { if ( HasSpell( from, 15 ) ) { new StrengthSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 16: { if ( HasSpell( from, 16 ) ) { new BlessSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 17: { if ( HasSpell( from, 17 ) ) { new FireballSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 18: { if ( HasSpell( from, 18 ) ) { new MagicLockSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 19: { if ( HasSpell( from, 19 ) ) { new PoisonSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 20: { if ( HasSpell( from, 20 ) ) { new TelekinesisSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 21: { if ( HasSpell( from, 21 ) ) { new TeleportSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 22: { if ( HasSpell( from, 22 ) ) { new UnlockSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 23: { if ( HasSpell( from, 23 ) ) { new WallOfStoneSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 24: { if ( HasSpell( from, 24 ) ) { new ArchCureSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 25: { if ( HasSpell( from, 25 ) ) { new ArchProtectionSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 26: { if ( HasSpell( from, 26 ) ) { new CurseSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 27: { if ( HasSpell( from, 27 ) ) { new FireFieldSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 28: { if ( HasSpell( from, 28 ) ) { new GreaterHealSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 29: { if ( HasSpell( from, 29 ) ) { new LightningSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 30: { if ( HasSpell( from, 30 ) ) { new ManaDrainSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 31: { if ( HasSpell( from, 31 ) ) { new RecallSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 32: { if ( HasSpell( from, 32 ) ) { new BladeSpiritsSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 33: { if ( HasSpell( from, 33 ) ) { new DispelFieldSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 34: { if ( HasSpell( from, 34 ) ) { new IncognitoSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 35: { if ( HasSpell( from, 35 ) ) { new MagicReflectSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 36: { if ( HasSpell( from, 36 ) ) { new MindBlastSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 37: { if ( HasSpell( from, 37 ) ) { new ParalyzeSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 38: { if ( HasSpell( from, 38 ) ) { new PoisonFieldSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 39: { if ( HasSpell( from, 39 ) ) { new SummonCreatureSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 40: { if ( HasSpell( from, 40 ) ) { new DispelSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 41: { if ( HasSpell( from, 41 ) ) { new EnergyBoltSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 42: { if ( HasSpell( from, 42 ) ) { new ExplosionSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 43: { if ( HasSpell( from, 43 ) ) { new InvisibilitySpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 44: { if ( HasSpell( from, 44 ) ) { new MarkSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 45: { if ( HasSpell( from, 45 ) ) { new MassCurseSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 46: { if ( HasSpell( from, 46 ) ) { new ParalyzeFieldSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 47: { if ( HasSpell( from, 47 ) ) { new RevealSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 48: { if ( HasSpell( from, 48 ) ) { new ChainLightningSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 49: { if ( HasSpell( from, 49 ) ) { new EnergyFieldSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 50: { if ( HasSpell( from, 50 ) ) { new FlameStrikeSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 51: { if ( HasSpell( from, 51 ) ) { new GateTravelSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 52: { if ( HasSpell( from, 52 ) ) { new ManaVampireSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 53: { if ( HasSpell( from, 53 ) ) { new MassDispelSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 54: { if ( HasSpell( from, 54 ) ) { new MeteorSwarmSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 55: { if ( HasSpell( from, 55 ) ) { new PolymorphSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 56: { if ( HasSpell( from, 56 ) ) { new EarthquakeSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 57: { if ( HasSpell( from, 57 ) ) { new EnergyVortexSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 58: { if ( HasSpell( from, 58 ) ) { new ResurrectionSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 59: { if ( HasSpell( from, 59 ) ) { new AirElementalSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 60: { if ( HasSpell( from, 60 ) ) { new SummonDaemonSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 61: { if ( HasSpell( from, 61 ) ) { new EarthElementalSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 62: { if ( HasSpell( from, 62 ) ) { new FireElementalSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
				case 63: { if ( HasSpell( from, 63 ) ) { new WaterElementalSpell( from, null ).Cast(); from.SendGump( new Tools_tools_sorcery( from, m_Scroll ) ); } break; }
			}
		}
	}
}