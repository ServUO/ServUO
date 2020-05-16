namespace Server.Items
{
    public class Frogfish : BaseFish
    {
        [Constructable]
        public Frogfish()
            : base(0xA36A)
        {
        }

        public Frogfish(Serial serial)
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
