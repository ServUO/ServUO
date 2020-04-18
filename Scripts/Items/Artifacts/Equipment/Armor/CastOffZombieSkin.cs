namespace Server.Items
{
    public class CastOffZombieSkin : GargishLeatherArms
    {
        public override bool IsArtifact => true;
        [Constructable]
        public CastOffZombieSkin()
        {
            Hue = 1893;
            SkillBonuses.SetValues(0, SkillName.Necromancy, 5.0);
            SkillBonuses.SetValues(1, SkillName.SpiritSpeak, 5.0);
            Attributes.LowerManaCost = 5;
            Attributes.LowerRegCost = 8;
            Attributes.IncreasedKarmaLoss = 5;
        }

        public CastOffZombieSkin(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1113538; // Cast-off Zombie Skin

        public override int BasePhysicalResistance => 13;
        public override int BaseFireResistance => -2;
        public override int BaseColdResistance => 17;
        public override int BasePoisonResistance => 18;
        public override int BaseEnergyResistance => 6;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
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
