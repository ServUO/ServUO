using System;
using System.Linq;

using System.Collections.Generic;
using Server.Network;
using Server.Mobiles;

namespace Server.Misc
{
    public class Aggression
    {
        private static readonly TimeSpan Delay = TimeSpan.FromMinutes(2.0);
        private const string AggressorFormat = "You are attacking {0}!";
        private const string AggressedFormat = "{0} is attacking you!";
        private const int Hue = 0x22;

        public static TimeSpan CombatHeatDelay { get { return Delay; } }

        public static void Initialize()
        {
            EventSink.AggressiveAction += EventSink_AggressiveAction;
            //EventSink.PlayerDeath += EventSink_PlayerDeath;
            //EventSink.CreatureDeath += EventSink_CreatureDeath;
        }

        public static void EventSink_AggressiveAction(AggressiveActionEventArgs e)
        {
            Mobile aggressor = e.Aggressor;
            Mobile aggressed = e.Aggressed;

            if (!aggressor.Player || !aggressed.Player)
                return;

            if (!CheckAggressions(aggressor, aggressed))
            {
                aggressor.LocalOverheadMessage(MessageType.Regular, Hue, true, String.Format(AggressorFormat, aggressed.Name));
                aggressed.LocalOverheadMessage(MessageType.Regular, Hue, true, String.Format(AggressedFormat, aggressor.Name));
            }

            BuffInfo.AddBuff(aggressor, new BuffInfo(BuffIcon.HeatOfBattleStatus, 1153801, 1153827, Delay, aggressor, true));
            BuffInfo.AddBuff(aggressed, new BuffInfo(BuffIcon.HeatOfBattleStatus, 1153801, 1153827, Delay, aggressed, true));
        }

        public static void EventSink_PlayerDeath(PlayerDeathEventArgs e)
        {
            var killed = e.Mobile;
             
            // Remove all those the one killed aggressed
            foreach (var m in killed.Aggressed.Select(x => x.Defender))
            {
                AggressorInfo info = m.Aggressed.FirstOrDefault(i => i.Defender == killed);

                if (info != null)
                {
                    m.Aggressed.Remove(info);
                }

                info = m.Aggressors.FirstOrDefault(i => i.Attacker == killed);

                if (info != null)
                {
                    m.Aggressors.Remove(info);
                }

                if (!CheckAggressions(m))
                {
                    BuffInfo.RemoveBuff(m, BuffIcon.HeatOfBattleStatus);
                }
            }

            // Remove all those who the one killed was aggressed by
            foreach (var m in killed.Aggressors.Select(x => x.Attacker))
            {
                AggressorInfo info = m.Aggressed.FirstOrDefault(i => i.Defender == killed);

                if (info != null)
                {
                    m.Aggressed.Remove(info);
                }

                info = m.Aggressors.FirstOrDefault(i => i.Attacker == killed);

                if (info != null)
                {
                    m.Aggressors.Remove(info);
                }

                if (!CheckAggressions(m))
                {
                    BuffInfo.RemoveBuff(m, BuffIcon.HeatOfBattleStatus);
                }
            }

            killed.Aggressors.Clear();
            killed.Aggressed.Clear();

            BuffInfo.RemoveBuff(killed, BuffIcon.HeatOfBattleStatus);
        }

        public static void EventSink_CreatureDeath(CreatureDeathEventArgs e)
        {
            var killed = e.Creature;

            // Remove all those the one killed aggressed
            foreach (var m in killed.Aggressed.Select(x => x.Defender))
            {
                AggressorInfo info = m.Aggressed.FirstOrDefault(i => i.Defender == killed);

                if (info != null)
                {
                    m.Aggressed.Remove(info);
                }

                info = m.Aggressors.FirstOrDefault(i => i.Attacker == killed);

                if (info != null)
                {
                    m.Aggressors.Remove(info);
                }

                if (!CheckAggressions(m))
                {
                    BuffInfo.RemoveBuff(m, BuffIcon.HeatOfBattleStatus);
                }
            }

            // Remove all those who the one killed was aggressed by
            foreach (var m in killed.Aggressors.Select(x => x.Attacker))
            {
                AggressorInfo info = m.Aggressed.FirstOrDefault(i => i.Defender == killed);

                if (info != null)
                {
                    m.Aggressed.Remove(info);
                }

                info = m.Aggressors.FirstOrDefault(i => i.Attacker == killed);

                if (info != null)
                {
                    m.Aggressors.Remove(info);
                }

                if (!CheckAggressions(m))
                {
                    BuffInfo.RemoveBuff(m, BuffIcon.HeatOfBattleStatus);
                }
            }

            killed.Aggressors.Clear();
            killed.Aggressed.Clear();
        }

        public static bool CheckAggressions(Mobile m1, Mobile m2)
        {
            List<AggressorInfo> list = m1.Aggressors;

            for (int i = 0; i < list.Count; ++i)
            {
                AggressorInfo info = list[i];

                if (info.Attacker == m2 && DateTime.UtcNow < (info.LastCombatTime + Delay))
                    return true;
            }

            list = m2.Aggressors;

            for (int i = 0; i < list.Count; ++i)
            {
                AggressorInfo info = list[i];

                if (info.Attacker == m1 && DateTime.UtcNow < (info.LastCombatTime + Delay))
                    return true;
            }

            return false;
        }

        public static bool CheckAggressions(Mobile m)
        {
            return m.Aggressed.Select(x => x.Defender).Any(mob => mob is PlayerMobile || (mob is BaseCreature && !((BaseCreature)mob).IsMonster)) ||
                                 m.Aggressors.Select(x => x.Attacker).Any(mob => mob is PlayerMobile || (mob is BaseCreature && !((BaseCreature)mob).IsMonster));
        }
    }
}