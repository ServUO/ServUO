namespace Server.Items
{
	public class NewbieArms : LeatherArms
	{
		[Constructable]
		public NewbieArms()
		{
		}

		[Constructable]
		public NewbieArms(int hue)
		{
			Hue = hue;
			LootType = LootType.Blessed;
			Attributes.BonusDex = 5;
			Attributes.BonusInt = 5;
			Attributes.BonusStam = 10;
			Attributes.BonusMana = 10;
			Attributes.RegenHits = 2;
			Attributes.RegenStam = 2;
			Attributes.LowerManaCost = 10;
			Attributes.LowerRegCost = 20;
			WeaponAttributes.SelfRepair = 5;
			ArmorAttributes.LowerStatReq = 100;
			Name = "Newbie Arms";
		}

		public override int BasePhysicalResistance => 15;
		public override int BaseFireResistance => 15;
		public override int BaseColdResistance => 15;
		public override int BasePoisonResistance => 15;
		public override int BaseEnergyResistance => 15;

		public NewbieArms(Serial serial)
			: base(serial)
		{
		}

		public override int InitMinHits => 255;
		public override int InitMaxHits => 255;


		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}
}
