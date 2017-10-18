using System;
using Server;

namespace Server.Items
{
	public class DreamWraithCostume : BaseCostume
	{
		[Constructable]
		public DreamWraithCostume() : base( )
		{
            this.CostumeBody = 740;
		}
		
		public override int LabelNumber
        {
            get
            {
                return 1114008;
            }
        }// dream wraith halloween costume

		public DreamWraithCostume( Serial serial ) : base( serial )
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
