using System;
using Server;

namespace Server.Items
{
	public class SkeletonCostume : BaseCostume
	{
		[Constructable]
		public SkeletonCostume() : base( )
		{
            Name = "a skeleton halloween costume";
            this.CostumeBody = 50;
		}

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
