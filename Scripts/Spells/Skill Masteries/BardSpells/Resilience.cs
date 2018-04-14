using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Mobiles;

/*Poison resistance increase(not the stat), Mortal, Bleed, Curse effect Durations decreased, 
Hp regen bonus 2-11, mana regen bonus 2-11, stamina regen bonus 2-11.*/

namespace Server.Spells.SkillMasteries
{
	public class ResilienceSpell : BardSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Resilience", "Kal Mani Tym",
				-1,
				9002
			);

		public override double RequiredSkill{ get { return 90; } }
		public override double UpKeep { get { return 4; } }
        public override int RequiredMana { get { return 16; } }
		public override bool PartyEffects { get { return true; } }
        public override SkillName CastSkill { get { return SkillName.Peacemaking; } }

        private int m_PropertyBonus;

		public ResilienceSpell( Mobile caster, Item scroll ) : base(caster, scroll, m_Info)
		{
		}
		
		public override void OnCast()
		{
            BardSpell spell = SkillMasterySpell.GetSpell(Caster, this.GetType()) as BardSpell;
			
			if(spell != null)
			{
				spell.Expire();
                Caster.SendLocalizedMessage(1115774); //You halt your spellsong.
			}
			else if ( CheckSequence() )
			{
                m_PropertyBonus = (int)((BaseSkillBonus * 8) + (CollectiveBonus * 3));

                foreach (Mobile m in GetParty())
                {
                    m.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
                    m.SendLocalizedMessage(1115738); // The bard's spellsong fills you with a feeling of resilience.

                    string args = String.Format("{0}\t{1}\t{2}", m_PropertyBonus, m_PropertyBonus, m_PropertyBonus);
                    BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Resilience, 1115614, 1115731, args.ToString()));
                }

				BeginTimer();
			}
			
			FinishSequence();
		}

        public override void EndEffects()
        {
            if (PartyList != null)
            {
                foreach (Mobile m in PartyList) //Original Party list
                {
                    BuffInfo.RemoveBuff(m, BuffIcon.Resilience);
                }
            }

            BuffInfo.RemoveBuff(Caster, BuffIcon.Resilience);
        }

		/// <summary>
		/// Called in AOS.cs - Regen bonus
		/// </summary>
		/// <returns>All 3 regen bonuses</returns>
		public override int PropertyBonus()
		{
            return m_PropertyBonus;
		}
		
        /*Notes:
         * Poison Resist 25% flat rate in spell is active - TODO: Get OSI Rate??? 
         * Bleed, Mortal and Curse cuts time by 1/2
         * Reference PlayerMobile, BleedAttack, MortalStrike and CurseSpell
         */ 

	}
}