namespace Server.Items
{
    public class TigerClawKey : PeerlessKey
    {
        [Constructable]
        public TigerClawKey()
            : base(0x2002)
        {
            Weight = 2.0;
            Hue = 105;
            LootType = LootType.Blessed;
        }

        public TigerClawKey(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074342;// tiger claw key
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
