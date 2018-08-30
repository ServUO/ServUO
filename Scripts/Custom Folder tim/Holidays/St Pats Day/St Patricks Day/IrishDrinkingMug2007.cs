using System;
using Server;

namespace Server.Items
{
	public class IrishDrinkingMug2007 : GlassMug
	{
		[Constructable]
		public IrishDrinkingMug2007() : base( BeverageType.Ale )
		{
			Hue = 64;
			Name = "Irish Drinking Mug 2007";
			LootType = LootType.Blessed;
		}

		public IrishDrinkingMug2007( Serial serial ) : base( serial )
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