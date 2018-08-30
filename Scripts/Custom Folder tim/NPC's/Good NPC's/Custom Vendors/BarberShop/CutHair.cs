using System;
using Server;

namespace Server.Items
{
	//[Flipable(0xDFC,0xDFD)]
	public class CutHair : Item
	{
		[Constructable]
		public CutHair() : base( 0xDFE )
		{
            Movable = true;
		}

		public CutHair( Serial serial ) : base( serial )
		{

		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}