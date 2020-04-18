namespace Server.Items
{
    public class GnawsFang : PeerlessKey
    {
        [Constructable]
        public GnawsFang()
            : base(0x10E8)
        {
            Weight = 1;
            Hue = 0x174; // TODO check
            LootType = LootType.Blessed;
        }

        public GnawsFang(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074332;// gnaw's fang
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
