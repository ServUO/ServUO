namespace Server.Items
{
	public class NewbieCap : LeatherCap
	{
		[Constructable]
		public NewbieCap()
		{
		}

		[Constructable]
		public NewbieCap(int hue)
		{
			Hue = hue;
			LootType = LootType.Blessed;
			Attributes.LowerRegCost = 17;
			WeaponAttributes.SelfRepair = 5;
			ArmorAttributes.LowerStatReq = 100;
			Name = "Newbie Promised Cap";
		}

		public override int BasePhysicalResistance => 7;
		public override int BaseFireResistance => 9;
		public override int BaseColdResistance => 6;
		public override int BasePoisonResistance => 8;
		public override int BaseEnergyResistance => 7;

		public NewbieCap(Serial serial)
			: base(serial)
		{
		}
		public override int InitMinHits => 255;
		public override int InitMaxHits => 255;

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0); //version
		}
	}
}
