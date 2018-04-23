using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server.Gumps;

namespace Server.Items
{
	public class GMSafeTradeTicket : Item
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
		public GMSafeTradeTicket() : base( 0x14F0 )
		{
			Weight = 1.0;
			LootType = LootType.Blessed;
			Name = "a safe trade ticket";
			Hue = 92;
		}

		public GMSafeTradeTicket( Serial serial ) : base( serial )
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

			public InternalGump( Mobile from, GMSafeTradeTicket ticket ) : base( 50, 50 )
			{
          		this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			AddPage(0);
			AddBackground(216, 101, 310, 254, 2620);
			AddLabel(240, 203, 900, "This deed entitles the player to a GM Safe ");
			AddLabel(322, 164, 1280, "GM Trade Deed");
			AddImage(335, 113, 9804);
			AddLabel(240, 221, 900, "trade between another player of his choice.");
			AddLabel(240, 277, 900, "A GM will handle both goods and the trade");
			AddLabel(240, 297, 900, "will be safely made via the GM.");


			}



		}
	}
}