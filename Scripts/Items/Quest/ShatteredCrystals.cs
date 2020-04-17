namespace Server.Items
{
    public class ShatteredCrystals : PeerlessKey
    {
        [Constructable]
        public ShatteredCrystals()
            : base(0x223F)
        {
            Weight = 1;
            Hue = 0x47E;
        }

        public ShatteredCrystals(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074266;// shattered crystal
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