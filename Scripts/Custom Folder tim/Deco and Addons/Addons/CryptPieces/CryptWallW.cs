using System;
using Server;

namespace Server.Items
{
	public class CryptWallW : BaseAddon
	{
		[ Constructable ]
		public CryptWallW()
		{
			AddComponent( new AddonComponent( 201 ), -1, 0, 0 );
		}

		public CryptWallW( Serial serial ) : base( serial )
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