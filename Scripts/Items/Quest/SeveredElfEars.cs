namespace Server.Items
{
    [Flipable(0x312D, 0x312E)]
    public class SeveredElfEars : Item
    {
        [Constructable]
        public SeveredElfEars()
            : this(1)
        {
        }

        [Constructable]
        public SeveredElfEars(int amount)
            : base(Utility.RandomList(0x312D, 0x312E))
        {
            Stackable = true;
            Amount = amount;
            Weight = 1;
        }

        public SeveredElfEars(Serial serial)
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
            reader.ReadInt();
        }
    }
}
