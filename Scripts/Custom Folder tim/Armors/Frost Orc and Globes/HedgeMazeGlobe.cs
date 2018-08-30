
using System;
using Server;

namespace Server.Items
{
	public class HedgeMazeGlobe : Item
	{
		


		[Constructable]
		public HedgeMazeGlobe() : base(0xE2E) 
		{
			Weight = 1.0;
			Name = "A Snowy Scene Of Hedge Maze";
			



			LootType = LootType.Blessed; 
		}

		public HedgeMazeGlobe( Serial serial ) : base( serial )
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