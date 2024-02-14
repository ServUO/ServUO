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
			Attributes.LowerRegCost = 17;
			WeaponAttributes.SelfRepair = 5;
			ArmorAttributes.LowerStatReq = 100;
			Name = "Newbie Promised Gloves";
		}

		public override int BasePhysicalResistance => 7;
		public override int BaseFireResistance => 7;
		public override int BaseColdResistance => 9;
		public override int BasePoisonResistance => 7;
		public override int BaseEnergyResistance => 6;
		
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
