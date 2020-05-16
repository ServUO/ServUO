namespace Server.Items
{
    public class Filefish : BaseFish
    {
        [Constructable]
        public Filefish()
            : base(0xA36F)
        {
        }

        public Filefish(Serial serial)
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
