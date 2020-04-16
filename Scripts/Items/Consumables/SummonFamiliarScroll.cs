namespace Server.Items
{
    public class SummonFamiliarScroll : SpellScroll
    {
        [Constructable]
        public SummonFamiliarScroll()
            : this(1)
        {
        }

        [Constructable]
        public SummonFamiliarScroll(int amount)
            : base(111, 0x226B, amount)
        {
        }

        public SummonFamiliarScroll(Serial serial)
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