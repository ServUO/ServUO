
using System;
using Server;

namespace Server.Items
{
	public class MaginciaGlobe : Item
	{
		


		[Constructable]
		public MaginciaGlobe() : base(0xE2E) 
		{
			Weight = 1.0;
			Name = "A Snowy Scene Of A Snowy Scene Of Magincia";
			



			LootType = LootType.Blessed; 
		}

		public MaginciaGlobe( Serial serial ) : base( serial )
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