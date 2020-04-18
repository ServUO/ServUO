namespace Server.Items
{
    public class ExecutionersCap : Item
    {
        [Constructable]
        public ExecutionersCap()
            : this(1)
        {
        }

        [Constructable]
        public ExecutionersCap(int amount)
            : base(0xF83)
        {
            Stackable = true;
            Amount = amount;
            Weight = 1.0;
        }

        public ExecutionersCap(Serial serial)
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