namespace Server.Items
{
    public abstract class BaseWaterContainer : Container, IHasQuantity
    {
        private int m_Quantity;
        public BaseWaterContainer(int Item_Id, bool filled)
            : base(Item_Id)
        {
            this.m_Quantity = (filled) ? this.MaxQuantity : 0;
        }

        public BaseWaterContainer(Serial serial)
            : base(serial)
        {
        }

        public abstract int voidItem_ID { get; }
        public abstract int fullItem_ID { get; }
        public abstract int MaxQuantity { get; }
        public override int DefaultGumpID
        {
            get
            {
                return 0x3e;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool IsEmpty
        {
            get
            {
                return (this.m_Quantity <= 0);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool IsFull
        {
            get
            {
                return (this.m_Quantity >= this.MaxQuantity);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int Quantity
        {
            get
            {
                return this.m_Quantity;
            }
            set
            {
                if (value != this.m_Quantity)
                {
                    this.m_Quantity = (value < 1) ? 0 : (value > this.MaxQuantity) ? this.MaxQuantity : value;

                    this.Movable = (!this.IsLockedDown) ? this.IsEmpty : false;

                    this.ItemID = (this.IsEmpty) ? this.voidItem_ID : this.fullItem_ID;

                    if (!this.IsEmpty)
                    {
                        IEntity rootParent = this.RootParentEntity;

                        if (rootParent != null && rootParent.Map != null && rootParent.Map != Map.Internal)
                            this.MoveToWorld(rootParent.Location, rootParent.Map);
                    }

                    this.InvalidateProperties();
                }
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsEmpty)
            {
                base.OnDoubleClick(from);
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            if (this.IsEmpty)
            {
                base.OnSingleClick(from);
            }
            else
            {
                if (this.Name == null)
                    this.LabelTo(from, this.LabelNumber);
                else
                    this.LabelTo(from, this.Name);
            }
        }

        public override void OnAosSingleClick(Mobile from)
        {
            if (this.IsEmpty)
            {
                base.OnAosSingleClick(from);
            }
            else
            {
                if (this.Name == null)
                    this.LabelTo(from, this.LabelNumber);
                else
                    this.LabelTo(from, this.Name);
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            if (this.IsEmpty)
            {
                base.GetProperties(list);
            }
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (!this.IsEmpty)
            {
                return false;
            }

            return base.OnDragDropInto(from, item, p);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write((int)this.m_Quantity);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            this.m_Quantity = reader.ReadInt();
        }
    }
}