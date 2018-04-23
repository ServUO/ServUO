using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Commands;
using Server.Prompts;
using Server.Targeting;



namespace Server.Items
{
	public class SuperSlayerDeeds : Item
	{

		[Constructable]
		public SuperSlayerDeeds() : base( 0x14F0 )
		{
			Weight = 1.0;
			LootType = LootType.Blessed;
			Name = "Super Slayer Deeds Picker";
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else
			{
				from.SendGump( new SuperSlayerDeedsGump( from ) );
				this.Delete();
			}
		}

		public SuperSlayerDeeds( Serial serial ) : base( serial )
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
	public class SuperSlayerDeedsGump : Gump
	{
		public static void Initialize()
		{
			CommandSystem.Register( "SuperSlayerDeedsGump", AccessLevel.GameMaster, new CommandEventHandler( SuperSlayerDeedsGump_OnCommand ) );
		}

		private static void SuperSlayerDeedsGump_OnCommand( CommandEventArgs e )
		{
			e.Mobile.SendGump( new SuperSlayerDeedsGump( e.Mobile ) );
		}

		public SuperSlayerDeedsGump( Mobile Owner ) : base( 0,0 )
		{
			AddPage(0);
		    
            AddBackground(85, 30, 600, 500, 3500);
            AddImage(136, 57, 10440); // Dragon - Left Side
			AddImage(526, 58, 10441); // Dragon - Right Side
			AddImage(199, 112, 5504); // UO Circle - Left
			AddImage(480, 111, 5504); // UO Circle - Right

		//	AddHtml(270, 115, 200, 16, @"<CENTER><BASEFONT COLOR=YELLOW>Server Name Here If You Want</CENTER>", (bool)false, (bool)false); 
			AddHtml(270, 147, 200, 16, @"<CENTER><BASEFONT COLOR=YELLOW>Super Slayer Deeds</Center>", (bool)false, (bool)false);

			AddPage(1);
           
            AddLabel(246, 198, 4030, @"Arachnid Slayer");
            AddButton(206, 194, 9721, 9724, 1, GumpButtonType.Reply, 0);

            AddLabel(246, 238, 4030, @"Demon Slayer");
            AddButton(206, 234, 9721, 9724, 2, GumpButtonType.Reply, 0);

            AddLabel(246, 278, 4030, @"Elemental Slayer");
            AddButton(206, 274, 9721, 9724, 3, GumpButtonType.Reply, 0);

            AddLabel(246, 318, 4030, @"Fey Slayer");
            AddButton(206, 314, 9721, 9724, 4, GumpButtonType.Reply, 0);

            AddLabel(246, 358, 4030, @"Repond Slayer");
            AddButton(206, 354, 9721, 9724, 5, GumpButtonType.Reply, 0);

            AddLabel(246, 398, 4030, @"Reptile Slayer");
            AddButton(206, 394, 9721, 9724, 6, GumpButtonType.Reply, 0);

            AddLabel(246, 438, 4030, @"Undead Slayer");
            AddButton(206, 434, 9721, 9724, 7, GumpButtonType.Reply, 0);
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
					SuperSlayerDeeds SuperSlayerDeeds = new SuperSlayerDeeds();
					from.AddToBackpack( SuperSlayerDeeds );
				}
				break;
				case 1: 
                    {
					ArachnidSlayerDeed ArachnidSlayerDeedX = new ArachnidSlayerDeed();
					from.AddToBackpack( ArachnidSlayerDeedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 2: 
				{
					DemonSlayerDeed DemonSlayerDeedX = new DemonSlayerDeed();
					from.AddToBackpack( DemonSlayerDeedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 3:
				{
					ElementalSlayerDeed ElementalSlayerDeedX = new ElementalSlayerDeed();
					from.AddToBackpack( ElementalSlayerDeedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 4:
				{
					FeySlayerDeed FeySlayerDeedX = new FeySlayerDeed();
					from.AddToBackpack( FeySlayerDeedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 5:
				{
					RepondSlayerDeed RepondSlayerDeedX = new RepondSlayerDeed();
					from.AddToBackpack( RepondSlayerDeedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 6:
				{
					ReptileSlayerDeed ReptileSlayerDeedX = new ReptileSlayerDeed();
					from.AddToBackpack( ReptileSlayerDeedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
				case 7:
				{
					UndeadSlayerDeed UndeadSlayerDeedX = new UndeadSlayerDeed();
					from.AddToBackpack( UndeadSlayerDeedX );
					from.SendMessage( "Your choice has been added to your backpack!" );
				}
				break;
			}
		}
	}
}