namespace Server.Items
{
    [Flipable(0x1F03, 0x1F04)]
    public class ConjureresGarbHarbringer : BaseOuterTorso
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1114052; // Conjurer's Garb

        [Constructable]
        public ConjureresGarbHarbringer()
            : base(0x1F03, 0x486)
        {
            Hue = 0x4AA;
            Weight = 3.0;
            Attributes.DefendChance = 5;
            Attributes.RegenMana = 2;
        }

        public ConjureresGarbHarbringer(Serial serial)
            : base(serial)
        {
        }

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
