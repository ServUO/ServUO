using System;

namespace Server.Items
{
    public class CheckeredKilt : BaseOuterLegs
    {
        [Constructable]
        public CheckeredKilt()
            : this(0)
        {
        }

        [Constructable]
        public CheckeredKilt(int hue)
            : base(0x781C, hue)
        {
            Weight = 2.0;
        }

        public CheckeredKilt(Serial serial)
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