namespace Server.Items
{
    public class ExorcismScroll : SpellScroll
    {
        [Constructable]
        public ExorcismScroll()
            : this(1)
        {
        }

        [Constructable]
        public ExorcismScroll(int amount)
            : base(116, 0x2270, amount)
        {
        }

        public ExorcismScroll(Serial serial)
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