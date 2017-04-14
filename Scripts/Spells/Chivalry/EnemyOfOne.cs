using System;
using System.Collections;
using Server.Mobiles;

namespace Server.Spells.Chivalry
{
	public class EnemyOfOneSpell : PaladinSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Enemy of One", "Forul Solum",
			-1,
			9002);

		private static readonly Hashtable m_Table = new Hashtable();

		public EnemyOfOneSpell(Mobile caster, Item scroll)
			: base(caster, scroll, m_Info)
		{
		}

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds(0.5); } }

		public override double RequiredSkill { get { return 45.0; } }
		public override int RequiredMana { get { return 20; } }
		public override int RequiredTithing { get { return 10; } }
		public override int MantraNumber { get { return 1060723; } } // Forul Solum
		public override bool BlocksMovement { get { return false; } }

		public override TimeSpan GetCastDelay()
		{
			if (Core.SA && UnderEffect(Caster))
				return TimeSpan.Zero;

			return base.GetCastDelay();
		}

		public override void OnCast()
		{
			if (Core.SA && UnderEffect(Caster))
			{
				PlayEffects();

				// As per Pub 71, Enemy of one has now been changed to a Spell Toggle. You can remove the effect
				// before the duration expires by recasting the spell.
				RemoveEffect(Caster);
			}
			else if (CheckSequence())
			{
				PlayEffects();

				Timer t = (Timer)m_Table[Caster];

				if (t != null)
				{
					t.Stop();
					RemoveEffect(Caster);
				}

				double delay = (double)ComputePowerValue(1) / 60;

				// TODO: Should caps be applied?
				if (delay < 1.5)
					delay = 1.5;
				else if (delay > 3.5)
					delay = 3.5;

				m_Table[Caster] = Timer.DelayCall(TimeSpan.FromMinutes(delay), RemoveEffect, Caster);

				if (Caster is PlayerMobile)
				{
					((PlayerMobile)Caster).EnemyOfOneType = null;
					((PlayerMobile)Caster).WaitingForEnemy = true;

					BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.EnemyOfOne, 1075653, 1075902, TimeSpan.FromMinutes(delay), Caster, "50\t100"));
				}
			}

			FinishSequence();
		}

		private void PlayEffects()
		{
			Caster.PlaySound(0x0F5);
			Caster.PlaySound(0x1ED);

			Caster.FixedParticles(0x375A, 1, 30, 9966, 33, 2, EffectLayer.Head);
			Caster.FixedParticles(0x37B9, 1, 30, 9502, 43, 3, EffectLayer.Head);
		}

		private static bool UnderEffect(Mobile m)
		{
			return m_Table.ContainsKey(m);
		}

		private static void RemoveEffect(Mobile m)
		{
			m_Table.Remove(m);

			m.PlaySound(0x1F8);

			if (m is PlayerMobile)
			{
				((PlayerMobile)m).EnemyOfOneType = null;
				((PlayerMobile)m).WaitingForEnemy = false;
			}

			BuffInfo.RemoveBuff(m, BuffIcon.EnemyOfOne);
		}
	}
}
