using System;

namespace Server.Items
{
    public class DecoBlackmoor : Item
    {
        [Constructable]
        public DecoBlackmoor()
            : base(0xF79)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoBlackmoor(Serial serial)
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