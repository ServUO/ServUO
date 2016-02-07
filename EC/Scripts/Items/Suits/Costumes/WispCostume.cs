using System;
using Server;

namespace Server.Items
{
	public class WispCostume : BaseCostume
	{
		[Constructable]
		public WispCostume() : base( )
		{
            Name = "a wisp halloween costume";
            this.CostumeBody = 58;
		}

		public WispCostume( Serial serial ) : base( serial )
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
