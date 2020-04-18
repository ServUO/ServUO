namespace Server.Items
{
    public class SingingAxe : OrnateAxe
    {
        public override bool IsArtifact => true;
        [Constructable]
        public SingingAxe()
        {
            SkillBonuses.SetValues(0, SkillName.Musicianship, 5);
        }

        public SingingAxe(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073546;// singing axe
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