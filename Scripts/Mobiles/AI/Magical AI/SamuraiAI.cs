#region References
using System;

using Server.Spells;
using Server.Spells.Bushido;
#endregion

namespace Server.Mobiles
{
	public class SamuraiAI : MeleeAI
	{
		private DateTime m_NextCastTime;
		private DateTime m_NextSpecial;

		public SamuraiAI(BaseCreature bc)
			: base(bc)
		{
			m_NextCastTime = DateTime.UtcNow;
			m_NextSpecial = DateTime.UtcNow;
		}

		public virtual SpecialMove GetSpecialMove()
		{
			var skill = (int)m_Mobile.Skills[SkillName.Bushido].Value;

			if (skill <= 50)
				return null;

			if (m_Mobile.Combatant != null && m_Mobile.Combatant.Hits <= 10 && skill >= 25)
				return SpellRegistry.GetSpecialMove(400); //new HonerableExecution();
			if (skill >= 70 && CheckForMomentumStrike() && 0.5 > Utility.RandomDouble())
				return SpellRegistry.GetSpecialMove(405); //new MomentumStrike();
			return SpellRegistry.GetSpecialMove(404); // new LightningStrike();
		}

		private bool CheckForMomentumStrike()
		{
			var count = 0;
			IPooledEnumerable eable = m_Mobile.GetMobilesInRange(1);

			foreach (Mobile m in eable)
			{
				if (m.CanBeHarmful(m_Mobile) && m != m_Mobile)
					count++;
			}

			eable.Free();

			return count > 1;
		}

		public virtual Spell GetRandomSpell()
		{
			// 25 - Confidence
			// 40 - Counter Attack
			// 60 - Evasion
			var skill = (int)m_Mobile.Skills[SkillName.Bushido].Value;

			if (skill < 25)
				return null;

			var avail = 1;

			if (skill >= 60)
				avail = 3;
			else if (skill >= 40)
				avail = 2;

			switch (Utility.Random(avail))
			{
				case 0:
					return new Confidence(m_Mobile, null);
				case 1:
					return new CounterAttack(m_Mobile, null);
				case 2:
					return new Evasion(m_Mobile, null);
			}

			return null;
		}

		public override bool DoActionCombat()
		{
			base.DoActionCombat();

			var c = m_Mobile.Combatant as Mobile;

			if (c != null)
			{
				var move = SpecialMove.GetCurrentMove(m_Mobile);

				if (move == null && m_NextSpecial < DateTime.UtcNow && 0.05 > Utility.RandomDouble())
				{
					move = GetSpecialMove();

					if (move != null)
					{
						SpecialMove.SetCurrentMove(m_Mobile, move);
						m_NextSpecial = DateTime.UtcNow + GetCastDelay();
					}
				}
				else if (m_Mobile.Spell == null && m_NextCastTime < DateTime.UtcNow && 0.05 > Utility.RandomDouble())
				{
					var spell = GetRandomSpell();

					if (spell != null)
					{
						spell.Cast();
						m_NextCastTime = DateTime.UtcNow + GetCastDelay();
					}
				}
			}

			return true;
		}

		public TimeSpan GetCastDelay()
		{
			var skill = (int)m_Mobile.Skills[SkillName.Bushido].Value;

			if (skill >= 60)
				return TimeSpan.FromSeconds(15);
			if (skill > 25)
				return TimeSpan.FromSeconds(30);

			return TimeSpan.FromSeconds(45);
		}
	}
}