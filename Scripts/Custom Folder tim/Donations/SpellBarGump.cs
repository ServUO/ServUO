/*
Original script by Djeryv: Spell toolbars. Link: http://www.runuo.com/community/threads/spell-toolbars.522805/

I have edited his original script to add Spellweaving and Mysticism, and then pack all 7 magic types into a single item/gump. Allowing players to have a "mixed bar" of spells.

In order to make this gump function correctly I used haazen's gump format ( found here: http://www.runuo.com/community/threads/calling-gumps.88093/ )
The normal gump format will revert back to Page 0 if you press a button on say Page 1. Whereas haazen's format will call the gump page that each button is coded to call.
*/



using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Commands;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Spells;
using Server.Spells.First;
using Server.Spells.Second;
using Server.Spells.Third;
using Server.Spells.Fourth;
using Server.Spells.Fifth;
using Server.Spells.Sixth;
using Server.Spells.Seventh;
using Server.Spells.Eighth;
using Server.Spells.Necromancy;
using Server.Spells.Chivalry;
using Server.Spells.Bushido;
using Server.Spells.Ninjitsu;
using Server.Spells.Spellweaving;
using Server.Spells.Mysticism;

namespace Server.Gumps
{
    public class SpellBarGump : Gump
    {
        private SpellBarScroll m_Scroll;
		
       
        Mobile caller;
        private int m_Page;
       
   

        public SpellBarGump( Mobile from, int page, SpellBarScroll scroll ) : base( 0, 0 )
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

/// NECROMANCY
            int mN01AnimateDeadSpell = m_Scroll.mN01AnimateDeadSpell;
            int mN02BloodOathSpell = m_Scroll.mN02BloodOathSpell;
            int mN03CorpseSkinSpell = m_Scroll.mN03CorpseSkinSpell;
            int mN04CurseWeaponSpell = m_Scroll.mN04CurseWeaponSpell;
            int mN05EvilOmenSpell = m_Scroll.mN05EvilOmenSpell;
            int mN06HorrificBeastSpell = m_Scroll.mN06HorrificBeastSpell;
            int mN07LichFormSpell = m_Scroll.mN07LichFormSpell;
            int mN08MindRotSpell = m_Scroll.mN08MindRotSpell;
            int mN09PainSpikeSpell = m_Scroll.mN09PainSpikeSpell;
            int mN10PoisonStrikeSpell = m_Scroll.mN10PoisonStrikeSpell;
            int mN11StrangleSpell = m_Scroll.mN11StrangleSpell;
            int mN12SummonFamiliarSpell = m_Scroll.mN12SummonFamiliarSpell;
            int mN13VampiricEmbraceSpell = m_Scroll.mN13VampiricEmbraceSpell;
            int mN14VengefulSpiritSpell = m_Scroll.mN14VengefulSpiritSpell;
            int mN15WitherSpell = m_Scroll.mN15WitherSpell;
            int mN16WraithFormSpell = m_Scroll.mN16WraithFormSpell;
            int mN17ExorcismSpell = m_Scroll.mN17ExorcismSpell;

    // CHIVALRY
            int mC01CleanseByFireSpell = m_Scroll.mC01CleanseByFireSpell;
            int mC02CloseWoundsSpell = m_Scroll.mC02CloseWoundsSpell;
            int mC03ConsecrateWeaponSpell = m_Scroll.mC03ConsecrateWeaponSpell;
            int mC04DispelEvilSpell = m_Scroll.mC04DispelEvilSpell;
            int mC05DivineFurySpell = m_Scroll.mC05DivineFurySpell;
            int mC06EnemyOfOneSpell = m_Scroll.mC06EnemyOfOneSpell;
            int mC07HolyLightSpell = m_Scroll.mC07HolyLightSpell;
            int mC08NobleSacrificeSpell = m_Scroll.mC08NobleSacrificeSpell;
            int mC09RemoveCurseSpell = m_Scroll.mC09RemoveCurseSpell;
            int mC10SacredJourneySpell = m_Scroll.mC10SacredJourneySpell;

// BUSHIDO
			int mB01Confidence = m_Scroll.mB01Confidence;
			int mB02CounterAttack = m_Scroll.mB02CounterAttack;
			int mB03Evasion = m_Scroll.mB03Evasion;
			int mB04LightningStrike = m_Scroll.mB04LightningStrike;
			int mB05MomentumStrike = m_Scroll.mB05MomentumStrike;
			int mB06HonorableExecution = m_Scroll.mB06HonorableExecution;

// NINJITSU
            int mI01DeathStrike = m_Scroll.mI01DeathStrike;
            int mI02AnimalForm = m_Scroll.mI02AnimalForm;
            int mI03KiAttack = m_Scroll.mI03KiAttack;
            int mI04SurpriseAttack = m_Scroll.mI04SurpriseAttack;
            int mI05Backstab = m_Scroll.mI05Backstab;
			//
            int mI06Shadowjump = m_Scroll.mI06Shadowjump; //changed to 06
            int mI07MirrorImage = m_Scroll.mI07MirrorImage; // to 07
			int mI08FocusAttack = m_Scroll.mI08FocusAttack; // to 08

// SPELLWEAVING
            int mS01ArcaneCircleSpell = m_Scroll.mS01ArcaneCircleSpell;
            int mS02GiftOfRenewalSpell = m_Scroll.mS02GiftOfRenewalSpell;
            int mS03ImmolatingWeaponSpell = m_Scroll.mS03ImmolatingWeaponSpell;
            int mS04AttuneWeaponSpell = m_Scroll.mS04AttuneWeaponSpell;
            int mS05ThunderstormSpell = m_Scroll.mS05ThunderstormSpell;
            int mS06NatureFurySpell = m_Scroll.mS06NatureFurySpell;
            int mS07SummonFeySpell = m_Scroll.mS07SummonFeySpell;
            int mS08SummonFiendSpell = m_Scroll.mS08SummonFiendSpell;
            int mS09ReaperFormSpell = m_Scroll.mS09ReaperFormSpell;
            int mS10WildfireSpell = m_Scroll.mS10WildfireSpell;
            int mS11EssenceOfWindSpell = m_Scroll.mS11EssenceOfWindSpell;
            int mS12DryadAllureSpell = m_Scroll.mS12DryadAllureSpell;
            int mS13EtherealVoyageSpell = m_Scroll.mS13EtherealVoyageSpell;
            int mS14WordOfDeathSpell = m_Scroll.mS14WordOfDeathSpell;
            int mS15GiftOfLifeSpell = m_Scroll.mS15GiftOfLifeSpell;
            int mS16ArcaneEmpowermentSpell = m_Scroll.mS16ArcaneEmpowermentSpell;

// MYSTICISM
			int mM01NetherBoltSpell = m_Scroll.mM01NetherBoltSpell;
			int mM02HealingStoneSpell = m_Scroll.mM02HealingStoneSpell;
			int mM03PurgeMagicSpell = m_Scroll.mM03PurgeMagicSpell;
			int mM04EnchantSpell = m_Scroll.mM04EnchantSpell;
			int mM05SleepSpell = m_Scroll.mM05SleepSpell;
			int mM06EagleStrikeSpell = m_Scroll.mM06EagleStrikeSpell;
			int mM07AnimatedWeaponSpell = m_Scroll.mM07AnimatedWeaponSpell;
			int mM08SpellTriggerSpell = m_Scroll.mM08SpellTriggerSpell;
			int mM09MassSleepSpell = m_Scroll.mM09MassSleepSpell;
			int mM10CleansingWindsSpell = m_Scroll.mM10CleansingWindsSpell;
			int mM11BombardSpell = m_Scroll.mM11BombardSpell;
			int mM12SpellPlagueSpell = m_Scroll.mM12SpellPlagueSpell;
			int mM13HailStormSpell = m_Scroll.mM13HailStormSpell;
			int mM14NetherCycloneSpell = m_Scroll.mM14NetherCycloneSpell;
			int mM15RisingColossusSpell = m_Scroll.mM15RisingColossusSpell;
			int mM16StoneFormSpell = m_Scroll.mM16StoneFormSpell;
			
			int mSwitch = m_Scroll.mSwitch;
			int mCount = m_Scroll.mCount;
			int mXselect_10 = m_Scroll.mXselect_10;
			int mXselect_15 = m_Scroll.mXselect_15;
			int mXselect_20 = m_Scroll.mXselect_20;
			int mXselect_30 = m_Scroll.mXselect_30;
			
		//	int xselect_var = 0;
			
			int mYselect_1 = m_Scroll.mYselect_1;
			int mYselect_2 = m_Scroll.mYselect_2;
			int mYselect_3 = m_Scroll.mYselect_3;
			int mYselect_4 = m_Scroll.mYselect_4;
			//int yselect_var = 0;
			//int mYselect_var = m_Scroll.mYselect_var;
			
			//xselect_var = mXselect_var;
			
			
			int yselect_var = 0; //xselect_var = 0;
			int xselect_num = 0;
			if ( m_Scroll.mXselect_10 == 1) { xselect_num = 10; } 
			if ( m_Scroll.mXselect_15 == 1) { xselect_num = 15; } 
			if ( m_Scroll.mXselect_20 == 1) { xselect_num = 20; } 
			if ( m_Scroll.mXselect_30 == 1) { xselect_num = 30; }
			if ( m_Scroll.mYselect_1 == 1) { yselect_var = 1; }
			if ( m_Scroll.mYselect_2 == 1) { yselect_var = 2; }
			if ( m_Scroll.mYselect_3 == 1) { yselect_var = 3; }
			if ( m_Scroll.mYselect_4 == 1) { yselect_var = 4; }
			

            m_Page = page;
            caller = from;
            this.Closable=true;
            this.Disposable=false;
            this.Dragable=true;
            this.Resizable=false;
           
            from.CloseGump(typeof(SpellBarGump));
			
			
            AddPage(0);
           
            //AddBackground(170, 0, 570, 500, 9200);
            //AddImageTiled(180, 10, 550, 480, 2624);
            //AddAlphaRegion(180, 10, 550, 480);
            AddBackground(0, 0, 165, 500, 9200);
            AddImageTiled(10, 10, 145, 480, 2624);
            AddAlphaRegion(10, 10, 145, 480);
/// begin magic selection buttons        
            AddButton(12, 20, 4005, 4006, (int)Buttons.Button1, GumpButtonType.Reply, 0);//magery
            AddButton(12, 50, 4005, 4006, (int)Buttons.Button2, GumpButtonType.Reply, 0);//necro
            AddButton(12, 80, 4005, 4006, (int)Buttons.Button3, GumpButtonType.Reply, 0);//chiv
            AddButton(12, 110, 4005, 4006, (int)Buttons.Button4, GumpButtonType.Reply, 0);//nin
            AddButton(12, 140, 4005, 4006, (int)Buttons.Button5, GumpButtonType.Reply, 0);//bush
            AddButton(12, 170, 4005, 4006, (int)Buttons.Button6, GumpButtonType.Reply, 0);//weave
            AddButton(12, 200, 4005, 4006, (int)Buttons.Button7, GumpButtonType.Reply, 0);//myst
            AddLabel(50, 20, 1153, @"Magery");
            AddLabel(50, 50, 1153, @"Necromancy");
            AddLabel(50, 80, 1153, @"Chivalry");
            AddLabel(50, 110, 1153, @"Bushido");
            AddLabel(50, 140, 1153, @"Ninjitsu");
            AddLabel(50, 170, 1153, @"Spellweaving");
            AddLabel(50, 200, 1153, @"Mysticism");
/// end magic selection

			AddButton(12, 230, 4011, 4012, (int)Buttons.Button148, GumpButtonType.Reply, 0); // options
            AddLabel(50, 230, 1153, @"Options Menu");
   
            AddButton(15, 275, 2152, 2152, (int)Buttons.Button8, GumpButtonType.Reply, 1); // TOOLBAR
            AddLabel(50, 278, 52, @"Open Toolbar");
			
			AddButton(16, 420, 2033, 2032, (int)Buttons.Button146, GumpButtonType.Reply, 1); // help
			
            AddButton( 19, 450, 2453, 2454, (int)Buttons.Button0, GumpButtonType.Reply, 1); // Cancel
           
		    AddLabel( 25, 336, 1153, "You have selected" );
			AddLabel( 35, 356, 1153, String.Format("{0} of {1} spells", mCount, xselect_num * yselect_var ) );
			
