// Provided for backward compatability

using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Engines.XmlSpawner2;

using CustomsFramework.Systems.SlayerTitleSystem;

namespace Server.SlayerTitles
{
    public class SlayerTitleAttachment : XmlAttachment
    {
        private List<SlayerSystemTracker> m_SystemEntries = new List<SlayerSystemTracker>();

        // a serial constructor is REQUIRED
        public SlayerTitleAttachment(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public SlayerTitleAttachment()
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write((int)m_SystemEntries.Count);

            foreach (SlayerSystemTracker entry in m_SystemEntries)
            {
                writer.Write((string)entry.SystemName);
                writer.Write((int)entry.SlayerCount);
                writer.Write((string)entry.LastTitleAwarded);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            int entryCount = reader.ReadInt();

            for (int i = 0; i < entryCount; i++)
                m_SystemEntries.Add(new SlayerSystemTracker(reader.ReadString(), reader.ReadInt(), reader.ReadString()));

            if (Owner is Mobile)
            {
                Mobile player = (Mobile)Owner;

                SlayerModule module = player.GetModule(typeof(SlayerModule)) as SlayerModule;

                if (module == null)
                    module = new SlayerModule(player);

                foreach (SlayerSystemTracker entry in m_SystemEntries)
                    module.SetSlayerCount(entry.SystemName, entry.SlayerCount);

                Delete();
            }
        }

        private class SlayerSystemTracker
        {
            private string m_SystemName = "";
            public string SystemName { get { return m_SystemName; } set { m_SystemName = value; } }

            private int m_SlayerCount = 0;
            public int SlayerCount { get { return m_SlayerCount; } set { m_SlayerCount = value; } }

            private string m_LastTitleAwarded = null;
            public string LastTitleAwarded { get { return m_LastTitleAwarded; } set { m_LastTitleAwarded = value; } }

            public SlayerSystemTracker(string systemName, int slayerCount, string lastTitleAwarded)
            {
                m_SystemName = systemName;
                m_SlayerCount = slayerCount;
                m_LastTitleAwarded = lastTitleAwarded;
            }
        }
    }
}