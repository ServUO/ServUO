using System;
using Server;

namespace Server.Items
{
	public class Crypt8x8 : BaseAddon
	{
		[ Constructable ]
		public Crypt8x8()
		{
			AddComponent( new AddonComponent( 1310 ), 3, 4, 0 );
			AddComponent( new AddonComponent( 1309 ), 2, 4, 0 );
			AddComponent( new AddonComponent( 1311 ), 2, 3, 0 );
			AddComponent( new AddonComponent( 1310 ), -3, -3, 0 );
			AddComponent( new AddonComponent( 1311 ), -2, -3, 0 );
			AddComponent( new AddonComponent( 1310 ), -1, -3, 0 );
			AddComponent( new AddonComponent( 1312 ), 0, -3, 0 );
			AddComponent( new AddonComponent( 1311 ), 1, -3, 0 );
			AddComponent( new AddonComponent( 1312 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 1310 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 1310 ), -1, -2, 0 );
			AddComponent( new AddonComponent( 1311 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 1309 ), 1, 2, 0 );
			AddComponent( new AddonComponent( 1310 ), 1, 3, 0 );
			AddComponent( new AddonComponent( 1309 ), 1, 4, 0 );
			AddComponent( new AddonComponent( 1311 ), -2, 2, 0 );
			AddComponent( new AddonComponent( 1311 ), 4, 4, 0 );
			AddComponent( new AddonComponent( 1312 ), 4, -1, 0 );
			AddComponent( new AddonComponent( 1310 ), 4, -2, 0 );
			AddComponent( new AddonComponent( 1309 ), 4, 1, 0 );
			AddComponent( new AddonComponent( 1311 ), 4, 0, 0 );
			AddComponent( new AddonComponent( 1309 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 1312 ), 0, -2, 0 );
			AddComponent( new AddonComponent( 1312 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 1311 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 1310 ), -1, 4, 0 );
			AddComponent( new AddonComponent( 1312 ), -2, 4, 0 );
			AddComponent( new AddonComponent( 1311 ), -2, 3, 0 );
			AddComponent( new AddonComponent( 1309 ), -1, 3, 0 );
			AddComponent( new AddonComponent( 1312 ), 3, 3, 0 );
			AddComponent( new AddonComponent( 1309 ), 3, 0, 0 );
			AddComponent( new AddonComponent( 1309 ), 3, 1, 0 );
			AddComponent( new AddonComponent( 1310 ), 3, -2, 0 );
			AddComponent( new AddonComponent( 1309 ), 3, -1, 0 );
			AddComponent( new AddonComponent( 1312 ), 2, -3, 0 );
			AddComponent( new AddonComponent( 1310 ), 3, -3, 0 );
			AddComponent( new AddonComponent( 1311 ), 4, -3, 0 );
			AddComponent( new AddonComponent( 1309 ), -3, 2, 0 );
			AddComponent( new AddonComponent( 1310 ), -3, 3, 0 );
			AddComponent( new AddonComponent( 1309 ), -3, 4, 0 );
			AddComponent( new AddonComponent( 1311 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 1311 ), 0, 3, 0 );
			AddComponent( new AddonComponent( 1309 ), 0, 2, 0 );
			AddComponent( new AddonComponent( 1310 ), -1, 2, 0 );
			AddComponent( new AddonComponent( 1312 ), 0, 4, 0 );
			AddComponent( new AddonComponent( 1312 ), 4, 3, 0 );
			AddComponent( new AddonComponent( 1310 ), 4, 2, 0 );
			AddComponent( new AddonComponent( 1312 ), 3, 2, 0 );
			AddComponent( new AddonComponent( 1310 ), 2, 2, 0 );
			AddComponent( new AddonComponent( 1312 ), 2, 1, 0 );
			AddComponent( new AddonComponent( 1310 ), 2, 0, 0 );
			AddComponent( new AddonComponent( 1312 ), 2, -1, 0 );
			AddComponent( new AddonComponent( 1310 ), 2, -2, 0 );
			AddComponent( new AddonComponent( 1309 ), 1, -2, 0 );
			AddComponent( new AddonComponent( 1311 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 1309 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 1309 ), -2, 1, 0 );
			AddComponent( new AddonComponent( 1311 ), -2, 0, 0 );
			AddComponent( new AddonComponent( 1309 ), -2, -1, 0 );
			AddComponent( new AddonComponent( 1309 ), -2, -2, 0 );
			AddComponent( new AddonComponent( 1309 ), -3, -2, 0 );
			AddComponent( new AddonComponent( 1312 ), -3, -1, 0 );
			AddComponent( new AddonComponent( 1311 ), -3, 0, 0 );
			AddComponent( new AddonComponent( 1310 ), -3, 1, 0 );
		}

		public Crypt8x8( Serial serial ) : base( serial )
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