using System;
using Server;

namespace Server.Items
{
	public class MaddeningHorrorCostume : BaseCostume
	{
		[Constructable]
		public MaddeningHorrorCostume() : base( )
		{
            Name = "a maddening horror halloween costume";
            this.CostumeBody = 721;
		}

		public MaddeningHorrorCostume( Serial serial ) : base( serial )
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
