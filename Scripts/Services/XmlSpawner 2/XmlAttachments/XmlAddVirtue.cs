using System;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
    public class XmlAddVirtue : XmlAttachment
    {
        private int m_DataValue;// default data
        private string m_Virtue;
        // a serial constructor is REQUIRED
        public XmlAddVirtue(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlAddVirtue(string virtue, int value)
        {
            this.Value = value;
            this.Virtue = virtue;
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
        public string Virtue
        {
            get
            {
                return this.m_Virtue;
            }
            set
            {
                this.m_Virtue = value;
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
            writer.Write(this.m_Virtue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            // version 0
            this.m_DataValue = reader.ReadInt();
            this.m_Virtue = reader.ReadString();
        }

        public override void OnAttach()
        {
            base.OnAttach();
		    
            // apply the mod
            if (this.AttachedTo is PlayerMobile)
            {
                // for players just add it immediately
                // lookup the virtue type
                VirtueName g = (VirtueName)0;
                bool valid = true;
                bool gainedPath = false;
                try
                {
                    g = (VirtueName)Enum.Parse(typeof(VirtueName), this.Virtue, true);
                }
                catch
                {
                    valid = false;
                }
                
                if (valid)
                {
                    VirtueHelper.Award((Mobile)this.AttachedTo, g, this.Value, ref gainedPath);

                    ((Mobile)this.AttachedTo).SendMessage("Receive {0}", this.OnIdentify((Mobile)this.AttachedTo));

                    if (gainedPath)
                    {
                        ((Mobile)this.AttachedTo).SendMessage("You have gained a path in {0}", this.Virtue);
                    }
                }
                else 
                {
                    ((Mobile)this.AttachedTo).SendMessage("{0}: no such Virtue", this.Virtue);
                }
                // and then remove the attachment
                Timer.DelayCall(TimeSpan.Zero, new TimerCallback(Delete));
                //Delete();
            }
            else if (this.AttachedTo is Item)
            {
                // dont allow item attachments
                this.Delete();
            }
        }

        public override void OnKilled(Mobile killed, Mobile killer)
        {
            base.OnKilled(killed, killer);

            if (killer == null)
                return;
		    
            VirtueName g = (VirtueName)0;
            bool valid = true;
            bool gainedPath = false;
            try
            {
                g = (VirtueName)Enum.Parse(typeof(VirtueName), this.Virtue, true);
            }
            catch
            {
                valid = false;
            }
            
            if (valid)
            {
                // give the killer the Virtue
                VirtueHelper.Award(killer, g, this.Value, ref gainedPath);

                if (gainedPath)
                {
                    killer.SendMessage("You have gained a path in {0}", this.Virtue);
                }

                killer.SendMessage("Receive {0}", this.OnIdentify(killer));
            }
        }

        public override string OnIdentify(Mobile from)
        {
            return String.Format("{0} {1} Virtue points", this.Value, this.Virtue);
        }
    }
}