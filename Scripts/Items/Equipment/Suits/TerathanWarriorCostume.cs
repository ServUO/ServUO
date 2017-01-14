using System;
using Server;

namespace Server.Items
{
	public class TerathanWarriorCostume : BaseCostume
	{
		[Constructable]
		public TerathanWarriorCostume() : base( )
		{
            Name = "a terathan warrior halloween costume";
            this.CostumeBody = 70;
		}

		public TerathanWarriorCostume( Serial serial ) : base( serial )
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
