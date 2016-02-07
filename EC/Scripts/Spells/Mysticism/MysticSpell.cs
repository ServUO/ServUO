using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Targeting;

namespace Server.Spells.Mystic
{
    public abstract class MysticSpell : Spell
    {
        public MysticSpell(Mobile caster, Item scroll, SpellInfo info)
            : base(caster, scroll, info)
        {
        }

        public abstract SpellCircle Circle { get; }

        private static int[] m_ManaTable = new int[] { 4, 6, 9, 11, 14, 20, 40, 50 };

        public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds(1.0); } }
        public override double CastDelayFastScalar { get { return 1.0; } }

        private const double ChanceOffset = 20.0, ChanceLength = 100.0 / 7.0;

        public override void GetCastSkills(out double min, out double max)
        {
            int circle = (int)Circle;

            if (Scroll != null)
                circle -= 2;

            double avg = ChanceLength * circle;

            min = avg - ChanceOffset;
            max = avg + ChanceOffset;
        }

        public override bool CheckFizzle()
        {
            double minSkill, maxSkill;
            GetCastSkills(out minSkill, out maxSkill);
            return Caster.CheckSkill(CastSkill, minSkill, maxSkill);
        }

        public override SkillName CastSkill { get { return SkillName.Mysticism; } }

        public override SkillName DamageSkill
        {
            get
            {
                if (Caster.Skills[SkillName.Imbuing].Value >= Caster.Skills[SkillName.Focus].Value)
                    return SkillName.Imbuing;
                return SkillName.Focus;
            }
        }

        public override int GetMana()
        {
            return m_ManaTable[(int)Circle];
        }

        public override TimeSpan GetCastRecovery()
        {
            if (Scroll is SpellStone)
                return TimeSpan.Zero;

            return TimeSpan.FromSeconds(0.75);
        }

        public override TimeSpan GetCastDelay()
        {
            if (Scroll is SpellStone)
                return TimeSpan.Zero;

            return TimeSpan.FromSeconds(0.5 + (0.25 * (int)Circle));
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            return true;
        }

        public virtual bool CheckResisted(Mobile target)
        {
            double n = GetResistPercent(target);

            n /= 100.0;

            if (n <= 0.0)
                return false;

            if (n >= 1.0)
                return true;

            int maxSkill = (1 + (int)Circle) * 10;
            maxSkill += (1 + ((int)Circle / 6)) * 25;

            if (target.Skills[SkillName.MagicResist].Value < maxSkill)
                target.CheckSkill(SkillName.MagicResist, 0.0, target.Skills[SkillName.MagicResist].Cap);

            return (n >= Utility.RandomDouble());
        }

        public virtual double GetResistPercentForCircle(Mobile target, SpellCircle circle)
        {
            double firstPercent = target.Skills[SkillName.MagicResist].Value / 5.0;
            double secondPercent = target.Skills[SkillName.MagicResist].Value - (((Caster.Skills[CastSkill].Value - 20.0) / 5.0) + (1 + (int)circle) * 5.0);

            return (firstPercent > secondPercent ? firstPercent : secondPercent) / 2.0; // Seems should be about half of what stratics says.
        }

        public virtual double GetResistPercent(Mobile target)
        {
            return GetResistPercentForCircle(target, Circle);
        }

        public virtual void OnTarget(Object o)
        {
        }

        // Ever wondered why in the hell RunUO coded a new target class for every spell?
        public class MysticSpellTarget : Target
        {
            private MysticSpell m_Owner;

            public MysticSpell Owner
            {
                get { return m_Owner; }
                set { m_Owner = value; }
            }

            public MysticSpellTarget(MysticSpell owner, TargetFlags flags)
                : this(owner, false, flags)
            {
            }

            public MysticSpellTarget(MysticSpell owner, bool allowland, TargetFlags flags)
                : base(12, allowland, flags)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o == null)
                    return;

                if (!from.CanSee(o))
                    from.SendLocalizedMessage(500237); // Target can not be seen.
                else
                {
                    SpellHelper.Turn(from, o);
                    m_Owner.OnTarget(o);
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}