namespace Server.Items
{
    public class FeeblemindScroll : SpellScroll
    {
        [Constructable]
        public FeeblemindScroll()
            : this(1)
        {
        }

        [Constructable]
        public FeeblemindScroll(int amount)
            : base(2, 0x1F30, amount)
        {
        }

        public FeeblemindScroll(Serial serial)
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