using Server.Items;
using Server.SkillHandlers;
using System;
using System.Collections;

namespace Server.Spells.Ninjitsu
{
    public class DeathStrike : NinjaMove
    {
        private static readonly Hashtable m_Table = new Hashtable();

        public override int BaseMana => 30;
        public override double RequiredSkill => 85.0;
        public override TextDefinition AbilityMessage => new TextDefinition(1063091);// You prepare to hit your opponent with a Death Strike.
        public static void AddStep(Mobile m)
        {
            DeathStrikeInfo info = m_Table[m] as DeathStrikeInfo;

            if (info == null)
                return;

            if (++info.m_Steps >= 5)
                ProcessDeathStrike(m);
        }

        public override double GetDamageScalar(Mobile attacker, Mobile defender)
        {
            return 0.5;
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!Validate(attacker) || !CheckMana(attacker, true))
                return;

            ClearCurrentMove(attacker);

            double ninjitsu = attacker.Skills[SkillName.Ninjitsu].Value;

            double chance;
            bool isRanged = attacker.Weapon is BaseRanged;

            if (ninjitsu < 100) //This formula is an approximation from OSI data.  TODO: find correct formula
                chance = 30 + (ninjitsu - 85) * 2.2;
            else
                chance = 63 + (ninjitsu - 100) * 1.1;

            if ((chance / 100) < Utility.RandomDouble())
            {
                attacker.SendLocalizedMessage(1070779); // You missed your opponent with a Death Strike.
                return;
            }

            DeathStrikeInfo info;

            int damageBonus = 0;

            if (m_Table.Contains(defender))
            {
                defender.SendLocalizedMessage(1063092); // Your opponent lands another Death Strike!

                info = (DeathStrikeInfo)m_Table[defender];

                if (info.m_Steps > 0)
                    damageBonus = attacker.Skills[SkillName.Ninjitsu].Fixed / 150;

                if (info.m_Timer != null)
                    info.m_Timer.Stop();

                m_Table.Remove(defender);
            }
            else
            {
                defender.SendLocalizedMessage(1063093); // You have been hit by a Death Strike!  Move with caution!
            }

            attacker.SendLocalizedMessage(1063094); // You inflict a Death Strike upon your opponent!

            defender.FixedParticles(0x374A, 1, 17, 0x26BC, EffectLayer.Waist);
            attacker.PlaySound(attacker.Female ? 0x50D : 0x50E);

            info = new DeathStrikeInfo(defender, attacker, damageBonus, isRanged);
            info.m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(5.0), new TimerStateCallback(ProcessDeathStrike), defender);

            m_Table[defender] = info;

            BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.DeathStrike, 1075645, TimeSpan.FromSeconds(5.0), defender, String.Format("{0}", damageBonus)));

            CheckGain(attacker);
        }

        private static void ProcessDeathStrike(object state)
        {
            Mobile defender = (Mobile)state;

            DeathStrikeInfo info = m_Table[defender] as DeathStrikeInfo;

            if (info == null)	//sanity
                return;

            int damage = 0;

            double ninjitsu = info.m_Attacker.Skills[SkillName.Ninjitsu].Value;
            double stalkingBonus = Tracking.GetStalkingBonus(info.m_Attacker, info.m_Target);

            double scalar = (info.m_Attacker.Skills[SkillName.Hiding].Value + info.m_Attacker.Skills[SkillName.Stealth].Value) / 220;

            if (scalar > 1)
                scalar = 1;

            // New formula doesn't apply DamageBonus anymore, caps must be, directly, 60/30.
            if (info.m_Steps >= 5)
                damage = (int)Math.Floor(Math.Min(60, (ninjitsu / 3) * (0.3 + 0.7 * scalar) + stalkingBonus));
            else
                damage = (int)Math.Floor(Math.Min(30, (ninjitsu / 9) * (0.3 + 0.7 * scalar) + stalkingBonus));

            if (info.m_isRanged)
                damage /= 2;

            AOS.Damage(info.m_Target, info.m_Attacker, damage, 0, 0, 0, 0, 0, 0, 100); // Damage is direct.

            if (info.m_Timer != null)
                info.m_Timer.Stop();

            m_Table.Remove(info.m_Target);
        }

        private class DeathStrikeInfo
        {
            public readonly Mobile m_Target;
            public readonly Mobile m_Attacker;
            public readonly int m_DamageBonus;
            public readonly bool m_isRanged;
            public int m_Steps;
            public Timer m_Timer;
            public DeathStrikeInfo(Mobile target, Mobile attacker, int damageBonus, bool isRanged)
            {
                m_Target = target;
                m_Attacker = attacker;
                m_DamageBonus = damageBonus;
                m_isRanged = isRanged;
            }
        }

        public static void Initialize()
        {
            EventSink.Movement += EventSink_Movement;
        }

        public static void EventSink_Movement(MovementEventArgs e)
        {
            AddStep(e.Mobile);
        }
    }
}
