using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Commands;

namespace Server.Items
{
	public class LesserSlayerDeed : Item
	{

		[Constructable]
		public LesserSlayerDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
			LootType = LootType.Blessed;
			Name = "Lesser Slayer Deeds Picker";
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else
			{
				from.SendGump( new LesserSlayerDeedGump( from ) );
				this.Delete();
			}
		}

		public LesserSlayerDeed( Serial serial ) : base( serial )
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
	public class LesserSlayerDeedGump : Gump
	{
		public static void Initialize()
		{
			CommandSystem.Register( "LesserSlayerDeedGump", AccessLevel.GameMaster, new CommandEventHandler( LesserSlayerDeedGump_OnCommand ) );
		}

		private static void LesserSlayerDeedGump_OnCommand( CommandEventArgs e )
		{
			e.Mobile.SendGump( new LesserSlayerDeedGump( e.Mobile ) );
		}

		public LesserSlayerDeedGump( Mobile Owner ) : base( 0,0 )
		{
			AddPage(0);
		    
            AddBackground(85, 30, 600, 500, 3500);
            AddImage(136, 57, 10440); // Dragon - Left Side
            AddImage(526, 58, 10441); // Dragon - Right Side
            AddImage(199, 112, 5504); // UO Circle - Left
            AddImage(480, 111, 5504); // UO Circle - Right

            //	AddHtml(270, 115, 200, 16, @"<CENTER><BASEFONT COLOR=YELLOW>Server Name Here If You Want</CENTER>", (bool)false, (bool)false); 
            AddHtml(270, 147, 200, 16, @"<CENTER><BASEFONT COLOR=YELLOW>Lesser Slayer Deeds</Center>", (bool)false, (bool)false); 

			AddPage(1);
			//Side One
			AddLabel(246, 198, 4030, @"Balron Slayer"); 
			AddButton(206, 194, 9721, 9724, 1, GumpButtonType.Reply, 0); 

			AddLabel(246, 238, 4030, @"Blood Drinker"); 
			AddButton(206, 234, 9721, 9724, 2, GumpButtonType.Reply, 0); 

			AddLabel(246, 278, 4030, @"Daemon Slayer"); 
			AddButton(206, 274, 9721, 9724, 3, GumpButtonType.Reply, 0); 

			AddLabel(246, 318, 4030, @"Dragon Slayer"); 
			AddButton(206, 314, 9721, 9724, 4, GumpButtonType.Reply, 0); 

			AddLabel(246, 358, 4030, @"Earth Shattering"); 
			AddButton(206, 354, 9721, 9724, 5, GumpButtonType.Reply, 0); 

			AddLabel(246, 398, 4030, @"Elemental Health"); 
			AddButton(206, 394, 9721, 9724, 6, GumpButtonType.Reply, 0); 

			AddLabel(265, 438, 4030, @"Back");
			AddButton(206, 424, 4508, 4508, 0, GumpButtonType.Page, 2);


			//Side Two
			AddLabel(430, 198, 4030, @"Flame Dousing"); 
			AddButton(390, 194, 9721, 9724, 7, GumpButtonType.Reply, 0); 

			AddLabel(430, 238, 4030, @"Gargoyle Slayer");
			AddButton(390, 234, 9721, 9724, 8, GumpButtonType.Reply, 0); 

			AddLabel(430, 278, 4030, @"Lizardman Slayer"); 
			AddButton(390, 274, 9721, 9724, 9, GumpButtonType.Reply, 0);

			AddLabel(430, 318, 4030, @"Ogre Slayer"); 
			AddButton(390, 314, 9721, 9724, 10, GumpButtonType.Reply, 0); 

			AddLabel(430, 358, 4030, @"Ophidian Slayer"); 
			AddButton(390, 354, 9721, 9724, 11, GumpButtonType.Reply, 0); 

			AddLabel(430, 398, 4030, @"Orc Slayer"); 
			AddButton(390, 394, 9721, 9724, 12, GumpButtonType.Reply, 0); 

			AddLabel(456, 438, 4030, @"Next");
			AddButton(491, 424, 4502, 4502, 0, GumpButtonType.Page, 2);

			AddPage(2);
			//Side One
			AddLabel(246, 198, 4030, @"Scorpion Slayer"); 
			AddButton(206, 194, 9721, 9724, 13, GumpButtonType.Reply, 0); 

			AddLabel(246, 238, 4030, @"Snake Slayer"); 
			AddButton(206, 234, 9721, 9724, 14, GumpButtonType.Reply, 0);

			AddLabel(246, 278, 4030, @"Spider Slayer"); 
			AddButton(206, 274, 9721, 9724, 15, GumpButtonType.Reply, 0);

			AddLabel(246, 318, 4030, @"Summer Wind"); 
			AddButton(206, 314, 9721, 9724, 16, GumpButtonType.Reply, 0);

			AddLabel(246, 358, 4030, @"Terathon Slayer"); 
			AddButton(206, 354, 9721, 9724, 17, GumpButtonType.Reply, 0); 

			AddLabel(246, 398, 4030, @"Troll Slayer"); 
			AddButton(206, 394, 9721, 9724, 18, GumpButtonType.Reply, 0); 

			AddLabel(265, 438, 4030, @"Back");
			AddButton(206, 424, 4508, 4508, 0, GumpButtonType.Page, 1);


			//Side Two
			AddLabel(430, 198, 4030, @"Vacuum Slayer"); 
			AddButton(390, 194, 9721, 9724, 19, GumpButtonType.Reply, 0); 

			AddLabel(430, 238, 4030, @"Water Dissipation"); 
			AddButton(390, 234, 9721, 9724, 20, GumpButtonType.Reply, 0); 

			/*AddLabel(430, 278, 4030, @"Monster Statuette"); 
			AddButton(390, 274, 9721, 9724, 21, GumpButtonType.Reply, 0); 

			AddLabel(430, 318, 4030, @"Skin"); 
			AddButton(390, 314, 9721, 9724, 11, GumpButtonType.Reply, 0); 

		    AddLabel(430, 358, 4030, @"Weapon - Ranged"); 
			AddButton(390, 354, 9721, 9724, 12, GumpButtonType.Reply, 0); 

			AddLabel(430, 398, 4030, @"Weapon - Slashing"); 
			AddButton(390, 394, 9721, 9724, 13, GumpButtonType.Reply, 0);

			AddLabel(456, 438, 4030, @"Next");
			AddButton(491, 424, 4502, 4502, 0, GumpButtonType.Page, 1);
        */
		}

