using System;

namespace Server.Items
{
    public class PileOfInspectedAgapiteIngots : PileOfInspectedIngots
    {
        public override int LabelNumber { get { return 1113028; } } // Pile of Inspected Agapite Ingots

        [Constructable]
        public PileOfInspectedAgapiteIngots()
            : base(0x979)
        {
        }

        public PileOfInspectedAgapiteIngots(Serial serial)
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