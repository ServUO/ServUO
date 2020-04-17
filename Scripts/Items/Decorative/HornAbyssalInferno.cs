namespace Server.Items
{
    public class HornAbyssalInferno : Item
    {
        [Constructable]
        public HornAbyssalInferno()
            : base(0x2dB7)
        {
        }

        public HornAbyssalInferno(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1031703;// Horn of Abyssal Infernal
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