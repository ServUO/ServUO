using System;

namespace Server.Items
{
    public class BrokenChair : Item
    {
        [Constructable]
        public BrokenChair()
            : base(Utility.Random(2) + 0xC19)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public BrokenChair(Serial serial)
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