
using System;
using Server;

namespace Server.Items
{
	public class DespiseGlobe : Item
	{
		


		[Constructable]
		public DespiseGlobe() : base(0xE2E) 
		{
			Weight = 1.0;
			Name = "A Snowy Scene Of Despise";
			



			LootType = LootType.Blessed; 
		}

		public DespiseGlobe( Serial serial ) : base( serial )
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