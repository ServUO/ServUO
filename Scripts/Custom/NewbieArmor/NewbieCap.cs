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
			Attributes.LowerRegCost = 20;
			Attributes.LowerManaCost = 40;
			WeaponAttributes.SelfRepair = 5;
			ArmorAttributes.LowerStatReq = 100;
			Attributes.CastRecovery = 6;
			Attributes.CastSpeed = 2;
			Attributes.RegenMana = 4;
			Name = "Newbie Cap";
		}

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
