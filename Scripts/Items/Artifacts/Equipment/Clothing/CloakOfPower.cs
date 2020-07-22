namespace Server.Items
{
    [Flipable(0x2FB9, 0x3173)]
    public class CloakOfPower : BaseOuterTorso, ICanBeElfOrHuman
    {
        public bool ElfOnly { get { return false; } set { } }
        public override bool IsArtifact => true;

        [Constructable]
        public CloakOfPower()
            : base(0x2FB9)
        {
            Weight = 2.0;
            Hue = 0xFE;
            Attributes.BonusStr = 2;
            Attributes.BonusDex = 2;
            Attributes.BonusInt = 2;
        }

        public CloakOfPower(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1112882; // Cloak of Power

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
