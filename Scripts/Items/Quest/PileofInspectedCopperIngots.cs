using System;

namespace Server.Items
{
    public class PileOfInspectedCopperIngots : PileOfInspectedIngots
    {
        public override int LabelNumber { get { return 1113023; } } // Pile of Inspected Copper Ingots

        [Constructable]
        public PileOfInspectedCopperIngots()
            : base(0x96D)
        {
        }

        public PileOfInspectedCopperIngots(Serial serial)
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