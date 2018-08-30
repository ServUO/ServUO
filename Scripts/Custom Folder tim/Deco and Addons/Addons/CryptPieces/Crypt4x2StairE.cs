using System;
using Server;

namespace Server.Items
{
	public class Crypt4x2StairE : BaseAddon
	{
		[ Constructable ]
		public Crypt4x2StairE()
		{
			AddComponent( new AddonComponent( 1822 ), 1, 1, 5 );
			AddComponent( new AddonComponent( 1822 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 1865 ), 1, 1, 10 );
			AddComponent( new AddonComponent( 1822 ), 2, 1, 10 );
			AddComponent( new AddonComponent( 1822 ), 2, 1, 5 );
			AddComponent( new AddonComponent( 1822 ), 2, 1, 0 );
			AddComponent( new AddonComponent( 1865 ), 2, 1, 15 );
			AddComponent( new AddonComponent( 1865 ), 2, 0, 15 );
			AddComponent( new AddonComponent( 1822 ), 2, 0, 0 );
			AddComponent( new AddonComponent( 1822 ), 2, 0, 5 );
			AddComponent( new AddonComponent( 1822 ), 2, 0, 10 );
			AddComponent( new AddonComponent( 1865 ), 1, 0, 10 );
			AddComponent( new AddonComponent( 1865 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 1865 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 1822 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 1865 ), 0, 1, 5 );
			AddComponent( new AddonComponent( 1865 ), 0, 0, 5 );
		}

		public Crypt4x2StairE( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}