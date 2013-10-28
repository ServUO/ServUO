using System;

namespace Server.Items
{
    public class DecoSilverIngots : Item
    {
        [Constructable]
        public DecoSilverIngots()
            : base(0x1BFA)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoSilverIngots(Serial serial)
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