			//AddButton(12, 457, 4020, 4021, (int)Buttons.Button147, GumpButtonType.Reply, 0);//reset
		//	AddLabel(50, 457, 1153, @"Reset selection");
			
           
            switch (page)
            {
/////////////////////////////////////
/// each case represents a "page"///
////////////////////////////////////
                case 1: /// magery
                    {
						AddBackground(170, 0, 570, 500, 9200);
						AddImageTiled(180, 10, 550, 480, 2624);
						AddAlphaRegion(180, 10, 550, 480);
					
					
                        AddLabel(205, 13, 1153, @"Clumsy");
                        AddLabel(205, 43, 1153, @"Create Food");
                        AddLabel(205, 73, 1153, @"Feeblemind");
                        AddLabel(205, 103, 1153, @"Heal");
                        AddLabel(205, 133, 1153, @"Magic Arrow");
                        AddLabel(205, 163, 1153, @"Night Sight");
                        AddLabel(205, 193, 1153, @"Reactive Armor");
                        AddLabel(205, 223, 1153, @"Weaken");
                        //
                        AddLabel(205, 253, 1153, @"Agility");
                        AddLabel(205, 283, 1153, @"Cunning");
                        AddLabel(205, 313, 1153, @"Cure");
                        AddLabel(205, 343, 1153, @"Harm");
                        AddLabel(205, 373, 1153, @"Magic Trap");
                        AddLabel(205, 403, 1153, @"Remove Trap");
                        AddLabel(205, 434, 1153, @"Protection");
                        AddLabel(205, 463, 1153, @"Strength");
            /// row 2
                        AddLabel(336, 13, 1153, @"Bless");
                        AddLabel(336, 43, 1153, @"Fireball");
                        AddLabel(336, 73, 1153, @"Magic Lock");
                        AddLabel(336, 103, 1153, @"Poison");
                        AddLabel(336, 133, 1153, @"Telekinesis");
                        AddLabel(336, 163, 1153, @"Teleport");
                        AddLabel(336, 193, 1153, @"Unlock");
                        AddLabel(336, 223, 1153, @"Wall of Stone");
                        AddLabel(336, 253, 1153, @"Arch Cure");
                        AddLabel(336, 283, 1153, @"Arch Protect");
                        AddLabel(336, 313, 1153, @"Curse");
                        AddLabel(336, 343, 1153, @"Fire Field");
                        AddLabel(336, 373, 1153, @"Greater Heal");
                        AddLabel(336, 403, 1153, @"Lightning");
                        AddLabel(336, 434, 1153, @"Mana Drain");
                        AddLabel(336, 463, 1153, @"Recall");
            ///row 3
                        AddLabel(465, 13, 1153, @"Blade Spirit");
                        AddLabel(465, 43, 1153, @"Dispel Field");
                        AddLabel(465, 73, 1153, @"Incognito");
                        AddLabel(465, 103, 1153, @"Magic Reflection");
                        AddLabel(465, 133, 1153, @"Mind Blast");
                        AddLabel(465, 163, 1153, @"Paralyze");
                        AddLabel(465, 193, 1153, @"Poison Field");
                        AddLabel(465, 223, 1153, @"Summ. Creature");
                        AddLabel(465, 253, 1153, @"Dispel");
                        AddLabel(465, 283, 1153, @"Energy Bolt");
                        AddLabel(465, 313, 1153, @"Explosion");
                        AddLabel(465, 343, 1153, @"Invisibility");
                        AddLabel(465, 373, 1153, @"Mark");
                        AddLabel(465, 403, 1153, @"Mass Curse");
                        AddLabel(465, 434, 1153, @"Paralyze Field");
                        AddLabel(465, 463, 1153, @"Reveal");
            ///row 4
                        AddLabel(597, 12, 1153, @"Chain Lightning");
                        AddLabel(597, 42, 1153, @"Energy Field");
                        AddLabel(597, 72, 1153, @"Flame Strike");
                        AddLabel(597, 102, 1153, @"Gate Travel");
                        AddLabel(597, 132, 1153, @"Mana Vampire");
                        AddLabel(597, 162, 1153, @"Mass Dispel");
                        AddLabel(597, 192, 1153, @"Meteor Swarm");
                        AddLabel(597, 222, 1153, @"Polymorph");
                        AddLabel(597, 252, 1153, @"Earthquake");
                        AddLabel(597, 282, 1153, @"Energy Vortex");
                        AddLabel(597, 312, 1153, @"Resurrection");
                        AddLabel(597, 342, 1153, @"Air Elemental");
                        AddLabel(597, 372, 1153, @"Summon Daemon");
                        AddLabel(597, 402, 1153, @"Earth Elemental");
                        AddLabel(597, 433, 1153, @"Fire Elemental");
                        AddLabel(597, 462, 1153, @"Water Elemental");
                       
                       
                        AddButton(12, 20, 4006, 4006, (int)Buttons.Button1, GumpButtonType.Reply, 0);//magery selected

           
                        if ( mW00_ClumsySpell == 1 ) {AddButton(190, 20, 2361, 2360, (int)Buttons.Button9, GumpButtonType.Reply, 1); }//clumsy
                        if ( mW00_ClumsySpell == 0 ) {AddButton(190, 20, 2360, 2361, (int)Buttons.Button9, GumpButtonType.Reply, 1);}//clumsy
                       
                        if ( mW01_CreateFoodSpell == 1 ) { this.AddButton(190, 50, 2361, 2361, (int)Buttons.Button10, GumpButtonType.Reply, 1); }
                        if ( mW01_CreateFoodSpell == 0 ) { this.AddButton(190, 50, 2360, 2360, (int)Buttons.Button10, GumpButtonType.Reply, 1); }
                       
                        if ( mW02_FeeblemindSpell == 1 ) { this.AddButton(190, 80, 2361, 2361, (int)Buttons.Button11, GumpButtonType.Reply, 1); }
                        if ( mW02_FeeblemindSpell == 0 ) { this.AddButton(190, 80, 2360, 2360, (int)Buttons.Button11, GumpButtonType.Reply, 1); }
                       
                        if ( mW03_HealSpell == 1 ) { this.AddButton(190, 110, 2361, 2361, (int)Buttons.Button12, GumpButtonType.Reply, 1); }
                        if ( mW03_HealSpell == 0 ) { this.AddButton(190, 110, 2360, 2360, (int)Buttons.Button12, GumpButtonType.Reply, 1); }
                       
                        if ( mW04_MagicArrowSpell == 1 ) { this.AddButton(190, 140, 2361, 2361, (int)Buttons.Button13, GumpButtonType.Reply, 1); }
                        if ( mW04_MagicArrowSpell == 0 ) { this.AddButton(190, 140, 2360, 2360, (int)Buttons.Button13, GumpButtonType.Reply, 1); }
                       
                        if ( mW05_NightSightSpell == 1 ) { this.AddButton(190, 170, 2361, 2361, (int)Buttons.Button14, GumpButtonType.Reply, 1); }
                        if ( mW05_NightSightSpell == 0 ) { this.AddButton(190, 170, 2360, 2360, (int)Buttons.Button14, GumpButtonType.Reply, 1); }
                       
                        if ( mW06_ReactiveArmorSpell == 1 ) { this.AddButton(190, 200, 2361, 2361, (int)Buttons.Button15, GumpButtonType.Reply, 1); }
                        if ( mW06_ReactiveArmorSpell == 0 ) { this.AddButton(190, 200, 2360, 2360, (int)Buttons.Button15, GumpButtonType.Reply, 1); }
                       
                        if ( mW07_WeakenSpell == 1 ) { this.AddButton(190, 230, 2361, 2361, (int)Buttons.Button16, GumpButtonType.Reply, 1); }
                        if ( mW07_WeakenSpell == 0 ) { this.AddButton(190, 230, 2360, 2360, (int)Buttons.Button16, GumpButtonType.Reply, 1); }
       
                        if ( mW08_AgilitySpell == 1 ) { this.AddButton(190, 260,  2361, 2361, (int)Buttons.Button17, GumpButtonType.Reply, 1); }
                        if ( mW08_AgilitySpell == 0 ) { this.AddButton(190, 260,  2360, 2360, (int)Buttons.Button17, GumpButtonType.Reply, 1); }
                       
                        if ( mW09_CunningSpell == 1 ) { this.AddButton(190, 290, 2361, 2361, (int)Buttons.Button18, GumpButtonType.Reply, 1); }
                        if ( mW09_CunningSpell == 0 ) { this.AddButton(190, 290, 2360, 2360, (int)Buttons.Button18, GumpButtonType.Reply, 1); }
                       
                        if ( mW10_CureSpell == 1 ) { this.AddButton(190, 320, 2361, 2361, (int)Buttons.Button19, GumpButtonType.Reply, 1); }
                        if ( mW10_CureSpell == 0 ) { this.AddButton(190, 320, 2360, 2360, (int)Buttons.Button19, GumpButtonType.Reply, 1); }
                       
                        if ( mW11_HarmSpell == 1 ) { this.AddButton(190, 350, 2361, 2361, (int)Buttons.Button20, GumpButtonType.Reply, 1); }
                        if ( mW11_HarmSpell == 0 ) { this.AddButton(190, 350, 2360, 2360, (int)Buttons.Button20, GumpButtonType.Reply, 1); }
                       
                        if ( mW12_MagicTrapSpell == 1 ) { this.AddButton(190, 380, 2361, 2361, (int)Buttons.Button21, GumpButtonType.Reply, 1); }
                        if ( mW12_MagicTrapSpell == 0 ) { this.AddButton(190, 380, 2360, 2360, (int)Buttons.Button21, GumpButtonType.Reply, 1); }
                       
                        if ( mW13_RemoveTrapSpell == 1 ) { this.AddButton(190, 410, 2361, 2361, (int)Buttons.Button22, GumpButtonType.Reply, 1); }
                        if ( mW13_RemoveTrapSpell == 0 ) { this.AddButton(190, 410, 2360, 2360, (int)Buttons.Button22, GumpButtonType.Reply, 1); }
                       
                        if ( mW14_ProtectionSpell == 1 ) { this.AddButton(190, 440, 2361, 2361, (int)Buttons.Button23, GumpButtonType.Reply, 1); }
                        if ( mW14_ProtectionSpell == 0 ) { this.AddButton(190, 440, 2360, 2360, (int)Buttons.Button23, GumpButtonType.Reply, 1); }
                       
                        if ( mW15_StrengthSpell == 1 ) { this.AddButton(190, 470, 2361, 2361, (int)Buttons.Button24, GumpButtonType.Reply, 1); }
                        if ( mW15_StrengthSpell == 0 ) { this.AddButton(190, 470, 2360, 2360, (int)Buttons.Button24, GumpButtonType.Reply, 1); }    
            ///row 2
                        if ( mW16_BlessSpell == 1 ) { this.AddButton(321, 20,  2361, 2361,  (int)Buttons.Button25, GumpButtonType.Reply, 1); }
                        if ( mW16_BlessSpell == 0 ) { this.AddButton(321, 20,  2360, 2360,  (int)Buttons.Button25, GumpButtonType.Reply, 1); }
                       
                        if ( mW17_FireballSpell == 1 ) { this.AddButton(321, 50, 2361, 2361,  (int)Buttons.Button26, GumpButtonType.Reply, 1); }
                        if ( mW17_FireballSpell == 0 ) { this.AddButton(321, 50, 2360, 2360,  (int)Buttons.Button26, GumpButtonType.Reply, 1); }
                       
                        if ( mW18_MagicLockSpell == 1 ) { this.AddButton(321, 80, 2361, 2361,  (int)Buttons.Button27, GumpButtonType.Reply, 1); }
                        if ( mW18_MagicLockSpell == 0 ) { this.AddButton(321, 80, 2360, 2360,  (int)Buttons.Button27, GumpButtonType.Reply, 1); }
                       
                        if ( mW19_PoisonSpell == 1 ) { this.AddButton(321, 110, 2361, 2361,  (int)Buttons.Button28, GumpButtonType.Reply, 1); }
                        if ( mW19_PoisonSpell == 0 ) { this.AddButton(321, 110, 2360, 2360,  (int)Buttons.Button28, GumpButtonType.Reply, 1); }
                       
                        if ( mW20_TelekinesisSpell == 1 ) { this.AddButton(321, 140, 2361, 2361,  (int)Buttons.Button29, GumpButtonType.Reply, 1); }
                        if ( mW20_TelekinesisSpell == 0 ) { this.AddButton(321, 140, 2360, 2360,  (int)Buttons.Button29, GumpButtonType.Reply, 1); }

                        if ( mW21_TeleportSpell == 1 ) { this.AddButton(321, 170, 2361, 2361,  (int)Buttons.Button30, GumpButtonType.Reply, 1); }
                        if ( mW21_TeleportSpell == 0 ) { this.AddButton(321, 170, 2360, 2360,   (int)Buttons.Button30, GumpButtonType.Reply, 1); }
                       
                        if ( mW22_UnlockSpell == 1 ) { this.AddButton(321, 200, 2361, 2361,   (int)Buttons.Button31, GumpButtonType.Reply, 1); }
                        if ( mW22_UnlockSpell == 0 ) { this.AddButton(321, 200, 2360, 2360,   (int)Buttons.Button31, GumpButtonType.Reply, 1); }
                       
                        if ( mW23_WallOfStoneSpell == 1 ) { this.AddButton(321, 230, 2361, 2361,   (int)Buttons.Button32, GumpButtonType.Reply, 1); }
                        if ( mW23_WallOfStoneSpell == 0 ) { this.AddButton(321, 230, 2360, 2360,   (int)Buttons.Button32, GumpButtonType.Reply, 1); }
                       
                        if ( mW24_ArchCureSpell == 1 ) { this.AddButton(321, 260,  2361, 2361,   (int)Buttons.Button33, GumpButtonType.Reply, 1); }
                        if ( mW24_ArchCureSpell == 0 ) { this.AddButton(321, 260,  2360, 2360,   (int)Buttons.Button33, GumpButtonType.Reply, 1); }
                       
                        if ( mW25_ArchProtectionSpell == 1 ) { this.AddButton(321, 290, 2361, 2361,   (int)Buttons.Button34, GumpButtonType.Reply, 1); }
                        if ( mW25_ArchProtectionSpell == 0 ) { this.AddButton(321, 290, 2360, 2360,   (int)Buttons.Button34, GumpButtonType.Reply, 1); }
                       
                        if ( mW26_CurseSpell == 1 ) { this.AddButton(321, 320, 2361, 2361,   (int)Buttons.Button35, GumpButtonType.Reply, 1); }
                        if ( mW26_CurseSpell == 0 ) { this.AddButton(321, 320, 2360, 2360,   (int)Buttons.Button35, GumpButtonType.Reply, 1); }
                       
                        if ( mW27_FireFieldSpell == 1 ) { this.AddButton(321, 350, 2361, 2361,   (int)Buttons.Button36, GumpButtonType.Reply, 1); }
                        if ( mW27_FireFieldSpell == 0 ) { this.AddButton(321, 350, 2360, 2360,   (int)Buttons.Button36, GumpButtonType.Reply, 1); }
                       
                        if ( mW28_GreaterHealSpell == 1 ) { this.AddButton(321, 380, 2361, 2361,   (int)Buttons.Button37, GumpButtonType.Reply, 1); }
                        if ( mW28_GreaterHealSpell == 0 ) { this.AddButton(321, 380, 2360, 2360,   (int)Buttons.Button37, GumpButtonType.Reply, 1); }
                       
                        if ( mW29_LightningSpell == 1 ) { this.AddButton(321, 410, 2361, 2361,   (int)Buttons.Button38, GumpButtonType.Reply, 1); }
                        if ( mW29_LightningSpell == 0 ) { this.AddButton(321, 410, 2360, 2360,   (int)Buttons.Button38, GumpButtonType.Reply, 1); }
                       
                        if ( mW30_ManaDrainSpell == 1 ) { this.AddButton(321, 440, 2361, 2361,   (int)Buttons.Button39, GumpButtonType.Reply, 1); }
                        if ( mW30_ManaDrainSpell == 0 ) { this.AddButton(321, 440, 2360, 2360,   (int)Buttons.Button39, GumpButtonType.Reply, 1); }
                       
                        if ( mW31_RecallSpell == 1 ) { this.AddButton(321, 470, 2361, 2361, (int)Buttons.Button40, GumpButtonType.Reply, 1); }
                        if ( mW31_RecallSpell == 0 ) { this.AddButton(321, 470, 2360, 2360,   (int)Buttons.Button40, GumpButtonType.Reply, 1); }
                       
                       
            /// 3RD ROW            
                        if ( mW32_BladeSpiritsSpell == 1 ) { this.AddButton(450, 20,  2361, 2361, (int)Buttons.Button41, GumpButtonType.Reply, 1); }
                        if ( mW32_BladeSpiritsSpell == 0 ) { this.AddButton(450, 20,  2360, 2360,  (int)Buttons.Button41, GumpButtonType.Reply, 1); }
                       
                        if ( mW33_DispelFieldSpell == 1 ) { this.AddButton(450, 50,  2361, 2361,  (int)Buttons.Button42, GumpButtonType.Reply, 1); }
                        if ( mW33_DispelFieldSpell == 0 ) { this.AddButton(450, 50,  2360, 2360,  (int)Buttons.Button42, GumpButtonType.Reply, 1); }
                       
                        if ( mW34_IncognitoSpell == 1 ) { this.AddButton(450, 80, 2361, 2361,  (int)Buttons.Button43, GumpButtonType.Reply, 1); }
                        if ( mW34_IncognitoSpell == 0 ) { this.AddButton(450, 80, 2360, 2360,  (int)Buttons.Button43, GumpButtonType.Reply, 1); }
                       
                        if ( mW35_MagicReflectSpell == 1 ) { this.AddButton(450, 110, 2361, 2361,  (int)Buttons.Button44, GumpButtonType.Reply, 1); }
                        if ( mW35_MagicReflectSpell == 0 ) { this.AddButton(450, 110, 2360, 2360,  (int)Buttons.Button44, GumpButtonType.Reply, 1); }
                       
                        if ( mW36_MindBlastSpell == 1 ) { this.AddButton(450, 140, 2361, 2361,  (int)Buttons.Button45, GumpButtonType.Reply, 1); }
                        if ( mW36_MindBlastSpell == 0 ) { this.AddButton(450, 140, 2360, 2360,  (int)Buttons.Button45, GumpButtonType.Reply, 1); }
                       
                        if ( mW37_ParalyzeSpell == 1 ) { this.AddButton(450, 170, 2361, 2361,  (int)Buttons.Button46, GumpButtonType.Reply, 1); }
                        if ( mW37_ParalyzeSpell == 0 ) { this.AddButton(450, 170, 2360, 2360,  (int)Buttons.Button46, GumpButtonType.Reply, 1); }
                       
                        if ( mW38_PoisonFieldSpell == 1 ) { this.AddButton(450, 200, 2361, 2361,  (int)Buttons.Button47, GumpButtonType.Reply, 1); }
                        if ( mW38_PoisonFieldSpell == 0 ) { this.AddButton(450, 200, 2360, 2360,  (int)Buttons.Button47, GumpButtonType.Reply, 1); }
                       
                        if ( mW39_SummonCreatureSpell == 1 ) { this.AddButton(450, 230, 2361, 2361,  (int)Buttons.Button48, GumpButtonType.Reply, 1); }
                        if ( mW39_SummonCreatureSpell == 0 ) { this.AddButton(450, 230, 2360, 2360,  (int)Buttons.Button48, GumpButtonType.Reply, 1); }
                       
                        if ( mW40_DispelSpell == 1 ) { this.AddButton(450, 260,  2361, 2361,  (int)Buttons.Button49, GumpButtonType.Reply, 1); }
                        if ( mW40_DispelSpell == 0 ) { this.AddButton(450, 260,  2360, 2360,  (int)Buttons.Button49, GumpButtonType.Reply, 1); }
                       
                        if ( mW41_EnergyBoltSpell == 1 ) { this.AddButton(450, 290,  2361, 2361, (int)Buttons.Button50, GumpButtonType.Reply, 1); }
                        if ( mW41_EnergyBoltSpell == 0 ) { this.AddButton(450, 290,  2360, 2360,  (int)Buttons.Button50, GumpButtonType.Reply, 1); }
                       
                        if ( mW42_ExplosionSpell == 1 ) { this.AddButton(450, 320, 2361, 2361,  (int)Buttons.Button51, GumpButtonType.Reply, 1); }
                        if ( mW42_ExplosionSpell == 0 ) { this.AddButton(450, 320, 2360, 2360,  (int)Buttons.Button51, GumpButtonType.Reply, 1); }
                       
                        if ( mW43_InvisibilitySpell == 1 ) { this.AddButton(450, 350, 2361, 2361,  (int)Buttons.Button52, GumpButtonType.Reply, 1); }
                        if ( mW43_InvisibilitySpell == 0 ) { this.AddButton(450, 350, 2360, 2360,  (int)Buttons.Button52, GumpButtonType.Reply, 1); }
                       
                        if ( mW44_MarkSpell == 1 ) { this.AddButton(450, 380, 2361, 2361,  (int)Buttons.Button53, GumpButtonType.Reply, 1); }
                        if ( mW44_MarkSpell == 0 ) { this.AddButton(450, 380, 2360, 2360,  (int)Buttons.Button53, GumpButtonType.Reply, 1); }
                       
                        if ( mW45_MassCurseSpell == 1 ) { this.AddButton(450, 410, 2361, 2361,  (int)Buttons.Button54, GumpButtonType.Reply, 1); }
                        if ( mW45_MassCurseSpell == 0 ) { this.AddButton(450, 410, 2360, 2360,  (int)Buttons.Button54, GumpButtonType.Reply, 1); }
                       
                        if ( mW46_ParalyzeFieldSpell == 1 ) { this.AddButton(450, 440, 2361, 2361,  (int)Buttons.Button55, GumpButtonType.Reply, 1); }
                        if ( mW46_ParalyzeFieldSpell == 0 ) { this.AddButton(450, 440, 2360, 2360,  (int)Buttons.Button55, GumpButtonType.Reply, 1); }
                       
                        if ( mW47_RevealSpell == 1 ) { this.AddButton(450, 470, 2361, 2361,  (int)Buttons.Button56, GumpButtonType.Reply, 1); }
                        if ( mW47_RevealSpell == 0 ) { this.AddButton(450, 470, 2360, 2360,  (int)Buttons.Button56, GumpButtonType.Reply, 1); }
                       
        ///4th row                
                        if ( mW48_ChainLightningSpell == 1 ) { this.AddButton(582, 20,  2361, 2361, (int)Buttons.Button57, GumpButtonType.Reply, 1); }
                        if ( mW48_ChainLightningSpell == 0 ) { this.AddButton(582, 20,  2360, 2360, (int)Buttons.Button57, GumpButtonType.Reply, 1); }
                       
                        if ( mW49_EnergyFieldSpell == 1 ) { this.AddButton(582, 50,  2361, 2361, (int)Buttons.Button58, GumpButtonType.Reply, 1); }
                        if ( mW49_EnergyFieldSpell == 0 ) { this.AddButton(582, 50,  2360, 2360, (int)Buttons.Button58, GumpButtonType.Reply, 1); }
                       
                        if ( mW50_FlameStrikeSpell == 1 ) { this.AddButton(582, 80, 2361, 2361, (int)Buttons.Button59, GumpButtonType.Reply, 1); }
                        if ( mW50_FlameStrikeSpell == 0 ) { this.AddButton(582, 80, 2360, 2360, (int)Buttons.Button59, GumpButtonType.Reply, 1); }
                       
                        if ( mW51_GateTravelSpell == 1 ) { this.AddButton(582, 110, 2361, 2361, (int)Buttons.Button60, GumpButtonType.Reply, 1); }
                        if ( mW51_GateTravelSpell == 0 ) { this.AddButton(582, 110, 2360, 2360, (int)Buttons.Button60, GumpButtonType.Reply, 1); }
                       
                        if ( mW52_ManaVampireSpell == 1 ) { this.AddButton(582, 140, 2361, 2361, (int)Buttons.Button61, GumpButtonType.Reply, 1); }
                        if ( mW52_ManaVampireSpell == 0 ) { this.AddButton(582, 140, 2360, 2360, (int)Buttons.Button61, GumpButtonType.Reply, 1); }
                       
                        if ( mW53_MassDispelSpell == 1 ) { this.AddButton(582, 170, 2361, 2361, (int)Buttons.Button62, GumpButtonType.Reply, 1); }
                        if ( mW53_MassDispelSpell == 0 ) { this.AddButton(582, 170, 2360, 2360, (int)Buttons.Button62, GumpButtonType.Reply, 1); }
                       
                        if ( mW54_MeteorSwarmSpell == 1 ) { this.AddButton(582, 200, 2361, 2361, (int)Buttons.Button63, GumpButtonType.Reply, 1); }
                        if ( mW54_MeteorSwarmSpell == 0 ) { this.AddButton(582, 200, 2360, 2360, (int)Buttons.Button63, GumpButtonType.Reply, 1); }
                       
                        if ( mW55_PolymorphSpell == 1 ) { this.AddButton(582, 230, 2361, 2361, (int)Buttons.Button64, GumpButtonType.Reply, 1); }
                        if ( mW55_PolymorphSpell == 0 ) { this.AddButton(582, 230, 2360, 2360, (int)Buttons.Button64, GumpButtonType.Reply, 1); }
                       
                        if ( mW56_EarthquakeSpell == 1 ) { this.AddButton(582, 260,  2361, 2361, (int)Buttons.Button65, GumpButtonType.Reply, 1); }
                        if ( mW56_EarthquakeSpell == 0 ) { this.AddButton(582, 260,  2360, 2360, (int)Buttons.Button65, GumpButtonType.Reply, 1); }
                       
                        if ( mW57_EnergyVortexSpell == 1 ) { this.AddButton(582, 290,  2361, 2361, (int)Buttons.Button66, GumpButtonType.Reply, 1); }
                        if ( mW57_EnergyVortexSpell == 0 ) { this.AddButton(582, 290,  2360, 2360, (int)Buttons.Button66, GumpButtonType.Reply, 1); }
                       
                        if ( mW58_ResurrectionSpell == 1 ) { this.AddButton(582, 320, 2361, 2361, (int)Buttons.Button67, GumpButtonType.Reply, 1); }
                        if ( mW58_ResurrectionSpell == 0 ) { this.AddButton(582, 320, 2360, 2360, (int)Buttons.Button67, GumpButtonType.Reply, 1); }
                       
                        if ( mW59_AirElementalSpell == 1 ) { this.AddButton(582, 350, 2361, 2361, (int)Buttons.Button68, GumpButtonType.Reply, 1); }
                        if ( mW59_AirElementalSpell == 0 ) { this.AddButton(582, 350, 2360, 2360, (int)Buttons.Button68, GumpButtonType.Reply, 1); }
                       
                        if ( mW60_SummonDaemonSpell == 1 ) { this.AddButton(582, 380, 2361, 2361, (int)Buttons.Button69, GumpButtonType.Reply, 1); }
                        if ( mW60_SummonDaemonSpell == 0 ) { this.AddButton(582, 380, 2360, 2360, (int)Buttons.Button69, GumpButtonType.Reply, 1); }
                       
                        if ( mW61_EarthElementalSpell == 1 ) { this.AddButton(582, 410, 2361, 2361, (int)Buttons.Button70, GumpButtonType.Reply, 1); }
                        if ( mW61_EarthElementalSpell == 0 ) { this.AddButton(582, 410, 2360, 2360, (int)Buttons.Button70, GumpButtonType.Reply, 1); }
                       
                        if ( mW62_FireElementalSpell == 1 ) { this.AddButton(582, 440, 2361, 2361, (int)Buttons.Button71, GumpButtonType.Reply, 1); }
                        if ( mW62_FireElementalSpell == 0 ) { this.AddButton(582, 440, 2360, 2360, (int)Buttons.Button71, GumpButtonType.Reply, 1); }
                       
                        if ( mW63_WaterElementalSpell == 1 ) { this.AddButton(582, 470, 2361, 2361, (int)Buttons.Button72, GumpButtonType.Reply, 1); }
                        if ( mW63_WaterElementalSpell == 0 ) { this.AddButton(582, 470, 2360, 2360, (int)Buttons.Button72, GumpButtonType.Reply, 1); }
           
                           
                        break;
                    }
                case 2: /// necromancy
                    {
						AddBackground(170, 0, 300, 500, 9200);
						AddImageTiled(180, 10, 280, 480, 2624);
						AddAlphaRegion(180, 10, 280, 480);
					
                        AddLabel(205, 13, 1153, @"Animated Dead");
                        AddLabel(205, 43, 1153, @"Blood Oath");
                        AddLabel(205, 73, 1153, @"Corpse Skin");
                        AddLabel(205, 103, 1153, @"Curse Weapon");
                        AddLabel(205, 133, 1153, @"Evil Omen");
                        AddLabel(205, 163, 1153, @"Horrific Beast");
                        AddLabel(205, 193, 1153, @"Lich Form");
                        AddLabel(205, 223, 1153, @"Mind Rot");
                       
                        AddLabel(205, 253, 1153, @"Pain Spike");
                        AddLabel(205, 283, 1153, @"Poison Strike");
                        AddLabel(205, 313, 1153, @"Strangle");
                        AddLabel(205, 343, 1153, @"Summon Familiar");
                        AddLabel(205, 373, 1153, @"Vampiric Embrace");
                        AddLabel(205, 403, 1153, @"Vengeful Spirit");
                        AddLabel(205, 434, 1153, @"Wither");
                        AddLabel(205, 463, 1153, @"Wraith Form");
                       
                        AddLabel(336, 13, 1153, @"Exorcism");
                   
                        AddButton(12, 50, 4006, 4006, (int)Buttons.Button2, GumpButtonType.Reply, 0);//necro selected
                       
                        if ( mN01AnimateDeadSpell == 1 ) { this.AddButton( 190, 20,  2361, 2361, (int)Buttons.Button73, GumpButtonType.Reply, 1); }
                        if ( mN01AnimateDeadSpell == 0 ) { this.AddButton( 190, 20,  2360, 2360, (int)Buttons.Button73, GumpButtonType.Reply, 1); }
                       
                        if ( mN02BloodOathSpell == 1 ) { this.AddButton( 190, 50,  2361, 2361, (int)Buttons.Button74, GumpButtonType.Reply, 1); }
                        if ( mN02BloodOathSpell == 0 ) { this.AddButton( 190, 50,  2360, 2360, (int)Buttons.Button74, GumpButtonType.Reply, 1); }
                       
                        if ( mN03CorpseSkinSpell == 1 ) { this.AddButton( 190, 80,  2361, 2361, (int)Buttons.Button75, GumpButtonType.Reply, 1); }
                        if ( mN03CorpseSkinSpell == 0 ) { this.AddButton( 190, 80,  2360, 2360, (int)Buttons.Button75, GumpButtonType.Reply, 1); }
                       
                        if ( mN04CurseWeaponSpell == 1 ) { this.AddButton( 190, 110,  2361, 2361, (int)Buttons.Button76, GumpButtonType.Reply, 1); }
                        if ( mN04CurseWeaponSpell == 0 ) { this.AddButton( 190, 110,  2360, 2360, (int)Buttons.Button76, GumpButtonType.Reply, 1); }
                       
                        if ( mN05EvilOmenSpell == 1 ) { this.AddButton( 190, 140,  2361, 2361, (int)Buttons.Button77, GumpButtonType.Reply, 1); }
                        if ( mN05EvilOmenSpell == 0 ) { this.AddButton( 190, 140,  2360, 2360, (int)Buttons.Button77, GumpButtonType.Reply, 1); }
                       
                        if ( mN06HorrificBeastSpell == 1 ) { this.AddButton( 190, 170,  2361, 2361, (int)Buttons.Button78, GumpButtonType.Reply, 1); }
                        if ( mN06HorrificBeastSpell == 0 ) { this.AddButton( 190, 170,  2360, 2360, (int)Buttons.Button78, GumpButtonType.Reply, 1); }
                       
                        if ( mN07LichFormSpell == 1 ) { this.AddButton( 190, 200,  2361, 2361, (int)Buttons.Button79, GumpButtonType.Reply, 1); }
                        if ( mN07LichFormSpell == 0 ) { this.AddButton( 190, 200,  2360, 2360, (int)Buttons.Button79, GumpButtonType.Reply, 1); }
                       
                        if ( mN08MindRotSpell == 1 ) { this.AddButton( 190, 230,  2361, 2361, (int)Buttons.Button80, GumpButtonType.Reply, 1); }
                        if ( mN08MindRotSpell == 0 ) { this.AddButton( 190, 230,  2360, 2360, (int)Buttons.Button80, GumpButtonType.Reply, 1); }
                       
                        if ( mN09PainSpikeSpell == 1 ) { this.AddButton( 190, 260,  2361, 2361, (int)Buttons.Button81, GumpButtonType.Reply, 1); }
                        if ( mN09PainSpikeSpell == 0 ) { this.AddButton( 190, 260,  2360, 2360, (int)Buttons.Button81, GumpButtonType.Reply, 1); }
                       
                        if ( mN10PoisonStrikeSpell == 1 ) { this.AddButton( 190, 290,  2361, 2361, (int)Buttons.Button82, GumpButtonType.Reply, 1); }
                        if ( mN10PoisonStrikeSpell == 0 ) { this.AddButton( 190, 290,  2360, 2360, (int)Buttons.Button82, GumpButtonType.Reply, 1); }
                       
                        if ( mN11StrangleSpell == 1 ) { this.AddButton( 190, 320,  2361, 2361, (int)Buttons.Button83, GumpButtonType.Reply, 1); }
                        if ( mN11StrangleSpell == 0 ) { this.AddButton( 190, 320,  2360, 2360, (int)Buttons.Button83, GumpButtonType.Reply, 1); }
                       
                        if ( mN12SummonFamiliarSpell == 1 ) { this.AddButton( 190, 350,  2361, 2361, (int)Buttons.Button84, GumpButtonType.Reply, 1); }
                        if ( mN12SummonFamiliarSpell == 0 ) { this.AddButton( 190, 350,  2360, 2360, (int)Buttons.Button84, GumpButtonType.Reply, 1); }
                       
                        if ( mN13VampiricEmbraceSpell == 1 ) { this.AddButton( 190, 380,  2361, 2361, (int)Buttons.Button85, GumpButtonType.Reply, 1); }
                        if ( mN13VampiricEmbraceSpell == 0 ) { this.AddButton( 190, 380,  2360, 2360, (int)Buttons.Button85, GumpButtonType.Reply, 1); }
                       
                        if ( mN14VengefulSpiritSpell == 1 ) { this.AddButton( 190, 410,  2361, 2361, (int)Buttons.Button86, GumpButtonType.Reply, 1); }
                        if ( mN14VengefulSpiritSpell == 0 ) { this.AddButton( 190, 410,  2360, 2360, (int)Buttons.Button86, GumpButtonType.Reply, 1); }
                       
                        if ( mN15WitherSpell == 1 ) { this.AddButton( 190, 440,  2361, 2361, (int)Buttons.Button87, GumpButtonType.Reply, 1); }
                        if ( mN15WitherSpell == 0 ) { this.AddButton( 190, 440,  2360, 2360, (int)Buttons.Button87, GumpButtonType.Reply, 1); }
                       
                        if ( mN16WraithFormSpell == 1 ) { this.AddButton( 190, 470,  2361, 2361, (int)Buttons.Button88, GumpButtonType.Reply, 1); }
                        if ( mN16WraithFormSpell == 0 ) { this.AddButton( 190, 470,  2360, 2360, (int)Buttons.Button88, GumpButtonType.Reply, 1); }
                       
                        if ( mN17ExorcismSpell == 1 ) { this.AddButton( 321, 20,  2361, 2361, (int)Buttons.Button89, GumpButtonType.Reply, 1); }
                        if ( mN17ExorcismSpell == 0 ) { this.AddButton( 321, 20,  2360, 2360, (int)Buttons.Button89, GumpButtonType.Reply, 1); }
                       
                        break;
                    }
                case 3: /// chivalry
                    {
					
						AddBackground(170, 0, 250, 500, 9200);
						AddImageTiled(180, 10, 230, 480, 2624);
						AddAlphaRegion(180, 10, 230, 480);
						
					
                        AddLabel(205, 13, 1153, @"Cleanse by Fire");
                        AddLabel(205, 43, 1153, @"Close Wounds");
                        AddLabel(205, 73, 1153, @"Consecrate Weapon");
                        AddLabel(205, 103, 1153, @"Dispel Evil");
                        AddLabel(205, 133, 1153, @"Divine Fury");
                        AddLabel(205, 163, 1153, @"Enemy of One");
                        AddLabel(205, 193, 1153, @"Holy Light");
                        AddLabel(205, 223, 1153, @"Noble Sacrifice");
                        AddLabel(205, 253, 1153, @"Remove Curse");
                        AddLabel(205, 283, 1153, @"Sacred Journey");
                   
                        AddButton(12, 80, 4006, 4006, (int)Buttons.Button3, GumpButtonType.Reply, 0);//chiv selected
                   
                        if ( mC01CleanseByFireSpell == 1 ) { this.AddButton( 190, 20,  2361, 2361, (int)Buttons.Button90, GumpButtonType.Reply, 1); }
                        if ( mC01CleanseByFireSpell == 0 ) { this.AddButton( 190, 20,  2360, 2360, (int)Buttons.Button90, GumpButtonType.Reply, 1); }
                       
                        if ( mC02CloseWoundsSpell == 1 ) { this.AddButton( 190, 50,  2361, 2361, (int)Buttons.Button91, GumpButtonType.Reply, 1); }
                        if ( mC02CloseWoundsSpell == 0 ) { this.AddButton( 190, 50,  2360, 2360, (int)Buttons.Button91, GumpButtonType.Reply, 1); }
                       
                        if ( mC03ConsecrateWeaponSpell == 1 ) { this.AddButton( 190, 80,  2361, 2361, (int)Buttons.Button92, GumpButtonType.Reply, 1); }
                        if ( mC03ConsecrateWeaponSpell == 0 ) { this.AddButton( 190, 80,  2360, 2360, (int)Buttons.Button92, GumpButtonType.Reply, 1); }
                       
                        if ( mC04DispelEvilSpell == 1 ) { this.AddButton( 190, 110,  2361, 2361, (int)Buttons.Button93, GumpButtonType.Reply, 1); }
                        if ( mC04DispelEvilSpell == 0 ) { this.AddButton( 190, 110,  2360, 2360, (int)Buttons.Button93, GumpButtonType.Reply, 1); }
                       
                        if ( mC05DivineFurySpell == 1 ) { this.AddButton( 190, 140,  2361, 2361, (int)Buttons.Button94, GumpButtonType.Reply, 1); }
                        if ( mC05DivineFurySpell == 0 ) { this.AddButton( 190, 140,  2360, 2360, (int)Buttons.Button94, GumpButtonType.Reply, 1); }
                       
                        if ( mC06EnemyOfOneSpell == 1 ) { this.AddButton( 190, 170,  2361, 2361, (int)Buttons.Button95, GumpButtonType.Reply, 1); }
                        if ( mC06EnemyOfOneSpell == 0 ) { this.AddButton( 190, 170,  2360, 2360, (int)Buttons.Button95, GumpButtonType.Reply, 1); }
                       
                        if ( mC07HolyLightSpell == 1 ) { this.AddButton( 190, 200,  2361, 2361, (int)Buttons.Button96, GumpButtonType.Reply, 1); }
                        if ( mC07HolyLightSpell == 0 ) { this.AddButton( 190, 200,  2360, 2360, (int)Buttons.Button96, GumpButtonType.Reply, 1); }
                       
                        if ( mC08NobleSacrificeSpell == 1 ) { this.AddButton( 190, 230,  2361, 2361, (int)Buttons.Button97, GumpButtonType.Reply, 1); }
                        if ( mC08NobleSacrificeSpell == 0 ) { this.AddButton( 190, 230,  2360, 2360, (int)Buttons.Button97, GumpButtonType.Reply, 1); }
                       
                        if ( mC09RemoveCurseSpell == 1 ) { this.AddButton( 190, 260,  2361, 2361, (int)Buttons.Button98, GumpButtonType.Reply, 1); }
                        if ( mC09RemoveCurseSpell == 0 ) { this.AddButton( 190, 260,  2360, 2360, (int)Buttons.Button98, GumpButtonType.Reply, 1); }
                       
                        if ( mC10SacredJourneySpell == 1 ) { this.AddButton( 190, 290,  2361, 2361, (int)Buttons.Button99, GumpButtonType.Reply, 1); }
                        if ( mC10SacredJourneySpell == 0 ) { this.AddButton( 190, 290,  2360, 2360, (int)Buttons.Button99, GumpButtonType.Reply, 1); }
                        break;
                    }
                case 4: /// bushido
                    {
						AddBackground(170, 0, 250, 500, 9200);
						AddImageTiled(180, 10, 230, 480, 2624);
						AddAlphaRegion(180, 10, 230, 480);
						
						AddButton(12, 110, 4006, 4006, (int)Buttons.Button4, GumpButtonType.Reply, 0);//nin
					
						AddLabel(205, 13, 1153, @"Honorable Execution");
                        AddLabel(205, 43, 1153, @"Confidence");
                        AddLabel(205, 73, 1153, @"Counter Attack");
                        AddLabel(205, 103, 1153, @"Evasion");
                        AddLabel(205, 133, 1153, @"Lightning Stike");
                        AddLabel(205, 163, 1153, @"Momentum Strike");
                        /*
						if (mB06HonorableExecution == 1 ) { this.AddButton( 190, 20,  2361, 2361, (int)Buttons.Button144, GumpButtonType.Reply, 1); }
                        if (mB06HonorableExecution == 0 ) { this.AddButton( 190, 20,  2360, 2360, (int)Buttons.Button144, GumpButtonType.Reply, 1); }
						*/
                        if (mB01Confidence == 1 ) { this.AddButton( 190, 50,  2361, 2361, (int)Buttons.Button100, GumpButtonType.Reply, 1); }
                        if (mB01Confidence == 0 ) { this.AddButton( 190, 50,  2360, 2360, (int)Buttons.Button100, GumpButtonType.Reply, 1); }

                        if (mB02CounterAttack == 1 ) { this.AddButton( 190, 80,  2361, 2361, (int)Buttons.Button101, GumpButtonType.Reply, 1); }
                        if (mB02CounterAttack == 0 ) { this.AddButton( 190, 80,  2360, 2360, (int)Buttons.Button101, GumpButtonType.Reply, 1); }

                        if (mB03Evasion == 1 ) { this.AddButton( 190, 110,  2361, 2361, (int)Buttons.Button102, GumpButtonType.Reply, 1); }
                        if (mB03Evasion == 0 ) { this.AddButton( 190, 110,  2360, 2360, (int)Buttons.Button102, GumpButtonType.Reply, 1); }

                        if (mB04LightningStrike == 1 ) { this.AddButton( 190, 140,  2361, 2361, (int)Buttons.Button103, GumpButtonType.Reply, 1); }
                        if (mB04LightningStrike == 0 ) { this.AddButton( 190, 140,  2360, 2360, (int)Buttons.Button103, GumpButtonType.Reply, 1); }

                        if (mB05MomentumStrike == 1 ) { this.AddButton( 190, 170,  2361, 2361, (int)Buttons.Button104, GumpButtonType.Reply, 1); }
                        if (mB05MomentumStrike == 0 ) { this.AddButton( 190, 170,  2360, 2360, (int)Buttons.Button104, GumpButtonType.Reply, 1); }
						
						if (mB06HonorableExecution == 1 ) { this.AddButton( 190, 20,  2361, 2361, (int)Buttons.Button144, GumpButtonType.Reply, 1); }
                        if (mB06HonorableExecution == 0 ) { this.AddButton( 190, 20,  2360, 2360, (int)Buttons.Button144, GumpButtonType.Reply, 1); }

                       
                        break;
                    }    
                case 5: /// ninjitsu
                    {
						AddBackground(170, 0, 250, 500, 9200);
						AddImageTiled(180, 10, 230, 480, 2624);
						AddAlphaRegion(180, 10, 230, 480);
						
						AddButton(12, 140, 4006, 4006, (int)Buttons.Button5, GumpButtonType.Reply, 0);//bush
						
						AddLabel(205, 13, 1153, @"Focus Attack");
                        AddLabel(205, 43, 1153, @"Death Strike ");
                        AddLabel(205, 73, 1153, @"Animal Form");
                        AddLabel(205, 103, 1153, @"Ki Attack ");
                        AddLabel(205, 133, 1153, @"Surprise Attack");
                        AddLabel(205, 163, 1153, @"Backstab");
                        AddLabel(205, 193, 1153, @"Shadowjump");
                        AddLabel(205, 223, 1153, @"Mirror Image");
						
						
						
						
						
                        if (mI01DeathStrike == 1 ) { this.AddButton( 190, 50,  2361, 2361, (int)Buttons.Button105, GumpButtonType.Reply, 1); }
                        if (mI01DeathStrike == 0 ) { this.AddButton( 190, 50,  2360, 2360, (int)Buttons.Button105, GumpButtonType.Reply, 1); }

                        if (mI02AnimalForm == 1 ) { this.AddButton( 190, 80,  2361, 2361, (int)Buttons.Button106, GumpButtonType.Reply, 1); }
                        if (mI02AnimalForm == 0 ) { this.AddButton( 190, 80,  2360, 2360, (int)Buttons.Button106, GumpButtonType.Reply, 1); }

                        if (mI03KiAttack == 1 ) { this.AddButton( 190, 110,  2361, 2361, (int)Buttons.Button107, GumpButtonType.Reply, 1); }
                        if (mI03KiAttack == 0 ) { this.AddButton( 190, 110,  2360, 2360, (int)Buttons.Button107, GumpButtonType.Reply, 1); }

                        if (mI04SurpriseAttack == 1 ) { this.AddButton( 190, 140,  2361, 2361, (int)Buttons.Button108, GumpButtonType.Reply, 1); }
                        if (mI04SurpriseAttack == 0 ) { this.AddButton( 190, 140,  2360, 2360, (int)Buttons.Button108, GumpButtonType.Reply, 1); }

                        if (mI05Backstab == 1 ) { this.AddButton( 190, 170,  2361, 2361, (int)Buttons.Button109, GumpButtonType.Reply, 1); }
                        if (mI05Backstab == 0 ) { this.AddButton( 190, 170,  2360, 2360, (int)Buttons.Button109, GumpButtonType.Reply, 1); }

                        if (mI06Shadowjump == 1 ) { this.AddButton( 190, 200,  2361, 2361, (int)Buttons.Button110, GumpButtonType.Reply, 1); }
                        if (mI06Shadowjump == 0 ) { this.AddButton( 190, 200,  2360, 2360, (int)Buttons.Button110, GumpButtonType.Reply, 1); }

                        if (mI07MirrorImage == 1 ) { this.AddButton( 190, 230,  2361, 2361, (int)Buttons.Button111, GumpButtonType.Reply, 1); }
                        if (mI07MirrorImage == 0 ) { this.AddButton( 190, 230,  2360, 2360, (int)Buttons.Button111, GumpButtonType.Reply, 1); }
						
						if (mI08FocusAttack == 1 ) { this.AddButton( 190, 20,  2361, 2361, (int)Buttons.Button145, GumpButtonType.Reply, 1); }
                        if (mI08FocusAttack == 0 ) { this.AddButton( 190, 20,  2360, 2360, (int)Buttons.Button145, GumpButtonType.Reply, 1); }

                        break;
                    }    
                case 6: /// spellweaving
                    {
						AddBackground(170, 0, 250, 500, 9200);
						AddImageTiled(180, 10, 230, 480, 2624);
						AddAlphaRegion(180, 10, 230, 480);
					
						AddButton(12, 170, 4006, 4006, (int)Buttons.Button6, GumpButtonType.Reply, 0);//weave
					
						AddLabel(205, 13, 1153, @"Arcane Circle");
                        AddLabel(205, 43, 1153, @"Gift of Renewal");
                        AddLabel(205, 73, 1153, @"Immolating Weapon");
                        AddLabel(205, 103, 1153, @"Attune Weapon");
                        AddLabel(205, 133, 1153, @"Thunderstorm");
                        AddLabel(205, 163, 1153, @"Nature's Fury");
                        AddLabel(205, 193, 1153, @"Summon Fey");
                        AddLabel(205, 223, 1153, @"Summon Fiend");
                        AddLabel(205, 253, 1153, @"Reapor Form");
                        AddLabel(205, 283, 1153, @"WildfireSpell");
                        AddLabel(205, 313, 1153, @"Essence of Wind");
                        AddLabel(205, 343, 1153, @"Dryad Allure");
                        AddLabel(205, 373, 1153, @"Ethereal Voyage");
                        AddLabel(205, 403, 1153, @"Word of Death");
                        AddLabel(205, 434, 1153, @"Gift of Life");
                        AddLabel(205, 463, 1153, @"Arcane Empowerment");
					
                        if (mS01ArcaneCircleSpell== 1 ) { this.AddButton( 190, 20,  2361, 2361, (int)Buttons.Button112, GumpButtonType.Reply, 1); }
                        if (mS01ArcaneCircleSpell== 0 ) { this.AddButton( 190, 20,  2360, 2360, (int)Buttons.Button112, GumpButtonType.Reply, 1); }

                        if (mS02GiftOfRenewalSpell== 1 ) { this.AddButton( 190, 50,  2361, 2361, (int)Buttons.Button113, GumpButtonType.Reply, 1); }
                        if (mS02GiftOfRenewalSpell== 0 ) { this.AddButton( 190, 50,  2360, 2360, (int)Buttons.Button113, GumpButtonType.Reply, 1); }

                        if (mS03ImmolatingWeaponSpell== 1 ) { this.AddButton( 190, 80,  2361, 2361, (int)Buttons.Button114, GumpButtonType.Reply, 1); }
                        if (mS03ImmolatingWeaponSpell== 0 ) { this.AddButton( 190, 80,  2360, 2360, (int)Buttons.Button114, GumpButtonType.Reply, 1); }

                        if (mS04AttuneWeaponSpell== 1 ) { this.AddButton( 190, 110,  2361, 2361, (int)Buttons.Button115, GumpButtonType.Reply, 1); }
                        if (mS04AttuneWeaponSpell== 0 ) { this.AddButton( 190, 110,  2360, 2360, (int)Buttons.Button115, GumpButtonType.Reply, 1); }

                        if (mS05ThunderstormSpell== 1 ) { this.AddButton( 190, 140,  2361, 2361, (int)Buttons.Button116, GumpButtonType.Reply, 1); }
                        if (mS05ThunderstormSpell== 0 ) { this.AddButton( 190, 140,  2360, 2360, (int)Buttons.Button116, GumpButtonType.Reply, 1); }

                        if (mS06NatureFurySpell== 1 ) { this.AddButton( 190, 170,  2361, 2361, (int)Buttons.Button117, GumpButtonType.Reply, 1); }
                        if (mS06NatureFurySpell== 0 ) { this.AddButton( 190, 170,  2360, 2360, (int)Buttons.Button117, GumpButtonType.Reply, 1); }

                        if (mS07SummonFeySpell== 1 ) { this.AddButton( 190, 200,  2361, 2361, (int)Buttons.Button118, GumpButtonType.Reply, 1); }
                        if (mS07SummonFeySpell== 0 ) { this.AddButton( 190, 200,  2360, 2360, (int)Buttons.Button118, GumpButtonType.Reply, 1); }

                        if (mS08SummonFiendSpell== 1 ) { this.AddButton( 190, 230,  2361, 2361, (int)Buttons.Button119, GumpButtonType.Reply, 1); }
                        if (mS08SummonFiendSpell== 0 ) { this.AddButton( 190, 230,  2360, 2360, (int)Buttons.Button119, GumpButtonType.Reply, 1); }

                        if (mS09ReaperFormSpell== 1 ) { this.AddButton( 190, 260,  2361, 2361, (int)Buttons.Button120, GumpButtonType.Reply, 1); }
                        if (mS09ReaperFormSpell== 0 ) { this.AddButton( 190, 260,  2360, 2360, (int)Buttons.Button120, GumpButtonType.Reply, 1); }

                        if (mS10WildfireSpell== 1 ) { this.AddButton( 190, 290,  2361, 2361, (int)Buttons.Button121, GumpButtonType.Reply, 1); }
                        if (mS10WildfireSpell== 0 ) { this.AddButton( 190, 290,  2360, 2360, (int)Buttons.Button121, GumpButtonType.Reply, 1); }

                        if (mS11EssenceOfWindSpell== 1 ) { this.AddButton( 190, 320,  2361, 2361, (int)Buttons.Button122, GumpButtonType.Reply, 1); }
                        if (mS11EssenceOfWindSpell== 0 ) { this.AddButton( 190, 320,  2360, 2360, (int)Buttons.Button122, GumpButtonType.Reply, 1); }

                        if (mS12DryadAllureSpell== 1 ) { this.AddButton( 190, 350,  2361, 2361, (int)Buttons.Button123, GumpButtonType.Reply, 1); }
                        if (mS12DryadAllureSpell== 0 ) { this.AddButton( 190, 350,  2360, 2360, (int)Buttons.Button123, GumpButtonType.Reply, 1); }

                        if (mS13EtherealVoyageSpell== 1 ) { this.AddButton( 190, 380,  2361, 2361, (int)Buttons.Button124, GumpButtonType.Reply, 1); }
                        if (mS13EtherealVoyageSpell== 0 ) { this.AddButton( 190, 380,  2360, 2360, (int)Buttons.Button124, GumpButtonType.Reply, 1); }

                        if (mS14WordOfDeathSpell== 1 ) { this.AddButton( 190, 410,  2361, 2361, (int)Buttons.Button125, GumpButtonType.Reply, 1); }
                        if (mS14WordOfDeathSpell== 0 ) { this.AddButton( 190, 410,  2360, 2360, (int)Buttons.Button125, GumpButtonType.Reply, 1); }

                        if (mS15GiftOfLifeSpell== 1 ) { this.AddButton( 190, 440,  2361, 2361, (int)Buttons.Button126, GumpButtonType.Reply, 1); }
                        if (mS15GiftOfLifeSpell== 0 ) { this.AddButton( 190, 440,  2360, 2360, (int)Buttons.Button126, GumpButtonType.Reply, 1); }

                        if (mS16ArcaneEmpowermentSpell== 1 ) { this.AddButton( 190, 470,  2361, 2361, (int)Buttons.Button127, GumpButtonType.Reply, 1); }
                        if (mS16ArcaneEmpowermentSpell== 0 ) { this.AddButton( 190, 470,  2360, 2360, (int)Buttons.Button127, GumpButtonType.Reply, 1); }

                        break;
                    }    
                case 7: /// mysticism
                    {
						AddBackground(170, 0, 250, 500, 9200);
						AddImageTiled(180, 10, 230, 480, 2624);
						AddAlphaRegion(180, 10, 230, 480);
					
						AddButton(12, 200, 4006, 4006, (int)Buttons.Button7, GumpButtonType.Reply, 0);//myst
					
						AddLabel(205, 13, 1153, @"Nether Bolt");
                        AddLabel(205, 43, 1153, @"Healing Stone");
                        AddLabel(205, 73, 1153, @"Purge Magic");
                        AddLabel(205, 103, 1153, @"Enchant Spell");
                        AddLabel(205, 133, 1153, @"Sleep");
                        AddLabel(205, 163, 1153, @"Eagle Strike");
                        AddLabel(205, 193, 1153, @"Animated Weapon");
                        AddLabel(205, 223, 1153, @"Stone Form");
                        AddLabel(205, 253, 1153, @"Spell Trigger");
                        AddLabel(205, 283, 1153, @"Mass Sleep");
                        AddLabel(205, 313, 1153, @"Cleansing Winds");
                        AddLabel(205, 343, 1153, @"Bombard");
                        AddLabel(205, 373, 1153, @"Spell Plague");
                        AddLabel(205, 403, 1153, @"Hail Storm");
                        AddLabel(205, 434, 1153, @"Nether Cyclone");
                        AddLabel(205, 463, 1153, @"Rising Colossus");
					
                        if (mM01NetherBoltSpell== 1 ) { this.AddButton( 190, 20,  2361, 2361, (int)Buttons.Button128, GumpButtonType.Reply, 1); }
                        if (mM01NetherBoltSpell== 0 ) { this.AddButton( 190, 20,  2360, 2360, (int)Buttons.Button128, GumpButtonType.Reply, 1); }
						
						if (mM02HealingStoneSpell== 1 ) { this.AddButton( 190, 50,  2361, 2361, (int)Buttons.Button129, GumpButtonType.Reply, 1); }
                        if (mM02HealingStoneSpell== 0 ) { this.AddButton( 190, 50,  2360, 2360, (int)Buttons.Button129, GumpButtonType.Reply, 1); }
						
						if (mM03PurgeMagicSpell== 1 ) { this.AddButton( 190, 80,  2361, 2361, (int)Buttons.Button130, GumpButtonType.Reply, 1); }
                        if (mM03PurgeMagicSpell== 0 ) { this.AddButton( 190, 80,  2360, 2360, (int)Buttons.Button130, GumpButtonType.Reply, 1); }
					
						if (mM04EnchantSpell== 1 ) { this.AddButton( 190, 110,  2361, 2361, (int)Buttons.Button131, GumpButtonType.Reply, 1); }
                        if (mM04EnchantSpell== 0 ) { this.AddButton( 190, 110,  2360, 2360, (int)Buttons.Button131, GumpButtonType.Reply, 1); }
					
						if (mM05SleepSpell== 1 ) { this.AddButton( 190, 140,  2361, 2361, (int)Buttons.Button132, GumpButtonType.Reply, 1); }
                        if (mM05SleepSpell== 0 ) { this.AddButton( 190, 140,  2360, 2360, (int)Buttons.Button132, GumpButtonType.Reply, 1); }
					
						if (mM06EagleStrikeSpell== 1 ) { this.AddButton( 190, 170,  2361, 2361, (int)Buttons.Button133, GumpButtonType.Reply, 1); }
                        if (mM06EagleStrikeSpell== 0 ) { this.AddButton( 190, 170,  2360, 2360, (int)Buttons.Button133, GumpButtonType.Reply, 1); }
					
						if (mM07AnimatedWeaponSpell== 1 ) { this.AddButton( 190, 200,  2361, 2361, (int)Buttons.Button134, GumpButtonType.Reply, 1); }
                        if (mM07AnimatedWeaponSpell== 0 ) { this.AddButton( 190, 200,  2360, 2360, (int)Buttons.Button134, GumpButtonType.Reply, 1); }
					
						if (mM08SpellTriggerSpell== 1 ) { this.AddButton( 190, 230,  2361, 2361, (int)Buttons.Button135, GumpButtonType.Reply, 1); }
                        if (mM08SpellTriggerSpell== 0 ) { this.AddButton( 190, 230,  2360, 2360, (int)Buttons.Button135, GumpButtonType.Reply, 1); }
					
						if (mM09MassSleepSpell== 1 ) { this.AddButton( 190, 260,  2361, 2361, (int)Buttons.Button136, GumpButtonType.Reply, 1); }
                        if (mM09MassSleepSpell== 0 ) { this.AddButton( 190, 260,  2360, 2360, (int)Buttons.Button136, GumpButtonType.Reply, 1); }
					
						if (mM10CleansingWindsSpell== 1 ) { this.AddButton( 190, 290,  2361, 2361, (int)Buttons.Button137, GumpButtonType.Reply, 1); }
                        if (mM10CleansingWindsSpell== 0 ) { this.AddButton( 190, 290,  2360, 2360, (int)Buttons.Button137, GumpButtonType.Reply, 1); }
					
						if (mM11BombardSpell== 1 ) { this.AddButton( 190, 320,  2361, 2361, (int)Buttons.Button138, GumpButtonType.Reply, 1); }
                        if (mM11BombardSpell== 0 ) { this.AddButton( 190, 320,  2360, 2360, (int)Buttons.Button138, GumpButtonType.Reply, 1); }
					
						if (mM12SpellPlagueSpell== 1 ) { this.AddButton( 190, 350,  2361, 2361, (int)Buttons.Button139, GumpButtonType.Reply, 1); }
                        if (mM12SpellPlagueSpell== 0 ) { this.AddButton( 190, 350,  2360, 2360, (int)Buttons.Button139, GumpButtonType.Reply, 1); }
					
						if (mM13HailStormSpell== 1 ) { this.AddButton( 190, 380,  2361, 2361, (int)Buttons.Button140, GumpButtonType.Reply, 1); }
                        if (mM13HailStormSpell== 0 ) { this.AddButton( 190, 380,  2360, 2360, (int)Buttons.Button140, GumpButtonType.Reply, 1); }
					
						if (mM14NetherCycloneSpell== 1 ) { this.AddButton( 190, 410,  2361, 2361, (int)Buttons.Button141, GumpButtonType.Reply, 1); }
                        if (mM14NetherCycloneSpell== 0 ) { this.AddButton( 190, 410,  2360, 2360, (int)Buttons.Button141, GumpButtonType.Reply, 1); }
					
						if (mM15RisingColossusSpell== 1 ) { this.AddButton( 190, 440,  2361, 2361, (int)Buttons.Button142, GumpButtonType.Reply, 1); }
                        if (mM15RisingColossusSpell== 0 ) { this.AddButton( 190, 440,  2360, 2360, (int)Buttons.Button142, GumpButtonType.Reply, 1); }
					
						if (mM16StoneFormSpell== 1 ) { this.AddButton( 190, 470,  2361, 2361, (int)Buttons.Button143, GumpButtonType.Reply, 1); }
                        if (mM16StoneFormSpell== 0 ) { this.AddButton( 190, 470,  2360, 2360, (int)Buttons.Button143, GumpButtonType.Reply, 1); }
					
						break;
                    }    
                case 8://help
                    {
						
						AddBackground(170, 0, 380, 300, 9200);
						//AddButton( 185,268, 2453,2454, 0, GumpButtonType.Reply, 1); // Cancel
						AddHtml( 180, 10, 350, 250, @"<br><H2>Spell Hot Bar Help</H2><br><br>This script is in Beta but should be fully fuctional and stable.<br>Use the Selection Menu to the left to navigate between the different types of magic. You can choose spells from any section, and they will all be combined into one Hot Bar.<br>*Note: Only spells that are available in your packback will be sent to the Hot Bar.<br><br>Your selection is limited based on how many Columns and are Rows are selected in the Options Menu. Below the Open Hot Bar button, you will see an indicator of how many spells you've selected and how many are allowed based on the current settings for Columns and Rows.<br>To easily reset your selection, click the default button in the Options Menu.<br><br>", (bool)true, (bool)true);    
						
						
						
                        break;
                    }
				case 9: // options
					{
						AddButton(12, 230, 4012, 4012, (int)Buttons.Button148, GumpButtonType.Reply, 0); // options
						
                        AddBackground(170, 0, 570, 350, 9200);
						AddImageTiled(180, 10, 550, 330, 2624);
						AddAlphaRegion(180, 10, 550, 330);
						
						AddButton(190, 20, 246, 244, (int)Buttons.Button147, GumpButtonType.Reply, 0);//reset
						AddLabel(260, 21, 1153, @"Reset selection");
						
						AddHtml( 193, 68, 526, 67, @"<br>The number of spells you can select is limited to the how many columns and rows you have selected. If you wish to reset your selection, click the Default button.<br><br><H2>Columns</H2><br>If the hot bar set to the horizontal position, columns indicate how many buttons will stretch from left to right.<br>If it's in the vertical position, columns indicate how many buttons will stretch up and down.<br><br><H2>Rows</H2><br>If the hot bar is in a horizontal position, rows indicate how many buttons will stretch up and down.<br>If it is in a vertical position, it indciates how many buttons stretch from left to right.<br><br>Example: If you choose 10 columns and 2 rows and then select only 10 spells, you'll only see one row of 10. However if you choose between 11 and 20 spells, you'll see the 11th spell drop down into a second row.<br><br>", (bool)true, (bool)true); 
			//////////////////////			
						AddLabel(395, 165, 1153, @"Columns");
						
						AddLabel(236, 189, 1153, @"10");
						AddLabel(336, 189, 1153, @"15");
						AddLabel(436, 189, 1153, @"20");
						AddLabel(536, 189, 1153, @"30");
						
						if (mXselect_10== 1 ) { this.AddButton( 214, 194,  2361, 2361, (int)Buttons.Button149, GumpButtonType.Reply, 1); }
                        if (mXselect_10== 0 ) { this.AddButton( 214, 194,  2360, 2360, (int)Buttons.Button149, GumpButtonType.Reply, 1); }
						
						if (mXselect_15== 1 ) { this.AddButton( 314, 194,  2361, 2361, (int)Buttons.Button150, GumpButtonType.Reply, 1); }
                        if (mXselect_15== 0 ) { this.AddButton( 314, 194,  2360, 2360, (int)Buttons.Button150, GumpButtonType.Reply, 1); }
						
						if (mXselect_20== 1 ) { this.AddButton( 414, 194,  2361, 2361, (int)Buttons.Button151, GumpButtonType.Reply, 1); }
                        if (mXselect_20== 0 ) { this.AddButton( 414, 194,  2360, 2360, (int)Buttons.Button151, GumpButtonType.Reply, 1); }
						
						if (mXselect_30== 1 ) { this.AddButton( 514, 194,  2361, 2361, (int)Buttons.Button152, GumpButtonType.Reply, 1); }
                        if (mXselect_30== 0 ) { this.AddButton( 514, 194,  2360, 2360, (int)Buttons.Button152, GumpButtonType.Reply, 1); }
			//////////////////////////	   
						AddLabel(403, 265, 1153, @"Rows");
						
						AddLabel(236, 289, 1153, @"1");
						AddLabel(336, 289, 1153, @"2");
						AddLabel(436, 289, 1153, @"3");
						AddLabel(536, 289, 1153, @"4");
						
						if (mYselect_1 == 1 ) { this.AddButton( 214, 294,  2361, 2361, (int)Buttons.Button153, GumpButtonType.Reply, 1); }
                        if (mYselect_1 == 0 ) { this.AddButton( 214, 294,  2360, 2360, (int)Buttons.Button153, GumpButtonType.Reply, 1); }
						
						if (mYselect_2== 1 ) { this.AddButton( 314, 294,  2361, 2361, (int)Buttons.Button154, GumpButtonType.Reply, 1); }
                        if (mYselect_2== 0 ) { this.AddButton( 314, 294,  2360, 2360, (int)Buttons.Button154, GumpButtonType.Reply, 1); }
						
						if (mYselect_3== 1 ) { this.AddButton( 414, 294,  2361, 2361, (int)Buttons.Button155, GumpButtonType.Reply, 1); }
                        if (mYselect_3== 0 ) { this.AddButton( 414, 294,  2360, 2360, (int)Buttons.Button155, GumpButtonType.Reply, 1); }
						
						if (mYselect_4== 1 ) { this.AddButton( 514, 294,  2361, 2361, (int)Buttons.Button156, GumpButtonType.Reply, 1); }
                        if (mYselect_4== 0 ) { this.AddButton( 514, 294,  2360, 2360, (int)Buttons.Button156, GumpButtonType.Reply, 1); }
			
			
						break;
					}
					case 10:
					{
						//AddBackground(170, 0, 250, 500, 9200);
						//AddImageTiled(180, 10, 230, 480, 2624);
						//AddLabel( 25, 336, 1153, "You have selected" );
			//AddLabel( 35, 356, 1153, String.Format("{0} of {1} spells", mCount, xselect_num * yselect_var ) );
						
						AddAlphaRegion(20, 15, 375, 425);
						AddHtml( 39, 35, 328, 318, @"<br>You have selected too many spells!<br><br>Please reduce the number of spells chosen or increase the size of the hot bar in the Options Menu.<br><br>Check the readout to the left to see how many spells are currently chosen and how many are currently allowed.", (bool)true, (bool)true); 
						
						
						AddButton(162, 366, 247, 249, (int)Buttons.Button157, GumpButtonType.Reply, 0); // options
						
						break;
					}
					
					
            }
        }
       

