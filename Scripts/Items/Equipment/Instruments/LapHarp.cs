namespace Server.Items
{
    public class LapHarp : BaseInstrument
    {
        [Constructable]
        public LapHarp()
            : base(0xEB2, 0x45, 0x46)
        {
            Weight = 10.0;
        }

        public LapHarp(Serial serial)
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