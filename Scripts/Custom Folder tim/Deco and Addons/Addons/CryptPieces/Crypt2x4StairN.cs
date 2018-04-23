using System;
using Server;

namespace Server.Items
{
	public class Crypt2x4StairN : BaseAddon
	{
		[ Constructable ]
		public Crypt2x4StairN()
		{
			AddComponent( new AddonComponent( 1822 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 1823 ), 1, 1, 5 );
			AddComponent( new AddonComponent( 1822 ), 1, 0, 5 );
			AddComponent( new AddonComponent( 1822 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 1823 ), 1, 0, 10 );
			AddComponent( new AddonComponent( 1823 ), 1, 2, 0 );
			AddComponent( new AddonComponent( 1822 ), 1, -1, 10 );
			AddComponent( new AddonComponent( 1822 ), 1, -1, 5 );
			AddComponent( new AddonComponent( 1822 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 1823 ), 1, -1, 15 );
			AddComponent( new AddonComponent( 1823 ), 0, 2, 0 );
			AddComponent( new AddonComponent( 1823 ), 0, 1, 5 );
			AddComponent( new AddonComponent( 1823 ), 0, 0, 10 );
			AddComponent( new AddonComponent( 1823 ), 0, -1, 15 );
		}

		public Crypt2x4StairN( Serial serial ) : base( serial )
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