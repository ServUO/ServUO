using System;
using Server;

namespace Server.Items
{
	public class RedCarpetAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new RedCarpetDeed(); } }
		public override bool RetainDeedHue{ get{ return true; } }

		[Constructable]
		public RedCarpetAddon( int hue )
		{

			//Body Of Rug
			AddComponent( new AddonComponent( 2760 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 2760 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 2760 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 2760 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 2760 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 2760 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 2760 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 2760 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 2760 ), -1, 1, 0 );

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

		public RedCarpetAddon( Serial serial ) : base( serial )
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

	public class RedCarpetDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new RedCarpetAddon( this.Hue ); } }

		[Constructable]
		public RedCarpetDeed()
		{
			Name = "a plain red rug deed";
		}

		public RedCarpetDeed( Serial serial ) : base( serial )
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