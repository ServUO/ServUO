using System;
using Server;

namespace Server.Items
{
	public class SamaritanVest : JinBaori
	{
		public override int LabelNumber{ get{ return 1094926; } } // Good Samaritan of Britannia [Replica]

		public override int BasePhysicalResistance{ get{ return 5; } }

		public override int InitMinHits{ get{ return 150; } }
		public override int InitMaxHits{ get{ return 150; } }

		public override bool CanFortify{ get{ return false; } }

		[Constructable]
		public SamaritanVest()
		{
			Name = "Good Samaritan of Britannia [Replica]";
			Hue = 0x2a3;
		}

		public SamaritanVest( Serial serial ) : base( serial )
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
