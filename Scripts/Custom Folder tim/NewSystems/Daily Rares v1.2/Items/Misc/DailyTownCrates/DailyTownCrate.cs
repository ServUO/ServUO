using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class DailyTownCrate : VoransTownCrate
	{
		//Just a towncrate but done like this for easy removal

		[Constructable]
		public DailyTownCrate()
		{
		}

		public DailyTownCrate( Serial serial ) : base( serial )
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