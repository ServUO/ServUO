namespace Server.Items
{
    public class BlackrockMoonstone : Item
    {
        public override int LabelNumber => 1156993;  // Black Moonstone

        [Constructable]
        public BlackrockMoonstone()
            : base(0x9CAA)
        {
            Hue = 1175;
        }

        public BlackrockMoonstone(Serial serial)
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
