using System;
using Server;

namespace Server.Items
{
	public class EtherealWarriorCostume : BaseCostume
	{
		[Constructable]
		public EtherealWarriorCostume() : base( )
		{
            Name = "an ethereal warrior halloween costume";
            this.CostumeBody = 123;
		}

		public EtherealWarriorCostume( Serial serial ) : base( serial )
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
