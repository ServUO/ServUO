namespace Server.Items
{
    [Flipable(0x4B9D, 0x4B9E)]
    public class AnniversaryRobe : BaseOuterTorso
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1153496;  // 15th Anniversary Commemorative Robe

        [Constructable]
        public AnniversaryRobe() : this(0x455)
        {
        }

        [Constructable]
        public AnniversaryRobe(int hue) : base(0x4B9D, hue)
        {
            LootType = LootType.Blessed;
            Weight = 1.0;
        }

        public AnniversaryRobe(Serial serial)
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