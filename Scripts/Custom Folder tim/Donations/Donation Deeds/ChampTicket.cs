using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;
using Server.Gumps;
//using Xanthos.Evo;

namespace Server
{
	public class NewChampRewarddeed : Item
	{
		private Mobile m_Owner;

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner
		{
			get{ return m_Owner; }
			set{ m_Owner = value; }
		}

		// What a disgustingly long name
		//public override int LabelNumber{ get{ return 1041492; } } // This is half a prize ticket! Double-click this ticket and target any other ticket marked NEW PLAYER and get a prize! This ticket will only work for YOU, so don't give it away!

		[Constructable]
		public NewChampRewarddeed() : base( 0x14F0 )
		{
			Weight = 6.0;
			LootType = LootType.Cursed;
			Name = "a champion reward deed";
			Hue = 1150;
		}

		public NewChampRewarddeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (Mobile) m_Owner );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Owner = reader.ReadMobile();
					break;
				}
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			//if ( from != m_Owner )
			//{
			//	from.SendLocalizedMessage( 501926 ); // This isn't your ticket! Shame on you! You have to use YOUR ticket.
			//}
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else
			{
				from.SendGump( new InternalGump( from, this ) );
			}
		}

		private class InternalGump : Gump
		{
			private Mobile m_From;
			private NewChampRewarddeed m_Ticket;

			public InternalGump( Mobile from, NewChampRewarddeed ticket ) : base( 50, 50 )
			{
				m_From = from;
				m_Ticket = ticket;

				AddBackground( 0, 0, 400, 385, 0xA28 );

				AddHtml(30, 45, 340, 70,"Thank you for helping us defeat this vile evil, for your courageousness we have given you a choice of rewards, you may choose only one though!",true,false);

				AddButton( 46, 128, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0 );
				AddHtml(80,129,240,24,"a 225 stat ball",true,false);
				//AddLabel( 80, 128, 0x489, "a 225 stat ball" );  //# 1test

				//AddButton( 46, 163, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0 );
				//AddHtml(80,164,240,24,"a random blood pentagram part",true,false);
				//AddLabel( 80, 163, 0x489, "a clothing bless deed" );  //# 1test

				AddButton(46, 163, 0xFA5, 0xFA7, 5, GumpButtonType.Reply, 0);
				AddHtml(80, 164, 240, 24, "a skin tone deed", true, false);

				AddButton( 46, 198, 0xFA5, 0xFA7, 3, GumpButtonType.Reply, 0 );
				AddHtml(80,199,240,24,"a promotional token",true,false);

				AddButton( 46, 233, 0xFA5, 0xFA7, 4, GumpButtonType.Reply, 0 );
				AddHtml(80,234,240,24,"an ethereal beetle",true,false);

				AddButton( 46, 269, 0xFA5, 0xFA7, 6, GumpButtonType.Reply, 0 );
				AddHtml(80,269,240,24,"a special hair dye",true,false);

				//AddButton( 46, 268, 0xFA5, 0xFA7, 5, GumpButtonType.Reply, 0 );
				//AddHtml(80,269,240,24,"a Skin Tone Deed",true,false);
				//AddLabel( 80, 268, 0x489, "a skin tone deed" );  //# 1test

				//AddButton( 46, 303, 0xFA5, 0xFA7, 6, GumpButtonType.Reply, 0 );
				//AddLabel( 50, 320, 0x489, "a wreath deed" );  //# 1test

				AddButton( 120, 310, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 154, 312, 100, 35, 1011012, false, false ); // CANCEL
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				if ( m_Ticket.Deleted || m_From == null || m_From.Backpack == null || !m_Ticket.IsChildOf( sender.Mobile.Backpack ))
					return;

				//int number = 0;

				Item item = null;

				switch ( info.ButtonID )
				{
                    case 1: item = new SkinToneDeed(); /*number = 1049368;*/ break; // You have been rewarded for your dedication to Justice!.
					case 2: break; // item = new BloodPentagramPart(); /*number = 1049368;*/ break; // You have been rewarded for your dedication to Justice!.
                    case 3: item = new SkinToneDeed(); /*number = 1049368;*/ break; // You have been rewarded for your dedication to Justice!.
					case 4: item = new EtherealBeetle(); /*number = 1049368;*/ break; // You have been rewarded for your dedication to Justice!.
					case 5: item = new SkinToneDeed(); /*number = 1049368;*/ break; // You have been rewarded for your dedication to Justice!.
					case 6: item = new SpecialHairDye(); /*number = 1049368;*/ break; // You have been rewarded for your dedication to Justice!.
				}

				if ( item != null)
				{
					m_Ticket.Delete();

					//m_From.SendLocalizedMessage( number );
					m_From.AddToBackpack( item );

				}
			}
		}
	}
}