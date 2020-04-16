namespace Server.Items
{
    public class LillyPad : Item
    {
        [Constructable]
        public LillyPad()
            : base(0xDBC)
        {
            Weight = 1.0;
        }

        public LillyPad(Serial serial)
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