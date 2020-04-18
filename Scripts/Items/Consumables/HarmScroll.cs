namespace Server.Items
{
    public class HarmScroll : SpellScroll
    {
        [Constructable]
        public HarmScroll()
            : this(1)
        {
        }

        [Constructable]
        public HarmScroll(int amount)
            : base(11, 0x1F38, amount)
        {
        }

        public HarmScroll(Serial serial)
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