namespace Server.Items
{
    public class Taffy : CandyCane
    {
        [Constructable]
        public Taffy()
            : this(1)
        {
        }

        public Taffy(int amount)
            : base(0x469D)
        {
            Stackable = true;
            Amount = amount;
        }

        public Taffy(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1096949;/* taffy */
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
