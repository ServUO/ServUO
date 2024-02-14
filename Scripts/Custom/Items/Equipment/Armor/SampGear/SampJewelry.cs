using System;

namespace Server.Items
{
	public class SampRing : GoldRing
	{
		[Constructable]
		public SampRing()
		{
			Hue = 1174;
			LootType = LootType.Blessed;
			Attributes.DefendChance = 20;
			Attributes.AttackChance = 20;
			Attributes.WeaponSpeed = 10;
			Attributes.WeaponDamage = 35;
			Attributes.BonusDex = 10;
			Name = "Sampire Ring";
		}

		public SampRing(Serial serial)
			: base(serial)
		{
		}
		public override int InitMinHits => 255;
		public override int InitMaxHits => 255;

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}

	public class SampBracelet : GoldBracelet
	{
		[Constructable]
		public SampBracelet()
		{
			Hue = 1174;
			LootType = LootType.Blessed;
			Attributes.DefendChance = 20;
			Attributes.AttackChance = 20;
			Attributes.WeaponSpeed = 10;
			Attributes.WeaponDamage = 35;
			Attributes.BonusDex = 10;
			Name = "Sampire Bracelet";
		}

		public SampBracelet(Serial serial)
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