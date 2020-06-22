namespace Server.Items
{
    public class CloakOfTheVirtuous : BaseCloak
    {
        public override int LabelNumber => 159340; // Cloak of the Virtuous

        public override bool IsArtifact => true;

        [Constructable]
        public CloakOfTheVirtuous()
            : base(0xA413)
        {
            Weight = 3;
        }

        public CloakOfTheVirtuous(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}

