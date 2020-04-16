namespace Server.Items
{
    public class SpellTriggerScroll : SpellScroll
    {
        [Constructable]
        public SpellTriggerScroll()
            : this(1)
        {
        }

        [Constructable]
        public SpellTriggerScroll(int amount)
            : base(685, 0x2DA6, amount)
        {
        }

        public SpellTriggerScroll(Serial serial)
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