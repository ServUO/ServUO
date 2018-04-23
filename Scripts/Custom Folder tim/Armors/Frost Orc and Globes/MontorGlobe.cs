
using System;
using Server;

namespace Server.Items
{
	public class MontorGlobe : Item
	{
		


		[Constructable]
		public MontorGlobe() : base(0xE2E) 
		{
			Weight = 1.0;
			Name = "  A Snowy Scene Of City of Montor";
			



			LootType = LootType.Blessed; 
		}

		public MontorGlobe( Serial serial ) : base( serial )
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