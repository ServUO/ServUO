namespace Server.Items
{
    public class SpinedBloodwormBracers : GargishClothArms
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113865;  // Spined Bloodworm Bracers
        public override int BasePhysicalResistance => 11;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 15;
        public override int BasePoisonResistance => 15;
        public override int BaseEnergyResistance => 10;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public SpinedBloodwormBracers()
        {
            Hue = 1642;
            Attributes.RegenHits = 2;
            Attributes.RegenStam = 2;
            Attributes.WeaponDamage = 10;
            Attributes.ReflectPhysical = 30;
            SAAbsorptionAttributes.EaterKinetic = 10;
        }

        public SpinedBloodwormBracers(Serial serial)
            : base(serial)
        {
        }

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
