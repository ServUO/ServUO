namespace Server.Items
{
    public class PileofInspectedDullCopperIngots : Item
    {
        [Constructable]
        public PileofInspectedDullCopperIngots()
            : base(0x1BEA)
        {
            Hue = CraftResources.GetHue(CraftResource.DullCopper);
        }

        public PileofInspectedDullCopperIngots(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1113021;//Pile of Inspected Dull Copper Ingots
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
