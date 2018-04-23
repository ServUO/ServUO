using System;
using Server;

namespace Server.Items
{
	public class BlueCarpetAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new BlueCarpetDeed(); } }
		public override bool RetainDeedHue{ get{ return true; } }

		[Constructable]
		public BlueCarpetAddon( int hue )
		{

			//Body Of Rug
			AddComponent( new AddonComponent( 2749 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 2749 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 2749 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 2749 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 2749 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 2749 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 2749 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 2749 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 2749 ), -1, 1, 0 );

			//Edges Of Rugs
			//North Edge
			AddComponent( new AddonComponent( 2807 ), 0, -2, 0 );
			AddComponent( new AddonComponent( 2807 ), -1, -2, 0 );
			AddComponent( new AddonComponent( 2807 ), 1, -2, 0 );
			//South Edge
			AddComponent( new AddonComponent( 2809 ), 0, 2, 0 );
			AddComponent( new AddonComponent( 2809 ), -1, 2, 0 );
			AddComponent( new AddonComponent( 2809 ), 1, 2, 0 );
			//West Edge
			AddComponent( new AddonComponent( 2806 ), -2, 0, 0 );
			AddComponent( new AddonComponent( 2806 ), -2, 1, 0 );
			AddComponent( new AddonComponent( 2806 ), -2, -1, 0 );
			//East Edge
			AddComponent( new AddonComponent( 2808 ), 2, 0, 0 );
			AddComponent( new AddonComponent( 2808 ), 2, 1, 0 );
			AddComponent( new AddonComponent( 2808 ), 2, -1, 0 );

			//Corners Of Rug
			//NW Corner
			AddComponent( new AddonComponent( 2755 ), -2, -2, 0 );
			//SW Corner
			AddComponent( new AddonComponent( 2756 ), -2, 2, 0 );
			//NE Corner
			AddComponent( new AddonComponent( 2757 ), 2, -2, 0 );
			//SE Corner
			AddComponent( new AddonComponent( 2754 ), 2, 2, 0 );
			Hue = hue;
		}

		public BlueCarpetAddon( Serial serial ) : base( serial )
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

	public class BlueCarpetDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new BlueCarpetAddon( this.Hue ); } }

		[Constructable]
		public BlueCarpetDeed()
		{
			Name = "a blue rug deed";
		}

		public BlueCarpetDeed( Serial serial ) : base( serial )
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