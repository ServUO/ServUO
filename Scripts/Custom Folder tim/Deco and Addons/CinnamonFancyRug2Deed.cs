using System;
using Server;

namespace Server.Items
{
	public class CinnamonFancyRug2Addon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new CinnamonFancyRug2Deed(); } }

		[Constructable]
		public CinnamonFancyRug2Addon()
		{
			AddComponent( new AddonComponent( 0x0AE4 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 0x0AE8 ),  0, -1, 0 );
			AddComponent( new AddonComponent( 0x0AE6 ),  1, -1, 0 );
			AddComponent( new AddonComponent( 0x0AE7 ), -1,  0, 0 );
			AddComponent( new AddonComponent( 0x0AEC ),  0,  0, 0 );
			AddComponent( new AddonComponent( 0x0AE9 ),  1,  0, 0 );
			AddComponent( new AddonComponent( 0x0AE5 ), -1,  1, 0 );
			AddComponent( new AddonComponent( 0x0AEA ),  0,  1, 0 );
			AddComponent( new AddonComponent( 0x0AE3 ),  1,  1, 0 );
		}

		public CinnamonFancyRug2Addon( Serial serial ) : base( serial )
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

	public class CinnamonFancyRug2Deed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new CinnamonFancyRug2Addon(); } }

		[Constructable]
		public CinnamonFancyRug2Deed()
		{
			Name = "Cinnamon decorative rug deed";
		}

		public CinnamonFancyRug2Deed( Serial serial ) : base( serial )
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
