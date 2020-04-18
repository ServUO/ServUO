namespace Server.Items
{
    public class PileofInspectedVeriteIngots : Item
    {
        [Constructable]
        public PileofInspectedVeriteIngots()
            : base(0x1BEA)
        {
            Name = "Pile of Inspected Agapite Ingots";

            Hue = 2207;
        }

        public PileofInspectedVeriteIngots(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}