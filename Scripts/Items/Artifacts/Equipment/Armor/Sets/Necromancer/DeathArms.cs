namespace Server.Items
{
    public class DeathArms : LeatherArms
    {
        public override bool IsArtifact => true;
        [Constructable]
        public DeathArms()
            : base()
        {
            SetHue = 0x455;

            Attributes.RegenHits = 1;
            Attributes.RegenMana = 1;

            SetAttributes.LowerManaCost = 10;

            SetSkillBonuses.SetValues(0, SkillName.Necromancy, 10);

            SetSelfRepair = 3;

            SetPhysicalBonus = 4;
            SetFireBonus = 5;
            SetColdBonus = 3;
            SetPoisonBonus = 4;
            SetEnergyBonus = 4;
        }

        public DeathArms(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074305;// Death's Essence
        public override SetItem SetID => SetItem.Necromancer;
        public override int Pieces => 5;
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

            int version = reader.ReadInt(); // version
        }
    }
}