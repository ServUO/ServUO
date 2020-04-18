namespace Server.Items
{
    public class PropheticManuscript : BaseJournal
    {
        public override int LabelNumber => 1155631;  // Prophetic Manuscript

        public int Index { get; set; }

        public override TextDefinition Title => 1155638 + Index;
        public override TextDefinition Body => 1155632 + Index;

        [Constructable]
        public PropheticManuscript()
            : this(Utility.RandomMinMax(0, 4))
        {
        }

        public PropheticManuscript(int index)
        {
            ItemID = 0x42BF;
            Index = index;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1154760, "#1155637"); // By: Owain the Blind Prophet
        }

        public PropheticManuscript(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Index);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Index = reader.ReadInt();
        }
    }
}