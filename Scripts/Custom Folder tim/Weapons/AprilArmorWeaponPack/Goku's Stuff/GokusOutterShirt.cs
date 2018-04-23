using System;

namespace Server.Items
{
	[FlipableAttribute( 0x1F7B, 0x1F7C )]
	public class GokusOutterShirt : BaseMiddleTorso
	{
		[Constructable]
		public GokusOutterShirt() : this( 0 )
		{
		}

		[Constructable]
		public GokusOutterShirt( int hue ) : base( 0x1F7B, hue )
		{
			Weight = 2.0;
			Name = "gokus outter shirt";
			Hue = 1255;
			LootType = LootType.Blessed;
		}

		public GokusOutterShirt( Serial serial ) : base( serial )
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