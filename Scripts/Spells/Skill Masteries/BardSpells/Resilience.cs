/*Poison resistance increase(not the stat), Mortal, Bleed, Curse effect Durations decreased, 
Hp regen bonus 2-11, mana regen bonus 2-11, stamina regen bonus 2-11.*/

namespace Server.Spells.SkillMasteries
{
    public class ResilienceSpell : BardSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
                "Resilience", "Kal Mani Tym",
                -1,
                9002
            );

        public override double RequiredSkill => 90;
        public override double UpKeep => 4;
        public override int RequiredMana => 16;
        public override bool PartyEffects => true;
        public override SkillName CastSkill => SkillName.Peacemaking;

        private int m_PropertyBonus;

        public ResilienceSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override void OnCast()
        {
            BardSpell spell = GetSpell(Caster, GetType()) as BardSpell;

            if (spell != null)
            {
                spell.Expire();
                Caster.SendLocalizedMessage(1115774); //You halt your spellsong.
            }
            else if (CheckSequence())
            {
                m_PropertyBonus = (int)((BaseSkillBonus * 2) + CollectiveBonus); // 2 - 16 (22)

                UpdateParty();
                BeginTimer();
            }

            FinishSequence();
        }

        public override void AddPartyEffects(Mobile m)
        {
            m.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
            m.SendLocalizedMessage(1115738); // The bard's spellsong fills you with a feeling of resilience.

            string args = string.Format("{0}\t{1}\t{2}", m_PropertyBonus, m_PropertyBonus, m_PropertyBonus);
            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Resilience, 1115614, 1115731, args.ToString()));

            m.ResetStatTimers();
        }

        public override void RemovePartyEffects(Mobile m)
        {
            BuffInfo.RemoveBuff(m, BuffIcon.Resilience);

            m.ResetStatTimers();
        }

        public override void EndEffects()
        {
            if (PartyList != null)
            {
                foreach (Mobile m in PartyList) //Original Party list
                {
                    RemovePartyEffects(m);
                }
            }

            RemovePartyEffects(Caster);
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

        public static bool UnderEffects(Mobile m)
        {
            return UnderPartyEffects(m, typeof(ResilienceSpell));
        }
    }
}
