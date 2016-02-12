using System;

namespace Server.Items
{
    public class GildedKilt : BaseOuterLegs
    {
        [Constructable]
        public GildedKilt()
            : this(0)
        {
        }

        [Constructable]
        public GildedKilt(int hue)
            : base(0x781B, hue)
        {
            Weight = 2.0;
        }

        public GildedKilt(Serial serial)
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