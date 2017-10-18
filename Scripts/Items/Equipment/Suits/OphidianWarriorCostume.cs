using System;
using Server;

namespace Server.Items
{
	public class OphidianWarriorCostume : BaseCostume
	{
		[Constructable]
		public OphidianWarriorCostume() : base( )
		{
            this.CostumeBody = 86;
		}

		public OphidianWarriorCostume( Serial serial ) : base( serial )
		{
		}
		
		public override int LabelNumber
        {
            get
            {
                return 1114229;
            }
        }// ophidian warrior costume

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
