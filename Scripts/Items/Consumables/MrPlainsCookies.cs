namespace Server.Items
{
    public class MrPlainsCookies : Food
    {
        [Constructable]
        public MrPlainsCookies()
            : base(0x160C)
        {
            Weight = 1.0;
            FillFactor = 4;
            Hue = 0xF4;
            Stackable = false;
        }

        public MrPlainsCookies(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName => "Mr Plain's Cookies";
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
                Stackable = false;
        }
    }
}