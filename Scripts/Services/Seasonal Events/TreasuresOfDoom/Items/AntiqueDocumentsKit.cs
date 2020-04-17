namespace Server.Items
{
    public class AntiqueDocumentsKit : Item
    {
        public override int LabelNumber => 1155630;  // Antique Documents Kit

        [Constructable]
        public AntiqueDocumentsKit()
            : base(0x1EBB)
        {
            LootType = LootType.Blessed;
        }

        public AntiqueDocumentsKit(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt(); // version
        }
    }
}