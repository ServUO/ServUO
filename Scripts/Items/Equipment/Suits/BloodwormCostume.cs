using System;
using Server;

namespace Server.Items
{
	public class BloodwormCostume : BaseCostume
	{
		[Constructable]
		public BloodwormCostume() : base( )
		{
            Name = "a bloodWorm halloween costume";
            this.CostumeBody = 287;
		}

		public BloodwormCostume( Serial serial ) : base( serial )
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
