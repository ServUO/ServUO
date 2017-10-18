using System;
using Server;

namespace Server.Items
{
	public class SatyrCostume : BaseCostume
	{
		[Constructable]
		public SatyrCostume() : base( )
		{
            this.CostumeBody = 271;
		}
		
		public override int LabelNumber
        {
            get
            {
                return 1114287;
            }
        }// satyr costume

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
