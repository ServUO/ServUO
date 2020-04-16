namespace Server.Items
{
    public class GoldShield : OrderShield
    {
        [Constructable]
        public GoldShield()
        {
            Name = "a gold shield";
            Hue = 0x501;
        }

        public GoldShield(Serial serial)
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