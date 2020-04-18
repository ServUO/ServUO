namespace Server.Items
{
    public class DetectiveCredentials : BaseNecklace
    {
        public override int LabelNumber => 1158641;  // RBG Detective Branch Official Credential

        [Constructable]
        public DetectiveCredentials()
            : base(0x1088)
        {
            Hue = 1176;
            LootType = LootType.Blessed;
        }

        public DetectiveCredentials(Serial serial)
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

            int version = reader.ReadInt();
        }
    }
}
