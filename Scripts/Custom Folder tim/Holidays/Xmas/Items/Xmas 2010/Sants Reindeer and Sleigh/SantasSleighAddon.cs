using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
	public class SantasSleigh : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new SantasSleighDeed(); } }

		[Constructable]
		public SantasSleigh( bool east )
		{
			if ( east )
			{
				AddonComponent ac = null;
				ac = new AddonComponent( 14964 );
				AddComponent( ac, 0, 0, 0);
				ac.Name = "Santa's Sleigh";

				ac = new AddonComponent( 14963 );
				AddComponent( ac, 1, 0, 0);
				ac.Name = "Santa's Sleigh";				
			}
			else
			{
				AddonComponent ac = null;
				ac = new AddonComponent( 14984 );
				AddComponent( ac, 0, 0, 0);
				ac.Name = "Santa's Sleigh";

				ac = new AddonComponent( 14983 );
				AddComponent( ac, 0, 1, 0);
				ac.Name = "Santa's Sleigh";				
			}
		}

		public SantasSleigh( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class SantasSleighDeed : BaseAddonDeed
	{
		private bool m_East;

		public override BaseAddon Addon{ get{ return new SantasSleigh( m_East ); } }

		[Constructable]
		public SantasSleighDeed()
		{
			Name = "A deed for Santa's Sleigh";
			LootType = LootType.Blessed;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				from.CloseGump( typeof( InternalGump ) );
				from.SendGump( new InternalGump( this ) );
			}
			else
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
		}

		private void SendTarget( Mobile m )
		{
			base.OnDoubleClick( m );
		}

		private class InternalGump : Gump
		{
			private SantasSleighDeed m_Deed;

			public InternalGump( SantasSleighDeed deed ) : base( 150, 50 )
			{
				m_Deed = deed;

				AddBackground( 0, 0, 350, 250, 0xA28 );

				AddItem( 90, 52, 14984 );
				AddItem( 73, 53, 14983 );
				AddButton( 70, 35, 0x868, 0x869, 1, GumpButtonType.Reply, 0 ); // South

				AddItem( 217, 51, 14964 );
				AddItem( 244, 52, 14963 );
				AddButton( 185, 35, 0x868, 0x869, 2, GumpButtonType.Reply, 0 ); // East
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				if ( m_Deed.Deleted || info.ButtonID == 0 )
					return;

				m_Deed.m_East = (info.ButtonID != 1);
				m_Deed.SendTarget( sender.Mobile );
			}
		}

		public SantasSleighDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}