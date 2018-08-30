using System;
using Server;

namespace Server.Items
{
	public class HalfApronOfTheCuSidhe : HalfApron
	{
		public override int LabelNumber{ get{ return 1075048; } } // Half Apron of the Cu Sidhe

		[Constructable]
		public HalfApronOfTheCuSidhe() : base( 0x47E )
		{
			Name = "Half Apron of the Cu Sidhe";
		}

		public HalfApronOfTheCuSidhe( Serial serial ) : base( serial )
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