using System;
using System.Collections.Generic;

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

        private static readonly Dictionary<Mobile, Timer> m_Table = new Dictionary<Mobile, Timer>();
        private static readonly List<Mobile> m_EffectReduction = new List<Mobile>();

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
            return m_Table.ContainsKey(m);
        }

        public static void BeginWound(Mobile m, TimeSpan duration)
        {
            Timer t;

            if (m_Table.ContainsKey(m))
            {
                EndWound(m, true);
            }

            if (Core.HS && m_EffectReduction.Contains(m))
            {
                double d = duration.TotalSeconds;
                duration = TimeSpan.FromSeconds(d / 2);
            }

            t = new InternalTimer(m, duration);
            m_Table[m] = t;

            t.Start();

            m.YellowHealthbar = true;
            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.MortalStrike, 1075810, 1075811, duration, m));
        }

        public static void EndWound(Mobile m, bool natural = false)
        {
            if (!IsWounded(m))
                return;

            Timer t = m_Table[m];

            if (t != null)
                t.Stop();

            m_Table.Remove(m);

            BuffInfo.RemoveBuff(m, BuffIcon.MortalStrike);

            m.YellowHealthbar = false;
            m.SendLocalizedMessage(1060208); // You are no longer mortally wounded.

            if (Core.HS && natural && !m_EffectReduction.Contains(m))
            {
                m_EffectReduction.Add(m);

                Timer.DelayCall(TimeSpan.FromSeconds(8), () =>
                    {
                        if (m_EffectReduction.Contains(m))
                            m_EffectReduction.Remove(m);
                    });
            }
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!Validate(attacker) || !CheckMana(attacker, true))
                return;

            ClearCurrentAbility(attacker);

            attacker.SendLocalizedMessage(1060086); // You deliver a mortal wound!
            defender.SendLocalizedMessage(1060087); // You have been mortally wounded!

            defender.PlaySound(0x1E1);
            defender.FixedParticles(0x37B9, 244, 25, 9944, 31, 0, EffectLayer.Waist);

            // Do not reset timer if one is already in place.
            if (Core.HS || !IsWounded(defender))
            {
                if (Spells.SkillMasteries.SkillMasterySpell.UnderPartyEffects(defender, typeof(Spells.SkillMasteries.ResilienceSpell))) //Halves time
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
                m_Mobile = m;
                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                EndWound(m_Mobile, true);
            }
        }
    }
}