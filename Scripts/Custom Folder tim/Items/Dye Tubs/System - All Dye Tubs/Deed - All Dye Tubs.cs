using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Commands;

namespace Server.Items
{
	public class AllDyeTubsDeed : Item
	{

		[Constructable]
		public AllDyeTubsDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
			LootType = LootType.Blessed;
			Name = "All Dye Tubs Reward Deed";
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else
			{
				from.SendGump( new AllDyeTubsDeedGump( from ) );
				this.Delete();
			}
		}

		public AllDyeTubsDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}

namespace Server.Gumps
{
	public class AllDyeTubsDeedGump : Gump
	{
		public static void Initialize()
		{
			CommandSystem.Register( "AllDyeTubsDeedGump", AccessLevel.GameMaster, new CommandEventHandler( AllDyeTubsDeedGump_OnCommand ) );
		}

		private static void AllDyeTubsDeedGump_OnCommand( CommandEventArgs e )
		{
			e.Mobile.SendGump( new AllDyeTubsDeedGump( e.Mobile ) );
		}

		public AllDyeTubsDeedGump( Mobile Owner ) : base( 0,0 )
		{
			AddPage(0);
			AddBackground(186, 87, 371, 390, 2620); // Background 1
      this.AddBackground(186, 87, 371, 100, 2620); 
			AddImage(136, 57, 10440); // dragonside left
			AddImage(526, 58, 10441); // Dragon side right
			AddImage(199, 112, 5504); // UO Circle left
			AddImage(480, 111, 5504); // UO Circle Right

			AddHtml(270, 115, 200, 16, @"<CENTER><BASEFONT COLOR=YELLOW>Ancient Prophecies</CENTER>", (bool)false, (bool)false); // HTML 1
			AddHtml(270, 147, 200, 16, @"<CENTER><BASEFONT COLOR=YELLOW>All Dye Tubs</Center>", (bool)false, (bool)false); // Copy of HTML 1

			AddPage(1);
			//Side One
			AddLabel(246, 198, 4030, @"Armor - Bone"); // Label 2
			AddButton(206, 194, 9721, 9724, 1, GumpButtonType.Reply, 0); // L B 1

			AddLabel(246, 238, 4030, @"Armor - Chain"); // Copy of Label 2
			AddButton(206, 234, 9721, 9724, 2, GumpButtonType.Reply, 0); // L B 2

			AddLabel(246, 278, 4030, @"Armor - Leather"); // Copy of Copy of Label 2
			AddButton(206, 274, 9721, 9724, 3, GumpButtonType.Reply, 0); // L B 3

			AddLabel(246, 318, 4030, @"Armor - Plate"); // Copy of Copy of Copy of Label 2
			AddButton(206, 314, 9721, 9724, 4, GumpButtonType.Reply, 0); // L B 4

			AddLabel(246, 358, 4030, @"Armor - Ring"); // Voting Menu
			AddButton(206, 354, 9721, 9724, 5, GumpButtonType.Reply, 0); // L B 5

			AddLabel(246, 398, 4030, @"Weapon - Staff"); // Copy of Voting Menu
			AddButton(206, 394, 9721, 9724, 6, GumpButtonType.Reply, 0); // L B 6

			//            AddLabel(246, 438, 4030, @"Beetle"); // Copy of Voting Menu
			//            AddButton(206, 434, 9721, 9724, 7, GumpButtonType.Reply, 0); // L B 7
			AddLabel(265, 438, 4030, @"Back");
			AddButton(206, 424, 4508, 4508, 0, GumpButtonType.Page, 2);


			//Side Two
			AddLabel(430, 198, 4030, @"Weapon - Axe"); // Copy of Label 2
			AddButton(390, 194, 9721, 9724, 7, GumpButtonType.Reply, 0); // R B 1

			AddLabel(430, 238, 4030, @"Weapon - Bashing"); // Copy of Copy of Label 2
			AddButton(390, 234, 9721, 9724, 8, GumpButtonType.Reply, 0); // R B 2

			AddLabel(430, 278, 4030, @"Weapon - Piercing"); // Copy of Copy of Copy of Label 2
			AddButton(390, 274, 9721, 9724, 9, GumpButtonType.Reply, 0); // R B 3

			AddLabel(430, 318, 4030, @"Weapon - PoleArm"); // Copy of Copy of Copy of Copy of Label 2
			AddButton(390, 314, 9721, 9724, 10, GumpButtonType.Reply, 0); // R B 4

			AddLabel(430, 358, 4030, @"Weapon - Ranged"); // WWW Sites
			AddButton(390, 354, 9721, 9724, 11, GumpButtonType.Reply, 0); // R B 5

			AddLabel(430, 398, 4030, @"Weapon - Slashing"); // Copy of WWW Sites
			AddButton(390, 394, 9721, 9724, 12, GumpButtonType.Reply, 0); // R B 6

			//            AddLabel(430, 438, 4030, @"Forums"); // Copy of WWW Sites
			//            AddButton(390, 434, 9721, 9724, 14, GumpButtonType.Reply, 0); // R B 7
			AddLabel(456, 438, 4030, @"Next");
			AddButton(491, 424, 4502, 4502, 0, GumpButtonType.Page, 2);

			AddPage(2);
			//Side One
			AddLabel(246, 198, 4030, @"Rune Book"); // Label 2
			AddButton(206, 194, 9721, 9724, 13, GumpButtonType.Reply, 0); // L B 1

			AddLabel(246, 238, 4030, @"Spell Book"); // Copy of Label 2
			AddButton(206, 234, 9721, 9724, 14, GumpButtonType.Reply, 0); // L B 2

			AddLabel(246, 278, 4030, @"Furniture"); // Copy of Copy of Label 2
			AddButton(206, 274, 9721, 9724, 15, GumpButtonType.Reply, 0); // L B 3

			AddLabel(246, 318, 4030, @"Mount"); // Copy of Copy of Copy of Label 2
			AddButton(206, 314, 9721, 9724, 16, GumpButtonType.Reply, 0); // L B 4

			AddLabel(246, 358, 4030, @"Ethereal Mount"); // Voting Menu
			AddButton(206, 354, 9721, 9724, 17, GumpButtonType.Reply, 0); // L B 5

			AddLabel(246, 398, 4030, @"Potion Keg"); // Copy of Voting Menu
			AddButton(206, 394, 9721, 9724, 18, GumpButtonType.Reply, 0); // L B 6

			//            AddLabel(246, 438, 4030, @"Beetle"); // Copy of Voting Menu
			//            AddButton(206, 434, 9721, 9724, 7, GumpButtonType.Reply, 0); // L B 7
			AddLabel(265, 438, 4030, @"Back");
			AddButton(206, 424, 4508, 4508, 0, GumpButtonType.Page, 1);


			//Side Two
			AddLabel(430, 198, 4030, @"Recall Rune"); // Copy of Label 2
			AddButton(390, 194, 9721, 9724, 19, GumpButtonType.Reply, 0); // R B 1

			AddLabel(430, 238, 4030, @"Shield"); // Copy of Copy of Label 2
			AddButton(390, 234, 9721, 9724, 20, GumpButtonType.Reply, 0); // R B 2

			AddLabel(430, 278, 4030, @"Monster Statuette"); // Copy of Copy of Copy of Label 2
			AddButton(390, 274, 9721, 9724, 21, GumpButtonType.Reply, 0); // R B 3

			AddLabel(430, 318, 4030, @"Skin"); // Copy of Copy of Copy of Copy of Label 2
			AddButton(390, 314, 9721, 9724, 11, GumpButtonType.Reply, 0); // R B 4

//			AddLabel(430, 358, 4030, @"Weapon - Ranged"); // WWW Sites
//			AddButton(390, 354, 9721, 9724, 12, GumpButtonType.Reply, 0); // R B 5
//
//			AddLabel(430, 398, 4030, @"Weapon - Slashing"); // Copy of WWW Sites
//			AddButton(390, 394, 9721, 9724, 13, GumpButtonType.Reply, 0); // R B 6

			//            AddLabel(430, 428, 4030, @"Forums"); // Copy of WWW Sites
			//            AddButton(390, 434, 9721, 9724, 14, GumpButtonType.Reply, 0); // R B 7
			AddLabel(456, 438, 4030, @"Next");
			AddButton(491, 424, 4502, 4502, 0, GumpButtonType.Page, 1);

		}

