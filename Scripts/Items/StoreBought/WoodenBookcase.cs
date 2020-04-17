namespace Server.Items
{
    [Furniture]
    [Flipable(0x0A9D, 0x0A9E)]
    public class WoodenBookcase : BaseContainer
    {
        public override int LabelNumber => 1071102;  // Wooden Bookcase
        public override int DefaultGumpID => 0x4D;

        public bool IsEmpty => Items.Count == 0;

        [CommandProperty(AccessLevel.Decorator)]
        public override int ItemID
        {
            get { return base.ItemID; }
            set
            {
                int id = value;

                if (!IsEmpty && (id == 0x0A9D || id == 0x0A9E))
                {
                    switch (id)
                    {
                        case 0x0A9D: base.ItemID = Utility.RandomList(0x0A97, 0x0A98, 0x0A9B); break;
                        case 0x0A9E: base.ItemID = Utility.RandomList(0x0A99, 0x0A9A, 0x0A9C); break;
                    }
                }
                else if (IsEmpty && id != 0x0A9D && id != 0x0A9E)
                {
                    switch (id)
                    {
                        case 0x0A97:
                        case 0x0A98:
                        case 0x0A9B: base.ItemID = 0x0A9D; break;
                        case 0x0A99:
                        case 0x0A9A:
                        case 0x0A9C: base.ItemID = 0x0A9E; break;
                    }
                }
                else
                {
                    base.ItemID = value;
                }
            }
        }

        [Constructable]
        public WoodenBookcase()
            : base(0x0A9D)
        {
        }

        public override void OnItemAdded(Item item)
        {
            base.OnItemAdded(item);

            if (ItemID == 0x0A9D || ItemID == 0x0A9E)
            {
                switch (ItemID)
                {
                    default:
                    case 0x0A9D: ItemID = Utility.RandomList(0x0A97, 0x0A98, 0x0A9B); break;
                    case 0x0A9E: ItemID = Utility.RandomList(0x0A99, 0x0A9A, 0x0A9C); break;
                }
            }
        }

        public override void OnItemRemoved(Item item)
        {
            base.OnItemRemoved(item);

            if (IsEmpty && ItemID != 0x0A9D && ItemID != 0x0A9E)
            {
                switch (ItemID)
                {
                    default:
                    case 0x0A97:
                    case 0x0A98:
                    case 0x0A9B: base.ItemID = 0x0A9D; break;
                    case 0x0A99:
                    case 0x0A9A:
                    case 0x0A9C: base.ItemID = 0x0A9E; break;
                }
            }
        }

        public void Flip()
        {
            switch (ItemID)
            {
                case 0x0A97: ItemID = 0x0A99; break;
                case 0x0A98: ItemID = 0x0A9A; break;
                case 0x0A99: ItemID = 0x0A97; break;
                case 0x0A9A: ItemID = 0x0A98; break;
                case 0x0A9B: ItemID = 0x0A9C; break;
                case 0x0A9C: ItemID = 0x0A9B; break;
                case 0x0A9D: ItemID = 0x0A9E; break;
                case 0x0A9E: ItemID = 0x0A9D; break;
            }
        }

        public WoodenBookcase(Serial serial)
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
