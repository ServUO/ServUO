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
			Attributes.LowerRegCost = 16;
			WeaponAttributes.SelfRepair = 5;
			ArmorAttributes.LowerStatReq = 100;
			Name = "Newbie Promised Legs";
		}

		public override int BasePhysicalResistance => 8;
		public override int BaseFireResistance => 8;
		public override int BaseColdResistance => 8;
		public override int BasePoisonResistance => 6;
		public override int BaseEnergyResistance => 8;

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
