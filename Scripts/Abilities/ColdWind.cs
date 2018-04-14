using System;
using System.Collections.Generic;

namespace Server.Items
{
    /// <summary>
    /// Currently on EA, this is only available for Creatures
    /// </summary>
    public class ColdWind : WeaponAbility
    {
        private static readonly Dictionary<Mobile, ExpireTimer> m_Table = new Dictionary<Mobile, ExpireTimer>();

        public ColdWind()
        {
        }

        public override int BaseMana
        {
            get
            {
                return 20;
            }
        }
        public override double DamageScalar
        {
            get
            {
                return 1.5;
            }
        }
        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!this.Validate(attacker) || !this.CheckMana(attacker, true))
                return;

            if (attacker.Map == null || attacker.Map == Map.Internal)
                return;

            ExpireTimer timer = null;

            if (m_Table.ContainsKey(defender))
                timer = m_Table[defender];

            if (timer != null)
            {
                timer.DoExpire();
                defender.SendLocalizedMessage(1070831); // The freezing wind continues to blow!
            }
            else
                defender.SendLocalizedMessage(1070832); // An icy wind surrounds you, freezing your lungs as you breathe!

            timer = new ExpireTimer(defender, attacker);
            timer.Start();
            m_Table[defender] = timer;
        }

        private class ExpireTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly Mobile m_From;
            private int m_Count;

            public ExpireTimer(Mobile m, Mobile from)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
            {
                m_Mobile = m;
                m_From = from;
                Priority = TimerPriority.TwoFiftyMS;
            }

            public void DoExpire()
            {
                Stop();
                m_Table.Remove(m_Mobile);
            }

            public void DrainLife()
            {
                if (m_Mobile.Alive)
                    m_Mobile.Damage(2, m_From);
                else
                    DoExpire();
            }

            protected override void OnTick()
            {
                DrainLife();

                if (++m_Count >= 5)
                {
                    DoExpire();
                    m_Mobile.SendLocalizedMessage(1070830); // The icy wind dissipates.
                }
            }
        }
    }
}