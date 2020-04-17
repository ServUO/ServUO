namespace Server.Items
{
    public class Aegis : HeaterShield
    {
        public override bool IsArtifact => true;
        [Constructable]
        public Aegis()
        {
            Hue = 0x47E;
            ArmorAttributes.SelfRepair = 5;
            Attributes.ReflectPhysical = 15;
            Attributes.DefendChance = 15;
            Attributes.LowerManaCost = 8;
        }

        public Aegis(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1061602;// Ægis
        public override int ArtifactRarity => 11;
        public override int BasePhysicalResistance => 15;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version < 1)
                PhysicalBonus = 0;
        }
    }
}