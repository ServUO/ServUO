using System;
using System.Collections.Generic;

/*Party Hit Points increased by up to 20 + 6(Collection Bonus), Party healed for 9-20 dmg every 4 seconds. 
(Provocation Based). Party Strength, Dex, Int, Increased by Up to 8.*/

namespace Server.Spells.SkillMasteries
{
    public class InvigorateSpell : BardSpell
    {
        public static readonly string StatModName = "Invigorate";

        private DateTime m_NextHeal;
        private readonly List<Mobile> m_Mods = new List<Mobile>();

        private static readonly SpellInfo m_Info = new SpellInfo(
                "Invigorate", "An Zu",
                -1,
                9002
            );

        public override double RequiredSkill => 90;
        public override double UpKeep => 5;
        public override int RequiredMana => 22;
        public override bool PartyEffects => true;
        public override SkillName CastSkill => SkillName.Provocation;

        private int m_HPBonus;
        private int m_StatBonus;

        public InvigorateSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
            m_NextHeal = DateTime.UtcNow + TimeSpan.FromSeconds(4);
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
                m_StatBonus = (int)(BaseSkillBonus + CollectiveBonus);
                m_HPBonus = (int)((2.5 * BaseSkillBonus) + CollectiveBonus);

                UpdateParty();
                BeginTimer();
            }

            FinishSequence();
        }

        public override void AddPartyEffects(Mobile m)
        {
            m.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
            m.SendLocalizedMessage(1115737); // You feel invigorated by the bard's spellsong.

            string args = string.Format("{0}\t{1}\t{2}\t{3}", m_HPBonus, m_StatBonus, m_StatBonus, m_StatBonus);
            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Invigorate, 1115613, 1115730, args.ToString()));

            m.AddStatMod(new StatMod(StatType.Str, StatModName + "str", m_StatBonus, TimeSpan.Zero));
            m.AddStatMod(new StatMod(StatType.Dex, StatModName + "dex", m_StatBonus, TimeSpan.Zero));
            m.AddStatMod(new StatMod(StatType.Int, StatModName + "int", m_StatBonus, TimeSpan.Zero));

            m_Mods.Add(m);
        }

        public override void RemovePartyEffects(Mobile m)
        {
            BuffInfo.RemoveBuff(m, BuffIcon.Invigorate);

            if (m_Mods.Contains(m))
            {
                m.RemoveStatMod(StatModName + "str");
                m.RemoveStatMod(StatModName + "dex");
                m.RemoveStatMod(StatModName + "int");

                m_Mods.Remove(m);
            }
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

        public override bool OnTick()
        {
            base.OnTick();

            if (m_NextHeal > DateTime.UtcNow)
                return false;

            PartyList.IterateReverse(m =>
            {
                if (CheckPartyEffects(m, true))
                {
                    int healRange = (int)((BaseSkillBonus * 2) + CollectiveBonus); // 4 - 16 (22)

                    if (m.Hits < m.HitsMax)
                    {
                        m.Heal(Utility.RandomMinMax(healRange - 2, healRange + 2));
                        m.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
                        m.PlaySound(0x1F2);
                    }
                }
                else
                {
                    RemovePartyMember(m);
                }
            });

            m_NextHeal = DateTime.UtcNow + TimeSpan.FromSeconds(4);
            return true;
        }

        /// <summary>
        /// Called in AOS.cs - HP Bonus
        /// </summary>
        /// <returns></returns>
        public override int StatBonus()
        {
            return m_HPBonus;
        }

        public static int GetHPBonus(Mobile m)
        {
            SkillMasterySpell spell = GetSpellForParty(m, typeof(InvigorateSpell));

            if (spell != null)
                return spell.StatBonus();

            return 0;
        }
    }
}
