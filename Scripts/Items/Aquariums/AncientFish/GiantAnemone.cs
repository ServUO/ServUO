namespace Server.Items
{
    public class GiantAnemone : BaseFish
    {
        [Constructable]
        public GiantAnemone()
            : base(0xA386)
        {
        }

        public GiantAnemone(Serial serial)
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
