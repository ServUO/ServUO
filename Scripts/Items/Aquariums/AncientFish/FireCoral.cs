namespace Server.Items
{
    public class FireCoral : BaseFish
    {
        [Constructable]
        public FireCoral()
            : base(0xA38B)
        {
        }

        public FireCoral(Serial serial)
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
