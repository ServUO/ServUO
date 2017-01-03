using Server;
using System;
using Server.Engines.Quests;
using Server.Mobiles;
using Server.Regions;
using Server.Commands;
using System.Collections.Generic;
using System.IO;

namespace Server.Items
{
    public class HighSeasPersistance
    {
        public static string FilePath = Path.Combine("Saves", "Highseas.bin");
        public static bool DefaultRestrictBoats = true;

        public static void Initialize()
        {
            if (Core.HS)
            {
                Region reg = new TokunoDocksRegion();
                reg.Register();

                SeaMarketRegion reg1 = new SeaMarketRegion(Map.Felucca);
                SeaMarketRegion reg2 = new SeaMarketRegion(Map.Trammel);

                reg1.Register();
                reg2.Register();

                SeaMarketRegion.SetRegions(reg1, reg2);

                CommandSystem.Register("RestrictBoats", AccessLevel.GameMaster, new CommandEventHandler(SeaMarketRegion.SetRestriction_OnCommand));
            }
        }

        public static void Configure()
        {
            if (Core.HS)
            {
                EventSink.WorldSave += OnSave;
                EventSink.WorldLoad += OnLoad;

                SeaMarketRegion.RestrictBoats = DefaultRestrictBoats;

                m_Instance = new HighSeasPersistance();
            }
        }

        private static HighSeasPersistance m_Instance;
        public static HighSeasPersistance Instance { get { return m_Instance; } }

        public bool HighSeasActive { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public CharydbisSpawner CharydbisSpawner { get { return CharydbisSpawner.SpawnInstance; } set { } }

        public HighSeasPersistance()
        {
        }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write((int)1);

                    Server.Regions.SeaMarketRegion.Save(writer);

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

                    Server.Regions.SeaMarketRegion.Load(reader);
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