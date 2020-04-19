using System;
using System.Collections.Generic;
using Server.Network;

namespace Server.Items
{
    /// <summary>
    /// Currently on EA, this is only available for Creatures
    /// </summary>
    public class ColdWind : WeaponAbility
    {
        private static readonly Dictionary<Mobile, ExpireTimer> m_Table = new Dictionary<Mobile, ExpireTimer>();

        public override int BaseMana => 20;

        public override double DamageScalar => 1.5;

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!Validate(attacker) || !CheckMana(attacker, true))
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
                {
                    AOS.Damage(m_Mobile, m_From, 14, 0, 0, 100, 0, 0);
                    Effects.SendPacket(m_Mobile.Location, m_Mobile.Map, new ParticleEffect(EffectType.FixedFrom, m_Mobile.Serial, Serial.Zero, 0x374A, m_Mobile.Location, m_Mobile.Location, 1, 15, false, false, 97, 0, 4, 9502, 1, m_Mobile.Serial, 163, 0));
                }
                else
                {
                    DoExpire();
                }
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
