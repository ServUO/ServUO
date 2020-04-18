namespace Server.Items
{
    [Flipable(0x1053, 0x1054)]
    public class Gears : Item
    {
        [Constructable]
        public Gears()
            : this(1)
        {
        }

        [Constructable]
        public Gears(int amount)
            : base(0x1053)
        {
            Stackable = true;
            Amount = amount;
            Weight = 1.0;
        }

        public Gears(Serial serial)
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