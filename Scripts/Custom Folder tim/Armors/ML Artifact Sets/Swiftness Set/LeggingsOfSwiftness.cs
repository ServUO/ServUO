using System;
using Server;

namespace Server.Items
{
	public class LeggingsOfSwiftness : WoodlandLegs
	{
		public override int LabelNumber{ get{ return 1075037; } } // Leggings of Swiftness
		
		public override int BasePhysicalResistance{ get{ return 8; } }
		public override int BaseFireResistance{ get{ return 7; } }
		public override int BaseColdResistance{ get{ return 8; } }
		public override int BasePoisonResistance{ get{ return 8; } }
		public override int BaseEnergyResistance{ get{ return 10; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public LeggingsOfSwiftness() : base()
		{
			Hue = 0x592;
			Name = "Leggings of Swiftness";
			Attributes.BonusInt = 8;
			Attributes.CastSpeed = 1;
			Attributes.CastRecovery = 1;
			ArmorAttributes.MageArmor = 1;
		}

		public LeggingsOfSwiftness( Serial serial ) : base( serial )
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
