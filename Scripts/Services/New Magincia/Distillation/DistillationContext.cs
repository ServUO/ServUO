using Server.Items;
using System.Collections.Generic;
using System.IO;

namespace Server.Engines.Distillation
{
    public class DistillationContext
    {
        private Group m_LastGroup;
        private Liquor m_LastLiquor;
        private readonly Yeast[] m_SelectedYeast = new Yeast[4];
        private bool m_MakeStrong;
        private bool m_Mark;
        private string m_Label;

        public Group LastGroup { get { return m_LastGroup; } set { m_LastGroup = value; } }
        public Liquor LastLiquor { get { return m_LastLiquor; } set { m_LastLiquor = value; } }
        public Yeast[] SelectedYeast => m_SelectedYeast;
        public bool MakeStrong { get { return m_MakeStrong; } set { m_MakeStrong = value; } }
        public bool Mark { get { return m_Mark; } set { m_Mark = value; } }
        public string Label { get { return m_Label; } set { m_Label = value; } }

        public DistillationContext()
        {
            m_LastGroup = Group.WheatBased;
            m_LastLiquor = Liquor.None;
            m_MakeStrong = false;
            m_Mark = true;
            m_Label = null;
        }

        public bool YeastInUse(Yeast yeast)
        {
            foreach (Yeast y in m_SelectedYeast)
            {
                if (y != null && y == yeast)
                    return true;
            }

            return false;
        }

        public void ClearYeasts()
        {
            for (int i = 0; i < m_SelectedYeast.Length; i++)
            {
                m_SelectedYeast[i] = null;
            }
        }

        public DistillationContext(GenericReader reader)
        {
            int version = reader.ReadInt();

            m_LastGroup = (Group)reader.ReadInt();
            m_LastLiquor = (Liquor)reader.ReadInt();
            m_MakeStrong = reader.ReadBool();
            m_Mark = reader.ReadBool();
            m_Label = reader.ReadString();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write((int)m_LastGroup);
            writer.Write((int)m_LastLiquor);
            writer.Write(m_MakeStrong);
            writer.Write(m_Mark);
            writer.Write(m_Label);
        }

        #region Serialize/Deserialize Persistence
        private static readonly string FilePath = Path.Combine("Saves", "CraftContext", "DistillationContexts.bin");

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

                    writer.Write(DistillationSystem.Contexts.Count);

                    foreach (KeyValuePair<Mobile, DistillationContext> kvp in DistillationSystem.Contexts)
                    {
                        writer.Write(kvp.Key);
                        kvp.Value.Serialize(writer);
                    }
                });
        }

        public static void OnLoad()
        {
            Persistence.Deserialize(
                FilePath,
                reader =>
                {
                    int version = reader.ReadInt();

                    int count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        Mobile m = reader.ReadMobile();
                        DistillationContext context = new DistillationContext(reader);

                        if (m != null)
                            DistillationSystem.Contexts[m] = context;
                    }
                });
        }
        #endregion
    }
}