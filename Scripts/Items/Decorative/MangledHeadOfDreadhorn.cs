namespace Server.Items
{
    [Flipable(0x3156, 0x3157)]
    public class MangledHeadOfDreadhorn : Item
    {
        [Constructable]
        public MangledHeadOfDreadhorn()
            : base(0x3156)
        {
        }

        public MangledHeadOfDreadhorn(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1072088;// The Mangled Head of Dread Horn
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