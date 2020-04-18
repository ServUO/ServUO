namespace Server.Items
{
    public class RaedsGlory : WarCleaver
    {
        public override bool IsArtifact => true;
        [Constructable]
        public RaedsGlory()
        {
            Hue = 0x1E6;
            Attributes.BonusMana = 8;
            Attributes.SpellChanneling = 1;
            Attributes.WeaponSpeed = 20;
            WeaponAttributes.HitLeechHits = 40;
        }

        public RaedsGlory(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075036;// Raed's Glory
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
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