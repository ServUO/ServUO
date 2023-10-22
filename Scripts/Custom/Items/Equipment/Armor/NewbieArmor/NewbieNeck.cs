using System;

namespace Server.Items
{
	public class NewbieNeck : LeatherGorget
	{
		[Constructable]
		public NewbieNeck(int hue)
		{
			Hue = hue;
			LootType = LootType.Blessed;
			Attributes.LowerRegCost = 16;
			WeaponAttributes.SelfRepair = 5;
			ArmorAttributes.LowerStatReq = 100;
			Name = "Newbieire Promised Gorget";
		}
		public override int BasePhysicalResistance => 7;
		public override int BaseFireResistance => 9;
		public override int BaseColdResistance => 7;
		public override int BasePoisonResistance => 8;
		public override int BaseEnergyResistance => 6;

		public NewbieNeck(Serial serial)
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