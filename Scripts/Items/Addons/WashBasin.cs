using Server.Gumps;

namespace Server.Items
{
    public class WaterContainerComponent : AddonComponent, IWaterSource
    {
        private int m_Quantity;
        public int Item_ID { get; set; }
        public int FullItem_ID { get; set; }
        public int MaxQuantity { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool IsEmpty => (m_Quantity <= 0);

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool IsFull => (m_Quantity >= MaxQuantity);

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int Quantity
        {
            get
            {
                return m_Quantity;
            }
            set
            {
                if (value != m_Quantity)
                {
                    m_Quantity = (value < 1) ? 0 : (value > MaxQuantity) ? MaxQuantity : value;

                    ItemID = (IsEmpty) ? Item_ID : FullItem_ID;
                }
            }
        }

        [Constructable]
        public WaterContainerComponent(int itemID, int fullitemid, int maxquantity)
            : base(itemID)
        {
            MaxQuantity = maxquantity;
            Item_ID = itemID;
            FullItem_ID = fullitemid;
        }

        public WaterContainerComponent(Serial serial)
            : base(serial)
        {
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            list.Add(1049522, "#1115912"); // a container with ~1_DRINK_NAME~
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(m_Quantity);
            writer.Write(Item_ID);
            writer.Write(FullItem_ID);
            writer.Write(MaxQuantity);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Quantity = reader.ReadInt();
            Item_ID = reader.ReadInt();
            FullItem_ID = reader.ReadInt();
            MaxQuantity = reader.ReadInt();
        }
    }

    public class WashBasinAddon : BaseAddon
    {
        [Constructable]
        public WashBasinAddon(DirectionType type)
        {
            switch (type)
            {
                case DirectionType.South:
                    AddComponent(new LocalizedAddonComponent(41646, 1125668), 1, 0, 0);
                    AddComponent(new WaterContainerComponent(41645, 41647, 5), 0, 0, 0);
                    AddComponent(new LocalizedAddonComponent(41644, 1125668), -1, 0, 0);
                    break;
                case DirectionType.East:
                    AddComponent(new LocalizedAddonComponent(41653, 1125668), 0, -1, 0);
                    AddComponent(new WaterContainerComponent(41652, 41654, 5), 0, 0, 0);
                    AddComponent(new LocalizedAddonComponent(41651, 1125668), 0, 1, 0);
                    break;
            }
        }

        public WashBasinAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new WashBasinDeed();

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

    public class WashBasinDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber => 1158966;  // Wash Basin

        public override BaseAddon Addon
        {
            get
            {
                WashBasinAddon addon = new WashBasinAddon(_Direction);

                return addon;
            }
        }

        private DirectionType _Direction;

        [Constructable]
        public WashBasinDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public WashBasinDeed(Serial serial)
            : base(serial)
        {
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)DirectionType.South, 1075386); // South
            list.Add((int)DirectionType.East, 1075387); // East
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            _Direction = (DirectionType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(AddonOptionGump));
                from.SendGump(new AddonOptionGump(this, 1154194)); // Choose a Facing:
            }
            else
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
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
