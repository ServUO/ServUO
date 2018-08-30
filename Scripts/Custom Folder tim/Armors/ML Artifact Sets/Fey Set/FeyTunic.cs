using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class FeyTunic : ChainChest
	{
		public override int LabelNumber{ get{ return 1075041; } } // Fey Tunic

		public override int BasePhysicalResistance{ get{ return 12; } }
		public override int BaseFireResistance{ get{ return 8; } }
		public override int BaseColdResistance{ get{ return 5; } }
		public override int BasePoisonResistance{ get{ return 4; } }
		public override int BaseEnergyResistance{ get{ return 17; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get { return 255; } }


		[Constructable]
		public FeyTunic()
		{
			Name = "Fey Tunic";
			Attributes.BonusHits = 4;
			Attributes.DefendChance = 15;

			ArmorAttributes.MageArmor = 1;
		}

		public override Race RequiredRace { get { return Race.Elf; } }

		public FeyTunic( Serial serial ) : base( serial )
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