namespace Server.Items
{
    public class EagleStrikeScroll : SpellScroll
    {
        [Constructable]
        public EagleStrikeScroll()
            : this(1)
        {
        }

        [Constructable]
        public EagleStrikeScroll(int amount)
            : base(682, 0x2DA3, amount)
        {
        }

        public EagleStrikeScroll(Serial serial)
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