        public enum Buttons { Button0, Button1, Button2, Button3, Button4, Button5, Button6, Button7, Button8, Button9, Button10, Button11, Button12, Button13, Button14, Button15, Button16, Button17, Button18, Button19, Button20, Button21, Button22, Button23, Button24, Button25, Button26, Button27, Button28, Button29, Button30, Button31, Button32, Button33, Button34, Button35, Button36, Button37, Button38, Button39, Button40, Button41, Button42, Button43, Button44, Button45, Button46, Button47, Button48, Button49, Button50, Button51, Button52, Button53, Button54, Button55, Button56, Button57, Button58, Button59, Button60, Button61, Button62, Button63, Button64, Button65, Button66, Button67, Button68, Button69, Button70, Button71, Button72, Button73, Button74, Button75, Button76, Button77, Button78, Button79, Button80, Button81, Button82, Button83, Button84, Button85, Button86, Button87, Button88, Button89, Button90, Button91, Button92, Button93, Button94, Button95, Button96, Button97, Button98, Button99, Button100, Button101, Button102, Button103, Button104, Button105, Button106, Button107, Button108, Button109, Button110, Button111, Button112, Button113, Button114, Button115, Button116, Button117, Button118, Button119, Button120, Button121, Button122, Button123, Button124, Button125, Button126, Button127, Button128, Button129, Button130, Button131, Button132, Button133, Button134, Button135, Button136, Button137, Button138, Button139, Button140, Button141, Button142, Button143, Button144, Button145, Button146, Button147, Button148, Button149, Button150, Button151, Button152, Button153, Button154, Button155, Button156, Button157, }


        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;
        

