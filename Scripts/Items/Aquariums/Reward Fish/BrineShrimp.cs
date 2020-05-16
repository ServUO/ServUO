namespace Server.Items
{
    public class BrineShrimp : BaseFish
    {
        [Constructable]
        public BrineShrimp()
            : base(0x3B11)
        {
        }

        public BrineShrimp(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074415;// Brine shrimp
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