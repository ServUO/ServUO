using System;
using Server;

namespace Server.Items
{
	public class PixieCostume : BaseCostume
	{
		[Constructable]
		public PixieCostume() : base( )
		{
            Name = "a pixie halloween costume";
            this.CostumeBody = 128;
		}

		public PixieCostume( Serial serial ) : base( serial )
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
