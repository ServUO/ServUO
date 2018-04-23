using System;
using Server.Items;

namespace Server.Items
{
	public class PotionStone : Item
	{
		public override string DefaultName
		{
			get { return "a Potion Stone"; }
		}

		[Constructable]
		public PotionStone() : base( 0xED4 )
		{
			Movable = false;
			Hue = 0x497;
		}

		public override void OnDoubleClick( Mobile from )
		{
			PotionBag PotionBag = new PotionBag();

			if ( !from.AddToBackpack( PotionBag ) )
				PotionBag.Delete();
		}

		public PotionStone( Serial serial ) : base( serial )
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