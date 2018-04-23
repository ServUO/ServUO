using System;
using Server;

namespace Server.Items
{
	public class BlueFancyRugAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new BlueFancyRugDeed(); } }

		[Constructable]
		public BlueFancyRugAddon()
		{
			AddComponent( new AddonComponent( 0x0AD3 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 0x0AD7 ),  0, -1, 0 );
			AddComponent( new AddonComponent( 0x0AD5 ),  1, -1, 0 );
			AddComponent( new AddonComponent( 0x0AD6 ), -1,  0, 0 );
			AddComponent( new AddonComponent( 0x0AD1 ),  0,  0, 0 );
			AddComponent( new AddonComponent( 0x0AD8 ),  1,  0, 0 );
			AddComponent( new AddonComponent( 0x0AD4 ), -1,  1, 0 );
			AddComponent( new AddonComponent( 0x0AD9 ),  0,  1, 0 );
			AddComponent( new AddonComponent( 0x0AD2 ),  1,  1, 0 );
		}

		public BlueFancyRugAddon( Serial serial ) : base( serial )
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

	public class BlueFancyRugDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new BlueFancyRugAddon(); } }
		public override int LabelNumber{ get{ return 1076273; } } // Blue Fancy Rug

		[Constructable]
		public BlueFancyRugDeed()
		{
		}

		public BlueFancyRugDeed( Serial serial ) : base( serial )
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
