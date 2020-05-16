namespace Server.Items
{
    public class YellowFinBluebelly : BaseFish
    {
        [Constructable]
        public YellowFinBluebelly()
            : base(0x3B07)
        {
        }

        public YellowFinBluebelly(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073831;// A Yellow Fin Bluebelly  
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