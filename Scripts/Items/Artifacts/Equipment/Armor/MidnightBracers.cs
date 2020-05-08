namespace Server.Items
{
    public class MidnightBracers : BoneArms
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1061093;// Midnight Bracers
        public override int ArtifactRarity => 11;
        public override int BasePhysicalResistance => 23;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public MidnightBracers()
        {
            Hue = 0x455;
            SkillBonuses.SetValues(0, SkillName.Necromancy, 20.0);
            Attributes.SpellDamage = 10;
            ArmorAttributes.MageArmor = 1;
        }

        public MidnightBracers(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
