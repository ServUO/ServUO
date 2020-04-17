namespace Server.Items
{
    public class MurkyMilk : Pitcher
    {
        public override int LabelNumber => 1153874;  // Murky Milk
        public override int MaxQuantity => 5;
        public override double DefaultWeight => 1;

        [Constructable]
        public MurkyMilk()
            : base(BeverageType.Milk)
        {
            Hue = 0x3e5;
            Quantity = MaxQuantity;
            ItemID = (Utility.RandomBool()) ? 0x09F0 : 0x09AD;
        }

        public MurkyMilk(Serial serial)
            : base(serial)
        {
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