namespace Server.Items
{
    public class CrownOfArcaneTemperament : Circlet, ICanBeElfOrHuman
    {
        public override bool IsArtifact => true;
        public bool ElfOnly { get { return false; } set { } }

        public override int LabelNumber => 1113762;  // Crown of Arcane Temperament

        [Constructable]
        public CrownOfArcaneTemperament()
        {
            Attributes.BonusMana = 8;
            Attributes.RegenMana = 3;
            Attributes.SpellDamage = 8;
            Attributes.LowerManaCost = 6;
            Hue = 2012;
            AbsorptionAttributes.CastingFocus = 2;
        }

        public CrownOfArcaneTemperament(Serial serial)
            : base(serial)
        {
        }
        public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 14;
        public override int BaseColdResistance => 4;
        public override int BasePoisonResistance => 12;
        public override int BaseEnergyResistance => 7;

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
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
