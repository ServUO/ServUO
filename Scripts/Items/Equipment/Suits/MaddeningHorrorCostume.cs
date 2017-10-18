using System;
using Server;

namespace Server.Items
{
	public class MaddeningHorrorCostume : BaseCostume
	{
		[Constructable]
		public MaddeningHorrorCostume() : base( )
		{
            this.CostumeBody = 721;
		}
		
		public override int LabelNumber
        {
            get
            {
                return 1114233;
            }
        }// maddening horror costume

		public MaddeningHorrorCostume( Serial serial ) : base( serial )
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
