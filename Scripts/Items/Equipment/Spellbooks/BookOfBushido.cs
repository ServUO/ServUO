namespace Server.Items
{
    public class BookOfBushido : Spellbook
    {
        [Constructable]
        public BookOfBushido()
            : this((ulong)0x3F)
        {
        }

        [Constructable]
        public BookOfBushido(ulong content)
            : base(content, 0x238C)
        {
        }

        public BookOfBushido(Serial serial)
            : base(serial)
        {
        }

        public override SpellbookType SpellbookType => SpellbookType.Samurai;
        public override int BookOffset => 400;
        public override int BookCount => 6;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}