
using System;
using Server;

namespace Server.Items
{
	public class SpiritualityGlobe : Item
	{
		


		[Constructable]
		public SpiritualityGlobe() : base(0xE2E) 
		{
			Weight = 1.0;
			Name = "A Snowy Scene Of The Shrine of Spirituality";
			



			LootType = LootType.Blessed; 
		}

		public SpiritualityGlobe( Serial serial ) : base( serial )
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