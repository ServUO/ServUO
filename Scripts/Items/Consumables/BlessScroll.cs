namespace Server.Items
{
    public class BlessScroll : SpellScroll
    {
        [Constructable]
        public BlessScroll()
            : this(1)
        {
        }

        [Constructable]
        public BlessScroll(int amount)
            : base(16, 0x1F3D, amount)
        {
        }

        public BlessScroll(Serial serial)
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