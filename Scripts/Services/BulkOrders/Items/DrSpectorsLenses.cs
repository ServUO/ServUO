namespace Server.Items
{
    public class DrSpectorsLenses : Glasses
    {
        public override int LabelNumber => 1156991;  // Dr. Spector's lenses
        public override bool IsArtifact => true;

        [Constructable]
        public DrSpectorsLenses()
        {
            Attributes.BonusInt = 8;
            Attributes.RegenMana = 4;
            Attributes.SpellDamage = 12;
            Attributes.LowerManaCost = 8;
            Attributes.LowerRegCost = 10;
        }

        public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 14;
        public override int BasePoisonResistance => 20;
        public override int BaseEnergyResistance => 20;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public DrSpectorsLenses(Serial serial)
            : base(serial)
        {
        }

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
