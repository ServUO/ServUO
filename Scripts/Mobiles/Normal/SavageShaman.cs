#region Header
// **********
// ServUO - SavageShaman.cs
// **********
#endregion

#region References
using System;
using System.Collections;

using Server.Items;
using Server.Spells;
#endregion

namespace Server.Mobiles
{
	[CorpseName("a savage corpse")]
	public class SavageShaman : BaseCreature
	{
		[Constructable]
		public SavageShaman()
			: base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			Name = NameList.RandomName("savage shaman");

			Female = true;
			Body = 186;

			SetStr(126, 145);
			SetDex(91, 110);
			SetInt(161, 185);

			SetDamage(4, 10);

			SetDamageType(ResistanceType.Physical, 100);

			SetResistance(ResistanceType.Physical, 30, 40);
			SetResistance(ResistanceType.Fire, 20, 30);
			SetResistance(ResistanceType.Cold, 20, 30);
			SetResistance(ResistanceType.Poison, 20, 30);
			SetResistance(ResistanceType.Energy, 40, 50);

			SetSkill(SkillName.EvalInt, 77.5, 100.0);
			SetSkill(SkillName.Fencing, 62.5, 85.0);
			SetSkill(SkillName.Macing, 62.5, 85.0);
			SetSkill(SkillName.Magery, 72.5, 95.0);
			SetSkill(SkillName.Meditation, 77.5, 100.0);
			SetSkill(SkillName.MagicResist, 77.5, 100.0);
			SetSkill(SkillName.Swords, 62.5, 85.0);
			SetSkill(SkillName.Tactics, 62.5, 85.0);
			SetSkill(SkillName.Wrestling, 62.5, 85.0);

			Fame = 1000;
			Karma = -1000;

			PackReg(10, 15);
			PackItem(new Bandage(Utility.RandomMinMax(1, 15)));

			if (0.1 > Utility.RandomDouble())
			{
				PackItem(new TribalBerry());
			}

			AddItem(new BoneArms());
			AddItem(new BoneLegs());
			AddItem(new DeerMask());
		}

		public SavageShaman(Serial serial)
			: base(serial)
		{ }

