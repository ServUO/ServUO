namespace Server.Items
{
    public class TableLamp : Item
    {
        [Constructable]
        public TableLamp()
            : base(0x49C2)
        {
            Weight = 1;
        }

        public TableLamp(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1151220;// table lamp

        public override void OnDoubleClick(Mobile from)
        {
            if (ItemID == 0x49C2)
                ItemID = 0x49C1;
            else if (ItemID == 0x49C1)
                ItemID = 0x49C2;
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