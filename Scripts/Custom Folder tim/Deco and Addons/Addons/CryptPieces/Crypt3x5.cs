using System;
using Server;

namespace Server.Items
{
	public class Crypt3x5 : BaseAddon
	{
		[ Constructable ]
		public Crypt3x5()
		{
			AddComponent( new AddonComponent( 1309 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 1311 ), -1, -2, 0 );
			AddComponent( new AddonComponent( 1310 ), 0, -2, 0 );
			AddComponent( new AddonComponent( 1312 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 1312 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 1312 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 1311 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 1309 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 1311 ), 0, 2, 0 );
			AddComponent( new AddonComponent( 1309 ), -1, 2, 0 );
			AddComponent( new AddonComponent( 1312 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 1312 ), 1, -2, 0 );
			AddComponent( new AddonComponent( 1310 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 1312 ), 1, 2, 0 );
			AddComponent( new AddonComponent( 1311 ), 0, 0, 0 );
		}

		public Crypt3x5( Serial serial ) : base( serial )
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