            switch(info.ButtonID)
            {
                case (int)Buttons.Button0://cancel
                    {
                        from.CloseGump( typeof( SpellBarGump ) );
                        break;
                    }
                case (int)Buttons.Button1://magery
                    {
                        int page = 0;
                        page = 1;
                        from.SendGump(new SpellBarGump(from, page, m_Scroll));
                        break;
                    }
                case (int)Buttons.Button2://necromancy
                {
                    int page = 0;
                    page = 2;
                    from.SendGump(new SpellBarGump(from, page, m_Scroll ));
                    break;
                }
                case (int)Buttons.Button3://chivalry
                {
                    int page = 0;
                    page = 3;
                    from.SendGump(new SpellBarGump(from, page, m_Scroll ));
                    break;
                }
                case (int)Buttons.Button4://ninjitsu
                {
                    int page = 0;
                    page = 4;
                    from.SendGump(new SpellBarGump(from, page, m_Scroll ));
                    break;
                }
                case (int)Buttons.Button5://bushido
                {
                    int page = 0;
                    page = 5;
                    from.SendGump(new SpellBarGump(from, page, m_Scroll ));
                    break;
                }
                case (int)Buttons.Button6://spellweaver
                {
                    int page = 0;
                    page = 6;
                    from.SendGump(new SpellBarGump(from, page, m_Scroll ));
                    break;
                }
                case (int)Buttons.Button7://mysticism
                {
                    int page = 0;
                    page = 7;
                    from.SendGump(new SpellBarGump(from, page, m_Scroll ));
                    break;
                }
                case (int)Buttons.Button8://send to spellbar_bargump
                {
					int mCount = m_Scroll.mCount;
					int yselect_var = 0; //xselect_var = 0;
			int xselect_num = 0;
			if ( m_Scroll.mXselect_10 == 1) { xselect_num = 10; } 
			if ( m_Scroll.mXselect_15 == 1) { xselect_num = 15; } 
			if ( m_Scroll.mXselect_20 == 1) { xselect_num = 20; } 
			if ( m_Scroll.mXselect_30 == 1) { xselect_num = 30; }
			if ( m_Scroll.mYselect_1 == 1) { yselect_var = 1; }
			if ( m_Scroll.mYselect_2 == 1) { yselect_var = 2; }
			if ( m_Scroll.mYselect_3 == 1) { yselect_var = 3; }
			if ( m_Scroll.mYselect_4 == 1) { yselect_var = 4; }
				
					if ( mCount > xselect_num * yselect_var )
					{
						int page = 0;
						page = 10;
						from.SendGump(new SpellBarGump(from, page, m_Scroll ));
					}
					else
					{
						int dbx = 0; //dbx = 50;
						int dbxa = 0; //dbxa = 45;
						int dby = 0; //dby = 5;
						int dbya = 0; //dbya = 0;
						int xselect_var = 0; //xselect_var = 0;
						
						from.CloseGump( typeof( SpellBarGump ) );
						from.CloseGump( typeof( SpellBar_BarGump ) );
						from.SendGump(new SpellBar_BarGump(from, dbx, dbxa, dby, dbya, xselect_var, m_Scroll ));
					}
                    break;
                }
                case (int)Buttons.Button9://clumsy
                {
                    int page = 0;
                    page = 1;
                    //from.SendGump(new SpellBarGump(from, page, m_Scroll ));
                    if ( m_Scroll.mW00_ClumsySpell == 0 ) { m_Scroll.mW00_ClumsySpell = 1; m_Scroll.mCount += 1;}
                   
                    else { m_Scroll.mW00_ClumsySpell = 0;  m_Scroll.mCount -= 1; }
					
                   
                    from.SendGump( new SpellBarGump ( from, page, m_Scroll ) );
           
                    break;
                }
                case (int)Buttons.Button10:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW01_CreateFoodSpell == 0 ) { m_Scroll.mW01_CreateFoodSpell = 1; m_Scroll.mCount += 1; }
                   
                    else { m_Scroll.mW01_CreateFoodSpell = 0; m_Scroll.mCount -= 1; }
                   
