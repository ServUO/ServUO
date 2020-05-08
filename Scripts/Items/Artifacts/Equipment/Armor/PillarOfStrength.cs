namespace Server.Items
{
    public class PillarOfStrength : LargeStoneShield
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113533; // Pillar Of Strength
        public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 0;
        public override int BaseColdResistance => 0;
        public override int BasePoisonResistance => 1;
        public override int BaseEnergyResistance => 0;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public PillarOfStrength()
            : base()
        {
            Attributes.BonusStr = 10;
            Attributes.BonusHits = 10;
            Attributes.WeaponDamage = 20;
        }

        public PillarOfStrength(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
