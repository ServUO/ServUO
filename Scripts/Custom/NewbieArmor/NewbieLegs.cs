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
			Attributes.LowerRegCost = 20;
			WeaponAttributes.SelfRepair = 5;
			ArmorAttributes.LowerStatReq = 100;
			Name = "Newbie Legs";
		}

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
