namespace Server.Items
{
    public class ExplosionScroll : SpellScroll
    {
        [Constructable]
        public ExplosionScroll()
            : this(1)
        {
        }

        [Constructable]
        public ExplosionScroll(int amount)
            : base(42, 0x1F57, amount)
        {
        }

        public ExplosionScroll(Serial serial)
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