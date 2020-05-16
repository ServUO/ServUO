namespace Server.Items
{
    public class MinocBlueFish : BaseFish
    {
        [Constructable]
        public MinocBlueFish()
            : base(0x3AFE)
        {
        }

        public MinocBlueFish(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073829;// A Minoc Blue Fish
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