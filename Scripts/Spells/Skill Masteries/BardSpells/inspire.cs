/*Party Hit chance increase by up to 15%, Damage increase by up to 40%, 
SDI increased by up to 15% (PvP Cap 15)(Provocation Based)*/

namespace Server.Spells.SkillMasteries
{
    public class InspireSpell : BardSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
                "Inspire", "Unus Por",
                -1,
                9002
            );

        public override double RequiredSkill => 90;
        public override double UpKeep => 4;
        public override int RequiredMana => 16;
        public override SkillName CastSkill => SkillName.Provocation;
        public override bool PartyEffects => true;

        private int m_PropertyBonus;
        private int m_DamageBonus;
        private int m_DamageModifier;

        public InspireSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
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
                m_PropertyBonus = (int)((BaseSkillBonus * 2) + CollectiveBonus);
                m_DamageBonus = (int)((BaseSkillBonus * 5) + (CollectiveBonus * 3));
                m_DamageModifier = (int)((BaseSkillBonus + 1) + CollectiveBonus);

                UpdateParty();
                BeginTimer();
            }

            FinishSequence();
        }

        public override void AddPartyEffects(Mobile m)
        {
            m.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
            m.SendLocalizedMessage(1115736); // You feel inspired by the bard's spellsong.

            string args = string.Format("{0}\t{1}\t{2}\t{3}", m_PropertyBonus, m_PropertyBonus, m_DamageBonus, m_DamageModifier);
            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Inspire, 1115683, 1151951, args.ToString()));
        }

        public override void RemovePartyEffects(Mobile m)
        {
            BuffInfo.RemoveBuff(m, BuffIcon.Inspire);
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
        /// Called in AOS.cs - Hit Chance/Spell Damage Bonus
        /// </summary>
        /// <returns>HCI/SDI Bonus</returns>
        public override int PropertyBonus()
        {
            return m_PropertyBonus;
        }

        /// <summary>
        /// Called in AOS.cs - Weapon Damage Bonus
        /// </summary>
        /// <returns>DamInc Bonus</returns>
        public override int DamageBonus()
        {
            return m_DamageBonus;
        }

        public void DoDamage(ref int damageTaken)
        {
            damageTaken += AOS.Scale(damageTaken, m_DamageModifier);
        }
    }
}
