namespace Server.Items
{
    public class DeathGloves : LeatherGloves
    {
        public override bool IsArtifact => true;
        [Constructable]
        public DeathGloves()
            : base()
        {
            this.SetHue = 0x455;

            this.Attributes.RegenHits = 1;
            this.Attributes.RegenMana = 1;

            this.SetAttributes.LowerManaCost = 10;

            this.SetSkillBonuses.SetValues(0, SkillName.Necromancy, 10);

            this.SetSelfRepair = 3;

            this.SetPhysicalBonus = 4;
            this.SetFireBonus = 5;
            this.SetColdBonus = 3;
            this.SetPoisonBonus = 4;
            this.SetEnergyBonus = 4;
        }

        public DeathGloves(Serial serial)
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

            int version = reader.ReadInt();
        }
    }
}