using Server;
using System;
using Server.Spells.Bushido;
using Server.Spells;

namespace Server.Mobiles
{
	public class SamuraiAI : MeleeAI
	{
		private DateTime m_NextCastTime;
		private DateTime m_NextSpecial;
		
		public SamuraiAI(BaseCreature bc) : base(bc)
		{
            m_NextCastTime = DateTime.UtcNow;
            m_NextSpecial = DateTime.UtcNow;
		}
		
		public virtual SpecialMove GetSpecialMove()
		{
			int skill = (int)m_Mobile.Skills[SkillName.Bushido].Value;
			
			if(skill <= 50)
				return null;

            if (m_Mobile.Combatant != null && m_Mobile.Combatant.Hits <= 10 && skill >= 25)
                return SpellRegistry.GetSpecialMove(400); //new HonerableExecution();
            else if (skill >= 70 && CheckForMomentumStrike() && 0.5 > Utility.RandomDouble())
                return SpellRegistry.GetSpecialMove(405); //new MomentumStrike();
            else
                return SpellRegistry.GetSpecialMove(404); // new LightningStrike();
		}
		
		private bool CheckForMomentumStrike()
		{
			int count = 0;
			IPooledEnumerable eable = m_Mobile.GetMobilesInRange(1);
			
			foreach(Mobile m in eable)
			{
				if(m.CanBeHarmful(m_Mobile) && m != m_Mobile)
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
			int skill = (int)m_Mobile.Skills[SkillName.Bushido].Value;
			
			if(skill < 25)
				return null;
			
			int avail = 1;
			
			if(skill >= 60)
				avail = 3;
			else if (skill >= 40)
				avail = 2;
				
			switch(Utility.Random(avail))
			{
				case 0: return new Confidence(m_Mobile, null);
				case 1: return new CounterAttack(m_Mobile, null);
				case 2: return new Evasion(m_Mobile, null);
			}

            return null;
		}
		
        public override bool DoActionCombat()
        {
			base.DoActionCombat();
			
			Mobile c = m_Mobile.Combatant;
			
			if(c != null)
			{
				SpecialMove move = SpecialMove.GetCurrentMove(m_Mobile);
				
				if(move == null && m_NextSpecial < DateTime.UtcNow && 0.05 > Utility.RandomDouble())
				{
					move = GetSpecialMove();

					if(move != null)
					{
						SpecialMove.SetCurrentMove(m_Mobile, move);
                        m_NextSpecial = DateTime.UtcNow + GetCastDelay();
					}
				}
				else if (m_Mobile.Spell == null && m_NextCastTime < DateTime.UtcNow && 0.05 > Utility.RandomDouble())
				{
					Spell spell = GetRandomSpell();
					
					if(spell != null)
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
            int skill = (int)m_Mobile.Skills[SkillName.Bushido].Value;

            if (skill >= 60)
                return TimeSpan.FromSeconds(15);
            else if (skill > 25)
                return TimeSpan.FromSeconds(30);

            return TimeSpan.FromSeconds(45);
        }
	}
}