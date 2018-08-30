using System;
using Server;

namespace Server.Items
{
	public class Crypt5x2StairW : BaseAddon
	{
		[ Constructable ]
		public Crypt5x2StairW()
		{
			AddComponent( new AddonComponent( 1846 ), 2, 0, 0 );

			AddComponent( new AddonComponent( 1846 ), 1, 1, 5 );

			AddComponent( new AddonComponent( 1822 ), 1, 1, 0 );

			AddComponent( new AddonComponent( 1846 ), -2, 0, 20 );

			AddComponent( new AddonComponent( 1846 ), -2, 1, 20 );

			AddComponent( new AddonComponent( 1822 ), -2, 1, 0 );

			AddComponent( new AddonComponent( 1822 ), -2, 1, 5 );

			AddComponent( new AddonComponent( 1822 ), -2, 1, 10 );

			AddComponent( new AddonComponent( 1822 ), -2, 1, 15 );

			AddComponent( new AddonComponent( 1846 ), 2, 1, 0 );

			AddComponent( new AddonComponent( 1846 ), 0, 0, 10 );

			AddComponent( new AddonComponent( 1846 ), -1, 0, 15 );

			AddComponent( new AddonComponent( 1846 ), 1, 0, 5 );

			AddComponent( new AddonComponent( 1846 ), -1, 1, 15 );

			AddComponent( new AddonComponent( 1822 ), -1, 1, 0 );

			AddComponent( new AddonComponent( 1822 ), -1, 1, 5 );

			AddComponent( new AddonComponent( 1822 ), -1, 1, 10 );

			AddComponent( new AddonComponent( 1846 ), 0, 1, 10 );

			AddComponent( new AddonComponent( 1822 ), 0, 1, 0 );

			AddComponent( new AddonComponent( 1822 ), 0, 1, 5 );

		}

		public Crypt5x2StairW( Serial serial ) : base( serial )
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