using System;

namespace Server.Items
{
    [Flipable(0xA491, 0xA492)]
    public class PillowChalice : Item
    {
        public override int LabelNumber { get { return 1025015; } } // pillow

        [Constructable]
        public PillowChalice()
            : base(0xA491)
        {
            Weight = 1;
        }

        public PillowChalice(Serial serial)
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
