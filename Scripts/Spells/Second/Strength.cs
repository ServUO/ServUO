using System;
using Server.Targeting;

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

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Second;
            }
        }
        public override bool CheckCast()
        {
            if (Engines.ConPVP.DuelContext.CheckSuddenDeath(this.Caster))
            {
                this.Caster.SendMessage(0x22, "You cannot cast this spell when in sudden death.");
                return false;
            }

            return base.CheckCast();
        }

        public override void OnCast()
        {
            this.Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile m)
        {
            if (!this.Caster.CanSee(m))
            {
                this.Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (this.CheckBSequence(m))
            {
                SpellHelper.Turn(this.Caster, m);

                SpellHelper.AddStatBonus(this.Caster, m, StatType.Str);

                m.FixedParticles(0x375A, 10, 15, 5017, EffectLayer.Waist);
                m.PlaySound(0x1EE);

                int percentage = (int)(SpellHelper.GetOffsetScalar(this.Caster, m, false) * 100);
                TimeSpan length = SpellHelper.GetDuration(this.Caster, m);

                BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Strength, 1075845, length, m, percentage.ToString()));
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly StrengthSpell m_Owner;
            public InternalTarget(StrengthSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.Beneficial)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                {
                    this.m_Owner.Target((Mobile)o);
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}