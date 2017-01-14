using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
    public class XmlAddFaction : XmlAttachment
    {
        private int m_DataValue;// default data
        private string m_GroupName;
        // a serial constructor is REQUIRED
        public XmlAddFaction(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlAddFaction(string factiontype, int value)
        {
            this.Value = value;
            this.FactionType = factiontype;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Value
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
        public string FactionType
        {
            get
            {
                return this.m_GroupName;
            }
            set
            {
                this.m_GroupName = value;
            }
        }
        // These are the various ways in which the message attachment can be constructed.  
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments
        public override bool HandlesOnKilled
        {
            get
            {
                return true;
            }
        }
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
            this.m_GroupName = reader.ReadString();
        }

        public override void OnAttach()
        {
            base.OnAttach();
		    
            // apply the mod
            if (this.AttachedTo is PlayerMobile)
            {
                // for players just add it immediately
                // lookup the group type
                XmlMobFactions.GroupTypes g = XmlMobFactions.GroupTypes.End_Unused;
                try
                {
                    g = (XmlMobFactions.GroupTypes)Enum.Parse(typeof(XmlMobFactions.GroupTypes), this.FactionType, true);
                }
                catch
                {
                }
                
                if (g != XmlMobFactions.GroupTypes.End_Unused)
                {
                    // get XmlMobFaction type attachments and add the faction
                    List<XmlAttachment> list = XmlAttach.FindAttachments(this.AttachedTo, typeof(XmlMobFactions));
                    if (list != null && list.Count > 0)
                    {
                        foreach (XmlMobFactions x in list)
                        {
                            x.SetFactionLevel(g, x.GetFactionLevel(g) + this.Value);
                        }
                    }

                    ((Mobile)this.AttachedTo).SendMessage("Receive {0}", this.OnIdentify((Mobile)this.AttachedTo));
                }
                else 
                {
                    ((Mobile)this.AttachedTo).SendMessage("{0}: no such faction", this.FactionType);
                }
                // and then remove the attachment
                this.Delete();
            }
            else if (this.AttachedTo is Item)
            {
                // dont allow item attachments
                this.Delete();
            }
        }

        public override void OnKilled(Mobile killed, Mobile killer)
        {
            base.OnKill(killed, killer);

            if (killer == null)
                return;
		    
            XmlMobFactions.GroupTypes g = XmlMobFactions.GroupTypes.End_Unused;
            try
            {
                g = (XmlMobFactions.GroupTypes)Enum.Parse(typeof(XmlMobFactions.GroupTypes), this.FactionType, true);
            }
            catch
            {
            }
            
            if (g != XmlMobFactions.GroupTypes.End_Unused)
            {
                // give the killer the faction
                // get XmlMobFaction type attachments and add the faction
                List<XmlAttachment> list = XmlAttach.FindAttachments(killer, typeof(XmlMobFactions));
                if (list != null && list.Count > 0)
                {
                    foreach (XmlMobFactions x in list)
                    {
                        x.SetFactionLevel(g, x.GetFactionLevel(g) + this.Value);
                    }
                }

                killer.SendMessage("Receive {0}", this.OnIdentify(killer));
            }
        }

        public override string OnIdentify(Mobile from)
        {
            return String.Format("{0} {1} Faction", this.Value, this.FactionType);
        }
    }
}