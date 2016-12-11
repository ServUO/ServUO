using System;
using System.Collections;

namespace Server.Spells.Bushido
{
    public class Confidence : SamuraiSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Confidence", null,
            -1,
            9002);
        private static readonly Hashtable m_Table = new Hashtable();
        private static readonly Hashtable m_RegenTable = new Hashtable();
        public Confidence(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(0.25);
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 25.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 10;
            }
        }
        public static bool IsConfident(Mobile m)
        {
            return m_Table.Contains(m);
        }

        public static void BeginConfidence(Mobile m)
        {
            Timer t = (Timer)m_Table[m];

            if (t != null)
                t.Stop();

            t = new InternalTimer(m);

            m_Table[m] = t;

            double bushido = m.Skills.Bushido.Value;
            string args = String.Format("12\t5\t{0}", (int)(15 + (bushido * bushido / 576)) * 2.5);
            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Confidence, 1060596, 1153809, TimeSpan.FromSeconds(15), m, args));

            t.Start();
        }

        public static void EndConfidence(Mobile m)
        {
            Timer t = (Timer)m_Table[m];

            if (t != null)
                t.Stop();

            m_Table.Remove(m);
            BuffInfo.RemoveBuff(m, BuffIcon.Confidence);

            OnEffectEnd(m, typeof(Confidence));
        }

        public static bool IsRegenerating(Mobile m)
        {
            return m_RegenTable.Contains(m);
        }

        public static void BeginRegenerating(Mobile m)
        {
            Timer t = (Timer)m_RegenTable[m];

            if (t != null)
                t.Stop();

            t = new RegenTimer(m);

            m_RegenTable[m] = t;

            t.Start();
        }

        public static void StopRegenerating(Mobile m)
        {
            Timer t = (Timer)m_RegenTable[m];

            if (t != null)
                t.Stop();

            m_RegenTable.Remove(m);
        }

        public override void OnBeginCast()
        {
            base.OnBeginCast();

            this.Caster.FixedEffect(0x37C4, 10, 7, 4, 3);
        }

        public override void OnCast()
        {
            if (this.CheckSequence())
            {
                this.Caster.SendLocalizedMessage(1063115); // You exude confidence.

                this.Caster.FixedParticles(0x375A, 1, 17, 0x7DA, 0x960, 0x3, EffectLayer.Waist);
                this.Caster.PlaySound(0x51A);

                this.OnCastSuccessful(this.Caster);

                BeginConfidence(this.Caster);
                BeginRegenerating(this.Caster);
            }

            this.FinishSequence();
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Mobile;
            public InternalTimer(Mobile m)
                : base(TimeSpan.FromSeconds(15.0))
            {
                this.m_Mobile = m;
                this.Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                EndConfidence(this.m_Mobile);
                this.m_Mobile.SendLocalizedMessage(1063116); // Your confidence wanes.
            }
        }

        private class RegenTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly int m_Hits;
            private int m_Ticks;
            public RegenTimer(Mobile m)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
            {
                this.m_Mobile = m;
                this.m_Hits = 15 + (m.Skills.Bushido.Fixed * m.Skills.Bushido.Fixed / 57600);
                this.Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                ++this.m_Ticks;

                if (this.m_Ticks >= 5)
                {
                    this.m_Mobile.Hits += (this.m_Hits - (this.m_Hits * 4 / 5));
                    StopRegenerating(this.m_Mobile);
                }

                this.m_Mobile.Hits += (this.m_Hits / 5);
            }
        }
    }
}