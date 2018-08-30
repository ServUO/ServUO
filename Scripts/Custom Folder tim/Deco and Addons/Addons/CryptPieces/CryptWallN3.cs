using System;
using Server;

namespace Server.Items
{
	public class CryptWallN3 : BaseAddon
	{
		[ Constructable ]
		public CryptWallN3()
		{
			AddComponent( new AddonComponent( 200 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 200 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 200 ), -1, -1, 0 );
		}

		public CryptWallN3( Serial serial ) : base( serial )
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