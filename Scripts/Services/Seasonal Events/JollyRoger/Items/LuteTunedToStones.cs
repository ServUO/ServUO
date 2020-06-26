namespace Server.Items
{
    public class LuteTunedToStones : BaseInstrument
    {
        public override int LabelNumber => 1159413;  // A Lute Tuned to Stones

        [Constructable]
        public LuteTunedToStones()
            : base(0xEB3, 0x682, 0x4D)
        {
            Weight = 5.0;
        }

        public LuteTunedToStones(Serial serial)
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
            reader.ReadInt();
        }
    }
}
