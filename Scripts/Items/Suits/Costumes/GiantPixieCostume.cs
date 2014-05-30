using System;
using Server;

namespace Server.Items
{
	public class GiantPixieCostume : BaseCostume
	{
		[Constructable]
		public GiantPixieCostume() : base( )
		{
            Name = "a giant pixie halloween costume";
            this.CostumeBody = 176;
		}

		public GiantPixieCostume( Serial serial ) : base( serial )
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
