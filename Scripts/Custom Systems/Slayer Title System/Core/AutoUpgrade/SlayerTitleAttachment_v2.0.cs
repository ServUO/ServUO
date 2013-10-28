// Provided for backward compatability

using System;
using System.Collections;
using Server;
using Server.Engines.XmlSpawner2;

using CustomsFramework.Systems.SlayerTitleSystem;

namespace Custom.SlayerTitleSystem
{
    public class SlayerTitleAttachment : XmlAttachment
    {
        private Hashtable m_TitleEntries = new Hashtable();

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

            writer.Write((Int32)0);

            writer.Write((Int32)m_TitleEntries.Keys.Count);

            foreach (String title in m_TitleEntries.Keys)
            {
                writer.Write((String)title);
                writer.Write((Int32)m_TitleEntries[title]);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            Int32 version = reader.ReadInt();

            Int32 entryCount = reader.ReadInt();

            for (Int32 i = 0; i < entryCount; i++)
                m_TitleEntries[reader.ReadString()] = reader.ReadInt();

            if (Owner is Mobile)
            {
                Mobile player = (Mobile)Owner;

                SlayerModule module = player.GetModule(typeof(SlayerModule)) as SlayerModule;

                if (module == null)
                    module = new SlayerModule(player);

                foreach (String title in m_TitleEntries.Keys)
                    module.SetSlayerCount(title, (Int32)m_TitleEntries[title]);

                Delete();
            }
        }
    }
}