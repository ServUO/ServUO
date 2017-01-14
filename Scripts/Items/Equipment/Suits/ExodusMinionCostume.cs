using System;
using Server;

namespace Server.Items
{
	public class ExodusMinionCostume : BaseCostume
	{
		[Constructable]
		public ExodusMinionCostume() : base( )
		{
            Name = "a exodus minion halloween costume";
            this.CostumeBody = 757;
		}

		public ExodusMinionCostume( Serial serial ) : base( serial )
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
