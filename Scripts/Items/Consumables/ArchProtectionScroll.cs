namespace Server.Items
{
    public class ArchProtectionScroll : SpellScroll
    {
        [Constructable]
        public ArchProtectionScroll()
            : this(1)
        {
        }

        [Constructable]
        public ArchProtectionScroll(int amount)
            : base(25, 0x1F46, amount)
        {
        }

        public ArchProtectionScroll(Serial serial)
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