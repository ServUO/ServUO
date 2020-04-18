namespace Server.Items
{
    public class IrksBrain : PeerlessKey
    {
        [Constructable]
        public IrksBrain()
            : base(0x1CF0)
        {
            Weight = 1;
            Hue = 0x453; // TODO check
            LootType = LootType.Blessed;
        }

        public IrksBrain(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074335;// irk's brain
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
