namespace Server.Items
{
    public class ObsidianEarrings : GargishEarrings
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113820;  // Obsidian Earrings

        public override int BasePhysicalResistance => 4;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 13;

        [Constructable]
        public ObsidianEarrings()
        {
            Attributes.BonusMana = 8;
            Attributes.RegenMana = 2;
            Attributes.RegenStam = 2;
            Attributes.SpellDamage = 8;
            AbsorptionAttributes.CastingFocus = 4;
        }

        public ObsidianEarrings(Serial serial)
            : base(serial)
        {
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

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
