namespace Server.Items
{
    public class FlamingoTongue : BaseFish
    {
        [Constructable]
        public FlamingoTongue()
            : base(0xA382)
        {
        }

        public FlamingoTongue(Serial serial)
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
