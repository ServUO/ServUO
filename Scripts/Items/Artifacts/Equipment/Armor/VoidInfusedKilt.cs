namespace Server.Items
{
    public class VoidInfusedKilt : GargishPlateKilt
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113868;  // Void Infused Kilt
        public override int BasePhysicalResistance => 13;
        public override int BaseFireResistance => 12;
        public override int BaseColdResistance => 8;
        public override int BasePoisonResistance => 9;
        public override int BaseEnergyResistance => 9;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public VoidInfusedKilt()
            : base()
        {
            Hue = 2124;
            Attributes.AttackChance = 5;
            Attributes.BonusStr = 5;
            Attributes.BonusDex = 5;
            Attributes.RegenMana = 1;
            Attributes.RegenStam = 1;
            AbsorptionAttributes.EaterDamage = 10;
        }

        public VoidInfusedKilt(Serial serial)
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
