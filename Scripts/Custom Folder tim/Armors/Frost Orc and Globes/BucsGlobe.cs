
using System;
using Server;

namespace Server.Items
{
	public class BucsGlobe : Item
	{
		


		[Constructable]
		public BucsGlobe() : base(0xE2E) 
		{
			Weight = 1.0;
			Name = "A Snowy Scene Of Buccaneers Den";
			



			LootType = LootType.Blessed; 
		}

		public BucsGlobe( Serial serial ) : base( serial )
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