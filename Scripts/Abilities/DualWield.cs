using System;
using System.Collections;

namespace Server.Items
{
    /// <summary>
    /// Attack faster as you swing with both weapons.
    /// </summary>
    public class DualWield : WeaponAbility
    {
        private static readonly Hashtable m_Registry = new Hashtable();
        public DualWield()
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
                return 20;
            }
        }
        public override bool CheckSkills(Mobile from)
        {
            if (this.GetSkill(from, SkillName.Ninjitsu) < 50.0)
            {
                from.SendLocalizedMessage(1063352, "50"); // You need ~1_SKILL_REQUIREMENT~ Ninjitsu skill to perform that attack!
                return false;
            }

            return base.CheckSkills(from);
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!this.Validate(attacker) || !this.CheckMana(attacker, true))
                return;

            if (Registry.Contains(attacker))
            {
                DualWieldTimer existingtimer = (DualWieldTimer)Registry[attacker];
                existingtimer.Stop();
                Registry.Remove(attacker);
            }

            ClearCurrentAbility(attacker);

            attacker.SendLocalizedMessage(1063362); // You dually wield for increased speed!

            attacker.FixedParticles(0x3779, 1, 15, 0x7F6, 0x3E8, 3, EffectLayer.LeftHand);

            Timer t = new DualWieldTimer(attacker, (int)(20.0 + 3.0 * (attacker.Skills[SkillName.Ninjitsu].Value - 50.0) / 7.0));	//20-50 % increase

            t.Start();
            Registry.Add(attacker, t);
        }

        public class DualWieldTimer : Timer
        {
            private readonly Mobile m_Owner;
            private readonly int m_BonusSwingSpeed;
            public DualWieldTimer(Mobile owner, int bonusSwingSpeed)
                : base(TimeSpan.FromSeconds(6.0))
            {
                this.m_Owner = owner;
                this.m_BonusSwingSpeed = bonusSwingSpeed;
                this.Priority = TimerPriority.FiftyMS;
            }

            public int BonusSwingSpeed
            {
                get
                {
                    return this.m_BonusSwingSpeed;
                }
            }
            protected override void OnTick()
            {
                Registry.Remove(this.m_Owner);
            }
        }
    }
}