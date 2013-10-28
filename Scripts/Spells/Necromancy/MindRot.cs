using System;
using System.Collections;
using Server.Targeting;

namespace Server.Spells.Necromancy
{
    public class MindRotSpell : NecromancerSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Mind Rot", "Wis An Ben",
            203,
            9031,
            Reagent.BatWing,
            Reagent.PigIron,
            Reagent.DaemonBlood);
        private static readonly Hashtable m_Table = new Hashtable();
        public MindRotSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(1.5);
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 30.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 17;
            }
        }
        public static void ClearMindRotScalar(Mobile m)
        {
            if (!m_Table.ContainsKey(m))
                return;

            BuffInfo.RemoveBuff(m, BuffIcon.Mindrot);
            MRBucket tmpB = (MRBucket)m_Table[m];
            MRExpireTimer tmpT = (MRExpireTimer)tmpB.m_MRExpireTimer;
            tmpT.Stop();
            m_Table.Remove(m);
            m.SendLocalizedMessage(1060872); // Your mind feels normal again.
        }

        public static bool HasMindRotScalar(Mobile m)
        {
            return m_Table.ContainsKey(m);
        }

        public static bool GetMindRotScalar(Mobile m, ref double scalar)
        {
            if (!m_Table.ContainsKey(m))
                return false;

            MRBucket tmpB = (MRBucket)m_Table[m];
            scalar = tmpB.m_Scalar;
            return true;
        }

        public static void SetMindRotScalar(Mobile caster, Mobile target, double scalar, TimeSpan duration)
        {
            if (!m_Table.ContainsKey(target))
            {
                m_Table.Add(target, new MRBucket(scalar, new MRExpireTimer(caster, target, duration)));
                BuffInfo.AddBuff(target, new BuffInfo(BuffIcon.Mindrot, 1075665, duration, target));
                MRBucket tmpB = (MRBucket)m_Table[target];
                MRExpireTimer tmpT = (MRExpireTimer)tmpB.m_MRExpireTimer;
                tmpT.Start();
                target.SendLocalizedMessage(1074384);
            }
        }

        public override void OnCast()
        {
            this.Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile m)
        {
            if (HasMindRotScalar(m))
            {
                this.Caster.SendLocalizedMessage(1005559); // This spell is already in effect.
            }
            else if (this.CheckHSequence(m))
            {
                SpellHelper.Turn(this.Caster, m);

                /* Attempts to place a curse on the Target that increases the mana cost of any spells they cast,
                * for a duration based off a comparison between the Caster's Spirit Speak skill and the Target's Resisting Spells skill.
                * The effect lasts for ((Spirit Speak skill level - target's Resist Magic skill level) / 50 ) + 20 seconds.
                */

                if (m.Spell != null)
                    m.Spell.OnCasterHurt();
				
                m.PlaySound(0x1FB);
                m.PlaySound(0x258);
                m.FixedParticles(0x373A, 1, 17, 9903, 15, 4, EffectLayer.Head);

                TimeSpan duration = TimeSpan.FromSeconds((((this.GetDamageSkill(this.Caster) - this.GetResistSkill(m)) / 5.0) + 20.0) * (m.Player ? 1.0 : 2.0));
                m.CheckSkill(SkillName.MagicResist, 0.0, 120.0);	//Skill check for gain

                if (m.Player)
                    SetMindRotScalar(this.Caster, m, 1.25, duration);
                else
                    SetMindRotScalar(this.Caster, m, 2.00, duration);

                this.HarmfulSpell(m);
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly MindRotSpell m_Owner;
            public InternalTarget(MindRotSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.Harmful)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                    this.m_Owner.Target((Mobile)o);
                else
                    from.SendLocalizedMessage(1060508); // You can't curse that.
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }

    public class MRExpireTimer : Timer
    {
        private readonly Mobile m_Caster;
        private readonly Mobile m_Target;
        private DateTime m_End;
        public MRExpireTimer(Mobile caster, Mobile target, TimeSpan delay)
            : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
        {
            this.m_Caster = caster;
            this.m_Target = target;
            this.m_End = DateTime.UtcNow + delay;
            this.Priority = TimerPriority.TwoFiftyMS;
        }

        public void RenewDelay(TimeSpan delay)
        {
            this.m_End = DateTime.UtcNow + delay;
        }

        public void Halt()
        {
            this.Stop();
        }

        protected override void OnTick()
        {
            if (this.m_Target.Deleted || !this.m_Target.Alive || DateTime.UtcNow >= this.m_End)
            {
                MindRotSpell.ClearMindRotScalar(this.m_Target);
                this.Stop();
            }
        }
    }

    public class MRBucket
    {
        public double m_Scalar;
        public MRExpireTimer m_MRExpireTimer;
        public MRBucket(double theScalar, MRExpireTimer theTimer)
        {
            this.m_Scalar = theScalar;
            this.m_MRExpireTimer = theTimer;
        }
    }
}