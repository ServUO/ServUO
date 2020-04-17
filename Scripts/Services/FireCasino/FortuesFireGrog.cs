namespace Server.Items
{
    public class FortunesFireGrog : BaseBeverage
    {
        public override int MaxQuantity => 1;

        [Constructable]
        public FortunesFireGrog() : base(BeverageType.Liquor)
        {
        }

        public override int ComputeItemID()
        {
            if (Quantity > 0)
                return Utility.RandomBool() ? 2542 : 2543;

            return ItemID = ItemID == 2543 ? 8067 : 8065;
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            list.Add(1049515, "#1153415"); // a mug of forune fires grog
        }

        public FortunesFireGrog(Serial serial)
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