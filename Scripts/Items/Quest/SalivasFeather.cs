namespace Server.Items
{
    public class SalivasFeather : Item
    {
        [Constructable]
        public SalivasFeather()
            : base(0x1020)
        {
            LootType = LootType.Blessed;
            Hue = 0x5C;
        }

        public SalivasFeather(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074234;// Saliva's Feather
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