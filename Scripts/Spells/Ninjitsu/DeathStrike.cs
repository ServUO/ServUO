#region

using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;

#endregion

namespace Server.Spells.Ninjitsu
{
    public class DeathStrike : NinjaMove
    {
        public static readonly TimeSpan DamageDelay = TimeSpan.FromSeconds(3.0);
        private static Dictionary<Mobile, DeathStrikeInfo> m_Table = new Dictionary<Mobile, DeathStrikeInfo>();

        public override int BaseMana => 30;
        public override double RequiredSkill => 85.0;
        public override TextDefinition AbilityMessage => new TextDefinition(1063091);// You prepare to hit your opponent with a Death Strike.

        public static void AddStep(Mobile m)
        {
            if (!m_Table.ContainsKey(m))
                return;

            DeathStrikeInfo info = m_Table[m];

            if (++info._Steps >= 5)
            {
                ProcessDeathStrike(info);
            }
        }

        public override double GetDamageScalar(Mobile attacker, Mobile defender)
        {
            return 0.5;
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!Validate(attacker) || !CheckMana(attacker, true))
            {
                return;
            }

            ClearCurrentMove(attacker);            

            if (!attacker.CheckSkill(MoveSkill, RequiredSkill - 12.5, RequiredSkill + 37.5))
            {
                attacker.SendLocalizedMessage(1070779); // You missed your opponent with a Death Strike.
                return;
            }

            DeathStrikeInfo info;

            if (m_Table.ContainsKey(defender))
            {
                defender.SendLocalizedMessage(1063092); // Your opponent lands another Death Strike!

                info = m_Table[defender];

                if (info._Timer != null)
                {
                    info._Timer.Stop();
                }

                m_Table.Remove(defender);
            }
            else
            {
                defender.SendLocalizedMessage(1063093); // You have been hit by a Death Strike!  Move with caution!
            }

            attacker.SendLocalizedMessage(1063094); // You inflict a Death Strike upon your opponent!

            defender.FixedParticles(0x374A, 1, 17, 0x26BC, EffectLayer.Waist);
            attacker.PlaySound(attacker.Female ? 0x50D : 0x50E);

            var ninjitsu = attacker.Skills[SkillName.Ninjitsu].Base;
            var hiding = attacker.Skills[SkillName.Hiding].Base;
            var stealth = attacker.Skills[SkillName.Stealth].Base;

            var avarage = (hiding + stealth) / 100;

            var scalar = ninjitsu / 9;

            var basedamage = ninjitsu / 5.7;

            var _totaldamage = (int)(basedamage + (avarage * scalar));

            if (_totaldamage > 50 && attacker is PlayerMobile && defender is PlayerMobile)
                _totaldamage = 50;

            info = new DeathStrikeInfo(defender, attacker, _totaldamage, attacker.Weapon is BaseRanged);
            info._Timer = Timer.DelayCall(DamageDelay, () => ProcessDeathStrike(info));

            m_Table[defender] = info;

            BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.DeathStrike, 1075645, DamageDelay, defender, string.Format("{0}", _totaldamage)));
        }

        private static void ProcessDeathStrike(DeathStrikeInfo info)
        {
            var damage = info._Damage;

            if (info._isRanged)
                damage /= 2;

            if (info._Steps < 5)
                damage /= 3;            

            AOS.Damage(info._Target, info._Attacker, damage, 0, 0, 0, 0, 0, 0, 100); // Damage is direct.

            if (info._Timer != null)
            {
                info._Timer.Stop();
            }

            m_Table.Remove(info._Target);
        }

        private class DeathStrikeInfo
        {
            public readonly Mobile _Target;
            public readonly Mobile _Attacker;
            public readonly int _Damage;
            public readonly bool _isRanged;
            public int _Steps;
            public Timer _Timer;

            public DeathStrikeInfo(Mobile target, Mobile attacker, int damageBonus, bool isRanged)
            {
                _Target = target;
                _Attacker = attacker;
                _Damage = damageBonus;
                _isRanged = isRanged;
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
