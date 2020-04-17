namespace Server.Items
{
    public class CrushedCrystals : PeerlessKey
    {
        [Constructable]
        public CrushedCrystals()
            : base(0x223C)
        {
            Weight = 1;
            Hue = 0x47E;
        }

        public CrushedCrystals(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074262;// crushed crystal pieces
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