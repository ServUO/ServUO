using System;
using Server;

namespace Server.Items
{
	public class ExodusMinionCostume : BaseCostume
	{
		[Constructable]
		public ExodusMinionCostume() : base( )
		{
            this.CostumeBody = 757;
		}
		
		public override int LabelNumber
        {
            get
            {
                return 1114239;
            }
        }// exodus minion costume

		public ExodusMinionCostume( Serial serial ) : base( serial )
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
