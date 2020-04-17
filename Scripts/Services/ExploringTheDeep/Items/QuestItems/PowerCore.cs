namespace Server.Items
{
    public class PowerCore : BaseDecayingItem
    {
        public override int LabelNumber => 1154429;  // Power Core

        [Constructable]
        public PowerCore()
            : base(0x47E6)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
            Hue = 1967;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override int Lifespan => 18000;

        public PowerCore(Serial serial)
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
