namespace Server.Items
{
	public class NewbieChest : LeatherChest
	{
		[Constructable]
		public NewbieChest()
		{
		}

		[Constructable]
		public NewbieChest(int hue)
		{
			Hue = hue;
			LootType = LootType.Blessed;
			Attributes.LowerRegCost = 20;
			WeaponAttributes.SelfRepair = 5;
			ArmorAttributes.LowerStatReq = 100;
			Name = "Newbie Chest";
		}

		public NewbieChest(Serial serial)
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
