using System;
using Server;

namespace Server.Items
{
	public class CinnamonFancyRugAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new CinnamonFancyRugDeed(); } }

		[Constructable]
		public CinnamonFancyRugAddon()
		{
			AddComponent( new AddonComponent( 0x0AE4 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 0x0AE8 ),  0, -1, 0 );
			AddComponent( new AddonComponent( 0x0AE6 ),  1, -1, 0 );
			AddComponent( new AddonComponent( 0x0AE7 ), -1,  0, 0 );
			AddComponent( new AddonComponent( 0x0AEB ),  0,  0, 0 );
			AddComponent( new AddonComponent( 0x0AE9 ),  1,  0, 0 );
			AddComponent( new AddonComponent( 0x0AE5 ), -1,  1, 0 );
			AddComponent( new AddonComponent( 0x0AEA ),  0,  1, 0 );
			AddComponent( new AddonComponent( 0x0AE3 ),  1,  1, 0 );
		}

		public CinnamonFancyRugAddon( Serial serial ) : base( serial )
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

	public class CinnamonFancyRugDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new CinnamonFancyRugAddon(); } }
		public override int LabelNumber{ get{ return 1076587; } } // Cinnamon Fancy Rug

		[Constructable]
		public CinnamonFancyRugDeed()
		{
		}

		public CinnamonFancyRugDeed( Serial serial ) : base( serial )
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
