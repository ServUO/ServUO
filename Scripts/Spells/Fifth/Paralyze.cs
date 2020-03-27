using System;
using Server.Mobiles;
using Server.Spells.Chivalry;
using Server.Targeting;

namespace Server.Spells.Fifth
{
    public class ParalyzeSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Paralyze", "An Ex Por",
            218,
            9012,
            Reagent.Garlic,
            Reagent.MandrakeRoot,
            Reagent.SpidersSilk);
        public ParalyzeSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Fifth;
            }
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
            else if (m.Frozen || m.Paralyzed || (m.Spell != null && m.Spell.IsCasting && !(m.Spell is PaladinSpell)))
            {
                this.Caster.SendLocalizedMessage(1061923); // The target is already frozen.
            }
            else if (this.CheckHSequence(m))
            {
                SpellHelper.Turn(this.Caster, m);

                SpellHelper.CheckReflect((int)this.Circle, this.Caster, ref m);

                double duration;
				
                int secs = (int)((this.GetDamageSkill(this.Caster) / 10) - (this.GetResistSkill(m) / 10));
					
                if (!m.Player)
                    secs *= 3;

                if (secs < 0)
                    secs = 0;

                duration = secs;

                if (m is PlagueBeastLord)
                {
                    ((PlagueBeastLord)m).OnParalyzed(this.Caster);
                    duration = 120;
                }

                m.Paralyze(TimeSpan.FromSeconds(duration));

                m.PlaySound(0x204);
                m.FixedEffect(0x376A, 6, 1);

                this.HarmfulSpell(m);
            }

            this.FinishSequence();
        }

        public class InternalTarget : Target
        {
            private readonly ParalyzeSpell m_Owner;
            public InternalTarget(ParalyzeSpell owner)
                : base(10, false, TargetFlags.Harmful)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                    this.m_Owner.Target((Mobile)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}
