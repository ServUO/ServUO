namespace Server.Items
{
    public class WizardsGlasses : ElvenGlasses
    {
        public override bool IsArtifact => true;
        [Constructable]
        public WizardsGlasses()
        {
            Attributes.BonusMana = 10;
            Attributes.RegenMana = 3;
            Attributes.SpellDamage = 15;
            Hue = 0x2B0;
        }

        public WizardsGlasses(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073374;//Wizard's Crystal Reading Glasses
        public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 5;
        public override int BaseColdResistance => 5;
        public override int BasePoisonResistance => 5;
        public override int BaseEnergyResistance => 5;
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

            if (version == 0 && Hue == 0)
                Hue = 0x2B0;
        }
    }
}