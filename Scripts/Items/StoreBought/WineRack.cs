using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public class WineRack : LockableContainer, IFlipable, IDyable
    {
        public override int LabelNumber => 1126367; // wine rack

        public override int DefaultGumpID => 0x44;

        public virtual int SouthID => 0xA568;
        public virtual int SouthEmptyID => 0xA567;
        public virtual int EastID => 0xA56A;
        public virtual int EastEmptyID => 0xA569;

        public bool IsEmpty => Items.Count == 0;

        public bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            Hue = sender.DyedHue;

            return true;
        }

        [CommandProperty(AccessLevel.Decorator)]
        public override int ItemID
        {
            get { return base.ItemID; }
            set
            {
                base.ItemID = value;

                CheckWineRack();
            }
        }

        public void CheckWineRack()
        {
            if (IsEmpty)
            {
                if (ItemID == SouthID)
                {
                    base.ItemID = SouthEmptyID;
                }
                else if (ItemID == EastID)
                {
                    base.ItemID = EastEmptyID;
                }
            }
            else
            {
                if (ItemID == SouthEmptyID)
                {
                    base.ItemID = SouthID;
                }
                else if (ItemID == EastEmptyID)
                {
                    base.ItemID = EastID;
                }
            }
        }

        [Constructable]
        public WineRack()
            : this(0xA567)
        {
        }

        [Constructable]
        public WineRack(int id)
            : base(id)
        {
        }

        public virtual void OnFlip(Mobile from)
        {
            if (ItemID == SouthID)
            {
                base.ItemID = EastID;
            }
            else if (ItemID == EastID)
            {
                base.ItemID = SouthID;
            }
            else if (ItemID == SouthEmptyID)
            {
                base.ItemID = EastEmptyID;
            }
            else if (ItemID == EastEmptyID)
            {
                base.ItemID = SouthEmptyID;
            }
        }

        public override bool DisplaysContent => false;

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072241, "{0}\t{1}\t{2}\t{3}", TotalItems, MaxItems, TotalWeight, MaxWeight);
            // Contents: ~1_COUNT~/~2_MAXCOUNT~ items, ~3_WEIGHT~/~4_MAXWEIGHT~ stones
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

        public virtual void OnItemDropped(Mobile from, Item item, BaseHouse house)
        {
            SecureInfo secure = house.GetSecureInfoFor(this);

            if (secure != null && !house.HasSecureAccess(from, secure))
            {
                item.InvalidateProperties();
            }
        }

        public override void OnItemAdded(Item item)
        {
            base.OnItemAdded(item);

            CheckWineRack();
        }

        public override void OnItemRemoved(Item item)
        {
            base.OnItemRemoved(item);

            CheckWineRack();
        }

        public WineRack(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
