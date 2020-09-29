#region References
using System.IO;
#endregion

namespace Server.Engines.Help
{
    public static class HelpPersistence
    {
        private static readonly string _FilePath = Path.Combine("Saves", "Help", "Pages.bin");


        [CallPriority(900)]
        public static void Configure()
        {
            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;
        }

        private static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                _FilePath,
                writer =>
                {
                    writer.Write(1); // version

                    writer.Write(ResponseEntry.Entries.Count);

                    foreach (ResponseEntry entry in ResponseEntry.Entries)
                        entry.Serialize(writer);

                    writer.Write(PageQueue.List.Count);

                    foreach (PageEntry pe in PageQueue.List)
                    {
                        writer.Write(pe.Sender);
                        writer.Write(pe.Message);
                        writer.Write((int)pe.Type);
                        writer.Write(pe.Handler);
                        writer.Write(pe.Sent);
                        writer.Write(pe.PageLocation);
                        writer.Write(pe.PageMap);

                    }
                });
        }

        private static void OnLoad()
        {
            Persistence.Deserialize(
                _FilePath,
                reader =>
                {
                    int version = reader.ReadInt();

                    switch (version)
                    {
                        case 1:
                            {
                                int c = reader.ReadInt();

                                for (int i = 0; i < c; ++i)
                                {
                                    new ResponseEntry(reader);
                                }
                            }
                            goto case 0;
                        case 0:
                            {
                                int count = reader.ReadInt();

                                for (int i = 0; i < count; ++i)
                                {
                                    Mobile sender = reader.ReadMobile();
                                    string message = reader.ReadString();
                                    PageType type = (PageType)reader.ReadInt();
                                    PageEntry pe = new PageEntry(sender, message, type)
                                    {
                                        Handler = reader.ReadMobile(),
                                        Sent = reader.ReadDateTime(),
                                        PageLocation = reader.ReadPoint3D(),
                                        PageMap = reader.ReadMap()
                                    };
                                    pe.Stop();

                                    PageQueue.Enqueue(pe);
                                }
                            }
                            break;
                    }
                });
        }
    }
}
