using System;
namespace Server.Items
{
	public class MarinersBrassSextant : Sextant
	{
		public override int LabelNumber{ get{ return 1075499; } } // Mariner's Brass Sextant

		[Constructable]
		public MarinersBrassSextant() : base()
		{
			LootType = LootType.Blessed;
		}

		public MarinersBrassSextant( Serial serial ) : base( serial )
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
