using System;

namespace Server.Items
{
	[FlipableAttribute( 0x170b, 0x170c )]
	public class GokusBoots : BaseShoes
	{
		[Constructable]
		public GokusBoots() : this( 0 )
		{
		}

		[Constructable]
		public GokusBoots( int hue ) : base( 0x170B, hue )
		{
			Weight = 3.0;
			Name = "gokus boots";
			Hue = 1176;
			LootType = LootType.Blessed;
		}

		public GokusBoots( Serial serial ) : base( serial )
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