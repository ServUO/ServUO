
using System;
using Server;

namespace Server.Items
{
	public class GreatWaterfallGlobe : Item
	{
		


		[Constructable]
		public GreatWaterfallGlobe() : base(0xE2E) 
		{
			Weight = 1.0;
			Name = "A Snowy Scene Of Great Waterfall";
			



			LootType = LootType.Blessed; 
		}

		public GreatWaterfallGlobe( Serial serial ) : base( serial )
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