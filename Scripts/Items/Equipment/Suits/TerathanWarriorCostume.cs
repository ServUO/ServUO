using System;
using Server;

namespace Server.Items
{
	public class TerathanWarriorCostume : BaseCostume
	{
		[Constructable]
		public TerathanWarriorCostume() : base( )
		{
            this.CostumeBody = 70;
		}
		
		public override int LabelNumber
        {
            get
            {
                return 1114228;
            }
        }// terathan warrior costume

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
