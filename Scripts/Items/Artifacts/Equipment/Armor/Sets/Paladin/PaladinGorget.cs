namespace Server.Items
{
    public class PaladinGorget : PlateGorget
    {
        public override bool IsArtifact => true;
        [Constructable]
        public PaladinGorget()
            : base()
        {
            SetHue = 0x47E;

            Attributes.RegenHits = 1;
            Attributes.AttackChance = 5;

            SetAttributes.ReflectPhysical = 25;
            SetAttributes.NightSight = 1;

            SetSkillBonuses.SetValues(0, SkillName.Chivalry, 10);

            SetSelfRepair = 3;
            SetPhysicalBonus = 2;
            SetFireBonus = 5;
            SetColdBonus = 5;
            SetPoisonBonus = 3;
            SetEnergyBonus = 5;
        }

        public PaladinGorget(Serial serial)
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