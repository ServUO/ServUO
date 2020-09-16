using Server.Engines.Plants;
using Server.Items;

using System.Collections.Generic;
using System.IO;

namespace Server.Engines.Craft
{
    public enum CraftMarkOption
    {
        MarkItem,
        DoNotMark,
        PromptForMark
    }

    public enum CraftQuestOption
    {
        QuestItem,
        NonQuestItem
    }

    public class CraftContext
    {
        public Mobile Owner { get; private set; }
        public CraftSystem System { get; private set; }

        public int LastResourceIndex { get; set; }
        public int LastResourceIndex2 { get; set; }
        public int LastGroupIndex { get; set; }
        public bool DoNotColor { get; set; }
        public CraftMarkOption MarkOption { get; set; }
        public CraftQuestOption QuestOption { get; set; }

        public List<CraftItem> Items { get; set; }

        public int MakeTotal { get; set; }

        public PlantHue RequiredPlantHue { get; set; }
        public PlantPigmentHue RequiredPigmentHue { get; set; }

        public CraftContext(Mobile owner, CraftSystem system)
        {
            Owner = owner;
            System = system;

            Items = new List<CraftItem>();
            LastResourceIndex = -1;
            LastResourceIndex2 = -1;
            LastGroupIndex = -1;

            QuestOption = CraftQuestOption.NonQuestItem;
            RequiredPlantHue = PlantHue.None;
            RequiredPigmentHue = PlantPigmentHue.None;

            Contexts.Add(this);
        }

        public CraftItem LastMade
        {
            get
            {
                if (Items.Count > 0)
                    return Items[0];

                return null;
            }
        }

        public void OnMade(CraftItem item)
        {
            Items.Remove(item);

            if (Items.Count == 10)
                Items.RemoveAt(9);

            Items.Insert(0, item);
        }

        public virtual void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(Owner);
            writer.Write(GetSystemIndex(System));
            writer.Write(LastResourceIndex);
            writer.Write(LastResourceIndex2);
            writer.Write(LastGroupIndex);
            writer.Write(DoNotColor);
            writer.Write((int)MarkOption);
            writer.Write((int)QuestOption);

            writer.Write(MakeTotal);
        }

        public CraftContext(GenericReader reader)
        {
            int version = reader.ReadInt();

            Items = new List<CraftItem>();

            Owner = reader.ReadMobile();
            int sysIndex = reader.ReadInt();
            LastResourceIndex = reader.ReadInt();
            LastResourceIndex2 = reader.ReadInt();
            LastGroupIndex = reader.ReadInt();
            DoNotColor = reader.ReadBool();
            MarkOption = (CraftMarkOption)reader.ReadInt();
            QuestOption = (CraftQuestOption)reader.ReadInt();

            MakeTotal = reader.ReadInt();
            System = GetCraftSystem(sysIndex);

            if (System != null && Owner != null)
            {
                System.AddContext(Owner, this);
            }
        }

        public int GetSystemIndex(CraftSystem system)
        {
            for (int i = 0; i < _Systems.Length; i++)
            {
                if (_Systems[i] == system)
                    return i;
            }

            return -1;
        }

        public CraftSystem GetCraftSystem(int i)
        {
            if (i >= 0 && i < _Systems.Length)
                return _Systems[i];

            return null;
        }

        #region Serialize/Deserialize Persistence
        private static readonly string FilePath = Path.Combine("Saves", "CraftContext", "Contexts.bin");

        private static readonly List<CraftContext> Contexts = new List<CraftContext>();
        private static readonly Dictionary<Mobile, AnvilOfArtifactsEntry> AnvilEntries = new Dictionary<Mobile, AnvilOfArtifactsEntry>();

        public static AnvilOfArtifactsEntry GetAnvilEntry(Mobile m)
        {
            return GetAnvilEntry(m, true);
        }

        public static AnvilOfArtifactsEntry GetAnvilEntry(Mobile m, bool create)
        {
            if (AnvilEntries.ContainsKey(m))
            {
                return AnvilEntries[m];
            }

            if (create)
            {
                var entry = new AnvilOfArtifactsEntry();

                AnvilEntries[m] = entry;

                return entry;
            }

            return null;
        }