                    from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button11:
                {
                    int page = 0;
                    page = 1;
                   if ( m_Scroll.mW02_FeeblemindSpell == 0 ) { m_Scroll.mW02_FeeblemindSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW02_FeeblemindSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button12:
                {
                    int page = 0;
                    page = 1;
                   if ( m_Scroll.mW03_HealSpell == 0 ) { m_Scroll.mW03_HealSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW03_HealSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button13:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW04_MagicArrowSpell == 0 ) { m_Scroll.mW04_MagicArrowSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW04_MagicArrowSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button14:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW05_NightSightSpell == 0 ) { m_Scroll.mW05_NightSightSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW05_NightSightSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button15:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW06_ReactiveArmorSpell == 0 ) { m_Scroll.mW06_ReactiveArmorSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW06_ReactiveArmorSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button16:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW07_WeakenSpell == 0 ) { m_Scroll.mW07_WeakenSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW07_WeakenSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
        ///        
                case (int)Buttons.Button17:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW08_AgilitySpell == 0 ) { m_Scroll.mW08_AgilitySpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW08_AgilitySpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button18:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW09_CunningSpell == 0 ) { m_Scroll.mW09_CunningSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW09_CunningSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button19:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW10_CureSpell == 0 ) { m_Scroll.mW10_CureSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW10_CureSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button20:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW11_HarmSpell == 0 ) { m_Scroll.mW11_HarmSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW11_HarmSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button21:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW12_MagicTrapSpell == 0 ) { m_Scroll.mW12_MagicTrapSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW12_MagicTrapSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button22:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW13_RemoveTrapSpell == 0 ) { m_Scroll.mW13_RemoveTrapSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW13_RemoveTrapSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button23:
                {
                    int page = 0;
                    page = 1;
                   if ( m_Scroll.mW14_ProtectionSpell == 0 ) { m_Scroll.mW14_ProtectionSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW14_ProtectionSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button24:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW15_StrengthSpell == 0 ) { m_Scroll.mW15_StrengthSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW15_StrengthSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
        // row 2
                case (int)Buttons.Button25://
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW16_BlessSpell == 0 ) { m_Scroll.mW16_BlessSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW16_BlessSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button26://
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW17_FireballSpell == 0 ) { m_Scroll.mW17_FireballSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW17_FireballSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button27://
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW18_MagicLockSpell == 0 ) { m_Scroll.mW18_MagicLockSpell = 1; m_Scroll.mCount += 1; }
					else { m_Scroll.mW18_MagicLockSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button28://
                {
                    int page = 0;
                    page = 1;
                        if ( m_Scroll.mW19_PoisonSpell == 0 ) { m_Scroll.mW19_PoisonSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW19_PoisonSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button29://
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW20_TelekinesisSpell == 0 ) { m_Scroll.mW20_TelekinesisSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW20_TelekinesisSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button30://
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW21_TeleportSpell == 0 ) { m_Scroll.mW21_TeleportSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW21_TeleportSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button31://
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW22_UnlockSpell == 0 ) { m_Scroll.mW22_UnlockSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW22_UnlockSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button32://
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW23_WallOfStoneSpell == 0 ) { m_Scroll.mW23_WallOfStoneSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW23_WallOfStoneSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button33://
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW24_ArchCureSpell == 0 ) { m_Scroll.mW24_ArchCureSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW24_ArchCureSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button34://
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW25_ArchProtectionSpell == 0 ) { m_Scroll.mW25_ArchProtectionSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW25_ArchProtectionSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button35://
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW26_CurseSpell == 0 ) { m_Scroll.mW26_CurseSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW26_CurseSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button36://
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW27_FireFieldSpell == 0 ) { m_Scroll.mW27_FireFieldSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW27_FireFieldSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button37://
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW28_GreaterHealSpell == 0 ) { m_Scroll.mW28_GreaterHealSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW28_GreaterHealSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button38://
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW29_LightningSpell == 0 ) { m_Scroll.mW29_LightningSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW29_LightningSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button39://
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW30_ManaDrainSpell == 0 ) { m_Scroll.mW30_ManaDrainSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW30_ManaDrainSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button40://
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW31_RecallSpell == 0 ) { m_Scroll.mW31_RecallSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW31_RecallSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page, m_Scroll ) );
                    break;
                }
    /// 3rd row
                case (int)Buttons.Button41:///
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW32_BladeSpiritsSpell == 0 ) { m_Scroll.mW32_BladeSpiritsSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW32_BladeSpiritsSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button42:///
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW33_DispelFieldSpell == 0 ) { m_Scroll.mW33_DispelFieldSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW33_DispelFieldSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button43:///
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW34_IncognitoSpell == 0 ) { m_Scroll.mW34_IncognitoSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW34_IncognitoSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button44:///
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW35_MagicReflectSpell == 0 ) { m_Scroll.mW35_MagicReflectSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW35_MagicReflectSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button45:///
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW36_MindBlastSpell == 0 ) { m_Scroll.mW36_MindBlastSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW36_MindBlastSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button46:///
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW37_ParalyzeSpell == 0 ) { m_Scroll.mW37_ParalyzeSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW37_ParalyzeSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button47:///
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW38_PoisonFieldSpell == 0 ) { m_Scroll.mW38_PoisonFieldSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW38_PoisonFieldSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button48:///
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW39_SummonCreatureSpell == 0 ) { m_Scroll.mW39_SummonCreatureSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW39_SummonCreatureSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button49:///
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW40_DispelSpell == 0 ) { m_Scroll.mW40_DispelSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW40_DispelSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button50:///
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW41_EnergyBoltSpell == 0 ) { m_Scroll.mW41_EnergyBoltSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW41_EnergyBoltSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                break;
                 
                }
                case (int)Buttons.Button51:///
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW42_ExplosionSpell == 0 ) { m_Scroll.mW42_ExplosionSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW42_ExplosionSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button52:///
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW43_InvisibilitySpell == 0 ) { m_Scroll.mW43_InvisibilitySpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW43_InvisibilitySpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button53:///
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW44_MarkSpell == 0 ) { m_Scroll.mW44_MarkSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW44_MarkSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button54:///
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW45_MassCurseSpell == 0 ) { m_Scroll.mW45_MassCurseSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW45_MassCurseSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button55:///
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW46_ParalyzeFieldSpell == 0 ) { m_Scroll.mW46_ParalyzeFieldSpell = 1; m_Scroll.mCount += 1; }
                else { m_Scroll.mW46_ParalyzeFieldSpell = 0; m_Scroll.mCount -= 1; }
                from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button56:///
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW47_RevealSpell == 0 ) { m_Scroll.mW47_RevealSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW47_RevealSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
               
    ///3rd row end
                case (int)Buttons.Button57:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW48_ChainLightningSpell == 0 ) { m_Scroll.mW48_ChainLightningSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW48_ChainLightningSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
               
                case (int)Buttons.Button58:///
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW49_EnergyFieldSpell == 0 ) { m_Scroll.mW49_EnergyFieldSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW49_EnergyFieldSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button59:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW50_FlameStrikeSpell == 0 ) { m_Scroll.mW50_FlameStrikeSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW50_FlameStrikeSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button60:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW51_GateTravelSpell == 0 ) { m_Scroll.mW51_GateTravelSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW51_GateTravelSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button61:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW52_ManaVampireSpell == 0 ) { m_Scroll.mW52_ManaVampireSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW52_ManaVampireSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button62:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW53_MassDispelSpell == 0 ) { m_Scroll.mW53_MassDispelSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW53_MassDispelSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button63:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW54_MeteorSwarmSpell == 0 ) { m_Scroll.mW54_MeteorSwarmSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW54_MeteorSwarmSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button64:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW55_PolymorphSpell == 0 ) { m_Scroll.mW55_PolymorphSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW55_PolymorphSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button65:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW56_EarthquakeSpell == 0 ) { m_Scroll.mW56_EarthquakeSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW56_EarthquakeSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button66:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW57_EnergyVortexSpell == 0 ) { m_Scroll.mW57_EnergyVortexSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW57_EnergyVortexSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button67:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW58_ResurrectionSpell == 0 ) { m_Scroll.mW58_ResurrectionSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW58_ResurrectionSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button68:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW59_AirElementalSpell == 0 ) { m_Scroll.mW59_AirElementalSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW59_AirElementalSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button69:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW60_SummonDaemonSpell == 0 ) { m_Scroll.mW60_SummonDaemonSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW60_SummonDaemonSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button70:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW61_EarthElementalSpell == 0 ) { m_Scroll.mW61_EarthElementalSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW61_EarthElementalSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button71:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW62_FireElementalSpell == 0 ) { m_Scroll.mW62_FireElementalSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW62_FireElementalSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
                case (int)Buttons.Button72:
                {
                    int page = 0;
                    page = 1;
                    if ( m_Scroll.mW63_WaterElementalSpell == 0 ) { m_Scroll.mW63_WaterElementalSpell = 1; m_Scroll.mCount += 1; }
                    else { m_Scroll.mW63_WaterElementalSpell = 0; m_Scroll.mCount -= 1; }
                    from.SendGump( new SpellBarGump( from, page,  m_Scroll ) );
                    break;
                }
				
	/// necromancy
                case (int)Buttons.Button73 : { int page = 0;
                    page = 2; { if ( m_Scroll.mN01AnimateDeadSpell == 0 ) { m_Scroll.mN01AnimateDeadSpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mN01AnimateDeadSpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page, m_Scroll ) ); break; } }
                case (int)Buttons.Button74 : {int page = 0;
                    page = 2;  { if ( m_Scroll.mN02BloodOathSpell == 0 ) { m_Scroll.mN02BloodOathSpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mN02BloodOathSpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page, m_Scroll ) ); break; } }
                case (int)Buttons.Button75 : {int page = 0;
                    page = 2;  { if ( m_Scroll.mN03CorpseSkinSpell == 0 ) { m_Scroll.mN03CorpseSkinSpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mN03CorpseSkinSpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page, m_Scroll ) ); break; } }
                case (int)Buttons.Button76 : {int page = 0;
                    page = 2;  { if ( m_Scroll.mN04CurseWeaponSpell == 0 ) { m_Scroll.mN04CurseWeaponSpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mN04CurseWeaponSpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page, m_Scroll ) ); break; }}
                case (int)Buttons.Button77 : {int page = 0;
                    page = 2;  { if ( m_Scroll.mN05EvilOmenSpell == 0 ) { m_Scroll.mN05EvilOmenSpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mN05EvilOmenSpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page, m_Scroll ) ); break; } }
                case (int)Buttons.Button78 : { int page = 0;
                    page = 2;  { if ( m_Scroll.mN06HorrificBeastSpell == 0 ) { m_Scroll.mN06HorrificBeastSpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mN06HorrificBeastSpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page, m_Scroll ) ); break; } }
                case (int)Buttons.Button79 : {int page = 0;
                    page = 2;  { if ( m_Scroll.mN07LichFormSpell == 0 ) { m_Scroll.mN07LichFormSpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mN07LichFormSpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page, m_Scroll ) ); break; } }
                case (int)Buttons.Button80 : {int page = 0;
                    page = 2;  { if ( m_Scroll.mN08MindRotSpell == 0 ) { m_Scroll.mN08MindRotSpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mN08MindRotSpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page, m_Scroll ) ); break; }}
                case (int)Buttons.Button81 : {int page = 0;
                    page = 2;  { if ( m_Scroll.mN09PainSpikeSpell == 0 ) { m_Scroll.mN09PainSpikeSpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mN09PainSpikeSpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page, m_Scroll ) ); break; }}
                case (int)Buttons.Button82 :{ int page = 0;
                    page = 2;  { if ( m_Scroll.mN10PoisonStrikeSpell == 0 ) { m_Scroll.mN10PoisonStrikeSpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mN10PoisonStrikeSpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page, m_Scroll ) ); break; }}
                case (int)Buttons.Button83 : {int page = 0;
                    page = 2;  { if ( m_Scroll.mN11StrangleSpell == 0 ) { m_Scroll.mN11StrangleSpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mN11StrangleSpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page, m_Scroll ) ); break; }}
                case (int)Buttons.Button84 : {int page = 0;
                    page = 2;  { if ( m_Scroll.mN12SummonFamiliarSpell == 0 ) { m_Scroll.mN12SummonFamiliarSpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mN12SummonFamiliarSpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page, m_Scroll ) ); break; }}
                case (int)Buttons.Button85 : {int page = 0;
                    page = 2;  { if ( m_Scroll.mN13VampiricEmbraceSpell == 0 ) { m_Scroll.mN13VampiricEmbraceSpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mN13VampiricEmbraceSpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page, m_Scroll ) ); break; }}
                case (int)Buttons.Button86 : {int page = 0;
                    page = 2;  { if ( m_Scroll.mN14VengefulSpiritSpell == 0 ) { m_Scroll.mN14VengefulSpiritSpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mN14VengefulSpiritSpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page, m_Scroll ) ); break; }}
                case (int)Buttons.Button87 : {int page = 0;
                    page = 2;  { if ( m_Scroll.mN15WitherSpell == 0 ) { m_Scroll.mN15WitherSpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mN15WitherSpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page, m_Scroll ) ); break; }}
                case (int)Buttons.Button88 : {int page = 0;
                    page = 2;  { if ( m_Scroll.mN16WraithFormSpell == 0 ) { m_Scroll.mN16WraithFormSpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mN16WraithFormSpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page, m_Scroll ) ); break; }    }
               
				case (int)Buttons.Button89 : {int page = 0;
                    page = 2;  { if ( m_Scroll.mN17ExorcismSpell == 0 ) { m_Scroll.mN17ExorcismSpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mN17ExorcismSpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page, m_Scroll ) ); break; }    }

    // CHIVALRY
                
				case (int)Buttons.Button90 : { int page = 0;
                    page = 3;  { if ( m_Scroll.mC01CleanseByFireSpell == 0 ) { m_Scroll.mC01CleanseByFireSpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mC01CleanseByFireSpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons.Button91 : { int page = 0;
                    page = 3;  { if ( m_Scroll.mC02CloseWoundsSpell == 0 ) { m_Scroll.mC02CloseWoundsSpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mC02CloseWoundsSpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ) ;break; } }
                case (int)Buttons.Button92 : { int page = 0;
                    page = 3;  { if ( m_Scroll.mC03ConsecrateWeaponSpell == 0 ) { m_Scroll.mC03ConsecrateWeaponSpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mC03ConsecrateWeaponSpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons.Button93 : { int page = 0;
                    page = 3;  { if ( m_Scroll.mC04DispelEvilSpell == 0 ) { m_Scroll.mC04DispelEvilSpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mC04DispelEvilSpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons.Button94 : { int page = 0;
                    page = 3;   { if ( m_Scroll.mC05DivineFurySpell == 0 ) { m_Scroll.mC05DivineFurySpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mC05DivineFurySpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons.Button95 : { int page = 0;
                    page = 3;   { if ( m_Scroll.mC06EnemyOfOneSpell == 0 ) { m_Scroll.mC06EnemyOfOneSpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mC06EnemyOfOneSpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons.Button96 : { int page = 0;
                    page = 3;   { if ( m_Scroll.mC07HolyLightSpell == 0 ) { m_Scroll.mC07HolyLightSpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mC07HolyLightSpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons.Button97 : { int page = 0;
                    page = 3;   { if ( m_Scroll.mC08NobleSacrificeSpell == 0 ) { m_Scroll.mC08NobleSacrificeSpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mC08NobleSacrificeSpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons.Button98 : { int page = 0;
                    page = 3;   { if ( m_Scroll.mC09RemoveCurseSpell == 0 ) { m_Scroll.mC09RemoveCurseSpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mC09RemoveCurseSpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons.Button99 : { int page = 0;
                    page = 3;   { if ( m_Scroll.mC10SacredJourneySpell == 0 ) { m_Scroll.mC10SacredJourneySpell = 1; m_Scroll.mCount += 1; } else { m_Scroll.mC10SacredJourneySpell = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
// BUSHIDO

				case (int)Buttons.Button100 : {int page = 0;
                    page = 4;  { if ( m_Scroll.mB01Confidence == 0 ) { m_Scroll.mB01Confidence = 1; m_Scroll.mCount += 1; } else { m_Scroll.mB01Confidence = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page, m_Scroll ) ); break; }    }

				case (int)Buttons.Button101 : {int page = 0;
                    page = 4;  { if ( m_Scroll.mB02CounterAttack == 0 ) { m_Scroll.mB02CounterAttack = 1; m_Scroll.mCount += 1; } else { m_Scroll.mB02CounterAttack = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page, m_Scroll ) ); break; }    }

				case (int)Buttons.Button102 : {int page = 0;
                    page = 4;  { if ( m_Scroll.mB03Evasion == 0 ) { m_Scroll.mB03Evasion = 1; m_Scroll.mCount += 1; } else { m_Scroll.mB03Evasion = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page, m_Scroll ) ); break; }    }

				case (int)Buttons.Button103 : {int page = 0;
                    page = 4;  { if ( m_Scroll.mB04LightningStrike == 0 ) { m_Scroll.mB04LightningStrike = 1; m_Scroll.mCount += 1; } else { m_Scroll.mB04LightningStrike = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page, m_Scroll ) ); break; }    }

				case (int)Buttons.Button104 : {int page = 0;
                    page = 4;  { if ( m_Scroll.mB05MomentumStrike == 0 ) { m_Scroll.mB05MomentumStrike = 1; m_Scroll.mCount += 1; } else { m_Scroll.mB05MomentumStrike = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page, m_Scroll ) ); break; }    }
			/*		
				case (int)Buttons.Button144 : 
				{
					int page = 0;
                    page = 4;  
					{ 
						if ( m_Scroll.mB06HonorableExecution == 0 ) 
						{ 
							m_Scroll.mB06HonorableExecution = 1; m_Scroll.mCount += 1; 
						} 
						else 
						{ 
							m_Scroll.mB06HonorableExecution = 0; 
						} 
						from.SendGump( new SpellBarGump( from, page, m_Scroll ) ); 
						break; 
					}    
				}
			*/		
					
// NINJITSU
                case (int)Buttons.Button105 : 
				{ 
					int page = 0;
                    page = 5;   
					{ 
						if ( m_Scroll.mI01DeathStrike == 0 ) 
						{ 
							m_Scroll.mI01DeathStrike = 1; 
							m_Scroll.mCount += 1; 
						} 
						else 
						{ 
							m_Scroll.mI01DeathStrike = 0;
							m_Scroll.mCount -= 1;
						} 
						from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); 
						break; 
					} 
				}
                case (int)Buttons. Button106: { int page = 0;
                    page = 5;   { if ( m_Scroll.mI02AnimalForm == 0 ) { m_Scroll.mI02AnimalForm = 1; m_Scroll.mCount += 1; } else { m_Scroll.mI02AnimalForm = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons. Button107: { int page = 0;
                    page = 5;   { if ( m_Scroll.mI03KiAttack == 0 ) { m_Scroll.mI03KiAttack = 1; m_Scroll.mCount += 1; } else { m_Scroll.mI03KiAttack = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons. Button108: { int page = 0;
                    page = 5;   { if ( m_Scroll.mI04SurpriseAttack == 0 ) { m_Scroll.mI04SurpriseAttack = 1; m_Scroll.mCount += 1; } else { m_Scroll.mI04SurpriseAttack = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons. Button109: { int page = 0;
                    page = 5;   { if ( m_Scroll.mI05Backstab == 0 ) { m_Scroll.mI05Backstab = 1; m_Scroll.mCount += 1; } else { m_Scroll.mI05Backstab = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons.Button110: { int page = 0;
                    page = 5;   { if ( m_Scroll.mI06Shadowjump == 0 ) { m_Scroll.mI06Shadowjump = 1; m_Scroll.mCount += 1; } else { m_Scroll.mI06Shadowjump = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons.Button111: { int page = 0;
                    page = 5;   { if ( m_Scroll.mI07MirrorImage == 0 ) { m_Scroll.mI07MirrorImage = 1; m_Scroll.mCount += 1; } else { m_Scroll.mI07MirrorImage = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
				/*
				case (int)Buttons. Button145: { int page = 0;
                    page = 5;   { if ( m_Scroll.mI08FocusAttack == 0 ) { m_Scroll.mI08FocusAttack = 1; m_Scroll.mCount += 1; } else { m_Scroll.mI08FocusAttack = 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
				*/
					
// SPELLWEAVING
                case (int)Buttons.Button112 : { int page = 0;
                    page = 6;   { if ( m_Scroll.mS01ArcaneCircleSpell== 0 ) { m_Scroll.mS01ArcaneCircleSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mS01ArcaneCircleSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons.Button113 : { int page = 0;
                    page = 6;   { if ( m_Scroll.mS02GiftOfRenewalSpell== 0 ) { m_Scroll.mS02GiftOfRenewalSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mS02GiftOfRenewalSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons.Button114 : { int page = 0;
                    page = 6;   { if ( m_Scroll.mS03ImmolatingWeaponSpell== 0 ) { m_Scroll.mS03ImmolatingWeaponSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mS03ImmolatingWeaponSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons.Button115 : { int page = 0;
                    page = 6;   { if ( m_Scroll.mS04AttuneWeaponSpell== 0 ) { m_Scroll.mS04AttuneWeaponSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mS04AttuneWeaponSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons.Button116 : { int page = 0;
                    page = 6;   { if ( m_Scroll.mS05ThunderstormSpell== 0 ) { m_Scroll.mS05ThunderstormSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mS05ThunderstormSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons.Button117 : { int page = 0;
                    page = 6;   { if ( m_Scroll.mS06NatureFurySpell== 0 ) { m_Scroll.mS06NatureFurySpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mS06NatureFurySpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons.Button118 : { int page = 0;
                    page = 6;   { if ( m_Scroll.mS07SummonFeySpell== 0 ) { m_Scroll.mS07SummonFeySpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mS07SummonFeySpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons.Button119 : { int page = 0;
                    page = 6;   { if ( m_Scroll.mS08SummonFiendSpell== 0 ) { m_Scroll.mS08SummonFiendSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mS08SummonFiendSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons.Button120 : { int page = 0;
                    page = 6;   { if ( m_Scroll.mS09ReaperFormSpell== 0 ) { m_Scroll.mS09ReaperFormSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mS09ReaperFormSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons.Button121 : { int page = 0;
                    page = 6;   { if ( m_Scroll.mS10WildfireSpell== 0 ) { m_Scroll.mS10WildfireSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mS10WildfireSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons.Button122 : { int page = 0;
                    page = 6;   { if ( m_Scroll.mS11EssenceOfWindSpell== 0 ) { m_Scroll.mS11EssenceOfWindSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mS11EssenceOfWindSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons.Button123 : { int page = 0;
                    page = 6;   { if ( m_Scroll.mS12DryadAllureSpell== 0 ) { m_Scroll.mS12DryadAllureSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mS12DryadAllureSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons.Button124 : { int page = 0;
                    page = 6;   { if ( m_Scroll.mS13EtherealVoyageSpell== 0 ) { m_Scroll.mS13EtherealVoyageSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mS13EtherealVoyageSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons.Button125 : { int page = 0;
                    page = 6;   { if ( m_Scroll.mS14WordOfDeathSpell== 0 ) { m_Scroll.mS14WordOfDeathSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mS14WordOfDeathSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons.Button126 : { int page = 0;
                    page = 6;   { if ( m_Scroll.mS15GiftOfLifeSpell== 0 ) { m_Scroll.mS15GiftOfLifeSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mS15GiftOfLifeSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
                case (int)Buttons.Button127 : { int page = 0;
                    page = 6;   { if ( m_Scroll.mS16ArcaneEmpowermentSpell== 0 ) { m_Scroll.mS16ArcaneEmpowermentSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mS16ArcaneEmpowermentSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
				
			// MYSTICISM
				case (int)Buttons.Button128 : { int page = 0;
                    page = 7;   { if ( m_Scroll.mM01NetherBoltSpell== 0 ) { m_Scroll.mM01NetherBoltSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mM01NetherBoltSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
				case (int)Buttons.Button129 : { int page = 0;
                    page = 7;   { if ( m_Scroll.mM02HealingStoneSpell== 0 ) { m_Scroll.mM02HealingStoneSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mM02HealingStoneSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
				case (int)Buttons.Button130 : { int page = 0;
                    page = 7;   { if ( m_Scroll.mM03PurgeMagicSpell== 0 ) { m_Scroll.mM03PurgeMagicSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mM03PurgeMagicSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
				case (int)Buttons.Button131 : { int page = 0;
                    page = 7;   { if ( m_Scroll.mM04EnchantSpell== 0 ) { m_Scroll.mM04EnchantSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mM04EnchantSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
				case (int)Buttons.Button132 : { int page = 0;
                    page = 7;   { if ( m_Scroll.mM05SleepSpell== 0 ) { m_Scroll.mM05SleepSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mM05SleepSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
				case (int)Buttons.Button133 : { int page = 0;
                    page = 7;   { if ( m_Scroll.mM06EagleStrikeSpell== 0 ) { m_Scroll.mM06EagleStrikeSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mM06EagleStrikeSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
				case (int)Buttons.Button134 : { int page = 0;
                    page = 7;   { if ( m_Scroll.mM07AnimatedWeaponSpell== 0 ) { m_Scroll.mM07AnimatedWeaponSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mM07AnimatedWeaponSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
				case (int)Buttons.Button135 : { int page = 0;
                    page = 7;   { if ( m_Scroll.mM08SpellTriggerSpell== 0 ) { m_Scroll.mM08SpellTriggerSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mM08SpellTriggerSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
				case (int)Buttons.Button136 : { int page = 0;
                    page = 7;   { if ( m_Scroll.mM09MassSleepSpell== 0 ) { m_Scroll.mM09MassSleepSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mM09MassSleepSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
				case (int)Buttons.Button137 : { int page = 0;
                    page = 7;   { if ( m_Scroll.mM10CleansingWindsSpell== 0 ) { m_Scroll.mM10CleansingWindsSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mM10CleansingWindsSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
				case (int)Buttons.Button138 : { int page = 0;
                    page = 7;   { if ( m_Scroll.mM11BombardSpell== 0 ) { m_Scroll.mM11BombardSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mM11BombardSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
				case (int)Buttons.Button139 : { int page = 0;
                    page = 7;   { if ( m_Scroll.mM12SpellPlagueSpell== 0 ) { m_Scroll.mM12SpellPlagueSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mM12SpellPlagueSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
				case (int)Buttons.Button140 : { int page = 0;
                    page = 7;   { if ( m_Scroll.mM13HailStormSpell== 0 ) { m_Scroll.mM13HailStormSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mM13HailStormSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
				case (int)Buttons.Button141 : { int page = 0;
                    page = 7;   { if ( m_Scroll.mM14NetherCycloneSpell== 0 ) { m_Scroll.mM14NetherCycloneSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mM14NetherCycloneSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
				case (int)Buttons.Button142 : { int page = 0;
                    page = 7;   { if ( m_Scroll.mM15RisingColossusSpell== 0 ) { m_Scroll.mM15RisingColossusSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mM15RisingColossusSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
				case (int)Buttons.Button143 : { int page = 0;
                    page = 7;   { if ( m_Scroll.mM16StoneFormSpell== 0 ) { m_Scroll.mM16StoneFormSpell= 1; m_Scroll.mCount += 1; } else { m_Scroll.mM16StoneFormSpell= 0; m_Scroll.mCount -= 1; } from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; } }
					
			//bush		
				case (int)Buttons.Button144 : 
				{
					int page = 0;
                    page = 4;  
					{ 
						if ( m_Scroll.mB06HonorableExecution == 0 ) 
						{ 
							m_Scroll.mB06HonorableExecution = 1; 
							m_Scroll.mCount += 1; 
						} 
						else 
						{ 
							m_Scroll.mB06HonorableExecution = 0; 
							m_Scroll.mCount -= 1;
						} 
						from.SendGump( new SpellBarGump( from, page, m_Scroll ) ); 
						break; 
					}    
				}
			// ninjitsu
				case (int)Buttons. Button145: 
					{ 
						int page = 0;
						page = 5;   
						{ 
							if ( m_Scroll.mI08FocusAttack == 0 ) 
							{ 
								m_Scroll.mI08FocusAttack = 1; m_Scroll.mCount += 1; 
							} 
							else 
							{ 
								m_Scroll.mI08FocusAttack = 0; m_Scroll.mCount -= 1; 
							} 
							
							from.SendGump( new SpellBarGump( from, page ,m_Scroll ) ); break; 
						} 
					}
		
		/// help button
				case (int)Buttons.Button146:
                {
                    int page = 0;
                    page = 8;
					from.SendGump( new SpellBarGump( from, page ,m_Scroll ) );
                    //from.SendGump(new SpellBar_HelpGump( from ));
                    break;
                }
				
				case (int)Buttons.Button147: // reset
                {
					int page = 0;
                    page = 9;
					
					if ( m_Scroll.mW00_ClumsySpell == 1 ) { m_Scroll.mW00_ClumsySpell = 0; }
					if ( m_Scroll.mW01_CreateFoodSpell == 1 ) { m_Scroll.mW01_CreateFoodSpell = 0; }
					if ( m_Scroll.mW02_FeeblemindSpell == 1 ) { m_Scroll.mW02_FeeblemindSpell = 0; }
					if ( m_Scroll.mW03_HealSpell == 1 ) { m_Scroll.mW03_HealSpell = 0;}
					if ( m_Scroll.mW04_MagicArrowSpell == 1 ) { m_Scroll.mW04_MagicArrowSpell = 0;}
					if ( m_Scroll.mW05_NightSightSpell == 1 ) { m_Scroll.mW05_NightSightSpell = 0;}
					if ( m_Scroll.mW06_ReactiveArmorSpell == 1 ) { m_Scroll.mW06_ReactiveArmorSpell = 0;}
					if ( m_Scroll.mW07_WeakenSpell == 1 ) { m_Scroll.mW07_WeakenSpell = 0;}
					if ( m_Scroll.mW08_AgilitySpell == 1 ) { m_Scroll.mW08_AgilitySpell = 0;}
					if ( m_Scroll.mW09_CunningSpell == 1 ) { m_Scroll.mW09_CunningSpell = 0;}
					if ( m_Scroll.mW10_CureSpell == 1 ) { m_Scroll.mW10_CureSpell = 0;}
					if ( m_Scroll.mW11_HarmSpell == 1 ) { m_Scroll.mW11_HarmSpell = 0;}
					if ( m_Scroll.mW12_MagicTrapSpell == 1 ) { m_Scroll.mW12_MagicTrapSpell = 0;}
					if ( m_Scroll.mW13_RemoveTrapSpell == 1 ) { m_Scroll.mW13_RemoveTrapSpell = 0;}
					if ( m_Scroll.mW14_ProtectionSpell == 1 ) { m_Scroll.mW14_ProtectionSpell = 0; }
					if ( m_Scroll.mW15_StrengthSpell == 1 ) { m_Scroll.mW15_StrengthSpell = 0;}
					if ( m_Scroll.mW16_BlessSpell == 1 ) { m_Scroll.mW16_BlessSpell = 0; }
					if ( m_Scroll.mW17_FireballSpell == 1 ) { m_Scroll.mW17_FireballSpell = 0; }
					if ( m_Scroll.mW18_MagicLockSpell == 1 ) { m_Scroll.mW18_MagicLockSpell = 0; }
					if ( m_Scroll.mW19_PoisonSpell == 1 ) { m_Scroll.mW19_PoisonSpell = 0; }
					if ( m_Scroll.mW20_TelekinesisSpell == 1 ) { m_Scroll.mW20_TelekinesisSpell = 0; }
					if ( m_Scroll.mW21_TeleportSpell == 1 ) { m_Scroll.mW21_TeleportSpell = 0; }
					if ( m_Scroll.mW22_UnlockSpell == 1 ) { m_Scroll.mW22_UnlockSpell = 0;}
					if ( m_Scroll.mW23_WallOfStoneSpell == 1 ) { m_Scroll.mW23_WallOfStoneSpell = 0; }
					if ( m_Scroll.mW24_ArchCureSpell == 1 ) { m_Scroll.mW24_ArchCureSpell = 0; }
					if ( m_Scroll.mW25_ArchProtectionSpell == 1 ) { m_Scroll.mW25_ArchProtectionSpell = 0; }
					if ( m_Scroll.mW26_CurseSpell == 1 ) { m_Scroll.mW26_CurseSpell = 0; }
					if ( m_Scroll.mW27_FireFieldSpell == 1 ) { m_Scroll.mW27_FireFieldSpell = 0; m_Scroll.mCount += 0; }
					if ( m_Scroll.mW28_GreaterHealSpell == 1 ) { m_Scroll.mW28_GreaterHealSpell = 0; }
					if ( m_Scroll.mW29_LightningSpell == 1 ) { m_Scroll.mW29_LightningSpell = 0; }
					if ( m_Scroll.mW30_ManaDrainSpell == 1 ) { m_Scroll.mW30_ManaDrainSpell = 0;}
					if ( m_Scroll.mW31_RecallSpell == 1 ) { m_Scroll.mW31_RecallSpell = 0; }
					if ( m_Scroll.mW32_BladeSpiritsSpell == 1 ) { m_Scroll.mW32_BladeSpiritsSpell = 0; }
					if ( m_Scroll.mW33_DispelFieldSpell == 1 ) { m_Scroll.mW33_DispelFieldSpell = 0; }
					if ( m_Scroll.mW34_IncognitoSpell == 1 ) { m_Scroll.mW34_IncognitoSpell = 0; }
					if ( m_Scroll.mW35_MagicReflectSpell == 1 ) { m_Scroll.mW35_MagicReflectSpell = 0; }
					if ( m_Scroll.mW36_MindBlastSpell == 1 ) { m_Scroll.mW36_MindBlastSpell = 0; }
					if ( m_Scroll.mW37_ParalyzeSpell == 1 ) { m_Scroll.mW37_ParalyzeSpell = 0; }
					if ( m_Scroll.mW38_PoisonFieldSpell == 1 ) { m_Scroll.mW38_PoisonFieldSpell = 0; }
					if ( m_Scroll.mW39_SummonCreatureSpell == 1 ) { m_Scroll.mW39_SummonCreatureSpell = 0; }
					if ( m_Scroll.mW40_DispelSpell == 1 ) { m_Scroll.mW40_DispelSpell = 0; }
					if ( m_Scroll.mW41_EnergyBoltSpell == 1 ) { m_Scroll.mW41_EnergyBoltSpell = 0; }
					if ( m_Scroll.mW42_ExplosionSpell == 1 ) { m_Scroll.mW42_ExplosionSpell = 0; }
					if ( m_Scroll.mW43_InvisibilitySpell == 1 ) { m_Scroll.mW43_InvisibilitySpell = 0; }
					if ( m_Scroll.mW44_MarkSpell == 1 ) { m_Scroll.mW44_MarkSpell = 0;}
					if ( m_Scroll.mW45_MassCurseSpell == 1 ) { m_Scroll.mW45_MassCurseSpell = 0; }
					if ( m_Scroll.mW46_ParalyzeFieldSpell == 1 ) { m_Scroll.mW46_ParalyzeFieldSpell = 0; }
					if ( m_Scroll.mW47_RevealSpell == 1 ) { m_Scroll.mW47_RevealSpell = 0; }
					if ( m_Scroll.mW48_ChainLightningSpell == 1 ) { m_Scroll.mW48_ChainLightningSpell = 0; }
					if ( m_Scroll.mW49_EnergyFieldSpell == 1 ) { m_Scroll.mW49_EnergyFieldSpell = 0; }
					if ( m_Scroll.mW50_FlameStrikeSpell == 1 ) { m_Scroll.mW50_FlameStrikeSpell = 0; }
					if ( m_Scroll.mW51_GateTravelSpell == 1 ) { m_Scroll.mW51_GateTravelSpell = 0; }
					if ( m_Scroll.mW52_ManaVampireSpell == 1 ) { m_Scroll.mW52_ManaVampireSpell = 0; }
					if ( m_Scroll.mW53_MassDispelSpell == 1 ) { m_Scroll.mW53_MassDispelSpell = 0; }
					if ( m_Scroll.mW54_MeteorSwarmSpell == 1 ) { m_Scroll.mW54_MeteorSwarmSpell = 0; }
					if ( m_Scroll.mW55_PolymorphSpell == 1 ) { m_Scroll.mW55_PolymorphSpell = 0; }
					if ( m_Scroll.mW56_EarthquakeSpell == 1 ) { m_Scroll.mW56_EarthquakeSpell = 0; }
					if ( m_Scroll.mW57_EnergyVortexSpell == 1 ) { m_Scroll.mW57_EnergyVortexSpell = 0; }
					if ( m_Scroll.mW58_ResurrectionSpell == 1 ) { m_Scroll.mW58_ResurrectionSpell = 0; }
					if ( m_Scroll.mW59_AirElementalSpell == 1 ) { m_Scroll.mW59_AirElementalSpell = 0; }
					if ( m_Scroll.mW60_SummonDaemonSpell == 1 ) { m_Scroll.mW60_SummonDaemonSpell = 0; }
					if ( m_Scroll.mW61_EarthElementalSpell == 1 ) { m_Scroll.mW61_EarthElementalSpell = 0; }
					if ( m_Scroll.mW62_FireElementalSpell == 1 ) { m_Scroll.mW62_FireElementalSpell = 0; }
					if ( m_Scroll.mW63_WaterElementalSpell == 1 ) { m_Scroll.mW63_WaterElementalSpell = 0; }
	// necro
					if ( m_Scroll.mN01AnimateDeadSpell == 1 ) { m_Scroll.mN01AnimateDeadSpell = 0; }
					if ( m_Scroll.mN02BloodOathSpell == 1 ) { m_Scroll.mN02BloodOathSpell = 0;}
					if ( m_Scroll.mN03CorpseSkinSpell == 1 ) { m_Scroll.mN03CorpseSkinSpell = 0; }
					if ( m_Scroll.mN04CurseWeaponSpell == 1 ) { m_Scroll.mN04CurseWeaponSpell = 0; }
					if ( m_Scroll.mN05EvilOmenSpell == 1 ) { m_Scroll.mN05EvilOmenSpell = 0; }
					if ( m_Scroll.mN06HorrificBeastSpell == 1 ) { m_Scroll.mN06HorrificBeastSpell = 0; }
					if ( m_Scroll.mN07LichFormSpell == 1 ) { m_Scroll.mN07LichFormSpell = 0; }
					if ( m_Scroll.mN08MindRotSpell == 1 ) { m_Scroll.mN08MindRotSpell = 0; }
					if ( m_Scroll.mN09PainSpikeSpell == 1 ) { m_Scroll.mN09PainSpikeSpell = 0; }
					if ( m_Scroll.mN10PoisonStrikeSpell == 1 ) { m_Scroll.mN10PoisonStrikeSpell = 0; }
					if ( m_Scroll.mN11StrangleSpell == 1 ) { m_Scroll.mN11StrangleSpell = 0; }
					if ( m_Scroll.mN12SummonFamiliarSpell == 1 ) { m_Scroll.mN12SummonFamiliarSpell = 0; }
					if ( m_Scroll.mN13VampiricEmbraceSpell == 1 ) { m_Scroll.mN13VampiricEmbraceSpell = 0; }
					if ( m_Scroll.mN14VengefulSpiritSpell == 1 ) { m_Scroll.mN14VengefulSpiritSpell = 0; }
					if ( m_Scroll.mN15WitherSpell == 1 ) { m_Scroll.mN15WitherSpell = 0; }
					if ( m_Scroll.mN16WraithFormSpell == 1 ) { m_Scroll.mN16WraithFormSpell = 0; }
					if ( m_Scroll.mN17ExorcismSpell == 1 ) { m_Scroll.mN17ExorcismSpell = 0;}
	//CHIVALRY
					if ( m_Scroll.mC01CleanseByFireSpell == 1 ) { m_Scroll.mC01CleanseByFireSpell = 0;}
					if ( m_Scroll.mC02CloseWoundsSpell == 1 ) { m_Scroll.mC02CloseWoundsSpell = 0; }
					if ( m_Scroll.mC03ConsecrateWeaponSpell == 1 ) { m_Scroll.mC03ConsecrateWeaponSpell = 0; }
					if ( m_Scroll.mC04DispelEvilSpell == 1 ) { m_Scroll.mC04DispelEvilSpell = 0; }
					if ( m_Scroll.mC05DivineFurySpell == 1 ) { m_Scroll.mC05DivineFurySpell = 0; }
					if ( m_Scroll.mC06EnemyOfOneSpell == 1 ) { m_Scroll.mC06EnemyOfOneSpell = 0; }
					if ( m_Scroll.mC07HolyLightSpell == 1 ) { m_Scroll.mC07HolyLightSpell = 0;}
					if ( m_Scroll.mC08NobleSacrificeSpell == 1 ) { m_Scroll.mC08NobleSacrificeSpell = 0;}
					if ( m_Scroll.mC09RemoveCurseSpell == 1 ) { m_Scroll.mC09RemoveCurseSpell = 0; }
					if ( m_Scroll.mC10SacredJourneySpell == 1 ) { m_Scroll.mC10SacredJourneySpell = 0; }
	// BUSHIDO
					if ( m_Scroll.mB01Confidence == 1 ) { m_Scroll.mB01Confidence = 0; }
					if ( m_Scroll.mB02CounterAttack == 1 ) { m_Scroll.mB02CounterAttack = 0; }
					if ( m_Scroll.mB03Evasion == 1 ) { m_Scroll.mB03Evasion = 0; }
					if ( m_Scroll.mB04LightningStrike == 1 ) { m_Scroll.mB04LightningStrike = 0; }
					if ( m_Scroll.mB05MomentumStrike == 1 ) { m_Scroll.mB05MomentumStrike = 0; }
	// NINJITSU
					if ( m_Scroll.mI01DeathStrike == 1 ) { m_Scroll.mI01DeathStrike = 0; }
					if ( m_Scroll.mI02AnimalForm == 1 ) { m_Scroll.mI02AnimalForm = 0; }
					if ( m_Scroll.mI03KiAttack == 1 ) { m_Scroll.mI03KiAttack = 0;}
					if ( m_Scroll.mI04SurpriseAttack == 1 ) { m_Scroll.mI04SurpriseAttack = 0;}
					if ( m_Scroll.mI05Backstab == 1 ) { m_Scroll.mI05Backstab = 0; }
					if ( m_Scroll.mI06Shadowjump == 1 ) { m_Scroll.mI06Shadowjump = 0; }
					if ( m_Scroll.mI07MirrorImage == 1 ) { m_Scroll.mI07MirrorImage = 0; }
	// SPELLWEAVING
					if ( m_Scroll.mS01ArcaneCircleSpell== 1 ) { m_Scroll.mS01ArcaneCircleSpell= 0; }
					if ( m_Scroll.mS02GiftOfRenewalSpell== 1 ) { m_Scroll.mS02GiftOfRenewalSpell= 0; }
					if ( m_Scroll.mS03ImmolatingWeaponSpell== 1 ) { m_Scroll.mS03ImmolatingWeaponSpell= 0; }
					if ( m_Scroll.mS04AttuneWeaponSpell== 1 ) { m_Scroll.mS04AttuneWeaponSpell= 0; }
					if ( m_Scroll.mS05ThunderstormSpell== 1 ) { m_Scroll.mS05ThunderstormSpell= 0; }
					if ( m_Scroll.mS06NatureFurySpell== 1 ) { m_Scroll.mS06NatureFurySpell= 0; }
					if ( m_Scroll.mS07SummonFeySpell== 1 ) { m_Scroll.mS07SummonFeySpell= 0; }
					if ( m_Scroll.mS08SummonFiendSpell== 1 ) { m_Scroll.mS08SummonFiendSpell= 0; }
					if ( m_Scroll.mS09ReaperFormSpell== 1 ) { m_Scroll.mS09ReaperFormSpell= 0; }
					if ( m_Scroll.mS10WildfireSpell== 1 ) { m_Scroll.mS10WildfireSpell= 0; }
					if ( m_Scroll.mS11EssenceOfWindSpell== 1 ) { m_Scroll.mS11EssenceOfWindSpell= 0; }
					if ( m_Scroll.mS12DryadAllureSpell== 1 ) { m_Scroll.mS12DryadAllureSpell= 0; }
					if ( m_Scroll.mS13EtherealVoyageSpell== 1 ) { m_Scroll.mS13EtherealVoyageSpell= 0; }
					if ( m_Scroll.mS14WordOfDeathSpell== 1 ) { m_Scroll.mS14WordOfDeathSpell= 0; }
					if ( m_Scroll.mS15GiftOfLifeSpell== 1 ) { m_Scroll.mS15GiftOfLifeSpell= 0; }
					if ( m_Scroll.mS16ArcaneEmpowermentSpell== 1 ) { m_Scroll.mS16ArcaneEmpowermentSpell= 0; }
	// MYSTICISM
					if ( m_Scroll.mM01NetherBoltSpell== 1 ) { m_Scroll.mM01NetherBoltSpell= 0; }
					if ( m_Scroll.mM02HealingStoneSpell== 1 ) { m_Scroll.mM02HealingStoneSpell= 0; }
					if ( m_Scroll.mM03PurgeMagicSpell== 1 ) { m_Scroll.mM03PurgeMagicSpell= 0; }
					if ( m_Scroll.mM04EnchantSpell== 1 ) { m_Scroll.mM04EnchantSpell= 0; }
					if ( m_Scroll.mM05SleepSpell== 1 ) { m_Scroll.mM05SleepSpell= 0; }
					if ( m_Scroll.mM06EagleStrikeSpell== 1 ) { m_Scroll.mM06EagleStrikeSpell= 0; }
					if ( m_Scroll.mM07AnimatedWeaponSpell== 1 ) { m_Scroll.mM07AnimatedWeaponSpell= 0; }
					if ( m_Scroll.mM08SpellTriggerSpell== 1 ) { m_Scroll.mM08SpellTriggerSpell= 0; }
					if ( m_Scroll.mM09MassSleepSpell== 1 ) { m_Scroll.mM09MassSleepSpell= 0; }
					if ( m_Scroll.mM10CleansingWindsSpell== 1 ) { m_Scroll.mM10CleansingWindsSpell= 0; }
					if ( m_Scroll.mM11BombardSpell== 1 ) { m_Scroll.mM11BombardSpell= 0; }
					if ( m_Scroll.mM12SpellPlagueSpell== 1 ) { m_Scroll.mM12SpellPlagueSpell= 0; }
					if ( m_Scroll.mM13HailStormSpell== 1 ) { m_Scroll.mM13HailStormSpell= 0; }
					if ( m_Scroll.mM14NetherCycloneSpell== 1 ) { m_Scroll.mM14NetherCycloneSpell= 0; }
					if ( m_Scroll.mM15RisingColossusSpell== 1 ) { m_Scroll.mM15RisingColossusSpell= 0; }
					if ( m_Scroll.mM16StoneFormSpell== 0 ) { m_Scroll.mM16StoneFormSpell= 0; }
					
					
					
					
					m_Scroll.mCount = 0;
					from.SendGump( new SpellBarGump( from, page ,m_Scroll ) );
					break;
				}
				
				case (int)Buttons.Button148: // options
                {
					int page = 0;
                    page = 9;
					
					from.SendGump( new SpellBarGump( from, page ,m_Scroll ) );
					break;
				}
				case (int)Buttons.Button149: // 
                {
					int page = 0;
                    page = 9;
					
					
					if ( m_Scroll.mXselect_10 == 0) 
					{ 
						m_Scroll.mXselect_10 = 1;
						m_Scroll.mXselect_15 = 0;
						m_Scroll.mXselect_20 = 0;
						m_Scroll.mXselect_30 = 0; 
						
					} 
					
					else 
					{ 
						m_Scroll.mXselect_10 = 0; 
					} 
					
							
					
					from.SendGump( new SpellBarGump( from, page ,m_Scroll ) );
					
					break;
				}
				
				
				case (int)Buttons.Button150: // 
                {
					int page = 0;
                    page = 9;
					
					
					if ( m_Scroll.mXselect_15 == 0) 
					{ 
						m_Scroll.mXselect_10 = 0;
						m_Scroll.mXselect_15 = 1;
						m_Scroll.mXselect_20 = 0;
						m_Scroll.mXselect_30 = 0; 
						
						
					} 
					
					else 
					{ 
						m_Scroll.mXselect_15 = 0; 
					} 
					
					from.SendGump( new SpellBarGump( from, page ,m_Scroll ) );
					
					break;
				}
				
				
				case (int)Buttons.Button151: // 
                {
					int page = 0;
                    page = 9;
					
					if ( m_Scroll.mXselect_20 == 0) 
					{ 
						m_Scroll.mXselect_10 = 0;
						m_Scroll.mXselect_15 = 0;
						m_Scroll.mXselect_20 = 1;
						m_Scroll.mXselect_30 = 0; 
					} 
					
					else 
					{ 
						m_Scroll.mXselect_20 = 0; 
					} 
					
					from.SendGump( new SpellBarGump( from, page ,m_Scroll ) );
					
					break;
				}
				
				
				case (int)Buttons.Button152: // 
                {
					int page = 0;
                    page = 9;
					
					if ( m_Scroll.mXselect_30 == 0) 
					{ 
						m_Scroll.mXselect_10 = 0;
						m_Scroll.mXselect_15 = 0;
						m_Scroll.mXselect_20 = 0;
						m_Scroll.mXselect_30 = 1; 
					} 
					
					else 
					{ 
						m_Scroll.mXselect_30 = 0; 
					} 
					
					from.SendGump( new SpellBarGump( from, page ,m_Scroll ) );
					
					break;
				}
				
	/////////////////
				case (int)Buttons.Button153: // 
                {
					int page = 0;
                    page = 9;
					int yselect_var = 0;
					
					if ( m_Scroll.mYselect_1 == 0) 
					{ 
						m_Scroll.mYselect_1 = 1;
						m_Scroll.mYselect_2 = 0;
						m_Scroll.mYselect_3 = 0;
						m_Scroll.mYselect_4 = 0; 
						
						yselect_var = 1;
					} 
					
					else 
					{ 
						m_Scroll.mYselect_1 = 0; 
					} 
					
					from.SendGump( new SpellBarGump( from, page ,m_Scroll ) );
					
					break;
				}
				case (int)Buttons.Button154: // 
                {
					int page = 0;
                    page = 9;
					int yselect_var = 0;
					
					if ( m_Scroll.mYselect_2 == 0) 
					{ 
						m_Scroll.mYselect_1 = 0;
						m_Scroll.mYselect_2 = 1;
						m_Scroll.mYselect_3 = 0;
						m_Scroll.mYselect_4 = 0; 
						
						yselect_var = 2;
					} 
					
					else 
					{ 
						m_Scroll.mYselect_2 = 0; 
					} 
					
					from.SendGump( new SpellBarGump( from, page ,m_Scroll ) );
					
					break;
				}
				case (int)Buttons.Button155: // 
                {
					int page = 0;
                    page = 9;
					int yselect_var = 0;
					
					if ( m_Scroll.mYselect_3 == 0) 
					{ 
						m_Scroll.mYselect_1 = 0;
						m_Scroll.mYselect_2 = 0;
						m_Scroll.mYselect_3 = 1;
						m_Scroll.mYselect_4 = 0; 
						
						yselect_var = 3;
					} 
					
					else 
					{ 
						m_Scroll.mYselect_3 = 0; 
					} 
					
					from.SendGump( new SpellBarGump( from, page ,m_Scroll ) );
					
					break;
				}
				case (int)Buttons.Button156: // 
                {
					int page = 0;
                    page = 9;
					int yselect_var = 0;
					
					if ( m_Scroll.mYselect_4 == 0) 
					{ 
						m_Scroll.mYselect_1 = 0;
						m_Scroll.mYselect_2 = 0;
						m_Scroll.mYselect_3 = 0;
						m_Scroll.mYselect_4 = 1; 
						
						yselect_var = 4;
					} 
					
					else 
					{ 
						m_Scroll.mYselect_4 = 0; 
					} 
					
					from.SendGump( new SpellBarGump( from, page ,m_Scroll ) );
					
					break;
				}
				case (int)Buttons.Button157: // 
                {
					int page = 0;
                    page = 0;
				
					from.SendGump( new SpellBarGump( from, page ,m_Scroll ) );
					
					break;
				}
				
			
            }
        }
        
/////////////////////////////
 
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
////******** spellbar_bargump  ************////
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//

////////////////////////////

    public class SpellBar_BarGump : Gump
    {
	
		
		
        public static bool HasSpell( Mobile from, int spellID )
        {
            Spellbook book = Spellbook.Find( from, spellID );
            return ( book != null && book.HasSpell( spellID ) );
        }

        private SpellBarScroll m_Scroll;
		private int m_Dbx;
		private int m_Dbxa;
		private int m_Dby;
		private int m_Dbya;
		public int mXselect_var;
		public int mYselect_var;
	
/*
		public void CalcPos ( )
		{
			int dbx = 0; //dbx = 50;
					int dbxa = 0; //dbxa = 45;
					int dby = 0; //dby = 5;
					int dbya = 0; //dbya = 0;
					int xselect_var = 0; //xselect_var = 0;
		
			if ( m_Scroll.mXselect_10 == 1) 
					{ xselect_var = 562; } 
				if ( m_Scroll.mXselect_15 == 1) 
					{ xselect_var = 787; } 
				if ( m_Scroll.mXselect_20 == 1) 
					{ xselect_var = 1012; } 
				if ( m_Scroll.mXselect_30 == 1) 
					{ xselect_var = 1462; }
					
				dbx = dbx + dbxa; dby = dby + dbya; 	
					dbx = 67; dbxa = 45; dby = 5; dbya = 0;
					
				
				
				if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; } 
				
				if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; } 
				
				
		}
	*/
		
		

        public SpellBar_BarGump( Mobile from, int dbx, int dbxa, int dby, int dbya, int xselect_var, SpellBarScroll scroll ) : base( 0, 0 )
        {
            m_Scroll = scroll;
			m_Dbx = dbx;
			m_Dbxa = dbxa;
			m_Dby = dby;
			m_Dbya = dbya;
			mXselect_var = xselect_var;
			//mYselect_var = yselect_var;
			
			
			
		
			

			if ( m_Scroll.mLock == 0 )
			{
				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;
			}
			else
			{
				this.Closable=false;
				this.Disposable=true;
				this.Dragable=false;
				this.Resizable=false;
			}
			
			
			
			this.AddPage(1);
			   
				
			if ( m_Scroll.mSwitch == 0 ) 
			{ 
				
				
				AddImage( 24, 0, 2234, 0);
				AddImageTiled( 0,0, 25,80, 2624 ); //options background
				AddAlphaRegion(0, 0, 25, 80);
				
					if ( m_Scroll.mLock == 0 )
					{
						dbx = 67; dbxa = 45; dby = 5; dbya = 0;

						this.AddButton( 2, 28,  22404, 22404, 138, GumpButtonType.Reply, 1); // flip button
						this.AddButton( 2, 5,  5603, 5603, 0, GumpButtonType.Page, 2); // minimize
						this.AddButton( 5, 54,  2510, 2510, 139, GumpButtonType.Reply, 1); // unlocked
					}
					else
					{
						this.AddButton( 5, 54,  2092, 2092, 139, GumpButtonType.Reply, 1); // locked
					}
			}
			else 
			{ 
					//m_Scroll.mSwitch = 1; m_Scroll.mCount += 1;
					dbx = 0; dbxa = 0; dby = 54; dbya = 45; 
					this.AddImage( 0, 0, 2234, 0);
					//this.AddBackground( 42,0, 47,51, 9270 ); //options background
					AddImageTiled( 42,0, 47,51, 2624 ); //options background
					AddAlphaRegion(42, 0, 47, 51);
					
					if ( m_Scroll.mLock == 0 )
					{
						this.AddButton( 48, 28,  22404, 22404, 138, GumpButtonType.Reply, 1); // flip button
						this.AddButton( 48, 7,  5600, 5600, 0, GumpButtonType.Page, 3); // minimize
						this.AddButton( 70, 16,  2510, 2510, 139, GumpButtonType.Reply, 1); // unlocked
					}
					else 
					{
						this.AddButton( 70, 16,  2092, 2092, 139, GumpButtonType.Reply, 1); // locked
					}
					
					
			}
			
	// magery 1015164 - 1015227, except for Circle # clilocs: 1015171 , 1015177 , 1015185 , 
	
	//1015193 , 1015202 ,

	//1015210 , 1015219
	// necro 1060509 - 1060525
	// chiv 1060493 - 1060502
	// bush 1060595 - 1060600
	// nin 1060610 - 1060617
	// sw 1031601 - 1031616
	// myst 1031678 - 1031693
            if ( HasSpell( from, 0 ) && m_Scroll.mW00_ClumsySpell == 1) 
			{ this.AddButton(dbx, dby, 2240, 2240, 1, GumpButtonType.Reply, 1); 
				dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } 
				
				if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  
				if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; } 
				if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  
				
				if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }
				
				AddTooltip( 1015164 ); 
			}
            if ( HasSpell( from, 1 ) && m_Scroll.mW01_CreateFoodSpell == 1)
			{
				this.AddButton(dbx, dby, 2241, 2241, 2, GumpButtonType.Reply, 1);
				
				dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }
				
				AddTooltip( 1015165 ); 
				
			}
			
            if ( HasSpell( from, 2 ) && m_Scroll.mW02_FeeblemindSpell == 1){ this.AddButton(dbx, dby, 2242, 2242, 3, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } } AddTooltip( 1015166 ); }
			
            if ( HasSpell( from, 3 ) && m_Scroll.mW03_HealSpell == 1){this.AddButton(dbx, dby, 2243, 2243, 4, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015167 ); }
			
            if ( HasSpell( from, 4 ) && m_Scroll.mW04_MagicArrowSpell == 1){this.AddButton(dbx, dby, 2244, 2244, 5, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015168 ); }
			
            if ( HasSpell( from, 5 ) && m_Scroll.mW05_NightSightSpell == 1){this.AddButton(dbx, dby, 2245, 2245, 6, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015169 ); }
			
            if ( HasSpell( from, 6 ) && m_Scroll.mW06_ReactiveArmorSpell == 1){this.AddButton(dbx, dby, 2246, 2246, 7, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015170 ); }
			
            if ( HasSpell( from, 7 ) && m_Scroll.mW07_WeakenSpell == 1){this.AddButton(dbx, dby, 2247, 2247, 8, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015172 );}
			
            if ( HasSpell( from, 8 ) && m_Scroll.mW08_AgilitySpell == 1)
			{this.AddButton(dbx, dby, 2248, 2248, 9, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }
				AddTooltip( 1015173 );
			}
			
            if ( HasSpell( from, 9 ) && m_Scroll.mW09_CunningSpell == 1)
			{
				this.AddButton(dbx, dby, 2249, 2249, 10, GumpButtonType.Reply, 1); 
				dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }
				   AddTooltip( 1015174 ); 
			}
				
            if ( HasSpell( from, 10 ) && m_Scroll.mW10_CureSpell == 1)
			{this.AddButton(dbx, dby, 2250, 2250, 11, GumpButtonType.Reply, 1);dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }
			 AddTooltip( 1015175 );
				} 
				
            if ( HasSpell( from, 11 ) && m_Scroll.mW11_HarmSpell == 1){this.AddButton(dbx, dby, 2251, 2251, 12, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }
				AddTooltip( 1015176 ); }
			
            if ( HasSpell( from, 12 ) && m_Scroll.mW12_MagicTrapSpell == 1){this.AddButton(dbx, dby, 2252, 2252, 13, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015178 );}
			
            if ( HasSpell( from, 13 ) && m_Scroll.mW13_RemoveTrapSpell == 1){this.AddButton(dbx, dby, 2253, 2253, 14, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015179 );}
			
            if ( HasSpell( from, 14 ) && m_Scroll.mW14_ProtectionSpell == 1){this.AddButton(dbx, dby, 2254, 2254, 15, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015180 );}
			
            if ( HasSpell( from, 15 ) && m_Scroll.mW15_StrengthSpell == 1){this.AddButton(dbx, dby, 2255, 2255, 16, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015181 );}
			
            if ( HasSpell( from, 16 ) && m_Scroll.mW16_BlessSpell == 1){this.AddButton(dbx, dby, 2256, 2256, 17, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015182 );}
			
            if ( HasSpell( from, 17 ) && m_Scroll.mW17_FireballSpell == 1){this.AddButton(dbx, dby, 2257, 2257, 18, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015183 );}
			
            if ( HasSpell( from, 18 ) && m_Scroll.mW18_MagicLockSpell == 1){this.AddButton(dbx, dby, 2258, 2258, 19, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015184 );}
			
            if ( HasSpell( from, 19 ) && m_Scroll.mW19_PoisonSpell == 1){this.AddButton(dbx, dby, 2259, 2259, 20, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015186 );}
			
            if ( HasSpell( from, 20 ) && m_Scroll.mW20_TelekinesisSpell == 1){this.AddButton(dbx, dby, 2260, 2260, 21, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015187 );}
			
            if ( HasSpell( from, 21 ) && m_Scroll.mW21_TeleportSpell == 1){this.AddButton(dbx, dby, 2261, 2261, 22, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015188 );}
			
            if ( HasSpell( from, 22 ) && m_Scroll.mW22_UnlockSpell == 1){this.AddButton(dbx, dby, 2262, 2262, 23, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015189 );}
			
            if ( HasSpell( from, 23 ) && m_Scroll.mW23_WallOfStoneSpell == 1){this.AddButton(dbx, dby, 2263, 2263, 24, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015190 ); }
			
            if ( HasSpell( from, 24 ) && m_Scroll.mW24_ArchCureSpell == 1){this.AddButton(dbx, dby, 2264, 2264, 25, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015191 ); }
			
            if ( HasSpell( from, 25 ) && m_Scroll.mW25_ArchProtectionSpell == 1){this.AddButton(dbx, dby, 2265, 2265, 26, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015192 ); }
			
            if ( HasSpell( from, 26 ) && m_Scroll.mW26_CurseSpell == 1){this.AddButton(dbx, dby, 2266, 2266, 27, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015194 ); }
			
            if ( HasSpell( from, 27 ) && m_Scroll.mW27_FireFieldSpell == 1){this.AddButton(dbx, dby, 2267, 2267, 28, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015195 ); }
			
            if ( HasSpell( from, 28 ) && m_Scroll.mW28_GreaterHealSpell == 1)
			{
				this.AddButton(dbx, dby, 2268, 2268, 29, GumpButtonType.Reply, 1); 
				dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }
				AddTooltip( 1015196 ); 
			}
			
            if ( HasSpell( from, 29 ) && m_Scroll.mW29_LightningSpell == 1){this.AddButton(dbx, dby, 2269, 2269, 30, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015197 ); }
			
            if ( HasSpell( from, 30 ) && m_Scroll.mW30_ManaDrainSpell == 1){this.AddButton(dbx, dby, 2270, 2270, 31, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015198 ); }
			
            if ( HasSpell( from, 31 ) && m_Scroll.mW31_RecallSpell == 1){this.AddButton(dbx, dby, 2271, 2271, 32, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015199 ); }
			
            if ( HasSpell( from, 32 ) && m_Scroll.mW32_BladeSpiritsSpell == 1){this.AddButton(dbx, dby, 2272, 2272, 33, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } } AddTooltip( 1015200 ); }
			
            if ( HasSpell( from, 33 ) && m_Scroll.mW33_DispelFieldSpell == 1){this.AddButton(dbx, dby, 2273, 2273, 34, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } } AddTooltip( 1015201 ); }
			
            if ( HasSpell( from, 34 ) && m_Scroll.mW34_IncognitoSpell == 1){this.AddButton(dbx, dby, 2274, 2274, 35, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015203 ); }
			
            if ( HasSpell( from, 35 ) && m_Scroll.mW35_MagicReflectSpell == 1){this.AddButton(dbx, dby, 2275, 2275, 36, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015204 ); }
			
            if ( HasSpell( from, 36 ) && m_Scroll.mW36_MindBlastSpell == 1){this.AddButton(dbx, dby, 2276, 2276, 37, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015205 ); }
			
            if ( HasSpell( from, 37 ) && m_Scroll.mW37_ParalyzeSpell == 1){this.AddButton(dbx, dby, 2277, 2277, 38, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015206 ); }
			
            if ( HasSpell( from, 38 ) && m_Scroll.mW38_PoisonFieldSpell == 1){this.AddButton(dbx, dby, 2278, 2278, 39, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015207 ); }
			
            if ( HasSpell( from, 39 ) && m_Scroll.mW39_SummonCreatureSpell == 1){this.AddButton(dbx, dby, 2279, 2279, 40, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015208 ); }
			
            if ( HasSpell( from, 40 ) && m_Scroll.mW40_DispelSpell == 1){this.AddButton(dbx, dby, 2280, 2280, 41, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015209 ); }
			
            if ( HasSpell( from, 41 ) && m_Scroll.mW41_EnergyBoltSpell == 1){this.AddButton(dbx, dby, 2281, 2281, 42, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015211 ); }
			
            if ( HasSpell( from, 42 ) && m_Scroll.mW42_ExplosionSpell == 1){this.AddButton(dbx, dby, 2282, 2282, 43, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015212 ); }
			
            if ( HasSpell( from, 43 ) && m_Scroll.mW43_InvisibilitySpell == 1){this.AddButton(dbx, dby, 2283, 2283, 44, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015213 ); }
			
            if ( HasSpell( from, 44 ) && m_Scroll.mW44_MarkSpell == 1){this.AddButton(dbx, dby, 2284, 2284, 45, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015214 ); }
			
            if ( HasSpell( from, 45 ) && m_Scroll.mW45_MassCurseSpell == 1){this.AddButton(dbx, dby, 2285, 2285, 46, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015215 ); }
			
            if ( HasSpell( from, 46 ) && m_Scroll.mW46_ParalyzeFieldSpell == 1){this.AddButton(dbx, dby, 2286, 2286, 47, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015216 ); }
			
            if ( HasSpell( from, 47 ) && m_Scroll.mW47_RevealSpell == 1){this.AddButton(dbx, dby, 2287, 2287, 48, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015217 ); }
			
			if ( HasSpell( from, 48 ) && m_Scroll.mW48_ChainLightningSpell == 1){this.AddButton(dbx, dby, 2287, 2287, 49, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015218 ); }
			
            if ( HasSpell( from, 49 ) && m_Scroll.mW49_EnergyFieldSpell == 1){this.AddButton(dbx, dby, 2289, 2289, 50, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip( 1015220 ); }
			
            if ( HasSpell( from, 50 ) && m_Scroll.mW50_FlameStrikeSpell == 1){this.AddButton(dbx, dby, 2290, 2290, 51, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip(1015221 );}
			
            if ( HasSpell( from, 51 ) && m_Scroll.mW51_GateTravelSpell == 1){this.AddButton(dbx, dby, 2291, 2291, 52, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip(1015222 );}
			
            if ( HasSpell( from, 52 ) && m_Scroll.mW52_ManaVampireSpell == 1){this.AddButton(dbx, dby, 2292, 2292, 53, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip(1015223 );}
			
            if ( HasSpell( from, 53 ) && m_Scroll.mW53_MassDispelSpell == 1){this.AddButton(dbx, dby, 2293, 2293, 54, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip(1015224 );}
			
            if ( HasSpell( from, 54 ) && m_Scroll.mW54_MeteorSwarmSpell == 1){this.AddButton(dbx, dby, 2294, 2294, 55, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip(1015225 );}
			
            if ( HasSpell( from, 55 ) && m_Scroll.mW55_PolymorphSpell == 1){this.AddButton(dbx, dby, 2295, 2295, 56, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip(1015226 );}
			
            if ( HasSpell( from, 56 ) && m_Scroll.mW56_EarthquakeSpell == 1){this.AddButton(dbx, dby, 2296, 2296, 57, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip(1015227 );}
			
            if ( HasSpell( from, 57 ) && m_Scroll.mW57_EnergyVortexSpell == 1){this.AddButton(dbx, dby, 2297, 2297, 58, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip(1015228 );}
			
            if ( HasSpell( from, 58 ) && m_Scroll.mW58_ResurrectionSpell == 1){this.AddButton(dbx, dby, 2298, 2298, 59, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip(1015229 );}
			
            if ( HasSpell( from, 59 ) && m_Scroll.mW59_AirElementalSpell == 1){this.AddButton(dbx, dby, 2299, 2299, 60, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }AddTooltip(1015230 );}
			
            if ( HasSpell( from, 60 ) && m_Scroll.mW60_SummonDaemonSpell == 1){this.AddButton(dbx, dby, 2300, 2300, 61, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 61 ) && m_Scroll.mW61_EarthElementalSpell == 1){this.AddButton(dbx, dby, 2301, 2301, 62, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 62 ) && m_Scroll.mW62_FireElementalSpell == 1){this.AddButton(dbx, dby, 2302, 2302, 63, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 63 ) && m_Scroll.mW63_WaterElementalSpell == 1){this.AddButton(dbx, dby, 2303, 2303, 64, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}

    // NECROMANCY

            if ( HasSpell( from, 100 ) && m_Scroll.mN01AnimateDeadSpell == 1) { this.AddButton(dbx, dby, 20480,20480, 65, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } } }
            if ( HasSpell( from, 101 ) && m_Scroll.mN02BloodOathSpell == 1){this.AddButton(dbx, dby, 20481,20481, 66, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 102 ) && m_Scroll.mN03CorpseSkinSpell == 1){this.AddButton(dbx, dby, 20482,20482, 67, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 103 ) && m_Scroll.mN04CurseWeaponSpell == 1){this.AddButton(dbx, dby, 20483 ,20483 , 68 , GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 104 ) && m_Scroll.mN05EvilOmenSpell == 1){this.AddButton(dbx, dby, 20484 ,20484 , 69 , GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 105 ) && m_Scroll.mN06HorrificBeastSpell == 1){this.AddButton(dbx, dby, 20485 ,20485 , 70 , GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 106 ) && m_Scroll.mN07LichFormSpell == 1){this.AddButton(dbx, dby, 20486 ,20486 , 71 , GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 107 ) && m_Scroll.mN08MindRotSpell == 1){this.AddButton(dbx, dby, 20487 ,20487 , 72 , GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 108 ) && m_Scroll.mN09PainSpikeSpell == 1){this.AddButton(dbx, dby, 20488 ,20488 , 73 , GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 109 ) && m_Scroll.mN10PoisonStrikeSpell == 1){this.AddButton(dbx, dby, 20489 ,20489 , 74 , GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 110 ) && m_Scroll.mN11StrangleSpell == 1){this.AddButton(dbx, dby, 20490 ,20490 , 75 , GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 111 ) && m_Scroll.mN12SummonFamiliarSpell == 1){this.AddButton(dbx, dby, 20491 ,20491 , 76 , GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 112 ) && m_Scroll.mN13VampiricEmbraceSpell == 1){this.AddButton(dbx, dby, 20492 ,20492 , 77 , GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 113 ) && m_Scroll.mN14VengefulSpiritSpell == 1){this.AddButton(dbx, dby, 20493 ,20493 , 78 , GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 114 ) && m_Scroll.mN15WitherSpell == 1){this.AddButton(dbx, dby, 20494 ,20494 , 79 , GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 115 ) && m_Scroll.mN16WraithFormSpell == 1){this.AddButton(dbx, dby, 20495 ,20495 , 80 , GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 116 ) && m_Scroll.mN17ExorcismSpell == 1){this.AddButton(dbx, dby, 20496 ,20496 , 81 , GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
           
        // CHIVALRY

            if ( HasSpell( from, 200 ) && m_Scroll.mC01CleanseByFireSpell == 1){this.AddButton(dbx, dby, 20736, 20736, 82, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 201 ) && m_Scroll.mC02CloseWoundsSpell == 1){this.AddButton(dbx, dby, 20737, 20737, 83, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 202 ) && m_Scroll.mC03ConsecrateWeaponSpell == 1){this.AddButton(dbx, dby, 20738, 20738, 84, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 203 ) && m_Scroll.mC04DispelEvilSpell == 1){this.AddButton(dbx, dby, 20739, 20739, 85, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 204 ) && m_Scroll.mC05DivineFurySpell == 1){this.AddButton(dbx, dby, 20740, 20740, 86, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 205 ) && m_Scroll.mC06EnemyOfOneSpell == 1){this.AddButton(dbx, dby, 20741, 20741, 87, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 206 ) && m_Scroll.mC07HolyLightSpell == 1){this.AddButton(dbx, dby, 20742, 20742, 88, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 207 ) && m_Scroll.mC08NobleSacrificeSpell == 1){this.AddButton(dbx, dby, 20743, 20743, 89, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 208 ) && m_Scroll.mC09RemoveCurseSpell == 1){this.AddButton(dbx, dby, 20744, 20744, 90, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 209 ) && m_Scroll.mC10SacredJourneySpell == 1){this.AddButton(dbx, dby, 20745, 20745, 91, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}

// BUSHIDO
			
            
            if ( HasSpell( from, 400) && m_Scroll.mB06HonorableExecution == 1)
			{
				if ( SpecialMove.GetCurrentMove( from ) is HonorableExecution )
				{
						this.AddButton(dbx, dby, 21536 ,21536 , 92 , GumpButtonType.Reply, 1);
						this.AddAlphaRegion(dbx, dby, 45, 45);
						dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; } if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; } if ( m_Scroll.mSwitch == 1 ) { if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; } }
						
						
				}
				else 
				{
					this.AddButton(dbx, dby, 21536 ,21536 , 92 , GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }
				}
			}
			if ( HasSpell( from, 401 ) && m_Scroll.mB01Confidence == 1) { this.AddButton(dbx, dby, 21537 ,21537 , 93 , GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }  }
			if ( HasSpell( from, 402 ) && m_Scroll.mB03Evasion == 1){this.AddButton(dbx, dby, 21538 ,21538 , 94 , GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 403 ) && m_Scroll.mB02CounterAttack  == 1){this.AddButton(dbx, dby, 21539 ,21539	 , 95 , GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 404 ) && m_Scroll.mB04LightningStrike == 1)
			{
				
				if ( SpecialMove.GetCurrentMove( from ) is LightningStrike )
				{
					this.AddButton(dbx, dby, 21540 ,21540 , 96 , GumpButtonType.Reply, 1);
					this.AddAlphaRegion(dbx, dby, 45, 45);
					dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; } if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; } if ( m_Scroll.mSwitch == 1 ) { if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; } }
					
				}
				else 
				{
					this.AddButton(dbx, dby, 21540 ,21540 , 96 , GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }
				}
			}
            if ( HasSpell( from, 405 ) && m_Scroll.mB05MomentumStrike == 1)
			{
				if ( SpecialMove.GetCurrentMove( from ) is MomentumStrike )
				{
					this.AddButton(dbx, dby, 21541 ,21541 , 97 , GumpButtonType.Reply, 1);
					this.AddAlphaRegion(dbx, dby, 45, 45);
					dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; } if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; } if ( m_Scroll.mSwitch == 1 ) { if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; } }
					
				}
				else
				{
					this.AddButton(dbx, dby, 21541 ,21541 , 97 , GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }
				}
			}
	
			

// NINJITSU
		
			if ( HasSpell( from, 500 ) && m_Scroll.mI08FocusAttack == 1)
			{ 
				if ( SpecialMove.GetCurrentMove( from ) is FocusAttack )
				{
					this.AddButton(dbx, dby, 21280 ,21280 , 98 , GumpButtonType.Reply, 1);
					this.AddAlphaRegion(dbx, dby, 45, 45);
					dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; } if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; } if ( m_Scroll.mSwitch == 1 ) { if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; } }
					  
				}
				else
				{
					this.AddButton(dbx, dby, 21280, 21280, 98, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }  
				}	
			}

            if ( HasSpell( from, 501 ) && m_Scroll.mI01DeathStrike == 1)
			{
				if ( SpecialMove.GetCurrentMove( from ) is DeathStrike )
				{
					this.AddButton(dbx, dby, 21281 ,21281 , 99 , GumpButtonType.Reply, 1);
					this.AddAlphaRegion(dbx, dby, 45, 45);
					dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; } if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; } if ( m_Scroll.mSwitch == 1 ) { if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; } }
					 
				}
				else
				{
					this.AddButton(dbx, dby, 21281, 21281, 99, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }
				}
			}
            if ( HasSpell( from, 502 ) && m_Scroll.mI02AnimalForm == 1){this.AddButton(dbx, dby, 21282, 21282, 100, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 503 ) && m_Scroll.mI03KiAttack == 1)
			{
				if ( SpecialMove.GetCurrentMove( from ) is KiAttack )
				{
					this.AddButton(dbx, dby, 21283 ,21283 , 101 , GumpButtonType.Reply, 1);
					this.AddAlphaRegion(dbx, dby, 45, 45);
					dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; } if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; } if ( m_Scroll.mSwitch == 1 ) { if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; } }
					
				}
				else
				{
					this.AddButton(dbx, dby, 21283, 21283, 101, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }
				}
			}
            if ( HasSpell( from, 504 ) && m_Scroll.mI04SurpriseAttack == 1)
			{
				if ( SpecialMove.GetCurrentMove( from ) is SurpriseAttack )
				{
					this.AddButton(dbx, dby, 21284 ,21284 , 102 , GumpButtonType.Reply, 1);
					this.AddAlphaRegion(dbx, dby, 45, 45);
					dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; } if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; } if ( m_Scroll.mSwitch == 1 ) { if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; } }
					
				}
				else
				{
					this.AddButton(dbx, dby, 21284, 21284, 102, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }
				}
			}
            if ( HasSpell( from, 505 ) && m_Scroll.mI05Backstab == 1)
			{
				
				if ( SpecialMove.GetCurrentMove( from ) is Backstab )
				{
					this.AddButton(dbx, dby, 21285 ,21285 , 103 , GumpButtonType.Reply, 1);
					this.AddAlphaRegion(dbx, dby, 45, 45);
					dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; } if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; } if ( m_Scroll.mSwitch == 1 ) { if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; } }
					
				}
				else
				{
					this.AddButton(dbx, dby, 21285, 21285, 103, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }
				}
			}
            if ( HasSpell( from, 506 ) && m_Scroll.mI06Shadowjump == 1){this.AddButton(dbx, dby, 21286, 21286, 104, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
            if ( HasSpell( from, 507 ) && m_Scroll.mI07MirrorImage == 1){this.AddButton(dbx, dby, 21287, 21287, 105, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			

// SPELLWEAVING

			if ( HasSpell( from, 600 ) && m_Scroll. mS01ArcaneCircleSpell== 1){this.AddButton(dbx, dby, 23000, 23000, 106, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 601 ) && m_Scroll. mS02GiftOfRenewalSpell== 1){this.AddButton(dbx, dby, 23001, 23001, 107, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 602 ) && m_Scroll. mS03ImmolatingWeaponSpell== 1){this.AddButton(dbx, dby, 23002, 23002, 108, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 603 ) && m_Scroll. mS04AttuneWeaponSpell== 1){this.AddButton(dbx, dby, 23003, 23003, 109, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 604 ) && m_Scroll. mS05ThunderstormSpell== 1){this.AddButton(dbx, dby, 23004, 23004, 110, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 605 ) && m_Scroll. mS06NatureFurySpell== 1){this.AddButton(dbx, dby, 23005, 23005, 111, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 606 ) && m_Scroll. mS07SummonFeySpell== 1){this.AddButton(dbx, dby, 23006, 23006, 112, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 607 ) && m_Scroll. mS08SummonFiendSpell== 1){this.AddButton(dbx, dby, 23007, 23007, 113, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 608 ) && m_Scroll. mS09ReaperFormSpell== 1){this.AddButton(dbx, dby, 23008, 23008, 114, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 609 ) && m_Scroll. mS10WildfireSpell== 1){this.AddButton(dbx, dby, 23009, 23009, 115, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 610 ) && m_Scroll. mS11EssenceOfWindSpell== 1){this.AddButton(dbx, dby, 23010, 23010, 116, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 611 ) && m_Scroll. mS12DryadAllureSpell== 1){this.AddButton(dbx, dby, 23011, 23011, 117, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 612 ) && m_Scroll. mS13EtherealVoyageSpell== 1){this.AddButton(dbx, dby, 23012, 23012, 118, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 613 ) && m_Scroll. mS14WordOfDeathSpell== 1){this.AddButton(dbx, dby, 23013, 23013, 119, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 614 ) && m_Scroll. mS15GiftOfLifeSpell== 1){this.AddButton(dbx, dby, 23014, 23014, 120, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 615 ) && m_Scroll. mS16ArcaneEmpowermentSpell== 1){this.AddButton(dbx, dby, 23015, 23015, 121, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}

 // MYSTICISM
			if ( HasSpell( from, 677 ) && m_Scroll. mM01NetherBoltSpell== 1){this.AddButton(dbx, dby, 24000, 24000, 122, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 678 ) && m_Scroll. mM02HealingStoneSpell== 1){this.AddButton(dbx, dby, 24001, 24001, 123, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 679 ) && m_Scroll. mM03PurgeMagicSpell== 1){this.AddButton(dbx, dby, 24002, 24002, 124, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 680 ) && m_Scroll. mM04EnchantSpell== 1){this.AddButton(dbx, dby, 24003, 24003, 125, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 681 ) && m_Scroll. mM05SleepSpell== 1){this.AddButton(dbx, dby, 24004, 24004, 126, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 682 ) && m_Scroll. mM06EagleStrikeSpell== 1){this.AddButton(dbx, dby, 24005, 24005, 127, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 683 ) && m_Scroll. mM07AnimatedWeaponSpell== 1){this.AddButton(dbx, dby, 24006, 24006, 128, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
	////
			if ( HasSpell( from, 684 ) && m_Scroll. mM16StoneFormSpell== 1){this.AddButton(dbx, dby, 24007, 2400, 137, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 685 ) && m_Scroll. mM08SpellTriggerSpell== 1){this.AddButton(dbx, dby, 24008, 24008, 129, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 686 ) && m_Scroll. mM09MassSleepSpell== 1){this.AddButton(dbx, dby, 24009, 24009, 130, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 687 ) && m_Scroll. mM10CleansingWindsSpell== 1){this.AddButton(dbx, dby, 24010, 24010, 131, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 688 ) && m_Scroll. mM11BombardSpell== 1){this.AddButton(dbx, dby, 24011, 24011, 132, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 689 ) && m_Scroll. mM12SpellPlagueSpell== 1){this.AddButton(dbx, dby, 24012, 24012, 133, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 690 ) && m_Scroll. mM13HailStormSpell== 1){this.AddButton(dbx, dby, 24013, 24013, 134, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 691 ) && m_Scroll. mM14NetherCycloneSpell== 1){this.AddButton(dbx, dby, 24014, 24014, 135, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			if ( HasSpell( from, 691 ) && m_Scroll. mM15RisingColossusSpell== 1){this.AddButton(dbx, dby, 24015, 24015, 136, GumpButtonType.Reply, 1); dbx = dbx + dbxa; dby = dby + dbya; if ( m_Scroll.mXselect_10 == 1) { xselect_var = 562; } if ( m_Scroll.mXselect_15 == 1) { xselect_var = 787; } if ( m_Scroll.mXselect_20 == 1) { xselect_var = 1012;  } if ( m_Scroll.mXselect_30 == 1) { xselect_var = 1462;  } if ( dbx + dbxa >= xselect_var & dby + dbya == 5 ) { dbx = 67; dbxa = 45; dby = 50; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 50 ) { dbx = 67; dbxa = 45; dby = 95; dbya = 0; }  if ( dbx + dbxa >= xselect_var & dby + dbya == 95 ) { dbx = 67; dbxa = 45; dby = 140; dbya = 0; }  if ( m_Scroll.mSwitch == 1 ) {  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 0 ) { dbx = 45; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 45 ) { dbx = 90; dbxa = 0; dby = 54; dbya = 45; }  if ( dby + dbya >= xselect_var - 13 & dbx + dbxa == 90 ) { dbx = 135; dbxa = 0; dby = 54; dbya = 45; } }}
			
			
			AddPage(2); // minimize
			
			this.AddImage( 24, 0, 2234, 0);
			this.AddBackground( 0,0, 25,80, 9270 ); //options background
			this.AddButton( 2, 5,  5601, 5601, 0, GumpButtonType.Page, 1); // minimize
			
			AddPage(3); // minimize
			
			this.AddImage( 0, 0, 2234, 0);
			this.AddBackground( 42,0, 47,51, 9270 ); 
			this.AddButton( 48, 7,  5602, 5602, 0, GumpButtonType.Page, 1); // minimize
			
			
        }
   


        public override void OnResponse( NetState state, RelayInfo info )
        {
            Mobile from = state.Mobile;
           
            switch ( info.ButtonID )
            {
                case 0: { break; }
                case 1: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 0 ) ) { new ClumsySpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 2: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 1 ) ) { new CreateFoodSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 3: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 2 ) ) { new FeeblemindSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 4: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 3 ) ) { new HealSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 5: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 4 ) ) { new MagicArrowSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 6: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 5 ) ) { new NightSightSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 7: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 6 ) ) { new ReactiveArmorSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 8: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 7 ) ) { new WeakenSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 9: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 8 ) ) { new AgilitySpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 10: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 9 ) ) { new CunningSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 11: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 10 ) ) { new CureSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 12: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 11 ) ) { new HarmSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 13: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 12 ) ) { new MagicTrapSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 14: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 13 ) ) { new RemoveTrapSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 15: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 14 ) ) { new ProtectionSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 16: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 15 ) ) { new StrengthSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 17: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 16 ) ) { new BlessSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 18: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 17 ) ) { new FireballSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 19: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 18 ) ) { new MagicLockSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 20: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 19 ) ) { new PoisonSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 21: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 20 ) ) { new TelekinesisSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 22: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 21 ) ) { new TeleportSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 23: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 22 ) ) { new UnlockSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 24: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 23 ) ) { new WallOfStoneSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 25: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 24 ) ) { new ArchCureSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 26: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 25 ) ) { new ArchProtectionSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 27: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 26 ) ) { new CurseSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 28: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 27 ) ) { new FireFieldSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 29: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 28 ) ) { new GreaterHealSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 30: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 29 ) ) { new LightningSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 31: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 30 ) ) { new ManaDrainSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 32: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 31 ) ) { new RecallSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 33: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 32 ) ) { new BladeSpiritsSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 34: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 33 ) ) { new DispelFieldSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 35: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 34 ) ) { new IncognitoSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 36: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 35 ) ) { new MagicReflectSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 37: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 36 ) ) { new MindBlastSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 38: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 37 ) ) { new ParalyzeSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 39: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 38 ) ) { new PoisonFieldSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 40: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 39 ) ) { new SummonCreatureSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 41: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 40 ) ) { new DispelSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 42: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 41 ) ) { new EnergyBoltSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 43: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 42 ) ) { new ExplosionSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 44: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 43 ) ) { new InvisibilitySpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 45: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 44 ) ) { new MarkSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 46: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 45 ) ) { new MassCurseSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 47: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 46 ) ) { new ParalyzeFieldSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 48: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 47 ) ) { new RevealSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				//if (HasSpell( from, 47 ) && m_Scroll.mW48_ChainLightningSpell == 1){this.AddButton(dbx, 5, 2287, 2287, 48, GumpButtonType.Reply, 1); }
				case 49: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 48 ) ) { new ChainLightningSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 50: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 49 ) ) { new EnergyFieldSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 51: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 50 ) ) { new FlameStrikeSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 52: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 51 ) ) { new GateTravelSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 53: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 52 ) ) { new ManaVampireSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 54: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 53 ) ) { new MassDispelSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 55: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 54 ) ) { new MeteorSwarmSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 56: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 55 ) ) { new PolymorphSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 57: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 56 ) ) { new EarthquakeSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 58: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 57 ) ) { new EnergyVortexSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 59: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 58 ) ) { new ResurrectionSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 60: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 59 ) ) { new AirElementalSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 61: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 60 ) ) { new SummonDaemonSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 62: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 61 ) ) { new EarthElementalSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 63: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 62 ) ) { new FireElementalSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 64: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 63 ) ) { new WaterElementalSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }

    // NECROMANCY
                case 65: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 100 ) ) { new AnimateDeadSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 66: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 101 ) ) { new BloodOathSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 67: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 102 ) ) { new CorpseSkinSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 68: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 103 ) ) { new CurseWeaponSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 69: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 104 ) ) { new EvilOmenSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 70: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 105 ) ) { new HorrificBeastSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 71: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 106 ) ) { new LichFormSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 72: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 107 ) ) { new MindRotSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 73: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 108 ) ) { new PainSpikeSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 74: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 109 ) ) { new PoisonStrikeSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 75: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 110 ) ) { new StrangleSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 76: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 111 ) ) { new SummonFamiliarSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 77: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 112 ) ) { new VampiricEmbraceSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 78: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 113 ) ) { new VengefulSpiritSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 79: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 114 ) ) { new WitherSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 80: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 115 ) ) { new WraithFormSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 81: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 116 ) ) { new ExorcismSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }

    // CHIVALRY

                case 82 : { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 200 ) ) { new CleanseByFireSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 83 : { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 201 ) ) { new CloseWoundsSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }  
                case 84 : { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 202 ) ) { new ConsecrateWeaponSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 85 : { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 203 ) ) { new DispelEvilSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 86 : { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 204 ) ) { new DivineFurySpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 87 : { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 205 ) ) { new EnemyOfOneSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 88 : { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 206 ) ) { new HolyLightSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 89 : { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 207 ) ) { new NobleSacrificeSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 90 :  { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 208 ) ) { new RemoveCurseSpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 91 : { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 209 ) ) { new SacredJourneySpell( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; } 
				
	// BUSHIDO
				
				case 92: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 400) )  {  SamuraiMove.SetCurrentMove( from, new HonorableExecution() ); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) );  }  break;  }
				
                case 93: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 401 ) ) { new Confidence ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 94: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 402 ) ) { new Evasion ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
                case 95: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 403 ) ) { new CounterAttack ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 96: 
				{ 
					int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 404 ) ) 
					{ 
						
						SamuraiMove.SetCurrentMove( from, new LightningStrike() ); 
						from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); 
					}
					
					break; 
				}
                case 97: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 405  ) ) { SamuraiMove.SetCurrentMove( from, new MomentumStrike() ); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				
				
	// NINJITSU
				case 98: 
				{  int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 500) ) { NinjaMove.SetCurrentMove( from, new FocusAttack() ); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); }  break; }
				
                case 99: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 501 ) ) { NinjaMove.SetCurrentMove( from, new DeathStrike() ); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				
                case 100: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 502 ) ) { new AnimalForm ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				
                case 101: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 503 ) ) { NinjaMove.SetCurrentMove( from, new KiAttack() ); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				
                case 102: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 504 ) ) { NinjaMove.SetCurrentMove( from, new SurpriseAttack() ); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				
                case 103: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 505 ) ) { NinjaMove.SetCurrentMove( from, new Backstab() ); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				
                 case 104: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 506 ) ) { new Shadowjump ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				 
                case 105: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 507 ) ) { new MirrorImage ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
			
				
			
	// SPELLWEAVING

                case 106 : { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 600 ) ) { new ArcaneCircleSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 107 : { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 601 ) ) { new GiftOfRenewalSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 108 : { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 602 ) ) { new ImmolatingWeaponSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 109 : { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 603 ) ) { new AttuneWeaponSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 110 : { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 604 ) ) { new ThunderstormSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 111 : { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 605 ) ) { new NatureFurySpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 112 : { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 606 ) ) { new SummonFeySpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 113 : { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 607 ) ) { new SummonFiendSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 114 : { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 608 ) ) { new ReaperFormSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 115 : { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 609) ) { new WildfireSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 116: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 610) ) { new EssenceOfWindSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 117: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 611) ) { new DryadAllureSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 118 : { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 612) ) { new EtherealVoyageSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 119: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 613) ) { new WordOfDeathSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 120: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 614) ) { new GiftOfLifeSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 121: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 615) ) { new ArcaneEmpowermentSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				
	// MYSTICISM
				
				case 122: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 677) ) { new NetherBoltSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 123: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 678) ) { new HealingStoneSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 124: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 679) ) { new PurgeMagicSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 125 : { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 680) ) { new EnchantSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 126: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 681) ) { new SleepSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 127: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 682) ) { new EagleStrikeSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 128: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 683) ) { new AnimatedWeaponSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
	/////////////////
				case 129: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 684) ) { new StoneFormSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				
				case 130: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 685) ) { new SpellTriggerSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 131: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 686) ) { new MassSleepSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 132: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 687) ) { new CleansingWindsSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 133: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 688) ) { new BombardSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 134: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 689) ) { new SpellPlagueSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 135: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 690) ) { new HailStormSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 136: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 691) ) { new NetherCycloneSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				case 137: { int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0; if ( HasSpell( from, 692) ) { new RisingColossusSpell ( from, null ).Cast(); from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); } break; }
				
				case 138: 
				{ 
					int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0;
					
					if ( m_Scroll.mSwitch == 0)
					{
						m_Scroll.mSwitch = 1; m_Scroll.mCount += 1;
						from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var, m_Scroll ) ); 
					}
					else
					{
						m_Scroll.mSwitch = 0;
						from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) );  
					} 

					break; 	
				}
				
				case 139: 
				{ 
					int dbx = 0; int dbxa = 0; int dby = 0; int dbya = 0; int xselect_var = 0;
					
					if ( m_Scroll.mLock == 0)
					{
						m_Scroll.mLock = 1; m_Scroll.mCount += 1;
						from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) ); 
					}
					else
					{
						m_Scroll.mLock = 0;
						from.SendGump( new SpellBar_BarGump( from, dbx, dbxa, dby, dbya, xselect_var,  m_Scroll ) );  
					} 

					break; 	
				}
				
            }
        }
    }
       
    }
 }

 