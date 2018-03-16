using System;
using Server;
using System.Collections.Generic;
using Server.Spells.Spellweaving;
using Server.Spells;
using Server.Targeting;
using Server.Items;

namespace Server.Mobiles
{
	public class SpellweavingAI : MagicalAI
	{
		public override SkillName CastSkill { get { return SkillName.Spellweaving; } }
		
		public SpellweavingAI(BaseCreature m) : base(m)
		{
		}
		
		public override Spell GetRandomBuffSpell()
		{
			BaseWeapon wep = m_Mobile.Weapon as BaseWeapon;
			int skill = (int)m_Mobile.Skills[SkillName.Spellweaving].Value;

			if (skill >= 24 && !ArcaneEmpowermentSpell.IsUnderEffects(m_Mobile) && 0.5 >= Utility.RandomDouble())
				return new ArcaneEmpowermentSpell(m_Mobile, null);
			else if(skill >= 10 && wep != null && !(wep is Fists) && !wep.Immolating)
				return new ImmolatingWeaponSpell(m_Mobile, null);
			else
				return base.GetRandomBuffSpell();
		}
		
		public override Spell GetRandomDamageSpell()
		{
			int skill = (int)m_Mobile.Skills[SkillName.Spellweaving].Value;
			int mana = m_Mobile.Mana;
			int select = 1;

			if(skill >= 83 && mana >= 50)
				select = 4;
			else if (skill >= 66 && mana >= 50)
				select = 3;
			else if (skill >= 52 && mana >= 40)
				select = 2;

		   switch ( Utility.Random(select) )
		   {
				case 0: return new ThunderstormSpell(m_Mobile, null);
				case 1: return new EssenceOfWindSpell(m_Mobile, null);
				case 2: return new WildfireSpell(m_Mobile, null);
				case 3: return new WordOfDeathSpell(m_Mobile, null);
			}
				
			return null;
		}
		
		public override Spell GetHealSpell()
		{
			return new GiftOfRenewalSpell(m_Mobile, null);
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