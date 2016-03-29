using System;

namespace Server.Items
{
    public class DecoTarot6 : Item
    {
        [Constructable]
        public DecoTarot6()
            : base(0x12AA)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoTarot6(Serial serial)
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