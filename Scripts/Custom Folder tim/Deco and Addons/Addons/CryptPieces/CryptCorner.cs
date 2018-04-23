using System;
using Server;

namespace Server.Items
{
	public class CryptCorner : BaseAddon
	{
		[ Constructable ]
		public CryptCorner()
		{
			AddComponent( new AddonComponent( 200 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 201 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 204 ), -1, -1, 0 );
		}

		public CryptCorner( Serial serial ) : base( serial )
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