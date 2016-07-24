using System;

namespace Server.Items
{
    public abstract class PileOfInspectedIngots : Item
    {
        [Constructable]
        public PileOfInspectedIngots(int hue)
            : base(0x1BF0)
        {
            this.Weight = 2.0;
            this.Hue = hue;
        }

        public PileOfInspectedIngots(Serial serial)
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