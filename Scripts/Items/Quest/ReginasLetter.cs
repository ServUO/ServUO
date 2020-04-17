namespace Server.Items
{
    public class ReginasLetter : Item
    {
        [Constructable]
        public ReginasLetter()
            : base(0x14ED)
        {
            LootType = LootType.Blessed;
            Weight = 1;
        }

        public ReginasLetter(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075306;// Regina's Letter
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