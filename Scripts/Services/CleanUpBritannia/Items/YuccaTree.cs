using System;

namespace Server.Items
{
    public class YuccaTree : Item
    {
        [Constructable]
        public YuccaTree()
            : base(0x0D37)
        {
            this.Weight = 10;
        }

        public YuccaTree(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1023383;
            }
        }// yucca
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