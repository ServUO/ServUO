using System;

namespace Server.Items
{
    public class DecoTarot2 : Item
    {
        [Constructable]
        public DecoTarot2()
            : base(0x12A6)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoTarot2(Serial serial)
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