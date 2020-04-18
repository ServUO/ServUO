namespace Server.Items
{
    public class ValentinesTable : Item
    {
        public override int LabelNumber => 1098492;  // table

        [Constructable]
        public ValentinesTable()
            : base(0xA004)
        {
            Weight = 10.0;
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (ItemID == 0xA004)
                ItemID = 0xA005;
            else
                ItemID = 0xA004;
        }

        public ValentinesTable(Serial serial)
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
            int version = reader.ReadInt();
        }
    }

    [Flipable(0xA05C, 0xA05D, 0xA05E, 0xA05F)]
    public class ValentinesChair : Item
    {
        public override int LabelNumber => 1098456;  // chair

        [Constructable]
        public ValentinesChair()
            : base(0xA05C)
        {
            Weight = 20.0;
            LootType = LootType.Blessed;
        }

        public ValentinesChair(Serial serial)
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
            int version = reader.ReadInt();
        }
    }
}
