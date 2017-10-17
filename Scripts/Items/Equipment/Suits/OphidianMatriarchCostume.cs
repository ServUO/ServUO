using System;
using Server;

namespace Server.Items
{
	public class OphidianMatriarchCostume : BaseCostume
	{
		[Constructable]
		public OphidianMatriarchCostume() : base( )
		{
            this.CostumeBody = 87;
		}
		
		public override int LabelNumber
        {
            get
            {
                return 1114230;
            }
        }// ophidian matriarch costume

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
