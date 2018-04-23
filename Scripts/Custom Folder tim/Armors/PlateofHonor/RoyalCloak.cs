using System;
using Server;
using Server.Items;

namespace Server.Items
{  
	public class RoyalCloak : BaseCloak
	{
		[Constructable]
		public RoyalCloak() : this( 0 )
		{
		}

		[Constructable]
		public RoyalCloak( int hue ) : base( 11012 )
		{
			Weight = 4.0;
                        Name = "Royal Cloak";
		}

		public RoyalCloak( Serial serial ) : base( serial )
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