using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Spells;
using Server.Engines.PartySystem;
using Server.Network;
using Server.Mobiles;

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

        public override SkillName GetSecondarySkill(Mobile from)
        {
            return from.Skills[SkillName.Ninjitsu].Base > from.Skills[SkillName.Bushido].Base ? SkillName.Ninjitsu : SkillName.Bushido;
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

            var targets = SpellHelper.AcquireIndirectTargets(attacker, attacker.Location, attacker.Map, 2).OfType<Mobile>().ToList();

            if (targets.Count > 0)
            {
                if (!CheckMana(attacker, true))
                    return;

                attacker.FixedEffect(0x3728, 10, 15);
                attacker.PlaySound(0x2A1);

                if (m_Registry.ContainsKey(attacker))
                {
                    RemoveFromRegistry(attacker);
                }

                m_Registry[attacker] = new InternalTimer(attacker, targets);

                foreach (var pm in targets.OfType<PlayerMobile>())
                {
                    BuffInfo.AddBuff(pm, new BuffInfo(BuffIcon.SplinteringEffect, 1153804, 1028852, TimeSpan.FromSeconds(2.0), pm));
                }

                if (defender is PlayerMobile && attacker is PlayerMobile)
                {
                    defender.SendSpeedControl(SpeedControlType.WalkSpeed);
                    BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.SplinteringEffect, 1153804, 1152144, TimeSpan.FromSeconds(2.0), defender));
                    Timer.DelayCall(TimeSpan.FromSeconds(2), mob => mob.SendSpeedControl(SpeedControlType.Disable), defender);
                }

                if (attacker is BaseCreature)
                    PetTrainingHelper.OnWeaponAbilityUsed((BaseCreature)attacker, SkillName.Ninjitsu);
            }

            ColUtility.Free(targets);
        }

        public static void RemoveFromRegistry(Mobile from)
        {
            if (m_Registry.ContainsKey(from))
            {
                m_Registry[from].Stop();
                m_Registry.Remove(from);
            }
        }

        private class InternalTimer : Timer
        {
            private Mobile m_Attacker;
            private IEnumerable<Mobile> m_List;
            private long m_Start;

            public InternalTimer(Mobile attacker, IEnumerable<Mobile> list)
                : base(TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(500))
            {
                m_Attacker = attacker;
                m_List = list;

                m_Start = Core.TickCount;

                Priority = TimerPriority.TwentyFiveMS;
                DoHit();

                Start();
            }

            protected override void OnTick()
            {
                if (m_Attacker.Alive)
                    DoHit();

                if (!m_Attacker.Alive || m_Start + 2000 < Core.TickCount)
                {
                    Server.Items.FrenziedWhirlwind.RemoveFromRegistry(m_Attacker);
                }
            }

            private void DoHit()
            {
                foreach (Mobile m in m_List)
                {
                    if (m_Attacker.InRange(m.Location, 2) && m.Alive && m.Map == m_Attacker.Map)
                    {
                        m_Attacker.FixedEffect(0x3728, 10, 15);
                        m_Attacker.PlaySound(0x2A1);

                        int skill = m_Attacker is BaseCreature ? (int)m_Attacker.Skills[SkillName.Ninjitsu].Value :
                                                              (int)Math.Max(m_Attacker.Skills[SkillName.Bushido].Value, m_Attacker.Skills[SkillName.Ninjitsu].Value);

                        int amount = Utility.RandomMinMax((int)(skill / 50) * 5, (int)(skill / 50) * 20) + 2;
                        AOS.Damage(m, m_Attacker, amount, 100, 0, 0, 0, 0);

                        //m_Attacker.SendLocalizedMessage(1060161); // The whirling attack strikes a target!
                        //m_Defender.SendLocalizedMessage(1060162); // You are struck by the whirling attack and take damage!
                    }
                }
            }
        }
    }
}