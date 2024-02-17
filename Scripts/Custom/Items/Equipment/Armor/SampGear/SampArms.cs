using System;

namespace Server.Items
{
	public class SampArms : LeatherArms
	{
		[Constructable]
		public SampArms()
		{
			Hue = 1174;
			LootType = LootType.Blessed;
			Attributes.BonusDex = 5;
			Attributes.BonusInt = 2;
			Attributes.BonusStam = 10;
			Attributes.BonusMana = 10;
			Attributes.RegenStam = 1;
			Attributes.LowerManaCost = 10;
			Name = "Sampire Arms";
		}
		public override int BasePhysicalResistance => 17;
		public override int BaseFireResistance => 19;
		public override int BaseColdResistance => 18;
		public override int BasePoisonResistance => 18;
		public override int BaseEnergyResistance => 19;

		public SampArms(Serial serial)
			: base(serial)
		{
		}
		public override int InitMinHits => 255;
		public override int InitMaxHits => 255;

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}
}