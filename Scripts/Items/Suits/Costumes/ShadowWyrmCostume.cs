using System;
using Server;

namespace Server.Items
{
	public class ShadowWyrmCostume : BaseCostume
	{
		[Constructable]
		public ShadowWyrmCostume() : base( )
		{
            Name = "a shadow wyrm halloween Costume";
            this.CostumeBody = 106;
		}

		public ShadowWyrmCostume( Serial serial ) : base( serial )
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
