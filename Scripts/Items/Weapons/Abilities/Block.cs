using System;
using System.Collections;

namespace Server.Items
{
    /// <summary>
    /// Raises your defenses for a short time. Requires Bushido or Ninjitsu skill.
    /// </summary>
    public class Block : WeaponAbility
    {
        private static readonly Hashtable m_Table = new Hashtable();
        public Block()
        {
        }

        public override int BaseMana
        {
            get
            {
                return 20;
            }
        }
        public static bool GetBonus(Mobile targ, ref int bonus)
        {
            BlockInfo info = m_Table[targ] as BlockInfo;

            if (info == null)
                return false;

            bonus = info.m_Bonus;
            return true;
        }

        public static void BeginBlock(Mobile m, int bonus)
        {
            EndBlock(m);

            BlockInfo info = new BlockInfo(m, bonus);
            info.m_Timer = new InternalTimer(m);

            m_Table[m] = info;
        }

        public static void EndBlock(Mobile m)
        {
            BlockInfo info = m_Table[m] as BlockInfo;

            if (info != null)
            {
                if (info.m_Timer != null)
                    info.m_Timer.Stop();

                m_Table.Remove(m);
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

            ClearCurrentAbility(attacker);

            attacker.SendLocalizedMessage(1063345); // You block an attack!
            defender.SendLocalizedMessage(1063346); // Your attack was blocked!

            attacker.FixedParticles(0x37C4, 1, 16, 0x251D, 0x39D, 0x3, EffectLayer.RightHand);

            int bonus = (int)(10.0 * ((Math.Max(attacker.Skills[SkillName.Bushido].Value, attacker.Skills[SkillName.Ninjitsu].Value) - 50.0) / 70.0 + 5));

            BeginBlock(attacker, bonus);
        }

        private class BlockInfo
        {
            public readonly Mobile m_Target;
            public readonly int m_Bonus;
            public Timer m_Timer;
            public BlockInfo(Mobile target, int bonus)
            {
                this.m_Target = target;
                this.m_Bonus = bonus;
            }
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Mobile;
            public InternalTimer(Mobile m)
                : base(TimeSpan.FromSeconds(6.0))
            {
                this.m_Mobile = m;
                this.Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                EndBlock(this.m_Mobile);
            }
        }
    }
}