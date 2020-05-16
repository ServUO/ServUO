namespace Server.Items
{
    public class MagnificentShrimpgoby : BaseFish
    {
        [Constructable]
        public MagnificentShrimpgoby()
            : base(0xA368)
        {
        }

        public MagnificentShrimpgoby(Serial serial)
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
