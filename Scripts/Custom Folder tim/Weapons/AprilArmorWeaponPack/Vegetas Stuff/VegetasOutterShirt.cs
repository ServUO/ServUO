using System;

namespace Server.Items
{
	[FlipableAttribute( 0x1F7B, 0x1F7C )]
	public class VegetasOutterShirt : BaseMiddleTorso
	{
		[Constructable]
		public VegetasOutterShirt() : this( 0 )
		{
		}

		[Constructable]
		public VegetasOutterShirt( int hue ) : base( 0x1F7B, hue )
		{
			Weight = 2.0;
			Name = "vegetas outter shirt";
			Hue = 1150;
			LootType = LootType.Blessed;
		}

		public VegetasOutterShirt( Serial serial ) : base( serial )
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