using System;

namespace Server.Items
{
    public class Cards2 : Item
    {
        [Constructable]
        public Cards2()
            : base(0xE16)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public Cards2(Serial serial)
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