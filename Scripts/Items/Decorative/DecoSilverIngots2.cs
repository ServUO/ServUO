using System;

namespace Server.Items
{
    public class DecoSilverIngots2 : Item
    {
        [Constructable]
        public DecoSilverIngots2()
            : base(0x1BF6)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoSilverIngots2(Serial serial)
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