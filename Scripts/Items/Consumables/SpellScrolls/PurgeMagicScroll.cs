namespace Server.Items
{
    public class PurgeMagicScroll : SpellScroll
    {
        [Constructable]
        public PurgeMagicScroll()
            : this(1)
        {
        }

        [Constructable]
        public PurgeMagicScroll(int amount)
            : base(679, 0x2DA0, amount)
        {
        }

        public PurgeMagicScroll(Serial serial)
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