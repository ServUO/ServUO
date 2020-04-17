namespace Server.Items
{
    public class HunterGloves : LeafGloves
    {
        public override bool IsArtifact => true;
        [Constructable]
        public HunterGloves()
            : base()
        {
            SetHue = 0x483;

            Attributes.RegenHits = 1;
            Attributes.Luck = 50;

            SetAttributes.BonusDex = 10;

            SetSkillBonuses.SetValues(0, SkillName.Stealth, 40);

            SetSelfRepair = 3;

            SetPhysicalBonus = 5;
            SetFireBonus = 4;
            SetColdBonus = 3;
            SetPoisonBonus = 4;
            SetEnergyBonus = 4;
        }

        public HunterGloves(Serial serial)
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