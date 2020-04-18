namespace Server.Items
{
    public class SpleenOfThePutrefier : PeerlessKey
    {
        [Constructable]
        public SpleenOfThePutrefier()
            : base(0x1CEE)
        {
            Weight = 1.0;
        }

        public SpleenOfThePutrefier(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074329;// spleen of the putrefier
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