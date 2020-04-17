namespace Server.Items
{
    public class PaladinHelm : PlateHelm
    {
        public override bool IsArtifact => true;
        [Constructable]
        public PaladinHelm()
            : base()
        {
            this.SetHue = 0x47E;

            this.Attributes.RegenHits = 1;
            this.Attributes.AttackChance = 5;

            this.SetAttributes.ReflectPhysical = 25;
            this.SetAttributes.NightSight = 1;

            this.SetSkillBonuses.SetValues(0, SkillName.Chivalry, 10);

            this.SetSelfRepair = 3;

            this.SetPhysicalBonus = 2;
            this.SetFireBonus = 5;
            this.SetColdBonus = 5;
            this.SetPoisonBonus = 3;
            this.SetEnergyBonus = 5;
        }

        public PaladinHelm(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074303;// Plate of Honor
        public override SetItem SetID => SetItem.Paladin;
        public override int Pieces => 6;
        public override int BasePhysicalResistance => 4;
        public override int BaseFireResistance => 9;
        public override int BaseColdResistance => 3;
        public override int BasePoisonResistance => 6;
        public override int BaseEnergyResistance => 8;
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