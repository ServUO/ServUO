using System;

namespace Server.Items
{
    [Furniture]
    [FlipableAttribute(0x4142, 0x4143)] // 0x4141 & 0x4144 have the "you got mail" flag up.
    public class Mailbox : LockableContainer
    {
        public override int DefaultGumpID { get { return 0x11A; } }

        [Constructable]
        public Mailbox()
            : base(0x4142)
        {
            Weight = 5.0;
        }

        public override void OnItemAdded(Item item)
        {
            base.OnItemAdded(item);

            if (ItemID == 0x4142)
                ItemID = 0x4141;
            else if (ItemID == 0x4144)
                ItemID = 0x4143;
        }

        public override void OnItemRemoved(Item item)
        {
            base.OnItemRemoved(item);

            if (Items.Count == 0)
            {
                if (ItemID == 0x4141)
                    ItemID = 0x4142;
                else if (ItemID == 0x4143)
                    ItemID = 0x4144;
            }
        }

        public Mailbox(Serial serial)
            : base(serial)
        {
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
