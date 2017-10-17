using System;
using Server;

namespace Server.Items
{
	public class GiantToadCostume : BaseCostume
	{
		[Constructable]
		public GiantToadCostume() : base( )
		{
            this.CostumeBody = 80;
		}
		
		public override int LabelNumber
        {
            get
            {
                return 1114226;
            }
        }// giant toad costume

		public GiantToadCostume( Serial serial ) : base( serial )
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
