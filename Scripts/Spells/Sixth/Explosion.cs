using System;
using Server.Targeting;

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

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Sixth;
            }
        }
        public override bool DelayedDamageStacking
        {
            get
            {
                return !Core.AOS;
            }
        }
        public override bool DelayedDamage
        {
            get
            {
                return false;
            }
        }
        public override void OnCast()
        {
            this.Caster.Target = new InternalTarget(this);
        }

        public void Target(IDamageable m)
        {
            Mobile defender = m as Mobile;

            if (!this.Caster.CanSee(m))
            {
                this.Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (this.Caster.CanBeHarmful(m) && this.CheckSequence())
            {
                Mobile attacker = this.Caster;

                SpellHelper.Turn(this.Caster, m);

                if(defender != null)
                    SpellHelper.CheckReflect((int)this.Circle, this.Caster, ref defender);

                InternalTimer t = new InternalTimer(this, attacker, defender != null ? defender : m);
                t.Start();
            }

            this.FinishSequence();
        }

        private class InternalTimer : Timer
        {
            private readonly MagerySpell m_Spell;
            private readonly IDamageable m_Target;
            private readonly Mobile m_Attacker;

            public InternalTimer(MagerySpell spell, Mobile attacker, IDamageable target)
                : base(TimeSpan.FromSeconds(Core.AOS ? 3.0 : 2.5))
            {
                m_Spell = spell;
                m_Attacker = attacker;
                m_Target = target;

                if (this.m_Spell != null)
                    this.m_Spell.StartDelayedDamageContext(attacker, this);

                this.Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                Mobile defender = m_Target as Mobile;

                if (m_Attacker.HarmfulCheck(m_Target))
                {
                    double damage = 0;

                    if (Core.AOS)
                    {
                        damage = this.m_Spell.GetNewAosDamage(40, 1, 5, m_Target);
                    }
                    else if (defender != null)
                    {
                        damage = Utility.Random(23, 22);

                        if (this.m_Spell.CheckResisted(defender))
                        {
                            damage *= 0.75;

                            defender.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                        }

                        damage *= this.m_Spell.GetDamageScalar(defender);
                    }

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
                        SpellHelper.Damage(this.m_Spell, this.m_Target, damage, 0, 100, 0, 0, 0);
                    }

                    if (this.m_Spell != null)
                        this.m_Spell.RemoveDelayedDamageContext(this.m_Attacker);
                }
            }
        }

        private class InternalTarget : Target
        {
            private readonly ExplosionSpell m_Owner;
            public InternalTarget(ExplosionSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.Harmful)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IDamageable)
                    this.m_Owner.Target((IDamageable)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}