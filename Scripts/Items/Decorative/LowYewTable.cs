namespace Server.Items
{
    public class LowYewTable : Item
    {
        public override int LabelNumber => 1075502;  // Low Yew Table

        [Constructable]
        public LowYewTable()
            : base(0x281A)
        {
            LootType = LootType.Blessed;
            Hue = 1192;
        }

        public LowYewTable(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1011032); // Yew
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
