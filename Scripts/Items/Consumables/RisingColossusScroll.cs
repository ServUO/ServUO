namespace Server.Items
{
    public class RisingColossusScroll : SpellScroll
    {
        [Constructable]
        public RisingColossusScroll()
            : this(1)
        {
        }

        [Constructable]
        public RisingColossusScroll(int amount)
            : base(692, 0x2DAD, amount)
        {
        }

        public RisingColossusScroll(Serial serial)
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