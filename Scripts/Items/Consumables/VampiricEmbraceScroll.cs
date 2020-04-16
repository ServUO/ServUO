namespace Server.Items
{
    public class VampiricEmbraceScroll : SpellScroll
    {
        [Constructable]
        public VampiricEmbraceScroll()
            : this(1)
        {
        }

        [Constructable]
        public VampiricEmbraceScroll(int amount)
            : base(112, 0x226C, amount)
        {
        }

        public VampiricEmbraceScroll(Serial serial)
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