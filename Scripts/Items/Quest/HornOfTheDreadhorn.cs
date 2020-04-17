namespace Server.Items
{
    [Flipable(0x315C, 0x315D)]
    public class HornOfTheDreadhorn : Item
    {
        [Constructable]
        public HornOfTheDreadhorn()
            : base(0x315C)
        {
        }

        public HornOfTheDreadhorn(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1072089;// Horn of the Dread
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