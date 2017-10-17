using System;
using Server;

namespace Server.Items
{
	public class LadyOfTheSnowCostume : BaseCostume
	{
		[Constructable]
		public LadyOfTheSnowCostume() : base( )
		{
            this.CostumeBody = 252;
		}
		
		public override int LabelNumber
        {
            get
            {
                return 1114241;
            }
        }// Lady of the Snow costume

		public LadyOfTheSnowCostume( Serial serial ) : base( serial )
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
