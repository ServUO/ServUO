
using System;
using Server;

namespace Server.Items
{
	public class UmbraGlobe : Item
	{
		


		[Constructable]
		public UmbraGlobe() : base(0xE2E) 
		{
			Weight = 1.0;
			Name = "A Snowy Scene Of Umbra";
			



			LootType = LootType.Blessed; 
		}

		public UmbraGlobe( Serial serial ) : base( serial )
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