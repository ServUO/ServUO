using System;

namespace Server.Items
{
    public class PileOfInspectedGoldIngots : PileOfInspectedIngots
    {
        public override int LabelNumber { get { return 1113027; } } // Pile of Inspected Gold Ingots

        [Constructable]
        public PileOfInspectedGoldIngots()
            : base(0x8A5)
        {
        }

        public PileOfInspectedGoldIngots(Serial serial)
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