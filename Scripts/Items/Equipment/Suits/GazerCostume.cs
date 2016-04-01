using System;
using Server;

namespace Server.Items
{
	public class GazerCostume : BaseCostume
	{
		[Constructable]
		public GazerCostume() : base( )
		{
            Name = "a gazer halloween costume";
            this.CostumeBody = 22;
		}

		public GazerCostume( Serial serial ) : base( serial )
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
