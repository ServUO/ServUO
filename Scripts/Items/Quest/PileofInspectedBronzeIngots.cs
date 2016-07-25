using System;

namespace Server.Items
{
    public class PileOfInspectedBronzeIngots : PileOfInspectedIngots
    {
        public override int LabelNumber { get { return 1113024; } } // Pile of Inspected Bronze Ingots

        [Constructable]
        public PileOfInspectedBronzeIngots()
            : base(0x972)
        {
        }

        public PileOfInspectedBronzeIngots(Serial serial)
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