        public static bool IsAnvilReady(Mobile m)
        {
            var entry = GetAnvilEntry(m, false);

            if (entry != null)
            {
                return entry.Ready;
            }

            return false;
        }

        public static CraftSystem[] Systems => _Systems;
        private static readonly CraftSystem[] _Systems = new CraftSystem[11];

        public static void Configure()
        {
            _Systems[0] = DefAlchemy.CraftSystem;
            _Systems[1] = DefBlacksmithy.CraftSystem;
            _Systems[2] = DefBowFletching.CraftSystem;
            _Systems[3] = DefCarpentry.CraftSystem;
            _Systems[4] = DefCartography.CraftSystem;
            _Systems[5] = DefCooking.CraftSystem;
            _Systems[6] = DefGlassblowing.CraftSystem;
            _Systems[7] = DefInscription.CraftSystem;
            _Systems[8] = DefMasonry.CraftSystem;
            _Systems[9] = DefTailoring.CraftSystem;
            _Systems[10] = DefTinkering.CraftSystem;

            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;
        }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write(1); // version

                    writer.Write(AnvilEntries.Count);
                    foreach (var kvp in AnvilEntries)
                    {
                        writer.Write(kvp.Key);
                        kvp.Value.Serialize(writer);
                    }

                    writer.Write(Contexts.Count);
                    Contexts.ForEach(c => c.Serialize(writer));
                });
        }

        public static void OnLoad()
        {
            Persistence.Deserialize(
                FilePath,
                reader =>
                {
                    int version = reader.ReadInt();
                    int count = 0;

                    switch (version)
                    {
                        case 1:
                            count = reader.ReadInt();
                            for (int i = 0; i < count; i++)
                            {
                                var m = reader.ReadMobile();

                                var anvilEntry = new AnvilOfArtifactsEntry();
                                anvilEntry.Deserialize(reader);

                                if (m != null)
                                {
                                    AnvilEntries[m] = anvilEntry;
                                }
                            }
                            goto case 0;
                        case 0:

                            count = reader.ReadInt();
                            for (int i = 0; i < count; i++)
                            {
                                var context = new CraftContext(reader);
                                Contexts.Add(context);
                            }
                            break;
                    }
                });
        }
        #endregion
    }

    public class AnvilOfArtifactsEntry
    {
        private AnvilofArtifactsAddon _Anvil;

        public Dictionary<ResistanceType, int> Exceptional { get; set; }
        public Dictionary<ResistanceType, int> Runic { get; set; }
        public AnvilofArtifactsAddon Anvil
        {
            get { return _Anvil; }
            set
            {
                _Anvil = value;

                if (_Anvil != null)
                {
                    Ready = true;
                }
                else
                {
                    Ready = false;
                }
            }
        }

        public bool Ready { get; set; }

        public AnvilOfArtifactsEntry()
        {
            Exceptional = CreateArray();
            Runic = CreateArray();

            Ready = false;
        }

        public void Clear(Mobile m)
        {
            var gump = m.FindGump<AnvilofArtifactsGump>();

            if (gump != null)
            {
                gump.Refresh();
            }

            Anvil = null;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(Exceptional.Count);

            foreach (var kvp in Exceptional)
            {
                writer.Write((int)kvp.Key);
                writer.Write(kvp.Value);
            }

            writer.Write(Runic.Count);

            foreach (var kvp in Runic)
            {
                writer.Write((int)kvp.Key);
                writer.Write(kvp.Value);
            }

            writer.Write(Ready);
            writer.WriteItem(_Anvil);
        }

        public void Deserialize(GenericReader reader)
        {
            reader.ReadInt();

            var count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                Exceptional[(ResistanceType)reader.ReadInt()] = reader.ReadInt();
            }

            count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                Runic[(ResistanceType)reader.ReadInt()] = reader.ReadInt();
            }

            Ready = reader.ReadBool();
            _Anvil = reader.ReadItem<AnvilofArtifactsAddon>();
        }

        public static Dictionary<ResistanceType, int> CreateArray()
        {
            return new Dictionary<ResistanceType, int>
                {
                    { ResistanceType.Physical, 0 },
                    { ResistanceType.Fire, 0 },
                    { ResistanceType.Cold, 0 },
                    { ResistanceType.Poison, 0 },
                    { ResistanceType.Energy, 0 },
                };
        }
    }
}
