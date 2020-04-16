namespace Server.Items
{
    public class MassSleepScroll : SpellScroll
    {
        [Constructable]
        public MassSleepScroll()
            : this(1)
        {
        }

        [Constructable]
        public MassSleepScroll(int amount)
            : base(686, 0x2DA7, amount)
        {
        }

        public MassSleepScroll(Serial serial)
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