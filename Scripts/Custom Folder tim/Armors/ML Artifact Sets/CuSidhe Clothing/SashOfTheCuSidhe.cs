using System;
using Server;

namespace Server.Items
{
	public class SashOfTheCuSidhe : BodySash
	{
		public override int LabelNumber{ get{ return 1075048; } } // Sash of the Cu Sidhe

		[Constructable]
		public SashOfTheCuSidhe() : base( 0x47E )
		{
			Name = "Sash of the Cu Sidhe";
		}

		public SashOfTheCuSidhe( Serial serial ) : base( serial )
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