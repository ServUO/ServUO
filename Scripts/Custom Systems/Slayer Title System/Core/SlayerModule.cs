using System;
using System.Collections;

using Server;
using Server.Gumps;
using Server.Mobiles;

namespace CustomsFramework.Systems.SlayerTitleSystem
{
    public class SlayerModule : BaseModule
    {
        private Hashtable m_TitleEntries = new Hashtable();

        private Boolean m_Enabled;
        public Boolean Enabled { get { return m_Enabled; } set { m_Enabled = value; } }

        public SlayerModule(Mobile from)
            : base()
        {
            this.Enabled = true;
            this.LinkMobile(from);
        }

        public SlayerModule(CustomSerial serial)
            : base(serial)
        {
        }

        public override String Name
        {
            get
            {
                if (this.LinkedMobile != null)
                    return String.Format("Slayer Title Module - {0}", this.LinkedMobile.Name);
                else
                    return "Unlinked Slayer Title Module";
            }
        }

        public override String Description
        {
            get
            {
                if (this.LinkedMobile != null)
                    return String.Format(@"Slayer Title Module that is linked to {0}", this.LinkedMobile.Name);
                else
                    return "Unlinked Slayer Title Module";
            }
        }

        public override String Version
        {
            get
            {
                return SlayerTitleCore.SystemVersion;
            }
        }

        public override AccessLevel EditLevel
        {
            get
            {
                return AccessLevel.Developer;
            }
        }

        public override Gump SettingsGump
        {
            get
            {
                return base.SettingsGump;
            }
        }

        public override void Prep()
        {
            base.Prep();
        }

        public void IncrementCounter(String titleName)
        {
            if (m_TitleEntries.ContainsKey(titleName))
                m_TitleEntries[titleName] = (Int32)m_TitleEntries[titleName] + 1;
            else
                m_TitleEntries[titleName] = 1;
        }

        public Int32 GetSlayerCount(String titleName)
        {
            if (m_TitleEntries.ContainsKey(titleName))
                return (Int32)m_TitleEntries[titleName];

            return 0;
        }

        public void SetSlayerCount(String titleName, Int32 count)
        {
            m_TitleEntries[titleName] = count;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            Utilities.WriteVersion(writer, 0);

            // Version 0
            writer.Write((Boolean)m_Enabled);
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

            switch (version)
            {
                case 0:
                    // Version 0
                    m_Enabled = reader.ReadBool();

                    Int32 entryCount = reader.ReadInt();

                    for (Int32 i = 0; i < entryCount; i++)
                        m_TitleEntries[reader.ReadString()] = reader.ReadInt();

                    break;
            }
        }
    }
}
