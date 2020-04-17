namespace Server.Items
{
    public class PaladinArms : PlateArms
    {
        public override bool IsArtifact => true;
        [Constructable]
        public PaladinArms()
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

        public PaladinArms(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074303;// Plate of Honor
        public override SetItem SetID => SetItem.Paladin;
        public override int Pieces => 6;
        public override int BasePhysicalResistance => 8;
        public override int BaseFireResistance => 5;
        public override int BaseColdResistance => 5;
        public override int BasePoisonResistance => 7;
        public override int BaseEnergyResistance => 5;
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