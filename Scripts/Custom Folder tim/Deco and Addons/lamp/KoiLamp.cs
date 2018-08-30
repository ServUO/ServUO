using System;
using Server;

namespace Server.Items
{
	public class KoiLamp : BaseLight
	{
		public override int LitItemID{ get { return 0x4C49; } }
		public override int UnlitItemID{ get { return 0x4C48; } }
		
		[Constructable]
		public KoiLamp() : base( 0x4C49 )
		{
			Movable = true;
			Duration = TimeSpan.Zero; // Never burnt out
			Burning = false;
			Light = LightType.Circle300;
			Weight = 40.0;
		}

		public KoiLamp( Serial serial ) : base( serial )
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
