namespace Server.Items
{
    public class AriellesBauble : Item
    {
        [Constructable]
        public AriellesBauble()
            : base(0x23B)
        {
            Weight = 2.0;
            LootType = LootType.Blessed;
        }

        public AriellesBauble(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073137;// A bauble
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