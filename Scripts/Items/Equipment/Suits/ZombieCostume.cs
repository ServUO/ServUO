using System;
using Server;

namespace Server.Items
{
	public class ZombieCostume : BaseCostume
	{
		[Constructable]
		public ZombieCostume() : base( )
		{
            Name = "a zombie costume";
            this.CostumeBody = 3;
		}

		public ZombieCostume( Serial serial ) : base( serial )
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
