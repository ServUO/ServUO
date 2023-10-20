using System;

namespace Server.Items
{
	public class SampAxe : DoubleAxe
	{
		[Constructable]
		public SampAxe(bool isFire)
		{
			WeaponAttributes.HitLeechMana = 100;
			WeaponAttributes.HitLeechStam = 100;
			WeaponAttributes.HitLeechHits = 100;
			WeaponAttributes.HitLowerDefend = 100;
			WeaponAttributes.HitLowerAttack = 100;

			if (isFire)
			{
				AosElementDamages.Fire = 100;
				Hue = 1174;
				WeaponAttributes.HitFireArea = 100;
			}
			else
			{
				AosElementDamages.Cold = 100;
				Hue = 0x4F2;
				WeaponAttributes.HitColdArea = 100;
			}
		}

		public SampAxe(Serial serial)
			: base(serial)
		{
		}
		public override int InitMinHits => 255;
		public override int InitMaxHits => 255;

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}


}