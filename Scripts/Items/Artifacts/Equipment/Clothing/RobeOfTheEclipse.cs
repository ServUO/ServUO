namespace Server.Items
{
    [Flipable(0x1F03, 0x1F04)]
    public class RobeOfTheEclipse : BaseOuterTorso
    {
        public override bool IsArtifact => true;

        [Constructable]
        public RobeOfTheEclipse()
            : base(0x1F03, 0x486)
        {
            Weight = 3.0;
            Attributes.Luck = 95;
        }

        public RobeOfTheEclipse(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075082;// Robe of the Eclipse
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}
