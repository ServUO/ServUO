namespace Server.Items
{
    public class BlackPowder : Item, ICommodity
    {
        public override int LabelNumber => 1095826;  // black powder

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        [Constructable]
        public BlackPowder()
            : this(1)
        {
        }

        [Constructable]
        public BlackPowder(int amount)
            : base(0x423A)
        {
            Stackable = true;
            Amount = amount;
            Hue = 1109;
        }

        public BlackPowder(Serial serial)
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
