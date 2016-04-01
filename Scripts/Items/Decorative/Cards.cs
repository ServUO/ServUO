using System;

namespace Server.Items
{
    public class Cards : Item
    {
        [Constructable]
        public Cards()
            : base(0xE19)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public Cards(Serial serial)
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