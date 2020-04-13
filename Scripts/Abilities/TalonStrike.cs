using System;
using System.Collections;
using Server.Mobiles;

namespace Server.Items
{
    /// <summary>
    /// Attack with increased damage with additional damage over time.
    /// </summary>
    public class TalonStrike : WeaponAbility
    {
        private static readonly Hashtable m_Registry = new Hashtable();
        public TalonStrike()
        {
        }

        public override SkillName GetSecondarySkill(Mobile from)
        {
            return from.Skills[SkillName.Ninjitsu].Base > from.Skills[SkillName.Bushido].Base ? SkillName.Ninjitsu : SkillName.Bushido;
        }

        public static Hashtable Registry => m_Registry;

        public override int BaseMana => 20;

        public override double DamageScalar => 1.2;

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (Registry.Contains(defender) || !Validate(attacker) || !CheckMana(attacker, true))
                return;

            ClearCurrentAbility(attacker);

            attacker.SendLocalizedMessage(1063358); // You deliver a talon strike!
            defender.SendLocalizedMessage(1063359); // Your attacker delivers a talon strike!

            defender.FixedParticles(0x373A, 1, 17, 0x26BC, 0x662, 0, EffectLayer.Waist);

            Timer t = new InternalTimer(defender, (int)(10.0 * (attacker.Skills[SkillName.Ninjitsu].Value - 50.0) / 70.0 + 5), attacker);	//5 - 15 damage

            BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.TalonStrike, 1028856, 1151309, TimeSpan.FromSeconds(5.0), defender, "40"));

            t.Start();

            Registry.Add(defender, t);

            if (attacker is BaseCreature)
                PetTrainingHelper.OnWeaponAbilityUsed((BaseCreature)attacker, SkillName.Ninjitsu);
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Defender;
            private readonly Mobile m_Attacker;
            private readonly double DamagePerTick;
            private double m_DamageRemaining;
            private double m_DamageToDo;
            public InternalTimer(Mobile defender, int totalDamage, Mobile attacker)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(0.25), 12)// 3 seconds at .25 seconds apart = 12.  Confirm delay inbetween of .25 each.
            {
                m_Defender = defender;
                m_DamageRemaining = (double)totalDamage;
                Priority = TimerPriority.TwentyFiveMS;

                m_Attacker = attacker;

                DamagePerTick = (double)totalDamage / 12 + .01;
            }

            protected override void OnTick()
            {
                if (!m_Defender.Alive || m_DamageRemaining <= 0)
                {
                    Stop();
                    Server.Items.TalonStrike.Registry.Remove(m_Defender);
                    return;
                }

                m_DamageRemaining -= DamagePerTick;
                m_DamageToDo += DamagePerTick;

                if (m_DamageRemaining <= 0 && m_DamageToDo < 1)
                    m_DamageToDo = 1.0; //Confirm this 'round up' at the end

                int damage = (int)m_DamageToDo;

                if (damage > 0)
                {
                    //m_Defender.Damage( damage, m_Attacker, false );
                    m_Defender.Hits -= damage;	//Don't show damage, don't disrupt
                    m_DamageToDo -= damage;
                }

                if (!m_Defender.Alive || m_DamageRemaining <= 0)
                {
                    Stop();
                    Server.Items.TalonStrike.Registry.Remove(m_Defender);
                }
            }
        }
    }
}