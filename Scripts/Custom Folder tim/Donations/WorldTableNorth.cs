using System;
using Server;

namespace Server.Items
{
	public class WorldTableNorthAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new WorldTableNorthDeed(); } }

		[Constructable]
		public WorldTableNorthAddon()
		{
			AddComponent( new AddonComponent( 0x4BF9 ),  0, -1, 0 );
			AddComponent( new AddonComponent( 0x4BF8 ),  1, -1, 0 );
			AddComponent( new AddonComponent( 0x4BFA ),  0, 0, 0 );
			AddComponent( new AddonComponent( 0x4BF7 ),  1, 0, 0 );
			AddComponent( new AddonComponent( 0x4BFB ),  0, 1, 0 );
			AddComponent( new AddonComponent( 0x4BF6 ),  1, 1, 0 );
		}

		public WorldTableNorthAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class WorldTableNorthDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new WorldTableNorthAddon(); } }


		[Constructable]
		public WorldTableNorthDeed()
		{
		}

		public WorldTableNorthDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}