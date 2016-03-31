using System;
using Server;

namespace Server.Items
{
	public class SkitteringHopperCostume : BaseCostume
	{
		[Constructable]
		public SkitteringHopperCostume() : base( )
		{
            Name = "a skittering hopper halloween costume";
            this.CostumeBody = 302;
		}

		public SkitteringHopperCostume( Serial serial ) : base( serial )
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
