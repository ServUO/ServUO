using System;
namespace Server.Items
{
	public class LowYewTable : Item
	{
		public override int LabelNumber{ get{ return 1075502; } } // Low Yew Table

		[Constructable]
		public LowYewTable() : base( 0x281A )
		{
			LootType = LootType.Blessed;
			Hue = 1192;
			Weight = 1;
		}

		public LowYewTable( Serial serial ) : base( serial )
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
