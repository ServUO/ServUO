using System;
using Server;

namespace Server.Items
{
	public class OphidianWarriorCostume : BaseCostume
	{
		[Constructable]
		public OphidianWarriorCostume() : base( )
		{
            Name = "a ophidian warrior halloween costume";
            this.CostumeBody = 86;
		}

		public OphidianWarriorCostume( Serial serial ) : base( serial )
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
