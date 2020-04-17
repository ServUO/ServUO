namespace Server.Items
{
    public class SorcererFemaleChest : FemaleLeatherChest
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1080469;  // Sorcerer's Female Armor

        public override SetItem SetID => SetItem.Sorcerer;
        public override int Pieces => 6;

        public override int BasePhysicalResistance => 7;
        public override int BaseFireResistance => 7;
        public override int BaseColdResistance => 7;
        public override int BasePoisonResistance => 7;
        public override int BaseEnergyResistance => 7;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public SorcererFemaleChest() : base()
        {
            Hue = 1165;
            Weight = 1;

            Attributes.BonusInt = 1;
            Attributes.LowerRegCost = 10;

            SetAttributes.BonusInt = 6;
            SetAttributes.RegenMana = 2;
            SetAttributes.DefendChance = 10;
            SetAttributes.LowerManaCost = 5;
            SetAttributes.LowerRegCost = 40;

            SetHue = 1165;

            SetPhysicalBonus = 28;
            SetFireBonus = 28;
            SetColdBonus = 28;
            SetPoisonBonus = 28;
            SetEnergyBonus = 28;
        }

        public SorcererFemaleChest(Serial serial) : base(serial)
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