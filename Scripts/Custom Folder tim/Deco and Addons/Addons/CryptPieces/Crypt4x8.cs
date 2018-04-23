using System;
using Server;

namespace Server.Items
{
	public class Crypt4x8 : BaseAddon
	{
		[ Constructable ]
		public Crypt4x8()
		{
			AddComponent( new AddonComponent( 1310 ), 1, 4, 0 );
			AddComponent( new AddonComponent( 1311 ), -1, -3, 0 );
			AddComponent( new AddonComponent( 1310 ), 0, -3, 0 );
			AddComponent( new AddonComponent( 1312 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 1311 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 1312 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 1311 ), 1, -2, 0 );
			AddComponent( new AddonComponent( 1309 ), 0, -2, 0 );
			AddComponent( new AddonComponent( 1312 ), -1, -2, 0 );
			AddComponent( new AddonComponent( 1312 ), 1, 3, 0 );
			AddComponent( new AddonComponent( 1310 ), 1, 2, 0 );
			AddComponent( new AddonComponent( 1312 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 1310 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 1311 ), 2, -2, 0 );
			AddComponent( new AddonComponent( 1310 ), 2, -1, 0 );
			AddComponent( new AddonComponent( 1310 ), 2, 0, 0 );
			AddComponent( new AddonComponent( 1310 ), 2, -3, 0 );
			AddComponent( new AddonComponent( 1309 ), 2, 2, 0 );
			AddComponent( new AddonComponent( 1309 ), 2, 3, 0 );
			AddComponent( new AddonComponent( 1312 ), 2, 4, 0 );
			AddComponent( new AddonComponent( 1309 ), 2, 1, 0 );
			AddComponent( new AddonComponent( 1309 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 1312 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 1312 ), -1, 3, 0 );
			AddComponent( new AddonComponent( 1311 ), -1, 2, 0 );
			AddComponent( new AddonComponent( 1309 ), 0, 4, 0 );
			AddComponent( new AddonComponent( 1309 ), -1, 4, 0 );
			AddComponent( new AddonComponent( 1309 ), 0, 2, 0 );
			AddComponent( new AddonComponent( 1311 ), 0, 3, 0 );
			AddComponent( new AddonComponent( 1309 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 1311 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 1312 ), 1, -3, 0 );
		}

		public Crypt4x8( Serial serial ) : base( serial )
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