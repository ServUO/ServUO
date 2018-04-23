using System;
using Server;

namespace Server.Items
{
	public class CinnamonRugAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new CinnamonRugDeed(); } }

		[Constructable]
		public CinnamonRugAddon()
		{
			AddComponent( new AddonComponent( 0x0AAE ), -1, -1, 0 );
			AddComponent( new AddonComponent( 0x0AAA ),  0, -1, 0 );
			AddComponent( new AddonComponent( 0x0AAF ),  1, -1, 0 );
			AddComponent( new AddonComponent( 0x0AAD ), -1,  0, 0 );
			AddComponent( new AddonComponent( 0x0AA9 ),  0,  0, 0 );
			AddComponent( new AddonComponent( 0x0AAB ),  1,  0, 0 );
			AddComponent( new AddonComponent( 0x0AB1 ), -1,  1, 0 );
			AddComponent( new AddonComponent( 0x0AAC ),  0,  1, 0 );
			AddComponent( new AddonComponent( 0x0AB0 ),  1,  1, 0 );
		}

		public CinnamonRugAddon( Serial serial ) : base( serial )
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

	public class CinnamonRugDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new CinnamonRugAddon(); } }

		[Constructable]
		public CinnamonRugDeed()
		{
			Name = "Large cinnamon rug";
		}

		public CinnamonRugDeed( Serial serial ) : base( serial )
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
