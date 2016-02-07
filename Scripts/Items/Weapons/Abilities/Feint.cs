using System;
using System.Collections;

namespace Server.Items
{
    /// <summary>
    /// Gain a defensive advantage over your primary opponent for a short time.
    /// </summary>
    public class Feint : WeaponAbility
    {
        private static readonly Hashtable m_Registry = new Hashtable();
        public Feint()
        {
        }

        public static Hashtable Registry
        {
            get
            {
                return m_Registry;
            }
        }
        public override int BaseMana
        {
            get
            {
                return 30;
            }
        }
        public override bool CheckSkills(Mobile from)
        {
            if (this.GetSkill(from, SkillName.Ninjitsu) < 50.0 && this.GetSkill(from, SkillName.Bushido) < 50.0)
            {
                from.SendLocalizedMessage(1063347, "50"); // You need ~1_SKILL_REQUIREMENT~ Bushido or Ninjitsu skill to perform that attack!
                return false;
            }

            return base.CheckSkills(from);
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!this.Validate(attacker) || !this.CheckMana(attacker, true))
                return;

            if (Registry.Contains(defender))
            {
                FeintTimer existingtimer = (FeintTimer)Registry[defender];
                existingtimer.Stop();
                Registry.Remove(defender);
            }

            ClearCurrentAbility(attacker);

            attacker.SendLocalizedMessage(1063360); // You baffle your target with a feint!
            defender.SendLocalizedMessage(1063361); // You were deceived by an attacker's feint!

            attacker.FixedParticles(0x3728, 1, 13, 0x7F3, 0x962, 0, EffectLayer.Waist);

            Timer t = new FeintTimer(defender, (int)(20.0 + 3.0 * (Math.Max(attacker.Skills[SkillName.Ninjitsu].Value, attacker.Skills[SkillName.Bushido].Value) - 50.0) / 7.0));	//20-50 % decrease

            t.Start();
            Registry.Add(defender, t);
        }

        public class FeintTimer : Timer
        {
            private readonly Mobile m_Defender;
            private readonly int m_SwingSpeedReduction;
            public FeintTimer(Mobile defender, int swingSpeedReduction)
                : base(TimeSpan.FromSeconds(6.0))
            {
                this.m_Defender = defender;
                this.m_SwingSpeedReduction = swingSpeedReduction;
                this.Priority = TimerPriority.FiftyMS;
            }

            public int SwingSpeedReduction
            {
                get
                {
                    return this.m_SwingSpeedReduction;
                }
            }
            protected override void OnTick()
            {
                Registry.Remove(this.m_Defender);
            }
        }
    }
}