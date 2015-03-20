using System;
using Server;

namespace Server.Items
{
	public class TitanCostume : BaseCostume
	{
		[Constructable]
		public TitanCostume() : base( )
		{
            Name = "a titan halloween costume";
            this.CostumeBody = 76;
		}

		public TitanCostume( Serial serial ) : base( serial )
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
