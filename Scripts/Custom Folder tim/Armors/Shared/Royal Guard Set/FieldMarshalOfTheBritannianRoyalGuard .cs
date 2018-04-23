using System;
using Server;

namespace Server.Items
{
	public class FieldMarshalOfTheBritannianRoyalGuard : BodySash
	{
		public override int LabelNumber{ get{ return 1094910; } } // FieldMarshal of the Britannian Royal Guard [Replica]

		public override int InitMinHits{ get{ return 150; } }
		public override int InitMaxHits{ get{ return 150; } }

		public override bool CanFortify{ get{ return false; } }

		[Constructable]
		public FieldMarshalOfTheBritannianRoyalGuard()
		{
			Name = "Field Marshal Of The Britannian Royal Guard [Replica]";
			Hue = 1256;

			Attributes.BonusInt = 10;
			Attributes.RegenMana = 3;
			Attributes.LowerRegCost = 10;
		}

		public FieldMarshalOfTheBritannianRoyalGuard( Serial serial ) : base( serial )
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
