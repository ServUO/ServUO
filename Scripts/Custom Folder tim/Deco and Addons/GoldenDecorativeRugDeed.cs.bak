using System;
using Server;

namespace Server.Items
{
	public class GoldenDecorativeRugAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new GoldenDecorativeRugDeed(); } }

		[Constructable]
		public GoldenDecorativeRugAddon()
		{
			AddComponent( new AddonComponent( 0x0ADC ), -1, -1, 0 );
			AddComponent( new AddonComponent( 0x0AE0 ),  0, -1, 0 );
			AddComponent( new AddonComponent( 0x0ADE ),  1, -1, 0 );
			AddComponent( new AddonComponent( 0x0ADF ), -1,  0, 0 );
			AddComponent( new AddonComponent( 0x0ADA ),  0,  0, 0 );
			AddComponent( new AddonComponent( 0x0AE1 ),  1,  0, 0 );
			AddComponent( new AddonComponent( 0x0ADD ), -1,  1, 0 );
			AddComponent( new AddonComponent( 0x0AE2 ),  0,  1, 0 );
			AddComponent( new AddonComponent( 0x0ADB ),  1,  1, 0 );
		}

		public GoldenDecorativeRugAddon( Serial serial ) : base( serial )
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

	public class GoldenDecorativeRugDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new GoldenDecorativeRugAddon(); } }
		public override int LabelNumber{ get{ return 1076586; } } // Golden Decorative Rug

		[Constructable]
		public GoldenDecorativeRugDeed()
		{
		}

		public GoldenDecorativeRugDeed( Serial serial ) : base( serial )
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
