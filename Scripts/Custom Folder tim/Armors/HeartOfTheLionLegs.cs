using System;
using Server;

namespace Server.Items
{
	public class HeartOfTheLionLegs : PlateLegs
	{
		public override int LabelNumber{ get{ return 1070817; } } // Heart of the Lion

		public override int BasePhysicalResistance{ get{ return 12; } }
		public override int BaseFireResistance{ get{ return 6; } }
		public override int BaseColdResistance{ get{ return 6; } }
		public override int BasePoisonResistance{ get{ return 6; } }
		public override int BaseEnergyResistance{ get{ return 6; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public HeartOfTheLionLegs()
		{
			Hue = 0x501;
			Attributes.Luck = 90;
			Attributes.DefendChance = 3;
			ArmorAttributes.LowerStatReq = 40;
			ArmorAttributes.MageArmor = 1;
		}

		public HeartOfTheLionLegs( Serial serial ) : base( serial )
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