using System;

namespace Server.Items
{
	public class SampNeck : LeatherGorget
	{
		[Constructable]
		public SampNeck()
		{
			Hue = 1174;
			LootType = LootType.Blessed;
			Attributes.BonusStr = 5;
			Attributes.BonusHits = 5;
			Attributes.BonusStam = 8;
			Attributes.BonusMana = 8;
			Attributes.LowerManaCost = 7;
			Name = "Sampire Gorget";
		}
		public override int BasePhysicalResistance => 4;
		public override int BaseFireResistance => 13;
		public override int BaseColdResistance => 6;
		public override int BasePoisonResistance => 5;
		public override int BaseEnergyResistance => 11;

		public SampNeck(Serial serial)
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