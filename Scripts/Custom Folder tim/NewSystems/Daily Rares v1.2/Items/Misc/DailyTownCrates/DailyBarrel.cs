using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class DailyBarrel : DailyTownCrate
	{
		//Just a towncrate but done like this for easy removal

		public override int DefaultGumpID{ get{ return 0x3E; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 33, 36, 109, 112 ); }
		}

		[Constructable]
		public DailyBarrel()
		{
			ItemID = 0xE77;
		}

		public DailyBarrel( Serial serial ) : base( serial )
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