namespace Server.Items
{
    public class WristWatch : Clock
    {
        [Constructable]
        public WristWatch()
            : base(0x1086)
        {
            Weight = DefaultWeight;
            LootType = LootType.Blessed;
            Layer = Layer.Bracelet;
        }

        public WristWatch(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1041421;// a wrist watch
        public override double DefaultWeight => 1.0;
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