using System;

namespace Server.Items
{
    public class MysticBook : Spellbook
    {
        [Constructable]
        public MysticBook()
            : this((ulong)0)
        {
        }

        [Constructable]
        public MysticBook(ulong content)
            : base(content, 0x2D9D)
        {
            this.Layer = Layer.OneHanded;
        }

        public MysticBook(Serial serial)
            : base(serial)
        {
        }

        public override SpellbookType SpellbookType
        {
            get
            {
                return SpellbookType.Mystic;
            }
        }
        public override int BookOffset
        {
            get
            {
                return 677;
            }
        }
        public override int BookCount
        {
            get
            {
                return 16;
            }
        }
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