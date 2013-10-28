using System;

namespace Server.Engines.XmlSpawner2
{
    public class XmlDate : XmlAttachment
    {
        private DateTime m_DataValue;
        // a serial constructor is REQUIRED
        public XmlDate(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlDate(string name)
        {
            this.Name = name;
            this.Date = DateTime.UtcNow;
        }

        [Attachable]
        public XmlDate(string name, double expiresin)
        {
            this.Name = name;
            this.Date = DateTime.UtcNow;
            this.Expiration = TimeSpan.FromMinutes(expiresin);
        }

        [Attachable]
        public XmlDate(string name, DateTime value, double expiresin)
        {
            this.Name = name;
            this.Date = value;
            this.Expiration = TimeSpan.FromMinutes(expiresin);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Date
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
        // These are the various ways in which the message attachment can be constructed.  
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            // version 0
            writer.Write(this.m_DataValue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            // version 0
            this.m_DataValue = reader.ReadDateTime();
        }

        public override string OnIdentify(Mobile from)
        {
            if (from == null || from.IsPlayer())
                return null;

            if (this.Expiration > TimeSpan.Zero)
            {
                return String.Format("{2}: Date {0} expires in {1} mins", this.Date, this.Expiration.TotalMinutes, this.Name);
            }
            else
            {
                return String.Format("{1}: Date {0}", this.Date, this.Name);
            }
        }
    }
}