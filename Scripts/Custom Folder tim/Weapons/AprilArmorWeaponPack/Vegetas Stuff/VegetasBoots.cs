using System;

namespace Server.Items
{
	[FlipableAttribute( 0x170b, 0x170c )]
	public class VegetasBoots : BaseShoes
	{
		[Constructable]
		public VegetasBoots() : this( 0 )
		{
		}

		[Constructable]
		public VegetasBoots( int hue ) : base( 0x170B, hue )
		{
			Weight = 3.0;
			Name = "vegetas boots";
			Hue = 1150;
			LootType = LootType.Blessed;
		}

		public VegetasBoots( Serial serial ) : base( serial )
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