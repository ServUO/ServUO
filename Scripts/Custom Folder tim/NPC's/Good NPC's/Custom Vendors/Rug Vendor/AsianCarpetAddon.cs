using System;
using Server;

namespace Server.Items
{
	public class AsianCarpetAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new AsianCarpetDeed(); } }
		public override bool RetainDeedHue{ get{ return true; } }

		[Constructable]
		public AsianCarpetAddon( int hue )
		{

			//Body Of Rug
			AddComponent( new AddonComponent( 2769 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 2769 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 2769 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 2769 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 2769 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 2769 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 2769 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 2769 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 2769 ), -1, 1, 0 );

			//Edges Of Rugs
			//North Edge
			AddComponent( new AddonComponent( 2775 ), 0, -2, 0 );
			AddComponent( new AddonComponent( 2775 ), -1, -2, 0 );
			AddComponent( new AddonComponent( 2775 ), 1, -2, 0 );
			//South Edge
			AddComponent( new AddonComponent( 2777 ), 0, 2, 0 );
			AddComponent( new AddonComponent( 2777 ), -1, 2, 0 );
			AddComponent( new AddonComponent( 2777 ), 1, 2, 0 );
			//West Edge
			AddComponent( new AddonComponent( 2774 ), -2, 0, 0 );
			AddComponent( new AddonComponent( 2774 ), -2, 1, 0 );
			AddComponent( new AddonComponent( 2774 ), -2, -1, 0 );
			//East Edge
			AddComponent( new AddonComponent( 2776 ), 2, 0, 0 );
			AddComponent( new AddonComponent( 2776 ), 2, 1, 0 );
			AddComponent( new AddonComponent( 2776 ), 2, -1, 0 );

			//Corners Of Rug
			//NW Corner
			AddComponent( new AddonComponent( 2771 ), -2, -2, 0 );
			//SW Corner
			AddComponent( new AddonComponent( 2772 ), -2, 2, 0 );
			//NE Corner
			AddComponent( new AddonComponent( 2773 ), 2, -2, 0 );
			//SE Corner
			AddComponent( new AddonComponent( 2770 ), 2, 2, 0 );
			Hue = hue;
		}

		public AsianCarpetAddon( Serial serial ) : base( serial )
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

	public class AsianCarpetDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new AsianCarpetAddon( this.Hue ); } }

		[Constructable]
		public AsianCarpetDeed()
		{
			Name = "an asian rug deed";
		}

		public AsianCarpetDeed( Serial serial ) : base( serial )
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