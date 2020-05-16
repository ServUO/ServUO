namespace Server.Items
{
    public class Mandarinfish : BaseFish
    {
        [Constructable]
        public Mandarinfish()
            : base(0xA369)
        {
        }

        public Mandarinfish(Serial serial)
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
