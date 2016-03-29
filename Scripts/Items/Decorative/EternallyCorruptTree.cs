using System;

namespace Server.Items
{
    public class EternallyCorruptTree : Item
    {
        [Constructable]
        public EternallyCorruptTree()
            : base(0x20FA)
        {
            this.Hue = Utility.RandomMinMax(0x899, 0x8B0);
        }

        public EternallyCorruptTree(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072093;
            }
        }// Eternally Corrupt Tree
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