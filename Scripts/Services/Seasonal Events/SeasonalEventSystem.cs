using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Server;
using Server.Mobiles;
using Server.Gumps;
using Server.Misc;
using Server.Commands;
using Server.Engines.TreasuresOfDoom;
using Server.Engines.Khaldun;
using Server.Engines.SorcerersDungeon;

namespace Server.Engines.SeasonalEvents
{
    public enum EventType
    {
        TreasuresOfTokuno,
        VirtueArtifacts,
        TreasuresOfKotlCity,
        SorcerersDungeon,
        TreasuresOfDoom,
        TreasuresOfKhaldun
    }

    public enum EventStatus
    {
        Inactive,
        Active,
        Seasonal,
    }

    public interface ISeasonalEventObject
    {
        EventType EventType { get; }
        bool EventActive { get; }
    }

	public class SeasonalEventSystem
	{
        public static string FilePath = Path.Combine("Saves/Misc", "SeasonalEvents.bin");

        public static List<SeasonalEventEntry> Entries { get; set; }

        public static void Configure()
        {
            LoadEntries();

            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;

            CommandSystem.Register("SeasonSystemGump", AccessLevel.Administrator, SendGump);
        }

        public static void LoadEntries()
        {
            Entries = new List<SeasonalEventEntry>();

            Entries.Add(new SeasonalEventEntry(EventType.TreasuresOfTokuno, "Treasures of Tokuno", EventStatus.Inactive));
            Entries.Add(new SeasonalEventEntry(EventType.VirtueArtifacts, "Virtue Artifacts", EventStatus.Active));
            Entries.Add(new SeasonalEventEntry(EventType.TreasuresOfKotlCity, "Treasures of Kotl", EventStatus.Inactive, 10, 1, 60));
            Entries.Add(new SeasonalEventEntry(EventType.SorcerersDungeon, "Sorcerer's Dungeon", EventStatus.Seasonal, 10, 1, 60));
            Entries.Add(new SeasonalEventEntry(EventType.TreasuresOfDoom, "Treasures of Doom", EventStatus.Seasonal, 10, 1, 60));
            Entries.Add(new SeasonalEventEntry(EventType.TreasuresOfKhaldun, "Treasures of Khaldun", EventStatus.Seasonal, 10, 1, 60));
        }

        [Usage("SeasonSystemGump")]
        [Description("Displays a menu to configure various seasonal systems.")]
        public static void SendGump(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile)
            {
                BaseGump.SendGump(new SeasonalEventGump((PlayerMobile)e.Mobile));
            }
        }

        public static bool IsActive(EventType type)
        {
            var entry = GetEntry(type);

            if (entry != null)
            {
                return entry.IsActive();
            }

            return false;
        }

        public static SeasonalEventEntry GetEntry(EventType type)
        {
            return Entries.FirstOrDefault(e => e.EventType == type);
        }

        public static void OnToTDeactivated(Mobile from)
        {
            var entry = GetEntry(EventType.TreasuresOfTokuno);

            if (entry != null)
            {
                entry.Status = EventStatus.Inactive;

                if (from is PlayerMobile)
                {
                    BaseGump.SendGump(new SeasonalEventGump((PlayerMobile)from));
                }
            }
        }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write(0);

                    writer.Write(Entries.Count);

                    for(int i = 0; i < Entries.Count; i++)
                    {
                        writer.Write((int)Entries[i].EventType);
                        Entries[i].Serialize(writer);
                    }
                });
        }

        public static void OnLoad()
        {
            Persistence.Deserialize(
                FilePath,
                reader =>
                {
                    reader.ReadInt(); // version

                    int count = reader.ReadInt();

                    for (int i = 0; i < count; i++)
                    {
                        var entry = GetEntry((EventType)reader.ReadInt());
                        entry.Deserialize(reader);
                    }
                });
        }
	}

    [PropertyObject]
    public class SeasonalEventEntry
    {
        private EventStatus _Status;

        [CommandProperty(AccessLevel.Administrator)]
        public EventStatus Status
        {
            get
            {
                return _Status;
            }
            set
            {
                var old = _Status;

                _Status = value;

                if (old != _Status)
                {
                    OnStatusChange();
                }
            }
        }

        [CommandProperty(AccessLevel.Administrator)]
        public string Name { get; private set; }

        [CommandProperty(AccessLevel.Administrator)]
        public EventType EventType { get; private set; }

        [CommandProperty(AccessLevel.Administrator)]
        public int MonthStart { get; set; }

        [CommandProperty(AccessLevel.Administrator)]
        public int DayStart { get; set; }

        [CommandProperty(AccessLevel.Administrator)]
        public int Duration { get; set; }

        public SeasonalEventEntry(EventType type, string name, EventStatus status)
        {
            EventType = type;
            Name = name;
            _Status = status;
            MonthStart = 1;
            DayStart = 1;
            Duration = 365;
        }

        public SeasonalEventEntry(EventType type, string name, EventStatus status, int month, int day, int duration)
        {
            EventType = type;
            Name = name;
            _Status = status;
            MonthStart = month;
            DayStart = day;
            Duration = duration;
        }

        /// <summary>
        /// Dynamically checks if this event is active or not, based on time of year/override
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            // ToT uses its own system, this just reads it
            if (EventType == EventType.TreasuresOfTokuno)
            {
                return TreasuresOfTokuno.DropEra != TreasuresOfTokunoEra.None;
            }

            switch (Status)
            {
                default:
                    {
                        return false;
                    }
                case EventStatus.Active:
                    {
                        return true;
                    }
                case EventStatus.Seasonal:
                    {
                        if (Duration >= 365)
                            return true;

                        var now = DateTime.Now;
                        var starts = new DateTime(now.Year, MonthStart, DayStart, 0, 0, 0);

                        return now > starts && now < starts + TimeSpan.FromDays(Duration);
                    }
            }
        }

        public void OnStatusChange()
        {
            switch (EventType)
            {
                case EventType.TreasuresOfDoom:
                    TreasuresOfDoomGeneration.CheckEnabled();
                    break;
                case EventType.TreasuresOfKhaldun:
                    TreasuresOfKhaldunGeneration.CheckEnabled();
                    break;
                case EventType.SorcerersDungeon:
                    SorcerersDungeonGenerate.CheckEnabled();
                    break;
            }
        }

        public virtual void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write((int)_Status);
            writer.Write(MonthStart);
            writer.Write(DayStart);
            writer.Write(Duration);
        }

        public virtual void Deserialize(GenericReader reader)
        {
            reader.ReadInt(); // version

            _Status = (EventStatus)reader.ReadInt();

            MonthStart = reader.ReadInt();
            DayStart = reader.ReadInt();
            Duration = reader.ReadInt();
        }
    }
}
