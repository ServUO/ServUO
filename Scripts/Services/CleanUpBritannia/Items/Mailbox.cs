using System;
using System.Collections.Generic;
using Server.Multis;
using Server.ContextMenus;
using Server.Network;
using System.Linq;

namespace Server.Items
{
    [Furniture]
    [FlipableAttribute(0x4142, 0x4143)]
    public class Mailbox : LockableContainer, IFlipable
    {
        public override int DefaultGumpID { get { return 0x11A; } }

        public Dictionary<Item, Mobile> Contents { get; set; }

        [CommandProperty(AccessLevel.Decorator)]
        public override int ItemID
        {
            get { return base.ItemID; }
            set
            {
                base.ItemID = value;

                if (Items.Count > 0 && (ItemID == 0x4142 || ItemID == 0x4144))
                {
                    base.ItemID = ItemID - 1;
                }
                else if (Items.Count == 0 && (ItemID == 0x4141 || ItemID == 0x4143))
                {
                    base.ItemID = ItemID + 1;
                }
            }
        }

        [Constructable]
        public Mailbox()
            : base(0x4142)
        {
            Weight = 5.0;
        }

        public void OnFlip()
        {
            if (ItemID == 0x4141 || ItemID == 0x4142)
                ItemID = ItemID + 2;
            else
                ItemID = ItemID - 2;
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
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (!CheckHold(from, dropped, true, true))
            {
                return false;
            }

            if (house != null && IsLockedDown)
            {
                if (!house.CheckAccessibility(this, from))
                {
                    this.PrivateOverheadMessage(MessageType.Regular, 0x21, 1061637, from.NetState); // You are not allowed to access this!
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

        public override void SendFullItemsMessage(Mobile to, Item item) // That mailbox is completely full.
        {
            to.SendLocalizedMessage(1113940);  // That mailbox is completely full.
        }

        public virtual void OnItemDropped(Mobile from, Item item, BaseHouse house)
        {
            var secure = house.GetSecureInfoFor(this);

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

            if (ItemID == 0x4142 && ItemID != 0x141)
            {
                ItemID = 0x4141;
            }
            else if (ItemID == 0x4144 && ItemID != 0x143)
            {
                ItemID = 0x4143;
            }
        }

        public override void OnItemRemoved(Item item)
        {
            base.OnItemRemoved(item);

            if (Items.Count == 0)
            {
                if (ItemID == 0x4141 && ItemID != 0x4142)
                {
                    ItemID = 0x4142;
                }
                else if (ItemID == 0x4143 && ItemID != 0x4144)
                {
                    ItemID = 0x4144;
                }
            }

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

            var remove = Contents.Keys.Where(k => k.Deleted || !Items.Contains(k)).ToList();

            foreach (var item in remove)
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

            writer.Write((int)1);

            writer.Write(Contents == null ? 0 : Contents.Count);

            if (Contents != null)
            {
                foreach (var kvp in Contents)
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
