using System;
using Server;

namespace Server.Items
{
	public class GiantToadCostume : BaseCostume
	{
		[Constructable]
		public GiantToadCostume() : base( )
		{
            Name = "a giant toad halloween costume";
            this.CostumeBody = 80;
		}

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
