using System;
using Server;

namespace Server.Items
{
	public class TitanCostume : BaseCostume
	{
		[Constructable]
		public TitanCostume() : base( )
		{
            this.CostumeBody = 76;
		}
		
		public override int LabelNumber
        {
            get
            {
                return 1114238;
            }
        }// titan costume

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
