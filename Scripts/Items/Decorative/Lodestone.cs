using System;

namespace Server.Items
{
    public class Lodestone : Item
    {
        [Constructable]
        public Lodestone()
            : base(0x5739)
        {
        }

        public Lodestone(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113348;
            }
        }// lodestone
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}