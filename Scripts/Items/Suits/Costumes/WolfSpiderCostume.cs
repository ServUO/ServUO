using System;
using Server;

namespace Server.Items
{
	public class WolfSpiderCostume : BaseCostume
	{
		[Constructable]
		public WolfSpiderCostume() : base( )
		{
            Name = "a wolf spider halloween costume";
            this.CostumeBody = 376;
		}

		public WolfSpiderCostume( Serial serial ) : base( serial )
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
