using System;
using Server.Items;

namespace Server.Items
{
	[Flipable( 0x1539, 0x153A )]
	public class VegetasPants : BasePants
	{
		[Constructable]
		public VegetasPants() : this( 0 )
		{
		}

		[Constructable]
		public VegetasPants( int hue ) : base( 0x1539, hue )
		{
			Weight = 2.0;
			Hue = 1156;
			Name = "Vegetas Pants";
			LootType = LootType.Blessed;
		}

		public VegetasPants( Serial serial ) : base( serial )
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