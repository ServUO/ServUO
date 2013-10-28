using System;

namespace Server.Items
{
    public class DecoSilverIngot2 : Item
    {
        [Constructable]
        public DecoSilverIngot2()
            : base(0x1BF8)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoSilverIngot2(Serial serial)
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