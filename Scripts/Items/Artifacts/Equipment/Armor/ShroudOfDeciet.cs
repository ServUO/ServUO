namespace Server.Items
{
    public class ShroudOfDeceit : BoneChest
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1094914;// Shroud of Deceit [Replica]
        public override int BasePhysicalResistance => 11;
        public override int BaseFireResistance => 6;
        public override int BaseColdResistance => 18;
        public override int BasePoisonResistance => 15;
        public override int BaseEnergyResistance => 13;
        public override int InitMinHits => 150;
        public override int InitMaxHits => 150;
        public override bool CanFortify => false;

        [Constructable]
        public ShroudOfDeceit()
        {
            Hue = 0x38F;
            Attributes.RegenHits = 3;
            ArmorAttributes.MageArmor = 1;
            SkillBonuses.Skill_1_Name = SkillName.MagicResist;
            SkillBonuses.Skill_1_Value = 10;
        }

        public ShroudOfDeceit(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
