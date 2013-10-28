using System;
using Server.Items;

namespace Server.Engines.XmlSpawner2
{
    public class XmlSaveItem : XmlAttachment
    {
        private Item m_SavedItem;
        private Container m_Container;
        private Mobile m_WasOwnedBy;
        // a serial constructor is REQUIRED
        public XmlSaveItem(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlSaveItem()
        {
            this.m_Container = new SaveItemPack();
        }

        [Attachable]
        public XmlSaveItem(string name)
        {
            this.Name = name;
        }

        public XmlSaveItem(string name, Item saveditem)
        {
            this.Name = name;
            this.SavedItem = saveditem;
        }

        public XmlSaveItem(string name, Item saveditem, Mobile wasownedby)
        {
            this.Name = name;
            this.SavedItem = saveditem;
            this.WasOwnedBy = wasownedby;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Container Container
        {
            get
            {
                return this.m_Container;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Item SavedItem 
        { 
            get
            { 
                // if the item has been moved off of the internal map, then forget about it
                if (this.m_SavedItem != null && (this.m_SavedItem.Parent != this.m_Container || this.m_SavedItem.Deleted))
                {
                    this.m_WasOwnedBy = null;
                    this.m_SavedItem = null;
                }

                return this.m_SavedItem; 
            }
            set 
            { 
                // delete any existing item before assigning a new value
                if (this.SavedItem != null)
                {
                    this.SafeItemDelete(this.m_SavedItem);
                    //m_SavedItem.Delete();
                    this.m_SavedItem = null;
                }

                // dont allow saving the item if it is attached to it
                if (value != this.AttachedTo)
                {
                    this.m_SavedItem = value; 
                }

                // automatically internalize any saved item
                if (this.m_SavedItem != null)
                {
                    this.AddToContainer(this.m_SavedItem);
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool RestoreItem
        { 
            get
            {
                return false;
            }
            set 
            { 
                if (value == true && this.SavedItem != null && this.AttachedTo is IEntity && ((IEntity)this.AttachedTo).Map != Map.Internal && ((IEntity)this.AttachedTo).Map != null)
                {
                    // move the item to the location of the object the attachment is attached to
                    if (this.AttachedTo is Item)
                    {
                        this.m_SavedItem.Map = ((Item)this.AttachedTo).Map;
                        this.m_SavedItem.Location = ((Item)this.AttachedTo).Location;
                        this.m_SavedItem.Parent = ((Item)this.AttachedTo).Parent;
                    }
                    else if (this.AttachedTo is Mobile)
                    {
                        this.m_SavedItem.Map = ((Mobile)this.AttachedTo).Map;
                        this.m_SavedItem.Location = ((Mobile)this.AttachedTo).Location;
                        this.m_SavedItem.Parent = null;
                    }

                    this.m_SavedItem = null;
                    this.m_WasOwnedBy = null;
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile WasOwnedBy
        {
            get
            {
                return this.m_WasOwnedBy;
            }
            set
            {
                this.m_WasOwnedBy = value;
            }
        }
        public Item GetItem()
        {
            Item returneditem = this.SavedItem;

            this.m_SavedItem = null;
            this.m_WasOwnedBy = null;

            return returneditem;
        }

        // These are the various ways in which the message attachment can be constructed.  
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments
        public override void OnDelete()
        {
            base.OnDelete();

            // delete the item
            if (this.SavedItem != null)
            {
                //SavedItem.Delete();
                this.SafeItemDelete(this.SavedItem);
            }

            if (this.m_Container != null)
            {
                this.SafeItemDelete(this.m_Container);
                //m_Container.Delete();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            // version 0
            if (this.SavedItem != null)
            {
                writer.Write(this.m_SavedItem);
            }
            else
            {
                writer.Write((Item)null);
            }
            writer.Write(this.m_WasOwnedBy);
            writer.Write(this.m_Container);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            // version 0
            this.m_SavedItem = reader.ReadItem();
            this.m_WasOwnedBy = reader.ReadMobile();
            this.m_Container = (Container)reader.ReadItem();

            this.AddToContainer(this.m_SavedItem);
        }

        public override string OnIdentify(Mobile from)
        {
            if (from == null || from.IsPlayer())
                return null;

            if (this.Expiration > TimeSpan.Zero)
            {
                return String.Format("{2}: Item {0} expires in {1} mins", this.SavedItem, this.Expiration.TotalMinutes, this.Name);
            }
            else
            {
                return String.Format("{1}: Item {0}", this.SavedItem, this.Name);
            }
        }

        private void AddToContainer(Item item)
        {
            if (item == null)
                return;

            if (this.m_Container == null)
            {
                this.m_Container = new SaveItemPack();
            }

            // need to place in a container to prevent internal map cleanup of the item
            this.m_Container.DropItem(item);
            this.m_Container.Internalize();
        }

        private class SaveItemPack : Container
        {
            public SaveItemPack()
                : base(0x9B2)
            {
            }

            public SaveItemPack(Serial serial)
                : base(serial)
            {
            }

            public override int MaxWeight
            {
                get
                {
                    return 0;
                }
            }
            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write((int)0);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();
            }
        }
    }
}