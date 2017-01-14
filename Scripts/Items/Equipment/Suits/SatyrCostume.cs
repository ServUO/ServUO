using System;
using Server;

namespace Server.Items
{
	public class SatyrCostume : BaseCostume
	{
		[Constructable]
		public SatyrCostume() : base( )
		{
            Name = "an satyr halloween costume";
            this.CostumeBody = 271;
		}

		public SatyrCostume( Serial serial ) : base( serial )
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
