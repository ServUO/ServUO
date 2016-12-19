using System;
using System.Collections;

namespace Server.Items
{
    public class HitLower
    {
        public static readonly TimeSpan AttackEffectDuration = TimeSpan.FromSeconds(10.0);
        public static readonly TimeSpan DefenseEffectDuration = TimeSpan.FromSeconds(8.0);
        private static readonly Hashtable m_AttackTable = new Hashtable();
        private static readonly Hashtable m_DefenseTable = new Hashtable();
        public static bool IsUnderAttackEffect(Mobile m)
        {
            return m_AttackTable.Contains(m);
        }

        public static bool ApplyAttack(Mobile m)
        {
            if (IsUnderAttackEffect(m))
                return false;

            m_AttackTable[m] = new AttackTimer(m);
            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.HitLowerAttack, 1151315, 1151314, AttackEffectDuration, m, "25"));
            m.SendLocalizedMessage(1062319); // Your attack chance has been reduced!
            return true;
        }

        public static bool IsUnderDefenseEffect(Mobile m)
        {
            return m_DefenseTable.Contains(m);
        }

        public static bool ApplyDefense(Mobile m)
        {
            if (IsUnderDefenseEffect(m))
                return false;

            m_DefenseTable[m] = new DefenseTimer(m);
            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.HitLowerDefense, 1151313, 1151312, DefenseEffectDuration, m, "25"));
            m.SendLocalizedMessage(1062318); // Your defense chance has been reduced!
            return true;
        }

        private static void RemoveAttack(Mobile m)
        {
            m_AttackTable.Remove(m);
            m.SendLocalizedMessage(1062320); // Your attack chance has returned to normal.
        }

        private static void RemoveDefense(Mobile m)
        {
            m_DefenseTable.Remove(m);
            m.SendLocalizedMessage(1062321); // Your defense chance has returned to normal.
        }

        private class AttackTimer : Timer
        {
            private readonly Mobile m_Player;
            public AttackTimer(Mobile player)
                : base(AttackEffectDuration)
            {
                this.m_Player = player;

                this.Priority = TimerPriority.TwoFiftyMS;

                this.Start();
            }

            protected override void OnTick()
            {
                RemoveAttack(this.m_Player);
            }
        }

        private class DefenseTimer : Timer
        {
            private readonly Mobile m_Player;
            public DefenseTimer(Mobile player)
                : base(DefenseEffectDuration)
            {
                this.m_Player = player;

                this.Priority = TimerPriority.TwoFiftyMS;

                this.Start();
            }

            protected override void OnTick()
            {
                RemoveDefense(this.m_Player);
            }
        }
    }
}
