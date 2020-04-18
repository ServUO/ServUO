namespace Server.Items
{
    public class ResurrectionScroll : SpellScroll
    {
        [Constructable]
        public ResurrectionScroll()
            : this(1)
        {
        }

        [Constructable]
        public ResurrectionScroll(int amount)
            : base(58, 0x1F67, amount)
        {
        }

        public ResurrectionScroll(Serial serial)
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