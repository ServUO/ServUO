using System;
using Server.Targeting;

namespace Server.Spells.Third
{
    public class BlessSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Bless", "Rel Sanct",
            203,
            9061,
            Reagent.Garlic,
            Reagent.MandrakeRoot);
        public BlessSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Third;
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
                SpellHelper.DisableSkillCheck = true;
                SpellHelper.AddStatBonus(this.Caster, m, StatType.Dex);
                SpellHelper.AddStatBonus(this.Caster, m, StatType.Int);
                SpellHelper.DisableSkillCheck = false;

                m.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
                m.PlaySound(0x1EA);

                int percentage = (int)(SpellHelper.GetOffsetScalar(this.Caster, m, false) * 100);
                TimeSpan length = SpellHelper.GetDuration(this.Caster, m);

                string args = String.Format("{0}\t{1}\t{2}", percentage, percentage, percentage);

                BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Bless, 1075847, 1075848, length, m, args.ToString()));
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly BlessSpell m_Owner;
            public InternalTarget(BlessSpell owner)
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