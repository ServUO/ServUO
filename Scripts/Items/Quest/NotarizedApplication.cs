namespace Server.Items
{
    public class NotarizedApplication : Item
    {
        [Constructable]
        public NotarizedApplication()
            : base(0x14EF)
        {
            LootType = LootType.Blessed;
            Weight = 1.0;
        }

        public NotarizedApplication(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073135;// Notarized Application
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