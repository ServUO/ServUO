using System;

namespace Server.Items
{
    public class FancyKilt : BaseOuterLegs
    {
        [Constructable]
        public FancyKilt()
            : this(0)
        {
        }

        [Constructable]
        public FancyKilt(int hue)
            : base(0x781D, hue)
        {
            Weight = 2.0;
        }

        public FancyKilt(Serial serial)
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