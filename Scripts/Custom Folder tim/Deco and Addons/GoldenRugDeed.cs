using System;
using Server;

namespace Server.Items
{
	public class GoldenRugAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new GoldenRugDeed(); } }

		[Constructable]
		public GoldenRugAddon()
		{
			AddComponent( new AddonComponent( 0x0AB8 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 0x0AB4 ),  0, -1, 0 );
			AddComponent( new AddonComponent( 0x0AB9 ),  1, -1, 0 );
			AddComponent( new AddonComponent( 0x0AB7 ), -1,  0, 0 );
			AddComponent( new AddonComponent( 0x0AB3 ),  0,  0, 0 );
			AddComponent( new AddonComponent( 0x0AB5 ),  1,  0, 0 );
			AddComponent( new AddonComponent( 0x0ABB ), -1,  1, 0 );
			AddComponent( new AddonComponent( 0x0AB6 ),  0,  1, 0 );
			AddComponent( new AddonComponent( 0x0ABA ),  1,  1, 0 );
		}

		public GoldenRugAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class GoldenRugDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new GoldenRugAddon(); } }

		[Constructable]
		public GoldenRugDeed()
		{
			Name = "Large golden rug";
		}

		public GoldenRugDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
