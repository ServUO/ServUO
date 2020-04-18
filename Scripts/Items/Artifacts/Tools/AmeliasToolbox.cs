namespace Server.Items
{
    public class AmeliasToolbox : TinkerTools
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1077749; // Amelias Toolbox

        [Constructable]
        public AmeliasToolbox()
            : base(500)
        {
            LootType = LootType.Blessed;
            Hue = 1895; // TODO check
        }

        public AmeliasToolbox(Serial serial)
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