using System;
using Server;
using System.Collections.Generic;
using Server.Items;
using Server.Gumps;
using System.IO;

namespace Server.Items
{
    public class ReforgingContext
    {
        public Dictionary<BaseTool, ReforgingOption> Contexts { get; set; }

        public ReforgedPrefix Prefix { get; set; }
        public ReforgedSuffix Suffix { get; set; }

        public ReforgingContext(Mobile m)
        {
            Contexts = new Dictionary<BaseTool, ReforgingOption>();

            ReforgingContexts[m] = this;
        }

        public ReforgingContext(GenericReader reader)
        {
            Contexts = new Dictionary<BaseTool, ReforgingOption>();

            int version = reader.ReadInt();

            Prefix = (ReforgedPrefix)reader.ReadInt();
            Suffix = (ReforgedSuffix)reader.ReadInt();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                BaseTool tool = reader.ReadItem() as BaseTool;
                ReforgingOption option = (ReforgingOption)reader.ReadInt();

                if (tool != null)
                    Contexts[tool] = option;
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write((int)Prefix);
            writer.Write((int)Suffix);

            writer.Write(Contexts.Count);
            foreach (var kvp in Contexts)
            {
                writer.Write(kvp.Key);
                writer.Write((int)kvp.Value);
            }
        }

        #region Serialize/Deserialize Persistence
        private static string FilePath = Path.Combine("Saves", "CraftContext", "ReforgingContexts.bin");

        public static Dictionary<Mobile, ReforgingContext> ReforgingContexts { get; set; }

        public static ReforgingContext GetContext(Mobile m)
        {
            if (ReforgingContexts.ContainsKey(m))
            {
                return ReforgingContexts[m];
            }

            return new ReforgingContext(m);
        }

        public static void Configure()
        {
            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;
        }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write(0); // version

                    writer.Write(ReforgingContexts.Count);

                    foreach (var kvp in ReforgingContexts)
                    {
                        writer.Write(kvp.Key);
                        kvp.Value.Serialize(writer);
                    }
                });
        }

        public static void OnLoad()
        {
            ReforgingContexts = new Dictionary<Mobile, ReforgingContext>();

            Persistence.Deserialize(
                FilePath,
                reader =>
                {
                    int version = reader.ReadInt();

                    int count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        Mobile m = reader.ReadMobile();
                        var context = new ReforgingContext(reader);

                        if (m != null)
                            ReforgingContexts[m] = context;
                    }
                });
        }
        #endregion
    }
}