		public override int Meat { get { return 1; } }
		public override bool AlwaysMurderer { get { return true; } }
		public override bool ShowFameTitle { get { return false; } }
        public override TribeType Tribe { get { return TribeType.Savage; } }

        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.SavagesAndOrcs;
            }
        }

		public override void GenerateLoot()
		{
			AddLoot(LootPack.Average);
		}

		public override bool IsEnemy(Mobile m)
		{
			if (m.BodyMod == 183 || m.BodyMod == 184)
			{
				return false;
			}

			return base.IsEnemy(m);
		}

		public override void AggressiveAction(Mobile aggressor, bool criminal)
		{
			base.AggressiveAction(aggressor, criminal);

			if (aggressor.BodyMod == 183 || aggressor.BodyMod == 184)
			{
				AOS.Damage(aggressor, 50, 0, 100, 0, 0, 0);
				aggressor.BodyMod = 0;
				aggressor.HueMod = -1;
				aggressor.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
				aggressor.PlaySound(0x307);
				aggressor.SendLocalizedMessage(1040008); // Your skin is scorched as the tribal paint burns away!

				if (aggressor is PlayerMobile)
				{
					((PlayerMobile)aggressor).SavagePaintExpiration = TimeSpan.Zero;
				}
			}
		}

		public override void AlterMeleeDamageTo(Mobile to, ref int damage)
		{
			if (to is Dragon || to is WhiteWyrm || to is SwampDragon || to is Drake || to is Nightmare || to is Hiryu ||
				to is LesserHiryu || to is Daemon)
			{
				damage *= 3;
			}
		}

		public override void OnGotMeleeAttack(Mobile attacker)
		{
			base.OnGotMeleeAttack(attacker);

			if (0.1 > Utility.RandomDouble())
			{
				BeginSavageDance();
			}
		}

		public void BeginSavageDance()
		{
			if (Map == null)
			{
				return;
			}

			ArrayList list = new ArrayList();
            IPooledEnumerable eable = GetMobilesInRange(8);

			foreach (Mobile m in eable)
			{
				if (m != this && m is SavageShaman)
				{
					list.Add(m);
				}
			}
            eable.Free();

			Animate(111, 5, 1, true, false, 0); // Do a little dance...

			if (AIObject != null)
			{
				AIObject.NextMove = Core.TickCount + 1000;
			}

			if (list.Count >= 3)
			{
				for (int i = 0; i < list.Count; ++i)
				{
					SavageShaman dancer = (SavageShaman)list[i];

					dancer.Animate(111, 5, 1, true, false, 0); // Get down tonight...

					if (dancer.AIObject != null)
					{
						dancer.AIObject.NextMove = Core.TickCount + 1000;
					}
				}

				Timer.DelayCall(TimeSpan.FromSeconds(1.0), EndSavageDance);
			}
		}

		public void EndSavageDance()
		{
			if (Deleted)
			{
				return;
			}

			ArrayList list = new ArrayList();
            IPooledEnumerable eable = GetMobilesInRange(8);

			foreach (Mobile m in eable)
			{
				list.Add(m);
			}
            eable.Free();

			if (list.Count > 0)
			{
				switch (Utility.Random(3))
				{
					case 0: /* greater heal */
						{
							foreach (Mobile m in list)
							{
								bool isFriendly = (m is Savage || m is SavageRider || m is SavageShaman || m is SavageRidgeback);

								if (!isFriendly)
								{
									continue;
								}

								if (m.Poisoned || MortalStrike.IsWounded(m) || !CanBeBeneficial(m))
								{
									continue;
								}

								DoBeneficial(m);

								// Algorithm: (40% of magery) + (1-10)

								int toHeal = (int)(Skills[SkillName.Magery].Value * 0.4);
								toHeal += Utility.Random(1, 10);

								m.Heal(toHeal, this);

								m.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
								m.PlaySound(0x202);
							}

							break;
						}
					case 1: /* lightning */
						{
							foreach (Mobile m in list)
							{
								bool isFriendly = (m is Savage || m is SavageRider || m is SavageShaman || m is SavageRidgeback);

								if (isFriendly)
								{
									continue;
								}

								if (!CanBeHarmful(m))
								{
									continue;
								}

								DoHarmful(m);

								double damage;

								if (Core.AOS)
								{
									int baseDamage = 6 + (int)(Skills[SkillName.EvalInt].Value / 5.0);

									damage = Utility.RandomMinMax(baseDamage, baseDamage + 3);
								}
								else
								{
									damage = Utility.Random(12, 9);
								}

								m.BoltEffect(0);

								SpellHelper.Damage(TimeSpan.FromSeconds(0.25), m, this, damage, 0, 0, 0, 0, 100);
							}

							break;
						}
					case 2: /* poison */
						{
							foreach (Mobile m in list)
							{
								bool isFriendly = (m is Savage || m is SavageRider || m is SavageShaman || m is SavageRidgeback);

								if (isFriendly)
								{
									continue;
								}

								if (!CanBeHarmful(m))
								{
									continue;
								}

								DoHarmful(m);

								if (m.Spell != null)
								{
									m.Spell.OnCasterHurt();
								}

								m.Paralyzed = false;

								double total = Skills[SkillName.Magery].Value + Skills[SkillName.Poisoning].Value;

								double dist = GetDistanceToSqrt(m);

								if (dist >= 3.0)
								{
									total -= (dist - 3.0) * 10.0;
								}

								int level;

								if (total >= 200.0 && Utility.Random(1, 100) <= 10)
								{
									level = 3;
								}
								else if (total > 170.0)
								{
									level = 2;
								}
								else if (total > 130.0)
								{
									level = 1;
								}
								else
								{
									level = 0;
								}

								m.ApplyPoison(this, Poison.GetPoison(level));

								m.FixedParticles(0x374A, 10, 15, 5021, EffectLayer.Waist);
								m.PlaySound(0x474);
							}

							break;
						}
				}
			}
		}

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
