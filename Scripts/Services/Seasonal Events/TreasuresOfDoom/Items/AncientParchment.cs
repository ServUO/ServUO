namespace Server.Items
{
    public class AncientParchment : Item
    {
        public override int LabelNumber => 1155627;  // Ancient Parchment

        [Constructable]
        public AncientParchment()
            : this(1)
        {
        }

        [Constructable]
        public AncientParchment(int amount)
            : base(0x2269)
        {
            LootType = LootType.Blessed;
            Stackable = true;
            Amount = amount;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack))
            {
                m.PrivateOverheadMessage(Network.MessageType.Regular, 0x3B2, 1155628, m.NetState); // *The parchment appears heavily worn and in need of restoration by a skilled Scribe...*
            }
        }

        public AncientParchment(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt(); // version
        }
    }
}
