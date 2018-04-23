using System;
using Server;

namespace Server.Items
{
	public class BirdLamp : BaseLight
	{
		public override int LitItemID{ get { return 0x4C45; } }
		public override int UnlitItemID{ get { return 0x4C44; } }
		
		[Constructable]
		public BirdLamp() : base( 0x4C45 )
		{
			Movable = true;
			Duration = TimeSpan.Zero; // Never burnt out
			Burning = false;
			Light = LightType.Circle300;
			Weight = 40.0;
		}

		public BirdLamp( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
