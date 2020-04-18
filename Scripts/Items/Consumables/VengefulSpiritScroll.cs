namespace Server.Items
{
    public class VengefulSpiritScroll : SpellScroll
    {
        [Constructable]
        public VengefulSpiritScroll()
            : this(1)
        {
        }

        [Constructable]
        public VengefulSpiritScroll(int amount)
            : base(113, 0x226D, amount)
        {
        }

        public VengefulSpiritScroll(Serial serial)
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