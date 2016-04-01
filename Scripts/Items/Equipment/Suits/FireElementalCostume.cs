using System;
using Server;

namespace Server.Items
{
	public class FireElementalCostume : BaseCostume
	{
		[Constructable]
		public FireElementalCostume() : base( )
		{
            Name = "a fire elemental halloween costume";
            this.CostumeBody = 15;
		}

		public FireElementalCostume( Serial serial ) : base( serial )
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
