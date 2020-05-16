namespace Server.Items
{
    public class ClownTriggerfish : BaseFish
    {
        [Constructable]
        public ClownTriggerfish()
            : base(0xA36C)
        {
        }

        public ClownTriggerfish(Serial serial)
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
