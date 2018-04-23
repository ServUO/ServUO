using System;
using Server;

namespace Server.Items
{
	public class ShipModelOfTheHMSAdventure : Item
	{
		public override int LabelNumber{ get{ return 1063476; } }
		
		[Constructable]
		public ShipModelOfTheHMSAdventure() : base( 0x14F3 )
		{
			Name = "Ship Model Of The H.M.S. Adventure";
			Hue = 1937;
		}

		public ShipModelOfTheHMSAdventure( Serial serial ) : base( serial )
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