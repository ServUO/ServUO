namespace Server.Items
{
    public class MyrmidonChest : StuddedChest
    {
        public override bool IsArtifact => true;
        [Constructable]
        public MyrmidonChest()
            : base()
        {
            this.SetHue = 0x331;

            this.Attributes.BonusStr = 1;
            this.Attributes.BonusHits = 2;

            this.SetAttributes.Luck = 500;
            this.SetAttributes.NightSight = 1;

            this.SetSelfRepair = 3;

            this.SetPhysicalBonus = 3;
            this.SetFireBonus = 3;
            this.SetColdBonus = 3;
            this.SetPoisonBonus = 3;
            this.SetEnergyBonus = 3;
        }

        public MyrmidonChest(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074306;// Myrmidon Armor
        public override SetItem SetID => SetItem.Myrmidon;
        public override int Pieces => 6;
        public override int BasePhysicalResistance => 7;
        public override int BaseFireResistance => 7;
        public override int BaseColdResistance => 3;
        public override int BasePoisonResistance => 5;
        public override int BaseEnergyResistance => 3;
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