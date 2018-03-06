using System;
using System.Collections.Generic;

namespace Server.Spells.Bushido
{
    public class Confidence : SamuraiSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Confidence", null,
            -1,
            9002);

        private static Dictionary<Mobile, Timer> m_Table = new Dictionary<Mobile, Timer>();
        private static Dictionary<Mobile, Timer> m_RegenTable = new Dictionary<Mobile, Timer>();

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
            return m_Table.ContainsKey(m);
        }

        public static void BeginConfidence(Mobile m)
        {
            Timer t;

            if (m_Table.TryGetValue(m, out t))
                t.Stop();

            t = new InternalTimer(m);

            m_Table[m] = t;

            t.Start();

            double bushido = m.Skills[SkillName.Bushido].Value;
            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Confidence, 1060596, 1153809, TimeSpan.FromSeconds(4), m, String.Format("{0}\t{1}\t{2}", ((int)(bushido / 12)).ToString(), ((int)(bushido / 5)).ToString(), "100"))); // Successful parry will heal for 1-~1_HEAL~ hit points and refresh for 1-~2_STAM~ stamina points.<br>+~3_HP~ hit point regeneration (4 second duration).

            int anticipateHitBonus = SkillMasteries.MasteryInfo.AnticipateHitBonus(m);

            if (anticipateHitBonus > 0)
                BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.AnticipateHit, 1155905, 1156057, TimeSpan.FromSeconds(4), m, String.Format("{0}\t{1}", anticipateHitBonus.ToString(), "75"))); // ~1_CHANCE~% chance to reduce Confidence heal by ~2_REDUCE~% when hit. 
        }

        public static void EndConfidence(Mobile m)
        {
            if (!m_Table.ContainsKey(m))
                return;

            Timer t = m_Table[m];

            t.Stop();
            m_Table.Remove(m);

            OnEffectEnd(m, typeof(Confidence));

            BuffInfo.RemoveBuff(m, BuffIcon.Confidence);
            BuffInfo.RemoveBuff(m, BuffIcon.AnticipateHit);
        }

        public static bool IsRegenerating(Mobile m)
        {
            return m_RegenTable.ContainsKey(m);
        }

        public static void BeginRegenerating(Mobile m)
        {
            Timer t;

            if (m_RegenTable.TryGetValue(m, out t))
                t.Stop();

            t = new RegenTimer(m);

            m_RegenTable[m] = t;

            t.Start();
        }

        public static void StopRegenerating(Mobile m)
        {
            Timer t;
            int anticipateHitBonus = SkillMasteries.MasteryInfo.AnticipateHitBonus(m);

            if (anticipateHitBonus >= Utility.Random(100) && m_RegenTable.TryGetValue(m, out t))
            {
                if (t is RegenTimer)
                    ((RegenTimer)t).Hits /= 2;

                return;
            }

            if (m_RegenTable.TryGetValue(m, out t))
                t.Stop();

            if (m_RegenTable.ContainsKey(m))
                m_RegenTable.Remove(m);

            BuffInfo.RemoveBuff(m, BuffIcon.AnticipateHit);
        }

        public override void OnBeginCast()
        {
            base.OnBeginCast();

            Caster.FixedEffect(0x37C4, 10, 7, 4, 3);
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                Caster.SendLocalizedMessage(1063115); // You exude confidence.

                Caster.FixedParticles(0x375A, 1, 17, 0x7DA, 0x960, 0x3, EffectLayer.Waist);
                Caster.PlaySound(0x51A);

                OnCastSuccessful(Caster);

                BeginConfidence(Caster);
                BeginRegenerating(Caster);
            }

            FinishSequence();
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Mobile;
            public InternalTimer(Mobile m)
                : base(TimeSpan.FromSeconds(15.0))
            {
                m_Mobile = m;
                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                EndConfidence(m_Mobile);
                m_Mobile.SendLocalizedMessage(1063116); // Your confidence wanes.
            }
        }

        private class RegenTimer : Timer
        {
            private Mobile m_Mobile;
            private int m_Ticks;
            private int m_Hits;

            public int Hits { get { return m_Hits; } set { m_Hits = value; } }

            public RegenTimer(Mobile m)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
            {
                m_Mobile = m;
                m_Hits = 15 + (m.Skills.Bushido.Fixed * m.Skills.Bushido.Fixed / 57600);
                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                ++m_Ticks;

                if (m_Ticks >= 5)
                {
                    m_Mobile.Hits += (m_Hits - (m_Hits * 4 / 5));
                    StopRegenerating(m_Mobile);
                }

                m_Mobile.Hits += (m_Hits / 5);
            }
        }
    }
}