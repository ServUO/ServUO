using System;
using Server;
using Server.Items;

namespace Knives.TownHouses
{
	public class RentalLicense : Item
	{
		private Mobile c_Owner;

		public Mobile Owner{ get{ return c_Owner; } set{ c_Owner = value; InvalidateProperties(); } }

		public RentalLicense() : base( 0x14F0 )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			if ( c_Owner != null )
				list.Add( "a renter's license belonging to " + c_Owner.Name );
			else
				list.Add( "a renter's license" );
		}

		public override void OnDoubleClick( Mobile m )
		{
			if ( c_Owner == null )
				c_Owner = m;
		}

		public RentalLicense( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( c_Owner );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			c_Owner = reader.ReadMobile();
		}
	}
}