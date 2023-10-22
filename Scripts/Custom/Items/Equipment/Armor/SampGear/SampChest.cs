using System;

namespace Server.Items
{
	public class SampChest : LeatherChest
	{
		[Constructable]
		public SampChest()
		{
			Hue = 1174;
			LootType = LootType.Blessed;
			Attributes.BonusStr = 5;
			Attributes.BonusHits = 5;
			Attributes.RegenHits = 1;
			Attributes.BonusInt = 4;
			Attributes.RegenMana = 1;
			Attributes.BonusMana = 6;
			Attributes.LowerManaCost = 10;
			Name = "Sampire Chest";
		}

		public override int BasePhysicalResistance => 17;
		public override int BaseFireResistance => 19;
		public override int BaseColdResistance => 18;
		public override int BasePoisonResistance => 18;
		public override int BaseEnergyResistance => 19;

		public SampChest(Serial serial)
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