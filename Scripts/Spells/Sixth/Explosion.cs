using Server.Targeting;
using System;

namespace Server.Spells.Sixth
{
    public class ExplosionSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Explosion", "Vas Ort Flam",
            230,
            9041,
            Reagent.Bloodmoss,
            Reagent.MandrakeRoot);
        public ExplosionSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle => SpellCircle.Sixth;
        public override bool DelayedDamageStacking => false;
        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(IDamageable m)
        {
            if (HasDelayContext(m))
            {
                DoHurtFizzle();
                return;
            }

            if (!Caster.CanSee(m))
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (Caster.CanBeHarmful(m) && CheckSequence())
            {
                Mobile attacker = Caster;

                SpellHelper.Turn(Caster, m);

                SpellHelper.CheckReflect(this, Caster, ref m);

                InternalTimer t = new InternalTimer(this, attacker, m);
                t.Start();
            }

            FinishSequence();
        }

        private class InternalTimer : Timer
        {
            private readonly MagerySpell m_Spell;
            private readonly IDamageable m_Target;
            private readonly Mobile m_Attacker;

            public InternalTimer(MagerySpell spell, Mobile attacker, IDamageable target)
                : base(TimeSpan.FromSeconds(3.0))
            {
                m_Spell = spell;
                m_Attacker = attacker;
                m_Target = target;

                if (m_Spell != null)
                    m_Spell.StartDelayedDamageContext(attacker, this);

                Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                Mobile defender = m_Target as Mobile;

                if (m_Attacker.HarmfulCheck(m_Target))
                {
                    double damage = m_Spell.GetNewAosDamage(40, 1, 5, m_Target);

                    if (defender != null)
                    {
                        defender.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
                        defender.PlaySound(0x307);
                    }
                    else
                    {
                        Effects.SendLocationParticles(m_Target, 0x36BD, 20, 10, 5044);
                        Effects.PlaySound(m_Target.Location, m_Target.Map, 0x307);
                    }

                    if (damage > 0)
                    {
                        SpellHelper.Damage(m_Spell, m_Target, damage, 0, 100, 0, 0, 0);
                    }

                    if (m_Spell != null)
                        m_Spell.RemoveDelayedDamageContext(m_Attacker);
                }
            }
        }

        private class InternalTarget : Target
        {
            private readonly ExplosionSpell m_Owner;
            public InternalTarget(ExplosionSpell owner)
                : base(10, false, TargetFlags.Harmful)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IDamageable)
                    m_Owner.Target((IDamageable)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
