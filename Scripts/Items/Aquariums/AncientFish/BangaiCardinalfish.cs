namespace Server.Items
{
    public class BangaiCardinalfish : BaseFish
    {
        [Constructable]
        public BangaiCardinalfish()
            : base(0xA367)
        {
        }

        public BangaiCardinalfish(Serial serial)
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
