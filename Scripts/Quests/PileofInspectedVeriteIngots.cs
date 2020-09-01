namespace Server.Items
{
    public class PileofInspectedVeriteIngots : Item
    {
        public override int LabelNumber => 1113029; // Pile of Inspected Verite Ingots

        [Constructable]
        public PileofInspectedVeriteIngots()
            : base(0x1BEA)
        {
            Hue = CraftResources.GetHue(CraftResource.Verite);
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
