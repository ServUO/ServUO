namespace Server.Items
{
    public class Beaugregory : BaseFish
    {
        [Constructable]
        public Beaugregory()
            : base(0xA378)
        {
        }

        public Beaugregory(Serial serial)
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
