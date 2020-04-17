namespace Server.Items
{
    public class CoagulatedLegs : PeerlessKey
    {
        [Constructable]
        public CoagulatedLegs()
            : base(0x1CDF)
        {
            Weight = 1;
        }

        public CoagulatedLegs(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074327;// coagulated legs
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