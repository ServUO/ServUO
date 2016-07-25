using System;

namespace Server.Items
{
    public class PileOfInspectedShadowIronIngots : PileOfInspectedIngots
    {
        public override int LabelNumber { get { return 1113022; } } // Pile of Inspected Shadow Iron Ingots

        [Constructable]
        public PileOfInspectedShadowIronIngots()
            : base(0x966)
        {
        }

        public PileOfInspectedShadowIronIngots(Serial serial)
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