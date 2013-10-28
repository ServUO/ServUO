using System;

namespace Server.Engines.XmlSpawner2
{
    public class XmlHue : XmlAttachment
    {
        private int m_Originalhue;
        private int m_Hue;
        // These are the various ways in which the message attachment can be constructed.  
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments

        // a serial constructor is REQUIRED
        public XmlHue(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlHue(int value)
        {
            this.m_Hue = value;
            this.Expiration = TimeSpan.FromSeconds(30.0);    // default 30 second duration
        }

        [Attachable]
        public XmlHue(int value, double duration)
        {
            this.m_Hue = value;
            this.Expiration = TimeSpan.FromMinutes(duration);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Hue
        {
            get
            {
                return this.m_Hue;
            }
            set
            {
                this.m_Hue = value;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            // version 0
            writer.Write(this.m_Originalhue);
            writer.Write(this.m_Hue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            // version 0

            this.m_Originalhue = reader.ReadInt();
            this.m_Hue = reader.ReadInt();
        }

        public override string OnIdentify(Mobile from)
        {
            base.OnIdentify(from);

            if (from == null || from.IsPlayer())
                return null;

            if (this.Expiration > TimeSpan.Zero)
            {
                return String.Format("Hue {0} expires in {1} mins", this.m_Hue, this.Expiration.TotalMinutes);
            }
            else
            {
                return String.Format("Hue {0}", this.m_Hue);
            }
        }

        public override void OnDelete()
        {
            base.OnDelete();

            // remove the mod
            if (this.AttachedTo is Mobile)
            {
                ((Mobile)this.AttachedTo).Hue = this.m_Originalhue;
            }
            else if (this.AttachedTo is Item)
            {
                ((Item)this.AttachedTo).Hue = this.m_Originalhue;
            }
        }

        public override void OnAttach()
        {
            base.OnAttach();

            // apply the mod
            if (this.AttachedTo is Mobile)
            {
                Mobile m = this.AttachedTo as Mobile;
                this.m_Originalhue = m.Hue;
                m.Hue = this.m_Hue;
            }
            else if (this.AttachedTo is Item)
            {
                Item i = this.AttachedTo as Item;
                this.m_Originalhue = i.Hue;
                i.Hue = this.m_Hue;
            }
            else
                this.Delete();
        }
    }
}