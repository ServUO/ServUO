namespace Server.Items
{
    [Flipable(0x4037, 0x4038)]
    public class GargishBanner : Item
    {
        [Constructable]
        public GargishBanner()
            : base(0x4037)
        {
            Weight = 10;
        }

        public GargishBanner(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

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
