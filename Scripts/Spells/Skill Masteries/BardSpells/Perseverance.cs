/*Party Defense Chance increased by up to 22%, Damage reduced by up to 22%.
Casting focus bonus 1-6%.*/

namespace Server.Spells.SkillMasteries
{
    public class PerseveranceSpell : BardSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
                "Perserverance", "Unus Jux Sanct",
                -1,
                9002
            );

        public override double RequiredSkill => 90;
        public override double UpKeep => 5;
        public override int RequiredMana => 18;
        public override bool PartyEffects => true;
        public override SkillName CastSkill => SkillName.Peacemaking;

        private int m_PropertyBonus;
        private int m_PropertyBonus2;
        private int m_DamageMod;

        public PerseveranceSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
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
                m_PropertyBonus = (int)((BaseSkillBonus * 3) + CollectiveBonus);            // 2 - 24 (30)
                m_PropertyBonus2 = (int)((BaseSkillBonus / 2) + (CollectiveBonus / 3));     // 1 - 4 (6)
                m_DamageMod = (int)((BaseSkillBonus * 3) + CollectiveBonus);                // 2 - 24 (30)

                UpdateParty();
                BeginTimer();
            }

            FinishSequence();
        }

        public override void AddPartyEffects(Mobile m)
        {
            m.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
            m.SendLocalizedMessage(1115739); // The bard's spellsong fills you with a feeling of invincibility.

            string args = string.Format("{0}\t-{1}\t{2}", m_PropertyBonus, m_DamageMod, m_PropertyBonus2);
            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Perseverance, 1115615, 1115732, args.ToString()));
        }

        public override void RemovePartyEffects(Mobile m)
        {
            BuffInfo.RemoveBuff(m, BuffIcon.Perseverance);
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
        /// Defense Chance Bonus
        /// </summary>
        /// <returns>Defense Chance Bonus</returns>
        public override int PropertyBonus()
        {
            return m_PropertyBonus;
        }

        /// <summary>
        /// Casting Focus
        /// </summary>
        /// <returns>Casting Focus</returns>
		public override int PropertyBonus2()
        {
            return m_PropertyBonus2;
        }

        /// <summary>
        /// modifies total damage dealt
        /// </summary>
        /// <param name="damage"></param>
        public void AbsorbDamage(ref int damage)
        {
            damage -= AOS.Scale(damage, m_DamageMod);
        }
    }
}
