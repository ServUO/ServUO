using System;

namespace Server.Items
{
	[FlipableAttribute( 0x1efd, 0x1efe )]
	public class VegetasUnderShirt : BaseShirt
	{
		[Constructable]
		public VegetasUnderShirt() : this( 0 )
		{
		}

		[Constructable]
		public VegetasUnderShirt( int hue ) : base( 0x1EFD, hue )
		{
			Weight = 2.0;
			Name = "vegetas under shirt";
			Hue = 1156;
			LootType = LootType.Blessed;
		}

		public VegetasUnderShirt( Serial serial ) : base( serial )
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