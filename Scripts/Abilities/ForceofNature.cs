using System;
using Server;
using System.Collections.Generic;

namespace Server.Items
{
    public class ForceOfNature : WeaponAbility
    {
        public ForceOfNature()
        {
        }

        public override int BaseMana { get { return 35; } }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!Validate(attacker) || !CheckMana(attacker, true))
                return;

            ClearCurrentAbility(attacker);

            attacker.SendLocalizedMessage(1074374); // You attack your enemy with the force of nature!
            defender.SendLocalizedMessage(1074375); // You are assaulted with great force!

            defender.PlaySound(0x22F);
            defender.FixedParticles(0x36CB, 1, 9, 9911, 67, 5, EffectLayer.Head);
            defender.FixedParticles(0x374A, 1, 17, 9502, 1108, 4, (EffectLayer)255);

            if (m_Table.ContainsKey(attacker))
                Remove(attacker);

            ForceOfNatureTimer t = new ForceOfNatureTimer(attacker, defender);
            t.Start();

            m_Table[attacker] = t;
        }

        private static Dictionary<Mobile, ForceOfNatureTimer> m_Table = new Dictionary<Mobile, ForceOfNatureTimer>();

        public static bool Remove(Mobile m)
        {
            ForceOfNatureTimer t;

            m_Table.TryGetValue(m, out t);

            if (t == null)
                return false;

            t.Stop();
            m_Table.Remove(m);
            return true;
        }

        public static void OnHit(Mobile from, Mobile target)
        {
            if (m_Table.ContainsKey(from))
            {
                ForceOfNatureTimer t = m_Table[from];

                t.Hits++;
                t.LastHit = DateTime.Now;

                if (t.Hits % 12 == 0)
                {
                    int duration = target.Skills[SkillName.MagicResist].Value >= 90.0 ? 1 : 2;
                    target.Paralyze(TimeSpan.FromSeconds(duration));
                    t.Hits = 0;

                    from.SendLocalizedMessage(1004013); // You successfully stun your opponent!
                    target.SendLocalizedMessage(1004014); // You have been stunned!
                }
            }
        }

        public static int GetBonus(Mobile from, Mobile target)
        {
            if (m_Table.ContainsKey(from))
            {
                ForceOfNatureTimer t = m_Table[from];

                if (t.Target == target)
                {
                    int bonus = Math.Max(50, from.Str - 50);
                    if (bonus > 100) bonus = 100;
                    return bonus;
                }
            }

            return 0;
        }

        private class ForceOfNatureTimer : Timer
        {
            private Mobile m_Target, m_From;

            private DateTime m_LastHit;
            private int m_Tick, m_Hits;

            public Mobile Target { get { return m_Target; } }
            public Mobile From { get { return m_From; } }
            public int Hits { get { return m_Hits; } set { m_Hits = value; } }
            public DateTime LastHit { get { return m_LastHit; } set { m_LastHit = value; } }

            public ForceOfNatureTimer(Mobile from, Mobile target)
                : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
                m_Target = target;
                m_From = from;
                m_Tick = 0;
                m_Hits = 1;
                m_LastHit = DateTime.Now;
            }

            protected override void OnTick()
            {
                m_Tick++;

                if (!m_From.Alive || !m_Target.Alive || m_Target.Map != m_From.Map || m_Target.GetDistanceToSqrt(m_From.Location) > 10 || m_LastHit + TimeSpan.FromSeconds(20) < DateTime.Now || m_Tick > 36)
                {
                    Server.Items.ForceOfNature.Remove(m_From);
                    return;
                }

                if (m_Tick == 1)
                {
                    int damage = Utility.RandomMinMax(15, 35);

                    AOS.Damage(m_Target, m_From, damage, false, 0, 0, 0, 0, 0, 0, 100, false, false, false);
                }
            }
        }
    }
}
