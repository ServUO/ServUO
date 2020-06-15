namespace Server.Items
{
    public class TeleportScroll : SpellScroll
    {
        [Constructable]
        public TeleportScroll()
            : this(1)
        {
        }

        [Constructable]
        public TeleportScroll(int amount)
            : base(21, 0x1F42, amount)
        {
        }

        public TeleportScroll(Serial serial)
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