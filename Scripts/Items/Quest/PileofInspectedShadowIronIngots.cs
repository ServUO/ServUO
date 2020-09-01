namespace Server.Items
{
    public class PileofInspectedShadowIronIngots : Item
    {
        [Constructable]
        public PileofInspectedShadowIronIngots()
            : base(0x1BEA)
        {
            Hue = CraftResources.GetHue(CraftResource.ShadowIron);
        }

        public PileofInspectedShadowIronIngots(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1113022;//Pile of Inspected Shadow Iron Ingots
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
