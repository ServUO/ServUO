using System;
using Server;

namespace Server.Items
{
	public class MinotaurCostume : BaseCostume
	{
		[Constructable]
		public MinotaurCostume() : base( )
		{
            Name = "a minotaur halloween costume";
            this.CostumeBody = 263;
		}

		public MinotaurCostume( Serial serial ) : base( serial )
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
