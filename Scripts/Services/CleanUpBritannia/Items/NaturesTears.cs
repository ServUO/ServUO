namespace Server.Items
{
    public class NaturesTears : BaseInstrument
    {
        [Constructable]
        public NaturesTears()
        {
            Hue = 2075;
            Weight = 5;
            Slayer = SlayerName.Fey;

            UsesRemaining = 450;
        }

        public NaturesTears(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1154373;// Nature's Tears
        public override int InitMinUses => 450;
        public override int InitMaxUses => 450;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}