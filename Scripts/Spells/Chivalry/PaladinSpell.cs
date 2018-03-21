#region Header
// **********
// ServUO - PaladinSpell.cs
// **********
#endregion

#region References
using System;

using Server.Network;
#endregion

namespace Server.Spells.Chivalry
{
	public abstract class PaladinSpell : Spell
	{
		public PaladinSpell(Mobile caster, Item scroll, SpellInfo info)
			: base(caster, scroll, info)
		{ }

		public abstract double RequiredSkill { get; }
		public abstract int RequiredMana { get; }
		public abstract int RequiredTithing { get; }
		public abstract int MantraNumber { get; }
		public override SkillName CastSkill { get { return SkillName.Chivalry; } }
		public override SkillName DamageSkill { get { return SkillName.Chivalry; } }
		public override bool ClearHandsOnCast { get { return false; } }
		//public override int CastDelayBase{ get{ return 1; } }
		public override int CastRecoveryBase { get { return 7; } }

		public static int ComputePowerValue(Mobile from, int div)
		{
			if (from == null)
			{
				return 0;
			}

			int v = (int)Math.Sqrt(from.Karma + 20000 + (from.Skills.Chivalry.Fixed * 10));

			return v / div;
		}

		public override bool CheckCast()
		{
			int mana = ScaleMana(RequiredMana);

			if (!base.CheckCast())
			{
				return false;
			}

			if (Caster.Player && Caster.TithingPoints < RequiredTithing)
			{
				Caster.SendLocalizedMessage(1060173, RequiredTithing.ToString());
					// You must have at least ~1_TITHE_REQUIREMENT~ Tithing Points to use this ability,
				return false;
			}
			else if (Caster.Mana < mana)
			{
				Caster.SendLocalizedMessage(1060174, mana.ToString());
					// You must have at least ~1_MANA_REQUIREMENT~ Mana to use this ability.
				return false;
			}

			return true;
		}

		public override bool CheckFizzle()
		{
			int requiredTithing = Caster.Player ? RequiredTithing : 0;

			if (AosAttributes.GetValue(Caster, AosAttribute.LowerRegCost) > Utility.Random(100))
			{
				requiredTithing = 0;
			}

			int mana = ScaleMana(RequiredMana);

			if (Caster.TithingPoints < requiredTithing)
			{
				Caster.SendLocalizedMessage(1060173, RequiredTithing.ToString());
					// You must have at least ~1_TITHE_REQUIREMENT~ Tithing Points to use this ability,
				return false;
			}
			else if (Caster.Mana < mana)
			{
				Caster.SendLocalizedMessage(1060174, mana.ToString());
					// You must have at least ~1_MANA_REQUIREMENT~ Mana to use this ability.
				return false;
			}

			Caster.TithingPoints -= requiredTithing;

			if (!base.CheckFizzle())
			{
				return false;
			}

			Caster.Mana -= mana;

			return true;
		}

		public override void SayMantra()
		{
            if(Caster.Player)
			    Caster.PublicOverheadMessage(MessageType.Regular, 0x3B2, MantraNumber, "", false);
		}

		public override void DoFizzle()
		{
			Caster.PlaySound(0x1D6);
			Caster.NextSpellTime = Core.TickCount;
		}

		public override void DoHurtFizzle()
		{
			Caster.PlaySound(0x1D6);
		}

		public override bool CheckDisturb(DisturbType type, bool firstCircle, bool resistable)
		{
			// Cannot disturb Chivalry spells
			return false;
		}

		public override void SendCastEffect()
		{
            if(Caster.Player)
			    Caster.FixedEffect(0x37C4, 87, (int)(GetCastDelay().TotalSeconds * 28), 4, 3);
		}

		public override void GetCastSkills(out double min, out double max)
		{
			min = RequiredSkill;
			max = RequiredSkill + 50.0;
		}

		public override int GetMana()
		{
			return 0;
		}

		public int ComputePowerValue(int div)
		{
			return ComputePowerValue(Caster, div);
		}
	}
}