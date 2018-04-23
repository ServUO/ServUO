using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
	public class SnowTreeAddon : BaseAddon
	{
		public override BaseAddonDeed Deed { get { return new SnowTreeDeed(); } }

		[Constructable]
		public SnowTreeAddon( bool trunk )
		{
			if ( !trunk )
			{
				AddComponent( new LocalizedAddonComponent( 0x36A0, 1071103 ), 0, 0, 0 );
				Hue = 1153;
			}

			AddComponent( new LocalizedAddonComponent( 0xCE0, 1071103 ), 0, 0, 0 );
		}

		public SnowTreeAddon( Serial serial )
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

	public class SnowTreeDeed : BaseAddonDeed
	{
		public override BaseAddon Addon { get { return new SnowTreeAddon( m_Trunk ); } }
		public override int LabelNumber { get { return 1071103; } } // Snow Tree

		private bool m_Trunk;

		[Constructable]
		public SnowTreeDeed()
		{
			LootType = LootType.Blessed;
		}

		public SnowTreeDeed( Serial serial )
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
				from.SendLocalizedMessage( 1062334 ); // This item must be in your backpack to be used.
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

			private SnowTreeDeed m_Deed;

			public InternalGump( SnowTreeDeed deed )
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
				AddHtmlLocalized( 44, 47, 213, 20, 1071103, 0x7FFF, false, false ); // Snow Tree
				AddButton( 19, 73, 0x845, 0x846, 101, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 44, 71, 213, 20, 1071300, 0x7FFF, false, false ); // Snow Tree (trunk)
			}

			public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
			{
				if ( m_Deed == null || m_Deed.Deleted )
					return;

				int buttonId = info.ButtonID;

				if ( buttonId == 100 || buttonId == 101 )
				{
					m_Deed.m_Trunk = ( buttonId == 101 );
					m_Deed.SendTarget( sender.Mobile );
				}
			}
		}
	}
}