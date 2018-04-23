using System;
using Server;

namespace Server.Items
{
	public class FancyRedCarpetAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new FancyRedCarpetDeed(); } }
		public override bool RetainDeedHue{ get{ return true; } }

		[Constructable]
		public FancyRedCarpetAddon( int hue )
		{

			//Body Of Rug
			AddComponent( new AddonComponent( 2758 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 2758 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 2758 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 2758 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 2758 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 2758 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 2758 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 2758 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 2758 ), -1, 1, 0 );

			//Edges Of Rugs
			//North Edge
			AddComponent( new AddonComponent( 2766 ), 0, -2, 0 );
			AddComponent( new AddonComponent( 2766 ), -1, -2, 0 );
			AddComponent( new AddonComponent( 2766 ), 1, -2, 0 );
			//South Edge
			AddComponent( new AddonComponent( 2768 ), 0, 2, 0 );
			AddComponent( new AddonComponent( 2768 ), -1, 2, 0 );
			AddComponent( new AddonComponent( 2768 ), 1, 2, 0 );
			//West Edge
			AddComponent( new AddonComponent( 2765 ), -2, 0, 0 );
			AddComponent( new AddonComponent( 2765 ), -2, 1, 0 );
			AddComponent( new AddonComponent( 2765 ), -2, -1, 0 );
			//East Edge
			AddComponent( new AddonComponent( 2767 ), 2, 0, 0 );
			AddComponent( new AddonComponent( 2767 ), 2, 1, 0 );
			AddComponent( new AddonComponent( 2767 ), 2, -1, 0 );

			//Corners Of Rug
			//NW Corner
			AddComponent( new AddonComponent( 2762 ), -2, -2, 0 );
			//SW Corner
			AddComponent( new AddonComponent( 2763 ), -2, 2, 0 );
			//NE Corner
			AddComponent( new AddonComponent( 2764 ), 2, -2, 0 );
			//SE Corner
			AddComponent( new AddonComponent( 2761 ), 2, 2, 0 );
			Hue = hue;
		}

		public FancyRedCarpetAddon( Serial serial ) : base( serial )
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

	public class FancyRedCarpetDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new FancyRedCarpetAddon( this.Hue ); } }

		[Constructable]
		public FancyRedCarpetDeed()
		{
			Name = "a fancy red rug deed";
		}

		public FancyRedCarpetDeed( Serial serial ) : base( serial )
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