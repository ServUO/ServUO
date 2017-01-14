using System;
using Server;

namespace Server.Items
{
	public class LadyOfTheSnowCostume : BaseCostume
	{
		[Constructable]
		public LadyOfTheSnowCostume() : base( )
		{
            Name = "a lady of the snow halloween costume";
            this.CostumeBody = 252;
		}

		public LadyOfTheSnowCostume( Serial serial ) : base( serial )
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
