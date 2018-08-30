
using System;
using Server;

namespace Server.Items
{
	public class YewGlobe : Item
	{
		


		[Constructable]
		public YewGlobe() : base(0xE2E) 
		{
			Weight = 1.0;
			Name = "A Snowy Scene Of Yew";
			



			LootType = LootType.Blessed; 
		}

		public YewGlobe( Serial serial ) : base( serial )
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