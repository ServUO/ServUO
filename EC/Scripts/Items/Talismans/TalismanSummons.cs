#region Header
// **********
// ServUO - TalismanSummons.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;

using Server.ContextMenus;
using Server.Items;
using Server.Regions;
#endregion

namespace Server.Mobiles
{
	public class BaseTalismanSummon : BaseCreature
	{
		public BaseTalismanSummon()
			: base(AIType.AI_Melee, FightMode.None, 10, 1, 0.2, 0.4)
		{ }

		public BaseTalismanSummon(Serial serial)
			: base(serial)
		{ }

		public override bool Commandable { get { return false; } }
		public override bool InitialInnocent { get { return true; } }
		public override bool IsInvulnerable { get { return true; } }

		public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
		{
			if (from.Alive && ControlMaster == from)
			{
				list.Add(new TalismanReleaseEntry(this));
			}
		}

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

		private class TalismanReleaseEntry : ContextMenuEntry
		{
			private readonly Mobile m_Mobile;

			public TalismanReleaseEntry(Mobile m)
				: base(6118, 3)
			{
				m_Mobile = m;
			}

			public override void OnClick()
			{
				Effects.SendLocationParticles(
					EffectItem.Create(m_Mobile.Location, m_Mobile.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
				Effects.PlaySound(m_Mobile, m_Mobile.Map, 0x201);

				m_Mobile.Delete();
			}
		}
	}

	public class SummonedAntLion : BaseTalismanSummon
	{
		[Constructable]
		public SummonedAntLion()
		{
			Name = "an ant lion";
			Body = 787;
			BaseSoundID = 1006;
		}

		public SummonedAntLion(Serial serial)
			: base(serial)
		{ }

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

	public class SummonedArcticOgreLord : BaseTalismanSummon
	{
		[Constructable]
		public SummonedArcticOgreLord()
		{
			Name = "an arctic ogre lord";
			Body = 135;
			BaseSoundID = 427;
		}

		public SummonedArcticOgreLord(Serial serial)
			: base(serial)
		{ }

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

	public class SummonedBakeKitsune : BaseTalismanSummon
	{
		[Constructable]
		public SummonedBakeKitsune()
		{
			Name = "a bake kitsune";
			Body = 246;
			BaseSoundID = 0x4DD;
		}

		public SummonedBakeKitsune(Serial serial)
			: base(serial)
		{ }

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

	public class SummonedBogling : BaseTalismanSummon
	{
		[Constructable]
		public SummonedBogling()
		{
			Name = "a bogling";
			Body = 779;
			BaseSoundID = 422;
		}

		public SummonedBogling(Serial serial)
			: base(serial)
		{ }

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

	public class SummonedBullFrog : BaseTalismanSummon
	{
		[Constructable]
		public SummonedBullFrog()
		{
			Name = "a bull frog";
			Body = 81;
			Hue = Utility.RandomList(0x5AC, 0x5A3, 0x59A, 0x591, 0x588, 0x57F);
			BaseSoundID = 0x266;
		}

		public SummonedBullFrog(Serial serial)
			: base(serial)
		{ }

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

	public class SummonedChicken : BaseTalismanSummon
	{
		[Constructable]
		public SummonedChicken()
		{
			Name = "a chicken";
			Body = 0xD0;
			BaseSoundID = 0x6E;
		}

		public SummonedChicken(Serial serial)
			: base(serial)
		{ }

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

	public class SummonedCow : BaseTalismanSummon
	{
		[Constructable]
		public SummonedCow()
		{
			Name = "a cow";
			Body = Utility.RandomList(0xD8, 0xE7);
			BaseSoundID = 0x78;
		}

		public SummonedCow(Serial serial)
			: base(serial)
		{ }

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

	public class SummonedDoppleganger : BaseTalismanSummon
	{
		[Constructable]
		public SummonedDoppleganger()
		{
			Name = "a doppleganger";
			Body = 0x309;
			BaseSoundID = 0x451;
		}

		public SummonedDoppleganger(Serial serial)
			: base(serial)
		{ }

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

	public class SummonedFrostSpider : BaseTalismanSummon
	{
		[Constructable]
		public SummonedFrostSpider()
		{
			Name = "a frost spider";
			Body = 20;
			BaseSoundID = 0x388;
		}

