namespace Server.Items
{
    public class DragonFlameKey : PeerlessKey
    {
        [Constructable]
        public DragonFlameKey()
            : base(0x2002)
        {
            Weight = 2.0;
            Hue = 42;
            LootType = LootType.Blessed;
        }

        public DragonFlameKey(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074343;// dragon flame key
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
