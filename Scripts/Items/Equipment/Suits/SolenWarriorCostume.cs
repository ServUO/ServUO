using System;
using Server;

namespace Server.Items
{
	public class SolenWarriorCostume : BaseCostume
	{
		[Constructable]
		public SolenWarriorCostume() : base( )
		{
            Name = "a solen warrior halloween costume";
            this.CostumeBody = 782;
		}

		public SolenWarriorCostume( Serial serial ) : base( serial )
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
