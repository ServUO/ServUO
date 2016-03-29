using System;

namespace Server.Items
{
    public class RightLeg : Item
    {
        [Constructable]
        public RightLeg()
            : base(0x1DA4)
        {
        }

        public RightLeg(Serial serial)
            : base(serial)
        {
        }

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