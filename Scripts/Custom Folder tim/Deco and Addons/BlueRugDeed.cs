using System;
using Server;

namespace Server.Items
{
	public class BlueRugAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new BlueRugDeed(); } }

		[Constructable]
		public BlueRugAddon()
		{
			AddComponent( new AddonComponent( 0x0AC3 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 0x0AF7 ),  0, -1, 0 );
			AddComponent( new AddonComponent( 0x0AC5 ),  1, -1, 0 );
			AddComponent( new AddonComponent( 0x0AF6 ), -1,  0, 0 );
			AddComponent( new AddonComponent( 0x0ABE ),  0,  0, 0 );
			AddComponent( new AddonComponent( 0x0AF8 ),  1,  0, 0 );
			AddComponent( new AddonComponent( 0x0AC4 ), -1,  1, 0 );
			AddComponent( new AddonComponent( 0x0AF9 ),  0,  1, 0 );
			AddComponent( new AddonComponent( 0x0AC2 ),  1,  1, 0 );
		}

		public BlueRugAddon( Serial serial ) : base( serial )
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

	public class BlueRugDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new BlueRugAddon(); } }

		[Constructable]
		public BlueRugDeed()
		{
			Name = "Blue rug";
		}

		public BlueRugDeed( Serial serial ) : base( serial )
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
