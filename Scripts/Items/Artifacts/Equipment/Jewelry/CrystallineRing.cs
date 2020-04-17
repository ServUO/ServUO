namespace Server.Items
{
    public class CrystallineRing : GoldRing
    {
        public override int LabelNumber => 1075096;  // Crystalline Ring
        public override bool IsArtifact => true;

        [Constructable]
        public CrystallineRing()
        {
            Hue = 0x480;
            Attributes.RegenHits = 5;
            Attributes.RegenMana = 3;
            Attributes.SpellDamage = 20;
            SkillBonuses.SetValues(0, SkillName.Magery, 20.0);
            SkillBonuses.SetValues(1, SkillName.Focus, 20.0);
        }

        public CrystallineRing(Serial serial)
            : base(serial)
        {
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}