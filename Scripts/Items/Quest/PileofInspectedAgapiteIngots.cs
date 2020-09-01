namespace Server.Items
{
    public class PileofInspectedAgapiteIngots : Item
    {
        [Constructable]
        public PileofInspectedAgapiteIngots()
            : base(0x1BEA)
        {
            Hue = CraftResources.GetHue(CraftResource.Agapite);
        }

        public PileofInspectedAgapiteIngots(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1113028; //P ile of Inspected Agapite Ingots
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
