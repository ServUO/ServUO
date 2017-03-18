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
		public virtual bool UseMagery { get { return m_Mobile.Skills[SkillName.Magery].Value >= 20; } }
		
		public SpellweavingAI(BaseCreature m) : base(m)
		{
		}
		
		public override Spell GetRandomDamageSpell()
        {
            return UseMagery && 0.50 > Utility.RandomDouble() ? base.GetRandomDamageSpell() : GetRandomDamageSpellSpellweaving();
        }
		
		public override Spell GetRandomBuffSpell()
		{
			BaseWeapon wep = m_Mobile.Weapon as BaseWeapon;
			int skill = (int)m_Mobile.Skills[SkillName.Spellweaving].Value;

			if(!GiftOfRenewalSpell.IsUnderEffects(m_Mobile) && 0.33 >= Utility.RandomDouble())
				return new GiftOfRenewalSpell(m_Mobile, null);
			else if (skill >= 24 && ArcaneEmpowermentSpell.IsUnderEffects(m_Mobile) && 0.33 >= Utility.RandomDouble())
				return new ArcaneEmpowermentSpell(m_Mobile, null);
			else if(skill >= 10 && wep != null && !(wep is Fists) && !wep.Immolating && 0.33 >= Utility.RandomDouble())
				return new ImmolatingWeaponSpell(m_Mobile, null);
			else
				return base.GetRandomBuffSpell();
		}
		
		public virtual Spell GetRandomDamageSpellSpellweaving()
		{
			int skill = (int)m_Mobile.Skills[SkillName.Spellweaving].Value;

			if(skill >= 10 && skill - 70 > Utility.Random(100))
			{
               int pick = 4;
 
                if (skill < 10)
                    pick = 0;
                else if (skill < 52)
                    pick = 1;
                else if (skill < 66)
                    pick = 2;
                else if (skill < 80)
                    pick = 3;
 
               switch ( Utility.Random( pick ) )
               {
                   default:
					case 0: return new ThunderstormSpell(m_Mobile, null);
					case 1: return new EssenceOfWindSpell(m_Mobile, null);
					case 2: return new WildfireSpell(m_Mobile, null);
					case 3: return new WordOfDeathSpell(m_Mobile, null);
				}
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