#region Header
// **********
// ServUO - GreenGoblinScout.cs
// **********
#endregion

#region References
using Server.Targeting;
#endregion

namespace Server.Mobiles
{
	[CorpseName("an goblin corpse")]
	public class GreenGoblinScout : BaseCreature
	{
		//public override InhumanSpeech SpeechType { get { return InhumanSpeech.Orc; } }
		[Constructable]
		public GreenGoblinScout()
			: base(AIType.AI_OrcScout, FightMode.Closest, 10, 7, 0.2, 0.4)
		{
			Name = "an green goblin scout";
			Body = 723;
			BaseSoundID = 0x45A;

			SetStr(250, 261);
			SetDex(65, 70);
			SetInt(105, 108);

			SetHits(200, 204);
			SetMana(100, 108);

			SetDamage(5, 7);

			SetDamageType(ResistanceType.Physical, 100);

			SetResistance(ResistanceType.Physical, 35, 45);
			SetResistance(ResistanceType.Fire, 30, 33);
			SetResistance(ResistanceType.Cold, 25, 28);
			SetResistance(ResistanceType.Poison, 10, 13);
			SetResistance(ResistanceType.Energy, 10, 11);

			SetSkill(SkillName.MagicResist, 105.1, 110.2);
			SetSkill(SkillName.Tactics, 85.1, 89.1);

			SetSkill(SkillName.Wrestling, 90.1, 92.9);
			SetSkill(SkillName.Anatomy, 70.1, 80.3);

			Fame = 1500;
			Karma = -1500;
		}

		public GreenGoblinScout(Serial serial)
			: base(serial)
		{ }

		public override OppositionGroup OppositionGroup { get { return OppositionGroup.SavagesAndOrcs; } }
		public override bool CanRummageCorpses { get { return true; } }
		public override int Meat { get { return 1; } }

		public override void GenerateLoot()
		{
			AddLoot(LootPack.Meager);
		}

		public override void OnThink()
		{
			if (Utility.RandomDouble() < 0.2)
			{
				TryToDetectHidden();
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

		private Mobile FindTarget()
		{
			foreach (Mobile m in GetMobilesInRange(10))
			{
				if (m.Player && m.Hidden && m.IsPlayer())
				{
					return m;
				}
			}

			return null;
		}

		private void TryToDetectHidden()
		{
			Mobile m = FindTarget();

			if (m != null)
			{
				if (Core.TickCount >= NextSkillTime && UseSkill(SkillName.DetectHidden))
				{
					Target targ = Target;

					if (targ != null)
					{
						targ.Invoke(this, this);
					}

					Effects.PlaySound(Location, Map, 0x340);
				}
			}
		}
	}
}