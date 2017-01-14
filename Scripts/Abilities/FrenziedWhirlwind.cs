using System;
using System.Collections.Generic;
using Server;
using Server.Spells;
using Server.Engines.PartySystem;
using Server.Network;

namespace Server.Items
{
    /// <summary>
    /// A quick attack to all enemies in range of your weapon that causes damage over time. Requires Bushido or Ninjitsu skill.
    /// </summary>
    public class FrenziedWhirlwind : WeaponAbility
    {
        public FrenziedWhirlwind()
        {
        }

        public override bool CheckSkills(Mobile from)
        {
            if (GetSkill(from, SkillName.Ninjitsu) < 50.0 && GetSkill(from, SkillName.Bushido) < 50.0)
            {
                from.SendLocalizedMessage(1063347, "50"); // You need ~1_SKILL_REQUIREMENT~ Bushido or Ninjitsu skill to perform that attack!
                return false;
            }

            return base.CheckSkills(from);
        }

        public override int BaseMana { get { return 30; } }

        private static Dictionary<Mobile, Timer> m_Registry = new Dictionary<Mobile, Timer>();
        public static Dictionary<Mobile, Timer> Registry { get { return m_Registry; } }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!Validate(attacker))	//Mana check after check that there are targets
                return;

            ClearCurrentAbility(attacker);

            Map map = attacker.Map;

            if (map == null)
                return;

            BaseWeapon weapon = attacker.Weapon as BaseWeapon;

            if (weapon == null)
                return;

            List<Mobile> targets = new List<Mobile>();

            IPooledEnumerable eable = attacker.GetMobilesInRange(2);
            foreach (Mobile m in eable)
            {
                if (m != attacker && SpellHelper.ValidIndirectTarget(attacker, m))
                {
                    if (m == null || m.Deleted || m.Map != attacker.Map || !m.Alive || !attacker.CanSee(m) || !attacker.CanBeHarmful(m))
                        continue;

                    if (!attacker.InRange(m, weapon.MaxRange))
                        continue;

                    if (attacker.InLOS(m))
                        targets.Add(m);
                }
            }
            eable.Free();

            if (targets.Count > 0)
            {
                if (!CheckMana(attacker, true))
                    return;

                attacker.FixedEffect(0x3728, 10, 15);
                attacker.PlaySound(0x2A1);

                for (int i = 0; i < targets.Count; ++i)
                {
                    Mobile m = targets[i];
                    attacker.DoHarmful(m, true);

                    if (m_Registry.ContainsKey(m) && m_Registry[m] != null)
                        m_Registry[m].Stop();

                    Timer t = new InternalTimer(attacker, m);
                    t.Start();
                    m_Registry[m] = t;

                    m.Send(SpeedControl.WalkSpeed);
                }
            }
        }

        public static void RemoveFromRegistry(Mobile from)
        {
            if (m_Registry.ContainsKey(from))
                m_Registry.Remove(from);

            Timer.DelayCall(TimeSpan.FromSeconds(2), () => from.Send(SpeedControl.Disable));
        }

        private class InternalTimer : Timer
        {
            private Mobile m_Attacker;
            private Mobile m_Defender;

            public InternalTimer(Mobile attacker, Mobile defender)
                : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
                m_Attacker = attacker;
                m_Defender = defender;

                DoHit();
                Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                if (m_Defender.Alive && m_Attacker.Alive)
                    DoHit();

                Server.Items.FrenziedWhirlwind.RemoveFromRegistry(m_Defender);
                Stop();
            }

            private void DoHit()
            {
                if (m_Attacker.InRange(m_Defender.Location, 2))
                {
                    m_Attacker.FixedEffect(0x3728, 10, 15);
                    m_Attacker.PlaySound(0x2A1);

                    int amount = (int)(10.0 * ((Math.Max(m_Attacker.Skills[SkillName.Bushido].Value, m_Attacker.Skills[SkillName.Ninjitsu].Value) - 50.0) / 70.0 + 5));

                    AOS.Damage(m_Defender, m_Attacker, amount, 100, 0, 0, 0, 0);

                    m_Attacker.SendLocalizedMessage(1060161); // The whirling attack strikes a target!
                    m_Defender.SendLocalizedMessage(1060162); // You are struck by the whirling attack and take damage!
                }
            }
        }
    }
}