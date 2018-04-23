using System;
using Server;

namespace Server.Items
{
	public class RedPlainRugAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new RedPlainRugDeed(); } }

		[Constructable]
		public RedPlainRugAddon() 
		{
			AddComponent( new AddonComponent( 0x0ACA ), -1, -1, 0 );
			AddComponent( new AddonComponent( 0x0ACE ),  0, -1, 0 );
			AddComponent( new AddonComponent( 0x0ACC ),  1, -1, 0 );
			AddComponent( new AddonComponent( 0x0ACD ), -1,  0, 0 );
			AddComponent( new AddonComponent( 0x0AC7 ),  0,  0, 0 );
			AddComponent( new AddonComponent( 0x0ACF ),  1,  0, 0 );
			AddComponent( new AddonComponent( 0x0ACB ), -1,  1, 0 );
			AddComponent( new AddonComponent( 0x0AD0 ),  0,  1, 0 );
			AddComponent( new AddonComponent( 0x0AC9 ),  1,  1, 0 );
		}

		public RedPlainRugAddon( Serial serial ) : base( serial )
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

	public class RedPlainRugDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new RedPlainRugAddon(); } }
		public override int LabelNumber{ get{ return 1076588; } } // red plain  Rug

		[Constructable]
		public RedPlainRugDeed()
		{
		}

		public RedPlainRugDeed( Serial serial ) : base( serial )
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
