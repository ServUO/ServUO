namespace Server.Items
{
    public class SerpentFangKey : PeerlessKey
    {
        [Constructable]
        public SerpentFangKey()
            : base(0x2002)
        {
            Weight = 2.0;
            Hue = 53;
            LootType = LootType.Blessed;
        }

        public SerpentFangKey(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074341;// serpent fang key
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
