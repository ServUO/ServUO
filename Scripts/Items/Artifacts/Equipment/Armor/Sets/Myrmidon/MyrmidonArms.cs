namespace Server.Items
{
    public class MyrmidonArms : StuddedArms
    {
        public override bool IsArtifact => true;
        [Constructable]
        public MyrmidonArms()
            : base()
        {
            SetHue = 0x331;

            Attributes.BonusStr = 1;
            Attributes.BonusHits = 2;

            SetAttributes.Luck = 500;
            SetAttributes.NightSight = 1;

            SetSelfRepair = 3;

            SetPhysicalBonus = 3;
            SetFireBonus = 3;
            SetColdBonus = 3;
            SetPoisonBonus = 3;
            SetEnergyBonus = 3;
        }

        public MyrmidonArms(Serial serial)
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