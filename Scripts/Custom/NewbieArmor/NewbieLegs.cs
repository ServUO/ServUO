namespace Server.Items
{
	public class NewbieLegs : LeatherLegs
	{
		[Constructable]
		public NewbieLegs()
		{
		}

		[Constructable]
		public NewbieLegs(int hue)
		{
			Hue = hue;
			LootType = LootType.Blessed;
			Attributes.BonusDex = 5;
			Attributes.BonusInt = 5;
			Attributes.BonusStam = 10;
			Attributes.BonusMana = 10;
			Attributes.RegenHits = 2;
			Attributes.RegenStam = 2;
			Attributes.RegenMana = 4;
			Attributes.LowerRegCost = 20;
			WeaponAttributes.SelfRepair = 5;
			ArmorAttributes.LowerStatReq = 100;
			Name = "Newbie Legs";
		}

		public override int BasePhysicalResistance => 15;
		public override int BaseFireResistance => 15;
		public override int BaseColdResistance => 15;
		public override int BasePoisonResistance => 15;
		public override int BaseEnergyResistance => 15;

		public NewbieLegs(Serial serial)
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
