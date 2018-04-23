
using System;
using Server;

namespace Server.Items
{
	public class HiddenValleyGlobe : Item
	{
		


		[Constructable]
		public HiddenValleyGlobe() : base(0xE2E) 
		{
			Weight = 1.0;
			Name = "A Snowy Scene Of Hidden Valley";
			



			LootType = LootType.Blessed; 
		}

		public HiddenValleyGlobe( Serial serial ) : base( serial )
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