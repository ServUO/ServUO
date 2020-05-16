namespace Server.Items
{
    public class Spadefish : BaseFish
    {
        [Constructable]
        public Spadefish()
            : base(0xA379)
        {
        }

        public Spadefish(Serial serial)
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
