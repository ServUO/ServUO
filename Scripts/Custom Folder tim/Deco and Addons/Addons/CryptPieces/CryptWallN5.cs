using System;
using Server;

namespace Server.Items
{
	public class CryptWallN5 : BaseAddon
	{
		[ Constructable ]
		public CryptWallN5()
		{
			AddComponent( new AddonComponent( 200 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 200 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 200 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 200 ), 2, -1, 0 );
			AddComponent( new AddonComponent( 200 ), -2, -1, 0 );
		}

		public CryptWallN5( Serial serial ) : base( serial )
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