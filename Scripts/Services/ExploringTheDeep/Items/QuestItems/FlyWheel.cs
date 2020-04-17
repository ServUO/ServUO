namespace Server.Items
{
    public class FlyWheel : BaseDecayingItem
    {
        public override int LabelNumber => 1154427;  // Flywheel

        [Constructable]
        public FlyWheel()
            : base(0x46FE)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
            Hue = 1901;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override int Lifespan => 18000;

        public FlyWheel(Serial serial)
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
