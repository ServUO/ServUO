namespace Server.Items
{
    public class MarkScroll : SpellScroll
    {
        [Constructable]
        public MarkScroll()
            : this(1)
        {
        }

        [Constructable]
        public MarkScroll(int amount)
            : base(44, 0x1F59, amount)
        {
        }

        public MarkScroll(Serial serial)
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