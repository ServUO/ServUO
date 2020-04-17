namespace Server.Items
{
    public class StrippedSosarianSwill : BaseFish
    {
        [Constructable]
        public StrippedSosarianSwill()
            : base(0x3B0A)
        {
        }

        public StrippedSosarianSwill(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074594;// Stripped Sosarian Swill
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