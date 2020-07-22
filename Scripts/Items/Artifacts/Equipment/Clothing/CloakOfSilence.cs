namespace Server.Items
{
    [Flipable(0x2FB9, 0x3173)]
    public class CloakOfSilence : BaseOuterTorso, ICanBeElfOrHuman
    {
        public bool ElfOnly { get { return false; } set { } }
        public override bool IsArtifact => true;

        [Constructable]
        public CloakOfSilence()
            : base(0x2FB9)
        {
            Weight = 2.0;
            Hue = 0x2A0;
            SkillBonuses.SetValues(0, SkillName.Stealth, 10);
        }

        public CloakOfSilence(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1112883; // Cloak of Silence

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
