namespace Server.Items
{
    public class ScoutSmallPlateJingasa : SmallPlateJingasa
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1080473;  // Scout's Small Plate Jingasa

        public override SetItem SetID => SetItem.Scout;
        public override int Pieces => 6;

        public override int BasePhysicalResistance => 7;
        public override int BaseFireResistance => 7;
        public override int BaseColdResistance => 7;
        public override int BasePoisonResistance => 7;
        public override int BaseEnergyResistance => 7;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public ScoutSmallPlateJingasa() : base()
        {
            Hue = 1148;
            Weight = 5;

            Attributes.BonusDex = 1;
            ArmorAttributes.MageArmor = 1;

            SetAttributes.BonusDex = 6;
            SetAttributes.RegenHits = 2;
            SetAttributes.RegenMana = 2;
            SetAttributes.AttackChance = 10;
            SetAttributes.DefendChance = 10;

            SetHue = 0x46D;

            SetPhysicalBonus = 28;
            SetFireBonus = 28;
            SetColdBonus = 28;
            SetPoisonBonus = 28;
            SetEnergyBonus = 28;
        }

        public ScoutSmallPlateJingasa(Serial serial) : base(serial)
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