
using System;
using Server;

namespace Server.Items
{
	public class CompassionGlobe : Item
	{
		


		[Constructable]
		public CompassionGlobe() : base(0xE2E) 
		{
			Weight = 1.0;
			Name = " A Snowy Scene Of The Shrine of Compassion";
			



			LootType = LootType.Blessed; 
		}

		public CompassionGlobe( Serial serial ) : base( serial )
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