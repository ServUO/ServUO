using System;
using Server;

namespace Server.Items
{
	public class Crypt2x4StairS : BaseAddon
	{
		[ Constructable ]
		public Crypt2x4StairS()
		{
			AddComponent( new AddonComponent( 1847 ), 0, 0, 5 );
			AddComponent( new AddonComponent( 1847 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 1847 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 1822 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 1847 ), 1, 0, 5 );
			AddComponent( new AddonComponent( 1822 ), 1, 1, 5 );
			AddComponent( new AddonComponent( 1822 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 1847 ), 1, 1, 10 );
			AddComponent( new AddonComponent( 1822 ), 1, 2, 10 );
			AddComponent( new AddonComponent( 1822 ), 1, 2, 5 );
			AddComponent( new AddonComponent( 1822 ), 1, 2, 0 );
			AddComponent( new AddonComponent( 1847 ), 1, 2, 15 );
			AddComponent( new AddonComponent( 1847 ), 0, 2, 15 );
			AddComponent( new AddonComponent( 1822 ), 0, 2, 0 );
			AddComponent( new AddonComponent( 1822 ), 0, 2, 5 );
			AddComponent( new AddonComponent( 1822 ), 0, 2, 10 );
			AddComponent( new AddonComponent( 1847 ), 0, 1, 10 );
		}

		public Crypt2x4StairS( Serial serial ) : base( serial )
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