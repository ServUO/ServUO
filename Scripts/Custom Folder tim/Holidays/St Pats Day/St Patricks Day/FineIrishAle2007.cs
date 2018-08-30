using System;
using Server;

namespace Server.Items
{
	public class FineIrishAle2007 : BeverageBottle
	{
		[Constructable]
		public FineIrishAle2007() : base( BeverageType.Ale )
		{
			Hue = Utility.RandomList( 1436 );
			Name = "Fine Irish Ale 2007";
			LootType = LootType.Blessed;
		}

		public FineIrishAle2007( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}