using System;
using System.Collections.Generic;

namespace Server.Engines.XmlSpawner2
{
    public class XmlFactionEquip : XmlAttachment
    {
        private int m_DataValue;// default data
        private string m_GroupName;
        private XmlMobFactions.GroupTypes m_GroupType = XmlMobFactions.GroupTypes.End_Unused;
        // a serial constructor is REQUIRED
        public XmlFactionEquip(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlFactionEquip(string factiontype, int minvalue)
        {
            this.MinValue = minvalue;
            this.FactionType = factiontype;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MinValue
        {
            get
            {
                return this.m_DataValue;
            }
            set
            {
                this.m_DataValue = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string FactionType
        {
            get
            {
                return this.m_GroupName;
            }
            set
            { 
                this.m_GroupName = value; 
                try
                {
                    this.m_GroupType = (XmlMobFactions.GroupTypes)Enum.Parse(typeof(XmlMobFactions.GroupTypes), this.m_GroupName, true);
                }
                catch
                {
                }
            }
        }
        // These are the various ways in which the message attachment can be constructed.
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            // version 0
            writer.Write(this.m_DataValue);
            writer.Write(this.m_GroupName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            // version 0
            this.m_DataValue = reader.ReadInt();
            this.FactionType = reader.ReadString();
        }

        public override bool CanEquip(Mobile from)
        {
            // check to see if the owners faction is sufficient
            List<XmlAttachment> list = XmlAttach.FindAttachments(XmlAttach.MobileAttachments, from, typeof(XmlMobFactions), "Standard");
            if (list != null && list.Count > 0)
            {
                int faclevel = 0;

                XmlMobFactions x = list[0] as XmlMobFactions;

                if (this.m_GroupType != XmlMobFactions.GroupTypes.End_Unused)
                {
                    faclevel = x.GetFactionLevel(this.m_GroupType);
                }

                if (faclevel < this.MinValue && this.AttachedTo is Item && this.AttachedTo != null)
                {
                    Item item = this.AttachedTo as Item;
                    string name = item.Name;
                    if (name == null)
                    {
                        name = item.ItemData.Name;
                    }
                    from.SendMessage("Cannot equip {2}. Need {0} {1} faction.", this.MinValue, this.FactionType, name);
                    return false;
                }
            }

            return true;
        }

        public override string OnIdentify(Mobile from)
        {
            return String.Format("Requires {0} {1} faction to equip", this.MinValue, this.FactionType);
        }
    }
}