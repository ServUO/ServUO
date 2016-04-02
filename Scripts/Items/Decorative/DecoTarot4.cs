using System;

namespace Server.Items
{
    public class DecoTarot4 : Item
    {
        [Constructable]
        public DecoTarot4()
            : base(0x12A8)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoTarot4(Serial serial)
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