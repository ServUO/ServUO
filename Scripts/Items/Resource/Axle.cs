namespace Server.Items
{
    [Flipable(0x105B, 0x105C)]
    public class Axle : Item
    {
        [Constructable]
        public Axle()
            : this(1)
        {
        }

        [Constructable]
        public Axle(int amount)
            : base(0x105B)
        {
            Stackable = true;
            Amount = amount;
            Weight = 1.0;
        }

        public Axle(Serial serial)
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