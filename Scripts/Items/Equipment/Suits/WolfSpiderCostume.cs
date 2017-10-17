using System;
using Server;

namespace Server.Items
{
	public class WolfSpiderCostume : BaseCostume
	{
		[Constructable]
		public WolfSpiderCostume() : base( )
		{
            this.CostumeBody = 376;
		}
		
		public override int LabelNumber
        {
            get
            {
                return 1114232;
            }
        }// wolf spider costume

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
