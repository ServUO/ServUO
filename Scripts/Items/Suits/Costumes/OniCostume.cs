using System;
using Server;

namespace Server.Items
{
	public class OniCostume : BaseCostume
	{
		[Constructable]
		public OniCostume() : base( )
		{
            Name = "an oni halloween costume";
            this.CostumeBody = 241;
		}

		public OniCostume( Serial serial ) : base( serial )
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
