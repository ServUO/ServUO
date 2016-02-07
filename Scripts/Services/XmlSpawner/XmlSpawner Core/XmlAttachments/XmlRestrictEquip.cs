using System;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
    public class XmlRestrictEquip : XmlAttachment
    {
        private string m_TestValue = null;// default Test condition
        private string m_FailMsg = null;// message given when equipping fails
        private string m_PropertyListString = null;// string displayed in the properties list

        // a serial constructor is REQUIRED
        public XmlRestrictEquip(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlRestrictEquip()
        {
            this.Test = String.Empty;
        }

        [Attachable]
        public XmlRestrictEquip(string name)
        {
            this.Name = name;
            this.Test = String.Empty;
        }

        [Attachable]
        public XmlRestrictEquip(string name, string test)
        {
            this.Name = name;
            this.Test = test;
        }

        [Attachable]
        public XmlRestrictEquip(string name, string test, double expiresin)
        {
            this.Name = name;
            this.Test = test;
            this.Expiration = TimeSpan.FromMinutes(expiresin);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Test
        {
            get
            {
                return this.m_TestValue;
            }
            set
            {
                this.m_TestValue = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string FailMsg
        {
            get
            {
                return this.m_FailMsg;
            }
            set
            {
                this.m_FailMsg = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string PropertyListString
        {
            get
            {
                return this.m_PropertyListString;
            }
            set
            {
                this.m_PropertyListString = value;
                this.InvalidateParentProperties();
            }
        }
        // These are the various ways in which the message attachment can be constructed.  
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments
        public override bool CanEquip(Mobile from)
        {
            if (from == null)
                return false;

            bool allowequip = true;

            // test the condition if there is one
            if (this.Test != null && this.Test.Length > 0)
            {
                string status_str;

                allowequip = BaseXmlSpawner.CheckPropertyString(null, this.AttachedTo, this.Test, from, out status_str);

                if (!allowequip && this.FailMsg != null)
                {
                    from.SendMessage(this.FailMsg);
                }
            }

            return allowequip;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);
            // version 1
            writer.Write(this.m_PropertyListString);
            writer.Write(this.m_FailMsg);
            // version 0
            writer.Write(this.m_TestValue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 1:
                    this.m_PropertyListString = reader.ReadString();
                    this.m_FailMsg = reader.ReadString();
                    goto case 0;
                case 0:
                    this.m_TestValue = reader.ReadString();
                    break;
            }
        }

        public override string DisplayedProperties(Mobile from)
        {
            return this.PropertyListString;
        }

        public override string OnIdentify(Mobile from)
        {
            if (from == null || from.AccessLevel < AccessLevel.Counselor)
                return null;

            if (this.Expiration > TimeSpan.Zero)
            {
                return String.Format("{0}: RestrictEquip '{1}' expires in {2} mins", this.Name, this.Test, this.Expiration.TotalMinutes);
            }
            else
            {
                return String.Format("{0}: RestrictEquip '{1}'", this.Name, this.Test);
            }
        }
    }
}