namespace Server.Items
{
    public class BakeKitsuneStatue : Item
    {
        public override bool IsArtifact => true;
        [Constructable]
        public BakeKitsuneStatue()
            : base(0x2763)
        {
            LootType = LootType.Blessed;
            Weight = 1.0;
        }

        public BakeKitsuneStatue(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073189;// A Bake Kitsune Contribution Statue from the Britannia Royal Zoo.
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