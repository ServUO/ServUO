using System;
using Server;

namespace Server.Items
{
	public class CryptWallW3 : BaseAddon
	{
		[ Constructable ]
		public CryptWallW3()
		{
			AddComponent( new AddonComponent( 201 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 201 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 201 ), -1, -1, 0 );
		}

		public CryptWallW3( Serial serial ) : base( serial )
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