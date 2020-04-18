namespace Server.Items
{
    public class QuagmireStatue : Item
    {
        public override bool IsArtifact => true;
        [Constructable]
        public QuagmireStatue()
            : base(0x2614)
        {
            LootType = LootType.Blessed;
            Weight = 1.0;
        }

        public QuagmireStatue(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073195;// A Quagmire Contribution Statue from the Britannia Royal Zoo.
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