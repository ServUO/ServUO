namespace Server.Items
{
    public class BlueHermitCrab : BaseFish
    {
        [Constructable]
        public BlueHermitCrab()
            : base(0xA37F)
        {
        }

        public BlueHermitCrab(Serial serial)
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
