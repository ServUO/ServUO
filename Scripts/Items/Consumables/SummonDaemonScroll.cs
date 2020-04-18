namespace Server.Items
{
    public class SummonDaemonScroll : SpellScroll
    {
        [Constructable]
        public SummonDaemonScroll()
            : this(1)
        {
        }

        [Constructable]
        public SummonDaemonScroll(int amount)
            : base(60, 0x1F69, amount)
        {
        }

        public SummonDaemonScroll(Serial serial)
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