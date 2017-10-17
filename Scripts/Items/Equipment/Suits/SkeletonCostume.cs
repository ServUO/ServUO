using System;
using Server;

namespace Server.Items
{
	public class SkeletonCostume : BaseCostume
	{
		[Constructable]
		public SkeletonCostume() : base( )
		{
            this.CostumeBody = 50;
		}
		
		public override int LabelNumber
        {
            get
            {
                return 1113996;
            }
        }// skeleton halloween costume

		public SkeletonCostume( Serial serial ) : base( serial )
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
