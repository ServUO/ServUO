using System;

namespace Server.Items
{
    public class Chessmen : Item
    {
        [Constructable]
        public Chessmen()
            : base(0xE13)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public Chessmen(Serial serial)
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