		public override void OnResponse( NetState state, RelayInfo info ) //Function for GumpButtonType.Reply Buttons
		{
			Mobile from = state.Mobile;

			switch ( info.ButtonID )
			{
				case 0: //Case uses the ActionIDs defenied above. Case 0 defenies the actions for the button with the action id 0
				{
					//Cancel
					from.SendMessage( "You decide not to choose a Dye Tub." );
					AllDyeTubsDeed AllDyeTubsDeed = new AllDyeTubsDeed();
					from.AddToBackpack( AllDyeTubsDeed );
				}
				break;
				case 1: //Case uses the ActionIDs defenied above. Case 0 defenies the actions for the button with the action id 0
				{
					AllDyeTubsArmorBone AllDyeTubsArmorBoneX = new AllDyeTubsArmorBone();
					AllDyeTubsArmorBoneX.Charged = false;
					from.AddToBackpack( AllDyeTubsArmorBoneX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 2: //Same as above
				{
					AllDyeTubsArmorChain AllDyeTubsArmorChainX = new AllDyeTubsArmorChain();
					AllDyeTubsArmorChainX.Charged = false;
					from.AddToBackpack( AllDyeTubsArmorChainX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 3:
				{
					AllDyeTubsArmorLeather AllDyeTubsArmorLeatherX = new AllDyeTubsArmorLeather();
					AllDyeTubsArmorLeatherX.Charged = false;
					from.AddToBackpack( AllDyeTubsArmorLeatherX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 4:
				{
					AllDyeTubsArmorPlate AllDyeTubsArmorPlateX = new AllDyeTubsArmorPlate();
					AllDyeTubsArmorPlateX.Charged = false;
					from.AddToBackpack( AllDyeTubsArmorPlateX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 5:
				{
					AllDyeTubsArmorRing AllDyeTubsArmorRingX = new AllDyeTubsArmorRing();
					AllDyeTubsArmorRingX.Charged = false;
					from.AddToBackpack( AllDyeTubsArmorRingX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 6:
				{
					AllDyeTubsWeaponStaff AllDyeTubsWeaponStaffX = new AllDyeTubsWeaponStaff();
					AllDyeTubsWeaponStaffX.Charged = false;
					from.AddToBackpack( AllDyeTubsWeaponStaffX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 7:
				{
					AllDyeTubsWeaponAxe AllDyeTubsWeaponAxeX = new AllDyeTubsWeaponAxe();
					AllDyeTubsWeaponAxeX.Charged = false;
					from.AddToBackpack( AllDyeTubsWeaponAxeX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 8:
				{
					AllDyeTubsWeaponBashing AllDyeTubsWeaponBashingX = new AllDyeTubsWeaponBashing();
					AllDyeTubsWeaponBashingX.Charged = false;
					from.AddToBackpack( AllDyeTubsWeaponBashingX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 9:
				{
					AllDyeTubsWeaponPiercing AllDyeTubsWeaponPiercingX = new AllDyeTubsWeaponPiercing();
					AllDyeTubsWeaponPiercingX.Charged = false;
					from.AddToBackpack( AllDyeTubsWeaponPiercingX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 10:
				{
					AllDyeTubsWeaponPoleArm AllDyeTubsWeaponPoleArmX = new AllDyeTubsWeaponPoleArm();
					AllDyeTubsWeaponPoleArmX.Charged = false;
					from.AddToBackpack( AllDyeTubsWeaponPoleArmX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 11:
				{
					AllDyeTubsWeaponRanged AllDyeTubsWeaponRangedX = new AllDyeTubsWeaponRanged();
					AllDyeTubsWeaponRangedX.Charged = false;
					from.AddToBackpack( AllDyeTubsWeaponRangedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 12:
				{
					AllDyeTubsWeaponSlashing AllDyeTubsWeaponSlashingX = new AllDyeTubsWeaponSlashing();
					AllDyeTubsWeaponSlashingX.Charged = false;
					from.AddToBackpack( AllDyeTubsWeaponSlashingX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 13:
				{
					AllDyeTubsBookRune AllDyeTubsBookRuneX = new AllDyeTubsBookRune();
					AllDyeTubsBookRuneX.Charged = false;
					from.AddToBackpack( AllDyeTubsBookRuneX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 14:
				{
					AllDyeTubsBookSpell AllDyeTubsBookSpellX = new AllDyeTubsBookSpell();
					AllDyeTubsBookSpellX.Charged = false;
					from.AddToBackpack( AllDyeTubsBookSpellX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 15:
				{
					AllDyeTubsFurniture AllDyeTubsFurnitureX = new AllDyeTubsFurniture();
					AllDyeTubsFurnitureX.Charged = false;
					from.AddToBackpack( AllDyeTubsFurnitureX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 16:
				{
					AllDyeTubsMount AllDyeTubsMountX = new AllDyeTubsMount();
					AllDyeTubsMountX.Charged = false;
					from.AddToBackpack( AllDyeTubsMountX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 17:
				{
					AllDyeTubsMountEthereal AllDyeTubsMountEtherealX = new AllDyeTubsMountEthereal();
					AllDyeTubsMountEtherealX.Charged = false;
					from.AddToBackpack( AllDyeTubsMountEtherealX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 18:
				{
					AllDyeTubsPotionKeg AllDyeTubsPotionKegX = new AllDyeTubsPotionKeg();
					AllDyeTubsPotionKegX.Charged = false;
					from.AddToBackpack( AllDyeTubsPotionKegX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 19:
				{
					AllDyeTubsRecallRune AllDyeTubsRecallRuneX = new AllDyeTubsRecallRune();
					AllDyeTubsRecallRuneX.Charged = false;
					from.AddToBackpack( AllDyeTubsRecallRuneX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 20:
				{
					AllDyeTubsShield AllDyeTubsShieldX = new AllDyeTubsShield();
					AllDyeTubsShieldX.Charged = false;
					from.AddToBackpack( AllDyeTubsShieldX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 21:
				{
					AllDyeTubsStatuetteMonster AllDyeTubsStatuetteMonsterX = new AllDyeTubsStatuetteMonster();
					AllDyeTubsStatuetteMonsterX.Charged = false;
					from.AddToBackpack( AllDyeTubsStatuetteMonsterX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 22:
				{
					AllDyeTubsSkin AllDyeTubsSkinX = new AllDyeTubsSkin();
					AllDyeTubsSkinX.Charged = false;
					from.AddToBackpack( AllDyeTubsSkinX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;

			}
		}
	}
}