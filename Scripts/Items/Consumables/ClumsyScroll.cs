namespace Server.Items
{
    public class ClumsyScroll : SpellScroll
    {
        [Constructable]
        public ClumsyScroll()
            : this(1)
        {
        }

        [Constructable]
        public ClumsyScroll(int amount)
            : base(0, 0x1F2E, amount)
        {
        }

        public ClumsyScroll(Serial serial)
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