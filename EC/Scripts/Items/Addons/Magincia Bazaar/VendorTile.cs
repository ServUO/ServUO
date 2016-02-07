using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Regions;
using Server.Multis;
using Server.Targeting;

namespace Server.Items
{
	public class TownContractOfEmployment : Item
	{
		public override int LabelNumber { get { return 1041243; } }

		[Constructable]
		public TownContractOfEmployment( ) : base( 0x14F0 )
		{
			this.LootType = LootType.Blessed;
			Name = "a Public Vendor Contract of Employment";
		}

		public TownContractOfEmployment( Serial serial ) : base( serial ) { }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt( );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 );
			}
			else
			{
				from.Target = new InternalTarget( this );
				from.SendMessage( 3, "Target a player vendor tile to place your vendor" );
			}
		}

		private class InternalTarget : Target
		{

			private TownContractOfEmployment m_tcoe;
			public InternalTarget( TownContractOfEmployment TCOE ) : base( 6, true, TargetFlags.None )
			{
				m_tcoe = TCOE;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				PlayerVendorTile pvt = o as PlayerVendorTile;

				if ( o is PlayerVendorTile )
				{

					Mobile v = new PlayerVendor( from, BaseHouse.FindHouseAt( from ) );
					v.Location = pvt.Location;
					v.Direction = from.Direction & Direction.Mask;
					v.Map = pvt.Map;
					v.SayTo( from, 503246 );
					m_tcoe.Delete( );
					pvt.Delete( );
				}
				else
				{
					from.SendMessage( 33, "That is not a Player vendor Tile" );
				}
			}
		}
	}

	public class PlayerVendorTile : Item
	{
		[Constructable]
		public PlayerVendorTile( ) : base( 0x181D )
		{
			Name = "a Player Vendor Tile";
			Hue = 1153;
			Weight = 1.0;
			Movable = false;
		}

		public PlayerVendorTile( int itemID, Item target ) : base( itemID )
		{
			Movable = false;
		}

	public override bool Decays{ get{ return false; } }

		public PlayerVendorTile( Serial serial ) : base( serial ) { }
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt( );
		}
	}
}