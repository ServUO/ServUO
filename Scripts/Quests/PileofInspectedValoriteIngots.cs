namespace Server.Items
{
    public class PileofInspectedValoriteIngots : Item
    {
        [Constructable]
        public PileofInspectedValoriteIngots()
            : base(0x1BEA)
        {
            Hue = CraftResources.GetHue(CraftResource.Valorite);
        }

        public PileofInspectedValoriteIngots(Serial serial)
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
            reader.ReadInt();
        }
    }
}
