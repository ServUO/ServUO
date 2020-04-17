namespace Server.Items
{
    public class GwennosHarp : LapHarp
    {
        public override bool IsArtifact => true;
        [Constructable]
        public GwennosHarp()
        {
            Hue = 0x47E;
            Slayer = SlayerName.Repond;
            Slayer2 = SlayerName.ReptilianDeath;
        }

        public GwennosHarp(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1063480;
        public override int InitMinUses => 1600;
        public override int InitMaxUses => 1600;
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