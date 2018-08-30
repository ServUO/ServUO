using System;
using Server;

namespace Server.Items
{
	public class CryptWallN : BaseAddon
	{
		[ Constructable ]
		public CryptWallN()
		{
			AddComponent( new AddonComponent( 200 ), 0, -1, 0 );
		}

		public CryptWallN( Serial serial ) : base( serial )
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