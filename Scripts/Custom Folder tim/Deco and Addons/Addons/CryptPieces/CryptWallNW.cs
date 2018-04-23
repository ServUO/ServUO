using System;
using Server;

namespace Server.Items
{
	public class CryptWallNW : BaseAddon
	{
		[ Constructable ]
		public CryptWallNW()
		{
			AddComponent( new AddonComponent( 199 ), -1, -1, 0 );
		}

		public CryptWallNW( Serial serial ) : base( serial )
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