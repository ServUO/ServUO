using System;
using Server;

namespace Server.Items
{
	public class WorldTableWestAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new WorldTableWestDeed(); } }

		[Constructable]
		public WorldTableWestAddon()
		{
			AddComponent( new AddonComponent( 0x4BFF ), -1, 0, 0 );
			AddComponent( new AddonComponent( 0x4BFD ), -1, 1, 0 );
			AddComponent( new AddonComponent( 0x4C00 ),  0, 0, 0 );
			AddComponent( new AddonComponent( 0x4BFC ),  0, 1, 0 );
			AddComponent( new AddonComponent( 0x4C01 ),  1, 0, 0 );
			AddComponent( new AddonComponent( 0x4BFE ),  1, 1, 0 );
		}

		public WorldTableWestAddon( Serial serial ) : base( serial )
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

	public class WorldTableWestDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new WorldTableWestAddon(); } }


		[Constructable]
		public WorldTableWestDeed()
		{
		}

		public WorldTableWestDeed( Serial serial ) : base( serial )
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