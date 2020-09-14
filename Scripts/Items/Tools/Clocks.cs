#region References
using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Server.Items
{
    public enum MoonPhase
    {
        WaxingCrescent,
        FirstQuarter,
        WaxingGibbous,
        Full,
        WaningGibbous,
        LastQuarter,
        WaningCrescent,
        New
    }

    [Flipable(0x104B, 0x104C)]
    public class Clock : Item, ISecurable
    {
        public const double SecondsPerUOMinute = 5.0;
        public const double MinutesPerUODay = SecondsPerUOMinute * 24;

        private static readonly DateTime WorldStart = new DateTime(1997, 9, 1);

        public static DateTime ServerStart { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        [Constructable]
        public Clock()
            : this(0x104B)
        {
        }

        [Constructable]
        public Clock(int itemID)
            : base(itemID)
        {
            Weight = 3.0;
            Level = SecureLevel.CoOwners;
        }

        public Clock(Serial serial)
            : base(serial)
        {
        }

        [CallPriority(-1)]
        public static void Initialize()
        {
            ServerStart = DateTime.UtcNow;

            Timer.DelayCall(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2), ClockTime.Tick_Callback);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public static MoonPhase GetMoonPhase(Map map, int x, int y)
        {
            int hours, minutes, totalMinutes;

            GetTime(map, x, y, out hours, out minutes, out totalMinutes);

            if (map != null)
            {
                totalMinutes /= 10 + (map.MapIndex * 20);
            }

            return (MoonPhase)(totalMinutes % 8);
        }

        public static void GetTime(Map map, int x, int y, out int hours, out int minutes)
        {
            int totalMinutes;

            GetTime(map, x, y, out hours, out minutes, out totalMinutes);
        }

        public static void GetTime(Map map, int x, int y, out int hours, out int minutes, out int totalMinutes)
        {
            TimeSpan timeSpan = DateTime.UtcNow - WorldStart;

            totalMinutes = (int)(timeSpan.TotalSeconds / SecondsPerUOMinute);

            if (map != null)
            {
                totalMinutes += map.MapIndex * 320;
            }

            // Really on OSI this must be by subserver
            totalMinutes += x / 16;

            hours = (totalMinutes / 60) % 24;
            minutes = totalMinutes % 60;
        }

        public static void GetTime(out int generalNumber, out string exactTime)
        {
            GetTime(null, 0, 0, out generalNumber, out exactTime);
        }

        public static void GetTime(Mobile from, out int generalNumber, out string exactTime)
        {
            GetTime(from.Map, from.X, from.Y, out generalNumber, out exactTime);
        }

        public static void GetTime(Map map, int x, int y, out int generalNumber, out string exactTime)
        {
            int hours, minutes;

            GetTime(map, x, y, out hours, out minutes);

            // 00:00 AM - 00:59 AM : Witching hour
            // 01:00 AM - 03:59 AM : Middle of night
            // 04:00 AM - 07:59 AM : Early morning
            // 08:00 AM - 11:59 AM : Late morning
            // 12:00 PM - 12:59 PM : Noon
            // 01:00 PM - 03:59 PM : Afternoon
            // 04:00 PM - 07:59 PM : Early evening
            // 08:00 PM - 11:59 AM : Late at night

            if (hours >= 20)
            {
                generalNumber = 1042957; // It's late at night
            }
            else if (hours >= 16)
            {
                generalNumber = 1042956; // It's early in the evening
            }
            else if (hours >= 13)
            {
                generalNumber = 1042955; // It's the afternoon
            }
            else if (hours >= 12)
            {
                generalNumber = 1042954; // It's around noon
            }
            else if (hours >= 08)
            {
                generalNumber = 1042953; // It's late in the morning
            }
            else if (hours >= 04)
            {
                generalNumber = 1042952; // It's early in the morning
            }
            else if (hours >= 01)
            {
                generalNumber = 1042951; // It's the middle of the night
            }
            else
            {
                generalNumber = 1042950; // 'Tis the witching hour. 12 Midnight.
            }

            hours %= 12;

            if (hours == 0)
            {
                hours = 12;
            }

            exactTime = string.Format("{0}:{1:D2}", hours, minutes);
        }

        public override void OnDoubleClick(Mobile from)
        {
            int genericNumber;
            string exactTime;

            GetTime(from, out genericNumber, out exactTime);

            SendLocalizedMessageTo(from, genericNumber);
            SendLocalizedMessageTo(from, 1042958, exactTime); // ~1_TIME~ to be exact
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version

            writer.Write((int)Level);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    Level = (SecureLevel)reader.ReadInt();
                    break;
                case 0:
                    break;
            }
        }
    }

    public class ClockTime : Clock
    {
        private static readonly List<ClockTime> _Instances = new List<ClockTime>();

        [Constructable]
        public ClockTime()
            : this(0x104B)
        {
        }

        [Constructable]
        public ClockTime(int itemID)
            : base(itemID)
        {
            Weight = 10.0;
            LootType = LootType.Blessed;
            _Instances.Add(this);
        }

        public ClockTime(Serial serial)
            : base(serial)
        {
        }

        public override void Delete()
        {
            base.Delete();

            _Instances.Remove(this);
        }

        public static void Tick_Callback()
        {
            foreach (ClockTime clock in _Instances.Where(p => p != null && !p.Deleted && p.IsLockedDown))
            {
                IPooledEnumerable ie = clock.GetMobilesInRange(10);

                foreach (Mobile m in ie)
                {
                    if (m.Player)
                    {
                        int hours, minutes;

                        GetTime(m.Map, m.X, m.Y, out hours, out minutes);

                        if (minutes == 00 && (hours == 12 || hours == 00 || hours == 06 || hours == 18))
                            m.PlaySound(1634);
                        else if (minutes == 00)
                            m.PlaySound(1635);
                    }
                }

                ie.Free();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();

            _Instances.Add(this);
        }
    }

    [Flipable(0x104B, 0x104C)]
    public class ClockRight : Clock
    {
        [Constructable]
        public ClockRight()
            : base(0x104B)
        {
        }

        public ClockRight(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [Flipable(0x104B, 0x104C)]
    public class ClockLeft : Clock
    {
        [Constructable]
        public ClockLeft()
            : base(0x104C)
        {
        }

        public ClockLeft(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [Flipable(0x44DD, 0x44E1)]
    public class LargeGrandfatherClock : ClockTime
    {
        public override int LabelNumber => 1149902;  // Large Grandfather Clock

        [Constructable]
        public LargeGrandfatherClock()
            : base(0x44DD)
        {
        }

        public LargeGrandfatherClock(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [Flipable(0x44D5, 0x44D9)]
    public class SmallGrandfatherClock : ClockTime
    {
        public override int LabelNumber => 1149901;  // Small Grandfather Clock

        [Constructable]
        public SmallGrandfatherClock()
            : base(0x44D5)
        {
        }

        public SmallGrandfatherClock(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [Flipable(0x48D4, 0x48D8)]
    public class WhiteGrandfatherClock : ClockTime
    {
        public override int LabelNumber => 1149903;  // White Grandfather Clock

        [Constructable]
        public WhiteGrandfatherClock()
            : base(0x48D4)
        {
        }

        public WhiteGrandfatherClock(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}