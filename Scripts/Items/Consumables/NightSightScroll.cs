namespace Server.Items
{
    public class NightSightScroll : SpellScroll
    {
        [Constructable]
        public NightSightScroll()
            : this(1)
        {
        }

        [Constructable]
        public NightSightScroll(int amount)
            : base(5, 0x1F33, amount)
        {
        }

        public NightSightScroll(Serial ser)
            : base(ser)
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