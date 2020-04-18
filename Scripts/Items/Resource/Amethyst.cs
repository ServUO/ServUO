namespace Server.Items
{
    public class Amethyst : Item, IGem
    {
        [Constructable]
        public Amethyst()
            : this(1)
        {
        }

        [Constructable]
        public Amethyst(int amount)
            : base(0xF16)
        {
            Stackable = true;
            Amount = amount;
        }

        public Amethyst(Serial serial)
            : base(serial)
        {
        }

        public override double DefaultWeight => 0.1;
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