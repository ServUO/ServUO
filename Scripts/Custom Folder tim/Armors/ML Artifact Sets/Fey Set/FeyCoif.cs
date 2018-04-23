using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class FeyCoif : ChainCoif
	{
		public override int LabelNumber{ get{ return 1075041; } } // Fey Coif

		public override int BasePhysicalResistance{ get{ return 10; } }
		public override int BaseFireResistance{ get{ return 6; } }
		public override int BaseColdResistance{ get{ return 5; } }
		public override int BasePoisonResistance{ get{ return 4; } }
		public override int BaseEnergyResistance{ get{ return 14; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get { return 255; } }


		[Constructable]
		public FeyCoif()
		{
			Name = "Fey Coif";
			Attributes.BonusHits = 4;
			Attributes.DefendChance = 10;

			ArmorAttributes.MageArmor = 1;
		}

		public override Race RequiredRace { get { return Race.Elf; } }

		public FeyCoif( Serial serial ) : base( serial )
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