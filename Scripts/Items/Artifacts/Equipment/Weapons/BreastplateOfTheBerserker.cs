namespace Server.Items
{
    public class BreastplateOfTheBerserker : GargishPlateChest
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113539;  // Breastplate of the Berserker

        [Constructable]
        public BreastplateOfTheBerserker()
        {
            Hue = 1172;
            Attributes.WeaponSpeed = 10;
            Attributes.WeaponDamage = 15;
            Attributes.LowerManaCost = 4;
            Attributes.BonusHits = 5;
            Attributes.RegenStam = 3;
        }

        public BreastplateOfTheBerserker(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 18;
        public override int BaseFireResistance => 16;
        public override int BaseColdResistance => 5;
        public override int BasePoisonResistance => 11;
        public override int BaseEnergyResistance => 5;
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
