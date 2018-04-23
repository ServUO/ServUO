using System;
using Server;

namespace Server.Items
{
	public class Crypt7x7 : BaseAddon
	{
		[ Constructable ]
		public Crypt7x7()
		{
			AddComponent( new AddonComponent( 1310 ), 3, 3, 0 );
			AddComponent( new AddonComponent( 1309 ), 2, 3, 0 );
			AddComponent( new AddonComponent( 1311 ), 2, 2, 0 );
			AddComponent( new AddonComponent( 1310 ), 2, 1, 0 );
			AddComponent( new AddonComponent( 1312 ), 2, 0, 0 );
			AddComponent( new AddonComponent( 1310 ), 2, -1, 0 );
			AddComponent( new AddonComponent( 1312 ), 2, -2, 0 );
			AddComponent( new AddonComponent( 1310 ), 2, -3, 0 );
			AddComponent( new AddonComponent( 1309 ), 0, -2, 0 );
			AddComponent( new AddonComponent( 1312 ), 0, -3, 0 );
			AddComponent( new AddonComponent( 1312 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 1311 ), -1, -2, 0 );
			AddComponent( new AddonComponent( 1310 ), -1, 3, 0 );
			AddComponent( new AddonComponent( 1310 ), 1, 2, 0 );
			AddComponent( new AddonComponent( 1309 ), 1, 3, 0 );
			AddComponent( new AddonComponent( 1311 ), -2, 1, 0 );
			AddComponent( new AddonComponent( 1309 ), 1, -3, 0 );
			AddComponent( new AddonComponent( 1311 ), 1, -2, 0 );
			AddComponent( new AddonComponent( 1309 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 1311 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 1311 ), 0, 2, 0 );
			AddComponent( new AddonComponent( 1312 ), -2, 3, 0 );
			AddComponent( new AddonComponent( 1311 ), -2, 2, 0 );
			AddComponent( new AddonComponent( 1309 ), -1, 2, 0 );
			AddComponent( new AddonComponent( 1309 ), -3, 1, 0 );
			AddComponent( new AddonComponent( 1310 ), -3, 2, 0 );
			AddComponent( new AddonComponent( 1309 ), -3, 3, 0 );
			AddComponent( new AddonComponent( 1311 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 1309 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 1309 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 1310 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 1312 ), 0, 3, 0 );
			AddComponent( new AddonComponent( 1312 ), 3, 1, 0 );
			AddComponent( new AddonComponent( 1312 ), 3, 2, 0 );
			AddComponent( new AddonComponent( 1309 ), 3, -1, 0 );
			AddComponent( new AddonComponent( 1309 ), 3, 0, 0 );
			AddComponent( new AddonComponent( 1310 ), 3, -3, 0 );
			AddComponent( new AddonComponent( 1309 ), 3, -2, 0 );
			AddComponent( new AddonComponent( 1312 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 1310 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 1310 ), -1, -3, 0 );
			AddComponent( new AddonComponent( 1309 ), -2, 0, 0 );
			AddComponent( new AddonComponent( 1311 ), -2, -1, 0 );
			AddComponent( new AddonComponent( 1309 ), -2, -2, 0 );
			AddComponent( new AddonComponent( 1309 ), -2, -3, 0 );
			AddComponent( new AddonComponent( 1309 ), -3, -3, 0 );
			AddComponent( new AddonComponent( 1312 ), -3, -2, 0 );
			AddComponent( new AddonComponent( 1311 ), -3, -1, 0 );
			AddComponent( new AddonComponent( 1310 ), -3, 0, 0 );
		}

		public Crypt7x7( Serial serial ) : base( serial )
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