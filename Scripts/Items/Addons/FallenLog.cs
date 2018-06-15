using System;
using Server;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
	public class FallenLogAddon : BaseAddon
	{
		public override BaseAddonDeed Deed { get { return new FallenLogDeed(); } }

		[Constructable]
		public FallenLogAddon()
			: this( true )
		{
		}

		[Constructable]
		public FallenLogAddon( bool east )
			: base()
		{
			if ( east )
			{
				AddComponent( new AddonComponent( 0x0CF5 ), -1, 0, 0 );
				AddComponent( new AddonComponent( 0x0CF6 ), 0, 0, 0 );
				AddComponent( new AddonComponent( 0x0CF7 ), 1, 0, 0 );
			}
			else
			{
				AddComponent( new AddonComponent( 0x0CF4 ), 0, 0, 0 );
				AddComponent( new AddonComponent( 0x0CF3 ), 0, -1, 0 );
			}
		}

		public FallenLogAddon( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			/*int version = */
			reader.ReadEncodedInt();
		}
	}

	public class FallenLogDeed : BaseAddonDeed
	{
		public override int LabelNumber { get { return 1071088; } } // Fallen Log
		public override BaseAddon Addon { get { return new FallenLogAddon( m_East ); } }

		private bool m_East;

		[Constructable]
		public FallenLogDeed()
			: base()
		{
			LootType = LootType.Blessed;
		}

		public FallenLogDeed( Serial serial )
			: base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				from.CloseGump( typeof( InternalGump ) );
				from.SendGump( new InternalGump( this ) );
			}
			else
				from.SendLocalizedMessage( 1042038 ); // You must have the object in your backpack to use it.    
		}

		private void SendTarget( Mobile m )
		{
			base.OnDoubleClick( m );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			/*int version = */
			reader.ReadEncodedInt();
		}

		private class InternalGump : Gump
		{
			//public override int TypeID { get { return 0x2341; } }

			private FallenLogDeed m_Deed;

			public InternalGump( FallenLogDeed deed )
				: base( 60, 36 )
			{
				m_Deed = deed;

				AddPage( 0 );

				AddBackground( 0, 0, 273, 324, 0x13BE );
				AddImageTiled( 10, 10, 253, 20, 0xA40 );
				AddImageTiled( 10, 40, 253, 244, 0xA40 );
				AddImageTiled( 10, 294, 253, 20, 0xA40 );
				AddAlphaRegion( 10, 10, 253, 304 );

				AddButton( 10, 294, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 45, 296, 450, 20, 1060051, 0x7FFF, false, false ); // CANCEL

				AddHtmlLocalized( 14, 12, 273, 20, 1076170, 0x7FFF, false, false ); // Choose Direction

				AddPage( 1 );

				AddButton( 19, 49, 0x845, 0x846, 100, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 44, 47, 213, 20, 1071192, 0x7FFF, false, false ); // Fallen Log (East)
				AddButton( 19, 73, 0x845, 0x846, 101, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 44, 71, 213, 20, 1071193, 0x7FFF, false, false ); // Fallen Log (South)
			}

			public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
			{
				if ( m_Deed == null || m_Deed.Deleted )
					return;

				int buttonId = info.ButtonID;

				if ( buttonId == 100 || buttonId == 101 )
				{
					m_Deed.m_East = ( buttonId == 100 );
					m_Deed.SendTarget( sender.Mobile );
				}
			}
		}
	}
}