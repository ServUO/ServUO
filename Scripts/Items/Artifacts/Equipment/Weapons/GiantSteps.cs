namespace Server.Items
{
    public class GiantSteps : GargishStoneLegs
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113537;  // Giant Steps

        [Constructable]
        public GiantSteps()
            : base()
        {
            Hue = 656;
            Attributes.BonusStr = 5;
            Attributes.BonusDex = 5;
            Attributes.BonusHits = 5;
            Attributes.RegenHits = 2;
            Attributes.WeaponDamage = 10;
        }

        public GiantSteps(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 18;
        public override int BaseFireResistance => 16;
        public override int BaseColdResistance => 4;
        public override int BasePoisonResistance => 8;
        public override int BaseEnergyResistance => 12;
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