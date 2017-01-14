using System;
using System.Collections;

namespace Server.Items
{
    /// <summary>
    /// The assassin's friend.
    /// A successful Mortal Strike will render its victim unable to heal any damage for several seconds. 
    /// Use a gruesome follow-up to finish off your foe.
    /// </summary>
    public class MortalStrike : WeaponAbility
    {
        public static readonly TimeSpan PlayerDuration = TimeSpan.FromSeconds(6.0);
        public static readonly TimeSpan NPCDuration = TimeSpan.FromSeconds(12.0);
        private static readonly Hashtable m_Table = new Hashtable();
        public MortalStrike()
        {
        }

        public override int BaseMana
        {
            get
            {
                return 30;
            }
        }
        public static bool IsWounded(Mobile m)
        {
            return m_Table.Contains(m);
        }

        public static void BeginWound(Mobile m, TimeSpan duration)
        {
            Timer t = (Timer)m_Table[m];

            if (t != null)
                t.Stop();

            t = new InternalTimer(m, duration);
            m_Table[m] = t;

            t.Start();

            m.YellowHealthbar = true;
        }

        public static void EndWound(Mobile m)
        {
            if (!IsWounded(m))
                return;

            Timer t = (Timer)m_Table[m];

            if (t != null)
                t.Stop();

            m_Table.Remove(m);

            m.YellowHealthbar = false;
            m.SendLocalizedMessage(1060208); // You are no longer mortally wounded.
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!this.Validate(attacker) || !this.CheckMana(attacker, true))
                return;

            ClearCurrentAbility(attacker);

            attacker.SendLocalizedMessage(1060086); // You deliver a mortal wound!
            defender.SendLocalizedMessage(1060087); // You have been mortally wounded!

            defender.PlaySound(0x1E1);
            defender.FixedParticles(0x37B9, 244, 25, 9944, 31, 0, EffectLayer.Waist);

            // Do not reset timer if one is already in place.
            if (!IsWounded(defender))
            {
                if (Spells.SkillMasteries.BardSpell.GetSpellForParty(defender, typeof(Spells.SkillMasteries.ResilienceSpell)) != null)//Halves time
                    BeginWound(defender, defender.Player ? TimeSpan.FromSeconds(3.0) : TimeSpan.FromSeconds(6));
                else
                    BeginWound(defender, defender.Player ? PlayerDuration : NPCDuration);
            }
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Mobile;
            public InternalTimer(Mobile m, TimeSpan duration)
                : base(duration)
            {
                this.m_Mobile = m;
                this.Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                EndWound(this.m_Mobile);
            }
        }
    }
}