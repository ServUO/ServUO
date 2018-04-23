using System;
using Server;

namespace Server.Items
{
	public class HighConstableOfTheBritannianRoyalGuard : BodySash
	{
		public override int LabelNumber{ get{ return 1094910; } } // HighConstable of the Britannian Royal Guard [Replica]

		public override int InitMinHits{ get{ return 150; } }
		public override int InitMaxHits{ get{ return 150; } }

		public override bool CanFortify{ get{ return false; } }

		[Constructable]
		public HighConstableOfTheBritannianRoyalGuard()
		{
			Name = "High Constable Of The Britannian Royal Guard [Replica]";
			Hue = 1258;

			Attributes.BonusInt = 10;
			Attributes.RegenMana = 3;
			Attributes.LowerRegCost = 15;
		}

		public HighConstableOfTheBritannianRoyalGuard( Serial serial ) : base( serial )
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
