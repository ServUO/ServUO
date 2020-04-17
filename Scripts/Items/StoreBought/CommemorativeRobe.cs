namespace Server.Items
{
    public class CommemorativeRobe : BaseOuterTorso
    {
        public override int LabelNumber => 1157009;  // Commemorative Robe

        [Constructable]
        public CommemorativeRobe()
            : base(0x4B9D)
        {
            LootType = LootType.Blessed;
        }

        public CommemorativeRobe(Serial serial)
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

            int version = reader.ReadInt();
        }
    }
}
