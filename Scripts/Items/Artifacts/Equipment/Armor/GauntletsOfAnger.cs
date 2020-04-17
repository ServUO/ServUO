namespace Server.Items
{
    public class GauntletsOfAnger : PlateGloves
    {
        public override bool IsArtifact => true;
        [Constructable]
        public GauntletsOfAnger()
        {
            Hue = 0x29b;
            Attributes.BonusHits = 8;
            Attributes.RegenHits = 2;
            Attributes.DefendChance = 10;
        }

        public GauntletsOfAnger(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1094902;// Gauntlets of Anger [Replica]
        public override int BasePhysicalResistance => 4;
        public override int BaseFireResistance => 4;
        public override int BaseColdResistance => 5;
        public override int BasePoisonResistance => 6;
        public override int BaseEnergyResistance => 5;
        public override int InitMinHits => 150;
        public override int InitMaxHits => 150;
        public override bool CanFortify => false;
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