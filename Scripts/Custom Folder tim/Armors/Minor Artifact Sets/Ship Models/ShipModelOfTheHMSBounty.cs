using System;
using Server;

namespace Server.Items
{
	public class ShipModelOfTheHMSBounty : Item
	{
		public override int LabelNumber{ get{ return 1063476; } }
		
		[Constructable]
		public ShipModelOfTheHMSBounty() : base( 0x14F3 )
		{
			Name = "Ship Model Of The H.M.S. Bounty";
			Hue = 2075;
		}

		public ShipModelOfTheHMSBounty( Serial serial ) : base( serial )
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