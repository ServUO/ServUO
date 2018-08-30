
using System;
using Server;

namespace Server.Items
{
	public class OccloGlobe : Item
	{
		


		[Constructable]
		public OccloGlobe() : base(0xE2E) 
		{
			Weight = 1.0;
			Name = "A Snowy Scene Of Occlo";
			



			LootType = LootType.Blessed; 
		}

		public OccloGlobe( Serial serial ) : base( serial )
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