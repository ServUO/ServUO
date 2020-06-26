namespace Server.Items
{
    public class CurseScroll : SpellScroll
    {
        [Constructable]
        public CurseScroll()
            : this(1)
        {
        }

        [Constructable]
        public CurseScroll(int amount)
            : base(26, 0x1F47, amount)
        {
        }

        public CurseScroll(Serial serial)
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