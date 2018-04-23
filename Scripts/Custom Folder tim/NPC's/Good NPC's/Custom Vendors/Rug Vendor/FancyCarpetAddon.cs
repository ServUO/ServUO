using System;
using Server;

namespace Server.Items
{
	public class FancyCarpetAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new FancyCarpetDeed(); } }
		public override bool RetainDeedHue{ get{ return true; } }

		[Constructable]
		public FancyCarpetAddon( int hue )
		{

			//Body Of Rug
			AddComponent( new AddonComponent( 2796 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 2796 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 2796 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 2796 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 2796 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 2797 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 2797 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 2797 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 2797 ), -1, 1, 0 );

			//Edges Of Rugs
			//North Edge
			AddComponent( new AddonComponent( 2803 ), 0, -2, 0 );
			AddComponent( new AddonComponent( 2803 ), -1, -2, 0 );
			AddComponent( new AddonComponent( 2803 ), 1, -2, 0 );
			//South Edge
			AddComponent( new AddonComponent( 2805 ), 0, 2, 0 );
			AddComponent( new AddonComponent( 2805 ), -1, 2, 0 );
			AddComponent( new AddonComponent( 2805 ), 1, 2, 0 );
			//West Edge
			AddComponent( new AddonComponent( 2802 ), -2, 0, 0 );
			AddComponent( new AddonComponent( 2802 ), -2, 1, 0 );
			AddComponent( new AddonComponent( 2802 ), -2, -1, 0 );
			//East Edge
			AddComponent( new AddonComponent( 2804 ), 2, 0, 0 );
			AddComponent( new AddonComponent( 2804 ), 2, 1, 0 );
			AddComponent( new AddonComponent( 2804 ), 2, -1, 0 );

			//Corners Of Rug
			//NW Corner
			AddComponent( new AddonComponent( 2799 ), -2, -2, 0 );
			//SW Corner
			AddComponent( new AddonComponent( 2800 ), -2, 2, 0 );
			//NE Corner
			AddComponent( new AddonComponent( 2801 ), 2, -2, 0 );
			//SE Corner
			AddComponent( new AddonComponent( 2798 ), 2, 2, 0 );
			Hue = hue;
		}

		public FancyCarpetAddon( Serial serial ) : base( serial )
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

	public class FancyCarpetDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new FancyCarpetAddon( this.Hue ); } }

		[Constructable]
		public FancyCarpetDeed()
		{
			Name = "a persian rug deed";
		}

		public FancyCarpetDeed( Serial serial ) : base( serial )
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