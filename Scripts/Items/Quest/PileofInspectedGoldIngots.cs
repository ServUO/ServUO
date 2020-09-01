namespace Server.Items
{
    public class PileofInspectedGoldIngots : Item
    {
        [Constructable]
        public PileofInspectedGoldIngots()
            : base(0x1BEA)
        {
            Hue = CraftResources.GetHue(CraftResource.Gold);
        }

        public PileofInspectedGoldIngots(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1113027;//Pile of Inspected Gold Ingots
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
