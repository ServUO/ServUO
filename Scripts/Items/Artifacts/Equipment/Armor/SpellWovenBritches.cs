namespace Server.Items
{
    public class SpellWovenBritches : LeafLegs
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1072929;// Spell Woven Britches
        public override int BaseFireResistance => 15;
        public override int BasePoisonResistance => 16;

        [Constructable]
        public SpellWovenBritches()
        {
            Hue = 0x487;
            SkillBonuses.SetValues(0, SkillName.Meditation, 10.0);
            Attributes.BonusInt = 8;
            Attributes.SpellDamage = 10;
            Attributes.LowerManaCost = 10;
        }

        public SpellWovenBritches(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}
