using System;

namespace Server.Items
{
    [FlipableAttribute(0xE41, 0xE40)]
    public class TrashChest : Container
    {
        [Constructable]
        public TrashChest()
            : base(0xE41)
        {
            this.Movable = false;
        }

        public TrashChest(Serial serial)
            : base(serial)
        {
        }

        public override int DefaultMaxWeight
        {
            get
            {
                return 0;
            }
        }// A value of 0 signals unlimited weight
        public override bool IsDecoContainer
        {
            get
            {
                return false;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!base.OnDragDrop(from, dropped))
                return false;

            this.PublicOverheadMessage(Network.MessageType.Regular, 0x3B2, Utility.Random(1042891, 8));
            dropped.Delete();

            return true;
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (!base.OnDragDropInto(from, item, p))
                return false;

            this.PublicOverheadMessage(Network.MessageType.Regular, 0x3B2, Utility.Random(1042891, 8));
            item.Delete();

            return true;
        }
    }
}