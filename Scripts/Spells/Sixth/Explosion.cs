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

        public void Target(Mobile m)
        {
            if (!this.Caster.CanSee(m))
            {
                this.Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (this.Caster.CanBeHarmful(m) && this.CheckSequence())
            {
                Mobile attacker = this.Caster, defender = m;

                SpellHelper.Turn(this.Caster, m);

                SpellHelper.CheckReflect((int)this.Circle, this.Caster, ref m);

                InternalTimer t = new InternalTimer(this, attacker, defender, m);
                t.Start();
            }

            this.FinishSequence();
        }

        private class InternalTimer : Timer
        {
            private readonly MagerySpell m_Spell;
            private readonly Mobile m_Target;
            private readonly Mobile m_Attacker;
            private readonly Mobile m_Defender;
            public InternalTimer(MagerySpell spell, Mobile attacker, Mobile defender, Mobile target)
                : base(TimeSpan.FromSeconds(Core.AOS ? 3.0 : 2.5))
            {
                this.m_Spell = spell;
                this.m_Attacker = attacker;
                this.m_Defender = defender;
                this.m_Target = target;

                if (this.m_Spell != null)
                    this.m_Spell.StartDelayedDamageContext(attacker, this);

                this.Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                if (this.m_Attacker.HarmfulCheck(this.m_Defender))
                {
                    double damage;

                    if (Core.AOS)
                    {
                        damage = this.m_Spell.GetNewAosDamage(40, 1, 5, this.m_Defender);
                    }
                    else
                    {
                        damage = Utility.Random(23, 22);

                        if (this.m_Spell.CheckResisted(this.m_Target))
                        {
                            damage *= 0.75;

                            this.m_Target.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                        }

                        damage *= this.m_Spell.GetDamageScalar(this.m_Target);
                    }

                    this.m_Target.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
                    this.m_Target.PlaySound(0x307);

                    SpellHelper.Damage(this.m_Spell, this.m_Target, damage, 0, 100, 0, 0, 0);

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