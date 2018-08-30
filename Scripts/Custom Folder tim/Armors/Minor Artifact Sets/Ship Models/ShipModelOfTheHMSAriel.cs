using System;
using Server;

namespace Server.Items
{
	public class ShipModelOfTheHMSAriel : Item
	{
		public override int LabelNumber{ get{ return 1063476; } }
		
		[Constructable]
		public ShipModelOfTheHMSAriel() : base( 0x14F3 )
		{
			Name = "Ship Model Of The H.M.S. Ariel";
			Hue = 1927;
		}

		public ShipModelOfTheHMSAriel( Serial serial ) : base( serial )
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