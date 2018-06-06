using System;
using Server;
using System.Collections.Generic;
using Server.Spells.Spellweaving;
using Server.Spells;
using Server.Targeting;
using Server.Items;

namespace Server.Mobiles
{
    public class SpellweavingAI : MageAI
	{
		public override SkillName CastSkill { get { return SkillName.Spellweaving; } }
        public override bool UsesMagery { get { return m_Mobile.Skills[SkillName.Magery].Base >= 20.0 && !m_Mobile.Controlled; } }

		public SpellweavingAI(BaseCreature m) : base(m)
		{
		}
		
		public override Spell GetRandomBuffSpell()
        {
            if (UsesMagery && 0.5 > Utility.RandomDouble())
            {
                return base.GetRandomBuffSpell();
            }
            else
            {
                int mana = m_Mobile.Mana;
                BaseWeapon wep = m_Mobile.Weapon as BaseWeapon;

                if (mana >= 50 && !ArcaneEmpowermentSpell.IsUnderEffects(m_Mobile) && 0.5 >= Utility.RandomDouble())
                    return new ArcaneEmpowermentSpell(m_Mobile, null);
                else if (mana >= 32 && wep != null && !ImmolatingWeaponSpell.IsImmolating(m_Mobile, wep))
                    return new ImmolatingWeaponSpell(m_Mobile, null);
            }

            return null;
		}

        public override Spell GetRandomCurseSpell()
        {
            if (UsesMagery)
            {
                return base.GetRandomCurseSpell();
            }

            return null;
        }
		
		public override Spell GetRandomDamageSpell()
		{
            if (UsesMagery && 0.5 > Utility.RandomDouble())
            {
                return base.GetRandomDamageSpell();
            }
            else
            {
                int mana = m_Mobile.Mana;
                int select = 1;

                if (mana >= 50)
                    select = 4;
                else if (mana >= 40)
                    select = 3;
                else if (mana >= 30)
                    select = 2;

                switch (Utility.Random(select))
                {
                    case 0: return new ThunderstormSpell(m_Mobile, null);
                    case 1: return new EssenceOfWindSpell(m_Mobile, null);
                    case 2: return new WildfireSpell(m_Mobile, null);
                    case 3: return new WordOfDeathSpell(m_Mobile, null);
                }
            }

			return null;
		}
		
		public override Spell GetHealSpell()
		{
            if (UsesMagery && 0.5 > Utility.RandomDouble())
            {
                return base.GetHealSpell();
            }
            else if (m_Mobile.Mana >= 24)
            {
                return new GiftOfRenewalSpell(m_Mobile, null);
            }

            return null;
		}

        public override Spell GetCureSpell()
        {
            if (UsesMagery)
            {
                return base.GetCureSpell();
            }

            return null;
        }
		
		protected override bool ProcessTarget()
		{
			Target t = m_Mobile.Target;
			
			if(t is WildfireSpell.InternalTarget)
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