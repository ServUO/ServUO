using System;
using Server;

namespace Server.Items
{
	public class Crypt8x2 : BaseAddon
	{
		[ Constructable ]
		public Crypt8x2()
		{
			AddComponent( new AddonComponent( 1309 ), 5, 0, 0 );
			AddComponent( new AddonComponent( 1312 ), 3, 1, 0 );
			AddComponent( new AddonComponent( 1310 ), 3, 0, 0 );
			AddComponent( new AddonComponent( 1312 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 1310 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 1312 ), 4, 0, 0 );
			AddComponent( new AddonComponent( 1312 ), 4, 1, 0 );
			AddComponent( new AddonComponent( 1310 ), 2, 0, 0 );
			AddComponent( new AddonComponent( 1309 ), 2, 1, 0 );
			AddComponent( new AddonComponent( 1309 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 1312 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 1310 ), -2, 0, 0 );
			AddComponent( new AddonComponent( 1309 ), -3, 0, 0 );
			AddComponent( new AddonComponent( 1309 ), -4, 0, 0 );
			AddComponent( new AddonComponent( 1312 ), -4, 1, 0 );
			AddComponent( new AddonComponent( 1309 ), -3, 1, 0 );
			AddComponent( new AddonComponent( 1311 ), -2, 1, 0 );
			AddComponent( new AddonComponent( 1309 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 1311 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 1310 ), 5, 1, 0 );
		}

		public Crypt8x2( Serial serial ) : base( serial )
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