namespace Server.Items
{
    [TypeAlias("Server.Items.AdmiralHeartyRum")]
    public class AdmiralsHeartyRum : BeverageBottle
    {
        public override bool IsArtifact => true;
        [Constructable]
        public AdmiralsHeartyRum()
            : base(BeverageType.Ale)
        {
            Hue = 0x66C;
        }

        public AdmiralsHeartyRum(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1063477;
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