using Server.Mobiles;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    /// <summary>
    /// Gain a defensive advantage over your primary opponent for a short time.
    /// </summary>
    public class Feint : WeaponAbility
    {
        private static readonly Dictionary<Mobile, FeintTimer> m_Registry = new Dictionary<Mobile, FeintTimer>();
        public static Dictionary<Mobile, FeintTimer> Registry => m_Registry;

        public override int BaseMana => 30;

        public override SkillName GetSecondarySkill(Mobile from)
        {
            return from.Skills[SkillName.Ninjitsu].Base > from.Skills[SkillName.Bushido].Base ? SkillName.Ninjitsu : SkillName.Bushido;
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!Validate(attacker) || !CheckMana(attacker, true))
                return;

            if (Registry.ContainsKey(attacker))
            {
                if (m_Registry[attacker] != null)
                    m_Registry[attacker].Stop();

                Registry.Remove(attacker);
            }

            bool creature = attacker is BaseCreature;
            ClearCurrentAbility(attacker);

            attacker.SendLocalizedMessage(1063360); // You baffle your target with a feint!
            defender.SendLocalizedMessage(1063361); // You were deceived by an attacker's feint!

            attacker.FixedParticles(0x3728, 1, 13, 0x7F3, 0x962, 0, EffectLayer.Waist);
            attacker.PlaySound(0x525);

            double skill = creature ? attacker.Skills[SkillName.Bushido].Value :
                                                   Math.Max(attacker.Skills[SkillName.Ninjitsu].Value, attacker.Skills[SkillName.Bushido].Value);

            int bonus = (int)(20.0 + 3.0 * (skill - 50.0) / 7.0);

            FeintTimer t = new FeintTimer(attacker, defender, bonus);   //20-50 % decrease

            t.Start();
            m_Registry[attacker] = t;

            string args = string.Format("{0}\t{1}", defender.Name, bonus);
            BuffInfo.AddBuff(attacker, new BuffInfo(BuffIcon.Feint, 1151308, 1151307, TimeSpan.FromSeconds(6), attacker, args));

            if (creature)
                PetTrainingHelper.OnWeaponAbilityUsed((BaseCreature)attacker, SkillName.Bushido);
        }

        public class FeintTimer : Timer
        {
            private readonly Mobile m_Owner;
            private readonly Mobile m_Enemy;
            private readonly int m_DamageReduction;

            public Mobile Owner => m_Owner;
            public Mobile Enemy => m_Enemy;

            public int DamageReduction => m_DamageReduction;

            public FeintTimer(Mobile owner, Mobile enemy, int DamageReduction)
                : base(TimeSpan.FromSeconds(6.0))
            {
                m_Owner = owner;
                m_Enemy = enemy;
                m_DamageReduction = DamageReduction;
                Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                Registry.Remove(m_Owner);
            }
        }
    }
}
