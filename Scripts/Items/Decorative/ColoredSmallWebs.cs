namespace Server.Items
{
    public class ColoredSmallWebs : Item
    {
        [Constructable]
        public ColoredSmallWebs()
            : base(Utility.RandomBool() ? 0x10d6 : 0x10d7)
        {
            Hue = Utility.RandomBool() ? 0x455 : 0x4E9;
        }

        public ColoredSmallWebs(Serial serial)
            : base(serial)
        {
        }

        public override double DefaultWeight => 5;
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