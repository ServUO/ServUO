namespace Server.Items
{
    public class HunterLegs : LeafLegs
    {
        public override bool IsArtifact => true;
        [Constructable]
        public HunterLegs()
            : base()
        {
            this.SetHue = 0x483;

            this.Attributes.RegenHits = 1;
            this.Attributes.Luck = 50;

            this.SetAttributes.BonusDex = 10;

            this.SetSkillBonuses.SetValues(0, SkillName.Stealth, 40);

            this.SetSelfRepair = 3;

            this.SetPhysicalBonus = 5;
            this.SetFireBonus = 4;
            this.SetColdBonus = 3;
            this.SetPoisonBonus = 4;
            this.SetEnergyBonus = 4;
        }

        public HunterLegs(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074301;// Hunter's Garb
        public override SetItem SetID => SetItem.Hunter;
        public override int Pieces => 4;
        public override int BasePhysicalResistance => 9;
        public override int BaseFireResistance => 6;
        public override int BaseColdResistance => 3;
        public override int BasePoisonResistance => 8;
        public override int BaseEnergyResistance => 4;
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