		public SummonedFrostSpider(Serial serial)
			: base(serial)
		{ }

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

	public class SummonedGreatHart : BaseTalismanSummon
	{
		[Constructable]
		public SummonedGreatHart()
		{
			Name = "a great hart";
			Body = 0xEA;
			BaseSoundID = 0x82;
		}

		public SummonedGreatHart(Serial serial)
			: base(serial)
		{ }

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

	public class SummonedLavaSerpent : BaseTalismanSummon
	{
		private DateTime m_NextWave;

		[Constructable]
		public SummonedLavaSerpent()
		{
			Name = "a lava serpent";
			Body = 90;
			BaseSoundID = 219;
		}

		public SummonedLavaSerpent(Serial serial)
			: base(serial)
		{ }

		public override void OnThink()
		{
			if (m_NextWave < DateTime.UtcNow)
			{
				AreaHeatDamage();
			}
		}

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

		public void AreaHeatDamage()
		{
			Mobile mob = ControlMaster;

			if (mob != null)
			{
				if (mob.InRange(Location, 2))
				{
					if (mob.IsStaff())
					{
						AOS.Damage(mob, Utility.Random(2, 3), 0, 100, 0, 0, 0);
						mob.SendLocalizedMessage(1008112); // The intense heat is damaging you!
					}
				}

				GuardedRegion r = Region as GuardedRegion;

				if (r != null && mob.Alive)
				{
					foreach (Mobile m in GetMobilesInRange(2))
					{
						if (!mob.CanBeHarmful(m))
						{
							mob.CriminalAction(false);
						}
					}
				}
			}

			m_NextWave = DateTime.UtcNow + TimeSpan.FromSeconds(3);
		}
	}

	public class SummonedOrcBrute : BaseTalismanSummon
	{
		[Constructable]
		public SummonedOrcBrute()
		{
			Body = 189;
			Name = "an orc brute";
			BaseSoundID = 0x45A;
		}

		public SummonedOrcBrute(Serial serial)
			: base(serial)
		{ }

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

	public class SummonedPanther : BaseTalismanSummon
	{
		[Constructable]
		public SummonedPanther()
		{
			Name = "a panther";
			Body = 0xD6;
			Hue = 0x901;
			BaseSoundID = 0x462;
		}

		public SummonedPanther(Serial serial)
			: base(serial)
		{ }

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

	public class SummonedSheep : BaseTalismanSummon
	{
		[Constructable]
		public SummonedSheep()
		{
			Name = "a sheep";
			Body = 0xCF;
			BaseSoundID = 0xD6;
		}

		public SummonedSheep(Serial serial)
			: base(serial)
		{ }

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

	public class SummonedSkeletalKnight : BaseTalismanSummon
	{
		[Constructable]
		public SummonedSkeletalKnight()
		{
			Name = "a skeletal knight";
			Body = 147;
			BaseSoundID = 451;
		}

		public SummonedSkeletalKnight(Serial serial)
			: base(serial)
		{ }

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

	public class SummonedVorpalBunny : BaseTalismanSummon
	{
		[Constructable]
		public SummonedVorpalBunny()
		{
			Name = "a vorpal bunny";
			Body = 205;
			Hue = 0x480;
			BaseSoundID = 0xC9;

			Timer.DelayCall(TimeSpan.FromMinutes(30.0), BeginTunnel);
		}

		public SummonedVorpalBunny(Serial serial)
			: base(serial)
		{ }

		public virtual void BeginTunnel()
		{
			if (Deleted)
			{
				return;
			}

			new VorpalBunny.BunnyHole().MoveToWorld(Location, Map);

			Frozen = true;
			Say("* The bunny begins to dig a tunnel back to its underground lair *");
			PlaySound(0x247);

			Timer.DelayCall(TimeSpan.FromSeconds(5.0), Delete);
		}

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

	public class SummonedWailingBanshee : BaseTalismanSummon
	{
		[Constructable]
		public SummonedWailingBanshee()
		{
			Name = "a wailing banshee";
			Body = 310;
			BaseSoundID = 0x482;
		}

		public SummonedWailingBanshee(Serial serial)
			: base(serial)
		{ }

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