using System;

namespace Server.Items
{
    public class Checkers2 : Item
    {
        [Constructable]
        public Checkers2()
            : base(0xE1B)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public Checkers2(Serial serial)
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