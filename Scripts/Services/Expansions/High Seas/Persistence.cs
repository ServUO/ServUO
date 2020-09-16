using Server.Commands;
using Server.Engines.Quests;
using Server.Mobiles;
using Server.Regions;
using System.IO;

namespace Server.Items
{
    public class HighSeasPersistance
    {
        public static string FilePath = Path.Combine("Saves", "Highseas.bin");
        public static bool DefaultRestrictBoats = true;

        public static void Configure()
        {
            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;

            SeaMarketRegion.RestrictBoats = DefaultRestrictBoats;

            m_Instance = new HighSeasPersistance();

            CommandSystem.Register("RestrictBoats", AccessLevel.GameMaster, SeaMarketRegion.SetRestriction_OnCommand);
        }

        private static HighSeasPersistance m_Instance;
        public static HighSeasPersistance Instance => m_Instance;

        public bool HighSeasActive { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public CharydbisSpawner CharydbisSpawner { get { return CharydbisSpawner.SpawnInstance; } set { } }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write(1);

                    SeaMarketRegion.Save(writer);

                    writer.Write(PlayerFishingEntry.FishingEntries.Count);

                    foreach (PlayerFishingEntry entry in PlayerFishingEntry.FishingEntries.Values)
                        entry.Serialize(writer);

                    if (CharydbisSpawner.SpawnInstance != null)
                    {
                        writer.Write(0);
                        CharydbisSpawner.SpawnInstance.Serialize(writer);
                    }
                    else
                        writer.Write(1);

                    ForgedPardon.Save(writer);
                });
        }

        public static void OnLoad()
        {
            Persistence.Deserialize(
                FilePath,
                reader =>
                {
                    int version = reader.ReadInt();

                    SeaMarketRegion.Load(reader);
                    int count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                        new PlayerFishingEntry(reader);

                    if (version == 0 || reader.ReadInt() == 0)
                    {
                        CharydbisSpawner.SpawnInstance = new CharydbisSpawner();
                        CharydbisSpawner.SpawnInstance.Deserialize(reader);
                    }

                    ForgedPardon.Load(reader);
                });
        }
    }
}
