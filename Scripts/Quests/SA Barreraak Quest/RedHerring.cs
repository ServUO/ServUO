using System;
using Server;

namespace Server.Items
{
	public class RedHerring : Item
	{		
		public override int LabelNumber{ get{ return 1095046; } } // Britain Crown Fish
		
		[Constructable]
		public RedHerring() : base( 0x9cc )
		{
                    Hue = 337;
		}

		public RedHerring( Serial serial ) : base( serial )
		{		
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}