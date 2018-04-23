using System;

namespace Server.Items
{
	[FlipableAttribute( 0x1efd, 0x1efe )]
	public class GokusUnderShirt : BaseShirt
	{
		[Constructable]
		public GokusUnderShirt() : this( 0 )
		{
		}

		[Constructable]
		public GokusUnderShirt( int hue ) : base( 0x1EFD, hue )
		{
			Weight = 2.0;
			Name = "gokus under shirt";
			Hue = 1176;
			LootType = LootType.Blessed;
		}

		public GokusUnderShirt( Serial serial ) : base( serial )
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