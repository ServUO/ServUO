namespace Server.Items
{
    public class EtherealSand : Item, ICommodity
    {
        public override int LabelNumber => 1125984;  // ethereal sand
        public override double DefaultWeight => 0.1;

        [Constructable]
        public EtherealSand(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public EtherealSand()
            : this(1)
        {
        }

        [Constructable]
        public EtherealSand(int amount)
            : base(0xA3E8)
        {
            Stackable = true;
            Amount = amount;
        }

        public EtherealSand(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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
