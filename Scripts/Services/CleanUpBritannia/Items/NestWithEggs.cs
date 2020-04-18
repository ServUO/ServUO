namespace Server.Items
{
    public class NestWithEggs : Item
    {
        [Constructable]
        public NestWithEggs()
            : base(0x1AD4)
        {
            Hue = 2415;
            Weight = 2;
        }

        public NestWithEggs(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1026868;// nest with eggs
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