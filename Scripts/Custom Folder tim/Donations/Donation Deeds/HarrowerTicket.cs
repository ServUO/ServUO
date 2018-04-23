using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
	public class HarrowerTicket : Item
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
		public HarrowerTicket() : base( 0x14F0 )
		{
			Weight = 6.0;
			LootType = LootType.Cursed;
			Name = "a super champion reward deed";
			Hue = 1161;
		}

		public HarrowerTicket( Serial serial ) : base( serial )
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

		private class InternalTarget : Target
		{
			private HarrowerTicket m_Ticket;

			public InternalTarget( HarrowerTicket ticket ) : base( 2, false, TargetFlags.None )
			{
				m_Ticket = ticket;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted == m_Ticket )
				{
					from.SendLocalizedMessage( 501928 ); // You can't target the same ticket!
				}
				else if ( targeted is HarrowerTicket )
				{
					HarrowerTicket theirTicket = targeted as HarrowerTicket;
					Mobile them = theirTicket.m_Owner;

					if ( them == null || them.Deleted )
					{
						from.SendLocalizedMessage( 501930 ); // That is not a valid ticket.
					}
					else
					{
						from.SendGump( new InternalGump( from, m_Ticket ) );
						them.SendGump( new InternalGump( them, theirTicket ) );
					}
				}
				else if ( targeted is Item && ((Item)targeted).ItemID == 0x14F0 )
				{
					from.SendLocalizedMessage( 501931 ); // You need to find another ticket marked NEW PLAYER.
				}
				else
				{
					from.SendLocalizedMessage( 501929 ); // You will need to select a ticket.
				}
			}
		}

		private class InternalGump : Gump
		{
			private Mobile m_From;
			private HarrowerTicket m_Ticket;

			public InternalGump( Mobile from, HarrowerTicket ticket ) : base( 50, 50 )
			{
				m_From = from;
				m_Ticket = ticket;

				AddBackground( 0, 0, 400, 385, 0xA28 );

				AddHtml(30, 45, 340, 70,"Wow! You have defeated the Harrower!, for your courageousness we have given you a choice of rewards, you may choose only one though! so choose carefully brave hero...",true,true);

				AddButton( 46, 128, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0 );
				AddHtml(80,129,240,24,"a Limited Seven GM Skillball",true,false);
				//AddLabel( 80, 128, 0x489, "a Seven GM Skillball" );  //# 1test

				AddButton( 46, 163, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0 );
                AddHtml(80, 164, 240, 24, "a Limited Seven GM Skillball", true, false);
				//AddLabel( 80, 163, 0x489, "an Ethereal WolfRider" );  //# 1test

				AddButton( 46, 198, 0xFA5, 0xFA7, 3, GumpButtonType.Reply, 0 );
				AddHtml(80,199,240,24, "A Limited Seven GM Skillball",true,false);
				//AddLabel( 80, 198, 0x489, "a Special Hair Restyling Deed" );  //# 1test

				AddButton( 46, 233, 0xFA5, 0xFA7, 4, GumpButtonType.Reply, 0 );
				AddHtml(80,234,240,24,"a Black Hair Dye",true,false);
				//AddLabel( 80, 233, 0x489, "a Black Hair Dye" );  //# 1test

				AddButton( 46, 268, 0xFA5, 0xFA7, 5, GumpButtonType.Reply, 0 );
				AddHtml(80,269,240,24,"a hooded shroud of the shadows",true,false);
				//AddLabel( 80, 268, 0x489, "a hooded shroud of the shadows" );  //# 1test

				//AddButton( 46, 303, 0xFA5, 0xFA7, 6, GumpButtonType.Reply, 0 );
				//AddLabel( 50, 320, 0x489, "a wreath deed" );  //# 1test

				AddButton( 120, 340, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 154, 342, 100, 35, 1011012, false, false ); // CANCEL
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				if ( m_Ticket == null || m_Ticket.Deleted || !m_Ticket.IsChildOf( sender.Mobile.Backpack ) )
					return;

				//int number = 0;

				Item item = null;
				Item item2 = null;

				switch ( info.ButtonID )
				{
					case 1: item = new DyeTub(); /*number = 1049368;*/ break; // You have been rewarded for your dedication to Justice!.
					case 2: item = new DyeTub(); /*number = 1049368;*/ break; // You have been rewarded for your dedication to Justice!.
					case 3: item = new DyeTub(); /*number = 1049368;*/ break; // You have been rewarded for your dedication to Justice!.
                    case 4: item = new DyeTub(); /*number = 1049368;*/ break; // You have been rewarded for your dedication to Justice!.
					case 5: item = new HoodedShroudOfShadows(); /*number = 1049368;*/ break; // You have been rewarded for your dedication to Justice!.
					case 6: item = new DyeTub(); item2 = new Dyes(); /*number = 1049368;*/ break; // You have been rewarded for your dedication to Justice!.
				}

				if ( item != null )
				{
					m_Ticket.Delete();

					//m_From.SendLocalizedMessage( number );
					m_From.AddToBackpack( item );

					if ( item2 != null)
						m_From.AddToBackpack( item2 );
				}
			}
		}
	}
}