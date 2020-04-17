using Server.Targeting;
using System;

namespace Server.Spells.Second
{
    public class StrengthSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Strength", "Uus Mani",
            212,
            9061,
            Reagent.MandrakeRoot,
            Reagent.Nightshade);
        public StrengthSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle => SpellCircle.Second;

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile m)
        {
            if (!Caster.CanSee(m))
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (CheckBSequence(m))
            {
                SpellHelper.Turn(Caster, m);

                int oldStr = SpellHelper.GetBuffOffset(m, StatType.Str);
                int newStr = SpellHelper.GetOffset(Caster, m, StatType.Str, false, true);

                if (newStr < oldStr || newStr == 0)
                {
                    DoHurtFizzle();
                }
                else
                {
                    SpellHelper.AddStatBonus(Caster, m, false, StatType.Str);
                    int percentage = (int)(SpellHelper.GetOffsetScalar(Caster, m, false) * 100);
                    TimeSpan length = SpellHelper.GetDuration(Caster, m);
                    BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Strength, 1075845, length, m, percentage.ToString()));

                    m.FixedParticles(0x375A, 10, 15, 5017, EffectLayer.Waist);
                    m.PlaySound(0x1EE);
                }
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly StrengthSpell m_Owner;
            public InternalTarget(StrengthSpell owner)
                : base(10, false, TargetFlags.Beneficial)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                {
                    m_Owner.Target((Mobile)o);
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
