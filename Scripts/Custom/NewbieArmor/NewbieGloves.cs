namespace Server.Items
{
	public class NewbieGloves : LeatherGloves
	{
		[Constructable]
		public NewbieGloves()
		{
		}

		[Constructable]
		public NewbieGloves(int hue)
		{
			Hue = hue;
			LootType = LootType.Blessed;
			Attributes.LowerRegCost = 20;
			WeaponAttributes.SelfRepair = 5;
			ArmorAttributes.LowerStatReq = 100;
			Name = "Newbie Gloves";
		}

		public NewbieGloves(Serial serial)
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
