namespace Server.Items
{
    public class GargishRing : BaseRing
    {
        [Constructable]
        public GargishRing()
            : base(0x4212)
        {
            //Weight = 0.1;
        }

        public GargishRing(Serial serial)
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
