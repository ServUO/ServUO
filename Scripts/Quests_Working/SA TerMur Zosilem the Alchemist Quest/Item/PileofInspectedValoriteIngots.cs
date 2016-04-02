using System;

namespace Server.Items
{
    public class PileofInspectedValoriteIngots : Item
    {
        [Constructable]
        public PileofInspectedValoriteIngots()
            : base(0x1BEA)
        {
            this.Name = "Pile of Inspected Valorite Ingots";

            this.Hue = 2219;
        }

        public PileofInspectedValoriteIngots(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}