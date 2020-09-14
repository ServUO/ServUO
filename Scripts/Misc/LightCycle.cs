#region References
using Server.Commands;
using Server.Items;
using Server.Network;
using System;
#endregion

namespace Server
{
    public static class LightCycle
    {
        public const int DayLevel = 0;
        public const int NightLevel = 12;
        public const int DungeonLevel = 26;
        public const int JailLevel = 9;

        private static int _LevelOverride = int.MinValue;

        public static int LevelOverride
        {
            get { return _LevelOverride; }
            set
            {
                _LevelOverride = value;

                CheckLightLevels();
            }
        }

        public static void Initialize()
        {
            new LightCycleTimer(Clock.SecondsPerUOMinute).Start();

            EventSink.Login += OnLogin;

            CommandSystem.Register("GlobalLight", AccessLevel.GameMaster, Light_OnCommand);
        }

        public static void OnLogin(LoginEventArgs args)
        {
            Mobile m = args.Mobile;

            if (m != null)
                m.CheckLightLevels(true);
        }

        public static int ComputeLevelFor(Mobile from)
        {
            if (_LevelOverride > int.MinValue)
                return _LevelOverride;

            int hours, minutes;

            Clock.GetTime(from.Map, from.X, from.Y, out hours, out minutes);

            /* OSI times:
            * 
            * Midnight ->  3:59 AM : Night
            *  4:00 AM -> 11:59 PM : Day
            * 
            * RunUO times:
            * 
            * 10:00 PM -> 11:59 PM : Scale to night
            * Midnight ->  3:59 AM : Night
            *  4:00 AM ->  5:59 AM : Scale to day
            *  6:00 AM ->  9:59 PM : Day
            */

            if (hours < 4)
                return NightLevel;

            if (hours < 6)
                return NightLevel + (((((hours - 4) * 60) + minutes) * (DayLevel - NightLevel)) / 120);

            if (hours < 22)
                return DayLevel;

            if (hours < 24)
                return DayLevel + (((((hours - 22) * 60) + minutes) * (NightLevel - DayLevel)) / 120);

            return NightLevel; // should never be
        }

        public static void CheckLightLevels()
        {
            int i = NetState.Instances.Count;

            while (--i >= 0)
            {
                if (i >= NetState.Instances.Count)
                    continue;

                NetState ns = NetState.Instances[i];

                if (ns == null)
                    continue;

                Mobile m = ns.Mobile;

                if (m != null)
                    m.CheckLightLevels(false);
            }
        }

        [Usage("GlobalLight <value>")]
        [Description("Sets the current global light level.")]
        private static void Light_OnCommand(CommandEventArgs e)
        {
            if (e.Length >= 1)
            {
                LevelOverride = e.GetInt32(0);
                e.Mobile.SendMessage("Global light level override has been changed to {0}.", _LevelOverride);
            }
            else
            {
                LevelOverride = int.MinValue;
                e.Mobile.SendMessage("Global light level override has been cleared.");
            }
        }

        public class NightSightTimer : Timer
        {
            private readonly Mobile m_Owner;

            public NightSightTimer(Mobile owner)
                : base(TimeSpan.FromMinutes(Utility.Random(15, 25)))
            {
                m_Owner = owner;

                Priority = TimerPriority.OneMinute;
            }

            protected override void OnTick()
            {
                m_Owner.EndAction(typeof(LightCycle));
                m_Owner.LightLevel = 0;

                BuffInfo.RemoveBuff(m_Owner, BuffIcon.NightSight);
            }
        }

        private class LightCycleTimer : Timer
        {
            public LightCycleTimer(double interval)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(interval))
            {
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                CheckLightLevels();
            }
        }
    }
}