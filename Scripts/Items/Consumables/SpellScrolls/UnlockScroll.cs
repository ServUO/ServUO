namespace Server.Items
{
    public class UnlockScroll : SpellScroll
    {
        [Constructable]
        public UnlockScroll()
            : this(1)
        {
        }

        [Constructable]
        public UnlockScroll(int amount)
            : base(22, 0x1F43, amount)
        {
        }

        public UnlockScroll(Serial serial)
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