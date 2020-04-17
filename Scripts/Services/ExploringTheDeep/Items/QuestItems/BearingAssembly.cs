namespace Server.Items
{
    public class BearingAssembly : BaseDecayingItem
    {
        public override int LabelNumber => 1154430;  // Bearing Assembly

        [Constructable]
        public BearingAssembly()
            : base(0xE74)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override int Lifespan => 18000;

        public BearingAssembly(Serial serial)
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