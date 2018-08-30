using System;
using Server;

namespace Server.Items
{
	public class CloakOfTheCuSidhe : Cloak
	{
		public override int LabelNumber{ get{ return 1075048; } } // Cloak of the Cu Sidhe

		[Constructable]
		public CloakOfTheCuSidhe() : base( 0x47E )
		{
			Name = "Cloak of the Cu Sidhe";
		}

		public CloakOfTheCuSidhe( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}