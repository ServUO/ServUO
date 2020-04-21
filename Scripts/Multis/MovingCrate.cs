using Server.Items;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Multis
{
    public class MovingCrate : Container
    {
        public static readonly int MaxItemsPerSubcontainer = 20;
        public static readonly int Rows = 3;
        public static readonly int Columns = 5;
        public static readonly int HorizontalSpacing = 25;
        public static readonly int VerticalSpacing = 25;
        private BaseHouse m_House;
        private Timer m_InternalizeTimer;
        public MovingCrate(BaseHouse house)
            : base(0xE3D)
        {
            Hue = 0x8A5;
            Movable = false;

            m_House = house;
        }

        public MovingCrate(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1061690;// Packing Crate
        public BaseHouse House
        {
            get
            {
                return m_House;
            }
            set
            {
                m_House = value;
            }
        }
        public override int DefaultMaxItems => 0;
        public override int DefaultMaxWeight => 0;
        public override bool IsDecoContainer => false;
        /*
        public override void AddNameProperties( ObjectPropertyList list )
        {
        base.AddNameProperties( list );

        if ( House != null && House.InternalizedVendors.Count > 0 )
        list.Add( 1061833, House.InternalizedVendors.Count.ToString() ); // This packing crate contains ~1_COUNT~ vendors/barkeepers.
        }
        */
        public override void DropItem(Item dropped)
        {
            // 1. Try to stack the item
            foreach (Item item in Items)
            {
                if (item is PackingBox)
                {
                    List<Item> subItems = item.Items;

                    for (int i = 0; i < subItems.Count; i++)
                    {
                        Item subItem = subItems[i];

                        if (!(subItem is Container) && subItem.StackWith(null, dropped, false))
                            return;
                    }
                }
            }

            // 2. Try to drop the item into an existing container
            foreach (Item item in Items)
            {
                if (item is PackingBox)
                {
                    Container box = (Container)item;
                    List<Item> subItems = box.Items;

                    if (subItems.Count < MaxItemsPerSubcontainer)
                    {
                        box.DropItem(dropped);
                        return;
                    }
                }
            }

            // 3. Drop the item into a new container
            Container subContainer = new PackingBox();
            subContainer.DropItem(dropped);

            Point3D location = GetFreeLocation();
            if (location != Point3D.Zero)
            {
                AddItem(subContainer);
                subContainer.Location = location;
            }
            else
            {
                base.DropItem(subContainer);
            }
        }

        public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
        {
            if (m.AccessLevel < AccessLevel.GameMaster)
            {
                m.SendLocalizedMessage(1061145); // You cannot place items into a house moving crate.
                return false;
            }

            return base.CheckHold(m, item, message, checkItems, plusItems, plusWeight);
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            return base.CheckLift(from, item, ref reject) && House != null && !House.Deleted && House.IsOwner(from);
        }

        public override bool CheckItemUse(Mobile from, Item item)
        {
            return base.CheckItemUse(from, item) && House != null && !House.Deleted && House.IsOwner(from);
        }

        public override void OnItemRemoved(Item item)
        {
            base.OnItemRemoved(item);

            if (TotalItems == 0)
                Delete();
        }

        public void RestartTimer()
        {
            if (m_InternalizeTimer == null)
            {
                m_InternalizeTimer = new InternalizeTimer(this);
                m_InternalizeTimer.Start();
            }
            else
            {
                m_InternalizeTimer.Stop();
                m_InternalizeTimer.Start();
            }
        }

        public void Hide()
        {
            if (m_InternalizeTimer != null)
            {
                m_InternalizeTimer.Stop();
                m_InternalizeTimer = null;
            }

            List<Item> toRemove = new List<Item>();
            foreach (Item item in Items)
                if (item is PackingBox && item.Items.Count == 0)
                    toRemove.Add(item);

            foreach (Item item in toRemove)
                item.Delete();

            if (TotalItems == 0)
                Delete();
            else
                Internalize();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (House != null && House.MovingCrate == this)
                House.MovingCrate = null;

            if (m_InternalizeTimer != null)
                m_InternalizeTimer.Stop();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1);

            writer.Write(m_House);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_House = reader.ReadItem() as BaseHouse;

            if (m_House != null)
            {
                m_House.MovingCrate = this;
                Timer.DelayCall(TimeSpan.Zero, Hide);
            }
            else
            {
                Timer.DelayCall(TimeSpan.Zero, Delete);
            }

            if (version == 0)
                MaxItems = -1; // reset to default
        }

        private Point3D GetFreeLocation()
        {
            bool[,] positions = new bool[Rows, Columns];

            foreach (Item item in Items)
            {
                if (item is PackingBox)
                {
                    int i = (item.Y - Bounds.Y) / VerticalSpacing;
                    if (i < 0)
                        i = 0;
                    else if (i >= Rows)
                        i = Rows - 1;

                    int j = (item.X - Bounds.X) / HorizontalSpacing;
                    if (j < 0)
                        j = 0;
                    else if (j >= Columns)
                        j = Columns - 1;

                    positions[i, j] = true;
                }
            }

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (!positions[i, j])
                    {
                        int x = Bounds.X + j * HorizontalSpacing;
                        int y = Bounds.Y + i * VerticalSpacing;

                        return new Point3D(x, y, 0);
                    }
                }
            }

            return Point3D.Zero;
        }

        public class InternalizeTimer : Timer
        {
            private readonly MovingCrate m_Crate;
            public InternalizeTimer(MovingCrate crate)
                : base(TimeSpan.FromMinutes(5.0))
            {
                m_Crate = crate;

                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                m_Crate.Hide();
            }
        }
    }

    public class PackingBox : BaseContainer
    {
        public PackingBox()
            : base(0x9A8)
        {
            Movable = false;
        }

        public PackingBox(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1061690;// Packing Crate
        public override int DefaultGumpID => 0x4B;
        public override int DefaultDropSound => 0x42;
        public override Rectangle2D Bounds => new Rectangle2D(16, 51, 168, 73);
        public override int DefaultMaxItems => 0;
        public override int DefaultMaxWeight => 0;
        public override void SendCantStoreMessage(Mobile to, Item item)
        {
            to.SendLocalizedMessage(1061145); // You cannot place items into a house moving crate.
        }

        public override void OnItemRemoved(Item item)
        {
            base.OnItemRemoved(item);

            if (item.GetBounce() == null && TotalItems == 0)
                Delete();
        }

        public override void OnItemBounceCleared(Item item)
        {
            base.OnItemBounceCleared(item);

            if (TotalItems == 0)
                Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (version == 0)
                MaxItems = -1; // reset to default
        }
    }
}
