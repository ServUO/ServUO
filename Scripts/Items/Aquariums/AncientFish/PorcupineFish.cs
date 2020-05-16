namespace Server.Items
{
    public class Porcupinefish : BaseFish
    {
        [Constructable]
        public Porcupinefish()
            : base(0xA370)
        {
        }

        public Porcupinefish(Serial serial)
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
