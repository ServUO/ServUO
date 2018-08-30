using System;
using Server;
using Server.Gumps;
using Server.Network;

//02c7

namespace Server.Items
{
	public class SilverSaplingReplica : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new SilverSaplingReplicaDeed(); } }
		public override bool RetainDeedHue{ get{ return true; } }

		[Constructable]
		public SilverSaplingReplica( bool potted )
		{
			if ( potted )
			{
				AddLightComponent( new AddonComponent( 0x42C8 ), 0, 0, 0 );
			}
			else
			{
				AddLightComponent( new AddonComponent( 0x42C7 ), 0, 0, 0 );
			}
		}
		
		private void AddLightComponent( AddonComponent component, int x, int y, int z )
		{
			component.Light = LightType.Circle150;
			AddComponent( component, x, y, z );
		}


		public SilverSaplingReplica( Serial serial ) : base( serial )
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

	public class SilverSaplingReplicaDeed : BaseAddonDeed, ITokunoDyable
	{
		private bool m_Potted;

		public override BaseAddon Addon{ get{ return new SilverSaplingReplica( m_Potted ); } }
		public override int LabelNumber{ get{ return 1113934; } } // Silver Sapling

		[Constructable]
		public SilverSaplingReplicaDeed()
		{
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
			private SilverSaplingReplicaDeed m_Deed;

			public InternalGump( SilverSaplingReplicaDeed deed ) : base( 150, 50 )
			{
				m_Deed = deed;

				AddBackground( 0, 0, 350, 250, 0xA28 );

				AddItem( 112, 35, 17095 );
				AddButton( 70, 35, 0x868, 0x869, 1, GumpButtonType.Reply, 0 ); // South

				AddItem( 242, 52, 17096 );
				AddButton( 185, 35, 0x868, 0x869, 2, GumpButtonType.Reply, 0 ); // East
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				if ( m_Deed.Deleted || info.ButtonID == 0 )
					return;

				m_Deed.m_Potted = (info.ButtonID != 1);
				m_Deed.SendTarget( sender.Mobile );
			}
		}

		public SilverSaplingReplicaDeed( Serial serial ) : base( serial )
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