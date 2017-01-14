using System;
using Server;

namespace Server.Items
{
	public class OphidianMatriarchCostume : BaseCostume
	{
		[Constructable]
		public OphidianMatriarchCostume() : base( )
		{
            Name = "an ophidian matriarch halloween costume";
            this.CostumeBody = 87;
		}

		public OphidianMatriarchCostume( Serial serial ) : base( serial )
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
