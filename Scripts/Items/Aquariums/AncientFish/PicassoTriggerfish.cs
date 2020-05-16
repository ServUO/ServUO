namespace Server.Items
{
    public class PicassoTriggerfish : BaseFish
    {
        [Constructable]
        public PicassoTriggerfish()
            : base(0xA36E)
        {
        }

        public PicassoTriggerfish(Serial serial)
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
