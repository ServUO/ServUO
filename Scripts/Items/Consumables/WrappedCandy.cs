namespace Server.Items
{
    public class WrappedCandy : CandyCane
    {
        [Constructable]
        public WrappedCandy()
            : this(1)
        {
        }

        public WrappedCandy(int amount)
            : base(0x469e)
        {
            Stackable = true;
            Amount = amount;
        }

        public WrappedCandy(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1096950;/* wrapped candy */
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
