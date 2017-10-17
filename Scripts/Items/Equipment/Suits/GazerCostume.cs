using System;
using Server;

namespace Server.Items
{
	public class GazerCostume : BaseCostume
	{
		[Constructable]
		public GazerCostume() : base( )
		{
            this.CostumeBody = 22;
		}
		
		public override int LabelNumber
        {
            get
            {
                return 1114004;
            }
        }// gazer halloween costume

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
