namespace Server.Items
{
    public class SpellweavingBook : Spellbook
    {
        [Constructable]
        public SpellweavingBook()
            : this((ulong)0)
        {
        }

        [Constructable]
        public SpellweavingBook(ulong content)
            : base(content, 0x2D50)
        {
            Hue = 0x8A2;
        }

        public SpellweavingBook(Serial serial)
            : base(serial)
        {
        }

        public override SpellbookType SpellbookType => SpellbookType.Arcanist;
        public override int BookOffset => 600;
        public override int BookCount => 16;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}