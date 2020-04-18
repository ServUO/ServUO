namespace Server.Items
{
    public class WireSpool : BaseDecayingItem
    {
        public override int LabelNumber => 1154428;  // Wire Spool

        [Constructable]
        public WireSpool()
            : base(0x4CDB)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
            Hue = 2315;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override int Lifespan => 18000;

        public WireSpool(Serial serial)
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
