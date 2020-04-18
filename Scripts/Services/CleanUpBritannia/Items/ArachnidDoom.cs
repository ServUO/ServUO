namespace Server.Items
{
    public class ArachnidDoom : BaseInstrument
    {
        [Constructable]
        public ArachnidDoom()
        {
            Hue = 1944;
            Weight = 4;
            Slayer = SlayerName.ArachnidDoom;

            UsesRemaining = 450;
        }

        public ArachnidDoom(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1154724;// Arachnid Doom
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