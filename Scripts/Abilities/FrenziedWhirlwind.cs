using System;
using System.Collections;
using Server.Spells;

namespace Server.Items
{
    /// <summary>
    /// A quick attack to all enemies in range of your weapon that causes damage over time. Requires Bushido or Ninjitsu skill.
    /// </summary>
    public class FrenziedWhirlwind : WeaponAbility
    {
        private static readonly Hashtable m_Registry = new Hashtable();
        public FrenziedWhirlwind()
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
            if (this.GetSkill(from, SkillName.Ninjitsu) < 50.0 && this.GetSkill(from, SkillName.Bushido) < 50.0)
            {
                from.SendLocalizedMessage(1063347, "50"); // You need ~1_SKILL_REQUIREMENT~ Bushido or Ninjitsu skill to perform that attack!
                return false;
            }

            return base.CheckSkills(from);
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!this.Validate(attacker))	//Mana check after check that there are targets
                return;

            ClearCurrentAbility(attacker);

            Map map = attacker.Map;

            if (map == null)
                return;

            BaseWeapon weapon = attacker.Weapon as BaseWeapon;

            if (weapon == null)
                return;

            ArrayList list = new ArrayList();

            foreach (Mobile m in attacker.GetMobilesInRange(1))
                list.Add(m);

            ArrayList targets = new ArrayList();

            for (int i = 0; i < list.Count; ++i)
            {
                Mobile m = (Mobile)list[i];

                if (m != defender && m != attacker && SpellHelper.ValidIndirectTarget(attacker, m))
                {
                    if (m == null || m.Deleted || m.Map != attacker.Map || !m.Alive || !attacker.CanSee(m) || !attacker.CanBeHarmful(m))
                        continue;

                    if (!attacker.InRange(m, weapon.MaxRange))
                        continue;

                    if (attacker.InLOS(m))
                        targets.Add(m);
                }
            }

            if (targets.Count > 0)
            {
                if (!this.CheckMana(attacker, true))
                    return;

                attacker.FixedEffect(0x3728, 10, 15);
                attacker.PlaySound(0x2A1);

                // 5-15 damage
                int amount = (int)(10.0 * ((Math.Max(attacker.Skills[SkillName.Bushido].Value, attacker.Skills[SkillName.Ninjitsu].Value) - 50.0) / 70.0 + 5));

                for (int i = 0; i < targets.Count; ++i)
                {
                    Mobile m = (Mobile)targets[i];
                    attacker.DoHarmful(m, true);

                    Timer t = Registry[m] as Timer;

                    if (t != null)
                    {
                        t.Stop();
                        Registry.Remove(m);
                    }

                    t = new InternalTimer(attacker, m, amount);
                    t.Start();
                    Registry.Add(m, t);
                }

                Timer.DelayCall(TimeSpan.FromSeconds(2.0), new TimerStateCallback(RepeatEffect), attacker);
            }
        }

        private void RepeatEffect(object state)
        {
            Mobile attacker = (Mobile)state;

            attacker.FixedEffect(0x3728, 10, 15);
            attacker.PlaySound(0x2A1);
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Attacker;
            private readonly Mobile m_Defender;
            private readonly double DamagePerTick;
            private double m_DamageRemaining;
            private double m_DamageToDo;
            public InternalTimer(Mobile attacker, Mobile defender, int totalDamage)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(0.25), 12)// 3 seconds at .25 seconds apart = 12.  Confirm delay inbetween of .25 each.
            {
                this.m_Attacker = attacker;
                this.m_Defender = defender;

                this.m_DamageRemaining = (double)totalDamage;
                this.DamagePerTick = (double)totalDamage / 12 + 0.01;

                this.Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                if (!this.m_Defender.Alive || this.m_DamageRemaining <= 0)
                {
                    this.Stop();
                    Server.Items.FrenziedWhirlwind.Registry.Remove(this.m_Defender);
                    return;
                }

                this.m_DamageRemaining -= this.DamagePerTick;
                this.m_DamageToDo += this.DamagePerTick;

                if (this.m_DamageRemaining <= 0 && this.m_DamageToDo < 1)
                    this.m_DamageToDo = 1.0; //Confirm this 'round up' at the end

                int damage = (int)this.m_DamageToDo;

                if (damage > 0)
                {
                    this.m_Defender.Damage(damage, this.m_Attacker);
                    this.m_DamageToDo -= damage;
                }

                if (!this.m_Defender.Alive || this.m_DamageRemaining <= 0)
                {
                    this.Stop();
                    Server.Items.FrenziedWhirlwind.Registry.Remove(this.m_Defender);
                }
            }
        }
    }
}