		public override void OnResponse( NetState state, RelayInfo info ) 
		{
			Mobile from = state.Mobile;

			switch ( info.ButtonID )
			{
				case 0: 
                    {
					//Cancel
					from.SendMessage( "You decide not to choose a Dye Tub." );
					LesserSlayerDeed LesserSlayerDeed = new LesserSlayerDeed();
					from.AddToBackpack( LesserSlayerDeed );
				}
				break;
				case 1: 
                    {
					BalronSlayerDeed BalronSlayerDeedX = new BalronSlayerDeed();
					from.AddToBackpack( BalronSlayerDeedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 2:
				{
					BloodDrinkingSlayerDeed BloodDrinkingSlayerDeedX = new BloodDrinkingSlayerDeed();
					from.AddToBackpack( BloodDrinkingSlayerDeedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 3:
				{
					DaemonSlayerDeed DaemonSlayerDeedX = new DaemonSlayerDeed();
					from.AddToBackpack( DaemonSlayerDeedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
                case 4:
                    {
                        DragonSlayerDeed DragonSlayerDeedX = new DragonSlayerDeed();
                        from.AddToBackpack(DragonSlayerDeedX);
                        from.SendMessage("Your choice has been added to your backpack!");
                    }
                    break;
                case 5:
				{
					EarthShatterSlayerDeed EarthShatterSlayerDeedX = new EarthShatterSlayerDeed();
					from.AddToBackpack( EarthShatterSlayerDeedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 6:
				{
					ElementalHealthSlayerDeed ElementalHealthSlayerDeedX = new ElementalHealthSlayerDeed();
					from.AddToBackpack( ElementalHealthSlayerDeedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 7:
				{
					FlameDousingSlayerDeed FlameDousingSlayerDeedX = new FlameDousingSlayerDeed();
					from.AddToBackpack( FlameDousingSlayerDeedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 8:
				{
					GargoyleSlayerDeed GargoyleSlayerDeedX = new GargoyleSlayerDeed();
					from.AddToBackpack( GargoyleSlayerDeedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 9:
				{
					LizardmanSlayerDeed LizardmanSlayerDeedX = new LizardmanSlayerDeed();
					from.AddToBackpack( LizardmanSlayerDeedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 10:
				{
					OgreSlayerDeed OgreSlayerDeedX = new OgreSlayerDeed();
					from.AddToBackpack( OgreSlayerDeedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 11:
				{
					OphidianSlayerDeed OphidianSlayerDeedX = new OphidianSlayerDeed();
					from.AddToBackpack( OphidianSlayerDeedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 12:
				{
					OrcSlayerDeed OrcSlayerDeedX = new OrcSlayerDeed();
					from.AddToBackpack( OrcSlayerDeedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 13:
				{
					ScorpionSlayerDeed ScorpionSlayerDeedX = new ScorpionSlayerDeed();
					from.AddToBackpack( ScorpionSlayerDeedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 14:
				{
					SnakeSlayerDeed SnakeSlayerDeedX = new SnakeSlayerDeed();
					from.AddToBackpack( SnakeSlayerDeedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 15:
				{
					SpidersDeathSlayerDeed SpidersDeathSlayerDeedX = new SpidersDeathSlayerDeed();
					from.AddToBackpack( SpidersDeathSlayerDeedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 16:
				{
					SummerWindSlayerDeed SummerWindSlayerDeedX = new SummerWindSlayerDeed();
					from.AddToBackpack( SummerWindSlayerDeedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 17:
				{
					TerathanSlayerDeed TerathanSlayerDeedX = new TerathanSlayerDeed();
					from.AddToBackpack( TerathanSlayerDeedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 18:
				{
					TrollSlayerDeed TrollSlayerDeedX = new TrollSlayerDeed();
					from.AddToBackpack( TrollSlayerDeedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 19:
				{
					VacuumSlayerDeed VacuumSlayerDeedX = new VacuumSlayerDeed();
					from.AddToBackpack( VacuumSlayerDeedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 20:
				{
					WaterDissipationSlayerDeed WaterDissipationSlayerDeedX = new WaterDissipationSlayerDeed();
					from.AddToBackpack( WaterDissipationSlayerDeedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;

			/*	case 21:
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
*/
			}
		}
	}
}