using System;
using Server;

namespace Server.Items
{
	public class GoreFiendCostume : BaseCostume
	{
		[Constructable]
		public GoreFiendCostume() : base( )
		{
            this.CostumeBody = 305;
		}
		
		public override int LabelNumber
        {
            get
            {
                return 1114227;
            }
        }// gore fiend costume

		public GoreFiendCostume( Serial serial ) : base( serial )
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
