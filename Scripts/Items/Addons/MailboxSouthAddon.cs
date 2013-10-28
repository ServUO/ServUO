using System;

namespace Server.Items
{
    public class MailboxSouthAddon : BaseAddonContainer
    {
        private Mobile m_Owner;
        [Constructable]
        public MailboxSouthAddon()
            : base(0x4142)
        {
            //AddComponent( new LocalizedContainerComponent( 1113927 ), -1, 0, 0 );
        }

        public MailboxSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonContainerDeed Deed
        {
            get
            {
                return new MailboxSouthAddonDeed();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1113927;
            }
        }// Mailbox (south)
        public override bool RetainDeedHue
        {
            get
            {
                return true;
            }
        }
        public override int DefaultGumpID
        {
            get
            {
                return 0x11A;
            }
        }
        public override int DefaultDropSound
        {
            get
            {
                return 0x42;
            }
        }
        public Mobile Owner
        {
            get
            {
                return this.m_Owner;
            }
            set
            {
                this.m_Owner = value;
                this.InvalidateProperties();
            }
        }
        //public override void OnTelekinesis(Mobile from)
        //{
        //return;
        //}
        public override void OnDoubleClick(Mobile from)
        {
            if (this.Owner == null)// && IsChildOf(from.Backpack))
            {
                this.Owner = from;
                from.SendMessage("You now own this mailbox!");
                base.OnDoubleClick(from);
            }
            else if (this.Owner == null)
            {
                from.SendMessage("You can only set your self as owner of this mailbox if it is in your house!");
            }
            else if (!this.IsSecure && this.IsAccessibleTo(from))
                base.OnDoubleClick(from);
            else if (this.Owner == from || from.AccessLevel > this.Owner.AccessLevel)
                base.OnDoubleClick(from);
            else
                from.SendMessage("This is not your mailbox, you must be the owner to open it!");
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1072304, this.Owner == null ? "nobody" : this.Owner.Name);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version
            writer.Write(this.m_Owner != null);
            if (this.m_Owner != null)
                writer.Write(this.m_Owner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
            if (reader.ReadBool())
                this.m_Owner = reader.ReadMobile();
        }
    }

    public class MailboxSouthAddonDeed : BaseAddonContainerDeed
    {
        [Constructable]
        public MailboxSouthAddonDeed()
            : base()
        {
        }

        public MailboxSouthAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonContainer Addon
        {
            get
            {
                return new MailboxSouthAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1113927;
            }
        }// Mailbox (south)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}