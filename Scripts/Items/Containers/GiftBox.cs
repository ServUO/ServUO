using System;

namespace Server.Items
{
    [Furniture]
    [Flipable(0x232A, 0x232B)]
    public class GiftBox : BaseContainer
    {
        [Constructable]
        public GiftBox()
            : this(Utility.RandomDyedHue())
        {
        }

        [Constructable]
        public GiftBox(int hue)
            : base(Utility.Random(0x232A, 2))
        {
            this.Weight = 2.0;
            this.Hue = hue;
        }

        public GiftBox(Serial serial)
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