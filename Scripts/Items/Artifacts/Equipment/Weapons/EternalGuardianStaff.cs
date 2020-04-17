namespace Server.Items
{
    public class EternalGuardianStaff : GnarledStaff
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1112443;  // Eternal Guardian Staff

        [Constructable]
        public EternalGuardianStaff()
        {
            Hue = 95;
            SkillBonuses.SetValues(0, SkillName.Mysticism, 15.0);
            Attributes.SpellDamage = 10;
            Attributes.LowerManaCost = 5;
            Attributes.SpellChanneling = 1;
        }

        public EternalGuardianStaff(Serial serial)
            : base(serial)
        {
        }

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