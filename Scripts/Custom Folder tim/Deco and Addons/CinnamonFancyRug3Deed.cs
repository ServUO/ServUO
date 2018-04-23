using System;
using Server;

namespace Server.Items
{
	public class CinnamonFancyRug3Addon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new CinnamonFancyRug3Deed(); } }

		[Constructable]
		public CinnamonFancyRug3Addon()
		{
			AddComponent( new AddonComponent( 0x0AE4 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 0x0AE8 ),  0, -1, 0 );
			AddComponent( new AddonComponent( 0x0AE6 ),  1, -1, 0 );
			AddComponent( new AddonComponent( 0x0AE7 ), -1,  0, 0 );
			AddComponent( new AddonComponent( 0x0AED ),  0,  0, 0 );
			AddComponent( new AddonComponent( 0x0AE9 ),  1,  0, 0 );
			AddComponent( new AddonComponent( 0x0AE5 ), -1,  1, 0 );
			AddComponent( new AddonComponent( 0x0AEA ),  0,  1, 0 );
			AddComponent( new AddonComponent( 0x0AE3 ),  1,  1, 0 );
		}

		public CinnamonFancyRug3Addon( Serial serial ) : base( serial )
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

	public class CinnamonFancyRug3Deed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new CinnamonFancyRug3Addon(); } }

		[Constructable]
		public CinnamonFancyRug3Deed()
		{
			Name = "Cinnamon artisan rug deed";
		}

		public CinnamonFancyRug3Deed( Serial serial ) : base( serial )
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
