#region Header
// **********
// ServUO - FairyDragon1.cs
// **********
#endregion

#region References
using Server.Items;
#endregion

namespace Server.Mobiles
{
	[CorpseName("a fairy dragon corpse")]
	public class FairyDragon1 : BaseCreature
	{
		[Constructable]
		public FairyDragon1()
			: base(AIType.AI_OmniAI, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Name = "a fairy dragon";
			Body = 718;

			SetStr(506, 561);
			SetDex(97, 103);
			SetInt(401, 580);

			SetHits(393, 409);

			SetDamage(15, 20);

			SetDamageType(ResistanceType.Fire, 25);
			SetDamageType(ResistanceType.Cold, 25);
			SetDamageType(ResistanceType.Poison, 25);
			SetDamageType(ResistanceType.Energy, 25);

			SetResistance(ResistanceType.Physical, 15, 30);
			SetResistance(ResistanceType.Fire, 40, 50);
			SetResistance(ResistanceType.Cold, 40, 50);
			SetResistance(ResistanceType.Poison, 40, 45);
			SetResistance(ResistanceType.Energy, 40, 50);

			SetSkill(SkillName.EvalInt, 10.4, 50.0);
			SetSkill(SkillName.Magery, 20.0, 30.0);
			SetSkill(SkillName.Anatomy, 65.0, 73.6);
			SetSkill(SkillName.Mysticism, 35.0, 55.0);
			SetSkill(SkillName.Meditation, 1.5, 3.5);
			SetSkill(SkillName.MagicResist, 120.2, 125.1);
			SetSkill(SkillName.Tactics, 93.8, 98.5);
			SetSkill(SkillName.Wrestling, 83.1, 89.9);

			PackReg(20);
			PackItem(new Bandage(10));
			PackItem(new DragonBlood(4));

			Tamable = false;
			//ControlSlots = 2;
			//MinTameSkill = 106.0;
		}

		public FairyDragon1(Serial serial)
			: base(serial)
		{ }

		public override void GenerateLoot()
		{
			AddLoot(LootPack.FilthyRich, 4);
			AddLoot(LootPack.MedScrolls);
		}

		public override void OnDeath(Container c)
		{
			base.OnDeath(c);

			if (Utility.RandomDouble() < 0.20)
			{
				switch (Utility.Random(4))
				{
					case 0:
						c.DropItem(new EssenceDiligence());
						break;
					case 1:
						c.DropItem(new FairyDragonWing());
						break;
					case 2:
						c.DropItem(new FaeryDust());
						break;
					case 3:
						c.DropItem(new FeyWings());
						break;
				}

				if (Utility.RandomDouble() < 0.30)
				{
					c.DropItem(new DraconicOrbKeyBlue());
				}
			}
		}

		public override int GetIdleSound()
		{
			return 1561;
		}

		public override int GetAngerSound()
		{
			return 1558;
		}

		public override int GetHurtSound()
		{
			return 1560;
		}

		public override int GetDeathSound()
		{
			return 1559;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			
			reader.ReadInt();
		}
	}
}