using System;
using Server;

namespace Server.Items
{
	public class GoreFiendCostume : BaseCostume
	{
		[Constructable]
		public GoreFiendCostume() : base( )
		{
            Name = "a gore fiend halloween costume";
            this.CostumeBody = 305;
		}

		public GoreFiendCostume( Serial serial ) : base( serial )
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
