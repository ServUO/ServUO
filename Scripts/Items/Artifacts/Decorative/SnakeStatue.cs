namespace Server.Items
{
    public class SnakeStatue : Item
    {
        public override bool IsArtifact => true;
        [Constructable]
        public SnakeStatue()
            : base(0x25C2)
        {
            LootType = LootType.Blessed;
            Weight = 1.0;
        }

        public SnakeStatue(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073194;// A Snake Contribution Statue from the Britannia Royal Zoo.
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