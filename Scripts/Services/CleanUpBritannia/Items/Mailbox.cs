using Server.ContextMenus;
using Server.Multis;
using Server.Network;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    [Furniture]
    public class Mailbox : LockableContainer, IFlipable
    {
        public override int LabelNumber => 1113927;  // Mailbox

        public override int DefaultGumpID => 0x11A;

        public virtual int SouthMailBoxID => 0x4141;
        public virtual int SouthEmptyMailBoxID => 0x4142;
        public virtual int EastMailBoxID => 0x4143;
        public virtual int EastEmptyMailBoxID => 0x4144;

        public Dictionary<Item, Mobile> Contents { get; set; }

        public bool IsEmpty => Items.Count == 0;

        [CommandProperty(AccessLevel.Decorator)]
        public override int ItemID
        {
            get { return base.ItemID; }
            set
            {
                base.ItemID = value;

                CheckMailBox();
            }
        }

        [Constructable]
        public Mailbox()
            : this(0x4142)
        {
        }

        [Constructable]
        public Mailbox(int id)
            : base(id)
        {
            Weight = 5.0;
        }

        public void CheckMailBox()
        {
            if (IsEmpty)
            {
                if (ItemID == SouthMailBoxID)
                {
                    base.ItemID = SouthEmptyMailBoxID;
                }
                else if (ItemID == EastMailBoxID)
                {
                    base.ItemID = EastEmptyMailBoxID;
                }
            }
            else
            {
                if (ItemID == SouthEmptyMailBoxID)
                {
                    base.ItemID = SouthMailBoxID;
                }
                else if (ItemID == EastEmptyMailBoxID)
                {
                    base.ItemID = EastMailBoxID;
                }
            }
        }

        public virtual void OnFlip(Mobile from)
        {
            if (ItemID == SouthMailBoxID)
            {
                base.ItemID = EastMailBoxID;
            }
            else if (ItemID == EastMailBoxID)
            {
                base.ItemID = SouthMailBoxID;
            }
            else if (ItemID == SouthEmptyMailBoxID)
            {
                base.ItemID = EastEmptyMailBoxID;
            }
            else if (ItemID == EastEmptyMailBoxID)
            {
                base.ItemID = SouthEmptyMailBoxID;
            }
        }

        public override void GetChildProperties(ObjectPropertyList list, Item item)
        {
            base.GetChildProperties(list, item);

            if (Contents != null && Contents.ContainsKey(item))
            {
                list.Add(1113938, Contents[item] != null ? Contents[item].Name : "Unknown");
            }
        }

        public override void GetChildContextMenuEntries(Mobile from, List<ContextMenuEntry> list, Item item)
        {
            base.GetChildContextMenuEntries(from, list, item);
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            bool dropped = base.OnDragDropInto(from, item, p);
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && dropped)
            {
                OnItemDropped(from, item, house);
            }

            return dropped;
        }

        public override bool TryDropItem(Mobile from, Item dropped, bool sendFullMessage)
        {
            if (!CheckHold(from, dropped, true, true))
            {
                return false;
            }

            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && IsLockedDown)
            {
                if (!house.CheckAccessibility(this, from))
                {
                    PrivateOverheadMessage(MessageType.Regular, 0x21, 1061637, from.NetState); // You are not allowed to access this!
                    from.SendLocalizedMessage(501727); // You cannot lock that down!
                    return false;
                }
            }

            DropItem(dropped);

            if (house != null && !IsLockedDown)
            {
                OnItemDropped(from, dropped, house);
            }

            return true;
        }

        public override bool IsAccessibleTo(Mobile m)
        {
            return true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && !house.IsOwner(from))
            {
                if (IsSecure)
                {
                    SendLocalizedMessageTo(from, 1010563); // This container is secure.                    
                    return;
                }
                else if (IsLockedDown)
                {
                    SendLocalizedMessageTo(from, 1061637); // You are not allowed to access this.
                    return;
                }
            }

            base.OnDoubleClick(from);
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            if (item == this)
            {
                return base.CheckLift(from, item, ref reject);
            }

            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && IsSecure)
            {
                SecureInfo secure = house.GetSecureInfoFor(this);

                return secure != null && house.HasSecureAccess(from, secure);
            }

            return base.CheckLift(from, item, ref reject);
        }

        public override void SendFullItemsMessage(Mobile to, Item item) // That mailbox is completely full.
        {
            to.SendLocalizedMessage(1113940);  // That mailbox is completely full.
        }

        public virtual void OnItemDropped(Mobile from, Item item, BaseHouse house)
        {
            SecureInfo secure = house.GetSecureInfoFor(this);

            if (secure != null && !house.HasSecureAccess(from, secure))
            {
                if (Contents == null)
                    Contents = new Dictionary<Item, Mobile>();

                Contents[item] = from;
                item.InvalidateProperties();
            }
        }

        public override void OnItemAdded(Item item)
        {
            base.OnItemAdded(item);

            CheckMailBox();
        }

        public override void OnItemRemoved(Item item)
        {
            base.OnItemRemoved(item);

            CheckMailBox();

            if (Contents != null && Contents.ContainsKey(item))
            {
                Contents.Remove(item);

                if (Contents.Count == 0)
                {
                    Contents = null;
                }
            }
        }

        public override void Open(Mobile from)
        {
            Defrag();
            DisplayTo(from);
        }

        private void Defrag()
        {
            if (Contents == null)
                return;

            List<Item> remove = Contents.Keys.Where(k => k.Deleted || !Items.Contains(k)).ToList();

            foreach (Item item in remove)
            {
                Contents.Remove(item);
            }

            ColUtility.Free(remove);
        }

        public Mailbox(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);

            writer.Write(Contents == null ? 0 : Contents.Count);

            if (Contents != null)
            {
                foreach (KeyValuePair<Item, Mobile> kvp in Contents)
                {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value);
                }
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        int count = reader.ReadInt();

                        for (int i = 0; i < count; i++)
                        {
                            Item item = reader.ReadItem();
                            Mobile m = reader.ReadMobile();

                            if (m != null && item != null)
                            {
                                if (Contents == null)
                                    Contents = new Dictionary<Item, Mobile>();

                                Contents[item] = m;
                            }
                        }
                        break;
                    }
            }
        }
    }
}
