namespace Server.Items
{
    public class EnchantScroll : SpellScroll
    {
        [Constructable]
        public EnchantScroll()
            : this(1)
        {
        }

        [Constructable]
        public EnchantScroll(int amount)
            : base(680, 0x2DA1, amount)
        {
        }

        public EnchantScroll(Serial serial)
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