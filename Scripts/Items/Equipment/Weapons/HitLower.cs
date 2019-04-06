using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class HitLower
    {
        public static readonly TimeSpan AttackEffectDuration = TimeSpan.FromSeconds(10.0);
        public static readonly TimeSpan DefenseEffectDuration = TimeSpan.FromSeconds(8.0);
        private static readonly Dictionary<Mobile, AttackTimer> m_AttackTable = new Dictionary<Mobile, AttackTimer>();
        private static readonly Dictionary<Mobile, DefenseTimer> m_DefenseTable = new Dictionary<Mobile, DefenseTimer>();

        public static bool IsUnderAttackEffect(Mobile m)
        {
            return m_AttackTable.ContainsKey(m);
        }

        public static bool ApplyAttack(Mobile m)
        {
            if (IsUnderAttackEffect(m))
                return false;

            m_AttackTable[m] = new AttackTimer(m);
            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.HitLowerAttack, 1151315, 1151314, AttackEffectDuration, m, "25"));
            m.SendLocalizedMessage(1062319); // Your attack chance has been reduced!

            m.Delta(MobileDelta.WeaponDamage);

            return true;
        }

        public static bool IsUnderDefenseEffect(Mobile m)
        {
            return m_DefenseTable.ContainsKey(m);
        }

        public static bool ApplyDefense(Mobile m)
        {
            if (!Core.HS)
            {
                if (IsUnderDefenseEffect(m))
                    return false;

                m_DefenseTable[m] = new DefenseTimer(m, 25);
                BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.HitLowerDefense, 1151313, 1151286, DefenseEffectDuration, m, "35"));
                m.SendLocalizedMessage(1062318); // Your defense chance has been reduced!

                m.Delta(MobileDelta.WeaponDamage);

                return true;
            }
            else
            {
                if (m_DefenseTable.ContainsKey(m))
                {
                    var timer = m_DefenseTable[m];

                    if (timer != null)
                    {
                        timer.Stop();
                        timer.DefenseMalus = 0;
                    }
                }

                int malus;

                if (m is PlayerMobile)
                {
                    malus = 45 + BaseArmor.GetRefinedDefenseChance(m);
                    malus = malus - (int)((double)malus * .35);
                }
                else
                {
                    malus = 25;
                }

                m_DefenseTable[m] = new DefenseTimer(m, malus);
                BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.HitLowerDefense, 1151313, 1151286, DefenseEffectDuration, m, malus.ToString()));
                m.SendLocalizedMessage(1062318); // Your defense chance has been reduced!

                m.Delta(MobileDelta.WeaponDamage);

                return true;
            }
        }

        private static void RemoveAttack(Mobile m)
        {
            if (m_AttackTable.ContainsKey(m))
            {
                m_AttackTable.Remove(m);
                m.SendLocalizedMessage(1062320); // Your attack chance has returned to normal.
            }
        }

        private static void RemoveDefense(Mobile m)
        {
            if (m_DefenseTable.ContainsKey(m))
            {
                m_DefenseTable.Remove(m);
                m.SendLocalizedMessage(1062321); // Your defense chance has returned to normal.

                m.Delta(MobileDelta.WeaponDamage);
            }
        }

        public static int GetDefenseMalus(Mobile m)
        {
            if (m_DefenseTable.ContainsKey(m))
            {
                return m_DefenseTable[m].DefenseMalus;
            }

            return 0;
        }

        private class AttackTimer : Timer
        {
            private readonly Mobile m_Player;

            public AttackTimer(Mobile player)
                : base(AttackEffectDuration)
            {
                m_Player = player;

                Priority = TimerPriority.TwoFiftyMS;

                Start();
            }

            protected override void OnTick()
            {
                RemoveAttack(m_Player);
            }
        }

        private class DefenseTimer : Timer
        {
            private readonly Mobile m_Player;
            public int DefenseMalus { get; set; }

            public DefenseTimer(Mobile player, int malus)
                : base(DefenseEffectDuration)
            {
                m_Player = player;
                DefenseMalus = malus;

                Priority = TimerPriority.TwoFiftyMS;
                Start();
            }

            protected override void OnTick()
            {
                RemoveDefense(m_Player);
            }
        }
    }
}
