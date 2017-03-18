using System;
using Server;
using System.Collections.Generic;
using Server.Spells.Mystic;
using Server.Spells;
using Server.Targeting;

namespace Server.Mobiles
{
	public class MysticAI : MageAI
	{
		public virtual bool UseMagery { get { return m_Mobile.Skills[SkillName.Magery].Value >= 20; } }
		
		public MysticAI(BaseCreature m) : base(m)
		{
		}
		
		public override Spell GetRandomDamageSpell()
        {
            return UseMagery && 0.50 > Utility.RandomDouble() ? base.GetRandomDamageSpell() : GetRandomDamageSpellMystic();
        }
		
		public override Spell GetRandomCurseSpell()
		{
			return UseMagery && 0.50 > Utility.RandomDouble() ? base.GetRandomCurseSpell() : GetRandomCurseSpellMystic();
		}
		
		public virtual Spell GetRandomDamageSpellMystic()
		{
			int circle = (int)((m_Mobile.Skills[SkillName.Mysticism].Value + 20.0) / (100.0 / 7.0));
			
			if (circle < 1) circle = 1;
            else if (circle > 8) circle = 8;

			switch (Utility.Random(circle))
                {
                    case 0:
                    case 1: return new NetherBoltSpell(m_Mobile, null);
                    case 2:
                    case 3: return new EagleStrikeSpell(m_Mobile, null);
                    case 4:
                    case 5: return new BombardSpell(m_Mobile, null);
                    case 6: return new HailStormSpell(m_Mobile, null);
                    case 7:
                    default: return new NetherCycloneSpell(m_Mobile, null);
                }
		}
		
		public virtual Spell GetRandomCurseSpellMystic()
		{
			int maxCircle = (int)((m_Mobile.Skills[SkillName.Mysticism].Value + 20.0) / (100.0 / 7.0));
			if (maxCircle < 0) maxCircle = 0;

			switch (Utility.Random(maxCircle))
			{
				case 0: return new PurgeMagicSpell(m_Mobile, null);
				case 1:
				case 2: return new SleepSpell(m_Mobile, null);
				case 3:
				case 4: return new MassSleepSpell(m_Mobile, null);
				case 5:
                case 6: return new HailStormSpell(m_Mobile, null);
				case 7:
				default: return new SpellPlagueSpell(m_Mobile, null);
			}
		}
		
		protected override bool ProcessTarget()
		{
			Target t = m_Mobile.Target;
			
			if(t is MysticSpell.MysticSpellTarget && ((MysticSpell.MysticSpellTarget)t).Owner is HailStormSpell)
			{
                if (m_Mobile.Combatant != null && m_Mobile.InRange(m_Mobile.Combatant.Location, 8))
				{
					t.Invoke(m_Mobile, m_Mobile.Combatant);
				}
				else
					t.Invoke(m_Mobile, m_Mobile);
					
				return true;
			}
			
			return base.ProcessTarget();
		}
	}
}