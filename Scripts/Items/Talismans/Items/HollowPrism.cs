using System;

namespace Server.Items
{
    public class HollowPrism : Item
    {
        [Constructable]
        public HollowPrism()
            : base(0x2F5D)
        {
            this.Weight = 1.0;
        }

        public HollowPrism(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072895;
            }
        }// hollow prism
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