using System;
using Server;

namespace Server.Items
{
	public class BlueDecorativeRugAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new BlueDecorativeRugDeed(); } }

		[Constructable]
		public BlueDecorativeRugAddon()
		{
			AddComponent( new AddonComponent( 0x0AEF ), -1, -1, 0 );
			AddComponent( new AddonComponent( 0x0AF3 ),  0, -1, 0 );
			AddComponent( new AddonComponent( 0x0AF1 ),  1, -1, 0 );
			AddComponent( new AddonComponent( 0x0AF2 ), -1,  0, 0 );
			AddComponent( new AddonComponent( 0x0AFA ),  0,  0, 0 );
			AddComponent( new AddonComponent( 0x0AF4 ),  1,  0, 0 );
			AddComponent( new AddonComponent( 0x0AF0 ), -1,  1, 0 );
			AddComponent( new AddonComponent( 0x0AF5 ),  0,  1, 0 );
			AddComponent( new AddonComponent( 0x0AEE ),  1,  1, 0 );
		}

		public BlueDecorativeRugAddon( Serial serial ) : base( serial )
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

	public class BlueDecorativeRugDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new BlueDecorativeRugAddon(); } }
		public override int LabelNumber{ get{ return 1076589; } } // BlueDecorative Rug

		[Constructable]
		public BlueDecorativeRugDeed()
		{
		}

		public BlueDecorativeRugDeed( Serial serial ) : base( serial )
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
