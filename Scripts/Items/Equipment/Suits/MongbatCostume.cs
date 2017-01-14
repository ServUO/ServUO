using System;
using Server;

namespace Server.Items
{
	public class MongbatCostume : BaseCostume
	{
		[Constructable]
		public MongbatCostume() : base( )
		{
            Name = "a Mongbat halloween Costume";
            this.CostumeBody = 39;
		}

		public MongbatCostume( Serial serial ) : base( serial )
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
