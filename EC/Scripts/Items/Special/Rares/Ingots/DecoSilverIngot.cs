using System;

namespace Server.Items
{
    public class DecoSilverIngot : Item
    {
        [Constructable]
        public DecoSilverIngot()
            : base(0x1BF5)
        {
            this.Movable = true;
            this.Stackable = true;
        }

        public DecoSilverIngot(Serial serial)
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